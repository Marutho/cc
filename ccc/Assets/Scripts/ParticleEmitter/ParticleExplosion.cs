using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class ParticleExplosion : MonoBehaviour
{    
     #region Structs

    /// <summary>
    /// Represents a Particle in our Script.
    /// </summary>
    private struct Particle
    {
        public GameObject Instance;
        public Vector2    OriginalPos;
        public Vector2    Dir;
    }

    #endregion
    
    #region Private

    [SerializeField] private GameObject _prefab;
    
    /// <summary>
    /// How many particles
    /// </summary>
    [SerializeField] private int   poolSize;
    /// <summary>
    /// Gravity. Self explanatory.
    /// </summary>
    [SerializeField] private float gravity = 9.8f;
    /// <summary>
    /// Speed of the explosion. Bigger values bigger explosions
    /// </summary>
    [SerializeField] private float speed = 3.0f;
    /// <summary>
    /// Beginning impulse for the explosion for bigger initial velocities.
    /// Recommended values: 5-10f
    /// </summary>
    [SerializeField] private float impulse = 10.0f;
    /// <summary>
    /// Vertical spreading. X represents the min. value and Y the max. value
    /// </summary>
    [SerializeField] private Vector2 verticalSpread = new Vector2(-10f, 20f);
    
    
    private Vector2      _acceleration;
    private Vector2      _velocity;
    private Particle[]   _particles;
    private float        _resetTime;
    private float        _elapsedT;
    private bool         _moving;
    
    #endregion

    #region Public
    

    #endregion

    #region Properties
    
    /// <summary>
    /// If the animation is still playing
    /// </summary>
    public bool IsPlaying => _moving;

    public Vector2 VerticalSpread
    {
        get => verticalSpread;
        set => verticalSpread = value;
    }

    public float Impulse
    {
        get => impulse;
        set => impulse = value;
    }

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public float Gravity
    {
        get => gravity;
        set => gravity = value;
    }
    
    #endregion
    
    #region MonoBehaviour
    
    private void Awake()
    {
        _particles = new Particle[poolSize];
        Initialize();
    }

    private void Update()
    {
        if (!_moving) return;
        float dt = Time.deltaTime;
        _elapsedT += dt;
        if (_elapsedT >= _resetTime)
        {
            _moving = false;
            SetActive(false);
            return;
        }
        for (int i = 0; i < poolSize; ++i)
        {
            UpdateParticle(_particles[i]);
        }
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Starts the animation (if it's not started yet)
    /// </summary>
    /// <param name="resetTime">How many seconds until the animation stops</param>
    /// <param name="resetRandomValues">If you want a different explosion every time this is called (This comes with more computational cost)</param>
    public void StartAnimation(float resetTime, bool resetRandomValues = true)
    {
        if (_moving) return;
        if (resetRandomValues) Initialize();
        _resetTime     = resetTime;
        _moving = true;
        _elapsedT = 0.0f;
        SetActive(true);
    }

    #endregion
    
    #region Private Methods

    /// <summary>
    /// Initializes the explosion
    /// </summary>
    private void Initialize()
    {
        Vector2 pos = transform.position;
        float delta = 2f * Mathf.PI / poolSize;
        float theta = 0.0f;
        for (int i = 0; i < poolSize; ++i)
        {
            _particles[i].Instance = Instantiate(_prefab);
            _particles[i].Dir = new Vector3(
                (impulse + Random.Range(-0.5f * impulse, 0.5f * impulse)) * Mathf.Cos(theta) + verticalSpread.x, 
                verticalSpread.y
            ).normalized;
            _particles[i].Instance.transform.SetParent(transform);
            _particles[i].OriginalPos = pos;
            _particles[i].Instance.SetActive(false);
            theta += delta;
        }
    }

    /// <summary>
    /// Sets the active variable in the particles gameObjects
    /// </summary>
    /// <param name="active">Activate or deactivate</param>
    private void SetActive(bool active)
    {
        for (int i = 0; i < poolSize; ++i)
        {
            _particles[i].Instance.SetActive(active);
        }
    }

    /// <summary>
    /// Formula that returns the next position of a particle
    /// </summary>
    /// <param name="originalP">Original position of the particle</param>
    /// <param name="dir">Direction of the particle</param>
    /// <returns>The next position frame</returns>
    private Vector3 CalculateNextFrame(Vector2 originalP, Vector2 dir)
    {
        _acceleration = Vector3.up * (0.5f * gravity * _elapsedT * _elapsedT);
        _velocity = dir * (speed * _elapsedT);
        Vector2 r = originalP + _velocity + _acceleration;
        return r;
    }

    /// <summary>
    /// Updates desired particle
    /// </summary>
    /// <param name="particle">The particle to update</param>
    private void UpdateParticle(Particle particle)
    {
        particle.Instance.transform.position = CalculateNextFrame(particle.OriginalPos, particle.Dir);
    }

    private void OnDestroy()
    {
        foreach (var particle in _particles)
        {
            Destroy(particle.Instance);
        }
    }

    #endregion
}
