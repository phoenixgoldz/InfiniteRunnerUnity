using UnityEngine;

public class GemCollect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // If player touches gem
        {
            PlayerUIManager.Instance.UpdateScore(5); //  Add 5 points per gem
            Destroy(gameObject); //  Remove gem
        }
    }
}
