using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGunCapacity : MonoBehaviour
{
    
    private TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        text.text = $"Gun Capacity: {PlayerBehaviour.UniquePlayer.Gun.AvailableAmmoSpace}";
    }
}
