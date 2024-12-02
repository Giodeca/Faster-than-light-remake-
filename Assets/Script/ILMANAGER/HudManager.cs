using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudManager : Singleton<HudManager>
{
    public TMP_Text textOxygen;
    public TMP_Text textEngine;
    public TMP_Text textEnergy;
    public TMP_Text jump;
    public TMP_Text drone;
    public TMP_Text missle;
    public TMP_Text Coin;
    public TMP_Text scoreText;
    public List<GameObject> hpBar = new List<GameObject>();

    public GameObject hazardIndicator;
    public TMP_Text condition;

    [SerializeField] private GameObject panelUpgrades;

    public GameObject EventPanel;


    public List<GameObject> obj;
    public int activationNumber;
    public int maxActivationN;


    public GameObject shopPanel;
    public GameObject rewardPanel;



    protected override void Awake()
    {
        base.Awake();
        EventManager.UpdateEnergyText += OnEnergyMoved;
        EventManager.Activate += OnActivate;
        EventManager.Deactivate += OnDeactivate;
        EventManager.callEvent += OnCallEvent;
        EventManager.ShopOpen += OnShopOpen;
    }
    private void Start()
    {
        UpdateText();
    }
    private void OnDisable()
    {
        EventManager.UpdateEnergyText -= OnEnergyMoved;
        EventManager.Activate -= OnActivate;
        EventManager.Deactivate -= OnDeactivate;
        EventManager.callEvent -= OnCallEvent;
        EventManager.ShopOpen -= OnShopOpen;
    }
    private void Update()
    {

        if (!hazardIndicator.activeSelf && GameManager.Instance.conditionType != CONDITIONTYPE.NONE)
        {
            hazardIndicator.SetActive(true);
            condition.enabled = true;
        }
        else if (hazardIndicator.activeSelf && GameManager.Instance.conditionType == CONDITIONTYPE.NONE)
        {
            hazardIndicator.SetActive(false);
            condition.enabled = false;
        }

        textEngine.text = GameManager.Instance.ship.ship.enginePower.ToString();
        textOxygen.text = GameManager.Instance.ship.ship.oxygenPercentual.ToString();
        Coin.text = GameManager.Instance.Coin.ToString();

        if (condition != null)
            condition.text = GameManager.Instance.conditionType.ToString();

        textEnergy.text = GameManager.Instance.ship.ship.energy.ToString();
        jump.text = GameManager.Instance.ship.ship.jumps.ToString();
        missle.text = GameManager.Instance.ship.ship.missle.ToString();
        drone.text = GameManager.Instance.ship.ship.droneChip.ToString();
    }
    //HUD UI PARTE VISIVA
    private void ActiveThings()
    {
        activationNumber++;
        for (int i = 0; i < obj.Count; i++)
        {
            if (i == activationNumber)
            {
                obj[i].SetActive(true);
                return;
            }
        }
    }

    private void OnActivate()
    {
        DectiveThings();
    }

    public void OnShopOpen()
    {
        if (shopPanel.activeSelf == false)
        {
            shopPanel.SetActive(true);
        }
        else
        { shopPanel.SetActive(false); }
    }
    private void DectiveThings()
    {

        activationNumber--;
        for (int i = 0; i < obj.Count; i++)
        {
            if (i == activationNumber)
            {
                obj[i].SetActive(false);
                return;
            }
        }
    }
    private void OnDeactivate()
    {
        ActiveThings();
    }

    public void UpdateText()
    {
        textEnergy.text = GameManager.Instance.ship.ship.energy.ToString();
        textEngine.text = GameManager.Instance.ship.ship.enginePower.ToString();
        jump.text = GameManager.Instance.ship.ship.jumps.ToString();
        missle.text = GameManager.Instance.ship.ship.missle.ToString();
        drone.text = GameManager.Instance.ship.ship.droneChip.ToString();
        scoreText.text = ScoreKeeper.Instance.score.ToString();
        //textJump.text = GameManager.Instance.jumps.ToString();
    }
    private void OnEnergyMoved()
    {
        UpdateText();
    }

    private void OnCallEvent()
    {
        EventPanel.SetActive(true);
    }
    public void ShopHandling()
    {
        if (panelUpgrades.activeSelf == false)
            panelUpgrades.SetActive(true);
        else
            panelUpgrades.SetActive(false);
    }


}
