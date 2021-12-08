using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAndThings : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public enum Type
    {
        Rat,
        Cockroach
    }

    [SerializeField]
    private Type type;

    private Transform targetPlayer;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pathfinding = GetComponent<Pathfinding>();
        Physics2D.IgnoreLayerCollision(LayerEnemy, LayerPlayer);
        Physics2D.IgnoreLayerCollision(LayerEnemy, LayerEnemy);
        targetPlayer = FindObjectOfType<PlayerBehaviour>().transform;
        target = targetPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        
       
        
        */
        Vector2 dir = (target.position - transform.position).normalized;
        if (type == Type.Cockroach)
        {
            if (dir.x >= 0)
            {
                transform.rotation = Quaternion.AngleAxis(0, Vector3.up);    
            }
            else if (dir.x < 0)
            {
                transform.rotation = Quaternion.AngleAxis(180, Vector3.up);    
            }    
        }
        else
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);    
        }
        
        
        if (DistanceToTarget() > 2f)
            pathfinding.MoveToTarget();
        else
        {
            DumbFollowTarget(targetPlayer, Time.deltaTime);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            
            other.GetComponentInParent<PlayerBehaviour>().Hit();
        }
    }
}
