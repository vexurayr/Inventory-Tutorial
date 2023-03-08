using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This will go on any object capable of using powerups
public class PowerupManager : MonoBehaviour
{
    #region Variables
    // All active powerups in powerup manager
    private List<Powerup> powerups;
    private List<Powerup> removedPowerupQueue;

    #endregion Variables

    #region MonoBehaviours
    private void Start()
    {
        // Makes list empty, not null
        powerups = new List<Powerup>();
        removedPowerupQueue = new List<Powerup>();
    }

    private void Update()
    {
        if (powerups.Count > 0)
        {
            DecrementPowerupTimers();
        }
    }

    private void LateUpdate()
    {
        if (removedPowerupQueue.Count > 0)
        {
            ApplyRemovePowerupsQueue();
        }
    }

    #endregion MonoBehaviours

    #region AddPowerup
    // This method activates the powerup's effects and adds it to a list of active powerups
    public void Add(Powerup powerup)
    {
        bool isAlreadyApplied = false;
        int indexPowerupExists = 0;
        int index = 0;

        // See if the incoming powerup is already in the powerup manager's list
        foreach (Powerup power in powerups)
        {
            if (powerup.GetPrimaryEffect() == power.GetPrimaryEffect() &&
                powerup.GetSecondEffect() == power.GetSecondEffect())
            {
                isAlreadyApplied = true;
                indexPowerupExists = index;
            }
            index++;
        }

        // Apply powerup if it's not already in this list
        if (!isAlreadyApplied)
        {
            powerup.ApplyPrimaryEffect(this);
            powerup.ApplySecondEffect(this);
            
            powerup.SetTimesPrimaryEffectApplied(powerup.GetTimesPrimaryEffectApplied() + 1);
            powerup.SetTimesSecondEffectApplied(powerup.GetTimesSecondEffectApplied() + 1);
            // Add powerup to list
            powerups.Add(powerup);
        }
        // If it is in the list and can't stack, just reset its timer
        else if (!powerup.GetIsStackable())
        {
            powerups[indexPowerupExists].SetCurrentPowerupDuration(powerup.GetMaxPowerupDuration());

            if (powerup.GetIsPrimaryEffectAlwaysApplied())
            {
                powerup.ApplyPrimaryEffect(this);
                powerups[indexPowerupExists].SetTimesPrimaryEffectApplied(powerup.GetTimesPrimaryEffectApplied() + 1);
            }
            if (powerup.GetIsSecondEffectAlwaysApplied())
            {
                powerup.ApplySecondEffect(this);
                powerups[indexPowerupExists].SetTimesSecondEffectApplied(powerup.GetTimesSecondEffectApplied() + 1);
            }
        }
        // It is in the list and can stack, also reset the timer
        else
        {
            powerup.ApplyPrimaryEffect(this);
            powerup.ApplySecondEffect(this);

            powerups[indexPowerupExists].SetCurrentPowerupDuration(powerup.GetMaxPowerupDuration());
            powerups[indexPowerupExists].SetTimesPrimaryEffectApplied(powerup.GetTimesPrimaryEffectApplied() + 1);
            powerups[indexPowerupExists].SetTimesSecondEffectApplied(powerup.GetTimesSecondEffectApplied() + 1);
        }
    }

    #endregion AddPowerup

    #region RemovePowerup
    // This method will remove a powerup's effects from the object
    public void Remove(Powerup powerup)
    {
        if (!powerup.GetIsPrimaryPermanent())
        {
            // For each time the effect was applied, remove its stat change
            for (int i = powerup.GetTimesPrimaryEffectApplied(); i > 0; i--)
            {
                powerup.RemovePrimaryEffect(this);
            }
            powerup.SetTimesPrimaryEffectApplied(0);
        }
        
        if (!powerup.GetIsSecondPermanent())
        {
            for (int i = powerup.GetTimesSecondEffectApplied(); i > 0; i--)
            {
                powerup.RemoveSecondEffect(this);
            }
            powerup.SetTimesSecondEffectApplied(0);
        }

        // Even permanent powerups are removed, but their effect is not
        removedPowerupQueue.Add(powerup);
    }

    #endregion RemovePowerup

    #region TimerFunctions
    public void DecrementPowerupTimers()
    {
        foreach (Powerup powerup in powerups)
        {
            // Decrease the time it has remaining
            float remainingDuration = powerup.GetCurrentPowerupDuration();
            remainingDuration -= Time.deltaTime;
            powerup.SetCurrentPowerupDuration(remainingDuration);

            // Remove powerup if time is up
            if (powerup.GetCurrentPowerupDuration() <= 0)
            {
                Remove(powerup);
            }
        }
    }

    private void ApplyRemovePowerupsQueue()
    {
        foreach (Powerup powerup in removedPowerupQueue)
        {
            powerups.Remove(powerup);
        }
        // Clear list for next time powerups need to be removed
        removedPowerupQueue.Clear();
    }

    #endregion TimerFunctions
}