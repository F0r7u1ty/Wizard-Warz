using UnityEngine;
using System.Collections;

// Place this on a GameObject in your scene (e.g., a "Game Manager" object)
public class GameDataRegenHelper : MonoBehaviour
{
    // Singleton pattern for easy static access
    public static GameDataRegenHelper Instance { get; private set; }

    // This must be non-static
    private Coroutine regenCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // Optional: Don't destroy this object when loading new scenes
            DontDestroyOnLoad(gameObject);
        }
    }

    // This public method is called from the static GameData class
    public void DoResetRegenDelay()
    {
        GameData.manaRegenAccumulator = 0f;
        if (regenCoroutine != null)
        {
            // Use the non-static StopCoroutine available on MonoBehaviour
            StopCoroutine(regenCoroutine); 
        }
        // Use the non-static StartCoroutine available on MonoBehaviour
        regenCoroutine = StartCoroutine(ManaRegenRoutine()); 
    }

    private IEnumerator ManaRegenRoutine()
    {
        // 1. Wait for the required delay after mana consumption
        yield return new WaitForSeconds(GameData.REGEN_DELAY);

        // 2. Start regenerating mana continuously
        while (GameData.playerMana < GameData.MAX_PLAYER_MANA)
        {
            // Access static variables and constants from GameData
            float manaToAccumulate = GameData.MANA_PER_SECOND * Time.deltaTime;
            GameData.manaRegenAccumulator += manaToAccumulate;

            if (GameData.manaRegenAccumulator >= 1f)
            {
                int manaToCharge = Mathf.FloorToInt(GameData.manaRegenAccumulator);
                GameData.ChargePlayerMana(manaToCharge);
                GameData.manaRegenAccumulator -= manaToCharge;
            }

            yield return null;
        }

        GameData.manaRegenAccumulator = 0f;
    }
}