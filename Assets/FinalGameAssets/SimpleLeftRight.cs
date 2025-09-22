using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLeftRight : MonoBehaviour
{
    [Header("Objetos de juego")]
    public Transform _objectParent;
    public GameObject _player;
    public GameObject _chef;
    public Animator _playerAnimator;
    public GameObject _bullet;
    public GameObject _enemyObject;

    [Header("Movimiento y ataque")]
    public float moveSpeed = 500f;
    public float _timer;
    public bool _readyToAttack;
    public bool _canServe;
    public bool _movementLocked;

    [Header("Enemigos")]
    public float _enemyTimer;
    public int _onEnemy;
    public int _maxEnemies = 10;
    public int _enemiesFail;
    public int _maxEnemiesFail = 3;
    public List<GameObject> _allEnemies = new List<GameObject>();

    [Header("Balas activas")]
    public List<GameObject> _activeBullets = new List<GameObject>();

    [Header("Estado del juego")]
    public bool _finished;
    public bool _win;

    [Header("Distancia de colisión")]
    public float destroyDistance = 50f; // Ajusta según escala de tu UI/mundo


    public void StartGame()
    {
        StartCoroutine(StartNumerator());
    }

    public IEnumerator StartNumerator()
    {


        yield return new WaitForSeconds(1);


        transform.parent.GetComponent<GameCodesMain>()._timerAssets._active = true;
        transform.parent.GetComponent<GameCodesMain>().ActivateCacletaNumerator();
        InstantiateEnemies();

    }


    void Update()
    {
        if (!_movementLocked)
        {
            HandlePlayerMovement();
            HandleAttack();
            DetectBulletHits();
        }

        HandleEnemySpawning();
    }

    #region Movimiento y ataque
    void HandlePlayerMovement()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        Vector3 move = new Vector3(moveInput * moveSpeed * Time.deltaTime, 0f, 0f);
        _player.transform.position += move;

        // Limitar movimiento del jugador
        Vector3 clampedPosition = _player.transform.localPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -400f, 400f);
        _player.transform.localPosition = clampedPosition;

        // Interacción con el chef
        if (_player.transform.localPosition.x > _chef.transform.localPosition.x &&
            _player.transform.localPosition.x < _chef.transform.localPosition.x + 2f &&
            !_readyToAttack && _canServe && moveInput != 0)
        {
            _playerAnimator.SetTrigger("Receive");
            _readyToAttack = true;
            _movementLocked = true;
            StartCoroutine(CatchNumerator());
        }

        _playerAnimator.SetBool("Ready", _readyToAttack);
        ChefController();
    }

    void HandleAttack()
    {
        _timer -= Time.deltaTime;

        if (Input.GetButtonDown("Submit") && _timer <= 0 && _readyToAttack)
        {
            _timer = 0.5f;
            _playerAnimator.SetTrigger("Attack");
            _canServe = false;
            StartCoroutine(AttackNumerator());
        }
    }

    public IEnumerator CatchNumerator()
    {
        yield return new WaitForSeconds(0.1f);
        _movementLocked = false;
    }

    public IEnumerator AttackNumerator()
    {
        yield return new WaitForSeconds(0.2f);
        Debug.Log("la caga");
        GameObject bullet = Instantiate(_bullet, _player.transform.position, _player.transform.rotation);
        bullet.transform.parent = _objectParent.transform;
        bullet.transform.localScale = Vector2.one;

        // Agregar la bala a la lista de balas activas
        _activeBullets.Add(bullet);

        yield return new WaitForSeconds(0.1f);
        _chef.transform.localPosition = new Vector2(Random.Range(-3f, 3f), _chef.transform.localPosition.y);
        _readyToAttack = false;

        yield return new WaitForSeconds(0.5f);
        _canServe = true;
    }

    void ChefController()
    {
        if (!_readyToAttack && _canServe)
            _chef.transform.localPosition = Vector2.Lerp(_chef.transform.localPosition, new Vector2(_chef.transform.localPosition.x, 400), 15 * Time.deltaTime);
        else
            _chef.transform.localPosition = Vector2.Lerp(_chef.transform.localPosition, new Vector2(_chef.transform.localPosition.x, 600), 15 * Time.deltaTime);
    }
    #endregion

    #region Enemigos
    public void InstantiateEnemies()
    {
        for (int i = 0; i < _maxEnemies; i++)
        {
            GameObject enemy = Instantiate(_enemyObject, transform.position, transform.rotation);
            enemy.GetComponent<EnemyFinalStage>()._scriptParent = this;
            enemy.transform.parent = _objectParent;
            enemy.transform.localScale = Vector2.one;
            enemy.transform.localPosition = new Vector2(Random.Range(-400f, 400f), -1000f);
            _allEnemies.Add(enemy);
            enemy.GetComponent<EnemyFinalStage>()._speed = Random.Range(3f, 4f);
            enemy.SetActive(false);
        }
        _movementLocked = false;
        _canServe = true;
        _playerAnimator.Play("IdleNotReady");
    }

    void HandleEnemySpawning()
    {
        if (_maxEnemies != _onEnemy)
            _enemyTimer -= Time.deltaTime;

        if (_enemyTimer <= 0 && !_finished && !_movementLocked)
        {
            EnemyAppears();
            _enemyTimer = Random.Range(1f, 2f);
        }
    }

    public void EnemyAppears()
    {
        if (_onEnemy < _allEnemies.Count)
        {
            _allEnemies[_onEnemy].SetActive(true);
            _onEnemy++;
        }
    }
    #endregion

    #region Detección de balas
    void DetectBulletHits()
    {
        // Recorremos la lista de balas activas
        for (int i = _activeBullets.Count - 1; i >= 0; i--)
        {
            GameObject bullet = _activeBullets[i];
            if (bullet == null)
            {
                _activeBullets.RemoveAt(i);
                continue;
            }

            foreach (GameObject enemy in _allEnemies)
            {
                if (enemy.activeInHierarchy)
                {
                    float distance = Vector2.Distance(bullet.transform.position, enemy.transform.position);
                    if (distance <= destroyDistance)
                    {
                        Debug.Log("Bala golpea enemigo!");

                        Destroy(bullet);
                        _activeBullets.RemoveAt(i);

                        EnemyFinalStage enemyScript = enemy.GetComponent<EnemyFinalStage>();
                        if (enemyScript != null && !enemyScript._destroyed)
                        {
                            enemyScript._destroyed = true;
                            enemyScript.GetComponent<Animator>().SetTrigger("Leaves");
                        }
                        break; // La bala solo golpea un enemigo
                    }
                }
            }
        }
    }
    #endregion

    public void LosesGame()
    {
        _movementLocked = true;
        ResetValues();
        StartCoroutine(LosesGameNumerator());
    }

    public IEnumerator LosesGameNumerator()
    {
        yield return new WaitForSeconds(0.5f);
        _playerAnimator.Play("LosesGame");
        transform.parent.GetComponent<GameCodesMain>().ShortenTimer();
        //transform.parent.GetComponent<GameCodesMain>().NextGameVoid();   
    }

    public IEnumerator WinGameNumerator()
    {
        yield return new WaitForSeconds(0.5f);
        _playerAnimator.Play("WinsGame");
        yield return new WaitForSeconds(2);
        Application.Quit();
    }

    public void ResetValues()
    {
        for (int i = 0; i < _allEnemies.Count; i++)
        {
            Destroy(_allEnemies[i]);
            _allEnemies.Clear();
        }
    }

}
