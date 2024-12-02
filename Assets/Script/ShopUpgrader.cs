using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShopUpgrader : MonoBehaviour, IPointerClickHandler/*, IPointerEnterHandler, IPointerExitHandler*/
{
    [SerializeField] private EnergyHandler energyHandler;
    [SerializeField] private int costUpdrade;
    [SerializeField] private GameObject panelInfo;
    [SerializeField] private int numberUpdatesPossible;
    [SerializeField] private int numberUpdateActual;
    [SerializeField] private GameObject[] numberActivation;
    [SerializeField] private TMP_Text textCost;



    private void Start()
    {
        upgradeAllow();
    }

    private void Update()
    {
        textCost.text = costUpdrade.ToString();
    }

    private void upgradeAllow()
    {
        Debug.Log("Here");
        for (int i = 0; i < numberUpdatesPossible; i++)
        {
            numberActivation[i].SetActive(true);
        }
    }

    private void UpgradeDone()
    {
        numberUpdateActual--;
        numberActivation[numberUpdateActual].SetActive(false);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (numberUpdateActual > 0)
        {
            UpgradeDone();
            if (GameManager.Instance.Coin >= costUpdrade)
                UpdateSkill(costUpdrade, ref GameManager.Instance.Coin);
        }
        else
        {
            Debug.Log("NoUpgradePossible");
        }

    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (panelInfo != null)
    //        panelInfo.SetActive(true);
    //}

    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    if (panelInfo != null)
    //        panelInfo.SetActive(false);
    //}

    private void UpdateSkill(int cost, ref int coin)
    {
        Debug.Log("1");
        coin -= cost;
        costUpdrade += costUpdrade * 20 / 100;
        energyHandler.activationNumberMax++;
    }
}
