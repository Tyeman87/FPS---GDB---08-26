using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class LoadoutManager : MonoBehaviour
{
    [Header("Selected Guns")] [SerializeField] private GunStats selectedGun1;
    [SerializeField] private GunStats selectedGun2;
    [Header("Selected Grenades")] [SerializeField] private GrenadeItemStats selectedGrenade1;
    [SerializeField] private GrenadeItemStats selectedGrenade2;
    [Header("Gun Selection Buttons")] [SerializeField] private Button gun1PreviousButton;
    [SerializeField] private Button gun1NextButton;
    [SerializeField] private Button gun2PreviousButton;
    [SerializeField] private Button gun2NextButton;
    [Header("Grenade Selection Buttons")] [SerializeField] private Button grenade1PreviousButton;
    [SerializeField] private Button grenade1NextButton;
    [SerializeField] private Button grenade2PreviousButton;
    [SerializeField] private Button grenade2NextButton;
    [Header("Loadout Presets")] [SerializeField] private Button preset1Button;
    [SerializeField] private Button preset2Button;
    [SerializeField] private Button preset3Button;
    [Header("UI")] [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [Header("Gun Previews")] [SerializeField] private Image gun1PreviewImage;
    [SerializeField] private Image gun2PreviewImage;
    [Header("Grenade Previews")] [SerializeField] private Image grenade1PreviewImage;
    [SerializeField] private Image grenade2PreviewImage;
    [Header("Upgrade Buttons")] [SerializeField] private Button damageUpgradeButton;
    [SerializeField] private Button shootRateUpgradeButton;
    [SerializeField] private Button magSizeUpgradeButton;
    [Header("Upgrade Preview")] [SerializeField] private TMP_Text postUpgradeDescriptionText;
    [Header("Loadout")] [SerializeField] private playerController player;
    [SerializeField] private Button setLoadoutButton;
    private List<GunStats> unlockedGuns = new List<GunStats>();
    private List<GrenadeItemStats> unlockedGrenades = new List<GrenadeItemStats>();
    private GunStats currentlySelectedGun;
    private int gun1Index = -1;
    private int gun2Index = -1;
    private int grenade1Index = -1;
    private int grenade2Index = -1;
    private int activePresetIndex;
    private void Start()
    {
        RefreshUnlockedItems();
        if (SaveDataManager.Instance != null)
        {
            activePresetIndex = SaveDataManager.Instance.GetActiveLoadoutPresetIndex();
        }

        LoadPreset(activePresetIndex);
        UpdateLoadoutButtons();
    }

    public void SelectPreset(int presetIndex)
    {
        activePresetIndex = Mathf.Clamp(presetIndex, 0, 2);
        if (SaveDataManager.Instance != null)
        {
            SaveDataManager.Instance.SetActiveLoadoutPresetIndex(activePresetIndex);
        }

        LoadPreset(activePresetIndex);
        Debug.Log("Selected Preset " + (activePresetIndex + 1));
    }

    private void LoadPreset(int presetIndex)
    {
        RefreshUnlockedItems();
        if (SaveDataManager.Instance == null)
        {
            return;
        }

        SaveDataManager.LoadoutPresetSaveData preset = SaveDataManager.Instance.GetLoadoutPreset(presetIndex);
        selectedGun1 = null;
        selectedGun2 = null;
        selectedGrenade1 = null;
        selectedGrenade2 = null;
        if (preset != null && !string.IsNullOrEmpty(preset.gun1ID))
        {
            GunStats gun = SaveDataManager.Instance.GetGunByID(preset.gun1ID);
            if (gun != null && unlockedGuns.Contains(gun))
            {
                selectedGun1 = gun;
            }
        }

        if (preset != null && !string.IsNullOrEmpty(preset.gun2ID))
        {
            GunStats gun = SaveDataManager.Instance.GetGunByID(preset.gun2ID);
            if (gun != null && gun != selectedGun1 && unlockedGuns.Contains(gun))
            {
                selectedGun2 = gun;
            }
        }

        if (preset != null && !string.IsNullOrEmpty(preset.grenade1ID))
        {
            GrenadeItemStats grenade = SaveDataManager.Instance.GetGrenadeByID(preset.grenade1ID);
            if (grenade != null && unlockedGrenades.Contains(grenade))
            {
                selectedGrenade1 = grenade;
            }
        }

        if (preset != null && !string.IsNullOrEmpty(preset.grenade2ID))
        {
            GrenadeItemStats grenade = SaveDataManager.Instance.GetGrenadeByID(preset.grenade2ID);
            if (grenade != null && grenade != selectedGrenade1 && unlockedGrenades.Contains(grenade))
            {
                selectedGrenade2 = grenade;
            }
        }

        if (preset != null && !preset.initialized && presetIndex == 0)
        {
            if (unlockedGuns.Count > 0)
            {
                selectedGun1 = unlockedGuns[0];
            }

            if (unlockedGuns.Count > 1)
            {
                selectedGun2 = unlockedGuns[1];
            }

            if (unlockedGrenades.Count > 0)
            {
                selectedGrenade1 = unlockedGrenades[0];
            }

            if (unlockedGrenades.Count > 1)
            {
                selectedGrenade2 = unlockedGrenades[1];
            }
        }

        UpdateIndices();
        ShowGun1Preview();
        ShowGun2Preview();
        ShowGrenade1Preview();
        ShowGrenade2Preview();
        currentlySelectedGun = selectedGun1;
        ShowSelectedGun(selectedGun1);
    }

    public void RefreshUnlockedItems()
    {
        RefreshUnlockedGuns();
        RefreshUnlockedGrenades();
    }

    public void RefreshUnlockedGuns()
    {
        unlockedGuns.Clear();
        if (SaveDataManager.Instance == null)
        {
            return;
        }

        List<GunStats> loaded = SaveDataManager.Instance.GetUnlockedGuns();
        if (loaded != null)
        {
            unlockedGuns = new List<GunStats>(loaded);
        }

        unlockedGuns.RemoveAll(gun => gun == null);
        if (selectedGun1 != null && !unlockedGuns.Contains(selectedGun1))
        {
            selectedGun1 = null;
        }

        if (selectedGun2 != null && !unlockedGuns.Contains(selectedGun2))
        {
            selectedGun2 = null;
        }
    }

    public void RefreshUnlockedGrenades()
    {
        unlockedGrenades.Clear();
        if (SaveDataManager.Instance == null)
        {
            return;
        }

        List<GrenadeItemStats> loaded = SaveDataManager.Instance.GetUnlockedGrenades();
        if (loaded != null)
        {
            unlockedGrenades = new List<GrenadeItemStats>(loaded);
        }

        unlockedGrenades.RemoveAll(grenade => grenade == null);
        if (selectedGrenade1 != null && !unlockedGrenades.Contains(selectedGrenade1))
        {
            selectedGrenade1 = null;
        }

        if (selectedGrenade2 != null && !unlockedGrenades.Contains(selectedGrenade2))
        {
            selectedGrenade2 = null;
        }
    }

    public void Gun1Previous()
    {
        RefreshUnlockedGuns();
        CycleGun1(-1);
    }

    public void Gun1Next()
    {
        RefreshUnlockedGuns();
        CycleGun1(1);
    }

    public void Gun2Previous()
    {
        RefreshUnlockedGuns();
        CycleGun2(-1);
    }

    public void Gun2Next()
    {
        RefreshUnlockedGuns();
        CycleGun2(1);
    }

    public void Grenade1Previous()
    {
        RefreshUnlockedGrenades();
        CycleGrenade1(-1);
    }

    public void Grenade1Next()
    {
        RefreshUnlockedGrenades();
        CycleGrenade1(1);
    }

    public void Grenade2Previous()
    {
        RefreshUnlockedGrenades();
        CycleGrenade2(-1);
    }

    public void Grenade2Next()
    {
        RefreshUnlockedGrenades();
        CycleGrenade2(1);
    }

    private void CycleGun1(int direction)
    {
        int totalOptions = unlockedGuns.Count + 1;
        int currentOption = selectedGun1 == null ? 0 : unlockedGuns.IndexOf(selectedGun1) + 1;
        for (int attempts = 0; attempts < totalOptions; attempts++)
        {
            currentOption += direction;
            if (currentOption < 0)
            {
                currentOption = totalOptions - 1;
            }

            if (currentOption >= totalOptions)
            {
                currentOption = 0;
            }

            if (currentOption == 0)
            {
                selectedGun1 = null;
                gun1Index = -1;
                currentlySelectedGun = null;
                ShowGun1Preview();
                ShowSelectedGun(null);
                return;
            }

            GunStats candidate = unlockedGuns[currentOption - 1];
            if (candidate == selectedGun2)
            {
                continue;
            }

            selectedGun1 = candidate;
            gun1Index = currentOption - 1;
            currentlySelectedGun = selectedGun1;
            ShowGun1Preview();
            ShowSelectedGun(selectedGun1);
            return;
        }
    }

    private void CycleGun2(int direction)
    {
        int totalOptions = unlockedGuns.Count + 1;
        int currentOption = selectedGun2 == null ? 0 : unlockedGuns.IndexOf(selectedGun2) + 1;
        for (int attempts = 0; attempts < totalOptions; attempts++)
        {
            currentOption += direction;
            if (currentOption < 0)
            {
                currentOption = totalOptions - 1;
            }

            if (currentOption >= totalOptions)
            {
                currentOption = 0;
            }

            if (currentOption == 0)
            {
                selectedGun2 = null;
                gun2Index = -1;
                currentlySelectedGun = null;
                ShowGun2Preview();
                ShowSelectedGun(null);
                return;
            }

            GunStats candidate = unlockedGuns[currentOption - 1];
            if (candidate == selectedGun1)
            {
                continue;
            }

            selectedGun2 = candidate;
            gun2Index = currentOption - 1;
            currentlySelectedGun = selectedGun2;
            ShowGun2Preview();
            ShowSelectedGun(selectedGun2);
            return;
        }
    }

    private void CycleGrenade1(int direction)
    {
        int totalOptions = unlockedGrenades.Count + 1;
        int currentOption = selectedGrenade1 == null ? 0 : unlockedGrenades.IndexOf(selectedGrenade1) + 1;
        for (int attempts = 0; attempts < totalOptions; attempts++)
        {
            currentOption += direction;
            if (currentOption < 0)
            {
                currentOption = totalOptions - 1;
            }

            if (currentOption >= totalOptions)
            {
                currentOption = 0;
            }

            if (currentOption == 0)
            {
                selectedGrenade1 = null;
                grenade1Index = -1;
                ShowGrenade1Preview();
                ShowSelectedGrenade(null);
                return;
            }

            GrenadeItemStats candidate = unlockedGrenades[currentOption - 1];
            if (candidate == selectedGrenade2)
            {
                continue;
            }

            selectedGrenade1 = candidate;
            grenade1Index = currentOption - 1;
            ShowGrenade1Preview();
            ShowSelectedGrenade(selectedGrenade1);
            return;
        }
    }

    private void CycleGrenade2(int direction)
    {
        int totalOptions = unlockedGrenades.Count + 1;
        int currentOption = selectedGrenade2 == null ? 0 : unlockedGrenades.IndexOf(selectedGrenade2) + 1;
        for (int attempts = 0; attempts < totalOptions; attempts++)
        {
            currentOption += direction;
            if (currentOption < 0)
            {
                currentOption = totalOptions - 1;
            }

            if (currentOption >= totalOptions)
            {
                currentOption = 0;
            }

            if (currentOption == 0)
            {
                selectedGrenade2 = null;
                grenade2Index = -1;
                ShowGrenade2Preview();
                ShowSelectedGrenade(null);
                return;
            }

            GrenadeItemStats candidate = unlockedGrenades[currentOption - 1];
            if (candidate == selectedGrenade1)
            {
                continue;
            }

            selectedGrenade2 = candidate;
            grenade2Index = currentOption - 1;
            ShowGrenade2Preview();
            ShowSelectedGrenade(selectedGrenade2);
            return;
        }
    }

    private void UpdateIndices()
    {
        gun1Index = selectedGun1 == null ? -1 : unlockedGuns.IndexOf(selectedGun1);
        gun2Index = selectedGun2 == null ? -1 : unlockedGuns.IndexOf(selectedGun2);
        grenade1Index = selectedGrenade1 == null ? -1 : unlockedGrenades.IndexOf(selectedGrenade1);
        grenade2Index = selectedGrenade2 == null ? -1 : unlockedGrenades.IndexOf(selectedGrenade2);
    }

    private void UpdateLoadoutButtons()
    {
        bool hasGuns = unlockedGuns.Count > 0;
        bool hasGrenades = unlockedGrenades.Count > 0;
        if (gun1PreviousButton != null) gun1PreviousButton.interactable = hasGuns;
        if (gun1NextButton != null) gun1NextButton.interactable = hasGuns;
        if (gun2PreviousButton != null) gun2PreviousButton.interactable = hasGuns;
        if (gun2NextButton != null) gun2NextButton.interactable = hasGuns;
        if (grenade1PreviousButton != null) grenade1PreviousButton.interactable = hasGrenades;
        if (grenade1NextButton != null) grenade1NextButton.interactable = hasGrenades;
        if (grenade2PreviousButton != null) grenade2PreviousButton.interactable = hasGrenades;
        if (grenade2NextButton != null) grenade2NextButton.interactable = hasGrenades;
        if (preset1Button != null) preset1Button.interactable = true;
        if (preset2Button != null) preset2Button.interactable = true;
        if (preset3Button != null) preset3Button.interactable = true;
        if (setLoadoutButton != null) setLoadoutButton.interactable = true;
    }

    private void ShowSelectedGun(GunStats gun)
    {
        if (gun == null)
        {
            currentlySelectedGun = null;
            if (itemNameText != null) itemNameText.text = "None";
            if (itemDescriptionText != null) itemDescriptionText.text = "";
            if (postUpgradeDescriptionText != null) postUpgradeDescriptionText.text = "";
            DisableUpgradeButtons();
            return;
        }

        currentlySelectedGun = gun;
        WeaponUpgradeData upgradeData = GetUpgradeData(gun);
        if (upgradeData == null) return;
        int currentDamage = gun.shootDamage + (gun.damageUpgradeAmount * upgradeData.damageUpgradeLevel);
        int maxDamage = gun.maxDamage > 0 ? gun.maxDamage : currentDamage;
        currentDamage = Mathf.Min(currentDamage, maxDamage);
        float currentShootRate = gun.shootRate - (gun.shootRateUpgradeAmount * upgradeData.shootRateUpgradeLevel);
        float minShootRate = gun.minShootRate > 0 ? gun.minShootRate : 0.1f;
        currentShootRate = Mathf.Max(currentShootRate, minShootRate);
        int currentMagSize = gun.magSize + (gun.magSizeUpgradeAmount * upgradeData.magSizeUpgradeLevel);
        int maxMagSize = gun.maxMagSize > 0 ? gun.maxMagSize : gun.magSize;
        currentMagSize = Mathf.Min(currentMagSize, maxMagSize);
        if (itemNameText != null)
        {
            itemNameText.text = gun.itemName;
        }

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = "Damage: " + currentDamage + "\nShoot Rate: " + currentShootRate.ToString("0.00") + "\nMagazine: " + currentMagSize + "\n\nDamage Upgrade: " + upgradeData.damageUpgradeLevel + "\nShoot Rate Upgrade: " + upgradeData.shootRateUpgradeLevel + "\nMagazine Upgrade: " + upgradeData.magSizeUpgradeLevel;
        }

        int nextDamage = Mathf.Min(currentDamage + gun.damageUpgradeAmount, maxDamage);
        float nextShootRate = Mathf.Max(currentShootRate - gun.shootRateUpgradeAmount, minShootRate);
        int nextMagSize = Mathf.Min(currentMagSize + gun.magSizeUpgradeAmount, maxMagSize);
        if (postUpgradeDescriptionText != null)
        {
            postUpgradeDescriptionText.text = "Next Upgrade:" + "\nDamage: " + nextDamage + "\nShoot Rate: " + nextShootRate.ToString("0.00") + "\nMagazine: " + nextMagSize;
        }

        if (damageUpgradeButton != null)
        {
            damageUpgradeButton.interactable = currentDamage < maxDamage;
        }

        if (shootRateUpgradeButton != null)
        {
            shootRateUpgradeButton.interactable = currentShootRate > minShootRate;
        }

        if (magSizeUpgradeButton != null)
        {
            magSizeUpgradeButton.interactable = currentMagSize < maxMagSize;
        }
    }

    private void ShowSelectedGrenade(GrenadeItemStats grenade)
    {
        currentlySelectedGun = null;
        DisableUpgradeButtons();
        if (postUpgradeDescriptionText != null)
        {
            postUpgradeDescriptionText.text = "";
        }

        if (grenade == null)
        {
            if (itemNameText != null) itemNameText.text = "None";
            if (itemDescriptionText != null) itemDescriptionText.text = "";
            return;
        }

        if (itemNameText != null)
        {
            itemNameText.text = grenade.itemName;
        }

        if (itemDescriptionText != null)
        {
            itemDescriptionText.text = "Damage: " + grenade.damage + "\nBlast Radius: " + grenade.blastRadius.ToString("0.0") + "\nThrow Range: " + grenade.throwRange.ToString("0.0");
        }
    }

    private void DisableUpgradeButtons()
    {
        if (damageUpgradeButton != null) damageUpgradeButton.interactable = false;
        if (shootRateUpgradeButton != null) shootRateUpgradeButton.interactable = false;
        if (magSizeUpgradeButton != null) magSizeUpgradeButton.interactable = false;
    }

    public void UpgradeDamage()
    {
        if (currentlySelectedGun == null)
        {
            return;
        }

        WeaponUpgradeData upgradeData = GetUpgradeData(currentlySelectedGun);
        if (upgradeData == null) return;
        int maxDamage = currentlySelectedGun.maxDamage > 0 ? currentlySelectedGun.maxDamage : currentlySelectedGun.shootDamage;
        int currentDamage = currentlySelectedGun.shootDamage + (currentlySelectedGun.damageUpgradeAmount * upgradeData.damageUpgradeLevel);
        if (currentDamage >= maxDamage)
        {
            return;
        }

        upgradeData.damageUpgradeLevel++;
        SaveDataManager.Instance.SaveWeaponUpgrades();
        ShowSelectedGun(currentlySelectedGun);
    }

    public void UpgradeShootRate()
    {
        if (currentlySelectedGun == null)
        {
            return;
        }

        WeaponUpgradeData upgradeData = GetUpgradeData(currentlySelectedGun);
        if (upgradeData == null) return;
        float minShootRate = currentlySelectedGun.minShootRate > 0 ? currentlySelectedGun.minShootRate : 0.1f;
        float currentShootRate = currentlySelectedGun.shootRate - (currentlySelectedGun.shootRateUpgradeAmount * upgradeData.shootRateUpgradeLevel);
        currentShootRate = Mathf.Max(currentShootRate, minShootRate);
        if (currentShootRate <= minShootRate)
        {
            return;
        }

        upgradeData.shootRateUpgradeLevel++;
        SaveDataManager.Instance.SaveWeaponUpgrades();
        ShowSelectedGun(currentlySelectedGun);
    }

    public void UpgradeMagSize()
    {
        if (currentlySelectedGun == null)
        {
            return;
        }

        WeaponUpgradeData upgradeData = GetUpgradeData(currentlySelectedGun);
        if (upgradeData == null) return;
        int maxMagSize = currentlySelectedGun.maxMagSize > 0 ? currentlySelectedGun.maxMagSize : currentlySelectedGun.magSize;
        int currentMagSize = currentlySelectedGun.magSize + (currentlySelectedGun.magSizeUpgradeAmount * upgradeData.magSizeUpgradeLevel);
        if (currentMagSize >= maxMagSize)
        {
            return;
        }

        upgradeData.magSizeUpgradeLevel++;
        SaveDataManager.Instance.SaveWeaponUpgrades();
        ShowSelectedGun(currentlySelectedGun);
    }

    public void SetLoadout()
    {
        if (player == null)
        {
            Debug.LogError("LoadoutManager: Player is not assigned.");
            return;
        }

        if (SaveDataManager.Instance == null)
        {
            return;
        }

        RefreshUnlockedItems();
        SaveDataManager.Instance.SaveLoadoutPreset(activePresetIndex, selectedGun1, selectedGun2, selectedGrenade1, selectedGrenade2);
        player.ClearGunInventory();
        player.ClearGrenadeInventory();
        if (selectedGun1 != null && unlockedGuns.Contains(selectedGun1))
        {
            int magSize = GetCurrentMagSize(selectedGun1);
            player.AddStoredGun(selectedGun1, magSize, selectedGun1.maxReserve);
        }

        if (selectedGun2 != null && selectedGun2 != selectedGun1 && unlockedGuns.Contains(selectedGun2))
        {
            int magSize = GetCurrentMagSize(selectedGun2);
            player.AddStoredGun(selectedGun2, magSize, selectedGun2.maxReserve);
        }

        if (selectedGrenade1 != null && unlockedGrenades.Contains(selectedGrenade1))
        {
            player.AddGrenade(selectedGrenade1);
        }

        if (selectedGrenade2 != null && selectedGrenade2 != selectedGrenade1 && unlockedGrenades.Contains(selectedGrenade2))
        {
            player.AddGrenade(selectedGrenade2);
        }

        int equippedIndex = 0;
        if (player.GetGunInventory().Count > 0)
        {
            player.SetGunIndex(0);
            equippedIndex = player.GetGunIndex();
        }

        SaveDataManager.Instance.SavePlayerInventory(player.GetGunInventory(), equippedIndex);
        SaveDataManager.Instance.Save();
        Debug.Log("Preset " + (activePresetIndex + 1) + " equipped." + " | Gun 1: " + GetItemName(selectedGun1) + " | Gun 2: " + GetItemName(selectedGun2) + " | Grenade 1: " + GetItemName(selectedGrenade1) + " | Grenade 2: " + GetItemName(selectedGrenade2));
    }

    private int GetCurrentMagSize(GunStats gun)
    {
        if (gun == null) return 0;
        WeaponUpgradeData upgradeData = GetUpgradeData(gun);
        if (upgradeData == null) return gun.magSize;
        int currentMagSize = gun.magSize + (gun.magSizeUpgradeAmount * upgradeData.magSizeUpgradeLevel);
        int maxMagSize = gun.maxMagSize > 0 ? gun.maxMagSize : gun.magSize;
        return Mathf.Min(currentMagSize, maxMagSize);
    }

    private string GetItemName(ItemStats item)
    {
        if (item == null) return "None";
        return item.itemName;
    }

    private void ShowGun1Preview()
    {
        ShowGunPreview(selectedGun1, gun1PreviewImage);
    }

    private void ShowGun2Preview()
    {
        ShowGunPreview(selectedGun2, gun2PreviewImage);
    }

    private void ShowGunPreview(GunStats gun, Image previewImage)
    {
        if (previewImage == null) return;
        previewImage.enabled = true;
        previewImage.gameObject.SetActive(true);
        previewImage.color = Color.white;
        previewImage.raycastTarget = false;
        if (gun == null || gun.weaponIcon == null)
        {
            previewImage.sprite = null;
            return;
        }

        previewImage.sprite = gun.weaponIcon;
        previewImage.preserveAspect = true;
    }

    private void ShowGrenade1Preview()
    {
        ShowGrenadePreview(selectedGrenade1, grenade1PreviewImage);
    }

    private void ShowGrenade2Preview()
    {
        ShowGrenadePreview(selectedGrenade2, grenade2PreviewImage);
    }

    private void ShowGrenadePreview(GrenadeItemStats grenade, Image previewImage)
    {
        if (previewImage == null) return;
        previewImage.enabled = true;
        previewImage.gameObject.SetActive(true);
        previewImage.color = Color.white;
        previewImage.raycastTarget = false;
        if (grenade == null || grenade.grenadeIcon == null)
        {
            previewImage.sprite = null;
            return;
        }

        previewImage.sprite = grenade.grenadeIcon;
        previewImage.preserveAspect = true;
    }

    private WeaponUpgradeData GetUpgradeData(GunStats gun)
    {
        if (gun == null || SaveDataManager.Instance == null)
        {
            return null;
        }

        return SaveDataManager.Instance.GetWeaponUpgradeData(gun.itemID);
    }
}
