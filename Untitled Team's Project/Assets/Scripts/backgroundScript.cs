using UnityEngine;
using System.Collections;

public class backgroundScript : MonoBehaviour {


    public Rigidbody2D COLOR;
    public float speed;

	// Use this for initialization
	void Start () {

        COLOR.velocity = new Vector3(speed, 0,0);

        GameObject lightGameObject = new GameObject("The Light");
        Light lightComp = lightGameObject.AddComponent<Light>();
        lightComp.color = Color.blue;
        lightGameObject.transform.position = new Vector3(COLOR.transform.position.x, 0, 0);
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (COLOR.position.x > 600.00)
        {
            COLOR.transform.position = new Vector3(COLOR.transform.position.x - 1200, 0, 5);

            Debug.Log("Restart");
        }
	
	}
}
