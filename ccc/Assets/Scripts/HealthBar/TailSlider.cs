using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TailSlider : MonoBehaviour
{
    #region Enums

    enum HealthBarStates
    {
        Idle,
        Changing
    }

    #endregion
    public PlayerBehaviour player;
    private HealthBarStates currentState;

    [Range(0f, 100f)]
    public float slider = 0f;
    public bool usePlayerHealth = false;
    private Image img;
    private SpriteRenderer sr;

    private float previousHealth = 100f;
    private float healthDiff = 0f;
    private float changeDuration = 0.5f;
    private float changeTimer = 0f;


    private float _lerpTimer = 0.0f;
    private float _target = 0.0f;
    private float _from = 0.0f;
    private float _current = 0.0f;
    private float _prev = 0.0f;
    private bool _doingLerp = false;

    private void Start()
    {
        if (usePlayerHealth)
        {
            player = PlayerBehaviour.UniquePlayer;
            previousHealth = player.HP;
        }
        img = GetComponent<Image>();
    }

    private float _reheal = 0.0f;
    private float _maxReheal = 10.0f;
    
    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        _reheal += dt;
        //HandleHealthBar(dt);
        if (_reheal >= _maxReheal && !PlayerBehaviour.UniquePlayer.Dead)
        {
            PlayerBehaviour.UniquePlayer.HP = Math.Min(PlayerBehaviour.UniquePlayer.MaxHp, PlayerBehaviour.UniquePlayer.HP + 10);
            _reheal = 0.0f;
        }
        float offset = usePlayerHealth ? player.HP : slider;
        if (_doingLerp)
        {
            _current = Mathf.Lerp(_from, _target, _lerpTimer);
            _lerpTimer += dt * 2f;
            if (_lerpTimer > 1f)
            {
                _current = _target;
                _lerpTimer = 0.0f;
                _target = 0.0f;
                _from = 0.0f;
                _doingLerp = false;
            }
        }
        else if (_prev != player.HP)
        {
            _lerpTimer = 0.0f;
            _target = offset;
            _from = _prev;
            _current = _prev;
            _doingLerp = true;
        }
        else
        {
            _current = offset;
        }
        _prev = player.HP;
        img.rectTransform.position = transform.parent.position + Vector3.left * (459 * 2f) + Vector3.right * _current * 10f + Vector3.up * (12.5f * 2f) ;
    }

    void HandleHealthBar(float dt)
    {
        //if (usePlayerHealth)
        {
            switch (currentState)
            {
                case HealthBarStates.Idle:
                    HandleIdle();
                    break;

                case HealthBarStates.Changing:
                    HandleChanging(dt);
                    break;
            }
        }
        //else
        {
            //img.rectTransform.position = transform.parent.position + Vector3.left * (500 - slider * 5f);
        }
    }

    void HandleIdle()
    {
        //sr.flipX = false;
        healthDiff = previousHealth - slider; //player.HP;
        ChangeStateIf(HealthBarStates.Changing, healthDiff > 0.5f);
    }

    void HandleChanging(float dt)
    {
        //Debug.Log("Supposedly moving!");
        Vector3 origin = img.rectTransform.position;
        Vector3 target = origin + Vector3.left * healthDiff;

        while (changeTimer <= changeDuration)
        {
            float elapsed = EaseOutExponential(changeTimer / changeDuration);
            img.rectTransform.position = Vector3.Lerp(origin, target, elapsed);
            Debug.Log(elapsed);
            changeTimer += dt;
        }
    }

    float EaseOutExponential(float x)
    {
        return (x < 1.01f && x > 0.99f) ? 1 : 1 - Mathf.Pow(2, -10 * x);
    }

    void ChangeStateIf(HealthBarStates nextState, bool condition = true)
    {
        if (condition) currentState = nextState;
    }
}
