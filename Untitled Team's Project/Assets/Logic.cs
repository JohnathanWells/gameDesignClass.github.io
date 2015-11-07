using UnityEngine;
using System.Collections;

public class Logic : MonoBehaviour {

	public Transform tile;
	public Sprite[] tiles;

	public Texture2D level;
	
	private string[,] scene;
	private Vector2 spawnPoint;

	public Camera mainCamera;
	private Vector2 cam;
	private Vector2 smoothCam;
	private Vector2 smoothCam2;
	private int screenshakeWOOOOAH;
	private Rect camBounds;

	public Transform player;
	private Vector2 playerPos;
	private Vector2 playerVelocity;
	private bool playerLastDirIsLeft = false;
	private bool playerFalling = true;
	private float playerLastFallSpeed = 0;
	private int playerJumpNum = 0;
	private int playerDashTimer = 0;

	private bool deletingTiles = false;

	private string action = "double";

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 10; //One problem with Flash is that 50 fps is super stable, while 60fps is pretty weird. In order to preserve the physics tick, I'm limiting the fps. Not a big deal?

		//This line calculates which blocks are going to be on the screen based on the camera position. It is also in the updateCameraAndTiles section
		camBounds = new Rect( (int) (mainCamera.transform.position.x-mainCamera.orthographicSize*2*mainCamera.aspect/2)/4-2,  (int)(mainCamera.transform.position.y - mainCamera.orthographicSize) / 4 - 2, (int) (mainCamera.orthographicSize*2*mainCamera.aspect/2)/2+5, (int) (mainCamera.orthographicSize)/2+5);

		loadLevel (level);//Load the level image to a multidimensional array

