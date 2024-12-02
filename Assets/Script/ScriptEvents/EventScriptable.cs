using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Event", menuName = "Event")]
public class EventScriptable : ScriptableObject
{
    public bool isCompleted;
    public int numberOption;
    public int costAction;
    public int RewardAction;
    public List<string> textMessage = new List<string>();
    public MissionType missionType;



    public void SetText(TMP_Text text)
    {
        text.text = textMessage[numberOption];
    }

    public void AssignAllFunctions()
    {
        Debug.Log(numberOption);
        switch (GameManager.Instance.planetType)
        {
            case PlanetType.ENTER:
                switch (numberOption)
                {
                    case 0:
                        missionType = MissionType.NOTHING;
                        TextWriting("You just enter in the secotor, prepare to visit the system");
                        break;
                    case 1:
                        missionType = MissionType.NOTHING;
                        TextWriting("");
                        break;
                        //case 2:
                        //    missionType = MissionType.NOTHING;
                        //    TextWriting("");
                        //    break;
                }
                break;
            case PlanetType.EXIT:
                switch (numberOption)
                {
                    case 0:
                        missionType = MissionType.NOTHING;
                        int random = Random.Range(10, 20);
                        costAction = random;
                        TextWriting("You are about to exit the secotor, prepare to visit the new system");
                        break;
                    case 1:
                        missionType = MissionType.NOTHING;
                        TextWriting("");
                        break;
                        //case 2:
                        //    missionType = MissionType.NOTHING;
                        //    TextWriting("");
                        //    break;
                }
                break;
            case PlanetType.SHOP:
                switch (numberOption)
                {
                    case 0:
                        missionType = MissionType.SHOP;
                        TextWriting("You can buy stuff in the shop now");

                        break;
                    case 1:
                        missionType = MissionType.NOTHING;
                        TextWriting("");
                        break;
                        //case 2:
                        //    missionType = MissionType.NOTHING;
                        //    TextWriting("");
                        //    break;
                }
                break;
            case PlanetType.BATTLE:
                switch (numberOption)
                {
                    case 0:
                        missionType = MissionType.FIGHT;
                        TextWriting("You engaged into a fight Get ready to battle");
                        break;
                    case 1:
                        missionType = MissionType.PAYFORSTUFF;
                        TextWriting("You can pay to avoid the fight");
                        int random = Random.Range(10, 20);
                        costAction = random;
                        break;
                        //case 2:
                        //    missionType = MissionType.NOTHING;
                        //    TextWriting("You can pay to avoid the fight");
                        //    break;
                }
                break;
            case PlanetType.SAVE:
                switch (numberOption)
                {
                    case 0:
                        missionType = MissionType.RESQUE;
                        TextWriting("You can recrit a new member, but we have to fight first");
                        int random = Random.Range(10, 20);
                        costAction = random;
                        break;
                    case 1:
                        missionType = MissionType.NOTHING;
                        TextWriting("Avoid the battle and leave the hostage alone");
                        break;
                        //case 2:
                        //    missionType = MissionType.NOTHING;
                        //    TextWriting("You can pay to avoid the fight");
                        //    break;
                }
                break;
            case PlanetType.NONE:
                switch (numberOption)
                {
                    case 0:
                        missionType = MissionType.NOTHING;
                        int random = Random.Range(10, 20);
                        costAction = random;
                        TextWriting("There is an empty station We can rest a bit");
                        break;
                    case 1:
                        missionType = MissionType.NOTHING;
                        TextWriting("");
                        break;
                        //case 2:
                        //    missionType = MissionType.NOTHING;
                        //    TextWriting("You can pay to avoid the fight");
                        //    break;
                }
                break;
        }
    }

    private void TextWriting(string text)
    {
        textMessage[numberOption] = text;
    }

    public void CaseManager()
    {
        switch (missionType)
        {
            case MissionType.FIGHT:
                Fight();
                break;
            case MissionType.SHOP:
                Shop();
                break;
            case MissionType.RESQUE:
                Resque();
                break;
            case MissionType.NOTHING:
                Nothing();
                break;
            case MissionType.REWARD:
                Reward();
                break;
            case MissionType.RESCRUIT:
                Recruit();
                break;
            case MissionType.PAYFORSTUFF:
                PayForStuff();
                break;

        }
    }

    public void Fight()
    {
        EventManager.StartFight?.Invoke();
    }

    public void Shop()
    {
        EventManager.ShopOpen?.Invoke();
    }

    public void Resque()
    {
        EventManager.Runway?.Invoke(costAction, GameManager.Instance.Coin);
    }

    public void Nothing()
    {
        bool random = Random.Range(0, 1) > 0.5f;
        EventManager.NothingHappen?.Invoke(random, costAction, GameManager.Instance.Coin);
    }

    public void Reward()
    {
        EventManager.GetReward?.Invoke(costAction, GameManager.Instance.Coin);
    }

    public void Recruit()
    {
        EventManager.Recruit?.Invoke();
    }

    public void PayForStuff()
    {
        EventManager.PayToRepair?.Invoke(costAction, GameManager.Instance.Coin); // O PayToDiscover, a seconda del contesto
    }
}