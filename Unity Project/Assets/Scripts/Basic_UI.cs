using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public class Basic_UI : MonoBehaviour
{

    //objects for showing particular stats
    public Transform healthUI;
    public Transform manaUI;
    //text component of objects
    private TextMeshProUGUI textMesh;

    void Update()
    {
        UpdateHealth(GameData.playerHealth);
        UpdateMana(GameData.playerMana);
    }

    void UpdateHealth(int health)
    {
        textMesh = healthUI.GetComponent<TextMeshProUGUI>();
        textMesh.text = "Health: " + health;
    }
    void UpdateMana(int mana)
    {
        textMesh = manaUI.GetComponent<TextMeshProUGUI>();
        textMesh.text = "Mana: " + mana;
    }
}