		//These loops set the initial blocks in the camera view. Do not change the camera size during runtime - it will cause instabilities
		for(int y = (int) camBounds.y; y<(int) camBounds.y+camBounds.height; y++) {
			for(int x = (int) camBounds.x; x<(int) camBounds.x+camBounds.width; x++) {
				if(getTile(x, y) != "") {
					Object tmp = Instantiate(tile, new Vector3(x*4, y*4, (modulus(x+y, 4))/4), Quaternion.identity);
					tmp.name = "tile"+x+","+y;
					updateTile(x, y);
				}
			}
		}
		playerPos = new Vector2 (spawnPoint.x, spawnPoint.y);
		player.transform.position = new Vector3 (playerPos.x*4-2, playerPos.y*4+4, -1);
	}
	void FixedUpdate () {
		//mainCamera.transform.position += new Vector3 (0, -.05f, 0);
		updateCameraAndTiles ();

		cam.y += (playerPos.y-cam.y)/10;
		cam.x += (playerPos.x+(playerLastDirIsLeft?-1:1)-cam.x)/10;
		
		screenshakeWOOOOAH = Mathf.Max(screenshakeWOOOOAH-1, 0);
		smoothCam2.x += (cam.x-smoothCam.x)/10;
		smoothCam2.y += ((cam.y+camBounds.height/6)-smoothCam.y)/10;
		
		smoothCam.x = smoothCam2.x+(Random.Range(0.0f, 1.0f)-.5f)*Mathf.Max(0, Mathf.Log(screenshakeWOOOOAH*100)/25);
		smoothCam.y = smoothCam2.y+(Random.Range(0.0f, 1.0f)-.5f)*+Mathf.Max(0, Mathf.Log(screenshakeWOOOOAH*100)/25);

		mainCamera.transform.position = new Vector3 (cam.x*4, cam.y*4, -10);

		doPlayerMovement ();
	}
	
	void Update() {

		Vector2 mP = new Vector2((int)(mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)).x+2)/4, (int)(mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)).y+2)/4);
		
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
		}

		//Some key stuff happens here, because Unity
		if(Input.GetButtonDown ("Jump") && playerJumpNum<(action == "double"?2:1)) {
			playerJumpNum++;
			playerVelocity.y=.5f;
		}
		if(action != "dash") {
			if(Input.GetButtonDown("DashLeft")) playerDashTimer=-20;
			if(Input.GetButtonDown ("DashRight")) playerDashTimer=20;
		}
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
			playerFalling = false;
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
		
		if(spikeCollision(new Rect(playerPos.x-.2f, playerPos.y, .4f, 1.1f), .1f)) {
			playerPos.x = spawnPoint.x;
			playerPos.y = spawnPoint.y;
			/*smoothCam.y = smoothCam2.y = cam.y = playerPos.y;
			smoothCam.x = smoothCam2.x = cam.x = playerPos.x+playerVelocity.x*10;*/ //I need a rerender function for respawning lol
			screenshakeWOOOOAH += 20;
		}

		player.transform.position = new Vector3 (playerPos.x*4-2, playerPos.y*4+4, -1);
	}
	bool collision(Rect rect, float precision) {
		for(float x = rect.x; x<=rect.x+rect.width; x+=precision) {
			for(float y = rect.y; y<=rect.y+rect.height; y+=precision) {
				switch(getTile((int) x, (int) Mathf.Ceil(y))) {
				case "1": //Whee we can add more!
					return true;
				}
			}
		}
		return false;
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
				}
			}
		}
		return false;
	}
	void updateCameraAndTiles() {
		//I reuse the camBounds width and height so the game doesn't break when you resize the camera.
		//If completely necessary, we could check if the size changes at any time. If so, delete all tiles and reinitialize them.
		Rect newCamBounds = new Rect( (int) (mainCamera.transform.position.x-mainCamera.orthographicSize*2*mainCamera.aspect/2)/4-2, (int)(mainCamera.transform.position.y - mainCamera.orthographicSize) / 4 - 2, camBounds.width, camBounds.height);

		//This code basically "rotates" the tiles on the screen, so not ALL tiles are loaded into memory at once, but only the ones in the camera view
		//Strips of blocks outside of the camera view are deleted or moved to the strips of blocks inside of the camera view. Tiles are reused when necessary.
		//This isn't a perfect solution, but it's so much better than having thousands of tiles always loaded.

		//This section rotates the tiles on the Y axis, if necessary
		int newBoundsY1 = 1;
		int newBoundsY2 = 0;
		bool down = false;
		if (newCamBounds.y > camBounds.y) {
			newBoundsY1 = (int) (camBounds.y+newCamBounds.height);
			newBoundsY2 = (int) (newCamBounds.y+newCamBounds.height);
			down = true;
		} else if (newCamBounds.y < camBounds.y) {
			newBoundsY1 = (int) newCamBounds.y;
			newBoundsY2 = (int) camBounds.y;
		}
		for(int y = newBoundsY1; y<newBoundsY2; y++) {
			for(int x = (int) camBounds.x; x < (int) camBounds.x+camBounds.width; x++) {
				if(getTile(x, y) != "") { //If there's an actual tile at this position...
					Object tmp;
					if(getTile(x, (int) (y+(down?-1:1)*camBounds.height)) == "") {
						tmp = Instantiate(tile, new Vector2(x*4, y*4), Quaternion.identity);
						tmp.name = "tile"+x+","+y;
					} else {
						tmp = GameObject.Find("tile"+x+","+(y+(down?-1:1)*camBounds.height)).GetComponent<Transform>();
						tmp.name = "tile"+x+","+y;
					}
					updateTile(x, y);
				} else { //If no tile exists, delete the corresponding tile on the end of the screen
					if(GameObject.Find("tile"+x+","+(y+(down?-1:1)*newCamBounds.height))!=null) Destroy(GameObject.Find("tile"+x+","+(y+(down?-1:1)*newCamBounds.height)));
				}
			}
		}
		
		//This section rotates the tiles on the X axis, if necessary
		//It uses the already rotated Y camera tiles in the calculation.
		int newBoundsX1 = 1;
		int newBoundsX2 = 0;
		bool left = false;
		if (newCamBounds.x > camBounds.x) {
			newBoundsX1 = (int) (camBounds.x+newCamBounds.width);
			newBoundsX2 = (int) (newCamBounds.x+newCamBounds.width);
			left = true;
		} else if (newCamBounds.x < camBounds.x) {
			newBoundsX1 = (int) newCamBounds.x;
			newBoundsX2 = (int) camBounds.x;
		}
		for(int y = (int) newCamBounds.y; y< (int) newCamBounds.y+newCamBounds.height; y++) {
			for(int x = newBoundsX1; x < newBoundsX2; x++) {
				if(getTile(x, y) != "") {
					Object tmp;
					if(getTile((int) (x+(left?-1:1)*camBounds.width), y) == "") {
						tmp = Instantiate(tile, new Vector3(x*4, y*4, (modulus(x+y, 4))/4), Quaternion.identity);
						tmp.name = "tile"+x+","+y;
					} else {
						tmp = GameObject.Find("tile"+((int) (x+(left?-1:1)*camBounds.width))+","+y).GetComponent<Transform>();
						tmp.name = "tile"+x+","+y;
					}
					updateTile(x, y);
				} else {
					if(GameObject.Find("tile"+((int) (x+(left?-1:1)*camBounds.width))+","+y)!=null) Destroy(GameObject.Find("tile"+((int) (x+(left?-1:1)*camBounds.width))+","+y));
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
		GameObject.Find ("tile"+x+","+y).GetComponent<Transform>().position = new Vector3(x*4, y*4, (modulus(x+y, 16)));
		if (getTile (x, y) == "1") {
			int sides = (getTile (x + 1, y) == "1" ? 1 : 0) + (getTile (x, y + 1) == "1" ? 2 : 0) + (getTile (x - 1, y) == "1" ? 4 : 0) + (getTile (x, y - 1) == "1" ? 8 : 0);
			GameObject.Find ("tile"+x+","+y).GetComponent<SpriteRenderer> ().sprite = tiles [(sides % 4) + (int)(sides / 4) * 8];
		} else if (getTile (x, y).Length == 2 && getTile (x, y).Substring (0, 1) == "s") {
			GameObject.Find ("tile"+x+","+y).GetComponent<SpriteRenderer> ().sprite = tiles [4 + int.Parse (getTile (x, y).Substring (1, 1))];
		} else if (getTile (x, y) == "b") {
			GameObject.Find ("tile"+x+","+y).GetComponent<SpriteRenderer> ().sprite = tiles [1 * 8 + 4];
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
				case 0xFF7700: //Orange
					//Add enemy
					break;
				}
			}
		}
	}

	string getTile(int x, int y) {
		if(x<=0 || x>=scene.GetLength(0) || y<=0 || y>=scene.GetLength(1)) return "1"; //Render blocks outside of world range
		else return scene[x, y];
	}

	void setTile(int x, int y, string type) {
		if(x<=0 || x>=scene.GetLength(0) || y<=0 || y>=scene.GetLength(1)) return; //Render blocks outside of world range
		else scene[x, y] = type;
		if(getTile(x, y) != "") {
			Object tmp;

			if(GameObject.Find("tile"+x+","+y)==null) {
				tmp = Instantiate(tile, new Vector3(x*4, y*4, (modulus(x+y, 4))/4), Quaternion.identity);
				tmp.name = "tile"+x+","+y;
			}
			updateTile(x, y);
		} else {
			if(GameObject.Find("tile"+x+","+y)!=null) Destroy(GameObject.Find("tile"+x+","+y));
		}
		updateTile (x - 1, y);
		updateTile (x + 1, y);
		updateTile (x, y - 1);
		updateTile (x, y + 1);
	}
	float modulus(float a, float b) {
		float c = a % b;
		return (c < 0) ? c + b : c;
	}
}
