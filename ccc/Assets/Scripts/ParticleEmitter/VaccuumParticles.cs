using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaccuumParticles : MonoBehaviour
{
    private PlayerGun _player;
    [SerializeField] private VaccuumParticle particle;

    private float _timer = 0.0f;
    private float _maxTimerBetween = 0.0076f;

    private bool _activated = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        _player = FindObjectOfType<PlayerGun>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_activated) return;
        _timer += Time.deltaTime;
        if (_timer >= _maxTimerBetween)
        {
            Spawn();    
            _timer = 0f;
        }
        
    }

    void Spawn()
    {
        Vector2 pos = transform.position;
        float x = Random.Range(-.8f, .8f) + pos.x;
        float y = Random.Range(-.8f, .8f) + pos.y;
        VaccuumParticle particleInstance = Instantiate(particle);
        particleInstance.Player = _player;
        particleInstance.transform.position = new Vector2(x, y);
    }

    public bool Activated
    {
        get => _activated;
        set => _activated = value;
    }
}
