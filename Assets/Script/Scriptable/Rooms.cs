using System.Collections.Generic;
using UnityEngine;

public class Rooms : AActions<RoomsScriptable>
{
    public RoomsScriptable scriptable;
    public bool UpdatedOxygen;

    private float elapsedTime = 0f;
    private float incrementInterval = 1f;

    private int HitMissleCount = 0;
    [SerializeField] private int hitBeforeFire = 2;
    [SerializeField] private int hitBeforeHole = 3;
    private int HitLaserCount = 0;
    public List<GameObject> pavmentScripts = new List<GameObject>();
    private int healCount;
    private bool isReseted;

    private bool oxygenUpdateInProgress;
    private void Awake()
    {
        scriptable = Instantiate(scriptable);
    }
    private void OnEnable()
    {
        EventManager.Nebulosa += OnNebulosa;
        EventManager.ResetNebulosa += OnResetRoomColorNebulosa;
    }

    private void OnDisable()
    {
        EventManager.Nebulosa -= OnNebulosa;
        EventManager.ResetNebulosa -= OnResetRoomColorNebulosa;
    }



    private void Update()
    {
        // Update the elapsed time
        elapsedTime += Time.deltaTime;

        // Check if it's time to manage oxygen levels
        if (elapsedTime >= incrementInterval)
        {
            OxygenLevelManager();
            elapsedTime = 0f; // Reset the elapsed time after processing
        }

        // Check for input
        if (Input.GetKeyDown(KeyCode.L))
        {
            scriptable.roomStatus = RoomStatus.ONFIRE;
            OnFireRoomColor();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            scriptable.oxygenRoomLevel = 0;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            scriptable.oxygenRoomLevel = 10;
        }
    }

    public override void ActionImplementation(int stat, RoomsScriptable value)
    {

    }

    private void OxygenLevelManager()
    {
        if (!UpdatedOxygen)
        {
            if (scriptable.roomStatus != RoomStatus.OK && scriptable.oxygenRoomLevel > 0)
            {
                scriptable.oxygenRoomLevel--;
                UpdatedOxygen = true;
            }
            else if (GameManager.Instance.ship.ship.oxygenPercentual == 0)
            {
                scriptable.roomStatus = RoomStatus.OK;
                OnLowOxygen();
                UpdatedOxygen = true;
            }
            else if (scriptable.roomStatus == RoomStatus.OK && GameManager.Instance.ship.ship.oxygenPercentual >= scriptable.oxygenRoomLevel)
            {

                scriptable.oxygenRoomLevel++;
                if (scriptable.oxygenRoomLevel < 30)
                {
                    OnLowOxygen();
                }

                UpdatedOxygen = true;
            }
            else if (scriptable.roomStatus != RoomStatus.OK && GameManager.Instance.ship.ship.oxygenPercentual < scriptable.oxygenRoomLevel)
            {
                if (scriptable.oxygenRoomLevel > 0)
                    scriptable.oxygenRoomLevel--;



                UpdatedOxygen = true;
            }
        }
        else
            UpdatedOxygen = false;

    }


    public override void Broken(int stat, RoomsScriptable value, int damage)
    {
        stat -= damage;
        if (stat <= value.roomHPUsable && value.roomStatus == RoomStatus.OK)
        {
            bool setFire = Random.Range(0f, 1f) > 0.5;
            if (setFire)
                value.roomStatus = RoomStatus.ONFIRE;
            else
                value.roomStatus = RoomStatus.HASHOLE;
        }

        if (stat <= 0)
            stat = 0;

        value.roomHP = stat;
    }

