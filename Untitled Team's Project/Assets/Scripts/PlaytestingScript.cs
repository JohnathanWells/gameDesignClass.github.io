using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlaytestingScript : MonoBehaviour {

    
    public GameObject timeUI;
    Text timeCounter;
    float time;
    string outputTime;
    string[] timeByLevel;
    string deathLog;
    public string fileLocation = "DeathLog";

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

	// Use this for initialization
	void Start () {
        timeCounter = timeUI.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
        outputTime = turnIntoTime(time);
        timeCounter.text = "Time: " + outputTime;
	}

    string turnIntoTime(float input)
    {
        int minutes = 00;
        int seconds = 00;
        //int miniseconds = 00;
        string endString = "00 : 00";

        if (input > 60)
        {
            minutes = Mathf.RoundToInt(input / 60);
            input %= input / 60;
        }
            seconds = Mathf.RoundToInt(input);


        if (seconds < 10 && minutes < 10)
            endString = ("0" + minutes + ":" + "0" + seconds);
        else if (seconds < 10 && minutes >= 10)
            endString = (minutes + ":" + "0" + seconds);
        else if (seconds >= 10 && minutes < 10)
            endString = ("0" + minutes + ":" + seconds);
        else if (seconds >= 10 && minutes >= 10)
            endString = (minutes + ":" + seconds);

        return endString;
    }

    public void registerDeath(Vector2 position)
    {
        string levelName = Application.loadedLevelName;
        deathLog += levelName + " - " + outputTime + " [" + Mathf.RoundToInt(position.x) + ", " + Mathf.RoundToInt(position.y) + "]" + System.Environment.NewLine;
        System.IO.File.WriteAllText(fileLocation, deathLog);
    }
}
