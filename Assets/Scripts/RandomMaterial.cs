using UnityEngine;
using System.Collections;

public class RandomMaterial : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        GetComponent<Renderer>().material = GetRandomMaterial();
    }
	


    /// <summary>
    /// helper method to get a random color
    /// </summary>
    /// <returns></returns>
    public Material GetRandomMaterial()
    {
        int x = Random.Range(0, 5);
        if (x == 0)
            return Resources.Load("Materials/redMaterial") as Material;
        else if (x == 1)
            return Resources.Load("Materials/greenMaterial") as Material;
        else if (x == 2)
            return Resources.Load("Materials/blueMaterial") as Material;
        else if (x == 3)
            return Resources.Load("Materials/yellowMaterial") as Material;
        else if (x == 4)
            return Resources.Load("Materials/purpleMaterial") as Material;
        else
            return Resources.Load("Materials/redMaterial") as Material;
    }

}
