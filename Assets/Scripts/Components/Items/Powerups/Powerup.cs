using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Powerup
{
    #region Variables
    public enum PrimaryEffect
    {
        None,
        Health,
        Hunger,
        Thirst,
        Stamina,
        StaminaRegen
    }

    public enum SecondEffect
    {
        None,
        Health,
        Hunger,
        Thirst,
        Stamina,
        StaminaRegen
    }

    private PrimaryEffect primaryEffect;
    private SecondEffect secondEffect;
    private float primaryStatChangeAmount;
    private float secondStatChangeAmount;
    private float maxPowerupDuration;
    private bool isStackable;
    private bool isPrimaryPermanent;
    private bool isSecondPermanent;
    private bool isPrimaryPercentChange;
    private bool isSecondPercentChange;
    private bool isPrimaryEffectAlwaysApplied;
    private bool isSecondEffectAlwaysApplied;
    // Will track stacking effects so Remove() is called for each stack applied
    private int timesPrimaryEffectApplied;
    private int timesSecondEffectApplied;
    private float currentPowerupDuration;

    #endregion Variables

    #region ApplyPrimaryEffect
    // This method will be called when the item is used
    public void ApplyPrimaryEffect(PowerupManager target)
    {
        currentPowerupDuration = maxPowerupDuration;

        if (primaryEffect == PrimaryEffect.None)
        {
            // Do nothing
        }
        else if (primaryEffect == PrimaryEffect.Health)
        {
            
        }
        else if (primaryEffect == PrimaryEffect.Hunger)
        {
            
        }
        else if (primaryEffect == PrimaryEffect.Thirst)
        {
            
        }
        else if (primaryEffect == PrimaryEffect.Stamina)
        {
            
        }
        else if (primaryEffect == PrimaryEffect.StaminaRegen)
        {
            
        }
    }

    #endregion ApplyPrimaryEffect

    #region ApplySecondEffect
    public void ApplySecondEffect(PowerupManager target)
    {
        currentPowerupDuration = maxPowerupDuration;

        if (secondEffect == SecondEffect.None)
        {
            // Do nothing
        }
        else if (secondEffect == SecondEffect.Health)
        {

        }
        else if (secondEffect == SecondEffect.Hunger)
        {
            
        }
        else if (secondEffect == SecondEffect.Thirst)
        {
            
        }
        else if (secondEffect == SecondEffect.Stamina)
        {
            
        }
        else if (secondEffect == SecondEffect.StaminaRegen)
        {
            
        }
    }

    #endregion ApplySecondEffect

    #region RemovePrimaryEffect
    // This method will be called when the item's effect wears off and it wasn't permanent
    public void RemovePrimaryEffect(PowerupManager target)
    {
        if (primaryEffect == PrimaryEffect.None)
        {
            // Do nothing
        }
        else if (primaryEffect == PrimaryEffect.Health)
        {
            
        }
        else if (primaryEffect == PrimaryEffect.Hunger)
        {
            
        }
        else if (primaryEffect == PrimaryEffect.Thirst)
        {
            
        }
        else if (primaryEffect == PrimaryEffect.Stamina)
        {
            
        }
        else if (primaryEffect == PrimaryEffect.StaminaRegen)
        {
            
        }
    }

    #endregion RemovePrimaryEffect

    #region RemoveSecondEffect
    public void RemoveSecondEffect(PowerupManager target)
    {
        if (secondEffect == SecondEffect.None)
        {
            // Do nothing
        }
        else if (secondEffect == SecondEffect.Health)
        {
            
        }
        else if (secondEffect == SecondEffect.Hunger)
        {
            
        }
        else if (secondEffect == SecondEffect.Thirst)
        {
            
        }
        else if (secondEffect == SecondEffect.Stamina)
        {
            
        }
        else if (secondEffect == SecondEffect.StaminaRegen)
        {
            
        }
    }

    #endregion RemoveSecondEffect

    #region GetSet
    public float GetPrimaryStatChangeAmount()
    {
        return primaryStatChangeAmount;
    }

    public void SetPrimaryStatChangeAmount(float newAmount)
    {
        if (newAmount <= 0)
        {
            // Do Nothing
        }
        else
        {
            primaryStatChangeAmount = newAmount;
        }
    }

    public float GetSecondStatChangeAmount()
    {
        return secondStatChangeAmount;
    }

    public void SetSecondStatChangeAmount(float newAmount)
    {
        secondStatChangeAmount = newAmount;
    }

    public float GetMaxPowerupDuration()
    {
        return maxPowerupDuration;
    }

    public void SetMaxPowerupDuration(float newDuration)
    {
        maxPowerupDuration = newDuration;
    }

    public float GetCurrentPowerupDuration()
    {
        return currentPowerupDuration;
    }

    public void SetCurrentPowerupDuration(float newDuration)
    {
        currentPowerupDuration = newDuration;
    }

    public bool GetIsStackable()
    {
        return isStackable;
    }

    public void SetIsStackable(bool newState)
    {
        isStackable = newState;
    }

    public bool GetIsPrimaryPermanent()
    {
        return isPrimaryPermanent;
    }

    public void SetIsPrimaryPermanent(bool newState)
    {
        isPrimaryPermanent = newState;
    }

    public bool GetIsSecondPermanent()
    {
        return isSecondPermanent;
    }

    public void SetIsSecondPermanent(bool newState)
    {
        isSecondPermanent = newState;
    }

    public PrimaryEffect GetPrimaryEffect()
    {
        return primaryEffect;
    }

    public void SetPrimaryEffect(PrimaryEffect newEffect)
    {
        primaryEffect = newEffect;
    }

    public SecondEffect GetSecondEffect()
    {
        return secondEffect;
    }

    public void SetSecondEffect(SecondEffect newEffect)
    {
        secondEffect = newEffect;
    }

    public bool GetIsPrimaryPercentChange()
    {
        return isPrimaryPercentChange;
    }

    public void SetIsPrimaryPercentChange(bool newState)
    {
        isPrimaryPercentChange = newState;
    }

    public bool GetIsSecondPercentChange()
    {
        return isSecondPercentChange;
    }

    public void SetIsSecondPercentChange(bool newState)
    {
        isSecondPercentChange = newState;
    }

    public bool GetIsPrimaryEffectAlwaysApplied()
    {
        return isPrimaryEffectAlwaysApplied;
    }

    public void SetIsPrimaryEffectAlwaysApplied(bool newState)
    {
        isPrimaryEffectAlwaysApplied = newState;
    }

    public bool GetIsSecondEffectAlwaysApplied()
    {
        return isSecondEffectAlwaysApplied;
    }

    public void SetIsSecondEffectAlwaysApplied(bool newState)
    {
        isSecondEffectAlwaysApplied = newState;
    }

    public int GetTimesPrimaryEffectApplied()
    {
        return timesPrimaryEffectApplied;
    }

    public void SetTimesPrimaryEffectApplied(int applied)
    {
        timesPrimaryEffectApplied = applied;
    }

    public int GetTimesSecondEffectApplied()
    {
        return timesSecondEffectApplied;
    }

    public void SetTimesSecondEffectApplied(int applied)
    {
        timesSecondEffectApplied = applied;
    }

    #endregion GetSet
}