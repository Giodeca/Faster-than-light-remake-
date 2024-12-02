using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnergyHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Statistc statToHandle;
    public List<GameObject> obj;
    public int activationNumber;
    public int activationNumberMax = 5;
    public GameObject popUp;
    public int energyCount;
    private bool toRemove;
    [SerializeField] private Ship ship;


    // Metodo per gestire UI Visiva
    private void ActiveThings()
    {

        activationNumber++;
        for (int i = 0; i < obj.Count; i++)
        {
            if (i == activationNumber)
            {
                //Debug.Log(activationNumber);
                obj[i].SetActive(true);

                //Debug.Log(activationNumber + "Second");
                return;
            }
        }
    }


    private void DectiveThings()
    {

        if (activationNumber >= 0)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                if (i == activationNumber)
                {
                    obj[i].SetActive(false);
                    activationNumber--;

                    //Debug.Log(activationNumber + "Second");
                    return;
                }
            }
        }

    }
    public void OnPointerClick(PointerEventData eventData)
    {


        if (Input.GetMouseButtonUp(1))
        {
            if (ship.ship.energy < ship.ship.energyMax)
            {
                if (activationNumber <= activationNumberMax)
                {

                    toRemove = true;
                    DectiveThings();
                    EventManager.EnergyMoved?.Invoke(statToHandle, toRemove);
                    EventManager.Deactivate?.Invoke();
                }
            }

        }
        else
        {
            if (activationNumber < activationNumberMax && HudManager.Instance.activationNumber > 0)
            {
                toRemove = false;
                ActiveThings();
                EventManager.EnergyMoved?.Invoke(statToHandle, toRemove);
                EventManager.Activate?.Invoke();
            }

        }
    }

    private void InvokeStat()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        popUp.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        popUp.gameObject.SetActive(false);
    }



}
