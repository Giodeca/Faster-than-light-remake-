using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Weapon", menuName = "WeaponCreate")]

public class WeaponScriptable : ScriptableObject
{
    public float realoadTime;
    public float timePassed;
    public bool canShoot;
    public bool isFull;
    public bool isAlreadyRunning;
    public bool isRunningDownNow;
    public string weaponName;
    public int damage;
    public int ammoShoot;
    public AmmoType ammoType;
    public int cost;
    public int shopCost;

    public ProjectileScriptable prj;

    //public float elapsedTime = 0f;
    //public float incrementInterval = 1f;

    public void AssignMaxValue(Slider loadingBar)
    {
        loadingBar.maxValue = realoadTime;
    }
    public IEnumerator RealoadRoutine(Slider loadingBar)
    {
        isAlreadyRunning = true;

        while (timePassed < realoadTime)
        {

            timePassed += Time.deltaTime;
            loadingBar.value = timePassed;
            //elapsedTime += Time.deltaTime;
            //if (elapsedTime >= incrementInterval)
            //{
            //    timePassed++;
            //    elapsedTime = 0f;
            //}

            if (timePassed >= realoadTime)
            {
                canShoot = true;
                isFull = true;
                timePassed = realoadTime;

            }
            yield return null;
        }

    }

    public void Shoot(/*int hpEnemy, ref int statToPay*/)
    {
        if (ammoType == AmmoType.MISSLE)
        {
            GameManager.Instance.ship.ship.missle--;
        }
        //hpEnemy -= damage;
        canShoot = false;
        //statToPay -= cost;
        timePassed = 0;
        isAlreadyRunning = false;
        isFull = false;


    }


    public IEnumerator SliderRoutine(Slider loadingBar)
    {
        canShoot = false;
        isFull = false;
        //isAlreadyRunning = true;
        isRunningDownNow = true;


        while (timePassed > 0)
        {
            timePassed -= Time.deltaTime;
            loadingBar.value = timePassed;

            if (timePassed <= 0)
            {
                timePassed = 0;
                isFull = false;
                isRunningDownNow = false;  // Resetta il flag qui

            }

            yield return null;
        }
    }
    public void ResetAmmo()
    {
        canShoot = false;
        timePassed = 0;
        isAlreadyRunning = false;
        isFull = false;

    }
}
