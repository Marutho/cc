using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(VaccuumParticles))]
public class CloudEnemy : Enemy
{
    #region Enums

    public enum CloudEnemyStates
    {
        Idle,
        Chasing,
        Attaching,
        OnPlayer,
        Dying,
        Dead
    }
    
    #endregion

    #region Attributes
    
    private const float AttachDistance = 2f;
    private const float OnPlayerDistance = 0.1f;
    
    private CloudEnemyStates currentState = CloudEnemyStates.Idle;
    private VaccuumParticles _vp;
    
    #endregion

    private void Awake()
    {
        if (target == null) target = FindObjectOfType<PlayerBehaviour>().transform;

        _vp = GetComponent<VaccuumParticles>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        HandleCloudEnemyBehavior(dt);

        Vector2 dir = (target.position - transform.position).normalized;

        if (dir.x >= 0)
        {
            transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        }
        else if (dir.x < 0)
        {
            transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
        }
    }

    void HandleCloudEnemyBehavior(float dt)
    {
        switch (currentState)
        {
            case CloudEnemyStates.Idle:
                HandleIdle();
                break;
            
            case CloudEnemyStates.Chasing:
                HandleChasing(dt);
                break;
            
            case CloudEnemyStates.Attaching:
                HandleAttatching(dt);
                break;
            
            case CloudEnemyStates.OnPlayer:
                HandleOnPlayer();
                break;
            case CloudEnemyStates.Dying:
            {
                DumbFollowTarget(target, dt);
                transform.localScale -= Vector3.one * Time.deltaTime;
                if (transform.localScale.magnitude <= 1f || transform.localScale.x <= 0)
                {
                    PlayerBehaviour.UniquePlayer.Score += 25;
                    _vp.Activated = false;
                    Destroy(gameObject);
                }
                break;
            }
        }
    }

    void HandleChasing(float dt)
    {
        DumbFollowTarget(target, dt);
        ChangeToStateIf(CloudEnemyStates.Attaching, DistanceToTarget() < AttachDistance);
    }

    void HandleAttatching(float dt)
    {
        maxVelocity = 20f;
        DumbFollowTarget(target, dt);
        ChangeToStateIf(CloudEnemyStates.OnPlayer, DistanceToTarget() < OnPlayerDistance);
    }

    void HandleOnPlayer()
    {
        transform.position = target.position;
    }

    void HandleIdle()
    {
        ChangeToStateIf(CloudEnemyStates.Chasing, target != null);
    }

    private void ChangeToStateIf(CloudEnemyStates nextState, bool condition)
    {
        if (condition) currentState = nextState;
    }

    public void DoDestroy()
    {
        _vp.Activated = true;
        currentState = CloudEnemyStates.Dying;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerBehaviour.UniquePlayer.Hit();
        }
    }
}
