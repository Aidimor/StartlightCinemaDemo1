using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CleanGameplay : MonoBehaviour
{
    public int _totalEnemies;
    public float moveSpeed;
    public float smoothTime = 0.1f;
    public GameObject _player;
    public Animator _playerAnimator;
    public GameObject _enemy;
    public GameObject _map;
    public List<GameObject> _allEnemies = new List<GameObject>();
    public Vector3 currentDirection;
    
    public List<Transform> _allExits = new List<Transform>();

    public float maxSpeed;
    public float minSpeed;
    public bool winCheked;
    public float _cleanDiameter;
    public float _reachDiameter;
    public bool _swiping;

    public float proximityDistance; // Change as needed
    public Animator _winCanvasAnimator;
    public GameObject _trashParent;
  

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void StartVoid()
    {
        StartCoroutine(StartNumerator());
    }

    public IEnumerator StartNumerator()
    {
 
     
        yield return new WaitForSeconds(1);


        transform.parent.GetComponent<GameCodesMain>()._timerAssets._active = true;
        transform.parent.GetComponent<GameCodesMain>().ActivateCacletaNumerator();
        GameStart();

    }

    public void GameStart()
    {
        for (int i = 0; i < _totalEnemies; i++)
        {
            // Instantiate enemy at map's position and rotation
            GameObject Enemy = Instantiate(_enemy, _map.transform.position, _map.transform.rotation);
            Enemy.GetComponent<EnemyScript>()._scriptClean = GetComponent<CleanGameplay>();
            Enemy.GetComponent<EnemyScript>()._garbageImages[Random.Range(0, 4)].SetActive(true);


            // Add to enemy list
            _allEnemies.Add(Enemy);

            // Set parent in hierarchy
            Enemy.transform.parent = _trashParent.transform;
            Enemy.transform.localScale = new Vector3(1, 1, 1);
            // -------- Generate position between -3 and 3 in X and Y --------
            Vector2 randomPoint;
            do
            {
                randomPoint = new Vector2(Random.Range(-80f, 80f), Random.Range(-40f, 40f));
            } while (randomPoint.magnitude < 0.1f); // Optional: Avoid center

            // Set enemy local position relative to parent
            Enemy.transform.GetComponent<RectTransform>().anchoredPosition = randomPoint;

            // -------- Find closest target from the list --------
            Transform closestTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (Transform target in _allExits)
            {
                float dist = Vector2.Distance(Enemy.transform.position, target.position); // world positions
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestTarget = target;
                }
            }

            // Assign closest target to enemy
            EnemyScript enemyAI = Enemy.GetComponent<EnemyScript>();
            if (enemyAI != null && closestTarget != null)
            {
                enemyAI._exitTarget = closestTarget;
            }
            Enemy.transform.localPosition = new Vector3(Enemy.transform.localPosition.x, Enemy.transform.localPosition.y, -0.3f);
        }

        winCheked = false;
        _player.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -150f);
    }



    // Update is called once per frame
    void Update()
    {
        if (!winCheked && transform.parent.GetComponent<GameCodesMain>()._gameStarts)
        {
            PlayerController();
        }

        CheckPlayerEnemyDistance();
    }

    void PlayerController()
    {
        // Raw input (snappy)
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector3 targetDirection = new Vector3(inputX, inputY, 0f).normalized;

        // Smooth interpolation factor
        float lerpFactor = smoothTime * Time.deltaTime;

        // Smoothly transition to target direction
        currentDirection = Vector3.Lerp(currentDirection, targetDirection, lerpFactor);

        // Move the player
        _player.transform.Translate(currentDirection * moveSpeed * Time.deltaTime);

        // --- Animation: Check if player is moving ---
        if (currentDirection.magnitude > 0.01f)
        {
  

            // --- Rotation ---
            float angle = Mathf.Atan2(-currentDirection.x, currentDirection.y) * Mathf.Rad2Deg;
            _playerAnimator.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
   

        if(inputX == 0 && inputY == 0)
        {
            _playerAnimator.SetBool("Moving", false);
        }
        else
        {
            _playerAnimator.SetBool("Moving", true);
        }
    }



    public void WinChecker()
    {
        transform.parent.GetComponent<GameCodesMain>()._wins = true;
        for (int i = 0; i < _allEnemies.Count; i++)
        {
            if (!_allEnemies[i].GetComponent<EnemyScript>()._outOfMap)
            {
                transform.parent.GetComponent<GameCodesMain>()._wins = false;
            }      
        }

        if (transform.parent.GetComponent<GameCodesMain>()._wins)
        {
            transform.parent.GetComponent<GameCodesMain>().ShortenTimer();
            _winCanvasAnimator.Play("WinAnimatorPanelAnim");
            //transform.parent.GetComponent<MainGameplayController>()._scriptTimer._timer = 1.5f;
     
            winCheked = true;
        }
       
    }
    void CheckPlayerEnemyDistance()
    {
        bool shouldSwipe = false;

        foreach (GameObject enemy in _allEnemies)
        {
            if (enemy == null || enemy.GetComponent<EnemyScript>()._outOfMap)
                continue;

            float distance = Vector3.Distance(_player.transform.position, enemy.transform.position);
            if (distance <= proximityDistance)
            {
                shouldSwipe = true;
                break; // No need to check further, one valid enemy is close enough
            }
        }

        _playerAnimator.SetBool("Sweep", shouldSwipe);
    }

    public void ResetValue(){
        for (int i = 0; i < _allEnemies.Count; i++)
        {
            Destroy(_allEnemies[i].gameObject);
        }
       _allEnemies.Clear();
       currentDirection = Vector2.zero;
        transform.parent.GetComponent<GameCodesMain>()._gameStarts = false;
    }


}
