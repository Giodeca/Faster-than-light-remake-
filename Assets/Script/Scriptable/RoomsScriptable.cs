using UnityEngine;
[CreateAssetMenu(fileName = "Room", menuName = "Rooms")]
public class RoomsScriptable : ScriptableObject
{
    [Header("RoomStat")]
    public int capStat;
    public int roomHP;
    public int repair;
    public int oxygenRoomLevel;
    public bool isOnFire;
    public int roomHPUsable;
    public bool isUsableAbility;
    public RoomType roomType;
    public RoomStatus roomStatus;


    public void DebugStats()
    {
        Debug.Log("Room Stats:");
        Debug.Log("CapStat: " + capStat);
        Debug.Log("RoomHP: " + roomHP);
        Debug.Log("Repair: " + repair);
        Debug.Log("Is Usable: " + isUsableAbility);
    }





    public void RoomEffect(RoomsScriptable value)
    {
        switch (value.roomType)
        {
            case RoomType.INFERMERY:
                break;
            case RoomType.ENGINEROOM:
                break;
            case RoomType.OXIGEN:
                break;
            case RoomType.WEAPON:
                break;
            case RoomType.PILOTING:
                GameManager.Instance.someoneIsPiloting = true;

                break;
        }
    }


    public void RoomEffectExit(RoomsScriptable value)
    {
        switch (value.roomType)
        {
            case RoomType.INFERMERY:
                break;
            case RoomType.ENGINEROOM:
                break;
            case RoomType.OXIGEN:
                break;
            case RoomType.WEAPON:
                break;
            case RoomType.PILOTING:
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.someoneIsPiloting = false;
                    Debug.Log(GameManager.Instance.someoneIsPiloting);
                }
                break;
        }
    }
}
