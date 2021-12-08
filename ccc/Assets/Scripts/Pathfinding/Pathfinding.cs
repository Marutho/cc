using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public enum KindOfHeuristic
{
  None,
  Euclidean,
  Diagonal,
  Manhattan,
}

public class Pathfinding : MonoBehaviour
{

  public Transform mSeeker;
  public Transform mTarget;
  public bool mSmoothPath = false;
  public KindOfHeuristic heuristic;
  
  NodePathfinding CurrentStartNode;
  NodePathfinding CurrentTargetNode;
  Grid Grid;
  
  private Rigidbody2D _rb;

  int Iterations = 0;
  float LastStepTime = 0.0f;
  float TimeBetweenSteps = 0.0f;
  public bool EightConnectivity = false;
  public bool mManhattanDistance = true;


  public float speed = 6.0f;
  public int currentNode = 1;


  /***************************************************************************/

  void Awake()
  {
    _rb = GetComponent<Rigidbody2D>();
    Grid = GetComponent<Grid>();
    Iterations = 0;
    LastStepTime = 0.0f;
    
  }

  private void Start()
  {
    //Grid = Grid.Instance;
    
  }

  /***************************************************************************/

  public void SetTarget(Transform target)
  {
    mTarget = target;
  }
  
  public void MoveToTarget(float parameterSpeed = 0.0f)
  {
    if (parameterSpeed <= 0.1f)
    {
      parameterSpeed = speed;
    }
    if (Grid && Grid.path != null && currentNode < Grid.path.Count && Vector3.Distance(mTarget.position, mSeeker.position) >= 3.5f)
    {
      NodePathfinding node = Grid.path[currentNode];
      Vector2 pos = (node.mWorldPosition - mSeeker.position);
      Vector3 dir = pos.normalized;
      _rb.velocity = dir * parameterSpeed;
      if ((mSeeker.position - node.mWorldPosition).magnitude <= 3.5f)
      {
        currentNode++;
      }
    }
    else if (mTarget)
    {
      Debug.Log("going to target");
      Vector2 pos = (mTarget.position - mSeeker.position);
      Vector3 dir = pos.normalized;
      _rb.velocity = dir * parameterSpeed;
      if ((mSeeker.position - mTarget.position ).magnitude <= 0.01)
      {
        Debug.Log("arrived");
      }
    }
  }

  public bool followTargetAlways = true;
  
  void Update()
  {
    
    if (followTargetAlways)
      MoveToTarget();

    if (!mTarget)
    {
      mTarget = FindObjectOfType<PlayerBehaviour>().transform;
      return;
    }

    // Positions changed?
    if (PathInvalid())
    {
      // Remove old path
      if (Grid.path != null)
      {
        currentNode = 0;
        Grid.path.Clear();
      }
      // Start calculating path again
      Iterations = 0;
      if (TimeBetweenSteps == 0.0f)
      {
        Iterations = -1;
      }
      FindPath(mSeeker.position, mTarget.position, Iterations);
    }
    else
    {
      // Path found?
      if (Iterations >= 0)
      {
        // One or more iterations?
        if (TimeBetweenSteps == 0.0f)
        {
          // One iteration, look until path is found
          Iterations = -1;
          FindPath(mSeeker.position, mTarget.position, Iterations);
        }
        else if (Time.time > LastStepTime + TimeBetweenSteps)
        {
          // Iterate increasing depth every time step
          LastStepTime = Time.time;
          Iterations++;
          FindPath(mSeeker.position, mTarget.position, Iterations);
        }
      }
    }
  }

  /***************************************************************************/

  private float _calc = 0.4f;
  private float _inBetween = 0.3f;
  
  bool PathInvalid()
  {
      _calc = 0.0f;
      
      //CurrentStartNode != Grid.NodeFromWorldPoint(mSeeker.position) || 
      if (CurrentTargetNode != Grid.NodeFromWorldPoint(mTarget.position))
        return true;
      return false;
  }

  /***************************************************************************/

