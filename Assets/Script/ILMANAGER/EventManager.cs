using System;

public static class EventManager
{
    public static Action<Statistc, bool> EnergyMoved;
    public static Action UpdateEnergyText;
    public static Action Deactivate;
    public static Action Activate;
    public static Action Boom;
    public static Action DeactivateUnit;
    public static Action<bool> StartBattle;
    public static Action SpawnShip;
    public static Action openAllDoors;

    public static Action callEvent;
    public static Action<int, int> GetReward;
    public static Action<int, int> Runway;
    public static Action<int, int> PayToRepair;
    public static Action<int, int> PayToDiscover;
    public static Action StartFight;
    public static Action<bool, int, int> NothingHappen;
    public static Action Recruit;
    public static Action ShopOpen;
    public static Action<WeaponScriptable> WeaponAdd;
    public static Action<int, int, int> RewardBattle;
    public static Action Nebulosa;
    public static Action ResetNebulosa;

    public static Action ResetSelected;
    public static Action CallNewArea;
    public static Action UnitCounter;

}
