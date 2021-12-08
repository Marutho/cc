using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spit : MonoBehaviour
{
    public float speed = 20f;
    public float lifespan = 2f;
    
    public Vector3 target;

    private BoxCollider2D box;
    
    void Start()
    {
        //target = FindObjectOfType<PlayerBehaviour>().transform.position;
        Destroy(gameObject, lifespan);
        box = GetComponent<BoxCollider2D>();
        
    }
    
    void Update()
    {
        float dt = Time.deltaTime;

        if (target != null) LinearShot(dt);
    }

    void LinearShot(float dt)
    {
        transform.position += target.normalized * (speed * dt);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerBehaviour.UniquePlayer.Hit();
            Destroy(gameObject);
        }
    }
}
