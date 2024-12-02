using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Slider slider;
    public WeaponScriptable weaponScriptable;
    [SerializeField] private int miniumToUse;
    [SerializeField] private int test;
    private Coroutine currentCoroutine;
    [SerializeField] private Ship ship;
    [SerializeField] private TMP_Text textWeapon;
    [SerializeField] private GameObject[] weaponCharge;
    public GameObject selectedEffect;

    private void Start()
    {
        weaponScriptable = Instantiate(weaponScriptable);
        miniumToUse = weaponScriptable.cost;
        weaponScriptable.AssignMaxValue(slider);
        textWeapon.text = weaponScriptable.weaponName;
        SetCost();
    }

    private void OnEnable()
    {
        EventManager.ResetSelected += OnResetSelected;
    }
    private void OnDisable()
    {
        EventManager.ResetSelected -= OnResetSelected;
    }
    private void SetCost()
    {
        for (int i = 0; i < weaponScriptable.cost; i++)
        {
            weaponCharge[i].SetActive(true);
        }
    }
    private void Update()
    {
        HandleShootingBar();

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (weaponScriptable.canShoot)
        {
            EventManager.ResetSelected?.Invoke();
            GameManager.Instance.weaponScript = this.gameObject.GetComponent<WeaponScript>();
            selectedEffect.SetActive(true);
            //GameManager.Instance.activeWeapon = weaponScriptable;
        }

    }

    private void OnResetSelected()
    {
        selectedEffect.SetActive(false);
    }
    public void OnSHOOT()
    {
        if (weaponScriptable.ammoType == AmmoType.LASER)
        {
            weaponScriptable.Shoot();
            OnResetSelected();
            GameManager.Instance.weaponScript = null;
        }
        else if (weaponScriptable.ammoType == AmmoType.MISSLE)
        {
            if (GameManager.Instance.ship.ship.missle > 0)
            {

                weaponScriptable.Shoot();
                OnResetSelected();
                GameManager.Instance.weaponScript = null;
            }
        }

    }
    private void HandleShootingBar()
    {
        if (ship.ship.weaponCharge >= miniumToUse && !weaponScriptable.isAlreadyRunning && !weaponScriptable.isFull)
        {
            //Debug.Log("Qui");
            //weaponScriptable.isAlreadyRunning = true;

            // Se c'è una coroutine in esecuzione, fermala
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                weaponScriptable.isRunningDownNow = false;
            }

            // Avvia la coroutine di ricarica
            currentCoroutine = StartCoroutine(weaponScriptable.RealoadRoutine(slider));

        }
        else if (ship.ship.weaponCharge < miniumToUse && !weaponScriptable.isRunningDownNow)
        {
            weaponScriptable.isAlreadyRunning = false;

            if (weaponScriptable.timePassed > 0)
            {
                // Se c'è una coroutine in esecuzione, fermala
                if (currentCoroutine != null)
                {
                    StopCoroutine(currentCoroutine);
                    weaponScriptable.isAlreadyRunning = false;
                }

                weaponScriptable.isRunningDownNow = true;

                // Avvia la coroutine che scala verso il basso
                currentCoroutine = StartCoroutine(weaponScriptable.SliderRoutine(slider));
            }
        }
    }
}

//IEnumerator TEST()
//{
//    StopCoroutine(weaponScriptable.RealoadRoutine(slider));
//    yield return new WaitForSeconds(0.1f);
//    StartCoroutine(weaponScriptable.SliderRoutine(slider));
//}

