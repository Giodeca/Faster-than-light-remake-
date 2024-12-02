using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject panelSettings;
    public void OnPointerEnter(PointerEventData eventData)
    {
        panelSettings.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        panelSettings.SetActive(false);
    }
}
