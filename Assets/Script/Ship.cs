using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Ship : MonoBehaviour
{
    [SerializeField]
    public List<Node> shipNodes = new List<Node>();
    public List<Rooms> rooms = new List<Rooms>();
    private bool canAttack = true;
    private Coroutine runningRoutine;
    [SerializeField] private Shield shieldToAssing;
    [SerializeField]
    private Slider lifeSlider;
    public ScriptableShip ship;

    public List<GameObject> unitEnemy = new List<GameObject>();

    private int rewardCoin;
    private int rewardFuel;
    private int rewardMissle;

    private void OnEnable()
    {
        EventManager.EnergyMoved += OnEnergyMoved;
    }

    private void OnDisable()
    {
        EventManager.EnergyMoved -= OnEnergyMoved;
    }

    private void Awake()
    {
        ship = Instantiate(ship);

    }
    private void Start()
    {
        lifeSlider.maxValue = ship.maxLife;
        lifeSlider.value = ship.life;
        AssingRooms();
        if (shieldToAssing != null)
            ship.shiedlScript = shieldToAssing;

        if (ship.shipType == ShipType.ENEMY)
        {
            GameManager.Instance.isFighting = true;
        }
    }


    private void AssingRooms()
    {
        foreach (Rooms room in rooms)
        {
            ship.shipRooms.Add(room);
        }

    }

    private void Update()
    {
        if (!GameManager.Instance.gamePaused)
        {
            if (ship.shipType == ShipType.PLAYER)
            {
                ship.OxygenHandle();
            }
            else
            {
                if (canAttack)
                {
                    canAttack = false;
                    runningRoutine = StartCoroutine(EnemyAttak());
                }
            }
        }
    }
    private void OnEnergyMoved(Statistc statistc, bool arg2)
    {
        ship.OnEnergyMoved(statistc, arg2);
    }

    public void ApplyDamage(int damage)
    {
        int previousHP = ship.life;
        ship.DamagedShip(damage);

        StartCoroutine(AnimateHPDecrease(previousHP, ship.life));
    }


    private void DropReward(int gold, int fuel, int missle)
    {
        gold = Random.Range(6, 15);
        fuel = Random.Range(0, 3);
        missle = Random.Range(0, 2);

        ship.jumps += fuel;
        ship.missle += missle;
        GameManager.Instance.Coin += gold;
        HudManager.Instance.UpdateText();
        EventManager.RewardBattle?.Invoke(gold, fuel, missle);
    }
    // Coroutine per diminuire lo slider in 1 secondo
    public IEnumerator AnimateHPDecrease(int startHP, int endHP)
    {
        float duration = 0.3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            lifeSlider.value = Mathf.Lerp(startHP, endHP, t);
            yield return null;
        }

        lifeSlider.value = endHP;
        if (endHP <= 0)
        {
            if (ship.shipType == ShipType.ENEMY)
            {
                HudManager.Instance.rewardPanel.SetActive(true);
                GameManager.Instance.isFighting = false;
                DropReward(rewardCoin, rewardFuel, rewardMissle);
                ScoreKeeper.Instance.score += 1000;
            }
            else if (ship.shipType == ShipType.BOSS)
            {
                ScoreKeeper.Instance.score += 5000;
                SceneManager.LoadScene("Win");

            }
            else
            {
                SceneManager.LoadScene("Lost");
            }

            Destroy(gameObject);
        }
    }

    IEnumerator EnemyAttak()
    {
        float elapseTime = 0;
        float TimePoint = Random.Range(5, 7);
        while (elapseTime < TimePoint)
        {
            elapseTime += Time.deltaTime;

            if (elapseTime >= TimePoint)
            {
                GameManager.Instance.BattleManager();
                canAttack = true;
            }
            yield return null;
        }
    }

}
