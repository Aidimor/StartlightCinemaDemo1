using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class RuletaScript : MonoBehaviour
{
    [SerializeField] private MainController _scriptMain;

    public Image _background;
    public GameObject _semiCirclePrefabs;
    public int _totalSpaces;
    public List<GameObject> allSpaces = new List<GameObject>();
    public List<int> colorsChoosed = new List<int>();

    public float _rotationSpeed;
    public bool _choosed;

    public Color[] _allColors;

    public Image _flecha;

    public GameObject OnFragment;

    public List<Vector2> _betweenIntervals = new List<Vector2>();

    public Image[] _winLoseImages;
    public Image[] _buttonImages;
    public Image[] _cameraImages;

    public float _currentAngle;

    void Start()
    {
        _scriptMain = GameObject.Find("MainController").GetComponent<MainController>();
    }

    public void GameStarts()
    {
        _rotationSpeed = Random.Range(200f, 300f);
        SemiCircleSet();
        StartCoroutine(StartNumerator());
    }

    IEnumerator StartNumerator()
    {
        yield return new WaitForSeconds(1);
        transform.parent.GetComponent<GameCodesMain>()._timerAssets._active = true;
        transform.parent.GetComponent<GameCodesMain>().ActivateCacletaNumerator();
    }

    void Update()
    {
        ControllerScript();
    }

    void ControllerScript()
    {
        if (!_choosed)
            _background.transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Submit") && !_choosed && transform.parent.GetComponent<GameCodesMain>()._gameStarts)
            StartCoroutine(ChooseNumerator());
    }

    void SemiCircleSet()
    {
        GenerateRandomOrder();

        float fillAmountPerFragment = 1f / _totalSpaces;
        float anglePerFragment = 360f / _totalSpaces;
        float startAngle = anglePerFragment / 2f;

        // 🔹 Generar intervalos de cada fragmento
        _betweenIntervals.Clear();

        for (int i = 0; i < _totalSpaces; i++)
        {
            GameObject Spaces = Instantiate(_semiCirclePrefabs, _background.transform.localPosition, Quaternion.identity);

            Spaces.name = colorsChoosed[i].ToString();

            allSpaces.Add(Spaces);
            Spaces.transform.SetParent(_background.transform, false);

            Image img = Spaces.GetComponent<Image>();
            img.fillAmount = fillAmountPerFragment;
            img.color = _allColors[colorsChoosed[i]];

            Spaces.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            float angle = startAngle + i * anglePerFragment;
            Spaces.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Guardar intervalo de este fragmento
            float min = i * anglePerFragment;
            float max = (i + 1) * anglePerFragment;
            _betweenIntervals.Add(new Vector2(min, max));
        }

        // Flecha con color del primer fragmento
        _flecha.color = _allColors[colorsChoosed[0]];

        // Resetar rotación de la ruleta
        _background.transform.rotation = Quaternion.identity;
    }

    void GenerateRandomOrder()
    {
        colorsChoosed.Clear();

        for (int i = 0; i < _totalSpaces; i++)
            colorsChoosed.Add(i);

        for (int i = 0; i < colorsChoosed.Count - 1; i++)
        {
            int rand = Random.Range(i, colorsChoosed.Count);
            int temp = colorsChoosed[i];
            colorsChoosed[i] = colorsChoosed[rand];
            colorsChoosed[rand] = temp;
        }
    }

    public IEnumerator ChooseNumerator()
    {
        _choosed = true;
        _buttonImages[0].gameObject.SetActive(false);
        _buttonImages[1].gameObject.SetActive(true);
        _cameraImages[0].gameObject.SetActive(false);
        _cameraImages[1].gameObject.SetActive(true);
        transform.parent.GetComponent<GameCodesMain>().ShortenTimer();
        yield return new WaitForSeconds(0.2f);

        _currentAngle = _background.GetComponent<RectTransform>().eulerAngles.z;

        bool win = false;
        if (_currentAngle >= _betweenIntervals[colorsChoosed[0]].x && _currentAngle <= _betweenIntervals[colorsChoosed[0]].y)
        {
            win = true; // cayó en este sector
            //break;
        }
        //for (int i = 0; i < _betweenIntervals.Count; i++)
        //{
     
        //}

        if (win)
        {
            _winLoseImages[0].gameObject.SetActive(true);
            transform.parent.GetComponent<GameCodesMain>()._wins = true;
        }
        else
        {
            _winLoseImages[1].gameObject.SetActive(true);
            transform.parent.GetComponent<GameCodesMain>()._wins = false;
        }
    }

    public void ResetValues()
    {
        // 🔹 Destruir los fragmentos creados
        for (int i = 0; i < allSpaces.Count; i++)
        {
            Destroy(allSpaces[i]);
        }
        allSpaces.Clear();

        // 🔹 Resetear listas y banderas
        colorsChoosed.Clear();
        _betweenIntervals.Clear();
        _choosed = false;

        // 🔹 Resetear estados visuales
        _winLoseImages[0].gameObject.SetActive(false);
        _winLoseImages[1].gameObject.SetActive(false);
        _buttonImages[0].gameObject.SetActive(true);
        _buttonImages[1].gameObject.SetActive(false);
        _cameraImages[0].gameObject.SetActive(true);
        _cameraImages[1].gameObject.SetActive(false);

        // 🔹 Reiniciar ruleta a rotación inicial
        _background.transform.rotation = Quaternion.identity;

        // 🔹 Reiniciar el estado del juego
        transform.parent.GetComponent<GameCodesMain>()._gameStarts = false;
    }
}
