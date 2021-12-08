using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[RequireComponent(typeof(VaccuumParticles))]
[RequireComponent(typeof(BasicMovement))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ObstacleAttract : MonoBehaviour
{
    public enum ObstacleType
    {
        Normal
    }

    [SerializeField] private ObstacleType type;
    
    private BasicMovement _movement;

    public bool shooting = false;
    public bool attracting = false;

    private Vector2 _targetPos = Vector2.zero;

    private ObstacleAttract _copy;
    
    public const int LayerObstacles = 7;
    public const int LayerObstaclesStatic = 9;

    [SerializeField] public float factorMassDamage = 1.0f;

    [SerializeField] private float rotationSpeedShoot = 0.0f;
    [SerializeField] private float rotationSpeedAttract = 0.0f;
    [SerializeField] private float speedForce = 0.0f;
    [SerializeField] private int durability = 10;

    private Shadow _shadow;
    
    private float _dragValue = 0;

    private Vector3 scaleOriginal;
    
    /// <summary>
    /// Set here the space that it will fill in the player
    /// </summary>
    [SerializeField] public int spaceThatFills = 10;
    
    private VaccuumParticles _vp;

    public Vector3 ScaleOriginal => scaleOriginal;
    private BoxCollider2D _box;
    private Rigidbody2D _rb;
    private bool _collidedWithPlayer = false;
    public bool CollidedWithPlayer => _collidedWithPlayer;
    public BoxCollider2D Box => _box;

    public Rigidbody2D Rb => _rb;

    public bool obstacleParent = false;
    
    private void Awake()
    {
        _movement = GetComponent<BasicMovement>();
        _box = GetComponent<BoxCollider2D>();
        _vp = GetComponent<VaccuumParticles>();
        _rb = GetComponent<Rigidbody2D>();
        _shadow = GetComponentInChildren<Shadow>();
        
        scaleOriginal = transform.localScale;
        _dragValue = _rb.drag;
           
        // Physics2D.IgnoreLayerCollision(7, 7);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!obstacleParent)
        {
            obstacleParent = true;
            ObstacleAttract attract = Instantiate(this);
            attract.obstacleParent = true;
            _copy = attract;
            _copy.transform.position = gameObject.transform.position;
            _copy.gameObject.SetActive(false);
        }
        
        if (attracting)
        {
            //_movement.Rb.AddTorque(rotationSpeedAttract, ForceMode2D.Impulse);
        }

        if (shooting)
        {
            //_movement.Rb.AddTorque(rotationSpeedShoot, ForceMode2D.Impulse);
            // Move(_targetPos);
        }
    }
    
    public void Move(Vector2 dir)
    {
        _movement.Move(dir);
    }

    public void StartAttract()
    {
        Box.isTrigger = true;
        attracting = true;
        shooting = false;
        _movement.Rb.constraints = RigidbodyConstraints2D.None;
        _collidedWithPlayer = false;
        _vp.Activated = true;
        WeakenDrag();
        if (_shadow) _shadow.gameObject.SetActive(false);
        //_movement.Rb.AddTorque(130.0f);
    }

    public void SetOriginalScale()
    {
        transform.localScale = scaleOriginal;
    }

    public void StartShoot(Vector2 attackPos)
    {
        if (_shadow) _shadow.gameObject.SetActive(false);
        Box.isTrigger = false;
        _movement.Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        shooting = true;
        _vp.Activated = false;
        _collidedWithPlayer = false;
        _targetPos = attackPos;
        _movement.Velocity = Vector2.zero;
        _movement.Rb.velocity = Vector2.zero;
        _movement.Rb.AddForce(attackPos * speedForce, ForceMode2D.Impulse);
    }

    public void GoToNormal()
    {
        if (_shadow) _shadow.gameObject.SetActive(true);
        Box.isTrigger = false;
        _vp.Activated = false;
        _movement.Rb.velocity = Vector2.zero;
        _movement.Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        shooting = false;
        attracting = false;
        _movement.Velocity = Vector2.zero;
        transform.rotation = Quaternion.identity;
        _collidedWithPlayer = false;
        StrengthDrag();
        foreach (var path in FindObjectsOfType<Grid>())
        {
            path.regenerate = true;
        }
    }

    
    
    public void WeakenDrag()
    {
        _rb.drag = 0.0f;
    }

    public void StrengthDrag()
    {
        if (_shadow) _shadow.gameObject.SetActive(true);
        _rb.drag = _dragValue;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && attracting)
        {
            _collidedWithPlayer = true;
        }
        
        if (shooting && !other.gameObject.CompareTag("Player"))
        {


            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (enemy)
            {
                enemy.OnHit(this);    
            }
            else
            {
                foreach (var path in FindObjectsOfType<Grid>())
                {
                    path.regenerate = true;
                }
                shooting = false;
                StrengthDrag();
            }
            
            // TODO Check here the mass of the enemy and see if this item is bigger than it
            if (enemy)
                durability -= 5 * (int) enemy.factorMass;
            else
            {
                durability -= 20;
            }
            
            //StrengthDrag();
            if (durability <= 0)
            {
                if (ObstacleManager.Instance)
                    ObstacleManager.Instance.Enqueue(_copy);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && attracting)
        {
            _collidedWithPlayer = true;
        }
    }
}
