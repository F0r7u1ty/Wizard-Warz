using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class GameData
{
    // primitives
    private const int MAX_PLAYER_HEALTH = 100;
    public static int playerHealth = MAX_PLAYER_HEALTH;
    private const int MAX_PLAYER_MANA = 100;
    public static int playerMana = MAX_PLAYER_MANA;

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
    // the return value of 0 can check for if we shouldn't proceed with an action due to limited mana
    public static int ExhaustPlayerMana(int amount)
    {
        if (playerMana >= amount)
        {
            playerMana -= amount;
            return playerMana;
        }
        else { return 0; }
    }
}
