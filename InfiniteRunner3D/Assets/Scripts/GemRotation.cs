using UnityEngine;

public class GemRotation : MonoBehaviour
{
    public float rotationSpeed = 100f; // Adjust speed as needed

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
