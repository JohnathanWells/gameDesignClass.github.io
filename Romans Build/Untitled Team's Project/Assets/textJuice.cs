using UnityEngine;
using System.Collections;

public class textJuice : MonoBehaviour {

    public GameObject text;
    Vector2 text1 = new Vector2();

    // Use this for initialization
    void Start()
    {

        //sloth = GetComponent<Collider2D>;
        text1 = new Vector2(text.transform.localScale.x, text.transform.localScale.y);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseEnter()
    {
        Debug.Log("Pointer Enter");
        text.transform.localScale = new Vector2(text.transform.localScale.x + .25f, text.transform.localScale.y + 0.25f);
        Invoke("Reset", .5f);
    }

    void Reset()
    {
        text.transform.localScale = new Vector2(text1.x, text1.y);
    }
}
