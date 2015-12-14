using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Logic : MonoBehaviour {
    public bool INSERTTHEJUICE = false;

    public ParticleSystem particleJuice;
    Vector3 playerPosition;


	public int levelNum = 1;

	public AudioClip[] sounds;

    public GameObject bulletLeft;
    public GameObject bulletRight;
    public PlaytestingScript stats;
    //float bulletSpeed;
    float coinCount = 0;
    bool textColor = false;

    
	public bool died = false;
	private bool won = false;

    public ParticleSystem dashParticles;
    public ParticleSystem doubleJumpParticles;
    public ParticleSystem shootParticles;
    public ParticleSystem blockDestructionParticles;
	public ParticleSystem deathParticles;
	public ParticleSystem winCelebrationParticles;
	public ParticleSystem coinParticles;


	public Transform tile;
	public Transform coinPrefab;
	public Sprite[] tiles;

	public bool renderAllTiles = false;

	public Texture2D level;

	public bool useScene = false;
	public string[,] scene;
	Vector2 spawnPoint;

	public Camera mainCamera;
	Vector2 cam;
	Vector2 smoothCam;
	Vector2 smoothCam2;
	int screenshakeWOOOOAH;
	Rect camBounds;

	public Transform player;
	Vector2 playerPos;
	Vector2 playerVelocity;
	bool playerLastDirIsLeft = false;
	bool playerFalling = true;
	float playerLastFallSpeed = 0;
	int playerJumpNum = 0;
	int playerDashTimer = 0;

	public Transform backgroundSlab;
	public string[] backgroundTypes;
	public int backgroundWidth = 25;
	public float backgroundScrollSpeed = 4;
	float backgroundScrollPosition;
	int numBackgrounds;

	bool alreadyDashed = true;


	public string[] signs;
	public Transform signTextBox;


	public Text actionText;
    public Text coinText;

	bool deletingTiles = false;

	string action = "double";

	// Use this for initialization
	void Start () {

        //particleJuice = particleJuice.GetComponent<ParticleAnimator>;
        //particleJuice.enableEmission = false;
        GameObject.Find("PlayerSprite").GetComponent<Animator>().SetBool("Dance", false);

		stats = GameObject.Find ("Stats").GetComponent<PlaytestingScript> ();
		if(!useScene)
            loadLevel(level);//Load the level image to a multidimensional array
		
		playerPos = new Vector2 (spawnPoint.x, spawnPoint.y);

		player.transform.position = new Vector3 (playerPos.x*4-2, playerPos.y*4+4, -1);

		updateSpritePosition ();


		cam.y = smoothCam.y = smoothCam2.y = playerPos.y+4;
		cam.x = smoothCam.x = smoothCam2.x = playerPos.x+(playerLastDirIsLeft?-1:1);
		

		mainCamera.transform.position = new Vector3 (cam.x*4, cam.y*4, -10);

		mainCamera.transform.position = new Vector3 (cam.x*4, cam.y*4, -50);

		//This line calculates which blocks are going to be on the screen based on the camera position. It is also in the updateCameraAndTiles section
		camBounds = new Rect( (int) (mainCamera.transform.position.x-mainCamera.orthographicSize*2*mainCamera.aspect/2)/4-2,  (int)(mainCamera.transform.position.y - mainCamera.orthographicSize) / 4 - 2, (int) (mainCamera.orthographicSize*2*mainCamera.aspect/2)/2+5, (int) (mainCamera.orthographicSize)/2+5);
		
		if (renderAllTiles) {
			for (int y = -1; y<level.height+1; y++) {
				for (int x = -1; x<level.width+1; x++) {
					if (getTile (x, y) != "") {
						drawTile (x, y);
					}
				}
			}
		} else {
			//These loops set the initial blocks in the camera view. Do not change the camera size during runtime - it will cause instabilities
			for (int y = (int) camBounds.y; y<(int) camBounds.y+camBounds.height; y++) {
				for (int x = (int) camBounds.x; x<(int) camBounds.x+camBounds.width; x++) {
					if (getTile (x, y) != "") {
						drawTile (x, y);
					}
				}
			}
		}
		numBackgrounds = (int)(camBounds.width*4/backgroundWidth)+2;
		for (int x = 0; x<numBackgrounds; x++) {
			Object tmp = Instantiate (backgroundSlab, new Vector3(-1000*4, -1000*4, 5), Quaternion.identity);
			tmp.name = "background"+x;
			GameObject.Find(tmp.name).GetComponent<Transform>().localScale = new Vector3(backgroundWidth, camBounds.height*4+20, 1);
		}
	}
	void FixedUpdate () {

		//mainCamera.transform.position += new Vector3 (0, -.05f, 0);

		cam.y += (playerPos.y-cam.y+4)/10;
		cam.x += (playerPos.x+(playerLastDirIsLeft?-1:1)-cam.x)/10;
		
		screenshakeWOOOOAH = Mathf.Max(screenshakeWOOOOAH-1, 0);
		smoothCam2.x += (cam.x-smoothCam.x)/10;
		smoothCam2.y += ((cam.y+camBounds.height/6)-smoothCam.y)/10;
		
		smoothCam.x = smoothCam2.x+(Random.Range(0.0f, 1.0f)-.5f)*Mathf.Max(0, Mathf.Log(screenshakeWOOOOAH*100)/25);

		smoothCam.y = smoothCam2.y+(Random.Range(0.0f, 1.0f)-.5f)*+Mathf.Max(0, Mathf.Log(screenshakeWOOOOAH*100)/25);

		smoothCam.y = smoothCam2.y+(Random.Range(0.0f, 1.0f)-.5f)*Mathf.Max(0, Mathf.Log(screenshakeWOOOOAH*100)/25);


		backgroundScrollPosition += backgroundScrollSpeed;
		for (int x = 0; x<numBackgrounds; x++) {
			float posX = (mainCamera.transform.position.x-Mathf.Floor(camBounds.width*4/2/backgroundWidth)*backgroundWidth-backgroundWidth/2+x*backgroundWidth-.5f*backgroundWidth-modulus(mainCamera.transform.position.x-backgroundScrollPosition/16, backgroundWidth));
			GameObject.Find("background"+x).GetComponent<Transform>().position = new Vector3(posX, mainCamera.transform.position.y+camBounds.height*2+10, 5);
			Color col = new Color();
			string tmpType = backgroundAtX(posX);
			if(tmpType == "shoot") col = new Color(224f/255f, 51f/255f, 53f/255f, 1);
			else if(tmpType == "dash") col = new Color(0/255f, 191f/255f, 95f/255f, 1);
			else if(tmpType == "double") col = new Color(93f/255f, 86f/255f, 210f/255f, 1);
			GameObject.Find("background"+x).GetComponent<SpriteRenderer>().color = col;
		}
		action = backgroundAtX(player.transform.position.x);
		actionText.text = action;


		mainCamera.transform.position = new Vector3 (cam.x*4, cam.y*4, -10);

		mainCamera.transform.position = new Vector3 (cam.x*4, cam.y*4, -50);


		updateCameraAndTiles();



		doPlayerMovement();
	}
	string backgroundAtX(float x) {
		return backgroundTypes [(int)(modulus (x / backgroundWidth - backgroundScrollPosition / 16 / backgroundWidth, backgroundTypes.Length))];
	}
	
	void Update() {

        /*if (Input.GetMouseButtonDown(0))
            Debug.Log("Pressed left click.");*/

        /*if (Input.GetMouseButtonDown(1)) {
            Debug.Log("Pressed right click.");
            Debug.Log(Input.mousePosition);
            bulletSpeed = bulletSpeed * Time.deltaTime;
            Instantiate(bullet, player.transform.position, Quaternion.identity);
            bullet.transform.position = Vector3.MoveTowards(player.position, Input.mousePosition, bulletSpeed);
        }*/

        /*if (Input.GetMouseButtonDown(2))
            Debug.Log("Pressed middle click.");*/


		if(!died && !won)
            doPlayerMovement ();
	}
	void updateSpritePosition() {
		player.transform.position = new Vector3 (playerPos.x * 4 - 2, playerPos.y * 4 + 4, -1);
	}
	string backgroundAtX(float x) {
		return backgroundTypes [(int)(modulus ((x+1.5f) / backgroundWidth - backgroundScrollPosition / 16 / backgroundWidth, backgroundTypes.Length))];
	}
	
	void Update() {

		Vector2 mP = new Vector2((int)(mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)).x)/4, (int)(mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)).y)/4+1);

		/*
		if(Input.GetMouseButtonDown(0)) {
			if(getTile((int) mP.x, (int) mP.y) == "1") {
				deletingTiles = true;
				setTile((int) mP.x, (int) mP.y, "");
			} else {
				deletingTiles = false;
				setTile((int) mP.x, (int) mP.y, "1");
			}
		}
		if(Input.GetMouseButton(0)) {
			if(deletingTiles) {
				setTile((int) mP.x, (int) mP.y, "");
			} else {
				setTile((int) mP.x, (int) mP.y, "1");
			}
		}*/

		//Some key stuff happens here, because Unity


		//Where jump and double jump is handled

		if (playerFalling) {
			if (Input.GetButtonDown ("Jump") && action == "double" && playerJumpNum <= 1) {
				playSound (0);
				playerJumpNum++;
				playerVelocity.y = .5f;

                EmitParticles(doubleJumpParticles, new Vector2(player.position.x + 2f, player.position.y - 0.2f), new Vector3(-13, 0, 0), null);

			}
		} else {
			if (Input.GetButtonDown ("Jump") && playerJumpNum < 1) {
				playSound (5);
				playerJumpNum++;
				playerVelocity.y = .5f;
			}
		}

		//Where dashing is handled

		if (action == "dash") {
			if(!alreadyDashed) {
				if (Input.GetButtonDown ("DashLeft")) {
					playSound (6);
					alreadyDashed = true;
					playerDashTimer = -20;

					EmitParticles(dashParticles, new Vector3(player.position.x + 4f, player.position.y - 1.5f, player.position.z - 10), new Vector3(90, 90, 0), player.transform);

				}
				if (Input.GetButtonDown ("DashRight")) {
					playSound (6);
					alreadyDashed = true;
					playerDashTimer = 20;

				}

                GameObject.Find("PlayerSprite").GetComponent<Animator>().SetBool("dashParticle", true);
			}
            else
            {
                GameObject.Find("PlayerSprite").GetComponent<Animator>().SetBool("dashParticle", false);
            }
            //Instantiate(particleJuice, player.transform.position, Quaternion.identity);
            playerPosition = new Vector3(player.transform.position.x, player.transform.position.y, 0);
            //particleJuice.Emit(playerPosition,new Vector3(1,1,1),1,1,Color.red);
            //particleJuice.enableEmission = true;
            //particleJuice.gameObject.SetActive(true);
        }
        
		if (action == "shoot" && (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow))) {
			playSound (3);
            Instantiate(bulletLeft, player.transform.position, Quaternion.identity);
        }
		if (action == "shoot" && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightArrow))) {
			playSound (3);
            Instantiate(bulletRight, player.transform.position, Quaternion.identity);

					EmitParticles(dashParticles, new Vector3(player.position.x + .5f, player.position.y - 1.5f, player.position.z - 10), new Vector3(90, -90, 0), player.transform);
				}
			}
		}
		
		//Where shooting is handled
		if (action == "shoot" && (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow))) 
        {
			playSound (3);
			Instantiate(bulletLeft, new Vector2(player.position.x, player.position.y - .5f), Quaternion.identity);
			EmitParticles(shootParticles, new Vector3(player.position.x + 0.5f, player.position.y - 1.5f, player.position.z - 10), new Vector3(90, -90, 0), player.transform);
        }
		if (action == "shoot" && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightArrow))) {
			playSound (3);
			Instantiate(bulletRight, new Vector2(player.position.x + 4, player.position.y - .5f), new Quaternion(0, 180, 0, 0));
			EmitParticles(shootParticles, new Vector3(player.position.x + 4f, player.position.y - 1.5f, player.position.z - 10), new Vector3(90, 90, 0), player.transform);

        }
	}
	void playSound(int num) {
		gameObject.GetComponent<AudioSource>().clip = sounds[num];
		gameObject.GetComponent<AudioSource>().Play();
	}
	
	void doPlayerMovement() {
		if (Input.GetAxisRaw("Horizontal")<0) {
			playerVelocity.x -= playerFalling ? .03f : .12f;
			playerLastDirIsLeft = true;
		}
		if (Input.GetAxisRaw("Horizontal")>0) {
			playerVelocity.x += playerFalling ? .03f : .12f;
			playerLastDirIsLeft = false;
		}

		//The rest of the key stuff is up there in Update()
		
		playerVelocity.y-=.02f;
		if(playerDashTimer > 0) {
			playerDashTimer--;
			playerVelocity.x=.5f;
			playerVelocity.y=0;
			if(playerDashTimer == 0) playerVelocity.x /= 2;
		} else if(playerDashTimer < 0) {
			playerDashTimer++;
			playerVelocity.x=-.5f;
			playerVelocity.y=0;
			if(playerDashTimer == 0) playerVelocity.x /= 2;
		}
		if(playerDashTimer != 0 && (Input.GetAxisRaw("DashLeft")==0 && Input.GetAxisRaw("DashRight")==0)) {
			playerDashTimer = 0;
			playerVelocity.x /= 2;
		}
		
		for(float fallX = .1f; fallX<Mathf.Abs(playerVelocity.x); fallX+=.1f) {
			if(playerVelocity.x < 0 && collision(new Rect(playerPos.x-.3f, playerPos.y+.2f, 0, .6f), .1f)) {
				playerVelocity.x = 0;
				break;
			}
			if(playerVelocity.x > 0 && collision(new Rect(playerPos.x+.3f, playerPos.y+.2f, 0, .6f), .1f)) {
				playerVelocity.x = 0;
				break;
			}
			playerPos.x += .1f*(playerVelocity.x<0?-1:1);
		}

		bool wasFalling = playerFalling;
		playerFalling = true;
		if(playerVelocity.y < 0 && collision(new Rect(playerPos.x-.2f, playerPos.y, .4f, 0), .1f)) {
			playerJumpNum = 0;
			if(wasFalling) screenshakeWOOOOAH += (int) Mathf.Abs(playerLastFallSpeed*10);
			if(wasFalling) playSound(4);
			playerFalling = false;
			
			alreadyDashed = false;
		}
		float fallY;
		for(fallY = .1f; fallY<Mathf.Abs(playerVelocity.y); fallY+=.1f) {
			if(playerVelocity.y < 0 && collision(new Rect(playerPos.x-.2f, playerPos.y, .4f, 0), .1f)) {
				playerVelocity.y = 0;
				break;
			}
			if(playerVelocity.y > 0 && collision(new Rect(playerPos.x-.2f, playerPos.y+1.2f, .4f, 0), .1f)) {
				playerVelocity.y = 0;
				break;
			}
			playerPos.y += .1f*(playerVelocity.y<0?-1:1);
		}
		playerLastFallSpeed = fallY;
		
		playerVelocity.x *= playerFalling?.9f:.5f;
		playerVelocity.y *= .98f;
		
		float tmpy = playerPos.y;
		while(collision(new Rect(playerPos.x-.25f, tmpy+.05f, .5f, 0), .05f)) {
			tmpy+=.05f;
			if(tmpy-playerPos.y > .8f) {
				tmpy = playerPos.y;
				break;
			}
		}
		playerPos.y = tmpy;
		float tmpx = playerPos.x;
		while(collision(new Rect(tmpx-.3f, playerPos.y+.2f, 0, .4f), .3f)) {
			tmpx+=.1f;
			if(tmpx-playerPos.x > 1) {
				tmpy = playerPos.x;
				break;
			}
		}
		playerPos.x = tmpx;
		tmpx = playerPos.x;
		while(collision(new Rect(tmpx+.3f, playerPos.y+.2f, 0, .4f), .3f)) {
			tmpx-=.1f;
			if(tmpx-playerPos.x < -1) {
				tmpy = playerPos.x;
				break;
			}
		}
		playerPos.x = tmpx;

        collectableCollision(new Rect(playerPos.x - .2f, playerPos.y, .4f, 1.1f), .1f);

        if (spikeCollision(new Rect(playerPos.x - .2f, playerPos.y-.1f, .4f, 1.2f), .1f))
        {
            stats.SendMessage("registerDeath", playerPos);
            playerPos.x = spawnPoint.x;
            playerPos.y = spawnPoint.y;
            /*smoothCam.y = smoothCam2.y = cam.y = playerPos.y;
            smoothCam.x = smoothCam2.x = cam.x = playerPos.x+playerVelocity.x*10;*/
			screenshakeWOOOOAH += 20;
			playSound (1);
        }
		player.transform.position = new Vector3 (playerPos.x*4-2, playerPos.y*4+4, -1);

        if (spikeCollision(new Rect(playerPos.x - .2f, playerPos.y-.1f, .4f, 1.2f), .1f)) {
			EmitParticles(deathParticles, new Vector3(player.position.x+.5f, player.position.y-5f, player.position.z), Vector3.zero, null);
			died = true;
			player.Find ("PlayerSprite").GetComponent<SpriteRenderer>().enabled = false;
			stats.SendMessage("registerDeath", playerPos);
			screenshakeWOOOOAH += 20;
			playSound (1);
			Invoke ("resetAfterDeath", 2f);
        }
		updateSpritePosition ();

		
		if (playerVelocity.x < -.1f) {
			GameObject.Find ("PlayerSprite").GetComponent<Animator>().SetBool("WalkingLeft", true);
		} else {
			GameObject.Find ("PlayerSprite").GetComponent<Animator>().SetBool("WalkingLeft", false);
		}
		if (playerVelocity.x > .1f) {
			GameObject.Find ("PlayerSprite").GetComponent<Animator>().SetBool("WalkingRight", true);
		} else {
			GameObject.Find ("PlayerSprite").GetComponent<Animator>().SetBool("WalkingRight", false);
		}
		if (playerFalling) {
			GameObject.Find ("PlayerSprite").GetComponent<Animator>().SetBool("Falling", true);
		} else {
			GameObject.Find ("PlayerSprite").GetComponent<Animator>().SetBool("Falling", false);
		}
        //coinText.color = Color.white;
	}
    public bool collision(Rect rect, float precision) {
        for (float x = rect.x; x <= rect.x + rect.width; x += precision) {
            for (float y = rect.y; y <= rect.y + rect.height; y += precision) {
                switch (getTile((int)x, (int)Mathf.Ceil(y))) {
                    case "b":
					case "1":
                        return true;
                }
            }
        }
        return false;
	}

	public bool collectableCollision(Rect rect, float precision) {
		for (float x = rect.x; x <= rect.x + rect.width; x += precision) {
			for (float y = rect.y; y <= rect.y + rect.height; y += precision) {
				switch (getTile((int)x, (int)Mathf.Ceil(y))) {

	public void resetAfterDeath() {
		playerPos.x = spawnPoint.x;
		playerPos.y = spawnPoint.y;
		/*smoothCam.y = smoothCam2.y = cam.y = playerPos.y;
            smoothCam.x = smoothCam2.x = cam.x = playerPos.x+playerVelocity.x*10;*/
		died = false;
		player.Find ("PlayerSprite").GetComponent<SpriteRenderer>().enabled = true;
		updateSpritePosition ();
	}
	public bool collectableCollision(Rect rect, float precision) {
		for (float x = rect.x; x <= rect.x + rect.width; x += precision) {
			for (float y = rect.y; y <= rect.y + rect.height; y += precision) {
				string tile = getTile((int)x, (int)Mathf.Ceil(y));
				switch (tile) {

				case "c": //Whee we can add more!
					setTile((int)x, (int)Mathf.Ceil(y), "");
					//ADD COIN STUFF
					coinCount++;
					coinText.text = "Coins: " + coinCount.ToString();
					playSound (2);

					EmitParticles(coinParticles, new Vector3(x*4+2, y*4+2, -10), new Vector3(90, 0, 0), null);

					
					if (INSERTTHEJUICE) {
						coinText.color = Color.yellow;
						textColor = true;
						Invoke("colorTextChangerRESET", .3f);
					}
					break;

				case "e": //Whee we can add more!
					stats.SendMessage("registerWin", playerPos);
					Application.LoadLevel ("level"+(levelNum+1));

				case "e":
					stats.SendMessage("registerWin", playerPos);
					EmitParticles(winCelebrationParticles, player.position, Vector3.zero, null);
					won = true;
					Invoke("loadNextLevel", 2);
					break;
				case "r":
					for(int y22 = (int) spawnPoint.y+1; getTile ((int) spawnPoint.x, y22)=="R"; y22++) setTile ((int) spawnPoint.x, y22, "r");
					int y2;
					for(y2 = (int) y; getTile ((int) x, y2)=="r"; y2--) continue;
					for(int y22 = (int) y2+1; getTile ((int) x, y22)=="r"; y22++) setTile ((int) x, y22, "R");
					spawnPoint = new Vector2(x, y2);
					break;
				default: //Whee we can add more!
					if(tile.Length>1 && tile.Substring (0, 1) == "g") {
						signTextBox.GetComponent<textFade>().fadeIn (signs[int.Parse(tile.Substring (1))]);
					}

					break;
				}
			}
		}
		return false;
	}
    void colorTextChangerRESET() {
        if (textColor) {
            coinText.color = Color.white;
        }

        textColor = false;

    }

    public bool destructableCollision(Rect rect, float precision) {

	public bool destructableCollision(Rect rect, float precision) {
		bool found = false;

        for (float x = rect.x; x <= rect.x + rect.width; x += precision) {
            for (float y = rect.y; y <= rect.y + rect.height; y += precision) {
                switch (getTile((int)x, (int)Mathf.Ceil(y))) {
				case "b":
					setTile((int)x, (int)Mathf.Ceil(y), "");

					return true;
                }
            }
        }
        return false;

					found = true;
					break;
                }
            }
        }
		return found;

    }
	bool spikeCollision(Rect rect, float precision) {
		for(float x = rect.x; x<=rect.x+rect.width; x+=precision) {
			for(float y = rect.y; y<=rect.y+rect.height; y+=precision) {
				string tile = getTile((int) x, (int) Mathf.Ceil(y));
				if(tile.Length == 2 && tile.Substring(0, 1) == "s") {
					if(int.Parse(tile.Substring(1, 1)) == 0) {
						if(modulus(x, 1)!=0 && modulus(x, 1)<.3f && Mathf.Abs(modulus(y, 1)-.5f)<.4f) return true;
					} else if(int.Parse(tile.Substring(1, 1)) == 1) {
						if(modulus(y, 1)!=0 && modulus(y, 1)<.3f && Mathf.Abs(modulus(x, 1)-.5f)<.4f) return true;
					} else if(int.Parse(tile.Substring(1, 1)) == 2) {
						if(modulus(x, 1)!=0 && modulus(x, 1)>.7f && Mathf.Abs(modulus(y, 1)-.5f)<.4f) return true;
					} else if(int.Parse(tile.Substring(1, 1)) == 3) {
						if(modulus(y, 1)!=0 && modulus(y, 1)>.8f && Mathf.Abs(modulus(x, 1)-.5f)<.4f) return true;
					}

				} else if(tile == "a") {
					if(modulus(y, 1)>.2f && modulus(y, 1)<.8f && modulus(y, 1)>.2f && modulus(y, 1)<.8f) return true;

				}
			}
		}
		return false;
	}
	void updateCameraAndTiles() {
		//I reuse the camBounds width and height so the game doesn't break when you resize the camera.
		//If completely necessary, we could check if the size changes at any time. If so, delete all tiles and reinitialize them.
		Rect newCamBounds = new Rect( (int) (mainCamera.transform.position.x-mainCamera.orthographicSize*2*mainCamera.aspect/2)/4-2, (int)(mainCamera.transform.position.y - mainCamera.orthographicSize) / 4 - 2, (int) (mainCamera.orthographicSize*2*mainCamera.aspect/2)/2+5, (int) (mainCamera.orthographicSize)/2+5);

		//This code basically "rotates" the tiles on the screen, so not ALL tiles are loaded into memory at once, but only the ones in the camera view
		//Strips of blocks outside of the camera view are deleted or moved to the strips of blocks inside of the camera view. Tiles are reused when necessary.
		//This isn't a perfect solution, but it's so much better than having thousands of tiles always loaded.

		//This section rotates the tiles on the Y axis, if necessary
		if(!renderAllTiles) {
			if(camBounds.x>newCamBounds.x) {
				for (int y = (int) camBounds.y; y<(int) camBounds.y+camBounds.height; y++) {
					for (int x = (int) newCamBounds.x; x < (int) camBounds.x; x++) {
						drawTile(x, y);
					}
				}
			} else if(camBounds.x<newCamBounds.x) {
				for (int y = (int) camBounds.y; y<(int) camBounds.y+camBounds.height; y++) {
					for (int x = (int) camBounds.x; x < (int) newCamBounds.x; x++) {
						if (getTile (x, y) != "") { //If there's an actual tile at this position...
							DestroyImmediate(GameObject.Find("tile" + x + "," + y));
						}
					}
				}
			}
			if(camBounds.x+camBounds.width<newCamBounds.x+newCamBounds.width) {
				for (int y = (int) camBounds.y; y<(int) camBounds.y+camBounds.height; y++) {
					for (int x = (int) (camBounds.x+camBounds.width); x < (int) (newCamBounds.x+newCamBounds.width); x++) {
						drawTile(x, y);
					}
				}
			} else if(camBounds.x+camBounds.width>newCamBounds.x+newCamBounds.width) {
				for (int y = (int) camBounds.y; y<(int) camBounds.y+camBounds.height; y++) {
					for (int x = (int) (newCamBounds.x+newCamBounds.width); x < (int) (camBounds.x+camBounds.width); x++) {
						if (getTile (x, y) != "") { //If there's an actual tile at this position...
							DestroyImmediate(GameObject.Find("tile" + x + "," + y));
						}
					}
				}
			}
			
			if(camBounds.y>newCamBounds.y) {
				for (int x = (int) newCamBounds.x; x<(int) newCamBounds.x+newCamBounds.width; x++) {
					for (int y = (int) newCamBounds.y; y < (int) camBounds.y; y++) {
						drawTile(x, y);
					}
				}
			} else if(camBounds.y<newCamBounds.y) {
				for (int x = (int) newCamBounds.x; x<(int) newCamBounds.x+newCamBounds.width; x++) {
					for (int y = (int) camBounds.y; y < (int) newCamBounds.y; y++) {
						if (getTile (x, y) != "") { //If there's an actual tile at this position...
							DestroyImmediate(GameObject.Find("tile" + x + "," + y));
						}
					}
				}
			}
			if(camBounds.y+camBounds.height<newCamBounds.y+newCamBounds.height) {
				for (int x = (int) newCamBounds.x; x<(int) newCamBounds.x+newCamBounds.width; x++) {
					for (int y = (int) (camBounds.y+camBounds.height); y < (int) (newCamBounds.y+newCamBounds.height); y++) {
						drawTile(x, y);
					}
				}
			} else if(camBounds.y+camBounds.height>newCamBounds.y+newCamBounds.height) {
				for (int x = (int) newCamBounds.x; x<(int) camBounds.x+camBounds.width; x++) {
					for (int y = (int) (newCamBounds.y+newCamBounds.height); y < (int) (camBounds.y+camBounds.height); y++) {
						if (getTile (x, y) != "") { //If there's an actual tile at this position...
							DestroyImmediate(GameObject.Find("tile" + x + "," + y));
						}
					}
				}
			}
			
		}

		//The camera bounds is updated so in the next loop we can check the camera movement again
		camBounds = newCamBounds;
	}

	//Places the specified tile to the right coords, scale, and image
	void updateTile(int x, int y) {
		if (GameObject.Find ("tile" + x + "," + y) == null) return;

		GameObject.Find("tile"+x+","+y).transform.localScale = new Vector3(4.2f, 4.2f, 1);

		GameObject.Find ("tile"+x+","+y).GetComponent<Transform>().position = new Vector3(x*4, y*4, (modulus(x+y, 4)/4));
		if (getTile (x, y) == "1") {
			int sides = (getTile (x + 1, y) == "1" ? 1 : 0) + (getTile (x, y + 1) == "1" ? 2 : 0) + (getTile (x - 1, y) == "1" ? 4 : 0) + (getTile (x, y - 1) == "1" ? 8 : 0);
			GameObject.Find ("tile"+x+","+y).GetComponent<SpriteRenderer> ().sprite = tiles [(sides % 4) + (int)(sides / 4) * 8];
		} else if (getTile (x, y).Length == 2 && getTile (x, y).Substring (0, 1) == "s") {
			GameObject.Find ("tile"+x+","+y).GetComponent<SpriteRenderer> ().sprite = tiles [4 + int.Parse (getTile (x, y).Substring (1, 1))];
        } else if (getTile(x, y) == "b") {
            GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[1 * 8 + 4];
		} else if (getTile(x, y) == "e") {
			GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[1 * 8 + 5];

		GameObject.Find("tile"+x+","+y).GetComponent<Transform>().position = new Vector3(x*4, y*4, (modulus(x+y, 4)/4));
		if (getTile (x, y) == "1") {
			int sides = (getTile (x + 1, y) == "1" || getTile (x + 1, y) == "a" ? 1 : 0) + (getTile (x, y + 1) == "1" || getTile (x, y + 1) == "a" ? 2 : 0) + (getTile (x - 1, y) == "1" || getTile (x - 1, y) == "a" ? 4 : 0) + (getTile (x, y - 1) == "1" || getTile (x, y - 1) == "a" ? 8 : 0);
			GameObject.Find ("tile"+x+","+y).GetComponent<SpriteRenderer> ().sprite = tiles [(sides % 4) + (int)(sides / 4) * 8];
		} else if (getTile (x, y).Length == 2 && getTile (x, y).Substring (0, 1) == "s") {
			GameObject.Find ("tile"+x+","+y).GetComponent<SpriteRenderer> ().sprite = tiles [4 + int.Parse (getTile (x, y).Substring (1, 1))];
        } else if (getTile(x, y) == "b") { //Barrier
            GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[1 * 8 + 4];
		} else if (getTile(x, y) == "e") { //End
			GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[1 * 8 + 5];
		} else if (getTile(x, y).Substring (0, 1) == "g") { //Sign
			GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[3 * 8 + 7];
		} else if (getTile(x, y) == "r") { //Respawn
			if(getTile (x, y - 1) == "1") {
				GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[1 * 8 + 6];
			} else if(getTile (x, y + 1) == "1") {
				GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[1 * 8 + 7];
			} else {
				GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[2 * 8 + 7];
			}
		} else if (getTile(x, y) == "R") { //Respawn selected :)
			if(getTile (x, y - 1) == "1") {
				GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[4 * 8 + 6];
			} else if(getTile (x, y + 1) == "1") {
				GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[4 * 8 + 7];
			} else {
				GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[5 * 8 + 7];
			}
		} else if (getTile(x, y) == "a") { //Lava
			if(getTile (x, y + 1) != "a" && getTile (x, y + 1) != "1") {
				if(x%2 == 0) GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[3 * 8 + 4];
				else GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[3 * 8 + 5];
			} else {
				GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[3 * 8 + 6];
			}

		}
	}

	void loadLevel(Texture2D image) {
		//Load the level from the image because we're cool
		scene = new string[image.width, image.height];

		int lvlHeight = image.height; //Cache height of level
		for(int x = 0; x<image.width; x++) {
			for(int y = 0; y<lvlHeight; y++) {
				scene[x, y] = ""; //Default to air

				switch((int)(image.GetPixel(x, y).r*255*256*256+image.GetPixel(x, y).g*255*256+image.GetPixel(x, y).b*255)) { //Read the pixel of the level
				case (0x0): //Black
					scene[x, y] = "1"; //Wall type 1
					break;
				case 0xFF00: //Green
					spawnPoint = new Vector2(x, y);
					break;
                case 0xFFFF00: //Coin
                    scene[x, y] = "c";

				int pix = (int)(image.GetPixel(x, y).r*255*256*256+image.GetPixel(x, y).g*255*256+image.GetPixel(x, y).b*255);
				switch(pix) { //Read the pixel of the level
				case (0x0): //Black
					scene[x, y] = "1"; //Wall type 1
					break;
				case 0xFF00:
					spawnPoint = new Vector2(x+.5f, y); //Spawn point
					break;
                case 0xFFFF00:
					scene[x, y] = "c"; //Coin

                    break;
                case 0xFF0000: //Spike facing right
                    scene[x, y] = "s0";
                    break;
                case 0xFF0001: //Spike facing up
					scene[x, y] = "s1";
					break;
				case 0xFF0002: //Spike facing left
					scene[x, y] = "s2";
					break;
				case 0xFF0003: //Spike facing down
					scene[x, y] = "s3";
					break;

				case 0x777777: //Barrier
					scene[x, y] = "b";
					break;
				case 0x0000FF: //Barrier
					scene[x, y] = "e";
					break;
				case 0xFF7700: //Orange
					//Add enemy

				case 0x777777:
					scene[x, y] = "b"; //Barrier
					break;
				case 0x0000FF:
					scene[x, y] = "e"; //End
					break;
				case 0xFF00FF:
					scene[x, y] = "r"; //Respawn
					break;
				case 0xFF7777:
					scene[x, y] = "a"; //Lava I guess
					break;
				default: //Other things
					if(pix>>8 == 0xFF77) {
						scene[x, y] = "g"+(pix-Mathf.Floor((pix>>8)*0x100)); //Sign
					}

					break;
				}
			}
		}
	}

	string getTile(int x, int y) {
		if(x<=0 || x>=scene.GetLength(0) || y<=0 || y>=scene.GetLength(1)) return "1"; //Render blocks outside of world range
		else return scene[x, y];
	}
	
	public void setTile(int x, int y, string type) {
		if(x<=0 || x>=scene.GetLength(0) || y<=0 || y>=scene.GetLength(1)) return; //Render blocks outside of world range

		if (getTile (x, y) == type)
			return;

		scene[x, y] = type;

		if (x >= camBounds.x && x <= camBounds.x+camBounds.width && y >= camBounds.y && y <= camBounds.y+camBounds.height) {
			drawTile (x, y);
		}

		updateTile (x - 1, y);
		updateTile (x + 1, y);
		updateTile (x, y - 1);
		updateTile (x, y + 1);
	}
	public void drawTile(int x, int y) {
		DestroyImmediate(GameObject.Find("tile"+x+","+y));

		if(getTile(x, y) != "") {
			Object tmp;
			if(getTile(x, y) == "c") tmp = Instantiate(coinPrefab, new Vector3(x*4, y*4, (modulus(x+y, 4))/4), Quaternion.identity);
			else tmp = Instantiate(tile, new Vector3(x*4, y*4, (modulus(x+y, 4))/4), Quaternion.identity);
			tmp.name = "tile"+x+","+y;
			updateTile(x, y);
		}
	}
	float modulus(float a, float b) {
		float c = a % b;
		return (c < 0) ? c + b : c;
	}

    void EmitParticles(ParticleSystem pS, Vector3 position, Vector3 angle, Transform particleParent) {
		ParticleSystem particle = Instantiate(pS, new Vector3(position.x, position.y, position.z-10), Quaternion.identity) as ParticleSystem;
		if(particleParent != null) particle.transform.parent = particleParent;
		particle.transform.rotation = Quaternion.Euler(new Vector3(angle.x-90, angle.y, angle.z));
		Destroy(particle.gameObject, particle.startLifetime);
    }
    
    void loadNextLevel() {
        Application.LoadLevel("level" + (levelNum + 1));
    }

}
