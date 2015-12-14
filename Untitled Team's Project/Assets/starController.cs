using UnityEngine;
using System.Collections;

public class starController : MonoBehaviour {

    public GameObject spawner;
    public GameObject star1;
    public GameObject blackhole;
    public GameObject astroid;
    public GameObject astroidSpawnZone;
    public float astroidRate = 0.5f;
    public GameObject astroidTrail1;
    public Camera mainCam;
    Vector2 d;

    Vector3 rotationStar;
    float rotationBlackHole;
    float spawnerX;
    float spawnerY;
    float randomSpawnAngle;


  

	// Use this for initialization
	void Start () {

        //astroidTrail.transform.position = new Vector3()

        Instantiate(blackhole, new Vector3(spawner.transform.position.x,spawner.transform.position.y , 4), Quaternion.identity);

        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere) as GameObject;

        astroidSpawnZone.transform.localScale = new Vector2(100, 100);

        //Instantiate(astroidSpawnZone, new Vector2(mainCam.pixelWidth - 50, mainCam.pixelHeight + 100), Quaternion.identity);

        spawnerX = mainCam.pixelWidth - 50;
        spawnerY = mainCam.pixelHeight + 100;

        astroidSpawnZone.transform.position = new Vector2(spawnerX, spawnerY);

        InvokeRepeating("spawnDatAstroid", .1f, astroidRate);

        //sphere.transform.position = new Vector2(mainCam.pixelWidth - 50, mainCam.pixelHeight + 100);


        
	
	}

    void SpawningLights()
    {

    }
	
	// Update is called once per frame
	void Update () {

	
	}

    void FixedUpdate()
    {
        star1.transform.RotateAround(spawner.transform.position, Vector3.up, 60 * Time.deltaTime);

        

        
    }

    void spawnDatAstroid()
    {
        randomSpawnAngle = 180 + Random.Range(15, 45);
        Instantiate(astroid, new Vector3(astroidSpawnZone.transform.position.x, astroidSpawnZone.transform.position.y, 4), Quaternion.Euler(0, 0, randomSpawnAngle));

        astroidTrail1.transform.eulerAngles = new Vector3(0, 0, randomSpawnAngle + 180);
        astroidTrail1.transform.rotation = Quaternion.Euler(new Vector3(0, 0, randomSpawnAngle + 180));

    }

   

   
}
