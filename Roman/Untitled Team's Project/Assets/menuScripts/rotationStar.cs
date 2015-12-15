using UnityEngine;
using System.Collections;

public class rotationStar : MonoBehaviour {

    public GameObject star1;
    public bool astroidSpawn = false;
    public float speedRot;

	// Use this for initialization
	void Start () {

 

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void FixedUpdate()
    {
        /*if (astroidSpawn == true)
        {
            star1.transform.Rotate(Vector3.forward * Time.deltaTime * speedRot);
        }
        else*/
        star1.transform.Rotate(Vector3.forward * Time.deltaTime * speedRot);
        //star1.transform.RotateAround()
        //blackhole.transform.Rotate(Vector3.forward * Time.deltaTime * 5);
    }
}
