using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    #region Public

    public GameObject playerRight;
    public GameObject playerLeft;

    #endregion
    
    #region Private

    [SerializeField]
    private Transform center;

    private Rigidbody2D _rb;
    
    private BoxCollider2D _boxCollider2D;

    private Vector2 _velocity;
    
    [SerializeField]
    private float _maxVelocity = 5.0f;
    
    [SerializeField]
    private float _acceleration = 5.0f;
    
    [SerializeField]
    private float _maxSpeed = 5.0f;

    private bool _dashing = false;

    private Vector2 _dashDirection;

    private float _timerDash = 0.0f;

    private bool _canDash = true;

    [SerializeField]
    private float _maxTimerDash = .3f;

    private float _dashTimer = 0.0f;
    
    [SerializeField]
    private float _maxDashTimer = 1.3f;

    public Animator animatorRight;
    public Animator animatorLeft;
    public bool rightSide;

    public Rigidbody2D Rb => _rb;
    #endregion

    #region Methods Unity3D

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();

        animatorRight = playerRight.GetComponent<Animator>();
        animatorLeft = playerLeft.GetComponent<Animator>();
        rightSide = true;
    }

    void Start()
    {
    }

    
    void Update()
    {
        if (!_dashing && !_canDash)
        {
            _dashTimer += Time.deltaTime;
            if (_dashTimer >= _maxDashTimer)
            {
                _dashTimer = 0.0f;
                _canDash = true;
            }
        }
        if (_dashing)
        {
            _timerDash += Time.deltaTime;
            _rb.velocity = _dashDirection * 10f;
            if (_timerDash >= _maxTimerDash)
            {
                _timerDash = 0.0f;
                _dashing = false;
            }
        }

        if (_velocity.x != 0 || _velocity.y != 0)
        {
            if (playerRight.active == true)
            {
                animatorRight.SetBool("RightSide", true);
                animatorRight.SetBool("Moving", true);
            }
            else
            {
                animatorLeft.SetBool("RightSide", false);
                animatorLeft.SetBool("Moving", true);
            }
        }
        else
        {
            animatorRight.SetBool("Moving", false);
            animatorLeft.SetBool("Moving", false);
        }
    }
    
    #endregion
    
    #region Methods

    public void HandleDash()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical= Input.GetAxisRaw("Vertical");
        float dash = Input.GetAxisRaw("Jump");
        if (dash >= 0.1f && _canDash)
        {
            _dashDirection = new Vector2(horizontal, vertical).normalized;
            _dashing = true;
            _canDash = false;
        }
    }

    public void HandleMove()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical= Input.GetAxisRaw("Vertical");
        Move(new Vector2(horizontal, vertical));
    }

    public void Move(Vector2 dir)
    {
        if (dir.magnitude >= 0.5f)
        {
           // Grid.Instance.regenerate = true;
        }
        if (_dashing) return;
        dir.Normalize();
        Vector2 velTarget = new Vector2(_maxSpeed * dir.x, dir.y * _maxSpeed);
        Vector2 velOffset = velTarget - _velocity;
        float dt = Time.deltaTime;
        velOffset = Vector2.ClampMagnitude(velOffset, _acceleration * dt);
        _velocity += velOffset;
        _rb.velocity = _velocity;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        Collider2D collider2D = other.collider;
        
        // Debug.Log("[COLLIDER] Collided");
        if (collider2D.CompareTag("ColliderUnmovable"))
        {
         //   Debug.Log("[COLLIDER] CollidedUnmovable");
            string side = WhatSideOfTheColliderWasHit(other.collider);
            // Debug.Log($"Side: {side}");
            if (side == "Bottom")
            {
                _velocity.y = Mathf.Clamp(_velocity.y, -_maxVelocity, 0);
            }

            if (side == "Top")
            {
                _velocity.y = Mathf.Clamp(_velocity.y, 0, _maxVelocity);
            }

            if (side == "Left")
            {
                _velocity.x = Mathf.Clamp(_velocity.x, -_maxVelocity, 0);
            }

            if (side == "Right")
            {
                _velocity.x = Mathf.Clamp(_velocity.x, 0, _maxVelocity);
            }
            
        }
    }
    
    private RaycastHit2D[] rays = new RaycastHit2D[128];
    
    private string WhatSideOfTheColliderWasHit(Collider2D collision, string expects = "ColliderUnmovable")
    {
        Vector2 center = this.center.position;
        
        int hit = Physics2D.RaycastNonAlloc(center, Vector2.right, rays);
        if (CheckRaycastsCollider(rays, collision, hit))
        {
            return "Left";
        }
        hit = Physics2D.RaycastNonAlloc(center, Vector2.left, rays);
        if (CheckRaycastsCollider(rays, collision, hit))
        {
            return "Right";
        }
        hit = Physics2D.RaycastNonAlloc(center, Vector2.up, rays);
        if (CheckRaycastsCollider(rays, collision, hit))
        {
            return "Bottom";
        }
        hit = Physics2D.RaycastNonAlloc(center, Vector2.down, rays);
        if (CheckRaycastsCollider(rays, collision, hit))
        {
            return "Top";
        }
        return "";
    }

    private bool CheckRaycastsCollider(RaycastHit2D[] hits, Collider2D collision, int nHits)
    {
        for (int i = 0; i < nHits; i++)
        {
            RaycastHit2D ray = hits[i];
            if (ray.collider != null && ray.collider == collision)
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}
