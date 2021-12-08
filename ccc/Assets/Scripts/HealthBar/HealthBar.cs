using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    #region Attributes

    public PlayerBehaviour player;
    
    [Range(0f, 100f)]
    public float slider = 0f;

    public Image faceKittyCat;
    public Image tailKittyCatTail;

    private float TailImageWidth => tailKittyCatTail.rectTransform.rect.width;

    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        //player = FindObjectOfType<PlayerBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        //var x = 1f - (slider / 100f);
        //var offset = new Vector2(x, 0f);
        //tailKittyCatTail.material.mainTextureOffset = offset;
        //
        //Debug.Log($"SLIDER: {slider}\tCALC: {x}\t OFFSET:{tailKittyCatTail.material.mainTextureOffset}");
    }
}
