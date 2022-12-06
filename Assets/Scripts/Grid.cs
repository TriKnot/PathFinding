using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Range(10, 178)]
    [SerializeField] private int _width;
    [Range(10, 100)]
    [SerializeField] private int _height;
    public Vector2Int GridSize => new (_width, _height);
    private Vector2 GridWorldSpaceOffset => new (_width / 2f - 0.5f, _height / 2f - 0.5f);

    private Node[,] _grid;
    
    public List<Node> Path { get; set; }
    
    [SerializeField] private GameObject _nodePrefab;
    
    private void Awake()
    {
        transform.localScale = new Vector3(_width / 10.0f, 1, _height / 10.0f);
        _grid = new Node[_width, _height];
        CreateGrid();
        CacheNeighbours();
    }


    private void CreateGrid()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                var pos = new Vector3((x - GridWorldSpaceOffset.x), 0, (z - GridWorldSpaceOffset.y));
                Node node = Instantiate(_nodePrefab, pos, Quaternion.identity).GetComponent<Node>();
                var nodeObject = node.gameObject;
                nodeObject.name = $"Node ({x}, {z})";
                nodeObject.transform.parent = transform;
                node.Init(new Vector2Int(x, z), this);
                _grid[x, z] = node;
            }
        }
    }

    
    private void CacheNeighbours()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                _grid[x, z].FindNeighbours();
            }
        }

    }
    
    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        var x = Mathf.RoundToInt(worldPosition.x + GridWorldSpaceOffset.x);
        var y = Mathf.RoundToInt(worldPosition.z + GridWorldSpaceOffset.y);
        return  _grid[x, y];
    }
    
    public Node GetNodeFromGridPoint(Vector2 gridPosition)
    {
        return _grid[(int)gridPosition.x, (int)gridPosition.y];
    }

    public void ResetAllNodes()
    {
        foreach (var node in _grid)
        {
            node.Reset();
        }
    }
    
}
