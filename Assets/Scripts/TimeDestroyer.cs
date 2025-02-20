using UnityEngine;
using System.Collections;

public class TimeDestroyer : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Invoke("DestroyObject", LifeTime);
    }


    void DestroyObject()
    {
        if (GameManager.Instance.GameState != GameState.Dead)
            Destroy(gameObject);
    }


    public float LifeTime = 10f;
}
