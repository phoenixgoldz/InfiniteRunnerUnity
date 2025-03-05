using UnityEngine;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
    [Header("Path Settings")]
    public GameObject[] pathPrefabs; // Paths (FlatIcePath, etc.)
    public int initialSegments = 5;  // How many segments spawn at start
    public float pathLength = 300f;  // Fixed distance between paths
    public float pathYPosition = -3.2f; // Ensuring paths spawn at this height

    [Header("Side Decorations")]
    public GameObject[] leftSidePrefabs;  // Assign Crystal Wall prefabs here
    public GameObject[] rightSidePrefabs; // Assign BlueCrystals prefabs here
    public float sideOffset = 150f; // Distance from center to the side

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

        // Spawn initial paths
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

    public void SpawnPath(int prefabIndex)
    {
        // Spawn main path segment at fixed y-position
        GameObject newPath = Instantiate(pathPrefabs[prefabIndex], 
            new Vector3(0, pathYPosition, spawnZ), Quaternion.identity);
        activePaths.Add(newPath);

        // Spawn left decoration
        if (leftSidePrefabs.Length > 0)
        {
            GameObject leftDeco = Instantiate(leftSidePrefabs[Random.Range(0, leftSidePrefabs.Length)], 
                new Vector3(-sideOffset, pathYPosition, spawnZ), Quaternion.identity);
            activePaths.Add(leftDeco);
        }

        // Spawn right decoration
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
        if (activePaths.Count > initialSegments * 3) // *3 since each path has 2 decorations
        {
            Destroy(activePaths[0]);
            activePaths.RemoveAt(0);
        }
    }
}
