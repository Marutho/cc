using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerBehaviour : MonoBehaviour
{

    public enum State
    {
        NORMAL,
        MENU,
    }

    
    #region Private 
    
    private ParticleExplosion _pExplosion;
    
    [SerializeField]
    private State _currentState = State.NORMAL;

    private PlayerMovement _pm;

    [SerializeField]
    private float _hp = 100f;

    private GridComponent grid;

    public GridComponent Grid => grid;

    /// <summary>
    /// Menu stuff
    /// </summary>
    public GameObject[] points;
    
    public int currentPoint;

    public static PlayerBehaviour UniquePlayer;

    [SerializeField]
    private PlayerGun _gun;

    private PlayerSprite _ps;

    private float _timerHit = 1.0f;
    
    private float _maxTimerHit = 1.0f;

    private int _score = 0;

    private float _timerColor = 0.0f;
    private float _maxTimerColor = 0.5f;
    private bool _pingColor = false;

    #endregion

    public PlayerGun Gun => _gun;

    public float HP
    {
        get => _hp;
        set => _hp = value;
    }

    private float _maxHp;

    public float MaxHp => _maxHp;

    public int Score
    {
        get => _score;
        set => _score = value;
    }

    public State CurrentState => _currentState;


    private void Awake()
    {
        _maxHp = _hp;
        _ps = GetComponent<PlayerSprite>();
        _pExplosion = GetComponent<ParticleExplosion>();
        _pm = GetComponent<PlayerMovement>();
        points = GameObject.FindGameObjectsWithTag("Terrain");
        UniquePlayer = this;
    }

    private SpriteRenderer _left;
    private SpriteRenderer _right;

    void Start()
    {
        _left = _ps.PlayerLeft.GetComponent<SpriteRenderer>();
        _right = _ps.PlayerRight.GetComponent<SpriteRenderer>();
    }

    private bool _dead = false;

    public bool Dead => _dead;

    public Color colorWhenHit;
    
    // Update is called once per frame
    void Update()
    {
        if (_dead)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(2);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                SceneManager.LoadScene(1);
            }
            return;
        }
        //Color c = new Color(246,4,210);
        Color c =  colorWhenHit;
        ;
        if (_timerHit < _maxTimerHit)
        {
            _timerColor += Time.deltaTime;
            if (_timerColor >= _maxTimerColor)
            {
                _timerColor = 0.0f;
                _pingColor = !_pingColor;
            }
            if (!_pingColor)
            {
                _left.color = c;
                _right.color = c;
            }
            else
            {
                _left.color = Color.white;
                _right.color = Color.white;
            }
        }
        else
        {
            _left.color = Color.white;
            _right.color = Color.white;
        }
        if (_hp <= 0f)
        {
            _pm.Rb.isKinematic = true;
            _pm.Rb.velocity = Vector2.zero;
            _dead = true;
            Dialogue.Instance.Show("Press 'R' to restart... \n'Q' to exit the game", () => {});
            _pExplosion.StartAnimation(10f, true);
            Destroy(_gun.gameObject);
            Destroy(_pm);
            Destroy(_ps);
           // Destroy(this);
            Destroy(_pExplosion, 5f);
            return;
        }

        _timerHit += Time.deltaTime;
        switch (_currentState)
        {
            case State.NORMAL:
            {
                HandleNormalState();
                break;
            }

            case State.MENU:
            {
                HandleMenuState();
                break;
            }
        }
    }

    public void Hit()
    {
//        Debug.Log("hit");
        if (_timerHit >= _maxTimerHit)
        {
            _hp -= 10f;
            _timerHit = 0.0f;
        }
  
    }

    private void HandleNormalState()
    {
        _pm.HandleMove();
        _pm.HandleDash();
    }


    #region Menu

    private float _timerWait = 0.0f;
    private float _maxTimerWait = 5.0f;
    
    private void HandleMenuState()
    {
        _pm.Move(Vector2.right);
        /*Transform p = points[currentPoint].transform;
        Vector2 distance = p.transform.position - transform.position;
        if (distance.magnitude <= 1f)
        {
            _pm.Move(Vector2.zero);
            _timerWait += Time.deltaTime;
            if (_timerWait >= _maxTimerWait)
            {
                _timerWait = 0;
                int nextPoint = Random.Range(0, points.Length - 1);
                currentPoint = nextPoint;
            } 
        }
        else
        {
            _pm.Move(distance.normalized);
        }*/
        
    }

    #endregion
}
