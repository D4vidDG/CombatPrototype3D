using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    // [SerializeField] Image weaponImage;
    // [SerializeField] TextMeshProUGUI ammoLeftText;
    // [SerializeField] TextMeshProUGUI ammoTotalText;
    // [SerializeField] UIBar reloadBar;


    // WeaponController weaponController;
    // FlashingComponent imageFlashingEffect;

    // private void Awake()
    // {
    //     weaponController = FindObjectOfType<WeaponController>();
    //     imageFlashingEffect = weaponImage.GetComponent<FlashingComponent>();
    // }

    // private void Start()
    // {
    //     reloadBar.SetPercentage(0);
    // }

    // private void Update()
    // {
    //     if (weaponController.GetCurrentWeapon() == null)
    //     {
    //         Hide();
    //     }
    //     else
    //     {
    //         Show();
    //         UpdateUI();
    //     }
    // }

    // private void UpdateUI()
    // {
    //     // Weapon currentWeapon = weaponController.GetCurrentWeapon();
    //     // weaponImage.sprite = currentWeapon.image;
    //     // //ammoLeftText.text = currentWeapon.GetAmmoLeft().ToString();
    //     // ammoTotalText.text = currentWeapon.maxAmmo.ToString();

    //     // if (//currentWeapon.IsReloading())
    //     // {
    //     //     reloadBar.gameObject.SetActive(true);
    //     //     imageFlashingEffect.ToggleEffect(true);
    //     //     //reloadBar.SetPercentage(currentWeapon.GetReloadPercentage());
    //     // }
    //     // else
    //     // {
    //     //     reloadBar.SetPercentage(0);
    //     //     imageFlashingEffect.ToggleEffect(false);
    //     //     weaponImage.color = new Color(255, 255, 255, 1);
    //     //     reloadBar.gameObject.SetActive(false);
    //     // }
    // }


    // private void Show()
    // {
    //     weaponImage.enabled = true;
    //     ammoLeftText.enabled = true;
    //     ammoTotalText.enabled = true;
    // }

    // private void Hide()
    // {
    //     weaponImage.enabled = false;
    //     ammoLeftText.enabled = false;
    //     ammoTotalText.enabled = false;
    //     reloadBar.SetPercentage(0);
    //     reloadBar.gameObject.SetActive(false);
    // }


}
