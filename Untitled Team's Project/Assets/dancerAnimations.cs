using UnityEngine;
using System.Collections;

public class dancerAnimations : MonoBehaviour {

    public bool dance;

	// Use this for initialization
	void Start () {
        
  
	
	}
	
	// Update is called once per frame
	void Update () {
	      if(dance)
          {
              GameObject.Find ("PlayerSprite").GetComponent<Animator>().SetBool("Dance", true);
              
          }
            
        else
            GameObject.Find("PlayerSprite").GetComponent<Animator>().SetBool("Dance", false);
	}
}
