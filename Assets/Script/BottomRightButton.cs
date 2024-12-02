using UnityEngine;
using UnityEngine.EventSystems;

public class BottomRightButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {

        EventManager.openAllDoors?.Invoke();

    }
}
