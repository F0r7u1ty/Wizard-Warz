using UnityEngine;
using System.Collections; // Required for Coroutines

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

    [Header("Teleport Settings")]
    public Transform playerCamera;
    public float teleportDistance = 10f; // How far to teleport
    // *** NEW: Public variable for the travel time (set in Inspector) ***
    public float teleportDuration = 0.25f; // Duration of the teleport in seconds

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (playerCamera == null)
        {
            Debug.LogError("PlayerCamera Transform is not assigned to the PlayerMotor script in the Inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        if (lerpCrouch)
        {
            // ... (Crouch logic remains the same)
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
    }

    // ... (ProcessMove and Jump functions remain the same)

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
        }
    }

    public void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }

    // *** MODIFIED: Start the Coroutine instead of instant movement ***
    public void Teleport()
    {
        if (playerCamera == null) return;

        Vector3 direction = playerCamera.forward;
        Vector3 rayDirection = direction.normalized;

        // 1. Raycast Check
        RaycastHit hit;
        if (Physics.Raycast(transform.position, rayDirection, out hit, teleportDistance))
        {
            if (hit.collider.CompareTag("boundaries"))
            {
                Debug.Log("Teleport blocked by 'boundaries' wall.");
                return;
            }
        }
        Debug.DrawRay(transform.position, rayDirection * teleportDistance, Color.red);

        //note: the function still exhausts mana, doing again after the if statement will double the exhaust
        if (!GameData.ExhaustPlayerMana(20)) {return;}
        // 2. Start the Coroutine for movement over time
        StartCoroutine(TeleportSequence(transform.position + direction * teleportDistance));
    }

    // *** NEW: Coroutine for smooth movement ***
    private IEnumerator TeleportSequence(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        // Ensure the CharacterController is disabled during the move to faze through walls
        controller.enabled = false;

        // Loop until the duration is met
        while (elapsedTime < teleportDuration)
        {
            // Update time
            elapsedTime += Time.deltaTime;

            // Calculate interpolation value (0 to 1)
            float t = elapsedTime / teleportDuration;

            // Optional: Smooth the movement using a curve like Quadratic (t * t) or Cubic (t * t * t)
            t = t * t;

            // Interpolate the position (only X and Z, keeping the current Y to avoid phasing through floor)
            float newX = Mathf.Lerp(startPosition.x, targetPosition.x, t);
            float newZ = Mathf.Lerp(startPosition.z, targetPosition.z, t);

            transform.position = new Vector3(newX, startPosition.y, newZ);

            // Wait until the next frame
            yield return null;
        }

        // Finalize the position
        transform.position = new Vector3(targetPosition.x, startPosition.y, targetPosition.z);

        // Re-enable the CharacterController collision
        controller.enabled = true;

        Debug.Log("Teleport sequence complete. Final position: " + transform.position);
    }
}