using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCatEnemy : Enemy
{
    #region Enums

    public enum ZombieCatEnemyStates
    {
        Idle,
        Chasing,
        Spitting,
        Dying
    }
    
    #endregion

    #region Attributes

    public GameObject projectile;

    public Transform spitPoint;

    public VaccuumParticles vacuumParticles;
    
    private const float SpittingFrequence = 5f;

    private float spittingTimer = 0f;

    private bool doneSpiting = false;

    private ZombieCatEnemyStates currentState = ZombieCatEnemyStates.Idle;

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        if (target == null) target = PlayerBehaviour.UniquePlayer.transform;

        maxVelocity = 1f;
        accel = 1f;

        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(LayerEnemy, 6);
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        Vector2 dir = (target.position - transform.position).normalized;
        
        if (dir.x >= 0)
        {
            transform.rotation = Quaternion.AngleAxis(180, Vector3.up);    
        }
        else if (dir.x < 0)
        {
            transform.rotation = Quaternion.AngleAxis(0, Vector3.up);    
        }    
    
        HandleZombieCatBehavior(dt);
    }
    
    void HandleZombieCatBehavior(float dt)
    {
        switch (currentState)
        {
            case ZombieCatEnemyStates.Idle:
                HandleIdle();
                break;
            
            case ZombieCatEnemyStates.Chasing:
                HandleChasing(dt);
                break;
            
            case ZombieCatEnemyStates.Spitting:
                HandleSpitting();
                break;
            
            case ZombieCatEnemyStates.Dying:
                //HandleDying();
                break;
        }
    }
    
    void ChangeToState(ZombieCatEnemyStates nextState, bool condition = true)
    {
        if (condition)
        {
            currentState = nextState;
            Debug.Log($"TrashBagEnemy: State changing to {nextState}");
        }
    }
    
    #region StateHandling

    void HandleIdle()
    {
        ChangeToState(ZombieCatEnemyStates.Chasing, target != null);
    }
    
    void HandleChasing(float dt)
    {
        spittingTimer += dt;
        Chase(dt);
        
        if (spittingTimer >= SpittingFrequence)
        {
            ChangeToState(ZombieCatEnemyStates.Spitting);
            spittingTimer = 0f;
        }
    }
    
    void HandleSpitting()
    {
        Spit();
    }
    
    void HandleDying()
    {
        
    }

    #endregion

    #region Behaviors

    void Spit()
    {
        GetComponent<Animator>().SetBool("Attacking", true);   
        SoundManager.instance.Spit(GetComponent<AudioSource>());
        GameObject spit = projectile;
        Spit spitComp = spit.GetComponent<Spit>();
        spitComp.target = DirectionToTargetNormalized();
        Instantiate(spit, spitPoint.position, Quaternion.identity);
        ChangeToState(ZombieCatEnemyStates.Chasing);
        GetComponent<Animator>().SetBool("Attacking", false);
    }

    void Chase(float dt)
    {
        FollowTargetSmart(target);
    }
    
    public void DoDestroy()
    {
        vacuumParticles.Activated = true;
        currentState = ZombieCatEnemyStates.Dying;
    }

    #endregion
}
