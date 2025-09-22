using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.PostProcessing;


public class GameCodesMain : MonoBehaviour
{
    public Canvas _mainCanvas;
    public Camera _mainCamera;
    public MainController _scriptMain;

    [System.Serializable]
    public class GamesAssets
    {
        public GameObject _gamePanel;
        public GameObject _gameCode;
        public string _description;
    }
    public GamesAssets[] _gameAssets;
    public int _onGame;


    [System.Serializable]
    public class FinalGamesAssets
    {
        public GameObject _gamePanel;
        public GameObject _gameCode;
    }
    public FinalGamesAssets[] _finalGameAssets;

    public bool _finalGame;

    [System.Serializable]
    public class TicketLives
    {
        public int _currentHP;
        public Image[] _allTickets;
        public Color[] _ticketsColors;
    }
    public TicketLives _ticketLives;

    [System.Serializable]
    public class TimerAssets
    {
        public bool _active;
        public GameObject _parent;
        public Animator _carreteAnimator;
        public Vector2[] _carretePos;
    }
    public TimerAssets _timerAssets;

    public bool _wins;

    public bool _cintaOn;
    public RawImage _cintaRawImage;
    // === Cinta (UI Tiled Animation) ===
    public GameObject[] _cinta;          //  Changed from Image → RawImage
    public float scrollSpeed = 0.5f; // Speed of scrolling
    private Vector2 offset = Vector2.zero;

    public bool _cacletaOn;
    public Animator _cacletaAnimator;
    public Vector2[] _cacletaPoses;
    public TextMeshProUGUI _turnText;
    public bool _gameStarts;

    public TextMeshProUGUI _descriptionText;
    public AudioSource _mainTheme;



    public PostProcessVolume postProcessVolume;
    Vignette vignette;
    AutoExposure autoExp;
    DepthOfField depthField;
    public float _depthQuantity;
    Grain grain;
    ChromaticAberration chromatic;

    public bool _gameFinished;

    [System.Serializable]
    public class EndPanelAssets
    {
        public GameObject _parent;
        public GameObject[] _Options;
        public bool _canMove;
        public GameObject _selector;
        public int _pos;
    }
    public EndPanelAssets _endPanelAssets;

    void Start()
    {
   
        _scriptMain = GameObject.Find("MainController").gameObject.GetComponent<MainController>();
        _scriptMain._intermissionCurrentAnimator = GameObject.Find("Canvas/IntermisssionAnimation").GetComponent<Animator>();
        _scriptMain._intermissionCurrentAnimator.Play("NormalIntermission");
        _scriptMain._loadingAnimator.SetBool("Loading", false);   
        _cintaOn = true;

       

        postProcessVolume.profile.TryGetSettings(out vignette);

        if (vignette != null)
        {

            vignette.intensity.value = 0;   // Set the focal length in mm
        }
    }

    void Update()
    {
        if (!_gameFinished)
        {
            CarreteVoid();
            CintaVoid(); // now runs every frame
            CacletaPosVoid();
        }
        else
        {
            EndPanelVoid();
        }
      
       
    
    }

    public void CacletaPosVoid()
    {
        _turnText.text = (_onGame + 1).ToString("F0");
        if (_cacletaOn)
        {
            _cacletaAnimator.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(_cacletaAnimator.GetComponent<RectTransform>().anchoredPosition,
              _cacletaPoses[0], 10 * Time.deltaTime);
        }
        else
        {
            _cacletaAnimator.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(_cacletaAnimator.GetComponent<RectTransform>().anchoredPosition,
              _cacletaPoses[1], 10 * Time.deltaTime);
        }
    }

    public void ActivateCacletaNumerator()
    {
        StartCoroutine(CacletaNumerator());
    }
    public IEnumerator CacletaNumerator()
    {
        _descriptionText.text = _gameAssets[_scriptMain._gamesToPLay[_onGame]]._description;
        _cacletaOn = true;
        _cacletaAnimator.Play("CacletaHitAnim");
        yield return new WaitForSeconds(1);
        _cacletaOn = false;
        _timerAssets._carreteAnimator.Play("CarreteAnimation");
                yield return new WaitForSeconds(1);
        _gameStarts = true;
    }

