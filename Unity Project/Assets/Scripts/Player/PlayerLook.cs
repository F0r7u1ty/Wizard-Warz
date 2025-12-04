using Unity.VisualScripting;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;

    // Inspector-exposed fallback defaults (only used if SettingsManager is missing)
    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    // Scale to convert slider range (0.1 - 5) into the feel you used previously (~30).
    // Tweak this number to taste. 30 will make slider=1 behave like your old 30 value.
    [Tooltip("Multiply slider value by this to get the runtime sensitivity used by look code.")]
    public float sensitivityScale = 30f;

    public bool slugCam = false;

    // runtime values actually used for rotation (after scaling)
    float runtimeX;
    float runtimeY;

    void Start()
    {
        // Initialize runtime values using SettingsManager if present
        if (SettingsManager.Instance != null)
        {
            // slider values are 0.1-5; scale them to match previous feel
            runtimeX = SettingsManager.Instance.xSensitivity * sensitivityScale;
            runtimeY = SettingsManager.Instance.ySensitivity * sensitivityScale;

            SettingsManager.Instance.OnSensitivityChanged += OnSensitivityChanged;
        }
        else
        {
            runtimeX = xSensitivity;
            runtimeY = ySensitivity;
        }
    }

    void OnDestroy()
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.OnSensitivityChanged -= OnSensitivityChanged;
    }

    void OnSensitivityChanged(float newX, float newY)
    {
        // Map slider values into runtime used by your look code
        runtimeX = newX * sensitivityScale;
        runtimeY = newY * sensitivityScale;
    }

    public void ProcessLook(Vector2 input)
    {
        if (slugCam) { input *= 0.1f; }

        float mouseX = input.x;
        float mouseY = input.y;

        // defensive fallback if something went wrong with initialization/subscription
        if (SettingsManager.Instance != null)
        {
            // if runtime values are zero (unlikely), pull directly
            if (runtimeX == 0f) runtimeX = SettingsManager.Instance.xSensitivity * sensitivityScale;
            if (runtimeY == 0f) runtimeY = SettingsManager.Instance.ySensitivity * sensitivityScale;
        }

        xRotation -= (mouseY * Time.deltaTime) * runtimeY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * runtimeX);
    }
}
