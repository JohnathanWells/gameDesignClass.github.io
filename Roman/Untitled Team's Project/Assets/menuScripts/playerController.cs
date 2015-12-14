using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour {

    public float fireRate = 0;
    public GameObject player;
    public GameObject fireObject;
    public GameObject enemy;
    public Transform spawnZone;
    public Transform spawnPointPlayer;
    public int _maxHealth;
    int _curHealth;
    public Vector2 speed = new Vector2(50, 50);

    // 2 - Store the movement
    private Vector2 movement;


    public Rigidbody2D rb;
    public Rigidbody2D enemyRb;
    public Rigidbody2D bulletRb;

    //public moveScriptBulletMainMenu boolBoy;

    GameObject bullet;

    float timeToFire = 0;

    public LayerMask whatToHit;
    Transform firePoint;
    Transform rotator;

    [SerializeField]
    private statusIndacator statusIdicator1;


    void Start()
    {

        _curHealth = _maxHealth;

        evilSlothScript enemyScript = enemy.GetComponent<evilSlothScript>();
 
        Debug.Log("Enemy health:" + enemyScript._curHealth);

        if (statusIdicator1 != null)
        {
            statusIdicator1.SetHealth(_curHealth, _maxHealth);
        }

  


        //GameObject g = GameObject.FindGameObjectWithTag("bullet");

        //boolBoy = g.GetComponent<moveScriptBulletMainMenu>();

        //boolBoy = GetComponent<moveScriptBulletMainMenu>();

        //boolBoy.left = true;
        //boolBoy.right = true;
    }

	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody2D>();
        //enemyRb = GetComponent<Rigidbody2D>();

        firePoint = transform.FindChild("FirePoint");

        rotator = transform.FindChild("Rotator");

       

       
	}

   
	
	// Update is called once per frame
	void Update () {

        // 3 - Retrieve axis information
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        // 4 - Movement per direction
        movement = new Vector2(
          speed.x * inputX,
          speed.y * inputY);

        // 6 - Make sure we are not outside the camera bounds
        var dist = (transform.position - Camera.main.transform.position).z;

        var leftBorder = Camera.main.ViewportToWorldPoint(
          new Vector3(0, 0, dist)
        ).x;

        var rightBorder = Camera.main.ViewportToWorldPoint(
          new Vector3(1, 0, dist)
        ).x;

        var topBorder = Camera.main.ViewportToWorldPoint(
          new Vector3(0, 0, dist)
        ).y;

        var bottomBorder = Camera.main.ViewportToWorldPoint(
          new Vector3(0, 1, dist)
        ).y;

        transform.position = new Vector3(
          Mathf.Clamp(transform.position.x, leftBorder, rightBorder),
          Mathf.Clamp(transform.position.y, topBorder, bottomBorder),
          transform.position.z
        );

    
       
	
	}

    void FixedUpdate()
    {
        if (_curHealth <= 0)
        {
            transform.position = new Vector3(spawnPointPlayer.position.x, spawnPointPlayer.position.y, -6);
        }



        rb.velocity = movement;
     

        if (fireRate == 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                shooting();
            }
        }
        else
        {
            if (Input.GetButton("Fire1") && (Time.time > timeToFire))
            {

                timeToFire = Time.time + 1 / fireRate;
                shooting();


            }
        }
        

        
        
    }

    void shooting()
    {

        Vector2 mousePostion = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2 (player.transform.position.x, player.transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, mousePostion - firePointPosition, 100, whatToHit);
        Debug.DrawLine(firePointPosition, mousePostion);
        if (hit.collider != null)
        {
            Debug.DrawLine(firePointPosition, hit.point, Color.red);
        }

        Instantiate(fireObject, player.transform.position, rotator.rotation);
    

    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
            _curHealth = _curHealth - 1;

            Debug.LogError("COLLSION DETECTED: Enemy collision");
            OnTriggerStay2D(coll);
        }

        if (statusIdicator1 != null)
        {
            statusIdicator1.SetHealth(_curHealth, _maxHealth);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        
        if (col.gameObject.tag == "Enemy")
        {
            _curHealth = _curHealth - 1;
            Debug.LogError("Collsion Detected: Enemy Stay");
        }

    }


   
}
