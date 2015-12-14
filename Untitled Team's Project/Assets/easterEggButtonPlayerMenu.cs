using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class easterEggButtonPlayerMenu : MonoBehaviour {

    public GameObject player;
    public GameObject spawnLocation;
    public GameObject cube;

    //Vector2 scalePlayer = new Vector2();
    float scalePlayerX = 0;
    float scalePlayerY = 0;

	// Use this for initialization
	void Start () {


        
        scalePlayerX = player.transform.localScale.x;
        scalePlayerY = player.transform.localScale.y;

        
        //cube.GetComponent<Renderer>().material.color = Color.red;
        //cube.transform.localScale = new Vector2(28.36f, 21.9f);
        Instantiate(cube, new Vector3(spawnLocation.transform.position.x, spawnLocation.transform.position.y, -1), Quaternion.identity);
        //cube.transform.position = new Vector3(spawnLocation.transform.position.x,spawnLocation.transform.position.y, -1);

        Debug.Log(scalePlayerX);
        Debug.Log(scalePlayerY);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