    public void InstantiateGameAssets()
    {
        switch (_finalGame)
        {
            case false:
                _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gamePanel.SetActive(true);
                _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.SetActive(true);
                _cintaOn = false;
                switch (_scriptMain._gamesToPLay[_onGame])
                {
                    case 0:
                        _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.GetComponent<MissingLetterScript>().ActivationWord();
                        break;
                    case 1:
                        _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.GetComponent<CleaningWindowScript>().StartGame();
                        break;
                    case 2:
                        _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.GetComponent<RuletaScript>().GameStarts();
                        break;
                    case 3:
                        _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.GetComponent<ChooseSecuenceScript>().GameStarts();
                        break;
                    case 4:
                        _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.GetComponent<CatchBallScript>().StartGameVoid();
                        break;
                    case 5:
                        _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.GetComponent<CleanGameplay>().StartVoid();
                        break;
                    case 6:
                        _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.GetComponent<SimpleLeftRight>().StartGame();
                        break;

                }
                break;
            case true:
                //_finalGameAssets[_scriptMain._finalGameID]._gamePanel.SetActive(true);
                //_finalGameAssets[_scriptMain._finalGameID]._gameCode.SetActive(true);       
                //_cintaOn = false;
                //switch (_scriptMain._finalGameID)
                //{
                //    case 0:
                //        _finalGameAssets[_scriptMain._finalGameID]._gameCode.GetComponent<SimpleLeftRight>().StartGame();
                //        break;
                //}
   
                break;
        }
 
    }

    public void CarreteVoid()
    {
        if (_timerAssets._active)
        {
            _timerAssets._parent.GetComponent<RectTransform>().anchoredPosition =
                Vector2.Lerp(
                    _timerAssets._parent.GetComponent<RectTransform>().anchoredPosition,
                    _timerAssets._carretePos[1],
                    15 * Time.deltaTime
                );
        }
        else
        {
            _timerAssets._parent.GetComponent<RectTransform>().anchoredPosition =
                Vector2.Lerp(
                    _timerAssets._parent.GetComponent<RectTransform>().anchoredPosition,
                    _timerAssets._carretePos[0],
                    5 * Time.deltaTime
                );
        }
    }

    public void NextGameVoid()
    {
        StartCoroutine(NextGameNumerator());


    }

    public IEnumerator NextGameNumerator()
    {
        bool Change = false;

            for(int i = 0; i < _scriptMain._changeAt.Length; i++){
            if (_scriptMain._changeAt[i] == _onGame)
            {
                Change = true;
            }
        }

        if (Change)
        {
            Debug.Log("Cambia");
            if (vignette != null)
            {

                vignette.intensity.value += 0.2f;   // Set the focal length in mm
            }
            _mainTheme.pitch += 0.05f;
        }

        if (_onGame >= _scriptMain._gamesToPLay.Count - 1 || _ticketLives._currentHP == 0)
        {
            StartCoroutine(EndPanelEnterNumerator());
        }

        _gameStarts = false;
        _timerAssets._active = false;
        yield return new WaitForSeconds(1);
        switch (_finalGame)
        {
            case false:
                switch (_scriptMain._gamesToPLay[_onGame])
                {
                    case 0:
                        _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.GetComponent<MissingLetterScript>().RestartVoid();
                        break;
                    case 1:
                        _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.GetComponent<CleaningWindowScript>().ResetVoid();
                        break;
                    case 2:
                        _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.GetComponent<RuletaScript>().ResetValues();
                        break;
                    case 3:
                        _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.GetComponent<ChooseSecuenceScript>().ResetValues();
                        break;
                    case 4:
                        _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.GetComponent<CatchBallScript>().ResetValues();
                        break;
                    case 5:
                        _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.GetComponent<CleanGameplay>().ResetValue();
                        break;
                    case 6:
                        _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.GetComponent<SimpleLeftRight>().ResetValues();
                        break;
                }
                _cintaOn = true;
                _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gamePanel.SetActive(false);
                _gameAssets[_scriptMain._gamesToPLay[_onGame]]._gameCode.SetActive(false);
                _onGame++;
                //if (_onGame >= _scriptMain._gamesToPLay.Count - 1)
                //{
                //    _finalGame = true;
                //}
                switch (_wins)
                {
                    case true:
                        _scriptMain._intermissionCurrentAnimator.Play("WinAnimIntermission");
                        break;
                    case false:
                        LoseLife();
                        _scriptMain._intermissionCurrentAnimator.Play("LoseAnimIntermission");
                        break;
                }
                _wins = false;
                break;
            case true:
                switch (_wins)
                {
                    case true:
                        _scriptMain._intermissionCurrentAnimator.Play("WinAnimIntermission");
                        break;
                    case false:
                        LoseLife();
                        _scriptMain._intermissionCurrentAnimator.Play("LoseAnimIntermission");
                        break;
                }
     
                break;
        }


    }

