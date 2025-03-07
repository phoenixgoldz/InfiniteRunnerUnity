using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private PathManager pathManager;

    void Start()
    {
        pathManager = FindFirstObjectByType<PathManager>(); 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PathTrigger"))
        {
            pathManager.SpawnPath(); 
        }
    }
}
