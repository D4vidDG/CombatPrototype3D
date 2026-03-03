using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    [SerializeField] Weapon weaponInScene;
    [SerializeField] Canvas pickUpUI;

    void Start()
    {
        ShowPickUpUI(false);
    }

    public Weapon GetWeapon()
    {
        return weaponInScene;
    }

    public void ShowPickUpUI(bool show)
    {
        pickUpUI.gameObject.SetActive(show);
    }
}
