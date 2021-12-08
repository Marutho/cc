using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BasicMovement))]
public class VaccuumParticle : MonoBehaviour
{
    private PlayerGun _player;
    private BasicMovement _bm;

    public PlayerGun Player
    {
        get => _player;
        set => _player = value;
    }

    private void Start()
    {
        _bm = GetComponent<BasicMovement>();
    }

    private void Update()
    {
        if (!_player)
        {
            Destroy(gameObject, 10f);
            return;
        }
        _bm.Move(_player.transform.position - transform.position);
        if ((_player.transform.position - transform.position).magnitude <= 3f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) Destroy(gameObject);
    }
}
