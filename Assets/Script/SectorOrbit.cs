using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum EDGE
{
    CHECKPOINT, CONNECTION
}
public class SectorOrbit : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GridSector sector;
    public EDGE edege;
    public List<GameObject> connection = new List<GameObject>();
    public BattleEnemy enemyType;
    public GameObject textPopUp;

    private void Start()
    {
        AssignEnemeyType();
    }

    private void AssignEnemeyType()
    {
        int random = Random.Range(0, 2);
        switch (random)
        {
            case 0:
                enemyType = BattleEnemy.ENEMY1;
                break;
            case 1:
                enemyType = BattleEnemy.ENEMY2;
                break;
            case 2:
                enemyType = BattleEnemy.ENEMY3;
                break;
        }
    }
    public bool CheckAllowToMove(GameObject nodeStart)
    {
        List<GameObject> startConnection = nodeStart.GetComponent<SectorOrbit>().connection;

        foreach (GameObject connectionNode in startConnection)
        {
            if (connection.Contains(connectionNode))
            { return true; }
        }

        return false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (edege == EDGE.CHECKPOINT && CheckAllowToMove(GridSector.Instance.activeNode))
        {
            //Fai Qualcosa 
            GameManager.Instance.enemyType = this.enemyType;
            GridSector.Instance.activeNode = this.gameObject;
            GridCreator.Instance.panelSector.SetActive(false);
            EventManager.CallNewArea?.Invoke();
            if (textPopUp.activeSelf)
            {
                textPopUp.SetActive(false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (edege == EDGE.CHECKPOINT)
            textPopUp.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (edege == EDGE.CHECKPOINT)
            textPopUp.SetActive(false);
    }
}
