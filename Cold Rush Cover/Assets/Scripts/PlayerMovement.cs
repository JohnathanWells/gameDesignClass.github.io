using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public bool facingRight = true;
    public bool isGrounded = false;
    public bool maxSpeedIsMin = false;
    public bool isFacingAWall = false;
    public bool canDoubleJump;
    public bool canDash = true;
    private bool gameOver = false;
    private bool dashCooled = true;
    public bool doublejumpEnabled = true;
    //public bool fullSpeed = true;

    public float jumpForce = 1f;
    public float dashForce = 1f;
    public float secondJumpForce = 1f;
    public float collisionSkin = 0.01f;
    public float distanceToGround = 0.1f;
    public float wallCollisionSkin = 0.02f;
    public float distanceToWall = 0.02f;
    public float maxSpeed = 3;
    public float speed = 1;
    public float dashCooldown = 1f;
    public float timeTrailDissapear = 1f;
    //public float slowSpeed = 0.8f;
    float h;
    private float gravity;
    private float tempS;

    int directionAxis;

    public string levelName;

    Rigidbody2D reggiebody;

    public Material dashTrail;

    //public Vector2 spawnPoint;

    Vector3 destination;
    Vector3 velocity = Vector3.zero;

    public LayerMask myLayerMask;
    public LayerMask ignoreMask;

	// Use this for initialization
	void Start () {
        reggiebody = transform.GetComponent<Rigidbody2D>();
        Debug.Log(transform.GetComponent<BoxCollider2D>().bounds.extents.x * 2);
        destination = transform.position;

        if (maxSpeedIsMin)
            maxSpeed = speed;

        gravity = reggiebody.gravityScale;
        tempS = speed;
	}
	
	// Update is called once per frame
	void Update () {

        if (!gameOver)
        {
            isGrounded = checkGround();
            facingRight = directionFace();

            if (facingRight)
                directionAxis = 1;
            else
                directionAxis = -1;

            if (Input.GetButtonDown("Jump"))
            {
                if (!isGrounded && canDoubleJump && doublejumpEnabled)
                {
                    reggiebody.velocity = new Vector2(reggiebody.velocity.x, 0);
                    reggiebody.AddForce(transform.up * secondJumpForce);
                    canDoubleJump = false;
                    //Debug.Log("Double jumped");
                }

                if (isGrounded)
                {
                    reggiebody.AddForce(transform.up * jumpForce);
                    canDoubleJump = true;
                }
                //Debug.Log("Jumping");
            }

            //if (!fullSpeed)
            //    speed = slowSpeed;
            //else
                speed = tempS;

            h = Input.GetAxis("Horizontal");
            isFacingAWall = false;
            //isFacingAWall = checkWall(1);
            drawLines();
            if (h * reggiebody.velocity.x <= maxSpeed)
            {
                isFacingAWall = checkWall(h);
                if (!isFacingAWall)
                    transform.Translate(transform.right * speed * h * Time.deltaTime);

                if ((h > 0 && transform.localScale.x < 0) || (h < 0 && transform.localScale.x > 0))
                    transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
                //reggiebody.AddForce(transform.right * speed * Input.GetAxis("Horizontal"));
                //destination += transform.right * h * speed;
                //transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, 0.3f);
            }

            if (Input.GetButtonDown("Dash"))
            {
                dashPlayer();
            }
        }
	}

    void OnGUI()
    {
        if (gameOver)
        {
            GUI.Label(new Rect(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(200, 200)), "You died from EXTREME conditions");
            GUI.Label(new Rect(new Vector2(700, 100), new Vector2(100, 100)), "Press T to restart");
            if (Input.GetKeyDown(KeyCode.T))
            {

                Application.LoadLevel(levelName);
            }
        }
    }

    void dashPlayer()
    {
        RaycastHit2D Distance = Physics2D.Linecast(transform.position, new Vector2(transform.position.x + dashForce * directionAxis, transform.position.y), ignoreMask);
        if (!Distance && Distance.distance == 0)
            Distance.distance = dashForce;

        //Debug.Log("Directio Axis: " + directionAxis + "\nDistance before Wall: " + Distance.distance);
        Debug.DrawLine(transform.position, new Vector2(transform.position.x + Distance.distance * directionAxis, transform.position.y));

        if (directionAxis * reggiebody.velocity.x <= maxSpeed + dashForce && canDash && dashCooled)
        {
            isFacingAWall = checkWall(directionAxis);
            if (!isFacingAWall && Distance.distance > 0)
            {
                //reggiebody.velocity = new Vector2(dashForce * directionAxis, 0);
                //reggiebody.AddForce(new Vector2(dashForce * directionAxis, 0));
                reggiebody.velocity = Vector2.zero;
                StartCoroutine(dashLine(transform.position, new Vector2(transform.position.x + Distance.distance * directionAxis - directionAxis * GetComponent<BoxCollider2D>().bounds.extents.x, transform.position.y)));
                transform.Translate(transform.right * Distance.distance * directionAxis );
                StartCoroutine(cooldownDash());
            }
        }
    }

    bool directionFace()
    {
        if (transform.localScale.x > 0)
            return true;
        else
            return false;
    }

    bool checkGround()
    {
        //bool something = false;
        bool result = false;

        for (int a = 0; a <= transform.GetComponent<BoxCollider2D>().bounds.extents.x * 2 / 0.3; a++)
        {
            if (!result)
                result = Physics2D.Linecast(new Vector2(transform.position.x - transform.GetComponent<BoxCollider2D>().bounds.extents.x + a * 0.3f, transform.position.y - transform.GetComponent<BoxCollider2D>().bounds.extents.y - collisionSkin), new Vector2(transform.position.x - transform.GetComponent<BoxCollider2D>().bounds.extents.x + a * 0.3f, transform.position.y - transform.GetComponent<BoxCollider2D>().bounds.extents.y - collisionSkin - distanceToGround), myLayerMask);
            //Debug.DrawLine(new Vector2(transform.position.x - transform.GetComponent<BoxCollider2D>().bounds.extents.x + a * 0.3f, transform.position.y - transform.GetComponent<BoxCollider2D>().bounds.extents.y - collisionSkin), new Vector2(transform.position.x - transform.GetComponent<BoxCollider2D>().bounds.extents.x + a * 0.3f, transform.position.y - transform.GetComponent<BoxCollider2D>().bounds.extents.y - collisionSkin - distanceToGround));
        }

        return result;
    }

    bool checkWall(float direction)
    {
        int d;
        if (direction < 0)
            d = -1;
        else if (direction == 0)
            return false;
        else
            d = 1;

        bool something = false;

        for (int a = 0; a <= transform.GetComponent<BoxCollider2D>().bounds.extents.y * 2 / 0.2f; a++)
        {
            if (!something)
                something = Physics2D.Linecast(new Vector2(transform.position.x + (transform.GetComponent<BoxCollider2D>().bounds.extents.x + wallCollisionSkin) * d, transform.position.y - transform.GetComponent<BoxCollider2D>().bounds.extents.y + (a + 1) * 0.2f + wallCollisionSkin), new Vector2(transform.position.x + (transform.GetComponent<BoxCollider2D>().bounds.extents.x + distanceToWall + wallCollisionSkin) * d, transform.position.y - transform.GetComponent<BoxCollider2D>().bounds.extents.y + a * 0.2f + wallCollisionSkin), 1 << 8);
            //Debug.DrawLine(new Vector2((transform.position.x + transform.GetComponent<BoxCollider2D>().bounds.extents.x + collisionSkin) * direction, transform.position.y - transform.GetComponent<BoxCollider2D>().bounds.extents.y + a * 0.3f + collisionSkin), new Vector2((transform.position.x + transform.GetComponent<BoxCollider2D>().bounds.extents.x + distanceToGround + collisionSkin) * direction, transform.position.y - transform.GetComponent<BoxCollider2D>().bounds.extents.y + a * 0.3f + collisionSkin));
        }

        return something;
    }

    IEnumerator cooldownDash()
    {
        dashCooled = false;
        yield return new WaitForSeconds(dashCooldown);
        dashCooled = true;
    }

    IEnumerator dashLine(Vector2 begining, Vector2 end)
    {
        LineRenderer Trail = gameObject.AddComponent<LineRenderer>();

        Trail.SetColors(Color.red, new Color(1, 0, 0, 0.8f));

        Trail.material = dashTrail;
        Trail.SetWidth(0.8f, 0.8f);

        Trail.SetPosition(0, begining);
        Trail.SetPosition(1, end);

        yield return new WaitForSeconds(timeTrailDissapear);

        Destroy(Trail);

    }

    void drawLines()
    {
        float direction;
        for (int a = 0; a <= transform.GetComponent<BoxCollider2D>().bounds.extents.y * 2 / 0.2f; a++)
        {
            direction = 1;
            Debug.DrawLine(new Vector2((transform.position.x + transform.GetComponent<BoxCollider2D>().bounds.extents.x + collisionSkin) * direction, transform.position.y - transform.GetComponent<BoxCollider2D>().bounds.extents.y + a * 0.2f + collisionSkin), new Vector2((transform.position.x + transform.GetComponent<BoxCollider2D>().bounds.extents.x + distanceToGround + collisionSkin) * direction, transform.position.y - transform.GetComponent<BoxCollider2D>().bounds.extents.y + a * 0.2f + collisionSkin));
            
            direction = -1;
            Debug.DrawLine(new Vector2(transform.position.x + (transform.GetComponent<BoxCollider2D>().bounds.extents.x + collisionSkin) * direction, transform.position.y - transform.GetComponent<BoxCollider2D>().bounds.extents.y + a * 0.2f + collisionSkin), new Vector2(transform.position.x + (transform.GetComponent<BoxCollider2D>().bounds.extents.x + distanceToGround + collisionSkin) * direction, transform.position.y - transform.GetComponent<BoxCollider2D>().bounds.extents.y + a * 0.2f + collisionSkin));
        }

        for (int a = 0; a <= transform.GetComponent<BoxCollider2D>().bounds.extents.x * 2 / 0.3f; a++)
            Debug.DrawLine(new Vector2(transform.position.x - transform.GetComponent<BoxCollider2D>().bounds.extents.x + a * 0.3f, transform.position.y - transform.GetComponent<BoxCollider2D>().bounds.extents.y - collisionSkin), new Vector2(transform.position.x - transform.GetComponent<BoxCollider2D>().bounds.extents.x + a * 0.3f, transform.position.y - transform.GetComponent<BoxCollider2D>().bounds.extents.y - collisionSkin - distanceToGround));

    }

    void isGameOver(bool dead)
    {
        gameOver = dead;
    }

    void notDash()
    {
        canDash = false;
    }

    void cantDoubleJump()
    {
        doublejumpEnabled = false;
    }
}
