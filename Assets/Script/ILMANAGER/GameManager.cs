using System.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class GameManager : Singleton<GameManager>
{
    //Debug Comand R for DeadShip
    // if (Input.GetKeyDown(KeyCode.L))
    //        {
    //            scriptable.roomStatus = RoomStatus.ONFIRE;
    //            OnFireRoomColor();
    //}
    //if (Input.GetKeyDown(KeyCode.T))
    //{
    //    scriptable.oxygenRoomLevel = 0;
    //}
    //if (Input.GetKeyDown(KeyCode.O))
    //{
    //    scriptable.oxygenRoomLevel = 10;
    //}


    [SerializeField] private GameObject map;

    [Header("Unit")]
    public GameObject activeUnit;
    public Transform target;
    public bool isMovingTarget;
    public bool someoneIsPiloting;
    GameObject saveHit;

    public Transform targetPosEnemy;
    public Ship ship;
    public Ship enemyActiveShip;
    public int Coin;

    [SerializeField] private Transform ProjectileSpwan;
    [SerializeField] private Transform ProjectileSpwanPlayer;
    [SerializeField] private GameObject[] projectile;


    [SerializeField] private Transform ConditionSpawnPoint;


    public WeaponScript weaponScript;
    public WeaponScriptable activeWeapon;
    //public Transform attackPoint;

    public Transform spawnPointShip;
    public GameObject[] enemyShipToSpawn;

    public bool gamePaused;
    public PlanetType planetType;
    public CONDITIONTYPE conditionType;
    public CONDITIONTYPE previousConditionType;
    public bool conditionIsCoocking = true;
    public bool isFighting;
    Coroutine attackWeather;
    public BattleEnemy enemyType;
    public int ripCount;
    protected override void Awake()
    {
        base.Awake();
        EventManager.SpawnShip += OnStartBattle;
        EventManager.callEvent += OnCallEvent;

        EventManager.StartFight += OnFightStarted;
        EventManager.Runway += OnRunwayStarted;
        EventManager.NothingHappen += OnNothingHappened;
        EventManager.GetReward += OnRewardReceived;
        EventManager.Recruit += OnRecruitment;
        EventManager.PayToRepair += OnPaymentForRepair;
        EventManager.PayToDiscover += OnPaymentForDiscovery;
    }

    private void OnDisable()
    {
        EventManager.SpawnShip -= OnStartBattle;
        EventManager.callEvent -= OnCallEvent;

        EventManager.StartFight -= OnFightStarted;

        EventManager.Runway -= OnRunwayStarted;
        EventManager.NothingHappen -= OnNothingHappened;
        EventManager.GetReward -= OnRewardReceived;
        EventManager.Recruit -= OnRecruitment;
        EventManager.PayToRepair -= OnPaymentForRepair;
        EventManager.PayToDiscover -= OnPaymentForDiscovery;
    }
    private void Update()
    {
        if (!gamePaused)
        {
            SelectActiveUnit();
            MoveUnit();

            if (conditionIsCoocking && conditionType != CONDITIONTYPE.NONE && conditionType != CONDITIONTYPE.NEBULOSA)
            {
                conditionIsCoocking = false;
                attackWeather = StartCoroutine(ConditionAttak());
            }

            if (attackWeather != null && conditionType == CONDITIONTYPE.NONE)
            {
                StopCoroutine(attackWeather);
                attackWeather = null;
            }


        }


        // debugButton
        if (Input.GetKeyDown(KeyCode.R) && enemyActiveShip != null)
        {
            enemyActiveShip.ship.life = 1;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            enemyType = BattleEnemy.BOSS;
        }

    }

    public void OnCallEvent()
    {
        gamePaused = true;
    }
    public void BattleManager()
    {
        int randomShoot = Random.Range(0, ship.shipNodes.Count);
        GameObject projectileLanch;
        if (enemyActiveShip.ship.missle > 0)
        {
            bool randomProjectile = Random.Range(0f, 1f) < 0.4f;
            if (randomProjectile)
            {
                Debug.Log(projectile[1]);
                projectileLanch = Instantiate(projectile[1], ProjectileSpwan.position, Quaternion.identity);
                enemyActiveShip.ship.missle--;
            }
            else
            {
                Debug.Log(projectile[0]);
                projectileLanch = Instantiate(projectile[0], ProjectileSpwan.position, Quaternion.identity);
            }
        }
        else
        {
            Debug.Log(projectile[0]);
            projectileLanch = Instantiate(projectile[0], ProjectileSpwan.position, Quaternion.identity);
        }
        Vector2 targetPoint = ship.shipNodes[randomShoot].worldPosition;
        StartCoroutine(projectileLanch.GetComponent<Projectile>().MovingObject(projectileLanch.transform.position, targetPoint));
    }

    private void OnStartBattle()
    {
        switch (enemyType)
        {
            case BattleEnemy.ENEMY1:
                GameObject obj = Instantiate(enemyShipToSpawn[0], spawnPointShip.position, Quaternion.identity);
                enemyActiveShip = obj.GetComponent<Ship>();
                break;
            case BattleEnemy.ENEMY2:
                GameObject obj1 = Instantiate(enemyShipToSpawn[1], spawnPointShip.position, Quaternion.identity);
                enemyActiveShip = obj1.GetComponent<Ship>();
                break;
            case BattleEnemy.ENEMY3:
                GameObject obj2 = Instantiate(enemyShipToSpawn[2], spawnPointShip.position, Quaternion.identity);
                enemyActiveShip = obj2.GetComponent<Ship>();
                break;
            case BattleEnemy.BOSS:
                GameObject obj3 = Instantiate(enemyShipToSpawn[3], spawnPointShip.position, Quaternion.identity);
                enemyActiveShip = obj3.GetComponent<Ship>();
                break;
        }

    }

    private void SelectActiveUnit()
    {
        if (!isMovingTarget)
        {
            if (Input.GetMouseButtonDown(0)) // Tasto sinistro del mouse
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hitRoom = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Room"));

                if (hitRoom.collider != null)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, Mathf.Infinity);

                    foreach (var hit in hits)
                    {
                        if (hit.collider != hitRoom.collider && hit.collider.gameObject.CompareTag("Unit"))
                        {
                            activeUnit = hit.collider.gameObject;
                            activeUnit.GetComponent<Unit>().unitSelected.SetActive(true);
                            activeWeapon = null;
                            if (activeUnit != saveHit && saveHit != null)
                            {
                                saveHit.GetComponent<Unit>().unitSelected.SetActive(false);
                            }
                            saveHit = hit.collider.gameObject;
                            break;
                        }
                        if (hit.collider != hitRoom.collider && hit.collider.gameObject.CompareTag("PavmentEnemy") && activeWeapon != null)
                        {
                            if (weaponScript != null)
                            {
                                activeUnit = null;
                                Vector2 targetPoint = hit.collider.transform.position;
                                GameObject projectileLanch;
                                switch (weaponScript.weaponScriptable.prj.type)
                                {
                                    case ProjectileType.MISSLE:
                                        projectileLanch = Instantiate(projectile[1], ProjectileSpwanPlayer.position, Quaternion.identity);
                                        StartCoroutine(projectileLanch.GetComponent<Projectile>().MovingObject(projectileLanch.transform.position, targetPoint));
                                        break;
                                    case ProjectileType.LASER:
                                        projectileLanch = Instantiate(projectile[0], ProjectileSpwanPlayer.position, Quaternion.identity);
                                        StartCoroutine(projectileLanch.GetComponent<Projectile>().MovingObject(projectileLanch.transform.position, targetPoint));
                                        break;
                                    case ProjectileType.SUN:
                                        projectileLanch = Instantiate(projectile[2], ProjectileSpwanPlayer.position, Quaternion.identity);
                                        StartCoroutine(projectileLanch.GetComponent<Projectile>().MovingObject(projectileLanch.transform.position, targetPoint));
                                        break;
                                    case ProjectileType.METEOR:
                                        projectileLanch = Instantiate(projectile[3], ProjectileSpwanPlayer.position, Quaternion.identity);
                                        StartCoroutine(projectileLanch.GetComponent<Projectile>().MovingObject(projectileLanch.transform.position, targetPoint));
                                        break;
                                }
                                weaponScript.OnSHOOT();
                            }
                            break;
                        }
                        if (hit.collider != hitRoom.collider && hit.collider.gameObject.CompareTag("Door"))
                        {
                            hit.collider.gameObject.GetComponent<Doors>().OpenTheDoor();
                            break;
                        }
                    }
                }
                else
                {
                    RaycastHit2D hitOther = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity);

                    if (hitOther.collider != null && hitOther.collider.gameObject.CompareTag("Unit"))
                    {
                        activeUnit = hitOther.collider.gameObject;
                        activeUnit.GetComponent<Unit>().unitSelected.SetActive(true);

                        if (activeUnit != saveHit && saveHit != null)
                        {
                            saveHit.GetComponent<Unit>().unitSelected.SetActive(false);
                        }
                        saveHit = hitOther.collider.gameObject;
                    }
                    else if (hitOther.collider != null && hitOther.collider.gameObject.CompareTag("PavmentEnemy"))
                    {
                        if (weaponScript != null)
                        {
                            activeUnit = null;
                            Vector2 targetPoint = hitOther.collider.transform.position;
                            GameObject projectileLanch;
                            switch (weaponScript.weaponScriptable.prj.type)
                            {
                                case ProjectileType.MISSLE:
                                    projectileLanch = Instantiate(projectile[1], ProjectileSpwanPlayer.position, Quaternion.identity);
                                    StartCoroutine(projectileLanch.GetComponent<Projectile>().MovingObject(projectileLanch.transform.position, targetPoint));
                                    break;
                                case ProjectileType.LASER:
                                    projectileLanch = Instantiate(projectile[0], ProjectileSpwanPlayer.position, Quaternion.identity);
                                    StartCoroutine(projectileLanch.GetComponent<Projectile>().MovingObject(projectileLanch.transform.position, targetPoint));
                                    break;
                                case ProjectileType.SUN:
                                    projectileLanch = Instantiate(projectile[2], ProjectileSpwanPlayer.position, Quaternion.identity);
                                    StartCoroutine(projectileLanch.GetComponent<Projectile>().MovingObject(projectileLanch.transform.position, targetPoint));
                                    break;
                                case ProjectileType.METEOR:
                                    projectileLanch = Instantiate(projectile[3], ProjectileSpwanPlayer.position, Quaternion.identity);
                                    StartCoroutine(projectileLanch.GetComponent<Projectile>().MovingObject(projectileLanch.transform.position, targetPoint));
                                    break;
                            }
                            weaponScript.OnSHOOT();
                        }
                    }
                    else if (hitOther.collider != hitRoom.collider && hitOther.collider.gameObject.CompareTag("Door"))
                        hitOther.collider.gameObject.GetComponent<Doors>().OpenTheDoor();
                    else
                    {
                        if (activeUnit != null)
                        {
                            activeUnit.GetComponent<Unit>().isActiveUnit = false;
                            saveHit = null;
                        }
                        activeUnit = null;
                        EventManager.DeactivateUnit?.Invoke();
                    }
                }
            }
        }
    }
    //Condition  Areas
    private void MoveUnit()
    {
        if (!isMovingTarget)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                RaycastHit2D hitRoom = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Room"));

                if (hitRoom.collider != null)
                {

                    RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, Mathf.Infinity);

                    foreach (var hit in hits)
                    {
                        if (hit.collider != hitRoom.collider && hit.collider.gameObject.CompareTag("Pavment") && activeUnit != null)
                        {
                            target = hit.collider.gameObject.transform;
                            activeUnit.GetComponent<Unit>().UnitMovement();
                            break;
                        }

                    }
                }
                else
                {

                    RaycastHit2D hitOther = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity);

                    if (hitOther.collider != null && hitOther.collider.gameObject.CompareTag("Pavment") && activeUnit != null)
                    {
                        target = hitOther.collider.gameObject.transform;
                        activeUnit.GetComponent<Unit>().UnitMovement();
                    }

                }
            }
        }
    }

    IEnumerator ConditionAttak()
    {
        float elapseTime = 0;
        float TimePoint = Random.Range(6, 8);
        while (elapseTime < TimePoint)
        {
            elapseTime += Time.deltaTime;

            if (elapseTime >= TimePoint)
            {
                ConditionManager();
                conditionIsCoocking = true;
            }
            yield return null;
        }
    }

    public void ConditionManager()
    {
        int randomShoot = Random.Range(0, ship.shipNodes.Count);

        if (conditionType == CONDITIONTYPE.METEOR)
        {
            Vector2 targetPoint = ship.shipNodes[randomShoot].worldPosition;
            GameObject projectileLanch = Instantiate(projectile[3], ConditionSpawnPoint.position, Quaternion.identity);
            Debug.Log(targetPoint);
            StartCoroutine(projectileLanch.GetComponent<Projectile>().MovingObject(projectileLanch.transform.position, targetPoint));
        }
        else if (conditionType == CONDITIONTYPE.SUN)
        {
            Vector2 targetPoint = ship.shipNodes[randomShoot].worldPosition;
            GameObject projectileLanch = Instantiate(projectile[2], ConditionSpawnPoint.position, Quaternion.identity);
            Debug.Log(targetPoint);
            StartCoroutine(projectileLanch.GetComponent<Projectile>().MovingObject(projectileLanch.transform.position, targetPoint));
        }
    }

    public void Jump()
    {
        if (ship.ship.jumps > 0 && someoneIsPiloting && ship.ship.enginePower > 0 && !gamePaused && !isFighting)
        {
            map.SetActive(true);
            //jumps--;
        }
    }

    private void OnFightStarted()
    {

        if (planetType != PlanetType.BATTLE)
            OnStartBattle();

    }


    private void OnRunwayStarted(int cost, int materialToPay)
    {
        materialToPay -= cost;
        Coin -= cost;
        Debug.Log("Rescue or runway started!");
    }

    private void OnNothingHappened(bool nonSo, int cost, int materialToPay)
    {
        Debug.Log("Nothing happened...");
        // Logica per quando non accade nulla
    }

    private void OnRewardReceived(int cost, int materialToPay)
    {
        Debug.Log("Reward received!");
        materialToPay -= cost;
        Coin += cost;
        // Logica per la ricezione di una ricompensa
    }

    private void OnRecruitment()
    {
        Debug.Log("New recruit!");
        // Logica per il reclutamento
    }

    private void OnPaymentForRepair(int cost, int materialToPay)
    {
        Debug.Log("Paid to repair!");
        materialToPay -= cost;
        Coin -= cost;
        // Logica per pagare la riparazione
    }

    private void OnPaymentForDiscovery(int cost, int materialToPay)
    {
        Debug.Log("Paid to discover!");
        materialToPay -= cost;
        Coin -= cost;
        // Logica per pagare la scoperta, se utilizzato
    }



}
