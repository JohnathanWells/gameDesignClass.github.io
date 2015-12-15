using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class bgMainScreen : MonoBehaviour {

    public Camera mainCam;
    public GameObject bgMainScreenObject;
    public GameObject light1;
    float scaleX = 1;
    float scaleY = 1;
    public GameObject light2;
    public GameObject light3;
    public GameObject playBtn;
    public GameObject spawnLocationButton;

    Vector2 d;
    Vector2 size; 
    //int camX;
    //int camY;
    float randomValue;
    int randomValueY;
    float scaleLightY;
    float scaleLightX;


	// Use this for initialization
	void Awake () {

        

        d = mainCam.ScreenToWorldPoint(new Vector2(0, 0));
        
        //bgMainScreenObject.transform.localScale = new Vector2(mainCam.transform.localScale.x / 1.62f, mainCam.transform.localScale.y / 3.36f);
        Instantiate(bgMainScreenObject, new Vector3(bgMainScreenObject.transform.position.x, bgMainScreenObject.transform.position.y, 10), Quaternion.identity);
        //Instantiate(bgMainScreenObject, new Vector3(d.x + 800, d.y + 190, 5), Quaternion.identity);



        for (d.x = 0; d.x <= mainCam.pixelWidth; d.x = d.x + 25)
        {
            for (d.y = 0; d.y <= mainCam.pixelHeight; d.y = d.y + 25)
            {
                //Instantiate(light1, new Vector2(d.x, d.y), Quaternion.identity);
                SpawningLights();
            }
        }


	
	}

    void Start()
    {
        //playBtn = GetComponent<Button>();
        //Debug.Log(mainCam.pixelWidth);
        //Debug.Log(mainCam.pixelHeight);

    }

    void SpawningLights()
    {
        int randomNumber = Random.Range(0, 200);

        if (randomNumber <= 5)
        {
            Instantiate(light1, new Vector3(d.x, d.y,6), Quaternion.identity);
            //Debug.Log("LIGHT ONE SPAWN");
        }
        if (randomNumber <= 15 && randomNumber >= 5)
        {
            Instantiate(light2, new Vector3(d.x, d.y,6), Quaternion.identity);
            //Debug.Log("LIGHT 2 Spawn");
        }
        if(randomNumber >= 15 && randomNumber <= 17)
        {
            light1.transform.localScale = new Vector2(light1.transform.localScale.x + 2, light1.transform.localScale.y + 2);
            Instantiate(light1, new Vector3(d.x, d.y, 6), Quaternion.identity);
            //Debug.Log("LIGHT ONE SPAWN LARGER");
            Reset();

        }
        else
            ;




    }

    void Reset()
    {
        light1.transform.localScale = new Vector2(scaleX, scaleY);
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    

   
}
