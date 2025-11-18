using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    // Assign these GameObjects in the Inspector
    public GameObject wizardStaff;
    public GameObject wizardWand;

    private void Start()
    {
        // 1. Set the default weapon on start
        SelectWeapon(0);
    }

    private void Update()
    {
        // 2. Handle numerical key inputs (1 and 2)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectWeapon(0); // 0 corresponds to the Staff
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectWeapon(1); // 1 corresponds to the Wand
        }

        // 3. Handle mouse scroll wheel input
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

        if (scrollDelta > 0f) // Scroll up/forward
        {
            // For a simple two-weapon toggle, you can just switch to the other one
            if (wizardStaff.activeSelf)
            {
                SelectWeapon(1); // Staff active -> switch to Wand
            }
            else
            {
                SelectWeapon(0); // Wand active -> switch to Staff
            }
        }
        else if (scrollDelta < 0f) // Scroll down/backward
        {
            // You can implement this to scroll in the opposite direction
            if (wizardStaff.activeSelf)
            {
                SelectWeapon(1); // Staff active -> switch to Wand
            }
            else
            {
                SelectWeapon(0); // Wand active -> switch to Staff
            }
        }
    }

    // A helper function to manage which weapon is visible
    void SelectWeapon(int weaponIndex)
    {
        // 0 = Staff, 1 = Wand
        if (weaponIndex == 0)
        {
            // Enable the Staff and Disable the Wand
            wizardStaff.SetActive(true);
            wizardWand.SetActive(false);
        }
        else if (weaponIndex == 1)
        {
            // Enable the Wand and Disable the Staff
            wizardStaff.SetActive(false);
            wizardWand.SetActive(true);
        }

        // Optional: Add code here to notify other scripts that the weapon has changed.
    }
}