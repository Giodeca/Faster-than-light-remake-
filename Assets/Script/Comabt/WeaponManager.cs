using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] List<GameObject> weapons = new List<GameObject>();
    private int weaponBought;
    private void OnEnable()
    {
        EventManager.WeaponAdd += OnWeaponBoutgh;
    }
    private void OnDisable()
    {
        EventManager.WeaponAdd -= OnWeaponBoutgh;
    }
    private void Start()
    {

    }

    private void OnWeaponBoutgh(WeaponScriptable weapon)
    {

        if (weaponBought < weapons.Count)
        {
            weapons[weaponBought].SetActive(true);
            weapons[weaponBought].GetComponent<WeaponScript>().weaponScriptable = weapon;
            weaponBought++;
        }

    }
}
