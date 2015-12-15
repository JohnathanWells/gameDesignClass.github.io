using UnityEngine;
using System.Collections;

public class easterEggMoon : MonoBehaviour {

    public int moonClickCount = 0;
    public bool EGcodition2 = false;

    public easterEggButtonPlayerMenu condition2;

	// Use this for initialization
	void Start () {


        condition2 = GameObject.FindWithTag("easterEgg").GetComponent<easterEggButtonPlayerMenu>();

	}
	
	// Update is called once per frame
	void FixedUpdate () {

        
	
	}

    void OnMouseDown()
    {
        Debug.Log("MOON CLICKED");
        moonClickCount++;
        Debug.LogError(moonClickCount);

        if (moonClickCount >= 3)
        {
            EGcodition2 = true;
            condition2.condition2 = true;
            Debug.LogError(condition2.condition2);
            Debug.LogError(EGcodition2);
        }
        else
        {
            condition2.condition2 = false;
        }

    }
}