  void FindPath(Vector3 startPos, Vector3 targetPos, int iterations)
  {
    CurrentStartNode = Grid.NodeFromWorldPoint(startPos);
    CurrentTargetNode = Grid.NodeFromWorldPoint(targetPos);
    
    Heap<NodePathfinding> openSetHeap = new Heap<NodePathfinding>((left, right) => (int) (left.fCost - right.fCost));
    HashSet<NodePathfinding> closedSet = new HashSet<NodePathfinding>();
    openSetHeap.Enqueue(CurrentStartNode);
    Grid.openSet = openSetHeap.ArrayHeap;

    int currentIteration = 0;
    NodePathfinding node = CurrentStartNode;
    while (openSetHeap.Count > 0 && node != CurrentTargetNode && (iterations == -1 || currentIteration < iterations))
    {

      node = openSetHeap.Dequeue();

      closedSet.Add(node);
      
      Grid.openSet = openSetHeap.ArrayHeap;
      
      Grid.closedSet = closedSet;
      
      // Check destination
      if (node != CurrentTargetNode)
      {

        NodePathfinding nodeToAdd = null;
        // Open neighbours
        foreach (NodePathfinding neighbour in Grid.GetNeighbours(node, EightConnectivity))
        {
          if (!neighbour.mWalkable || closedSet.Contains(neighbour)) continue;

          int cost = (int) (node.gCost + GetDistance(node, neighbour) * node.mCostMultiplier);
          bool contains = openSetHeap.ArrayHeap.Contains(neighbour);
          if (cost < neighbour.gCost || !contains)
          {            
            neighbour.gCost = cost;
            neighbour.hCost = Heuristic(neighbour, CurrentTargetNode);
            neighbour.mParent = node;
            nodeToAdd = neighbour;            
            openSetHeap.Enqueue(nodeToAdd);
          }
        }
        
        currentIteration++;
      }
      else
      {
        // Path found!
        RetracePath(CurrentStartNode, CurrentTargetNode);
        
        // Path found
        Iterations = -1;

        /*
        Debug.Log("Statistics:");
        Debug.LogFormat("Total nodes:  {0}", openSetHeap.ArrayHeap.Count + closedSet.Count);
        Debug.LogFormat("Open nodes:   {0}", openSetHeap.ArrayHeap.Count);
        Debug.LogFormat("Closed nodes: {0}", closedSet.Count);
        Debug.LogFormat("Length: {0}", Grid.path.Count);
        */
      }
    }
  }

  /***************************************************************************/

  void RetracePath(NodePathfinding startNode, NodePathfinding endNode)
  {
    List<NodePathfinding> path = new List<NodePathfinding>();
    NodePathfinding start = endNode;
    while (start != null && start != startNode)
    {
      path.Add(start);
      start = start.mParent;
    } 
    Assert.AreEqual(start, startNode);
    path.Add(start);
    path.Reverse();
    Grid.path = path;
    if (mSmoothPath)
    {
      path = SmoothPath02(path);
      Grid.path = path;
    }
    currentNode = 3;
  }

  /***************************************************************************/

  float GetDistance(NodePathfinding nodeA, NodePathfinding nodeB)
  {
    
    /**/
    
    switch (heuristic)
    {
      case KindOfHeuristic.None:
      {
        int dstX = Mathf.Abs(nodeA.mGridX - nodeB.mGridX);
        int dstY = Mathf.Abs(nodeA.mGridY - nodeB.mGridY);
 
        if (dstX > dstY)
        {
          return 3 * dstY + 2 * (dstX - dstY);
        }

        return 3 * dstX + 2 * (dstY - dstX);
      }

      case KindOfHeuristic.Manhattan:
      {
        int dstX = Mathf.Abs(nodeA.mGridX - nodeB.mGridX);
        int dstY = Mathf.Abs(nodeA.mGridY - nodeB.mGridY);
        return (dstX + dstY);
      }

      case KindOfHeuristic.Diagonal:
      {
        int dstX = Mathf.Abs(nodeA.mGridX - nodeB.mGridX);
        int dstY = Mathf.Abs(nodeA.mGridY - nodeB.mGridY);
        return (dstX + dstY) + (1- 2 * 1) * Math.Min(dstX, dstY);
      }

      case KindOfHeuristic.Euclidean:
      {
        int dstX = Mathf.Abs(nodeA.mGridX - nodeB.mGridX);
        int dstY = Mathf.Abs(nodeA.mGridY - nodeB.mGridY);
        return Mathf.Sqrt(dstX * dstX + dstY * dstY);
      }
      
    }
    return 0;
  }

  /***************************************************************************/

  public float mScale = 2.0f;
  
  float Heuristic(NodePathfinding nodeA, NodePathfinding nodeB)
  {
    switch (heuristic)
    {
      case KindOfHeuristic.None:
      {
        return 0;
      }

      case KindOfHeuristic.Manhattan:
      {
        int dstX = Mathf.Abs(nodeA.mGridX - nodeB.mGridX);
        int dstY = Mathf.Abs(nodeA.mGridY - nodeB.mGridY);
        return mScale * (dstX + dstY);
      }

      case KindOfHeuristic.Diagonal:
      {
        int dstX = Mathf.Abs(nodeA.mGridX - nodeB.mGridX);
        int dstY = Mathf.Abs(nodeA.mGridY - nodeB.mGridY);
        return mScale * (dstX + dstY) + (mScale - 2 * mScale) * Math.Min(dstX, dstY);
      }

      case KindOfHeuristic.Euclidean:
      {
        int dstX = Mathf.Abs(nodeA.mGridX - nodeB.mGridX);
        int dstY = Mathf.Abs(nodeA.mGridY - nodeB.mGridY);
        return mScale * Mathf.Sqrt(dstX * dstX + dstY * dstY);
      }
      
    }
    return 0;
  }

