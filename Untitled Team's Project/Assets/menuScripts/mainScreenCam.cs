using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mainScreenCam : MonoBehaviour {

    public float orthographicSize = 5;
    public float aspect = 1.33333f;
    public Camera mainCam;

	// Use this for initialization
	void Start () {
        Camera.main.projectionMatrix = Matrix4x4.Ortho(
                   -orthographicSize * aspect, orthographicSize * aspect,
                   -orthographicSize, orthographicSize,
                   mainCam.nearClipPlane, mainCam.farClipPlane);
 }
	
	
	// Update is called once per frame
	void Update () {
	
	}
}
