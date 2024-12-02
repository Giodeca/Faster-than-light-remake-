using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ship", menuName = "shipCreate")]
public class ScriptableShip : ScriptableObject
{
    [Header("Ship")]
    public int life;//  HP barra in alto
    public int maxLife;

    public int missle;//  Missili barra in alto
    public int droneChip;//barra in alto
    public int jumps;
    //public int oxigenLevel;
    public ShipType shipType;
    public List<Rooms> shipRooms = new List<Rooms>();

    public int healRate;
    public int infermeryEnergy;

    [Header("ShipAbility")]

    public int shipMember;// lato sinistra 
    public int shieldCapacity;// alto sinistra
    public int energy;// questo rappresenta barre verdi
    public int energyMax;// Non ancora utilizzato



    [Header("ShipEngine")]
    public int enginePower;// engine pover affiaco a oxygenPercentual

    [Header("ShipEngine")]
    public int oxygenPercentual;// da collegare in alto
    private int oxygenPercentualMax = 100;
    public int oxyigen;// rappresenta le barre 
    public int weaponCharge;// rapprenseta carica 

    private float elapsedTime = 0f;
    private float incrementInterval = 1f;

    public Shield shiedlScript;



    public void OxygenHandle()
    {
        switch (oxyigen)
        {
            case 0:
                if (oxygenPercentual > 0)
                {
                    DecrementStat(1);
                }
                break;
            case 1:
                if (oxygenPercentual < oxygenPercentualMax)
                {
                    IncrementStat(1);
                }
                break;
            case 2:
                if (oxygenPercentual < oxygenPercentualMax)
                {
                    IncrementStat(0.5f);
                }
                break;
            case 4: break;
            case 5: break;
        }
    }
    private void IncrementStat(float time)
    {
        elapsedTime += Time.deltaTime;
        incrementInterval = time;
        if (elapsedTime >= incrementInterval)
        {
            incrementInterval = 1;
            elapsedTime = 0f;
            oxygenPercentual++;
        }
    }
    private void DecrementStat(float time)
    {
        elapsedTime += Time.deltaTime;
        incrementInterval = time;
        if (elapsedTime >= incrementInterval)
        {
            incrementInterval = 1;
            elapsedTime = 0f;
            oxygenPercentual--;
        }
    }
    public void OnEnergyMoved(Statistc stat, bool toRemove)
    {
        switch (stat)
        {
            case Statistc.OXYGEN:
                if (energy > 0)
                    EnergyMoving(toRemove, ref oxyigen);
                break;
            case Statistc.ENGINE:
                if (energy > 0)
                {
                    if (!toRemove)
                    {
                        energy--;
                        if (enginePower <= 0)
                            enginePower += 10;
                        else
                            enginePower += 5;
                    }
                    else
                    {
                        energy++;
                        if (enginePower > 10)
                            enginePower -= 5;
                        else if (enginePower == 10)
                            enginePower -= 10;
                    }
                }
                break;
            case Statistc.WEAPON:
                if (energy > 0)
                    EnergyMoving(toRemove, ref weaponCharge);
                break;
            case Statistc.INFERMERY:
                if (energy > 0)
                    EnergyMoving(toRemove, ref infermeryEnergy);

                switch (infermeryEnergy)
                {
                    case 0:
                        healRate = 0;
                        break;
                    case 1:
                        healRate = 1;
                        break;
                    case 2:
                        healRate = 2;
                        break;
                    case 3:
                        healRate = 3;
                        break;
                }
                break;
            case Statistc.SHIELD:
                int shiedlData = shieldCapacity;
                if (energy > 0)
                    EnergyMoving(toRemove, ref shieldCapacity);

                if (shiedlData > shieldCapacity)
                    shiedlScript.DownGradeShield();
                else
                    shiedlScript.UpdateShield();
                break;
        }
        EventManager.UpdateEnergyText?.Invoke();
    }

    public void DamagedShip(int damage)
    {
        life -= damage;
        if (life <= 0)
        {
            life = 0;

        }
    }
    public void HealShip(int damage, ref int hp)
    {
        hp += damage;
    }

    private void EnergyMoving(bool toRemove, ref int stat)
    {
        if (energy > 0)
        {
            if (!toRemove)
            {
                energy--;
                stat++;
            }
            else
            {
                energy++;
                stat--;
            }
        }
    }


    public void RewardAfetMission()
    {

    }
}
