using UnityEngine;
using UnityEngine.EventSystems;

public class QuitInGameMenu : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Application.Quit();
    }


}
