using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SalaCinemaController : MonoBehaviour
{
    [SerializeField] private MainController _scriptMain;
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
    public bool _controllerOn;


    public bool _gameStarts;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_scriptMain._onStation == 1)        
            CandyShopVoid();
        
    }

    public IEnumerator StartGameNumerator()
    {

        for (int i = 0; i < candyShopAssets._candyShopPositions[candyShopAssets._onPos]._chars.Length; i++)
        {
            candyShopAssets._candyShopPositions[candyShopAssets._onPos]._chars[i].SetBool("Enter", true);
            yield return new WaitForSeconds(0.25f);
        }

        for (int i = 0; i < candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons.Length; i++)
        {
            candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons[i].GetComponent<Animator>().enabled = true;

        }

        for (int i = 0; i < candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons.Length; i++)
        {
            candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons[i].GetComponent<Animator>().SetBool("Active", true);
            yield return new WaitForSeconds(0.1f);

        }
      
   
        _scriptMain._onStation = 1;
        _controllerOn = true;
    }


    public void CandyShopVoid()
    {
        candyShopAssets._parent.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(candyShopAssets._parent.GetComponent<RectTransform>().anchoredPosition,
            candyShopAssets._candyShopPositions[candyShopAssets._onPos]._position, 5 * Time.deltaTime);

        candyShopAssets._parent.transform.localScale = Vector3.Lerp(candyShopAssets._parent.transform.localScale,
            new Vector3(candyShopAssets._candyShopPositions[candyShopAssets._onPos]._scale, candyShopAssets._candyShopPositions[candyShopAssets._onPos]._scale, 1)
     , 5 * Time.deltaTime);

        if (_controllerOn)
        {
            if (Input.GetAxisRaw("Horizontal") > 0 && !candyShopAssets._changing)
            {
        
                if (candyShopAssets._onPos < 2)
                {
                    for (int i = 0; i < candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons.Length; i++)
                    {
                        candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons[i].GetComponent<Animator>().SetBool("Active", false);
                    }

                    for (int i = 0; i < candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons.Length; i++)
                    {
                        candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons[i].GetComponent<Animator>().enabled = true;

                    }

                    candyShopAssets._onPos++;
                    candyShopAssets._changing = true;
                    StartCoroutine(CandyShopChangeNumerator());
                }

            
            }

            if (Input.GetAxisRaw("Horizontal") < 0 && !candyShopAssets._changing)
            {
             
                if (candyShopAssets._onPos > 0)
                {
                    for (int i = 0; i < candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons.Length; i++)
                    {
                        candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons[i].GetComponent<Animator>().SetBool("Active", false);
                    }

                    for (int i = 0; i < candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons.Length; i++)
                    {
                        candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons[i].GetComponent<Animator>().enabled = true;

                    }

                    candyShopAssets._onPos--;
                    candyShopAssets._changing = true;
                    StartCoroutine(CandyShopChangeNumerator());
                }
     


            }

            if (Input.GetButtonDown("Submit") && !_gameStarts)
            {
                switch (candyShopAssets._onPos)
                {
                    case 1:
                        StartCoroutine(EntradaSalaCine());
                        candyShopAssets._parent.SetActive(false);
                        candyShopAssets._CandyShopAvailable = false;
                        _gameStarts = true;
                        break;
                }
            }
        }
  

        if (candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons.Length > 1)
        {
            if (Input.GetAxisRaw("Vertical") < 0 && !candyShopAssets._buttonChange && candyShopAssets._candyShopPositions[candyShopAssets._onPos]._onButton <
               candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons.Length - 1 && !candyShopAssets._changing)
            {
                candyShopAssets._candyShopPositions[candyShopAssets._onPos]._onButton++;
                ChangeButton();
                candyShopAssets._buttonChange = true;
            }

            if (Input.GetAxisRaw("Vertical") > 0 && !candyShopAssets._buttonChange && candyShopAssets._candyShopPositions[candyShopAssets._onPos]._onButton > 0 && !candyShopAssets._changing)
            {
                candyShopAssets._candyShopPositions[candyShopAssets._onPos]._onButton--;
                ChangeButton();
                candyShopAssets._buttonChange = true;
            }

            if (Input.GetAxisRaw("Vertical") == 0)
            {
                candyShopAssets._buttonChange = false;
            }
        }



  
    }




    public void ChangeButton()
    {
        if (candyShopAssets._onPos == 2)
        {
            int _onRealButton = candyShopAssets._candyShopPositions[candyShopAssets._onPos]._onButton;

            for (int i = 0; i < candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons.Length; i++)
            {
                candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons[i].transform.localScale = new Vector3(1, 1, 1);
            }
          //  candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons[_onRealButton].transform.localScale = new Vector3(1.25f, 1.25f, 1);
        }
    }


public IEnumerator CandyShopChangeNumerator()
{
    candyShopAssets._candyShopPositions[0]._onButton = 0;
    candyShopAssets._candyShopPositions[1]._onButton = 0;
    candyShopAssets._candyShopPositions[2]._onButton = 0;

    for (int y = 0; y < candyShopAssets._candyShopPositions.Length; y++)
    {
        for (int i = 0; i < candyShopAssets._candyShopPositions[y]._chars.Length; i++)
        {
            candyShopAssets._candyShopPositions[y]._chars[i].SetBool("Enter", false);
        }
    }


    //yield return new WaitForSeconds(0.25f);

    for (int i = 0; i < candyShopAssets._candyShopPositions[candyShopAssets._onPos]._chars.Length; i++)
    {
        candyShopAssets._candyShopPositions[candyShopAssets._onPos]._chars[i].SetBool("Enter", true);
        yield return new WaitForSeconds(0.25f);
    }

    for (int i = 0; i < candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons.Length; i++)
    {
        candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons[i].GetComponent<Animator>().SetBool("Active", true);
        yield return new WaitForSeconds(0.05f);
    }
    yield return new WaitForSeconds(0.5f);


    for (int i = 0; i < candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons.Length; i++)
    {
        candyShopAssets._candyShopPositions[candyShopAssets._onPos]._buttons[i].GetComponent<Animator>().enabled = false;

    }
    ChangeButton();
    candyShopAssets._changing = false;
}

    public IEnumerator EntradaSalaCine(){
        NextGameListVoid();
        _scriptMain._loadingAnimator.SetBool("Loading", true);
        yield return new WaitForSeconds(4);       
        Application.LoadLevel(1);   
    }

    public void NextGameListVoid()
    {
        for(int i = 0; i < _scriptMain._totalGames; i++)
        {
            _scriptMain._gamesToPLay.Add(Random.Range(0, 7));
        }
    }
}
