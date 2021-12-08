using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TrashBagEnemy : Enemy
{
    #region Enums

    public enum TrashBagEnemyStates
    {
        Idle = 0,
        Wobbling,
        Charging,
        Rumbling,
        Dizzy,
        KnockBack,
        Dying,
        Dead
    }

    #endregion

    #region Attributes

    private const float CloseEnoughToCharge = 5f;
    private const float ChargingTime = 3f;
    private const float KnockBackTime = 5f;
    private const float DizzyTime = 5f;

    private float currentChargingTime = 0f;
    private float currentKnockBackTime = 0f;
    private float currentDizzyTime = 0f;
    
    private bool chargeComplete = false;
    private bool knockBackComplete = false;
    private bool doneKnockBack = false;
    
    private float spinSpeed = 720f;
    
    private TrashBagEnemyStates currentState = TrashBagEnemyStates.Idle;
    private VaccuumParticles vacuumParticles;

    #endregion

    void Awake()
    {
        if (target == null) target = FindObjectOfType<PlayerBehaviour>().transform;

        vacuumParticles = GetComponent<VaccuumParticles>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        HandleTrashBagEnemyBehavior(dt);
        /*PlayerBehaviour player = IsHittingPlayer();
        if (player)
        {
            player.Hit();
        }*/
    }

    void HandleTrashBagEnemyBehavior(float dt)
    {
        switch (currentState)
        {
            case TrashBagEnemyStates.Idle:
                HandleIdle();
                break;
            
            case TrashBagEnemyStates.Wobbling:
                HandleWobbling(dt);
                break;
            
            case TrashBagEnemyStates.Charging:
                HandleCharging(dt);
                break;
            
            case TrashBagEnemyStates.Rumbling:
                HandleRumbling(dt);
                break;
            
            case TrashBagEnemyStates.Dizzy:
                HandleDizzy(dt);
                break;
            
            case TrashBagEnemyStates.KnockBack:
                HandleKnockBack(dt);
                break;
            
            case TrashBagEnemyStates.Dying:
                //HandleOnPlayer();
                break;
        }
    }

    #region State Management
    
    void HandleIdle()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        transform.rotation = Quaternion.identity;
        maxVelocity = 5f;
        accel = 3f;
        vel = Vector2.zero;
        doneKnockBack = false;
        ChangeToState(TrashBagEnemyStates.Wobbling, target != null);
    }

    void HandleWobbling(float dt)
    {
        DumbFollowTarget(target, dt);
        ChangeToState(TrashBagEnemyStates.Charging, DistanceToTarget() < CloseEnoughToCharge);
    }
    
    void HandleCharging(float dt)
    {
        rb.velocity = Vector2.zero;
        ChargeRumbling(dt);
        ChangeToState(TrashBagEnemyStates.Rumbling, chargeComplete);
        FinishCharge();
    }

    void HandleRumbling(float dt)
    {
        Rumble(target, dt);
    }

    void HandleKnockBack(float dt)
    {
        KnockBack(dt);
        ChangeToState(TrashBagEnemyStates.Idle, knockBackComplete);
        FinishKnockBack();
    }

    void HandleDizzy(float dt)
    {
        Dizzy(dt);
    }

    void ChangeToState(TrashBagEnemyStates nextState, bool condition = true)
    {
        if (condition)
        {
            currentState = nextState;
            Debug.Log($"TrashBagEnemy: State changing to {nextState}");
        }
    }

    #endregion
    
    #region Behaviors

    void Dizzy(float dt)
    {
        currentDizzyTime += dt;
        if (currentDizzyTime >= DizzyTime)
        {
            currentDizzyTime = 0f;
            ChangeToState(TrashBagEnemyStates.Idle);
        }
    }

    void KnockBack(float dt)
    {
        if (!doneKnockBack)
        {
            rb.AddForce(-DirectionToTargetNormalized() * 2f, ForceMode2D.Impulse);
            doneKnockBack = true;
        }
        currentKnockBackTime += dt;
        if (currentKnockBackTime >= KnockBackTime)
        {
            knockBackComplete = true;
        }
    }

    void FinishKnockBack()
    {
        if(!knockBackComplete) return;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        knockBackComplete = false;
        currentKnockBackTime = 0f;
        doneKnockBack = false;
    }
    
    void Rumble(Transform t, float dt)
    {
        maxVelocity = 10f;
        accel = 10f;
        Move(DirectionToTargetNormalized(), dt);
    }

    void ChargeRumbling(float dt)
    {
        currentChargingTime += dt;
        Spin(dt);
        if (currentChargingTime > ChargingTime)
        {
            chargeComplete = true;
        }
    }

    void FinishCharge()
    {
        if (!chargeComplete) return;
        currentChargingTime = 0f;
        chargeComplete = false;
        transform.rotation = Quaternion.identity;
    }
    
    public void DoDestroy()
    {
        vacuumParticles.Activated = true;
        currentState = TrashBagEnemyStates.Dying;
    }

    public void Spin(float dt)
    {
        transform.Rotate(0f, 0f, spinSpeed * dt);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (currentState == TrashBagEnemyStates.Rumbling)
        {
            bool isPlayer = other.transform.gameObject.CompareTag("Player");
            ChangeToState(isPlayer
                ? TrashBagEnemyStates.KnockBack
                : TrashBagEnemyStates.Dizzy);
            if (isPlayer)
            {
                PlayerBehaviour.UniquePlayer.Hit();
            }
        }
    }

    #endregion


}
