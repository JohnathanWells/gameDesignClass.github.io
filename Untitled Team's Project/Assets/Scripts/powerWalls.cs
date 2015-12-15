using UnityEngine;
using System.Collections;

public class powerWalls : MonoBehaviour {

    Vector3 pos;
    float position = 0; //Red
    float positionB = 0; //Blue
    float positionG = 0; //Green
    //public float speed;

    public Transform Red;
    public Transform Green;
    public Transform Blue;
    //Vector3 redPos;
    //GameObject redTexture;
    //Rigidbody rb;
    //Vector3 restart(-80f,0f,0f);
    //float speed = 100000;

    //Rigidbody rb;

    // Use this for initialization
    void Start()
    {
            

        for (position = -500; position <= 500 ; position = position + 300)
        {
         
            Instantiate(Red, new Vector3(position, 0, 5), Quaternion.identity);

        }


        for (positionB = -400; positionB <= 500; positionB = positionB + 300)
        {
            
            Instantiate(Blue, new Vector3(positionB, 0, 5), Quaternion.identity);
            //Debug.Log("Creating Blue");

        }

        for (positionG = -300; positionG <= 600; positionG = positionG + 300)
        {
      
            Instantiate(Green, new Vector3(positionG, 0, 5), Quaternion.identity);
            //Debug.Log("Creating Green");
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {

        /*if (Red.transform.position.x >= 10)
        {
            Destroy(Red);
            Start();
        }
        if (Green.transform.position.x >= 10)
        {
            Destroy(Green);
            Start();
        }

        if (Blue.transform.position.x >= 10)
        {
            Destroy(Blue);
            Start();
        }*/

        /* Vector3 movement = new Vector3(
           speed.x * direction.x,
           speed.y * direction.y,
           0);

         movement *= Time.deltaTime;
         transform.Translate(movement);*/
    }
	// Update is called once per frame
	void Update () {
	
	}
}
