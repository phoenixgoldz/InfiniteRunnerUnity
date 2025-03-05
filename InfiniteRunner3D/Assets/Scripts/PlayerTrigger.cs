using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private PathManager pathManager;

    void Start()
    {
        pathManager = FindObjectOfType<PathManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PathTrigger"))
        {
            pathManager.SpawnPath(Random.Range(0, pathManager.pathPrefabs.Length));
        }
    }
}
