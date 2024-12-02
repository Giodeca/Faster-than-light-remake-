using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChoiseScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public EventScriptable scriptable;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Sprite over;
    [SerializeField] private Sprite outover;
    [SerializeField] private Image image;
    private void Start()
    {

    }

    public void CreateNewScriptable()
    {
        EventScriptable newScriptable = Instantiate(scriptable);
        scriptable = newScriptable;
    }
    public void TextHandle()
    {
        scriptable.AssignAllFunctions();
        scriptable.SetText(text);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        scriptable.CaseManager();
        GameManager.Instance.gamePaused = false;
        HudManager.Instance.EventPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = over;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = outover;
    }
}
