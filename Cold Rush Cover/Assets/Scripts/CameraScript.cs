using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
    public GameObject Player;
    private int lives = 3;
    private Vector3 Checkpoint;
    private GameObject PlayerAlive;
    public int CheckpointX;
    public int CheckpointY;
    public int CheckpointZ;
    public string currentLevel;
    public bool FollowsX;
    public bool FollowsY;
    private float FollowX;
    private float FollowY;

    void Start()
    {
        Checkpoint = new Vector3(CheckpointX, CheckpointY, CheckpointZ);
        PlayerAlive = GameObject.FindGameObjectWithTag("Player");
        //SpawnPlayer(Checkpoint);
    }

    //private void SpawnPlayer(Vector3 Checkpoint)
    //{
    //    PlayerAlive = Instantiate(Player, Checkpoint, Quaternion.identity) as GameObject;
    //}

    void Update()
    {
        //if (!PlayerAlive)
        //{
        //    Application.LoadLevel("" + currentLevel);
        //}
        if (FollowsX == true)
        {
            FollowX = PlayerAlive.transform.position.x;
        }

        if (FollowsY == true)
        {
            FollowY = PlayerAlive.transform.position.y;
        }

        transform.position = new Vector3(FollowX, FollowY, -10);
    }

    void OnGUI()
    {
        
    }
}
