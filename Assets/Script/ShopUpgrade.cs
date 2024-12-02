using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopUpgrade : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private BUYTHINGS buy;
    [SerializeField] private int cost;
    [SerializeField]
    private TMP_Text text;
    [SerializeField] private List<WeaponScriptable> weaponScriptables = new List<WeaponScriptable> { };
    [SerializeField] private WeaponScriptable weapon;
    private void Start()
    {

    }
    private void OnEnable()
    {

        if (buy == BUYTHINGS.WEAPON)
        {
            cost = weapon.shopCost;
        }
        text.text = cost.ToString();


    }
    private void OnDisable()
    {
        weapon = null;
    }
    private void WhatToBuy()
    {
        switch (buy)
        {
            case BUYTHINGS.REPAIR:
                int startHp = GameManager.Instance.ship.ship.life;
                if (GameManager.Instance.ship.ship.life < GameManager.Instance.ship.ship.maxLife)
                {

                    GameManager.Instance.ship.ship.life += 10;
                    StartCoroutine(GameManager.Instance.ship.AnimateHPDecrease(startHp, GameManager.Instance.ship.ship.life));
                    if (GameManager.Instance.ship.ship.life > GameManager.Instance.ship.ship.maxLife)
                    {
                        GameManager.Instance.ship.ship.life = GameManager.Instance.ship.ship.maxLife;
                    }
                }

                break;
            case BUYTHINGS.JUMPS:
                GameManager.Instance.ship.ship.jumps++;
                break;
            case BUYTHINGS.MISSLE:
                GameManager.Instance.ship.ship.missle++;
                break;
            case BUYTHINGS.DRONE:
                GameManager.Instance.ship.ship.droneChip++;
                break;
            case BUYTHINGS.WEAPON:
                EventManager.WeaponAdd?.Invoke(weapon);
                Destroy(this.gameObject);
                break;

        }
    }




    //private void AssignWeapon()
    //{
    //    if (weaponScriptables.Count == 0)
    //    {
    //        Debug.LogWarning("La lista weaponScriptables è vuota!");
    //        return;
    //    }

    //    // Ottiene un indice casuale dalla lista
    //    int randomIndex = Random.Range(0, weaponScriptables.Count);

    //    // Assegna l'elemento casuale alla variabile selectedWeaponScriptable
    //    weapon = weaponScriptables[randomIndex];

    //    Debug.Log("Arma selezionata: " + weapon.name);
    //}

    public void OnPointerClick(PointerEventData eventData)
    {

        if (GameManager.Instance.Coin >= cost)
        {
            GameManager.Instance.Coin -= cost;
            WhatToBuy();
            HudManager.Instance.UpdateText();
        }

    }
}
