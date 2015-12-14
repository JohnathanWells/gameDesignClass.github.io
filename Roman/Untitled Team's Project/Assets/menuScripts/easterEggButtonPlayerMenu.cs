using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class easterEggButtonPlayerMenu : MonoBehaviour {

    public GameObject DancingPlayer;
    //public GameObject spawnLocation;

    public bool EasterEgg = false;

    public bool condition1 = false;
    public bool condition2 = false;

    int numCount = 0;

    public easterEggMoon EGondition2;

	// Use this for initialization
	void Start () {

        condition2 = false;

        //EGondition2 = GameObject.FindWithTag("easterEggMOON").GetComponent<easterEggMoon>();
	
	}
    void Update()
    {

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (EGondition2.EGcodition2 == true)
        {
            Debug.LogError("OMG PLEASE WORK WTF");
        }

        if (condition1)
        {
            if (condition2)
            {
                EasterEgg = true;
            }
        }
	}

    void OnMouseEnter()
    {
        
        
    }

    void OnMouseDown()
    {
        numCount++;
        if (numCount == 3)
        {
            condition1 = true;
        }

        Debug.Log("OnMOUSEDOWN OMGGGGG");
    }
}
