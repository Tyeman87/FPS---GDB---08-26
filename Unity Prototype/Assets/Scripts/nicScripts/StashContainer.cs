using System;
using System.Collections.Generic;
using UnityEngine;

public class StashContainer : MonoBehaviour, IInteractable
{
    [SerializeField] private StashUI stashUI;
    [SerializeField] private GameObject stashPrompt;
    [SerializeField] private playerController player;

    [System.Serializable]
    public class StashedGun
    {
        
        public GunStats stats;
        public int currMag;
        public int currReserve;

        public StashedGun(GunStats gun)
        {
            stats = gun;
            currMag = gun.magSize;
            currReserve = gun.maxReserve;
        }
    }

    [Header("Stored Guns")]
    [SerializeField] List<StashedGun> storedGuns = new List<StashedGun>();

    public void Interact()
    {
        Debug.Log("Stash Interact called!");
        stashPrompt.SetActive(true);
        waitingForInput = true;
    }

    public List<StashedGun> GetStoredGuns()
    {
        return storedGuns;
    }

    public void RemoveGun(StashedGun gun)
    {
        storedGuns.Remove(gun);
    }

    public void StoreGun(GunStats gun, int currMag, int currReserve)
    {
        StashedGun newGun = new StashedGun(gun);
        newGun.currMag = currMag;
        newGun.currReserve = currReserve;

        storedGuns.Add(newGun);

        Debug.Log($"Stored {gun.name} | Ammo: {currMag} / {currReserve}");
    }
    private bool waitingForInput = false;
    private void Update()
    {
        if (!stashPrompt.activeSelf)
        {
            return;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            waitingForInput = false;
        }

        if (waitingForInput)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            stashUI.OpenStash(this);
            stashPrompt.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            playerController.GunAmmoData gun = player.GetCurrentGun();

            if (gun != null)
            {
                StoreGun(gun.stats, gun.currMag, gun.currReserve);
                player.RemoveCurrentGun();
            }

            stashPrompt.SetActive(false);
        }
    }
}
