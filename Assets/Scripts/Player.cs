using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Camera _camera;
    private Node _currentTargetNode;
    private bool _isMoving;
    [SerializeField] private float _moveSpeed = 5f;
    
    private Coroutine _pathCoroutine;
    private Coroutine _moveCoroutine;
    
    [SerializeField] private bool _visualizePath;

    private void Start()
    {
        _camera = Camera.main;
    }   
    
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(_pathCoroutine != null)
            {
                StopCoroutine(_pathCoroutine);
            }
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                Vector3 startPos;
                if (_isMoving)
                {
                    startPos = _currentTargetNode.WorldPosition;
                }
                else
                {
                    startPos = transform.position;
                }
                GameManager.Instance.Grid.ResetAllNodes();
                if (_visualizePath)
                {
                    _pathCoroutine = StartCoroutine(GameManager.Instance.PathFinding.FindPathVisualize(startPos, hit.point));
                }
                else
                {
                    GameManager.Instance.PathFinding.FindPath(startPos, hit.point);
                }
            }
        }
        if (Input.GetMouseButton(1))
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                Node node = GameManager.Instance.Grid.GetNodeFromWorldPoint(hit.point);
                node.SetWalkable(false); 
            }
        }
    }

    public void Move()
    {
        if(_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
        }
        _moveCoroutine = StartCoroutine(MoveCoroutine());
    }
    
    private IEnumerator MoveCoroutine()
    {
        _pathCoroutine = null;
        var path = GameManager.Instance.Grid.Path;
        _currentTargetNode = path[^1];
        _isMoving = true;
        var t = Time.deltaTime;
        foreach (var node in path)
        {
            while(transform.position != node.transform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, node.WorldPosition, _moveSpeed * t);
                yield return new WaitForSeconds(t);
            }
        }
        _isMoving = false;
    }
    
}
