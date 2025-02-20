using UnityEngine;
using System.Collections;

public class RedBorder : MonoBehaviour
{

    // Use this for initialization
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == Constants.PlayerTag)
            GameManager.Instance.Die();
    }
}
