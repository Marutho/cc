using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Grid : MonoBehaviour
{

  public LayerMask unwalkableMask;
  public LayerMask costMultiplierMask;
  public Vector2 gridWorldSize;
  [SerializeField]
  private GridComponent _prefab;
  public float nodeRadius;
  NodePathfinding[,] grid;
  
  public NodePathfinding[,] Grid1 => grid;

  float nodeDiameter;
  int gridSizeX, gridSizeY;

  public List<NodePathfinding> openSet;
  public HashSet<NodePathfinding> closedSet;

  public List<NodePathfinding> path;
  

  public bool regenerate = false;

  private static bool _doneGrid = false;
  
  /***************************************************************************/

  void Awake()
  {
    nodeDiameter = nodeRadius * 2;
    gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
    gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
    CreateGrid(!_doneGrid);
    _doneGrid = true;
    target = GameObject.FindWithTag("Grid").transform;
  }

  private void Update()
  {
    if (regenerate)
    {
      CreateGrid(false);
      regenerate = false;
    }
  }

  /***************************************************************************/

  [SerializeField]
  private Transform target;
  
  void CreateGrid(bool instantiate)
  {
    grid = new NodePathfinding[gridSizeX, gridSizeY];
    if (!target)
    {
        target = GameObject.FindWithTag("Grid").transform;
        if (!target)
        {
          Debug.Log(" RI P");
        }
    }
    Vector3 worldBottomLeft = target.transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

    for (int x = 0; x < gridSizeX; x++)
    {
      for (int y = 0; y < gridSizeY; y++)
      {
        Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
        bool walkable = !(Physics2D.CircleCast(worldPoint, nodeRadius, Vector3.one * (nodeDiameter), nodeDiameter/2, unwalkableMask));
          float costMultiplier = 1.0f;
          grid[x, y] = new NodePathfinding(true, worldPoint, x, y, walkable ? 1.0f : 1000000f);
      }
    }
  }

  /***************************************************************************/

  public List<NodePathfinding> GetNeighbours(NodePathfinding node, bool eightConnectivity)
  {
    List<NodePathfinding> neighbours = new List<NodePathfinding>();

    for (int x = -1; x <= 1; x++)
    {
      for (int y = -1; y <= 1; y++)
      {
        if ((x == 0 && y == 0))
        {
          continue;
        }
        if (!eightConnectivity && (Mathf.Abs(x) + Mathf.Abs(y) > 1))
        {
          continue;
        }

        int checkX = node.mGridX + x;
        int checkY = node.mGridY + y;

        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
        {
          neighbours.Add(grid[checkX, checkY]);
        }
      }
    }

    return neighbours;
  }

  /***************************************************************************/

  public NodePathfinding NodeFromWorldPoint(Vector3 worldPosition)
  {
    Vector3 worldBottomLeft = target.transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
      Vector2 pos = worldPosition - worldBottomLeft;
      int x = (int)Mathf.Floor(pos.x/2);
      int y = (int)Mathf.Floor(pos.y/2);
      return grid[x, y];
  }

  /***************************************************************************/

  void OnDrawGizmos()
  {
    //Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, n.y));

    if (grid != null)
    {
      foreach (NodePathfinding n in grid)
      {
        if (n == null) continue;
        Gizmos.color = (n.mWalkable) ? Color.white : Color.red;

        if (openSet != null)
        {
          if (openSet.Contains(n))
          {
            Gizmos.color = Color.green;
          }
        }

        if (closedSet != null)
        {
          if (closedSet.Contains(n))
          {
            Gizmos.color = Color.yellow;
            //Gizmos.DrawCube(n.mWorldPosition, Vector3.one * (nodeDiameter+ 1f) );
          }
        }

        if (path != null)
        {
          if (path.Contains(n))
          {
            Gizmos.color = n.debug ? Color.black : Color.blue;
            
          }
        }

        if (n.mCostMultiplier > 1.0f)
        {
          Gizmos.color += Color.blue;
        }
        //Gizmos.DrawCube(n.mWorldPosition, Vector3.one * (nodeDiameter+ 1f) );
        
      }
    }
  }

  /***************************************************************************/

}