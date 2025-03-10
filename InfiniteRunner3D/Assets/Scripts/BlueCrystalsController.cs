using UnityEngine;

public class BlueCrystalsController : MonoBehaviour
{
    private void Update()
    {
        Vector3 pos = transform.position;
        pos.y = 1f; // Lock Y position to 1
        transform.position = pos;
    }
}
