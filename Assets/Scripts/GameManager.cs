using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Grid _grid;
    public Grid Grid => _grid;
    
    public PathFinding PathFinding { get; private set; }

    public State CurrentState { get; private set; }

    public Player Player;
    
    public bool ShowPath { get; private set; }
    

    private void Awake()
    {  
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        PathFinding = GetComponent<PathFinding>();
    }

    private void Start()
    {
        SetCameraSize();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ShowPath = !ShowPath;
        }
    }

    private void SetCameraSize()
    {
        float maxHeight = 100.0f;
        float maxWidth = 178.0f;
        var height = _grid.GridSize.y / maxHeight;
        var width = _grid.GridSize.x / maxWidth;
        Camera.main.orthographicSize = height > width ? height * 50 : width * 50;
    }

    public void SetPathState()
    {
        CurrentState = State.PathFind;
    }
    
    public void SetWallState()
    {
        CurrentState = State.SetWall;
    }

}

public enum State
{
    PathFind,
    SetWall
}
