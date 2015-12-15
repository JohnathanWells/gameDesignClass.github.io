using UnityEngine;
using System.Collections;

public class evilSlothScript : MonoBehaviour {

    public Rigidbody2D rb;
    public GameObject enemy;
    public GameObject player;
    public int _maxHealth = 100;
    public int _curHealth;

    public float lengthRay;
    public float speed = 10f;
    public int rotationSpeed;
    bool death = false;

    public Transform target;

    moveScriptBulletMainMenu bullet;

    [SerializeField]
    private statusIndacator statusIdicator1;
    private Object bullet1;

	// Use this for initialization
	void Start () {

        _curHealth = _maxHealth;

        rb = GetComponent<Rigidbody2D>();

        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (statusIdicator1 != null)
        {
            statusIdicator1.SetHealth(_curHealth, _maxHealth);
        }

        //Rigidbody2D bullet1 = bullet.GetComponent<Rigidbody2D>() as Rigidbody2D;

	
	}
	
	// Update is called once per frame
	void Update () {

        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            // Only needed if objects don't share 'z' value.
            dir.z = 0.0f;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.FromToRotation(Vector3.right, dir),
                    rotationSpeed * Time.deltaTime);

            //Move Towards Target
            transform.position += (target.position - transform.position).normalized
                * speed * Time.deltaTime;

            //Debug.DrawLine(enemy.transform.position, player.transform.position);
        }

        
	
	}

    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down);
        //Debug.DrawRay(transform.position, Vector2.down);
        

       
 

        /*if(death == false)
            InvokeRepeating("movement", 0.5f, .5f);*/
    }

    void movement()
    {
        //Debug.LogError("OMG IT CALLED IT");
       /* float speedX = Random.Range(80, 100);
        float speedY = Random.Range(25, 50);
        Debug.DrawLine(enemy.transform.position, player.transform.position);*/
        //rb.transform.position = Vector2.MoveTowards(rb.transform.position, player.transform.position, speed);
        

        //rb.MovePosition(new Vector2(rb.transform.position.x + speedX, rb.transform.position.y + speedY));
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "bullet")
        {
            Debug.LogError("CollisionDetectedWITHBULLET");
            //enemy.transform.position = new Vector2(0, 0);
            //Destroy(enemy);
            _curHealth = _curHealth - 1;
            Debug.LogError("HEALTH!!!!");
            if (_curHealth <= 0)
            {
                Destroy(enemy);
            }

            if (statusIdicator1 != null)
            {
                statusIdicator1.SetHealth(_curHealth, _maxHealth);
            }

            //Destroy(bullet1);
        }
    }

    
}
