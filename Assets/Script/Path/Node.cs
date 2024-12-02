using UnityEngine;

public class Node : IHeapItem<Node>
{

    public bool walkable;
    public bool linked;
    public Vector3 worldPosition;

    public int gCost;
    public int hCost;

    public int gridX;
    public int gridY;

    public Node parent;
    public int heapIndex;

    public int movementCost;
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set { heapIndex = value; }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, bool isLinked)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        linked = isLinked;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
