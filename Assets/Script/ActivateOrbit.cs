using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class ActivateOrbit : AActions<PlanetType>, IPointerEnterHandler
{
    public GameObject _gameObject;
    public List<GameObject> nodesConnected;
    public bool isEnter;
    public bool isExit;
    public PlanetType type;
    public TMP_Text text;
    public GameObject textG;
    public CONDITIONTYPE eventType;

    public void SetStatus()
    {
        if (isEnter)
        {
            textG.SetActive(true);
            text.text = "Entry";
        }
        else if (isExit)
        {
            textG.SetActive(true);
            text.text = "Exit";
        }
    }
    public override void ActionImplementation(int stat, PlanetType value)
    {
        GameManager.Instance.conditionType = eventType;
        CallActivationEvent();
        GameManager.Instance.previousConditionType = eventType;
        switch (type)
        {
            case PlanetType.ENTER:
                GameManager.Instance.planetType = PlanetType.ENTER;

                break;
            case PlanetType.EXIT:
                GameManager.Instance.planetType = PlanetType.EXIT;
                break;
            case PlanetType.SHOP:
                GameManager.Instance.planetType = PlanetType.SHOP;
                break;
            case PlanetType.BATTLE:
                GameManager.Instance.planetType = PlanetType.BATTLE;
                break;
            case PlanetType.SAVE:
                GameManager.Instance.planetType = PlanetType.SAVE;
                break;
            case PlanetType.NONE:
                GameManager.Instance.planetType = PlanetType.NONE;
                break;
        }
    }

    private void CallActivationEvent()
    {
        switch (eventType)
        {
            case CONDITIONTYPE.NONE:

                if (GameManager.Instance.conditionType == CONDITIONTYPE.NONE)
                {
                    if (GameManager.Instance.previousConditionType == CONDITIONTYPE.NEBULOSA)
                        EventManager.ResetNebulosa?.Invoke();
                }

                break;
            case CONDITIONTYPE.SUN:
                if (GameManager.Instance.conditionType == CONDITIONTYPE.NONE)
                {
                    if (GameManager.Instance.previousConditionType == CONDITIONTYPE.NEBULOSA)
                        EventManager.ResetNebulosa?.Invoke();
                }

                break;
            case CONDITIONTYPE.METEOR
                :
                if (GameManager.Instance.conditionType == CONDITIONTYPE.NONE)
                {
                    if (GameManager.Instance.previousConditionType == CONDITIONTYPE.NEBULOSA)
                        EventManager.ResetNebulosa?.Invoke();
                }
                break;
            case CONDITIONTYPE.NEBULOSA
                :
                EventManager.Nebulosa?.Invoke();
                break;


        }
    }

    public void PlanetTypeAssignation()
    {
        float random = Random.Range(0f, 1f); // Genera un numero casuale tra 0 e 1

        if (random < 0.7f)
        {
            type = PlanetType.BATTLE; // 70% di probabilità di essere una missione di battaglia
        }
        else
        {
            int randomEvent = Random.Range(0, 3); // Gli altri eventi avranno un totale di 30%
            switch (randomEvent)
            {
                case 0:
                    type = PlanetType.SHOP;
                    break;
                case 1:
                    type = PlanetType.SAVE;
                    break;
                case 2:
                    type = PlanetType.NONE;
                    break;
            }
        }
    }
    public override void Broken(int stat, PlanetType value, int damage)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GridCreator.Instance.HoverNode(this.gameObject);
    }

    public override void Repair(int stat, PlanetType value)
    {
        throw new System.NotImplementedException();
    }
}
