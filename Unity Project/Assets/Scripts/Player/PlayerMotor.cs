using UnityEngine;
using System.Collections;
using Unity.VisualScripting; // Required for Coroutines

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    public float speed = 5f;
    public float gravity = -9.8f;
    private bool isGrounded;
    public float jumpHeight = 1.3f;
    private bool lerpCrouch;
    private bool crouching;
    private float crouchTimer;
    private float crouchSpeed = 11f;

    private const float REGEN_DELAY = 0.6f; // Time after use before regen starts (0.6s)
    private const float MANA_PER_SECOND = 6f; // Regeneration rate (6 mana per second)
    private Coroutine regenCoroutine;
    private float manaRegenAccumulator = 0f;

    [Header("Teleport Settings")]
    public Transform playerCamera;
    public float teleportDistance = 10f; // How far to teleport
    public float teleportDuration = 0.25f; // Duration of the teleport in seconds

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera == null)
        {
            Debug.LogError("PlayerCamera Transform is not assigned to the TestPlayerMotor script in the Inspector.");
        }
        //lock mouse to camera
        Cursor.lockState = CursorLockMode.Locked;

        // Hide the cursor
        Cursor.visible = false;
        // Start the continuous mana regeneration routine
        //regenCoroutine = StartCoroutine(GameData.ResetRegenDelay());
        GameData.ResetRegenDelay();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p *= p;
            if (crouching)
            {
                controller.height = Mathf.Lerp(controller.height, 1, p);
            }
            else
            {
                controller.height = Mathf.Lerp(controller.height, 2, p);
            }
            if (p > 1)
            {
                lerpCrouch = false;
                crouchTimer = 0f;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void ProcessMove(Vector2 input)
    {
        if (!controller.enabled)
        {
            // If the controller is disabled (e.g., during a teleport),
            // we exit early and do nothing.
            return;
        }
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        float currentSpeed = crouching ? crouchSpeed : speed;
        controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            GameData.numJumps = 1;
        }
        else if (GameData.playerMana >= 3 && GameData.numJumps == 1)
        {
            GameData.ExhaustPlayerMana(3);
            //GameData.playerMana -= 3;
            GameData.numJumps = 2;
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity); //can potentially change this so the 2nd jump is more or less potent than normal
            //GameData.ResetRegenDelay();
        }
    }

    public void StartCrouch()
    {
        if(!crouching)
        {
            crouching = true;
            lerpCrouch = true;
            crouchTimer = 0;
        }
    }

    public void EndCrouch()
    {
        if (crouching)
        {
            crouching = false;
            lerpCrouch = true;
            crouchTimer = 0;
        }
    }

    public void Teleport(Vector2 input)
    {
        // 1. Check for playerCamera
        if (playerCamera == null) return;

        // 2. Calculate the intended direction of travel based on camera and input
        Vector3 forward = playerCamera.forward;
        Vector3 right = playerCamera.right;

        // Zero out the Y component to keep movement purely horizontal
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 direction;

        if (input.magnitude < 0.1f)
        {
            // If NO directional input (magnitude is near zero), default to forward
            direction = forward;
        }
        else
        {
            // Otherwise, use the standard calculated direction based on input
            // Calculate the total horizontal direction vector
            direction = (forward * input.y + right * input.x).normalized;
        }

        // Calculate the intended target position (full distance)
        Vector3 intendedTarget = transform.position + direction * teleportDistance;
        Vector3 actualTarget = intendedTarget;

        // 3. Raycast Check & Boundary Adjustment (The rest of the method remains the same)
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, teleportDistance, ~0, QueryTriggerInteraction.Ignore);
        float closestBoundaryDistance = teleportDistance + 1f;
        RaycastHit boundaryHit = default;
        bool hitBoundary = false;

        // Iterate over all hits to find the closest object tagged "boundaries"
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("boundaries"))
            {
                if (hit.distance < closestBoundaryDistance)
                {
                    closestBoundaryDistance = hit.distance;
                    boundaryHit = hit;
                    hitBoundary = true;
                }
            }
        }

        // Adjust Target based on Boundary Hit
        if (hitBoundary)
        {
            // We pull back by half the controller's radius to ensure the CharacterController doesn't start inside the wall.
            actualTarget = boundaryHit.point - direction * (controller.radius * 0.5f);
            //Debug.Log("Teleport hit 'boundaries' at distance: " + closestBoundaryDistance + ". Player will stop and push against the wall.");
        }
        // If no boundary was hit, actualTarget remains the full intendedTarget, allowing phasing through other objects.

        Debug.DrawRay(transform.position, direction * teleportDistance, Color.red, 2f);


        // 4. Mana Check and Cost
        if (!GameData.ExhaustPlayerMana(10)) { return; }
        //GameData.ResetRegenDelay();

        // 5. Start the Coroutine for movement over time
        StartCoroutine(TeleportSequence(actualTarget));
    }

    //coroutine
    public IEnumerator TeleportSequence(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        // Disable the CharacterController during the teleport to allow phasing to the target
        controller.enabled = false;

        // Store the starting Y position to keep vertical movement consistent during the dash, don't let people fly away
        float startY = startPosition.y;

        // Loop until the duration is met
        while (elapsedTime < teleportDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / teleportDuration;

            // Optional: Smooth the movement using a curve (Quadratic: t*t)
            t = t * t;

            // Interpolate the horizontal position (X and Z only)
            float newX = Mathf.Lerp(startPosition.x, targetPosition.x, t);
            float newZ = Mathf.Lerp(startPosition.z, targetPosition.z, t);

            // Set the new position, keeping the initial Y
            transform.position = new Vector3(newX, startY, newZ);

            yield return null;
        }

        // Finalize the position
        transform.position = new Vector3(targetPosition.x, startY, targetPosition.z);

        // Re-enable the CharacterController collision. 
        // If the final position is near a wall, this immediately creates the "push against" effect.
        controller.enabled = true;

        //Debug.Log("Teleport sequence complete. Final position: " + transform.position);
    }

}