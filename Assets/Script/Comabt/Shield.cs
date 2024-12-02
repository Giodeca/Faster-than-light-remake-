using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;
    public float realoadTime;
    public float timePassed;
    public bool isAlreadyRunning;
    private bool isShieldUp;
    [SerializeField] private int shieldHp;
    [SerializeField] private int MaxshieldHp;
    [SerializeField] private int shieldHpUpgrade;
    [SerializeField] private BoxCollider2D shield;
    [SerializeField] private BoxCollider2D shieldSide;

    [SerializeField] private GameObject shieldView;
    [SerializeField] private GameObject[] shiedlCount;
    [SerializeField] private int activeShiedlCount;
    [SerializeField] private bool isEnemy;



    private void Start()
    {
        if (!isEnemy)
        {
            loadingBar.maxValue = realoadTime;
            activeShiedlCount = 0;
            shiedlCount[0].SetActive(true);
        }

    }
    private void Update()
    {
        if (shieldHp <= 0 && !isAlreadyRunning && !isEnemy)
        {
            isAlreadyRunning = true;
            StartCoroutine(RealoadRoutine(loadingBar));
        }
        if (shieldHp <= 0 && !isAlreadyRunning && isEnemy)
        {
            isAlreadyRunning = true;
            StartCoroutine(RealoadRoutineEnemy());
        }
    }

    public void UpdateShield()
    {
        if (!isEnemy)
        {
            MaxshieldHp += shieldHpUpgrade;
            shieldHp += shieldHpUpgrade;
            activeShiedlCount++;
            if (activeShiedlCount >= shiedlCount.Length)
            {
                activeShiedlCount = shiedlCount.Length;
            }
            if (shiedlCount[activeShiedlCount].activeSelf != true)
                shiedlCount[activeShiedlCount].SetActive(true);
        }


    }
    public void DownGradeShield()
    {
        MaxshieldHp -= shieldHpUpgrade;
        shieldHp -= shieldHpUpgrade;
        shiedlCount[activeShiedlCount].SetActive(false);
        activeShiedlCount--;


    }
    public void DestroyShield(int damageShidl)
    {
        shieldHp -= damageShidl;
        if (shieldHp <= 0)
        {
            if (activeShiedlCount > 0)
            {
                activeShiedlCount--;
            }

            if (!isEnemy)
            {
                shiedlCount[activeShiedlCount].SetActive(true);
            }



            timePassed = 0;
            shieldView.SetActive(false);
            shieldHp = 0;
            isShieldUp = false;
            shield.enabled = false;

            if (shieldSide != null)
                shieldSide.enabled = false;
        }
    }
    public void AssignMaxValue(Slider loadingBar)
    {
        loadingBar.maxValue = realoadTime;
    }
    public IEnumerator RealoadRoutine(Slider loadingBar)
    {

        Debug.Log("Iniziata ReloadRoutine");
        while (timePassed < realoadTime)
        {

            timePassed += Time.deltaTime;
            loadingBar.value = timePassed;
            //elapsedTime += Time.deltaTime;
            //if (elapsedTime >= incrementInterval)
            //{
            //    timePassed++;
            //    elapsedTime = 0f;
            //}

            if (timePassed >= realoadTime)
            {
                shieldHp = MaxshieldHp;
                isShieldUp = true;
                timePassed = realoadTime;
                shield.enabled = true;

                if (shieldSide != null)
                    shieldSide.enabled = true;
                isAlreadyRunning = false;
                shieldView.SetActive(true);
                Debug.Log("Sono qui Bello");
            }
            yield return null;
        }

    }

    public IEnumerator RealoadRoutineEnemy()
    {

        //Debug.Log("Iniziata ReloadRoutine Enemy");
        while (timePassed < realoadTime)
        {

            timePassed += Time.deltaTime;
            //elapsedTime += Time.deltaTime;
            //if (elapsedTime >= incrementInterval)
            //{
            //    timePassed++;
            //    elapsedTime = 0f;
            //}

            if (timePassed >= realoadTime)
            {
                shieldHp = MaxshieldHp;
                isShieldUp = true;
                timePassed = realoadTime;
                shield.enabled = true;
                if (shieldSide != null)
                    shieldSide.enabled = true;
                isAlreadyRunning = false;
                shieldView.SetActive(true);
                Debug.Log("Sono qui Bello");
            }
            yield return null;
        }

    }
}
