using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    private static ObstacleManager _instance;
    private Queue<ObstacleAttract> _obstacles;
    private float _timer = 0.0f;
    private float _timerInBetween = 5.0f;

    void Awake()
    {
        _instance = this;
        _obstacles = new Queue<ObstacleAttract>(10);
    }
    
    void Update()
    {
        if (_obstacles.Count > 0)
        {
            _timer += Time.deltaTime;
            if (_timer > _timerInBetween)
            {
                Debug.Log("Spawned");
                _timer = 0.0f;
                var deq = _obstacles.Dequeue();
                deq.gameObject.SetActive(true);
                deq.obstacleParent = false;
            }
        }
    }

    public static ObstacleManager Instance => _instance;

    public void Enqueue(ObstacleAttract obstacle)
    {
        _obstacles.Enqueue(obstacle);
    }
}
