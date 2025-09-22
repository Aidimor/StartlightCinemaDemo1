using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissingLetterScript : MonoBehaviour
{
    [SerializeField] private MainController _scriptMain;
    public int _wordChoosed;
    public string[] _allWords; // Assign in Inspector
    public TextMeshProUGUI _wordText; // Assign in Inspector
    public List<GameObject> lettersParent = new List<GameObject>();
    public List<string> letters = new List<string>();
    public GameObject _letterParent;
    public int _missingLetter;
    public float _speed;
    public TextMeshProUGUI[] _letterOptions;
    public int _onLetterOption;
    public int _correctLetterPos;
    public GameObject _cursor;

    bool _moved;
    public bool _optionChoosed;
    public bool _Win;
    public Image _resultPoint;
    public string selectedWord;
    public Image _girl;
    public GameObject _back;
    public Vector3[] _scales;
    public int _onScale;
 
    void Start()
    {
      
    }

    public void ActivationWord()
    {
        _scriptMain = GameObject.Find("MainController").gameObject.GetComponent<MainController>();
        StartCoroutine(ActivationWordNumerator());
    }

    public IEnumerator ActivationWordNumerator()
    {
        _back.transform.localScale = _scales[0];
        int randomWord = Random.Range(0, _allWords.Length);
        selectedWord = _allWords[randomWord];
        _wordChoosed = randomWord;    
 
        StartCoroutine(SplitWordSlowly(selectedWord));
        _onLetterOption = 1;
    
        yield return new WaitForSeconds(1f / _scriptMain._onLevel);
        _onScale = 1;

    
 
        transform.parent.GetComponent<GameCodesMain>()._timerAssets._active = true;   
        transform.parent.GetComponent<GameCodesMain>().ActivateCacletaNumerator();
    
    }

    public IEnumerator SplitWordSlowly(string input)
    {
        letters.Clear();
     
            _wordText.text = "";

            _missingLetter = Random.Range(0, input.Length); //  FIX: Set before loop

            for (int y = 0; y < input.Length; y++)
            {
                string letter;
                if (y == _missingLetter)
                {
                    letter = "_"; // hidden letter
                }
                else
                {
                    letter = input[y].ToString();
                }

                letters.Add(letter);
                _wordText.text += letter;

                yield return new WaitForSeconds(_speed / _scriptMain._onLevel);
            }

            _correctLetterPos = Random.Range(0, 3);
            char correctLetter = input[_missingLetter]; // the missing letter from the word
            SetupLetterOptions(correctLetter); //  pass the actual letter
     
  

    }



    void SetupLetterOptions(char correctLetter)
    {
        _correctLetterPos = Random.Range(0, 3); // Random position for the correct answer
        _letterParent.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            if (i == _correctLetterPos)
            {
                _letterOptions[i].text = correctLetter.ToString(); // Correct letter
            }
            else
            {
                char randomLetter;
                do
                {
                    randomLetter = (char)('A' + Random.Range(0, 26)); // Random uppercase letter
                }
                while (randomLetter == correctLetter); // Ensure it's not the correct one

                _letterOptions[i].text = randomLetter.ToString();
            }
        }
        //_gameOn = true;
    }

    public void Update()
    {
        if (_Win)
        {
            _girl.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(_girl.GetComponent<RectTransform>().anchoredPosition,
            new Vector2(600, 0), 15 * Time.deltaTime);
        }
        else
        {
            _girl.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(_girl.GetComponent<RectTransform>().anchoredPosition,
             new Vector2(1500, 0), 15 * Time.deltaTime);
        }

        if (transform.parent.GetComponent<GameCodesMain>()._gameStarts)
        {
            if (Input.GetAxisRaw("Horizontal") < 0 && !_moved)
            {

                if (_onLetterOption > 0)
                {
                    _onLetterOption--;
                    _moved = true;

                }
            }

            if (Input.GetAxisRaw("Horizontal") > 0 && !_moved)
            {

                if (_onLetterOption < _letterOptions.Length - 1)
                {
                    _onLetterOption++;
                    _moved = true;
                }

            }

            if (Input.GetAxisRaw("Horizontal") == 0){        
                    _moved = false;
            }

            if (Input.GetButtonDown("Submit") && !_optionChoosed)
            {
                if(_correctLetterPos == _onLetterOption)
                {
          
                    _Win = true;
               
                }
                else
                {
      
                    _Win = false;
          
                }
                transform.parent.GetComponent<GameCodesMain>().ShortenTimer();
                _optionChoosed = true;
                RewriteResult(selectedWord);
                transform.parent.GetComponent<GameCodesMain>()._gameStarts = false;


            }
            transform.parent.GetComponent<GameCodesMain>()._wins = _Win;
        
            _cursor.GetComponent<RectTransform>().anchoredPosition = lettersParent[_onLetterOption].GetComponent<RectTransform>().anchoredPosition;

            _letterParent.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(_letterParent.GetComponent<RectTransform>().anchoredPosition,
                new Vector2(_letterParent.GetComponent<RectTransform>().anchoredPosition.x, 100), 15 * Time.deltaTime);
        }
        else
        {
            _letterParent.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(_letterParent.GetComponent<RectTransform>().anchoredPosition,
    new Vector2(_letterParent.GetComponent<RectTransform>().anchoredPosition.x, -600), 15 * Time.deltaTime);
        }

        _back.transform.localScale = Vector3.Lerp(_back.transform.localScale, _scales[_onScale], 5 * Time.deltaTime);
     

    }

    public void RewriteResult(string input)
    {
        letters.Clear();
        //for(int y = 0; y < 2; y++)
        //{
            _wordText.text = "";

        //_missingLetter = Random.Range(0, input.Length); //  FIX: Set before loop
        _onScale = 0;
        for (int i = 0; i < input.Length; i++)
            {
                string letter;
                if (i == _missingLetter)
                {
                    switch (_Win)
                    {
                        case false:
                            letter = "<color=red>" + _letterOptions[_onLetterOption].text + "</color>";
                            break;
                        case true:
                            letter = "<color=green>" + _letterOptions[_onLetterOption].text + "</color>";
                            break;
                    }
                }
                else
                {
                    letter = input[i].ToString();
                }

                letters.Add(letter);
                _wordText.text += letter;

            }

            _correctLetterPos = Random.Range(0, 3);
            char correctLetter = input[_missingLetter]; // the missing letter from the word
            SetupLetterOptions(correctLetter); //  pass the actual letter

 

    }

    public void RestartVoid()
    {
        transform.parent.GetComponent<GameCodesMain>()._gameStarts = false;
        _optionChoosed = false;
        _letterParent.SetActive(false);  
        _Win = false;

        ActivationWord();

    }


}


