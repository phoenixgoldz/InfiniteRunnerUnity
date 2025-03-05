using UnityEngine;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
    [Header("Path Settings")]
    public GameObject[] pathPrefabs;
    public int initialSegments = 5;
    public float pathLength = 300f;
    public float pathYPosition = -3.2f;

    [Header("Side Decorations")]
    public GameObject[] leftSidePrefabs;
    public GameObject[] rightSidePrefabs;
    public float sideOffset = 150f;

    private Transform player;
    private List<GameObject> activePaths = new List<GameObject>();
    private float spawnZ = 0f;
    private float safeZone = 500f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("❌ PathManager: Player not found! Make sure the Player GameObject is tagged as 'Player'.");
            return;
        }

        // Spawn initial floor segment (permanent)
        SpawnInitialFloor();

        // Spawn other initial paths
        for (int i = 0; i < initialSegments; i++)
        {
            SpawnPath(i == 0 ? 0 : Random.Range(0, pathPrefabs.Length));
        }
    }

    void Update()
    {
        if (player == null) return;

        // Continuously spawn paths as the player moves forward
        if (player.position.z - safeZone > (spawnZ - (initialSegments * pathLength)))
        {
            SpawnPath(Random.Range(0, pathPrefabs.Length));
            DeleteOldPath();
        }
    }

    void SpawnInitialFloor()
    {
        GameObject initialFloor = Instantiate(pathPrefabs[0], new Vector3(0, pathYPosition, spawnZ), Quaternion.identity);
        activePaths.Add(initialFloor);
    }

    public void SpawnPath(int prefabIndex)
    {
        GameObject newPath = Instantiate(pathPrefabs[prefabIndex], new Vector3(0, pathYPosition, spawnZ), Quaternion.identity);
        activePaths.Add(newPath);

        if (leftSidePrefabs.Length > 0)
        {
            GameObject leftDeco = Instantiate(leftSidePrefabs[Random.Range(0, leftSidePrefabs.Length)],
                new Vector3(-sideOffset, pathYPosition, spawnZ), Quaternion.identity);
            activePaths.Add(leftDeco);
        }

        if (rightSidePrefabs.Length > 0)
        {
            GameObject rightDeco = Instantiate(rightSidePrefabs[Random.Range(0, rightSidePrefabs.Length)],
                new Vector3(sideOffset, pathYPosition, spawnZ), Quaternion.identity);
            activePaths.Add(rightDeco);
        }

        spawnZ += pathLength;
    }

    void DeleteOldPath()
    {
        // Prevent removing the first path segment
        if (activePaths.Count > initialSegments * 3 + 1) // +1 to exclude the initial floor
        {
            Destroy(activePaths[1]); // Remove the oldest segment (but not the first)
            activePaths.RemoveAt(1);
        }
    }
}
