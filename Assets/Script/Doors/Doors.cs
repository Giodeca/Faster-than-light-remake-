using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    [SerializeField] private List<Rooms> roomConnectd = new List<Rooms>();
    [SerializeField] private bool isOpen;
    private bool isUpdated;
    private Coroutine oxygenCoroutine;
    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        EventManager.openAllDoors += OpenTheDoor;
    }

    private void OnDisable()
    {
        EventManager.openAllDoors -= OpenTheDoor;
    }
    private void Update()
    {
        if (isOpen && !isUpdated)
        {
            isUpdated = true;
            if (oxygenCoroutine == null)
            {
                oxygenCoroutine = StartCoroutine(UpdateOxygenLevels());
            }
        }
        else if (!isOpen && isUpdated)
        {
            isUpdated = false;
            if (oxygenCoroutine != null)
            {
                StopCoroutine(oxygenCoroutine);
                oxygenCoroutine = null;
            }
        }
    }

    public void OpenTheDoor()
    {
        Debug.Log("Qui");
        if (!isOpen)
        {
            isOpen = true;
            animator.SetTrigger("DoorOpen");

        }
        else
        {
            isOpen = false;
            animator.SetTrigger("DoorClose");

        }
    }

    private IEnumerator UpdateOxygenLevels()
    {
        float doorOpenTime = 0f;
        float oxygenUpdateInterval = 1f;
        float fireSpreadDelay = 5f;

        while (isOpen)
        {
            MoveOxygenBetweenRooms(roomConnectd[0], roomConnectd[1]);

            doorOpenTime += oxygenUpdateInterval;
            if (doorOpenTime >= fireSpreadDelay)
            {
                CheckAndSpreadFire();
                doorOpenTime = 0f; // Reset the timer
            }

            yield return new WaitForSeconds(oxygenUpdateInterval);
        }

        oxygenCoroutine = null;
    }

    private void MoveOxygenBetweenRooms(Rooms room1, Rooms room2)
    {
        if (room1.scriptable.oxygenRoomLevel < room2.scriptable.oxygenRoomLevel)
        {
            if (room2.scriptable.roomType != RoomType.SPACE)
            {
                room1.scriptable.oxygenRoomLevel++;
                room2.scriptable.oxygenRoomLevel--;
            }
        }
        else if (room2.scriptable.oxygenRoomLevel < room1.scriptable.oxygenRoomLevel)
        {
            if (room1.scriptable.roomType != RoomType.SPACE)
            {
                room1.scriptable.oxygenRoomLevel--;
                room2.scriptable.oxygenRoomLevel++;
            }
        }
    }

    private void CheckAndSpreadFire()
    {
        if (roomConnectd[0].scriptable.isOnFire && !roomConnectd[1].scriptable.isOnFire)
        {
            roomConnectd[1].scriptable.isOnFire = true;
        }
        else if (roomConnectd[1].scriptable.isOnFire && !roomConnectd[0].scriptable.isOnFire)
        {
            roomConnectd[0].scriptable.isOnFire = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Unit"))
        {
            Debug.Log("Collision");
            animator.SetTrigger("DoorOpen");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Unit"))
        {
            animator.SetTrigger("DoorClose");
        }
    }
}
