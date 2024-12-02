using UnityEngine;

public enum PlanetType
{
    ENTER, EXIT, SHOP, BATTLE, SAVE, NONE
}
public enum Statistc
{
    OXYGEN, ENGINE, WEAPON, SHIELD, INFERMERY
}
public enum RoomType
{
    INFERMERY, OXIGEN, WEAPON, SHILED, ENGINEROOM, PILOTING, DOORSROOM, CAMERASROOM, NONE, SPACE
}
public enum ShipType
{
    ENEMY, PLAYER, BOSS
}
public enum RoomStatus
{
    OK, ONFIRE, HASHOLE
}

public enum MissionType
{
    FIGHT, SHOP, RESQUE, NOTHING, REWARD, RESCRUIT, PAYFORSTUFF
}
public enum AmmoType
{
    LASER, MISSLE
}
public enum BUYTHINGS
{
    REPAIR, JUMPS, MISSLE, DRONE, WEAPON
}

public enum CONDITIONTYPE
{
    NONE, SUN, METEOR, NEBULOSA,
}

public enum BattleEnemy
{
    ENEMY1, ENEMY2, ENEMY3, BOSS
}

public class EnumLibrary : MonoBehaviour
{

}
