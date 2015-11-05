using UnityEngine;
using System.Collections;

public class healthAndTemperature : MonoBehaviour {

    public float rightTemp = 27;
    float unbalance = 0;
    public float secondsBetweenHealthLose = 3f;
    public float balancePerSecond = 2;
    float tempBalance = 0;

    public int health = 100;
    public int breakingPoint = 10;
    public int damageByClimate = 30;

    float areaT;

    bool tempDeath = false;

    public Rect debugBox;
    //public GUIStyle guiFont;

    PlayerMovement movement;

	void Start () 
    {
        movement = GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.CompareTag("Death"))
            transform.BroadcastMessage("isGameOver", true);
    }

    void OnTriggerStay2D(Collider2D c)
    {
        if (c.tag == "Temperature")
        {
            areaT = c.GetComponent<areaTemperature>().aT;
            unbalance = areaT - rightTemp;
        }
    }

    void OnTriggerExit2D(Collider2D c)
    {
         if (c.tag == "Temperature")
         {
             unbalance = 0;
         }
    }

	void Update () {

        TemperatureManagement();

        if (unbalance > -breakingPoint)
            movement.doublejumpEnabled = true;
        if (unbalance < breakingPoint)
            //movement.fullSpeed = true;
            movement.canDash = true;

        if (!tempDeath && (tempBalance >= 100 || tempBalance <= -100))
            StartCoroutine(slowDeath());

        if (health <= 0)
        {
            transform.BroadcastMessage("isGameOver", true);
        }
	
	}

    void OnGUI()
    {
        GUI.Box(debugBox, "Health: " + health + "\nTemp Difference: " + unbalance + "\nTemp Balance: " + Mathf.Clamp(Mathf.RoundToInt(tempBalance), -100, 100));
    }

    IEnumerator slowDeath()
    {
        tempDeath = true;

        while (tempBalance >= 100 || tempBalance <= -100)
        {
            if (health > 0)
            {
                yield return new WaitForSeconds(secondsBetweenHealthLose);
                health -= damageByClimate;
                Mathf.Clamp(health, 0, 100);
            }
            else
                break;
        }
    }

    void TemperatureManagement()
    {
        if (unbalance != 0)
        {
            if (unbalance < 0 && tempBalance > -100)
            {
                tempBalance += unbalance / 2 * Time.deltaTime;
                
                //if (tempBalance < -100)
                //    tempBalance = -100;
                //Mathf.Clamp(tempBalance, -100, 100);
                if (unbalance <= -breakingPoint)
                {
                    movement.SendMessage("cantDoubleJump");
                    Debug.Log("Cant double jump");
                }
            }
            else if (unbalance > 0 && tempBalance < 100)
            {
                tempBalance += unbalance / 2 * Time.deltaTime;
                
                //if (tempBalance > 100)
                //    tempBalance = 100;
                //Mathf.Clamp(tempBalance, -100, 100);
                if (unbalance >= breakingPoint)
                {
                    movement.SendMessage("notDash");
                    Debug.Log("Wont dash");
                }
            }
        }
        else
        {
            if(tempBalance >= 0)
            {
                tempBalance -= balancePerSecond * Time.deltaTime;
                Mathf.Clamp(tempBalance, 0, 100);
            }
            else if (tempBalance <= 0)
            {
                tempBalance += balancePerSecond * Time.deltaTime;
                Mathf.Clamp(tempBalance, -100, 0);
            }

            movement.doublejumpEnabled = true;
            //movement.fullSpeed = true;
            movement.canDash = true;
            tempDeath = false;
        }
    }

    public void ChangeTemp(float byThisMuch)
    {
        tempBalance += byThisMuch;
    }
}