    public override void Repair(int stat, RoomsScriptable value)
    {
        stat += value.repair;
        if (stat >= value.roomHPUsable)
        {
            value.roomStatus = RoomStatus.OK;
            if (scriptable.oxygenRoomLevel > 30)
                OnReset();
        }

        if (stat >= value.capStat)
            stat = value.capStat;

        value.roomHP = stat;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Unit"))
        {
            if (GameManager.Instance.conditionType == CONDITIONTYPE.NEBULOSA && !isReseted)
            {
                OnResetRoomColorNebulosa();
                isReseted = true;
            }

            if (scriptable.roomStatus == RoomStatus.OK)
            {
                if (scriptable.isUsableAbility)
                {
                    scriptable.RoomEffect(scriptable);
                }

                elapsedTime += Time.deltaTime;

                if (elapsedTime >= incrementInterval)
                {
                    if (scriptable.roomType == RoomType.INFERMERY && scriptable.roomStatus == RoomStatus.OK)
                    {
                        collision.gameObject.GetComponent<Unit>().unitScriptable.hpUnit += GameManager.Instance.ship.ship.healRate;
                        StartCoroutine(collision.gameObject.GetComponent<Unit>().unitScriptable.UpdateHealthBar(collision.gameObject.GetComponent<Unit>().unitScriptable.hpUnit, collision.gameObject.GetComponent<Unit>().healthbar));
                        StartCoroutine(collision.gameObject.GetComponent<Unit>().unitScriptable.UpdateHealthBar(collision.gameObject.GetComponent<Unit>().unitScriptable.hpUnit, collision.gameObject.GetComponent<Unit>().healthbar2));
                    }
                    elapsedTime = 0f;
                    Repair(scriptable.roomHP, scriptable);
                }
            }
            else
            {
                if (scriptable.roomStatus == RoomStatus.ONFIRE || scriptable.oxygenRoomLevel < 20)
                {
                    elapsedTime += Time.deltaTime;
                    //Debug.Log(scriptable.roomType);
                    if (elapsedTime >= incrementInterval)
                    {
                        elapsedTime = 0f;
                        collision.gameObject.GetComponent<Unit>().unitScriptable.Damage(1, ref collision.gameObject.GetComponent<Unit>().unitScriptable.hpUnit);
                        StartCoroutine(collision.gameObject.GetComponent<Unit>().unitScriptable.UpdateHealthBar(collision.gameObject.GetComponent<Unit>().unitScriptable.hpUnit, collision.gameObject.GetComponent<Unit>().healthbar));
                        StartCoroutine(collision.gameObject.GetComponent<Unit>().unitScriptable.UpdateHealthBar(collision.gameObject.GetComponent<Unit>().unitScriptable.hpUnit, collision.gameObject.GetComponent<Unit>().healthbar2));
                        Repair(scriptable.roomHP, scriptable);
                    }

                }

            }

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Unit"))
        {
            if (GameManager.Instance != null && GameManager.Instance.conditionType == CONDITIONTYPE.NEBULOSA)
            {
                if (pavmentScripts.Count > 0)
                {
                    OnNebulosa();
                }

                isReseted = false;
            }
            elapsedTime = 0f;
            scriptable.RoomEffectExit(scriptable);
        }
    }



    private void OnNebulosa()
    {
        OnObscureRoomColor();
    }
    public void HitCounter(ProjectileType typeOfProjectile)
    {
        if (typeOfProjectile == ProjectileType.MISSLE)
        {
            HitMissleCount++;
            if (HitMissleCount >= hitBeforeFire && scriptable.roomStatus != RoomStatus.HASHOLE)
            {
                scriptable.roomHP = 30;
                scriptable.roomStatus = RoomStatus.ONFIRE;
                OnFireRoomColor();
            }
        }
        else if (typeOfProjectile == ProjectileType.LASER)
        {
            HitLaserCount++;
            if (HitMissleCount >= hitBeforeHole && scriptable.roomStatus != RoomStatus.ONFIRE)
            {
                scriptable.roomHP = 30;
                scriptable.roomStatus = RoomStatus.HASHOLE;
                OnHoleRoomColor();
            }
        }
    }

    public void OnResetRoomColorNebulosa()
    {
        Debug.Log("Here");
        foreach (GameObject obj in pavmentScripts)
        {
            obj.GetComponent<PavmentScript>().ResetColorObscure();
        }
    }

    public void OnLowOxygen()
    {
        foreach (GameObject obj in pavmentScripts)
        {
            obj.GetComponent<PavmentScript>().OxygenLow();
        }
    }
    public void OnFireRoomColor()
    {
        scriptable.roomHP = 30;
        foreach (GameObject obj in pavmentScripts)
        {

            obj.GetComponent<PavmentScript>().RoomOnFire();
        }
    }
    public void OnHoleRoomColor()
    {
        scriptable.roomHP = 30;
        foreach (GameObject obj in pavmentScripts)
        {
            obj.GetComponent<PavmentScript>().RoomHole();
        }
    }
    public void OnObscureRoomColor()
    {
        foreach (GameObject obj in pavmentScripts)
        {
            obj.GetComponent<PavmentScript>().RoomObscure();
        }
    }
    public void OnReset()
    {
        foreach (GameObject obj in pavmentScripts)
        {
            obj.GetComponent<PavmentScript>().ResetColor();
        }
    }


    public void ResetHitCounter()
    {
        HitMissleCount = 0;
        HitLaserCount = 0;
    }




}
