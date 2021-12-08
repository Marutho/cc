using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Enemy : MonoBehaviour
{
    #region Attributes
    
    public Transform target;

    public float maxVelocity = 10f;
    public float accel = 10f;
    public Vector2 vel = Vector2.zero;
    
    public Rigidbody2D rb;

    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    
    public float hp = 30.0f;

    public float factorMass = 1.0f;
    
    public Pathfinding pathfinding;

    public int scoreThatItGives = 100;
    
    #endregion

    public const int LayerEnemy = 8;
    public const int LayerPlayer = 6;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pathfinding = GetComponent<Pathfinding>();
        Physics2D.IgnoreLayerCollision(LayerEnemy, LayerPlayer);
        Physics2D.IgnoreLayerCollision(LayerEnemy, LayerEnemy);
    }

    void Update()
    {
        
        float dt = Time.deltaTime;
        if (target)
            DumbFollowTarget(target, dt);
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    public PlayerBehaviour IsHittingPlayer()
    {
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.forward.normalized, 10f);
        Debug.DrawRay(transform.position, transform.forward.normalized, Color.red, 100);
        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log("hit");
            if (hit.collider.GetComponent<PlayerBehaviour>())
            {
                return hit.collider.GetComponent<PlayerBehaviour>();    
            }
        }

        return null;
    }
    
    /// <summary>
    /// Method in charge of making the Enemy move around.
    /// </summary>
    /// <param name="direction">Vector3 describing the direction towards which the enemy will be moving</param>
    /// <param name="dt">Float describing the time differential. Usually passed as Time.deltaTime</param>
    public void Move(Vector2 direction, float dt)
    {
        Vector2 velTarget = direction * maxVelocity;
        Vector2 velOffset = velTarget - vel;
        velOffset = Vector2.ClampMagnitude(velOffset, accel * dt);
        vel += velOffset;
        rb.velocity = vel;
    }

    /// <summary>
    /// Dumb follow target that just follows the target around the map
    /// </summary>
    /// <param name="t">Transform of the target GameObject to dumbly follow</param>
    /// <param name="dt">Float describing the time differential. Usually passed as Time.deltaTime</param>
    public void DumbFollowTarget(Transform t, float dt)
    {
        // Basic target follow behavior that goes in your direction not taking terrain into account
        if (DistanceToTarget() > 0.2f)
            Move(DirectionToTargetNormalized(), dt);
    }

    public void FollowTargetSmart(Transform t)
    {
        pathfinding.mTarget = t;
        if (DistanceToTarget() > 2f)
            pathfinding.MoveToTarget();
        else
        {
            DumbFollowTarget(t, Time.deltaTime);
        }
    }
    
    public Vector2 DirectionToTargetNonNormalized()
    {
        return target.position - transform.position;
    }

    public Vector2 DirectionToTargetNormalized()
    {
        return DirectionToTargetNonNormalized().normalized;
    }

    public float DistanceToTarget()
    {
        return DirectionToTargetNonNormalized().magnitude;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        ObstacleAttract obstacle = other.gameObject.GetComponent<ObstacleAttract>();
        if (obstacle)
        {
            
        }
    }

    public void OnHit(ObstacleAttract obstacle)
    {
        if (SoundManager.instance)
            SoundManager.instance.Clock(SoundManager.instance.efxSource);

        hp -= obstacle.factorMassDamage * 30f;
        if (hp <= 0)
        {
            PlayerBehaviour.UniquePlayer.Score += scoreThatItGives;
            Destroy(gameObject);
            
        }
        if (obstacle.factorMassDamage > factorMass)
        {
            vel = obstacle.Rb.velocity * 20;
            rb.velocity = vel;
        }
        else
        {
            foreach (var path in FindObjectsOfType<Grid>())
            {
                path.regenerate = true;
            }
            obstacle.shooting = false;
            obstacle.StrengthDrag();
        }
    }
}
