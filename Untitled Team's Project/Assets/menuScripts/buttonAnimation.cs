using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class buttonAnimation : MonoBehaviour {

    //public Sprite myImage;
    public GameObject myBtn;

    /*public void buttonAnim(Sprite myImage)
    {
        
        Debug.Log("Button Hover");
    }*/

    void OnMouseEnter()
    {
        Debug.Log("POINTER ENTER THO BUTTON");
    }

	// Use this for initialization
	void Start () {

       //myBtn = GetComponent<Sprite>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
