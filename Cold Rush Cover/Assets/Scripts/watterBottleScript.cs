using UnityEngine;
using System.Collections;

public class watterBottleScript : MonoBehaviour {

    public float cool = -20f;

	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.tag == "Player")
        {
            c.SendMessage(("ChangeTemp"), cool);
            Destroy(gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
