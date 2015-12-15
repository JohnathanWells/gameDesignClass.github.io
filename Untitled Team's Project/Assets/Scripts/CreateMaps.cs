using UnityEngine;
using System.Collections;
using System.IO;


public class CreateMaps : MonoBehaviour
{
    // UI from Unity
    public UnityEngine.UI.Text CurrentText;

    // objects from unity
    public Transform tile;
    public Sprite[] tiles;
    public bool isTemp = false;
    public static string slothFile = "customMap.sloth";
    public StreamReader slothLoad = new StreamReader(slothFile);
    public bool loadFromSloth = false;
    public Texture2D imageLevel;
    public bool loadFromImage = false;
    public Transform coinPrefab;
    

    // Change from render all at once or load everything
    public bool renderAllTiles = false;

    // string array with game world
    private string[,] scene;

    // main camera declairations
    public Camera mainCamera;
    public float margin = 0.9f; // define how close the curser can be to move the camera
    public float scrollSpeed = 1; // scrooling speed
    private Rect camBounds;
    private Vector3 lastPointPos;
    

    // which block mode are we on
    private int blockMode = 0; // zero is nothing selected and may just become map drag and zoom
    private int spikeMode = 0; // for rotating the spike

    

    // Use this for initialization
    void Start()
    {
        // finds if need to load from an image file
        if (loadFromSloth)
        {
            if (isTemp)
                loadLevel(new StreamReader("temp.sloth"));
            else
                loadLevel(slothLoad);
        }
        else if (loadFromImage)
            loadLevel(imageLevel);
        else
            loadLevel();

        //This line calculates which blocks are going to be on the screen based on the camera position. It is also in the updateCameraAndTiles section
        camBounds = new Rect((int)(mainCamera.transform.position.x - mainCamera.orthographicSize * 2 * mainCamera.aspect / 2) / 4 - 2, (int)(mainCamera.transform.position.y - mainCamera.orthographicSize) / 4 - 2, (int)(mainCamera.orthographicSize * 2 * mainCamera.aspect / 2) / 2 + 5, (int)(mainCamera.orthographicSize) / 2 + 5);

        if (renderAllTiles)
        {
            for (int y = -1; y < scene.GetLength(1) + 1; y++)
            {
                for (int x = -1; x < scene.GetLength(0) + 1; x++)
                {
                    if (getTile(x, y) != "")
                    {
                        drawTile(x, y);
                    }
                }
            }
        }
        else
        {
            //These loops set the initial blocks in the camera view. Do not change the camera size during runtime - it will cause instabilities
            for (int y = (int)camBounds.y; y < (int)camBounds.y + camBounds.height; y++)
            {
                for (int x = (int)camBounds.x; x < (int)camBounds.x + camBounds.width; x++)
                {
                    if (getTile(x, y) != "")
                    {
                        drawTile(x, y);
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        //mainCamera.transform.position += new Vector3 (0, -.05f, 0);
        updateCameraAndTiles();

        //Scrolling
        #region 
            // scroll right
            if (Input.mousePosition.x >= Screen.width*margin && !(blockMode == 0 && Input.GetMouseButton(0)) || Input.GetKey("right"))
                mainCamera.transform.position += new Vector3(scrollSpeed, 0f, 0f);
            // scroll left
            if (Input.mousePosition.x <= Screen.width * (1 - margin) && !(blockMode == 0 && Input.GetMouseButton(0)) || Input.GetKey("left"))
                mainCamera.transform.position -= new Vector3(scrollSpeed , 0f, 0f);
            // scroll up
            if (Input.mousePosition.y >= Screen.height * margin && !(blockMode == 0 && Input.GetMouseButton(0)) || Input.GetKey("up"))
                mainCamera.transform.position += new Vector3(0f, scrollSpeed, 0f);
            // scroll down
            if (Input.mousePosition.y <= Screen.height * (1 - margin) && !(blockMode == 0 && Input.GetMouseButton(0)) || Input.GetKey("down"))
                mainCamera.transform.position -= new Vector3(0f, scrollSpeed, 0f);
        #endregion

    }

    void Update()
    {
        // finds where the mouse pointer is reletive to the world
        Vector2 mP = new Vector2((int)System.Math.Floor((mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)).x)/4), (int)System.Math.Ceiling((mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)).y)/4));

        Debug.Log("Mouse x: " + Input.mousePosition.x + " y: " + Input.mousePosition.y);
        Debug.Log("Screen to world point x: " + mainCamera.ScreenToWorldPoint(Input.mousePosition).x + " y: " + mainCamera.ScreenToWorldPoint(Input.mousePosition).y);
        Debug.Log("Screen to world point x: " + (int)System.Math.Floor(mainCamera.ScreenToWorldPoint(Input.mousePosition).x/4) + " y: " + (int)System.Math.Ceiling(mainCamera.ScreenToWorldPoint(Input.mousePosition).y/4));

        // Print key
        if (Input.GetKeyDown("p"))
        {
            printLevel();
        }
        // Set current object based on input
        if (Input.GetKeyDown("space"))
        {
            modeCamera();
        }
        if (Input.GetKeyDown("a"))
        {
            modeBlock();
        }
        if (Input.GetKeyDown("s"))
        {
            modeSpike(); // add spike mode
        }
        if (Input.GetKeyDown("e"))
        {
            modeErase(); // erase mode
        }
        if (Input.GetKeyDown("l"))
        {
            loadCurrent();
        }



        //Check Scroll wheel
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f) // scrooling down
        {
            // if in spike mode, rotate spike
            if (blockMode == 2)
            {
                if (spikeMode < 1) // if at first spike, wrap back around to the last
                    spikeMode = 3;
                else
                    spikeMode--;
            }
            else
            {
                mainCamera.orthographicSize = mainCamera.orthographicSize + 1; // increase the camera size

            }
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f) // scrolling up
        {
            // If in spike mode, rotate spike
            if (blockMode == 2)
            {
                if (spikeMode > 2) // if at last spike, wrap back around to the first
                    spikeMode = 0;
                else
                    spikeMode++;
            }
            else
            {
                mainCamera.orthographicSize = mainCamera.orthographicSize - 1; // decrease the camera size
            }
        } 
        
