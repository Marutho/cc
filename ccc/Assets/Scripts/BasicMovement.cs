using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    
    private Vector2 _velocity;

    public Rigidbody2D Rb => _rb;

    public Vector2 Velocity
    {
        get => _velocity;
        set => _velocity = value;
    }

    public float Acceleration
    {
        get => _acceleration;
        set => _acceleration = value;
    }

    public float MaxVelocity
    {
        get => _maxVelocity;
        set => _maxVelocity = value;
    }

    [SerializeField] private float _accelerationFactor = 0.0f;

    [SerializeField]
    private float _maxVelocity = 5.0f;
    
    [SerializeField]
    private float _acceleration = 5.0f;
    
    [SerializeField]
    private float _maxSpeed = 5.0f;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 dir)
    {
        _acceleration += Time.deltaTime * _accelerationFactor;
        dir.Normalize();
        Vector2 velTarget = new Vector2(_maxSpeed * dir.x, dir.y * _maxSpeed);
        Vector2 velOffset = velTarget - _velocity;
        float dt = Time.deltaTime;
        velOffset = Vector2.ClampMagnitude(velOffset, _acceleration * dt);
        _velocity += velOffset;
        _rb.velocity = _velocity;
    }
}
