using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingEnemy : MonoBehaviour
{
    GridEnemy grid;
    //public Transform seeker, target;
    HeapEnemy<Node> openSet;
    PathRequestManagerEnemy requestManager;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManagerEnemy>();
        grid = GetComponent<GridEnemy>();
    }

    private void Start()
    {
        openSet = new HeapEnemy<Node>(grid.MaxSize);
    }
    public void StartFindPath(Vector2 startPos, Vector2 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            openSet.Clear();
            HashSet<Node> closeSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                closeSet.Add(currentNode);

                if (currentNode == targetNode)
                {

                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closeSet.Contains(neighbour))
                    {
                        continue;
                    }
                    if (currentNode.linked == false && neighbour.linked == false)
                    {
                        continue;
                    }
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementCost;

                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);

        }
        requestManager.FinishProcessingPath(waypoints, pathSuccess);
    }



    Vector2[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        Vector2[] waypoints = new Vector2[path.Count];

        for (int i = 0; i < path.Count; i++)
        {
            waypoints[i] = path[i].worldPosition;
        }
        return waypoints;

    }

    //Vector2[] SimplifyPath(List<Node> path)
    //{
    //    List<Vector2> waypoints = new List<Vector2>();
    //    Vector2 directionOld = Vector2.zero;
    //    for (int i = 1; i < path.Count; i++)
    //    {
    //        Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
    //        if (directionNew != directionOld)
    //        {
    //            waypoints.Add(path[i].worldPosition);
    //        }
    //        directionOld = directionNew;
    //    }
    //    return waypoints.ToArray();
    //}
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 + dstY + 10 * (dstX - dstY);
        return 14 + dstX + 10 * (dstY - dstX);
    }
}
