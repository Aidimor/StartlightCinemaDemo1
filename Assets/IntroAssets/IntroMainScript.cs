using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class IntroMainScript : MonoBehaviour
{
    [SerializeField] private MainController _scriptMain;
    //[SerializeField] private PauseController _scriptPause;
    public bool _shopActive;
    public Image _rotationBack;
    public bool _controllerOn;
    public float _rotationSpeed;
    public Animator _logoAnimator;
    public bool _gameStarted;

    public GameObject WinLosePanel;
    public Image _mainChar;
    public Sprite[] _charSprites;
    public TextMeshProUGUI _winLoseText;
    public GameObject _regresarButton;

   

    [System.Serializable]
    public class CandyShopAssets
    {
        public bool _CandyShopAvailable;
        public bool _changing;
        public int _onPos;
        public GameObject _parent;
        [System.Serializable]
        public class CandyShopPositions
        {
            public Vector3 _position;
            public float _scale;
            public Animator[] _chars;
            public int _onButton;
            public GameObject[] _buttons;
        
        }
        public CandyShopPositions[] _candyShopPositions;
        public bool _buttonChange;
    }
    public CandyShopAssets candyShopAssets;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartGameNumerator());   
       
    }

    public IEnumerator StartGameNumerator()
    {
                _logoAnimator.gameObject.SetActive(true);
                yield return new WaitForSeconds(1);
               _logoAnimator.SetBool("Starts", true);
                yield return new WaitForSeconds(2);
                while (!Input.GetButtonDown("Submit"))
                {
                    yield return null;
                }
                _logoAnimator.SetBool("Starts", false);
             StartCoroutine(_scriptMain._scriptSala.StartGameNumerator());            
        }

   
    void Update()
    {
        _rotationBack.transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);   

    }



   
}
