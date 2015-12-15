using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class textFade : MonoBehaviour {

	int fadeTime = 0;
	bool fadeDir = false; //true is fade in, false is fade out
	int fadeTime2 = 0;
	public void fadeIn(string text) {
		gameObject.GetComponent<Text> ().text = text;
		fadeDir = true;
	}
	public void fadeOut(string text) {
		gameObject.GetComponent<Text> ().text = text;
		fadeDir = false;
	}
	// Use this for initialization
	void Start () {
		fadeTime = 0;
		fadeDir = false;
		transform.GetComponent<Text>().color = new Color(1, 1, 1, fadeTime/100f);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(!fadeDir) {
			if(fadeTime2 <= 0) {
				if(fadeTime>0) {
					fadeTime-=10;
					transform.GetComponent<Text>().color = new Color(1, 1, 1, fadeTime/100f);
				}
			} else {
				fadeTime = 100;
				transform.GetComponent<Text>().color = new Color(1, 1, 1, fadeTime/100f);
				fadeTime2--;
			}
		} else {
			if(fadeTime<101) {
				fadeTime+=10;
				transform.GetComponent<Text>().color = new Color(1, 1, 1, fadeTime/100f);
			}
			fadeDir = false;
			fadeTime2 = 70;
		}
	}
}
