using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

	private static Music Instance;
	// Use this for initialization
	void Start () {
		if (Instance != null)
			Destroy (gameObject);
		else {
			Instance = this;
			DontDestroyOnLoad(gameObject.transform);
		}
	}
}