        // place object if mouse click
        if (Input.GetMouseButton(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            switch (blockMode)
            {
                case 0:
                    if (Input.mousePosition != lastPointPos)
                    {
                        // the math for this scales with the zoom so that its is (almost) a one to one movement
                        mainCamera.transform.position += new Vector3(-1*(Input.mousePosition.x - lastPointPos.x)/12f*mainCamera.orthographicSize/30, -1*(Input.mousePosition.y - lastPointPos.y)/12f * mainCamera.orthographicSize / 30, 0f); 
                    }
                    break;
                case 1:
                    setTile((int)mP.x, (int)mP.y, "1"); // place land if in land mode
                    break;
                case 2:
                    switch (spikeMode) // when in spike mode look for which spick to place
                    {
                        case 0:
                            setTile((int)mP.x, (int)mP.y, "s0"); 
                            break;
                        case 1:
                            setTile((int)mP.x, (int)mP.y, "s1"); 
                            break;
                        case 2:
                            setTile((int)mP.x, (int)mP.y, "s2");
                            break;
                        case 3:
                            setTile((int)mP.x, (int)mP.y, "s3");
                            break;
                    }
                    break;
                case 3:
                    setTile((int)mP.x, (int)mP.y, ""); // erase the block
                    break;
            }
        }
        if (Input.GetMouseButton(1))
        {
            setTile((int)mP.x, (int)mP.y, ""); // erase the block
        }

        lastPointPos = Input.mousePosition;
    }

  

