
using UnityEngine;

public class Indestructible : MonoBehaviour
{
    void Update()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
