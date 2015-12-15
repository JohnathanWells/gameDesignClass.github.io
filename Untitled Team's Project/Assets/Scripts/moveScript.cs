using UnityEngine;
using System.Collections;

public class moveScript : MonoBehaviour {

    public Rigidbody2D bulletLeft;
    public Rigidbody2D bulletRight;

    public float speedLeft = -1;
    public float speedRight = 1;


	// Use this for initialization
	void Start () {

        bulletLeft.velocity = new Vector3(speedLeft, 0, 0);
        bulletRight.velocity = new Vector3(speedRight, 0, 0);

        /*if(GameObject.Find("Logic").GetComponent<Logic>().collision(new Rect(transform.position.x/8-3/8, transform.position.y/8-3/8, 3/8, 3/8), .1f) )
        {
            Destroy(bulletLeft);
            Destroy(bulletRight);
        }*/
	
	}
	
	// Update is called once per frame
	void Update () {

        if (GameObject.Find("Logic").GetComponent<Logic>().destructableCollision(new Rect(transform.position.x / 4, transform.position.y / 4, 0, 0), .1f)) {
            GameObject.Find("Logic").GetComponent<Logic>().setTile((int)Mathf.Round(transform.position.x / 4), (int)Mathf.Round(transform.position.y / 4), "");
            Destroy(gameObject);
        }
        if (GameObject.Find("Logic").GetComponent<Logic>().collision(new Rect(transform.position.x / 4 - 3 / 8, transform.position.y / 4 - 3 / 8, 3 / 8, 3 / 8), .1f)) {
            //Destroy(bulletLeft);
            //Destroy(bulletRight);
            Destroy(gameObject);
        }
	
	}
}
