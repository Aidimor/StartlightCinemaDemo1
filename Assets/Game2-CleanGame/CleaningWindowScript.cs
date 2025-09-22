using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class CleaningWindowScript : MonoBehaviour
{
    public MainController _scriptMain;
    public GameObject _mopPlayer;
    [SerializeField] private Animator _mopAnimator;
    public GameObject _parent;
    public float moveSpeed;


    [System.Serializable]
    public class AllManchas
    {
        public Image _manchaImage;
        public Vector2 _manchaPos;        
        public float _distance;
        public bool _cursorOver;
        public bool _activa;
    }
    public AllManchas[] _allManchas;

    public List<int> choosenManchas = new List<int>();
    public int _totalManchas;
    public float _minDistance;
    public float _multiplier;
    public bool _zoom;
    public bool _mopLocked;

    public Image _girl;
    public Vector2[] _girlPos;

    void Start()
    {
 
      

    }

    public void StartGame()
    {
               _scriptMain = GameObject.Find("MainController").gameObject.GetComponent<MainController>();
        StartCoroutine(GameStartsNumerator());
    }

    public IEnumerator GameStartsNumerator()
    {
        PickRandomManchas();
        yield return new WaitForSeconds(1);
        _zoom = true;
        yield return new WaitForSeconds(1);

        transform.parent.GetComponent<GameCodesMain>()._timerAssets._active = true;   
        transform.parent.GetComponent<GameCodesMain>().ActivateCacletaNumerator(); 
    }

    // Rename Shuffle to PickRandomManchas and remove unused parameter
    void PickRandomManchas()
    {
        for (int i = 0; i < _allManchas.Length; i++)
        {
            _allManchas[i]._manchaPos = _allManchas[i]._manchaImage.GetComponent<RectTransform>().anchoredPosition;
        }
        choosenManchas = PickUniqueRandomNumbers(_allManchas.Length, _totalManchas);

        for (int i = 0; i < choosenManchas.Count; i++)
        {
            _allManchas[choosenManchas[i]]._manchaImage.gameObject.SetActive(true);
            //_allManchas[choosenManchas[i]]._manchaImage.gameObject.transform.localScale = new Vector3(1, 1, 1);
            _allManchas[choosenManchas[i]]._activa = true;
        }
    }

    List<int> PickUniqueRandomNumbers(int maxExclusive, int count)
    {
        List<int> numbers = new List<int>();
        for (int i = 0; i < maxExclusive; i++)
        {
            numbers.Add(i);
        }

        for (int i = 0; i < numbers.Count; i++)
        {
            int randomIndex = Random.Range(i, numbers.Count);
            int temp = numbers[i];
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }        
 

        return numbers.GetRange(0, Mathf.Min(count, maxExclusive));

     
    }

    // Update is called once per frame
    void Update()
    {
        if (!transform.parent.gameObject.GetComponent<GameCodesMain>()._wins)
        {
            _girl.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(_girl.GetComponent<RectTransform>().anchoredPosition,
                _girlPos[0], 5 * Time.deltaTime);
        }
        else
        {
            _girl.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(_girl.GetComponent<RectTransform>().anchoredPosition,
        _girlPos[1], 5 * Time.deltaTime);
        }

        if (!_zoom)
        {
            _parent.transform.localScale = Vector2.Lerp(_parent.transform.localScale, new Vector2(1, 1), 5 * Time.deltaTime);
        }
        else
        {
            _parent.transform.localScale = Vector2.Lerp(_parent.transform.localScale, new Vector2(1.5f, 1.5f), 5 * Time.deltaTime);
        }
        if (transform.parent.GetComponent<GameCodesMain>()._gameStarts && !_mopLocked)
            MopController();
    }

    public void MopController()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _mopPlayer.GetComponent<RectTransform>().anchoredPosition += input * moveSpeed * Time.deltaTime;

        if (Input.GetButtonDown("Submit"))
        {
            LocateMob();
        }
    }


    public void LocateMob()
    {
      
        for(int i = 0; i < choosenManchas.Count; i++)
        {
            _allManchas[choosenManchas[i]]._distance = Vector2.Distance(
                _mopPlayer.GetComponent<RectTransform>().anchoredPosition, 
                _allManchas[choosenManchas[i]]._manchaImage.GetComponent<RectTransform>().anchoredPosition);

            if(_allManchas[choosenManchas[i]]._distance < _minDistance && _allManchas[choosenManchas[i]]._manchaImage.gameObject.active)
            {
                _allManchas[choosenManchas[i]]._activa = false;
                _allManchas[choosenManchas[i]]._manchaImage.gameObject.active = false;
                StartCoroutine(CleanNumerator());

            }
        }

        
    }

    public IEnumerator CleanNumerator()
    {
        _mopLocked = true;
        _mopAnimator.SetTrigger("Cleans");
        WinsChecker();
        yield return new WaitForSeconds(0.5f);
        if (!transform.parent.gameObject.GetComponent<GameCodesMain>()._wins)
        {
            _mopLocked = false;
        }
      
      
    }

    public void WinsChecker()
    {
        bool win = true;
        for(int i = 0; i < _allManchas.Length; i++)
        {
            if (_allManchas[i]._activa)
            {
                win = false;
            }            
        }
        if (win)
        {
            transform.parent.gameObject.GetComponent<GameCodesMain>()._wins = win;
            transform.parent.GetComponent<GameCodesMain>().ShortenTimer();
        }
  
    }

    public void ResetVoid()
    {
        transform.parent.GetComponent<GameCodesMain>()._gameStarts = false;
        _mopLocked = false;
        choosenManchas.Clear();
        for(int i = 0; i < _allManchas.Length; i++)
        {
            _allManchas[i]._activa = false;          
        }
        _zoom = false;
        _girl.GetComponent<RectTransform>().anchoredPosition = _girlPos[0];
    }
}
