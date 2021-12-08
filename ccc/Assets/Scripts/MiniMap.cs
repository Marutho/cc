using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Transform center;
    // Start is called before the first frame update
    private void LateUpdate()
    {
        Vector3 newPostion = center.position;
        //newPostion.y = transform.position.y;
        transform.position = newPostion;
    }
}
