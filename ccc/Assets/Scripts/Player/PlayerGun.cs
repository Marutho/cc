using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerGun : MonoBehaviour
{
    #region Private

    private Camera camera;
    
    private ObstacleAttract _currentAttraction;

    private ObstacleAttract _currentAttractionBeingSucked;

    private int _ammo = 0;
    
    private ObstacleAttract[] _ammoList = new ObstacleAttract[10];

    [SerializeField]
    private int _availableAmmoSpace = 30;
    
    private RaycastHit2D[] hits = new RaycastHit2D[128];

    private float _timerBetweenAttractions = 0.2f;
    
    private float _maxtimerBetweenAttractions = .2f;

    private ParticleExplosion _pe;

    private Transform parent;

    [SerializeField] private float factorOfGrabbing = 1.0f;

    #endregion
    
    #region Public

    public int AvailableAmmoSpace => _availableAmmoSpace;
    
    public int Ammo => _ammo;
    
    #endregion
    
    #region Unity2D
    
    void Awake()
    {
        camera = Camera.main;
        parent = transform.parent;
        _pe = GetComponent<ParticleExplosion>();
    }

    void Update()
    {
        
        Vector3 dir = Input.mousePosition - camera.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if (PlayerBehaviour.UniquePlayer.CurrentState == PlayerBehaviour.State.MENU) return;
        bool canAttractItem = Input.GetMouseButtonDown(1) && _timerBetweenAttractions > _maxtimerBetweenAttractions;
        bool canShoot = Input.GetMouseButtonDown(0) && _timerBetweenAttractions > _maxtimerBetweenAttractions;
        
        _timerBetweenAttractions += Time.deltaTime;

        if (canShoot)
        {
            
            _timerBetweenAttractions = 0.0f;
            HandleShooting(dir);
            Debug.Log($"Tried shooting");
        }
        
        if (canAttractItem)
        {
            if (SoundManager.instance)
                SoundManager.instance.Vacuum(GetComponent<AudioSource>());
            _timerBetweenAttractions = 0.0f;
            bool _ = TryToAttract(transform.TransformDirection(Vector3.right));    
            Debug.Log($"Tried attraction {_}");
        }
        

        if (_currentAttraction)
        {
            
            Vector3 posAttraction = _currentAttraction.transform.position;
            Vector3 pos = parent.position - posAttraction;
            Vector3 dirPos = pos.normalized;
            if (_currentAttraction.CollidedWithPlayer)
            {
                HandleWhenAttractionDestroyed();
                GetComponent<AudioSource>().Stop();
            }   
            else 
                _currentAttraction.Move(dirPos);
        }

        if (_currentAttractionBeingSucked)
        {
            dir.Normalize();
            Vector3 absorbingPoint = transform.position;
            absorbingPoint.x += dir.x * 1f;
            absorbingPoint.y += dir.y * 1f;
            _currentAttractionBeingSucked.transform.position = absorbingPoint;
            _currentAttractionBeingSucked.transform.RotateAround(transform.position, Vector3.forward, 40f * Time.deltaTime);
            //_currentAttractionBeingSucked.transform.Rotate(Vector3.up * 5f * Time.deltaTime);
            _currentAttractionBeingSucked.transform.localScale -= Time.deltaTime * _currentAttractionBeingSucked.ScaleOriginal * 1.3f;
            _currentAttractionBeingSucked.Rb.velocity = Vector2.zero;
            if (_currentAttractionBeingSucked.transform.localScale.magnitude <= .1f)
            {
                SoundManager.instance.GetObject(GetComponent<AudioSource>());
                _currentAttractionBeingSucked.gameObject.SetActive(false);
                _currentAttractionBeingSucked = null;
            }
        }
     
    }
    
    #endregion

    #region Methods

    private void HandleShooting(Vector2 aim)
    {
        if (_ammo == 0 || _currentAttractionBeingSucked)
        {
            // No ammo
            return;
        }
        if (SoundManager.instance)
            SoundManager.instance.ShootObject(GetComponent<AudioSource>());
        _pe.VerticalSpread = transform.right.normalized;
        _pe.StartAnimation(1f);
        ObstacleAttract bullet = _ammoList[--_ammo];
        _availableAmmoSpace += bullet.spaceThatFills;
        bullet.gameObject.SetActive(true);

        Vector3 euler = bullet.transform.rotation.eulerAngles;
        euler.z = 0;
        bullet.SetOriginalScale();
        bullet.transform.eulerAngles = euler;
        Physics2D.IgnoreCollision(parent.GetComponent<BoxCollider2D>(), bullet.GetComponent<BoxCollider2D>(), false);
        Vector2 offset = parent.transform.position;
        Vector2 forwardNormalized = transform.right.normalized;
        offset.x += ((bullet.Box.bounds.size.x+.96f) / 2) * forwardNormalized.x;
        offset.y += ((bullet.Box.bounds.size.y+.86f)/ 2)  * forwardNormalized.y;
        bullet.transform.position = new Vector3(offset.x, offset.y);
        bullet.StartShoot(aim);
    }

    private void HandleWhenAttractionDestroyed()
    {
        _currentAttractionBeingSucked = _currentAttraction; 
        // _currentAttraction.gameObject.SetActive(false);
        _ammoList[_ammo++] = _currentAttraction;
        _currentAttraction = null;
    }

    private bool notifyNotEnoughSpace = true;

    private bool TryToAttract(Vector2 dir)
    {
        if (_ammo == _ammoList.Length)
        {
            return false;
        }
        if (_currentAttraction)
        {
            Physics2D.IgnoreCollision(parent.GetComponent<BoxCollider2D>(), _currentAttraction.Box, false);
            _currentAttraction.GoToNormal();
            _availableAmmoSpace += _currentAttraction.spaceThatFills;
            _currentAttraction = null;
            return false;
        }
 
        int n = Physics2D.RaycastNonAlloc(transform.position, dir, hits, 8.5f);
        for (int i = 0; i < n; ++i)
        {
            RaycastHit2D hit = hits[i];
            if (!hit.collider) continue;
            ObstacleAttract obstacle = hit.collider.gameObject.GetComponent<ObstacleAttract>();
            if (!obstacle)
            {
                CloudEnemy cloud = hit.collider.gameObject.GetComponent<CloudEnemy>();
                if (!cloud)
                    continue;
                cloud.DoDestroy(); 
                continue;
            }
            
            if (obstacle.factorMassDamage > factorOfGrabbing || obstacle.spaceThatFills > _availableAmmoSpace)
            {
                if (notifyNotEnoughSpace)
                {
                    notifyNotEnoughSpace = false;
                    Dialogue.Instance.Show("Hmm... I can't absorb this item, my cleaner is full!", () =>
                        {
                            notifyNotEnoughSpace = true;
                        });    
                }
                
                continue;
            }
            
            _currentAttraction = obstacle;
            _currentAttraction.StartAttract();
            _availableAmmoSpace -= obstacle.spaceThatFills;
            
            return true;
        }

        return false;
    }
    

    #endregion
}
