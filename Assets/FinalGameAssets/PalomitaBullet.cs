using UnityEngine;

public class PalomitaBullet : MonoBehaviour
{
    public float _speed;
    public float _timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.down * _speed * Time.deltaTime);
        _timer -= Time.deltaTime;
        if(_timer <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
