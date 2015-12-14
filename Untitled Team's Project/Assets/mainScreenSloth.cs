using UnityEngine;
using System.Collections;

public class mainScreenSloth : MonoBehaviour {

    public GameObject sloth;
    public float scaleFactor = 0.1f;
    Vector2 sloth1 = new Vector2();

	// Use this for initialization
	void Start () {

        //sloth = GetComponent<Collider2D>;
        sloth1 = new Vector2(sloth.transform.localScale.x, sloth.transform.localScale.y);	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseEnter()
    {
        Debug.Log("Pointer Enter");
        sloth.transform.localScale = new Vector2(sloth.transform.localScale.x + scaleFactor, sloth.transform.localScale.y + scaleFactor);
        Invoke("Reset", .5f);
    }

    void Reset()
    {
        sloth.transform.localScale = new Vector2(sloth1.x , sloth1.y);
    }

}
