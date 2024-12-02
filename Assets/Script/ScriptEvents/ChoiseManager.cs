using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiseManager : MonoBehaviour
{
    [SerializeField] private string battle;
    [SerializeField] private string shop;
    [SerializeField] private string none;
    [SerializeField] private string rescue;
    [SerializeField] private string enter;
    [SerializeField] private string exit;
    [SerializeField] TMP_Text textToAssign;
    //[SerializeField] private string battle;
    [SerializeField] private List<GameObject> choises = new List<GameObject>();
    [SerializeField] TMP_Text textReward;


    private void OnEnable()
    {
        AssingText();
        AssingCoise();
        EventManager.RewardBattle += OnRewardObtained;
    }
    private void OnDisable()
    {
        textToAssign.text = "";
        EventManager.RewardBattle -= OnRewardObtained;
    }
    private void Start()
    {

    }


    private void OnRewardObtained(int gold, int fuel, int missle)
    {
        textReward.enabled = true;
        textReward.text = "You Obtained " + "Gold " + gold + " fuel " + fuel + " Missle " + missle;
    }


    private void AssingText()
    {
        switch (GameManager.Instance.planetType)
        {
            case PlanetType.ENTER:
                SetText(enter);
                break;
            case PlanetType.EXIT:
                SetText(exit);
                break;
            case PlanetType.SHOP:
                SetText(shop);
                break;
            case PlanetType.BATTLE:
                SetText(battle);
                break;
            case PlanetType.SAVE:
                SetText(rescue);
                break;
            case PlanetType.NONE:
                SetText(none);
                break;
        }
    }

    private void AssingCoise()
    {
        for (int i = 0; i < choises.Count; i++)
        {

            choises[i].SetActive(true);
            choises[i].GetComponent<ChoiseScript>().CreateNewScriptable();
            choises[i].GetComponent<ChoiseScript>().scriptable.numberOption = i;
            choises[i].GetComponent<ChoiseScript>().TextHandle();
        }
    }
    private void SetText(string text)
    {
        textToAssign.text = text;
    }

}
