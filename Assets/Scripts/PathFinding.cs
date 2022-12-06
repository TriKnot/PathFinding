using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private Grid _grid;
    private Heap<Node> _openSet;
    private HashSet<Node> _closedSet;
    [Header("Step Time")][Range(0, 1)]
    [SerializeField] private float _delay = 0.1f;

    
    public void Start()
    {
        _grid = GameManager.Instance.Grid;
    }

    public IEnumerator FindPathVisualize(Vector3 start, Vector3 end)
    {
        var wait = new WaitForSeconds(_delay);
        Node startNode = _grid.GetNodeFromWorldPoint(start);
        Node endNode = _grid.GetNodeFromWorldPoint(end);
        endNode.SetListType(Node.InListType.OnPath);
        endNode.GCost = GetDistance(startNode, endNode);
        _openSet = new Heap<Node>(_grid.GridSize.x * _grid.GridSize.y);
        _closedSet = new HashSet<Node>();
        
        _openSet.Add(startNode);
        startNode.SetListType(Node.InListType.Open);
        
        while (_openSet.Count > 0)
        {
            Node currentNode = _openSet.RemoveFirst();

            _closedSet.Add(currentNode);
            currentNode.SetListType(Node.InListType.Closed);
        
            if (currentNode == endNode)
            {
                StartCoroutine(RetracePathVisualize(startNode, endNode));
                yield break;
            }
        
            
            foreach (var neighbour in currentNode.Neighbours)
            {
                if(!neighbour.IsWalkable || _closedSet.Contains(neighbour)) continue;
                
                int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                if(newMovementCostToNeighbour < neighbour.GCost || !_openSet.Contains(neighbour))
                {
                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour, endNode);
                    neighbour.Parent = currentNode;
                
                    if (!_openSet.Contains(neighbour))
                    {
                        _openSet.Add(neighbour);
                        neighbour.SetListType(Node.InListType.Open);
                    }
                }
                yield return wait;
            }
        }
    }
    
    public void FindPath(Vector3 start, Vector3 end)
    {
        Stopwatch sw = Stopwatch.StartNew();
        Node startNode = _grid.GetNodeFromWorldPoint(start);
        Node endNode = _grid.GetNodeFromWorldPoint(end);
        _openSet = new Heap<Node>((int)(_grid.GridSize.x * _grid.GridSize.y));
        _closedSet = new HashSet<Node>();
        
        _openSet.Add(startNode);
        startNode.SetListType(Node.InListType.Open);
        
        while (_openSet.Count > 0)
        {
            Node currentNode = _openSet.RemoveFirst();

            _closedSet.Add(currentNode);
            currentNode.SetListType(Node.InListType.Closed);
        
            if (currentNode == endNode)
            {
                GameManager.Instance.Grid.Path = RetracePath(startNode, endNode);
                GameManager.Instance.Player.Move();
                sw.Stop();
                print("Path found: " + sw.ElapsedMilliseconds + " ms");
                return;
            }
        
            
            foreach (var neighbour in currentNode.Neighbours)
            {
                if(!neighbour.IsWalkable || _closedSet.Contains(neighbour)) continue;
                
                int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                if(newMovementCostToNeighbour < neighbour.GCost || !_openSet.Contains(neighbour))
                {
                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour, endNode);
                    neighbour.Parent = currentNode;
                
                    if (!_openSet.Contains(neighbour))
                    {
                        _openSet.Add(neighbour);
                        neighbour.SetListType(Node.InListType.Open);
                    }
                }
            }
        }
        sw.Stop();
        print("Path not found in " + sw.ElapsedMilliseconds + " ms");
    }
    
    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.GridPosition.x - nodeB.GridPosition.x);
        int distanceY = Mathf.Abs(nodeA.GridPosition.y - nodeB.GridPosition.y);

        if (distanceX > distanceY)
            return 14 * distanceY + 10 * (distanceX - distanceY);
        return 14 * distanceX + 10 * (distanceY - distanceX);
    }
    
    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode.SetListType(Node.InListType.OnPath);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }
    
    private IEnumerator RetracePathVisualize(Node startNode, Node endNode)
    {
        var wait = new WaitForSeconds(_delay);
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode.SetListType(Node.InListType.OnPath);
            currentNode = currentNode.Parent;
            yield return wait;
        }
        startNode.SetListType(Node.InListType.OnPath);
        path.Reverse();
        GameManager.Instance.Grid.Path = path;
        GameManager.Instance.Player.Move();
    }


}
