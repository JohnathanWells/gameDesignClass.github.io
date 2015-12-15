using UnityEngine;
using System.Collections;

public class changeToScene : MonoBehaviour {


    public Camera mainCam;
    public GameObject button;
    float fov;
    float startingFOV;
    public bool juiceItUpBB = false;
    //float minFov = 15f;
    //float maxFov = 90f;
    //float sensitivity = 10f;

    void Start()
    {
       
    }

    public void ChangeToScene(string sceneToChangeTo)
    {
        startingFOV = mainCam.fieldOfView;
        //cameraReset();
        Application.LoadLevel(sceneToChangeTo);
        /*Debug.Log("Button Clicked");
        Invoke("juicer", 0.1f);
        //button.transform.localScale = new Vector3(button.transform.localScale.x, button.transform.localScale.y + .05f, 0);
        Invoke("cameraReset", 0.5f);
        //button.transform.localScale = new Vector3(button.transform.localScale.x, button.transform.localScale.y - .01f, 0);
       //mainCam.fieldOfView = startingFOV;*/
       
    }

    void cameraReset()
    {
        //startingFOV = mainCam.fieldOfView;
        mainCam.fieldOfView = startingFOV;
    }

    void juicer()
    {
        fov = mainCam.fieldOfView;
        
        if (juiceItUpBB)
        {
            
       // fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        //fov = Mathf.Clamp(fov, minFov, maxFov);
            fov = fov - 1;
            //button.transform.localScale = new Vector3(button.transform.localScale.x, button.transform.localScale.y + 1f, 0);
             mainCam.fieldOfView = fov;
             //juiceItUpBB = false;
        }

       
        
    }

    void FixedUpdate()
    {
        fov = mainCam.fieldOfView;


    }
}