    void updateCameraAndTiles()
    {
        //I reuse the camBounds width and height so the game doesn't break when you resize the camera.
        //If completely necessary, we could check if the size changes at any time. If so, delete all tiles and reinitialize them.
        Rect newCamBounds = new Rect((int)(mainCamera.transform.position.x - mainCamera.orthographicSize * 2 * mainCamera.aspect / 2) / 4 - 2, (int)(mainCamera.transform.position.y - mainCamera.orthographicSize) / 4 - 2, (int)(mainCamera.orthographicSize * 2 * mainCamera.aspect / 2) / 2 + 5, (int)(mainCamera.orthographicSize) / 2 + 5);

        //This code basically "rotates" the tiles on the screen, so not ALL tiles are loaded into memory at once, but only the ones in the camera view
        //Strips of blocks outside of the camera view are deleted or moved to the strips of blocks inside of the camera view. Tiles are reused when necessary.
        //This isn't a perfect solution, but it's so much better than having thousands of tiles always loaded.

        //This section rotates the tiles on the Y axis, if necessary
        if (!renderAllTiles)
        {
            if (camBounds.x > newCamBounds.x)
            {
                for (int y = (int)camBounds.y; y < (int)camBounds.y + camBounds.height; y++)
                {
                    for (int x = (int)newCamBounds.x; x < (int)camBounds.x; x++)
                    {
                        drawTile(x, y);
                    }
                }
            }
            else if (camBounds.x < newCamBounds.x)
            {
                for (int y = (int)camBounds.y; y < (int)camBounds.y + camBounds.height; y++)
                {
                    for (int x = (int)camBounds.x; x < (int)newCamBounds.x; x++)
                    {
                        if (getTile(x, y) != "")
                        { //If there's an actual tile at this position...
                            DestroyImmediate(GameObject.Find("tile" + x + "," + y));
                        }
                    }
                }
            }
            if (camBounds.x + camBounds.width < newCamBounds.x + newCamBounds.width)
            {
                for (int y = (int)camBounds.y; y < (int)camBounds.y + camBounds.height; y++)
                {
                    for (int x = (int)(camBounds.x + camBounds.width); x < (int)(newCamBounds.x + newCamBounds.width); x++)
                    {
                        drawTile(x, y);
                    }
                }
            }
            else if (camBounds.x + camBounds.width > newCamBounds.x + newCamBounds.width)
            {
                for (int y = (int)camBounds.y; y < (int)camBounds.y + camBounds.height; y++)
                {
                    for (int x = (int)(newCamBounds.x + newCamBounds.width); x < (int)(camBounds.x + camBounds.width); x++)
                    {
                        if (getTile(x, y) != "")
                        { //If there's an actual tile at this position...
                            DestroyImmediate(GameObject.Find("tile" + x + "," + y));
                        }
                    }
                }
            }

            if (camBounds.y > newCamBounds.y)
            {
                for (int x = (int)newCamBounds.x; x < (int)newCamBounds.x + newCamBounds.width; x++)
                {
                    for (int y = (int)newCamBounds.y; y < (int)camBounds.y; y++)
                    {
                        drawTile(x, y);
                    }
                }
            }
            else if (camBounds.y < newCamBounds.y)
            {
                for (int x = (int)newCamBounds.x; x < (int)newCamBounds.x + newCamBounds.width; x++)
                {
                    for (int y = (int)camBounds.y; y < (int)newCamBounds.y; y++)
                    {
                        if (getTile(x, y) != "")
                        { //If there's an actual tile at this position...
                            DestroyImmediate(GameObject.Find("tile" + x + "," + y));
                        }
                    }
                }
            }
            if (camBounds.y + camBounds.height < newCamBounds.y + newCamBounds.height)
            {
                for (int x = (int)newCamBounds.x; x < (int)newCamBounds.x + newCamBounds.width; x++)
                {
                    for (int y = (int)(camBounds.y + camBounds.height); y < (int)(newCamBounds.y + newCamBounds.height); y++)
                    {
                        drawTile(x, y);
                    }
                }
            }
            else if (camBounds.y + camBounds.height > newCamBounds.y + newCamBounds.height)
            {
                for (int x = (int)newCamBounds.x; x < (int)camBounds.x + camBounds.width; x++)
                {
                    for (int y = (int)(newCamBounds.y + newCamBounds.height); y < (int)(camBounds.y + camBounds.height); y++)
                    {
                        if (getTile(x, y) != "")
                        { //If there's an actual tile at this position...
                            DestroyImmediate(GameObject.Find("tile" + x + "," + y));
                        }
                    }
                }
            }

        }

        //The camera bounds is updated so in the next loop we can check the camera movement again
        camBounds = newCamBounds;
    }

