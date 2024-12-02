using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{

    [SerializeField] float speed;
    Vector2[] path;
    int targetIndex;
    public bool isActiveUnit;
    public GameObject unitSelected;
    public UnitScriptable unitScriptable;
    public Slider healthbar;
    public Slider healthbar2;
    public Transform target;
    public GameObject refCanvas;
    public TMP_Text text;

    public Animator animator;
    public float valueX;
    public float valueY;


    /// <summary>
    /// Le unit gestiranno le collisioni con le stanze per il recupero degli Hp
    /// </summary>
    private void OnEnable()
    {
        EventManager.DeactivateUnit += OnDeselectUnit;

    }

    private void OnDisable()
    {
        EventManager.DeactivateUnit -= OnDeselectUnit;
    }

    private void Awake()
    {
        unitScriptable = Instantiate(unitScriptable);
        if (healthbar != null)
        {
            unitScriptable.SetBarHealth(healthbar);
            unitScriptable.SetBarHealth(healthbar2);
        }
        unitScriptable.isDead = false;
    }
    private void Start()
    {
        text.text = unitScriptable.unitName;
        valueX = transform.position.x;
        valueY = transform.position.y;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            unitScriptable.isDead = true;
        }


        if (unitScriptable.isDead)
        {
            Death();
        }

    }


    private void Death()
    {
        GameManager.Instance.ripCount++;
        Debug.Log(GameManager.Instance.ripCount);
        if (GameManager.Instance.ripCount == 3)
        {
            Debug.Log("GameOver");
            SceneManager.LoadScene("Lost");
        }
        refCanvas.SetActive(false);
        Destroy(this.gameObject);

    }
    public void UnitMovement()
    {

        PathRequestManager.RequestPath(transform.position, GameManager.Instance.target.position, OnPathFound);

    }



    public void DamageUnit(int damage, ref int hp)
    {
        hp -= damage;
        StartCoroutine(unitScriptable.UpdateHealthBar(hp, healthbar));
        StartCoroutine(unitScriptable.UpdateHealthBar(hp, healthbar2));
    }


    private void OnDeselectUnit()
    {
        isActiveUnit = false;
        unitSelected.SetActive(false);
    }
    public void OnPathFound(Vector2[] newPath, bool pathSuccessfull)
    {
        if (pathSuccessfull)
        {
            path = newPath;
            targetIndex = 0;  // Resetta targetIndex a 0
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    private void UnitAnimation()
    {
        Vector2 direction = new Vector2(valueX, valueY).normalized;

        // Imposta i parametri nell'animator
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Speed", speed * direction.magnitude); // Usa la velocità per attivare le animazioni
    }

    IEnumerator FollowPath()
    {
        Vector2 currentWaypoint = path[0];
        float threshold = 0.1f;
        GameManager.Instance.isMovingTarget = true;

        while (true)
        {
            if (Vector2.Distance(transform.position, currentWaypoint) < threshold)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    currentWaypoint = (Vector2)GameManager.Instance.target.position;

                    if (Vector2.Distance(transform.position, currentWaypoint) < threshold)
                    {
                        transform.position = currentWaypoint;
                        GameManager.Instance.isMovingTarget = false;
                        GameManager.Instance.activeUnit = null;
                        GameManager.Instance.target = null;

                        // Ferma le animazioni di movimento quando il target è raggiunto
                        animator.SetFloat("Speed", 0f);
                        yield break;
                    }
                }
                else
                {
                    currentWaypoint = path[targetIndex];
                }
            }

            Vector2 direction = (currentWaypoint - (Vector2)transform.position).normalized;

            // Aggiorna i valori per l'animazione
            valueX = direction.x;
            valueY = direction.y;
            UnitAnimation();

            transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
