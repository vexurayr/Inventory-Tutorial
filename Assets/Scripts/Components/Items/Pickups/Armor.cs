using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Powerup;

public class Armor : InventoryItem
{
    #region Variables
    [Tooltip("This number will be a percentage that reduces incoming damage.")]
        [Range(0, 1)][SerializeField] private float defenseProtection;
    [Tooltip("Flat increase to the range of cold temperatures that are safe for the player.")]
        [SerializeField] private float coldTemperatureProtection;
    [Tooltip("Flat increase to the range of hot temperatures that are safe for the player.")]
        [SerializeField] private float hotTemperatureProtection;

    #endregion Variables

    // Slightly different for armor, only using PowerupManager for player reference
    // Primary Action gives player the armor's buffs
    #region PrimaryAction
    public override bool PrimaryAction(PowerupManager powerupManager)
    {
        if (powerupManager == null)
        {
            return false;
        }

        return true;
    }

    #endregion PrimaryAction

    // Second Action removes the armor's buffs
    #region SecondAction
    public override bool SecondaryAction(PowerupManager powerupManager)
    {
        if (powerupManager == null)
        {
            return false;
        }

        return false;
    }

    #endregion SecondAction
}