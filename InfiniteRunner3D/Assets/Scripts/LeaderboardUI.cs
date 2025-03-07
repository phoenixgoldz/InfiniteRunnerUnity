using UnityEngine;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text distanceText;

    void Start()
    {
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        float lastDistance = PlayerPrefs.GetFloat("LastDistance", 0);

        scoreText.text = "Score: " + lastScore;
        distanceText.text = "Distance: " + Mathf.FloorToInt(lastDistance) + "m";
    }
}
