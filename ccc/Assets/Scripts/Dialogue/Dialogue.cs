using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public delegate void NotifyWhenFinish();

public class Dialogue : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Sprite textureCatOpen;
    [SerializeField] private Sprite textureCatClosed;
    [SerializeField] private Image _imageCat;
    
    
    private static Dialogue _instance;

    public static Dialogue Instance => _instance;

    private bool _showing = false;

    private string _currentText = "";

    private string _toShowText = "";

    private float _timerNextCharacter = 0.0f;

    private float _maxTimeNextChar = 0.08f;

    private float _timerEnd = 0.0f;
    
    private float _maxTimerEnd = 6.0f;

    private int _currentChar = 0;

    private bool _wrappingUp = false;

    private NotifyWhenFinish _notify;

    private float _imageCatTimer = 0.0f;

    private float _maxImageCatTimer = 1.3f;

    private bool _openedEyes = true;
    
    void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        gameObject.SetActive(false);
    }

    
    void Update()
    {
        if (!_showing) return;

        _imageCatTimer += Time.deltaTime;
        if (_imageCatTimer >= _maxImageCatTimer)
        {
            _imageCatTimer = 0.0f;
            if (_openedEyes)
            {
                _imageCat.sprite = textureCatClosed;
                _maxImageCatTimer = 0.4f;
                _openedEyes = false;
            }
            else
            {
                _imageCat.sprite = textureCatOpen;
                _maxImageCatTimer = 1.3f;
            }
        }
        
        if (_wrappingUp)
        {
            _timerEnd += Time.deltaTime;
            if (_timerEnd >= _maxTimerEnd)
            {
                _timerEnd = 0.0f;
                _wrappingUp = false;
                _showing = false;
                _notify();
                gameObject.SetActive(false);
            }
            return;
        }
        
        _timerNextCharacter += Time.deltaTime;
        if (_timerNextCharacter >= _maxTimeNextChar)
        {
            _timerNextCharacter = 0.0f;
            _currentText += _toShowText[_currentChar++];
            text.text = _currentText;
        }

        if (_currentText.Length == _toShowText.Length)
        {
            _wrappingUp = true;
            _timerNextCharacter = 0.0f;
            _currentChar = 0;
        }
        
    }


    public void Show(string textToSet, NotifyWhenFinish notify)
    {
        gameObject.SetActive(true);
        _showing = true;
        _timerNextCharacter = 0.0f;
        _notify = notify;
        _timerEnd = 0.0f;
        _wrappingUp = false;
        text.text = "";
        _currentText = "";
        _toShowText = textToSet;
    }
    
    
    
}