    public void drawTile(int x, int y)
    {
        DestroyImmediate(GameObject.Find("tile" + x + "," + y));

        if (getTile(x, y) != "")
        {
            Object tmp;
            if (getTile(x, y) == "c") tmp = Instantiate(coinPrefab, new Vector3(x * 4, y * 4, (modulus(x + y, 4)) / 4), Quaternion.identity);
            else tmp = Instantiate(tile, new Vector3(x * 4, y * 4, (modulus(x + y, 4)) / 4), Quaternion.identity);
            tmp.name = "tile" + x + "," + y;
            updateTile(x, y);
        }
    }

    //Places the specified tile to the right coords, scale, and image
    void updateTile(int x, int y)
    {
        if (GameObject.Find("tile" + x + "," + y) == null) return;

        GameObject.Find("tile" + x + "," + y).transform.localScale = new Vector3(4.2f, 4.2f, 1);
        GameObject.Find("tile" + x + "," + y).GetComponent<Transform>().position = new Vector3(x * 4, y * 4, (modulus(x + y, 16)));
        if (getTile(x, y) == "1")
        {
            int sides = (getTile(x + 1, y) == "1" ? 1 : 0) + (getTile(x, y + 1) == "1" ? 2 : 0) + (getTile(x - 1, y) == "1" ? 4 : 0) + (getTile(x, y - 1) == "1" ? 8 : 0);
            GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[(sides % 4) + (int)(sides / 4) * 8];
        }
        else if (getTile(x, y).Length == 2 && getTile(x, y).Substring(0, 1) == "s")
        {
            GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[4 + int.Parse(getTile(x, y).Substring(1, 1))];
        }
        else if (getTile(x, y) == "b")
        {
            GameObject.Find("tile" + x + "," + y).GetComponent<SpriteRenderer>().sprite = tiles[1 * 8 + 4];
        }
    }

    // load level when image is not given
    void loadLevel()
    {
        // default scene size is 200 by 200
        scene = new string[200, 200];
        for (int x = 0; x < 200; x++)
        {
            for (int y = 0; y < 200; y++)
            {
                scene[x, y] = ""; //Default every tile to air
            }
        }
    }

