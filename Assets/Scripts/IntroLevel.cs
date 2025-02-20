using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroLevel : MonoBehaviour {

	public void StraightLevelClick()
    {
        SceneManager.LoadScene("straightPathsLevel");
    }

    public void RotatedLevelClick()
    {
        SceneManager.LoadScene("rotatedPathsLevel");
    }
}
