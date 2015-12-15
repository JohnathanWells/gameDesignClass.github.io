using UnityEngine;
using System.Collections;

public class moveScriptBulletMainMenu : MonoBehaviour {

    public Rigidbody2D bullet;
    public GameObject bullet1;
    public float speed;
    public bool left; 
    public bool right;
    public float speedLeft;
    public float speedRight;

	// Use this for initialization
	void Start () {

        /*bullet.velocity = new Vector3(0, speed, 0);
        
        if (left == true)
            Debug.LogError("LEFT TRUE");
        if (right == true)
            Debug.LogError("RIGHT TRUE");*/

        //bullet.velocity = Vector2.right * speed;
	
	}

    void OnTriggerEnter2D(Collider2D coll)
    {
        Debug.LogError("Bullet Hit SOMETHING");
        if (coll.tag == "Enemy")
        {
            Destroy(bullet1);
        }
    }

	
	// Update is called once per frame
	void Update () {

        transform.Translate(Vector3.right * speed);
	
	}
}