    // load level when image is given
    void loadLevel(Texture2D image)
    {
        //Load the level from the image because we're cool
        scene = new string[image.width, image.height];

        int lvlHeight = image.height; //Cache height of level
        for (int x = 0; x < image.width; x++)
        {
            for (int y = 0; y < lvlHeight; y++)
            {
                scene[x, y] = ""; //Default to air
                switch ((int)(image.GetPixel(x, y).r * 255 * 256 * 256 + image.GetPixel(x, y).g * 255 * 256 + image.GetPixel(x, y).b * 255))
                { //Read the pixel of the level
                    case (0x0): //Black
                        scene[x, y] = "1"; //Wall type 1
                        break;
                    case 0xFF00: //Green
                        //spawnPoint = new Vector2(x, y);
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

    void loadLevel(StreamReader readFile)
    {
        // basic declarations
        string text = "", lineText = "";
        int i = 0, x = 0, y = 0;
        int width, height;

        // go through the file until scene sceneStart is found
        while (!text.Contains("sceneStart:") && !readFile.EndOfStream)
        {
            text = text + (char)readFile.Read();
            i++;
        }
        if (readFile.EndOfStream) // break if its at the end of the stream
        {
            return; // should be error eventually
        }

        //find width
        text = ""; // reset
        i = 0;
        while (!text.Contains(",") && !readFile.EndOfStream) // finds the ","
        {
            text = text + (char)readFile.Read();
            i++;
        }
        if (readFile.EndOfStream) // break if its at the end of the stream
        {
            return; // should be error eventually
        }
        text = text.Substring(0, text.Length - 1); // cut out comma
        if (!int.TryParse(text, out width))
        {
            return; // should be error eventually
        }

        //find Height
        text = ""; // reset
        while (!text.Contains("\n") && !readFile.EndOfStream) // finds the next line char
        {
            text = text + (char)readFile.Read();
            i++;
        }
        if (readFile.EndOfStream) // break if its at the end of the stream
        {
            return; // should be error eventually
        }
        text = text.Substring(0, text.Length - 1); // cut out next-line char
        if (!int.TryParse(text, out height))
        {
            return; // should be error eventually
        }

        // create map
        text = ""; // reset text string
        scene = new string[width, height];
        i = 0;
        lineText = readFile.ReadLine(); // Get the first line
        while (!lineText.Contains("endScene:") && y < height)  // loop trhough unless endScene is read,
        { 
            //loop though the string poping off each character
            while (lineText.Length > 0 && x < width)
            {
                text = text + lineText.Substring(0, 1); // push the char
                lineText = lineText.Substring(1, lineText.Length - 1);
                if (text.Contains("."))
                {
                    text = text.Replace(".", "");
                    scene[x, y] = text;
                    text = "";
                    x++;
                }

            }
            x = 0;
            y++;
            lineText = readFile.ReadLine(); // get the next line
        }

        readFile.Close();
        Debug.Log(width.ToString() + "\n" + height.ToString());
    }

    // prints the level in a text format
    public void printLevel()
    {
        // set up file name
        string fileName = "userCreated_" + System.DateTime.Now.Year + System.DateTime.Now.Month + System.DateTime.Now.Day + "_" + System.DateTime.Now.ToString("HHmmss") + ".sloth";
        // check for file
        if (File.Exists(fileName))
        {
            return; // some error should eventually be displayed
        }
        using (StreamWriter outFile = new StreamWriter(fileName)) // create file
        {
            // Write Header Saying that the next lines contains the array, also shows the array size
            outFile.Write("sceneStart:" + scene.GetLength(0) + "," + scene.GetLength(1) + "\n");

            // print the array
            for (int y = 0; y < scene.GetLength(1); y++)
            {
                for (int x = 0; x < scene.GetLength(0); x++)
                {
                    outFile.Write(scene[x, y] + ".");
                    outFile.Flush();
                }
                outFile.Write("\n");
                Debug.Log(y.ToString());
            }
            outFile.Write("sceneEnd:\n"); // end the array
        }
        
    }

    // reads the tile
    string getTile(int x, int y)
    {
        if (x <= 0 || x >= scene.GetLength(0) || y <= 0 || y >= scene.GetLength(1)) return "1"; //Render blocks outside of world range
        else return scene[x, y];
    }
        
    // sets the tile
    void setTile(int x, int y, string type)
    {
        if (x <= 0 || x >= scene.GetLength(0) || y <= 0 || y >= scene.GetLength(1)) return; //Render blocks outside of world range
        else scene[x, y] = type;
        if (getTile(x, y) != "")
        {
            Object tmp;

            if (GameObject.Find("tile" + x + "," + y) == null)
            {
                tmp = Instantiate(tile, new Vector3(x * 4, y * 4, (modulus(x + y, 4)) / 4), Quaternion.identity);
                tmp.name = "tile" + x + "," + y;
            }
            updateTile(x, y);
        }
        else
        {
            if (GameObject.Find("tile" + x + "," + y) != null) Destroy(GameObject.Find("tile" + x + "," + y));
        }
        updateTile(x - 1, y);
        updateTile(x + 1, y);
        updateTile(x, y - 1);
        updateTile(x, y + 1);
    }

    float modulus(float a, float b)
    {
        float c = a % b;
        return (c < 0) ? c + b : c;
    }

    public void loadCurrent()
    {
        // set up file name
        string fileName = "temp.sloth";
        // delete old file
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
        using (StreamWriter outFile = new StreamWriter(fileName)) // create file
        {
            // Write Header Saying that the next lines contains the array, also shows the array size
            outFile.Write("sceneStart:" + scene.GetLength(0) + "," + scene.GetLength(1) + "\n");

            // print the array
            for (int y = 0; y < scene.GetLength(1); y++)
            {
                for (int x = 0; x < scene.GetLength(0); x++)
                {
                    outFile.Write(scene[x, y] + ".");
                    outFile.Flush();
                }
                outFile.Write("\n");
                Debug.Log(y.ToString());
            }
            outFile.Write("sceneEnd:\n"); // end the array
        }

        Application.LoadLevel(1); //loads the play scene
    }



    // This is the modes switch functions that work with the UI
    #region mode switching

    public void modeBlock()
    {
        blockMode = 1;
        CurrentText.text = "Land";
    }

    public void modeSpike()
    {
        blockMode = 2;
        CurrentText.text = "Spike";
    }

    public void modeErase()
    {
        blockMode = 3;
        CurrentText.text = "Erase";
    }
    public void modeCamera()
    {
        blockMode = 0;
        CurrentText.text = "Camera";
    }

    #endregion

}