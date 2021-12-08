using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceStationary : MonoBehaviour
{
    [SerializeField]
    private Image _full;
    [SerializeField]
    private Image _half;
    [SerializeField]
    private Image _end;

    private Image img;
    
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        SetThis(_full, PlayerBehaviour.UniquePlayer.HP >= 60);
        SetThis(_half, PlayerBehaviour.UniquePlayer.HP < 60 && PlayerBehaviour.UniquePlayer.HP >= 0f);
        SetThis(_end, PlayerBehaviour.UniquePlayer.HP <= 0.1f);
        
     //   img.material.mainTextureOffset = Vector2.zero;
    }

    void SetThis(Image t, bool to)
    {
        t.gameObject.SetActive(to);
    }
}
