using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;
    private PlayerMotor motor; // used to be private PlayerMotor motor;
    private PlayerLook look;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        motor = GetComponent<PlayerMotor>(); // used to be: motor = GetComponent<PlayerMotor>();
        //anytime onFoot.jump performed, use callback context to call motor.Jump()
        onFoot.Jump.performed += ctx => motor.Jump();
        look = GetComponent<PlayerLook>();
        onFoot.Crouch.performed += ctx => motor.Crouch();
        onFoot.Teleport.performed += ctx => motor.Teleport(onFoot.Movement.ReadValue<Vector2>());
        // used to be: onFoot.Teleport.performed += ctx => motor.Teleport();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //tell playerMotor to move using value from input action
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
}