    public void ShortenTimer(){
        var anim = _timerAssets._carreteAnimator;
        // Jump directly to second 4 of a 5s clip
        anim.Play("CarreteAnimation", 0, 0.8f);
        anim.Update(0f); // forces immediate update to show frame
    }

 

    // === Fixed CintaVoid ===
    public void CintaVoid()
    {
        // Move horizontally
        offset.x += scrollSpeed * Time.deltaTime;
        _cinta[0].GetComponent<MeshRenderer>().material.mainTextureOffset = offset;
        _cinta[1].GetComponent<MeshRenderer>().material.mainTextureOffset = offset;
        switch (_cintaOn)
        {
            case true:
                _cintaRawImage.transform.localScale = Vector2.Lerp(_cintaRawImage.transform.localScale,
                    new Vector2(1f, 1f), 5 * Time.deltaTime);
                scrollSpeed = Mathf.Lerp(scrollSpeed, 5, 5 * Time.deltaTime);
                _cinta[1].SetActive(true);
                break;
            case false:
                _cintaRawImage.transform.localScale = Vector2.Lerp(_cintaRawImage.transform.localScale,
          new Vector2(1f, 1.3f), 3 * Time.deltaTime);
                scrollSpeed = Mathf.Lerp(scrollSpeed, 0, 5 * Time.deltaTime);
                _cinta[1].SetActive(false);
                break;
        }
    }

    public void LoseLife()
    {
        _ticketLives._currentHP--;
        for(int i = 0; i < _ticketLives._allTickets.Length; i++)
        {
            _ticketLives._allTickets[i].color = _ticketLives._ticketsColors[0];
        }
        for (int i = 0; i < _ticketLives._currentHP; i++)
        {
            _ticketLives._allTickets[i].color = _ticketLives._ticketsColors[1];
        }

        if(_ticketLives._currentHP <= 0)
        {
            Application.Quit();
        }
    }

    public IEnumerator EndPanelEnterNumerator()
    {
        _gameFinished = true;
        _endPanelAssets._parent.SetActive(true);
        yield return new WaitForSeconds(1);
        _endPanelAssets._canMove = true;
    }
    public void EndPanelVoid()
    {
        _endPanelAssets._selector.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(_endPanelAssets._selector.GetComponent<RectTransform>().anchoredPosition,
           _endPanelAssets._Options[_endPanelAssets._pos].GetComponent<RectTransform>().anchoredPosition, 5 * Time.deltaTime);

        if(_endPanelAssets._canMove)
        if(Input.GetAxisRaw("Vertical") > 0){
            _endPanelAssets._pos = 0;
        }

        if (Input.GetAxisRaw("Vertical") < 0)
        {
            _endPanelAssets._pos = 1;
        }

        if (Input.GetButton("Submit"))
        {
            switch (_endPanelAssets._pos)
            {
                case 0:
                    Application.LoadLevel(1);
                    break;
                case 1:
                    Application.LoadLevel(0);
                    break;
            }
        }
    }

}
