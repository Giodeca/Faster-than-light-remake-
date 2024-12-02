using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridSector : Singleton<GridSector>
{
    public GameObject nodePrefab;  // Prefab del nodo
    public int rows = 5;           // Numero di righe
    public int cols = 5;           // Numero di colonne
    public float spacing = 10f;    // Spaziatura tra i nodi
    public Vector2 nodeSize = new Vector2(50, 50);  // Dimensione di ciascun nodo

    [SerializeField] private int minDist;
    [SerializeField] private int maxDist;
    [SerializeField] private int square;
    [SerializeField] private int eventArea;
    [SerializeField] private int finalConnection;

    public Color selectedColor = Color.green;
    public Color defaultColor = Color.white;

    private GameObject[,] gridNodes;  // Array 2D per conservare i nodi
    private List<GameObject> gridNodesList;
    private GraphicRaycaster raycaster; // Riferimento al GraphicRaycaster
    private EventSystem eventSystem;   // Riferimento al sistema di eventi per UI
    private GameObject selectedNode = null;  // Nodo attualmente selezionato
    private HashSet<GameObject> excludedNodes;
    private HashSet<GameObject> walkableNodes;
    private HashSet<GameObject> eventNodes;
    private HashSet<GameObject> conditionAreas;
    //private List<GameObject> nodesSelectNode;

    public GameObject activeNode;

    [SerializeField] private GameObject map;
    [SerializeField] private GameObject parentPanel;

    private GameObject lastCreatedNode;
    void Start()
    {
        // Inizializza l'array e gli oggetti necessari per la selezione
        //nodesSelectNode = new List<GameObject>();
        gridNodes = new GameObject[rows, cols];
        excludedNodes = new HashSet<GameObject>();
        walkableNodes = new HashSet<GameObject>();
        eventNodes = new HashSet<GameObject>();
        conditionAreas = new HashSet<GameObject>();
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();

        StartCoroutine(GenerateGridCoroutine());

    }




    //GameObject GetNodeUnderMouse()
    //{
    //    // Configura il puntatore del mouse per il raycast
    //    PointerEventData pointerEventData = new PointerEventData(eventSystem);
    //    pointerEventData.position = Input.mousePosition;

    //    // Esegui il raycast per ottenere gli oggetti UI sotto il puntatore
    //    List<RaycastResult> results = new List<RaycastResult>();
    //    raycaster.Raycast(pointerEventData, results);

    //    // Ritorna il primo GameObject che viene colpito dal raycast
    //    foreach (RaycastResult result in results)
    //    {
    //        if (result.gameObject.CompareTag("NodeSector"))
    //        {
    //            return result.gameObject;
    //        }
    //    }

    //    return null;
    //}


    IEnumerator GenerateGridCoroutine()
    {
        RectTransform parentRect = parentPanel.GetComponent<RectTransform>(); // Usa il parentPanel

        // Calcola la dimensione della griglia totale
        float gridWidth = (nodeSize.x + spacing) * cols - spacing;
        float gridHeight = (nodeSize.y + spacing) * rows - spacing;

        // Posizione di partenza per la griglia
        Vector2 startPos = new Vector2(-gridWidth / 2, gridHeight / 2);

        // Crea i nodi gradualmente
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {

                // Calcola la posizione del nodo
                Vector2 position = startPos + new Vector2(col * (nodeSize.x + spacing), -row * (nodeSize.y + spacing));

                // Instanzia il prefab e assegna il parent come parentPanel
                GameObject node = Instantiate(nodePrefab, parentPanel.transform);

                // Posiziona il nodo
                RectTransform rectTransform = node.GetComponent<RectTransform>();
                rectTransform.sizeDelta = nodeSize;  // Imposta la dimensione del nodo
                rectTransform.anchoredPosition = position;  // Posiziona il nodo

                // Imposta il colore di default del nodo
                Image nodeImage = node.GetComponent<Image>();
                Color initialColor = defaultColor;
                initialColor.a = 0.05f;
                nodeImage.color = initialColor;

                // Assegna il nome al nodo e lo salva nell'array
                node.name = $"Node_{row}_{col}";
                gridNodes[row, col] = node;

                // Aspetta un frame o imposta una pausa per vedere ogni nodo creato gradualmente
                // yield return new WaitForSeconds(0.001f); // Modifica il tempo come preferisci
            }
        }

        // Procedi con il processamento dei nodi
        yield return StartCoroutine(ProcessNodesCoroutine());
        yield return new WaitForSeconds(0.1f);
        FinalTouch();
    }

    // Coroutine per processare i nodi walkable e fare il debug visivo

    IEnumerator ProcessNodesCoroutine()
    {
        int randomCol = 2;
        GameObject firstRowNode = gridNodes[randomCol, 0];
        firstRowNode.GetComponent<Image>().color = Color.green;
        activeNode = firstRowNode;


        GameObject LastRowNode = gridNodes[randomCol, cols - 1];
        LastRowNode.GetComponent<Image>().color = Color.green;
        LastRowNode.GetComponent<SectorOrbit>().edege = EDGE.CHECKPOINT;
        LastRowNode.GetComponent<SectorOrbit>().enemyType = BattleEnemy.BOSS;
        //firstRowNode.GetComponent<SectorOrbit>().sector = this.gameObject.GetComponent<GridSector>();
        if (!walkableNodes.Contains(firstRowNode))
        {
            walkableNodes.Add(firstRowNode);
        }

        Queue<GameObject> nodesToProcess = new Queue<GameObject>();
        HashSet<GameObject> processedNodes = new HashSet<GameObject>();

        nodesToProcess.Enqueue(firstRowNode);
        processedNodes.Add(firstRowNode);

        // Processa gradualmente ogni nodo
        while (nodesToProcess.Count > 0)
        {


            GameObject currentNode = nodesToProcess.Dequeue();
            Vector2Int currentPos = GetNodeGridPosition(currentNode);


            lastCreatedNode = currentNode;


            List<GameObject> neighbors = GetDiagonalNeighbors(currentPos, minDist, maxDist, currentNode);
            foreach (GameObject neighbor in neighbors)
            {
                if (!processedNodes.Contains(neighbor))
                {

                    nodesToProcess.Enqueue(neighbor);
                    processedNodes.Add(neighbor);


                    // Aspetta un momento per ogni passaggio
                    yield return new WaitForSeconds(0.01f); // Modifica il tempo come preferisci
                }
            }

            // Pausa tra i nodi processati
            yield return new WaitForSeconds(0.01f); // Modifica il tempo come preferisci
        }


    }

    public Vector2Int GetNodeGridPosition(GameObject selectedNode)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // Controlla se il nodo selezionato corrisponde a quello nell'array
                if (gridNodes[row, col] == selectedNode)
                {
                    // Ritorna la posizione come Vector2Int
                    return new Vector2Int(row, col);
                }
            }
        }

        // Se il nodo non viene trovato, ritorna un valore invalido (-1, -1)
        return new Vector2Int(-1, -1);
    }

    // Metodo per ottenere il nodo sotto il mouse


    List<GameObject> GetDiagonalNeighbors(Vector2Int currentPos, int minRange, int maxRange, GameObject nodeStart)
    {
        List<GameObject> neighbors = new List<GameObject>();

        if (currentPos.y == cols - 2)
        {
            // Permetti la generazione dei vicini solo se è nella terza riga (indice 2 perché le righe partono da 0)
            if (currentPos.x != 2)
            {
                return neighbors; // Se non è nella terza riga, non generare vicini
            }
        }


        int[,] directions = new int[,]
        {
        { 1, 1 },   // Diagonale in basso a destra
        { -1, 1 },  // Diagonale in alto a destra
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int dirX = directions[i, 0];
            int dirY = directions[i, 1];

            int distance = Random.Range(minRange, maxRange + 1);
            int targetRow = currentPos.x + dirX * distance;
            int targetCol = currentPos.y + dirY * distance;

            if (targetRow >= 0 && targetRow < rows && targetCol >= 0 && targetCol < cols)
            {
                GameObject neighbor = gridNodes[targetRow, targetCol];
                if (!excludedNodes.Contains(neighbor))
                {
                    neighbors.Add(neighbor);

                    if (nodeStart.GetComponent<SectorOrbit>().edege == EDGE.CHECKPOINT)
                    {
                        neighbor.GetComponent<SectorOrbit>().edege = EDGE.CONNECTION;
                        neighbor.GetComponent<Image>().color = Color.blue;
                    }
                    else
                    {
                        neighbor.GetComponent<SectorOrbit>().edege = EDGE.CHECKPOINT;
                        neighbor.GetComponent<Image>().color = Color.red;
                    }

                    // Aggiungi i due nodi creati alla lista pubblica
                    nodeStart.GetComponent<SectorOrbit>().connection.Add(neighbor);
                    neighbor.GetComponent<SectorOrbit>().connection.Add(nodeStart);
                    excludedNodes.Add(neighbor);
                    walkableNodes.Add(neighbor);
                }
            }
        }

        return neighbors;
    }

    private void FinalTouch()
    {
        foreach (GameObject obj in gridNodes)
        {
            Vector2Int currentPos = GetNodeGridPosition(obj);

            foreach (GameObject ob in GetNeighbors(currentPos))
            {
                if (ob.GetComponent<SectorOrbit>().edege == EDGE.CONNECTION && !obj.GetComponent<SectorOrbit>().connection.Contains(ob))
                {
                    obj.GetComponent<SectorOrbit>().connection.Add(ob);
                }
            }
        }

    }
    List<GameObject> GetNeighbors(Vector2Int currentPos)
    {
        List<GameObject> neighbors = new List<GameObject>();

        int[,] directions = new int[,]
        {
        { 1, 0 },   // Giù
        { -1, 0 },  // Su
        { 0, 1 },   // Destra
        { 0, -1 },  // Sinistra
        { 1, 1 },   // Diagonale in basso a destra
        { 1, -1 },  // Diagonale in basso a sinistra
        { -1, 1 },  // Diagonale in alto a destra
        { -1, -1 }  // Diagonale in alto a sinistra
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int newRow = currentPos.x + directions[i, 0];
            int newCol = currentPos.y + directions[i, 1];

            if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
            {
                neighbors.Add(gridNodes[newRow, newCol]);
            }
        }

        return neighbors;
    }



}
