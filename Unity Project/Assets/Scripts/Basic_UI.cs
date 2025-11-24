using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

public class Basic_UI : MonoBehaviour
{

    //objects for showing particular stats
    public Transform healthUI_text;
    public Transform manaUI_text;
    public Image healthUI_barbar;
    public Image manaUI_barbar;
    //text component of objects
    private TextMeshProUGUI textMesh;

    void Update()
    {
        UpdateHealth(GameData.playerHealth);
        UpdateMana(GameData.playerMana);
    }

    void UpdateHealth(int health)
    {
        textMesh = healthUI_text.GetComponent<TextMeshProUGUI>();
        textMesh.text = "Health: " + health;
        healthUI_barbar.fillAmount = (float)health / GameData.MAX_PLAYER_HEALTH;
    }
    void UpdateMana(int mana)
    {
        textMesh = manaUI_text.GetComponent<TextMeshProUGUI>();
        textMesh.text = "Mana: " + mana;
        manaUI_barbar.fillAmount = (float)mana / GameData.MAX_PLAYER_MANA;
    }
}


