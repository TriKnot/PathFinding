using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour, IHeapItem<Node>
{
    //G cost = distance from starting node
    //H cost = distance from end node
    //F cost = G cost + H cost
    private int _gCost;
    public int GCost
    {
        get => _gCost;
        set
        {
            _gCost = value;
            UpdateTextMesh();
        }
    }
    private int _hCost;
    public int HCost
    {
        get => _hCost;
        set
        {
            _hCost = value;
            UpdateTextMesh();
        }
    }
    private int FCost => GCost + HCost;
    public int HeapIndex { get; set; }
    public bool IsWalkable { get; private set; } = true;
    private Node _parent;
    public Node Parent
    {
        get => _parent;
        set
        {
            _parent = value;
            if(_parent != null && GameManager.Instance.ShowPath)
                DrawLineFromParent();
        }
    }
    private LineRenderer _lineRenderer;
    public Vector2Int GridPosition { get; private set; }
    public Vector3 WorldPosition { get; private set; }
    public Node[] Neighbours { get; private set; }
    private Grid _grid;
    public enum InListType
    {
        None,
        Open,
        Closed, 
        OnPath
    }
    private MeshRenderer _meshRenderer;
    private MeshRenderer _textMeshRenderer;
    private TextMesh _textMesh;

    public void Init (Vector2Int gridPos, Grid grid)
    {
        GridPosition = gridPos;
        WorldPosition = transform.position;
        _grid = grid;
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;
        _textMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        _textMeshRenderer.enabled = false;
        _textMesh = transform.GetChild(0).GetComponent<TextMesh>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void FindNeighbours()
    {
        List<Node> neighbours = new List<Node>();
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                //Skip self
                if(x == 0 && y == 0)
                    continue;
                //Check if inside grid
                var xPos = GridPosition.x + x;
                var yPos = GridPosition.y + y;
                if( xPos >= 0 && xPos < _grid.GridSize.x && yPos >= 0 && yPos < _grid.GridSize.y)
                {
                    //Get neighbour
                    var neighbour = _grid.GetNodeFromGridPoint(new Vector2(xPos, yPos));
                    //Add to list if exist
                    if(neighbour != null)
                        neighbours.Add(neighbour);
                }
                
            }
        }
        Neighbours = neighbours.ToArray();
    }

    public int CompareTo(Node other)
    {
        //Compare F cost
        var compare = FCost.CompareTo(other.FCost);
        //If F cost is the same, compare H cost
        if(compare == 0)
            compare = HCost.CompareTo(other.HCost);
        //Reverse result as we want the lowest F/H cost to be higher priority
        return -compare;
    }

    public void SetListType(InListType inListType)
    {
        switch (inListType)
        {
            case InListType.Open:
                _meshRenderer.enabled = true;
                _textMeshRenderer.enabled = true;
                SetMeshColor(new Color(0,0,1,0.5f));
                break;
            case InListType.Closed:
                _meshRenderer.enabled = true;
                _textMeshRenderer.enabled = true;
                SetMeshColor(new Color(1,0,0,0.5f));
                break;
            case InListType.OnPath:
                _meshRenderer.enabled = true;
                _textMeshRenderer.enabled = true;
                SetMeshColor(new Color(0,1,0,0.5f));
                break;
            case InListType.None:
            default:
                _meshRenderer.enabled = false;
                _textMeshRenderer.enabled = false;
                break;
        }
    }
    
    public void Reset()
    {
        if(!IsWalkable) return;
        GCost = 0;
        HCost = 0;
        Parent = null;
        SetListType(InListType.None);
        _lineRenderer.enabled = false;
    }
    
    private void SetMeshColor(Color color)
    {
        _meshRenderer.material.color = color;
    }
    
    public void SetWalkable(bool walkable)
    {
        IsWalkable = walkable;
        _meshRenderer.enabled = !walkable;
        _meshRenderer.material.color = walkable ? Color.white : Color.black;
    }
    
    private void UpdateTextMesh()
    {
        _textMesh.text = $"G: {GCost}\n\nH: {HCost}\n\nF: {FCost}";
    }
    
    private void DrawLineFromParent()
    {
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, WorldPosition);
        _lineRenderer.SetPosition(1, Parent.WorldPosition);
    }
}

