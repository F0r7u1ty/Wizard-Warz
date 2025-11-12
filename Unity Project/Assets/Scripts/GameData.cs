using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Unity.VisualScripting; // Required for Coroutines

public static class GameData
{
    // primitives
    public const int MAX_PLAYER_HEALTH = 100;
    public static int playerHealth = MAX_PLAYER_HEALTH;
    public const int MAX_PLAYER_MANA = 100;
    public static int playerMana = MAX_PLAYER_MANA;
    public static int numJumps = 0;
    private float manaRegenAccumulator = 0f;

    //health functions
    public static void DamagePlayerHealth(int amount)
    {
        playerHealth -= amount;
    }
    public static void HealPlayerHealth(int amount)
    {
        if (playerHealth < MAX_PLAYER_HEALTH)
        {
            playerHealth += amount;
            //if we exceed the maximum we set to maximum
            if (playerHealth > MAX_PLAYER_HEALTH) { playerHealth = MAX_PLAYER_HEALTH; }
        }
    }
    //mana functions
    public static void ChargePlayerMana(int amount)
    {
        if (playerMana < MAX_PLAYER_MANA)
        {
            playerMana += amount;
            //if we exceed the maximum we set to maximum
            if (playerMana > MAX_PLAYER_MANA) { playerMana = MAX_PLAYER_MANA; }
        }
    }
    // the return value of false can check for if we shouldn't proceed with an action due to limited mana
    // the return value of true can check for if we should proceed with an action
    public static Boolean ExhaustPlayerMana(int amount)
    {
        if (playerMana >= amount)
        {
            playerMana -= amount;
            ResetRegenDelay();
            return true;
        }
        else { return false; }
    }

    // --- Mana Regen
    public void ResetRegenDelay()
    {
        manaRegenAccumulator = 0f;
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
        }
        regenCoroutine = StartCoroutine(ManaRegenRoutine());
    }

    private IEnumerator ManaRegenRoutine()
    {
        // 1. Wait for the required delay after mana consumption
        yield return new WaitForSeconds(REGEN_DELAY);

        // 2. Start regenerating mana continuously
        while (playerMana < MAX_PLAYER_MANA)
        {
            float manaToAccumulate = MANA_PER_SECOND * Time.deltaTime;
            manaRegenAccumulator += manaToAccumulate;

            if (manaRegenAccumulator >= 1f)
            {
                int manaToCharge = Mathf.FloorToInt(manaRegenAccumulator);
                GameData.ChargePlayerMana(manaToCharge);
                manaRegenAccumulator -= manaToCharge;
            }

            yield return null;
        }

        manaRegenAccumulator = 0f;
    }
}
