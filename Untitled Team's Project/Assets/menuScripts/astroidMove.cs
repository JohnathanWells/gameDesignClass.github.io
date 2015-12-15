using UnityEngine;
using System.Collections;

public class astroidMove : MonoBehaviour {

    public GameObject astroid;
    public Camera mainCam;
    public float speed;

    float rotation;

    Rigidbody2D rb;

	// Use this for initialization
	void Start () {

        rb = astroid.GetComponent<Rigidbody2D>();

        //rb.velocity = new Vector2(1 + Random.Range(100, 150), rb.velocity.y);
        rb.AddForce(transform.right * speed);

        //rb.AddTorque(h);

        //rotation = rb.rotation;
        //Debug.LogError(rotation);
       
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