  /***************************************************************************/
  
  

  /***************************************************************************/

  List<NodePathfinding> SmoothPath(List<NodePathfinding> path)
  {
    int i = 0;
    int j = 0;
    List<NodePathfinding> newPath = new List<NodePathfinding>();
    newPath.Add(path[0]);
    float originalCost = 0;
    for (int x = 0; x < path.Count; x++)
    {
      originalCost += path[x].fCost;
    }
    while (i < path.Count)
    {

      while (i < path.Count && j < path.Count &&
             BresenhamWalkable(path[i].mGridX, path[i].mGridY, path[j].mGridX, path[j].mGridY, originalCost))
      {
        j++;
      }

      newPath.Add(path[j - 1]);
      i = j;
    }

    return newPath;
  }

  List<NodePathfinding> SmoothPath02(List<NodePathfinding> path)
  {
    int i = 0;
    int j = path.Count - 1;
    float originalCost = 0;
    for (int x = 1; x < path.Count; x++)
    {
      originalCost += GetDistance(path[x-1], path[x]) * path[x].mCostMultiplier;
    }
    List<NodePathfinding> newPath = new List<NodePathfinding>();
    //newPath.Add(path[0]);
    int secureIndex = 0;
    while (i < path.Count && secureIndex < 10000)
    {
      secureIndex++;
      j = path.Count - 1;
      float thisOriginal = originalCost;
      while (i < path.Count && j < path.Count && j > i && j>0&&
             !BresenhamWalkable(
               path[i].mGridX, path[i].mGridY, 
               path[j].mGridX, path[j].mGridY, 
               thisOriginal
               ))
      {
        j--;
       thisOriginal -= GetDistance(path[j], path[j+1]) * path[j+1].mCostMultiplier;
      }
      newPath.Add(path[j]);
      for (int x = i+1; x <= j; x++)
      {
        originalCost -= GetDistance(path[x-1], path[x]) * path[x].mCostMultiplier;
      }
      i = j+1;
    }
    Assert.AreNotEqual(secureIndex, 10000);
    //
    newPath.Add(path[path.Count - 1]);

    
    Debug.Log(newPath);
    
    return newPath;
  }

  /*for (int i = 0; i < path.Count; i++)
  {
    NodePathfinding node = path[i];
    if (i + 1>= path.Count) continue;
    NodePathfinding nodeNext = path[i + 1];
    if (BresenhamWalkable(node.mGridX, node.mGridY, nodeNext.mGridX, nodeNext.mGridY))
    {
      Debug.Log("WALKABLE ? ");
      nodeNext.debug = true;
      Debug.Log($"{node.mGridX} - {node.mGridY}");
    }
    else
    {
      nodeNext.debug = false;
      Debug.Log("non walkable");
      Debug.Log($"{node.mGridX} - {node.mGridY}");
    }
    
  }*/
    // TODO

    /***************************************************************************/

  public bool BresenhamWalkable(int x, int y, int x2, int y2, float originalCost)
  {
    //return false;
    //return false;
    // TODO: 4 Connectivity
    // TODO: Cost
    int w = x2 - x;
    int h = y2 - y;
    if (!EightConnectivity)
    {
      if (w != 0 && h != 0) return false;
    }
    int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
    if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
    if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
    if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
    int longest = Mathf.Abs(w);
    int shortest = Mathf.Abs(h);
    if (!(longest > shortest)){
      longest = Mathf.Abs(h);
      shortest = Mathf.Abs(w);
      if (h < 0){
        dy2 = -1;
      }
      else if (h > 0){
        dy2 = 1;
      }
      dx2 = 0;
    }
    int numerator = longest >> 1;
    float cost = 0;
    int prevX = x;
    int prevY = y;

    for (int i = 0; i <= longest; i++)
    {
      
      if( !Grid.Grid1[x, y].mWalkable){
        return false;
      }
      
      numerator += shortest;
      prevX = x;
      prevY = y;
      if (!(numerator < longest)){
        numerator -= longest;
        x += dx1;
        y += dy1;
      }
      else{
        x += dx2;
        y += dy2;
      }
    
      cost += GetDistance(Grid.Grid1[x, y], Grid.Grid1[prevX, prevY]) * Grid.Grid1[x, y].mCostMultiplier;
      
    }
    return cost <= originalCost;
  }



}
