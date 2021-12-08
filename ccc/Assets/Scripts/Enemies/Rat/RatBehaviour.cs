using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatBehaviour : Enemy
{

    // Start is called before the first frame update
    void Awake()
    {
        Physics2D.IgnoreCollision(target.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
