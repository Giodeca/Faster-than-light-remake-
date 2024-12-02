using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class GridCreator : Singleton<GridCreator>
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


    [SerializeField] private GameObject map;
    [SerializeField] private GameObject parentPanel;

    public ActivateOrbit activeOrbit;
    public GameObject panelSector;

    private GameObject lastCreatedNode;

    protected override void OnEnable()
    {
        base.OnEnable();

        EventManager.CallNewArea += OnCallNewArea;
    }
    void OnDisable()
    {
        EventManager.CallNewArea -= OnCallNewArea;
    }
    void Start()
    {
        SetUp();
    }
    private void SetUp()
    {
        gridNodes = new GameObject[rows, cols];
        excludedNodes = new HashSet<GameObject>();
        walkableNodes = new HashSet<GameObject>();
        eventNodes = new HashSet<GameObject>();
        conditionAreas = new HashSet<GameObject>();
        raycaster = GetComponent<GraphicRaycaster>();
        eventSystem = FindObjectOfType<EventSystem>();

        StartCoroutine(GenerateGridCoroutine());
    }
    void Update()
    {
        // Controlla se è stato cliccato un nodo
        if (Input.GetMouseButtonDown(0))
        {
            GameObject clickedNode = GetNodeUnderMouse();
            if (clickedNode != null && clickedNode.CompareTag("Node"))
            {
                SelectNode(clickedNode);
                if (clickedNode.GetComponent<ActivateOrbit>().isExit == true)
                    activeOrbit = (ActivateOrbit)clickedNode.GetComponent<ActivateOrbit>();
                else
                    activeOrbit = null;

                GameManager.Instance.ship.ship.jumps--;
            }
        }
    }
    public void OpenSectorButton()
    {
        if (activeOrbit != null)
        {
            panelSector.SetActive(true);
        }
    }
    private void OnCallNewArea()
    {
        foreach (GameObject node in gridNodes)
        {
            Destroy(node);
        }
        SetUp();
    }
    public void HoverNode(GameObject obj)
    {

        if (obj != null && walkableNodes.Contains(obj))
        {
            // Resetta i colori dei nodi prima di procedere (opzionale)
            ResetNodeColors();

            // Trova il percorso più breve tra il nodo selezionato e i nodi collegati
            foreach (GameObject connectedNode in obj.GetComponent<ActivateOrbit>().nodesConnected)
            {
                // Trova il percorso usando i nodi della griglia
                List<GameObject> path = FindShortestPathInGrid(obj, connectedNode);
                //if (selectedNode == obj)
                //{
                //    nodesSelectNode = FindShortestPathInGrid(obj, connectedNode);
                //}
                // Colora il percorso trovato
                if (path != null)
                {
                    ColorPath(path);
                }
            }
        }
    }

    public IEnumerator GenerateGridCoroutine()
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
                /* yield return new WaitForSeconds(0.001f)*/ // Modifica il tempo come preferisci
            }
        }

        // Procedi con il processamento dei nodi
        yield return StartCoroutine(ProcessNodesCoroutine());
        yield return StartCoroutine(ProcessWalkableNodes());
    }

    // Coroutine per processare i nodi walkable e fare il debug visivo
    IEnumerator ProcessWalkableNodes()
    {
        foreach (GameObject walkableNode in walkableNodes)
        {
            Vector2Int nodePos = GetNodeGridPosition(walkableNode);

            // Escludi i vicini di questo nodo in un quadrato di raggio 6
            AddNeighbors(nodePos, finalConnection, walkableNode);

            // Aspetta un momento per vedere ogni esclusione
            yield return new WaitForSeconds(0.01f); // Modifica il tempo come preferisci

            // Esegui un debug visivo: Cambia colore dei nodi esclusi intorno al nodo camminabile
            //foreach (GameObject excludedNode in excludedNodes)
            //{
            //    Debug.Log($"Nodo escluso intorno al nodo walkable: {excludedNode.name}");
            //    // Cambia il colore del nodo escluso per il debug visivo
            //    Image nodeImage = excludedNode.GetComponent<Image>();
            //    nodeImage.color = Color.red;
            //}

            // Aspetta un po' tra le iterazioni per vedere il debug visivo
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator ProcessNodesCoroutine()
    {
        int randomCol = Random.Range(5, 15);
        GameObject firstRowNode = gridNodes[randomCol, 1];
        firstRowNode.GetComponent<ActivateOrbit>().type = PlanetType.ENTER;
        selectedNode = firstRowNode;
        firstRowNode.GetComponent<ActivateOrbit>()._gameObject.SetActive(true);
        firstRowNode.GetComponent<ActivateOrbit>().isEnter = true;
        firstRowNode.GetComponent<ActivateOrbit>().SetStatus();

        if (!walkableNodes.Contains(firstRowNode))
        {
            walkableNodes.Add(firstRowNode);
        }

        // Cambia il colore del nodo selezionato in verde
        Image firstRowNodeImage = firstRowNode.GetComponent<Image>();
        firstRowNodeImage.color = Color.green;

        Queue<GameObject> nodesToProcess = new Queue<GameObject>();
        HashSet<GameObject> processedNodes = new HashSet<GameObject>();

        nodesToProcess.Enqueue(firstRowNode);
        processedNodes.Add(firstRowNode);

        // Processa gradualmente ogni nodo
        while (nodesToProcess.Count > 0)
        {
            GameObject currentNode = nodesToProcess.Dequeue();
            Vector2Int currentPos = GetNodeGridPosition(currentNode);

            // Aggiorna la variabile dell'ultimo nodo creato
            lastCreatedNode = currentNode;
            //Debug.Log($"Elaborazione nodo: {currentNode.name} alla posizione {currentPos}");

            List<GameObject> neighbors = GetDiagonalNeighbors(currentPos, minDist, maxDist, currentNode);

            foreach (GameObject neighbor in neighbors)
            {
                if (!processedNodes.Contains(neighbor))
                {
                    neighbor.GetComponent<ActivateOrbit>().PlanetTypeAssignation();
                    nodesToProcess.Enqueue(neighbor);
                    processedNodes.Add(neighbor);
                    ExcludeNeighbors(GetNodeGridPosition(neighbor), square);

                    bool isEventCalled = Random.Range(0f, 1f) < 0.3f;
                    if (isEventCalled)
                        CheckEventArea(GetNodeGridPosition(neighbor), eventArea, neighbor);
                    // Mostra quale nodo è stato appena aggiunto alla coda
                    //Debug.Log($"Aggiunto alla coda nodo: {neighbor.name} alla posizione {GetNodeGridPosition(neighbor)}");

                    // Aspetta un momento per ogni passaggio
                    yield return new WaitForSeconds(0.01f); // Modifica il tempo come preferisci
                }
            }

            // Pausa tra i nodi processati
            yield return new WaitForSeconds(0.01f); // Modifica il tempo come preferisci
        }

        // Log dell'ultimo nodo creato al termine della coroutine
        if (lastCreatedNode != null)
        {
            lastCreatedNode.GetComponent<ActivateOrbit>().isExit = true;
            lastCreatedNode.GetComponent<ActivateOrbit>().SetStatus();
        }
    }
    // Metodo per ottenere la posizione nella griglia di un nodo selezionato
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
    GameObject GetNodeUnderMouse()
    {
        // Configura il puntatore del mouse per il raycast
        PointerEventData pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        // Esegui il raycast per ottenere gli oggetti UI sotto il puntatore
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        // Ritorna il primo GameObject che viene colpito dal raycast
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("Node"))
            {
                return result.gameObject;
            }
        }

        return null;
    }

    void SelectNode(GameObject clickedNode)
    {
        // Se esiste un nodo selezionato, ripristina il colore precedente con alpha 0.05
        if (selectedNode != null && selectedNode.GetComponent<ActivateOrbit>().nodesConnected.Contains(clickedNode))
        {
            Image previousImage = selectedNode.GetComponent<Image>();
            previousImage.color = Color.yellow;
            previousImage.GetComponent<ActivateOrbit>()._gameObject.SetActive(false);
        }

        if (walkableNodes.Contains(clickedNode) && selectedNode.GetComponent<ActivateOrbit>().nodesConnected.Contains(clickedNode))
        {
            selectedNode = clickedNode;
            //nodesSelectNode.Clear();
            selectedNode.GetComponent<ActivateOrbit>().ActionImplementation(0, selectedNode.GetComponent<ActivateOrbit>().type);
            // Cambia il colore del nodo selezionato con alpha 1
            Image selectedImage = selectedNode.GetComponent<Image>();
            Color selectedColorWithAlpha = selectedColor;
            selectedColorWithAlpha.a = 1f;  // Imposta l'alpha a 1
            selectedImage.color = selectedColorWithAlpha;
            selectedImage.GetComponent<ActivateOrbit>()._gameObject.SetActive(true);


            // Ottieni e stampa la posizione del nodo nella griglia
            Vector2Int position = GetNodeGridPosition(selectedNode);
            map.SetActive(false);
            if (selectedNode.GetComponent<ActivateOrbit>().type == PlanetType.BATTLE)
            {
                EventManager.StartBattle?.Invoke(true);
            }
            else
                EventManager.StartBattle?.Invoke(false);
            //Debug.Log($"Nodo selezionato è alla posizione: {position.x}, {position.y}");
        }

    }

    List<GameObject> GetDiagonalNeighbors(Vector2Int currentPos, int minRange, int maxRange, GameObject nodeStart)
    {
        List<GameObject> neighbors = new List<GameObject>();

        int[,] directions = new int[,]
        {
            { 1, 1 },  // Diagonale in basso a destra
            /*{ 1, -1 },*/ // Diagonale in basso a sinistra
            { -1, 1 }, // Diagonale in alto a destra
            /*{ -1, -1 } */// Diagonale in alto a sinistra
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
                    if (nodeStart != null)
                        nodeStart.GetComponent<ActivateOrbit>().nodesConnected.Add(neighbor);
                    excludedNodes.Add(neighbor);
                    walkableNodes.Add(neighbor);

                }
            }
        }
        foreach (GameObject obj in neighbors)
        {
            obj.GetComponent<Image>().color = selectedColor;
            obj.GetComponent<ActivateOrbit>().nodesConnected.Add(nodeStart);
        }
        return neighbors;
    }


    void ExcludeNeighbors(Vector2Int currentPos, int size)
    {
        size = square;
        int halfSize = size / 2;
        int startRow = Mathf.Max(0, currentPos.x - halfSize);
        int startCol = Mathf.Max(0, currentPos.y - halfSize);
        int endRow = Mathf.Min(rows - 1, currentPos.x + halfSize);
        int endCol = Mathf.Min(cols - 1, currentPos.y + halfSize);

        for (int row = startRow; row <= endRow; row++)
        {
            for (int col = startCol; col <= endCol; col++)
            {
                GameObject node = gridNodes[row, col];
                if (node != null && !excludedNodes.Contains(node) && !walkableNodes.Contains(node))
                {  // Cambia il colore gradualmente per mostrare l'avanzamento
                    //Image neighborImage = node.GetComponent<Image>();
                    //neighborImage.color = Color.red;

                    excludedNodes.Add(node);
                }
            }
        }
    }

    void CheckEventArea(Vector2Int currentPos, int size, GameObject startingNode)
    {
        AssignType(startingNode);
        size = eventArea;
        int halfSize = size / 2;
        int startRow = Mathf.Max(0, currentPos.x - halfSize);
        int startCol = Mathf.Max(0, currentPos.y - halfSize);
        int endRow = Mathf.Min(rows - 1, currentPos.x + halfSize);
        int endCol = Mathf.Min(cols - 1, currentPos.y + halfSize);


        for (int row = startRow; row <= endRow; row++)
        {
            for (int col = startCol; col <= endCol; col++)
            {
                GameObject node = gridNodes[row, col];
                if (node != null && !conditionAreas.Contains(node) && !walkableNodes.Contains(node))
                {  // Cambia il colore gradualmente per mostrare l'avanzamento
                    node.GetComponent<ActivateOrbit>().eventType = startingNode.GetComponent<ActivateOrbit>().eventType;
                    Image neighborImage = node.GetComponent<Image>();
                    switch (node.GetComponent<ActivateOrbit>().eventType)
                    {
                        case CONDITIONTYPE.SUN:
                            neighborImage.color = Color.magenta;
                            break;
                        case CONDITIONTYPE.METEOR:
                            neighborImage.color = Color.grey;
                            break;
                        case CONDITIONTYPE.NEBULOSA:
                            neighborImage.color = Color.cyan;
                            break;
                    }


                    conditionAreas.Add(node);
                }
                else if (node != null && !conditionAreas.Contains(node) && walkableNodes.Contains(node) && !eventNodes.Contains(node))
                {
                    Image neighborImage = node.GetComponent<Image>();
                    neighborImage.color = Color.cyan;

                    eventNodes.Add(node);
                    node.GetComponent<ActivateOrbit>().eventType = startingNode.GetComponent<ActivateOrbit>().eventType;
                }
            }
        }
    }

    private void AssignType(GameObject node)
    {
        int value = Random.Range(0, 3);
        switch (value)
        {
            case 0:
                node.GetComponent<ActivateOrbit>().eventType = CONDITIONTYPE.SUN;
                break;
            case 1:
                node.GetComponent<ActivateOrbit>().eventType = CONDITIONTYPE.NEBULOSA;
                break;
            case 2:
                node.GetComponent<ActivateOrbit>().eventType = CONDITIONTYPE.METEOR;
                break;
        }
    }


    void AddNeighbors(Vector2Int currentPos, int size, GameObject nodeBegin)
    {
        size = finalConnection;
        int halfSize = size / 2;
        int startRow = Mathf.Max(0, currentPos.x - halfSize);
        int startCol = Mathf.Max(0, currentPos.y - halfSize);
        int endRow = Mathf.Min(rows - 1, currentPos.x + halfSize);
        int endCol = Mathf.Min(cols - 1, currentPos.y + halfSize);

        for (int row = startRow; row <= endRow; row++)
        {
            for (int col = startCol; col <= endCol; col++)
            {
                GameObject node = gridNodes[row, col];
                if (node != null && walkableNodes.Contains(node))
                {  // Cambia il colore gradualmente per mostrare l'avanzamento
                    Image neighborImage = node.GetComponent<Image>();
                    neighborImage.color = Color.yellow;

                    List<GameObject> checkDuplicate = nodeBegin.GetComponent<ActivateOrbit>().nodesConnected;

                    if (!checkDuplicate.Contains(node) && node != nodeBegin)
                        nodeBegin.GetComponent<ActivateOrbit>().nodesConnected.Add(node);

                }
            }
        }
    }

    List<GameObject> FindShortestPathInGrid(GameObject startNode, GameObject endNode)
    {
        if (startNode == null || endNode == null || startNode == endNode)
            return null;

        Queue<List<GameObject>> queue = new Queue<List<GameObject>>();
        HashSet<GameObject> visited = new HashSet<GameObject>();

        // Aggiungi il nodo di partenza alla coda
        queue.Enqueue(new List<GameObject> { startNode });
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            List<GameObject> currentPath = queue.Dequeue();
            GameObject currentNode = currentPath[currentPath.Count - 1];

            // Se raggiungi il nodo finale, restituisci il percorso
            if (currentNode == endNode)
            {
                return currentPath;
            }

            // Itera sui vicini (nodi della griglia)
            Vector2Int currentPos = GetNodeGridPosition(currentNode);
            foreach (GameObject neighbor in GetNeighbors(currentPos))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);

                    // Crea un nuovo percorso che include questo vicino
                    List<GameObject> newPath = new List<GameObject>(currentPath);
                    newPath.Add(neighbor);

                    queue.Enqueue(newPath);
                }
            }
        }

        // Nessun percorso trovato
        return null;
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

    void ColorPath(List<GameObject> path)
    {
        if (path == null || path.Count < 2)
            return;


        for (int i = 1; i < path.Count - 1; i++)  // Escludi il primo (start) e l'ultimo (end)
        {
            Image nodeImage = path[i].GetComponent<Image>();
            nodeImage.color = Color.blue;
        }
    }

    void ResetNodeColors()
    {
        foreach (GameObject node in gridNodes)
        {
            if (!walkableNodes.Contains(node) && !conditionAreas.Contains(node))  // Salta i nodi camminabili
            {

                Image nodeImage = node.GetComponent<Image>();
                Color color = defaultColor;
                color.a = 0;
                nodeImage.color = color;


            }
            else if (!walkableNodes.Contains(node) && conditionAreas.Contains(node))
            {
                Image neighborImage = node.GetComponent<Image>();
                switch (node.GetComponent<ActivateOrbit>().eventType)
                {
                    case CONDITIONTYPE.SUN:
                        neighborImage.color = Color.magenta;
                        break;
                    case CONDITIONTYPE.METEOR:
                        neighborImage.color = Color.grey;
                        break;
                    case CONDITIONTYPE.NEBULOSA:
                        neighborImage.color = Color.cyan;
                        break;
                }
            }
        }
    }

    public void CloseMap()
    {
        map.SetActive(false);
    }
}