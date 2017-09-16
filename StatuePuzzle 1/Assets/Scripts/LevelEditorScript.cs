using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorScript : MonoBehaviour {

    public Level level; 

    public float width;
    public float height;
    public float padding;
    
    private Image[,] displayGrid;

    private coord playerCoord = new coord(-1, -1);

    public Image gridSquare;

    public Canvas canvas;

    public List<Sprite> tileSprites;

    private delegate void ClickTileDelegate(int row, int col);
    private ClickTileDelegate clickTile;

    public bool loadLevel;
    public string levelName; 

	// Use this for initialization
	void Start () {
        // initialize grid
        if (loadLevel && levelName != null) {
            level = IOScript.ParseLevel(levelName);
        }
        else {
            level.board = new int[level.rows, level.cols];
            fillOutline();
        }
        displayGrid = new Image[level.rows, level.cols];
        width *= canvas.pixelRect.width;
        height *= canvas.pixelRect.height;

        clickTile = toggleWall;

        // create gridSquares
        for (int r = 0; r < level.rows; r++) {
            for (int c = 0; c < level.cols; c++){
                Image i = GameObject.Instantiate(gridSquare);
                i.transform.parent = canvas.transform;
                positionSquare(i, r, c);
                displayGrid[r, c] = i;
                i.sprite = tileSprites[level.board[r, c]];
                if (level.board[r, c] == 2) {
                    if(playerCoord.row == -1 && playerCoord.col == -1) {
                        playerCoord.row = r;
                        playerCoord.col = c;
                    } else {
                        throw new System.Exception("Can't have two players in initial level!");
                    }
                }
                i.color = level.board[r, c] == 1 ? Color.black : Color.white;
            }
        }
	}

    // position square in the correct row/column
    void positionSquare(Image square, int r, int c) {
        square.rectTransform.anchoredPosition = new Vector2(width / level.cols * (c + 0.5f), height / level.rows * (r + 0.5f));
        square.rectTransform.sizeDelta = new Vector2(width / (float)level.cols - padding / 2f, height / (float)level.rows - padding / 2f);
        square.rectTransform.localScale = Vector2.one;
    }

    // Set the outer rows/columns of the grid to be walls
    void fillOutline() {
        for (int r = 0; r < level.rows; r++) {
            level.board[r, 0] = 1;
            level.board[r, level.cols - 1] = 1;
        }
        for (int c = 1; c < level.cols - 1; c++) {
            level.board[0, c] = 1;
            level.board[level.rows - 1, c] = 1;
        }
    }

    // Update is called once per frame
    void Update () {
        // escape or right click resets delegate
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Alpha0) 
            || Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Keypad1)) {
            clickTile = this.toggleWall;
            // p enters player placement mode
        } else if (Input.GetKeyDown(KeyCode.P)) {
            clickTile = this.placePlayer;
        } else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha2)) {
            clickTile = this.placePlayer;
        } else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha3)) {
            clickTile = this.place3;
        } else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Alpha4)) {
            clickTile = this.place4;
            // if left click this frame, handle click0
        } else if (Input.GetMouseButtonDown(0)) {
            click0(Input.mousePosition.x, Input.mousePosition.y);
        }
	}

    // return the grid column that should fall under mouseX
    int getMouseCol(float mouseX) {
        return (int)(mouseX / (width / level.cols));
    }

    // return the grid column that should fall under mouseY
    int getMouseRow(float mouseY) {
        return (int)(mouseY / (height / level.rows));
    }

    // toggle the value of the tile under the mouse
    void click0(float mouseX, float mouseY) {
        int c = getMouseCol(mouseX);
        int r = getMouseRow(mouseY);

        if (c >= level.cols) {
            clickRightSidebar(Input.mousePosition.x, Input.mousePosition.y);
        } else {
            clickTile(r, c);
        }
    }

    #region clickTile methods
    // sets grid space [r, c] to 0 if 1, and to 1 if 0
    void toggleWall(int row, int col) {
        // don't allow toggle on player space
        if(playerCoord.row == row && playerCoord.col == col) {
            return;
        }

        int v = level.board[row, col];
        int v2 = v == 0 ? 1 : 0;

        level.board[row, col] = v2;
        displayGrid[row, col].sprite = tileSprites[v2];
        displayGrid[row, col].color = v2 == 1 ? Color.black : Color.white;
    }

    //sets the player position
    void placePlayer(int row, int col) {
        if(level.board[row, col] == 0) {
            int r = playerCoord.row;
            int c = playerCoord.col;

            if (r != -1 && c != -1) {
                level.board[r, c] = 0;
                displayGrid[r, c].color = Color.white;
                displayGrid[r, c].sprite = tileSprites[0];
            }
            playerCoord.row = row;
            playerCoord.col = col;
            displayGrid[row, col].sprite = tileSprites[2];
        }
    }

    //places any kind of statue
    void placeStatue(int stat, int row, int col) {
        if(level.board[row, col] == stat) {
            level.board[row, col] = 0;
            displayGrid[row, col].sprite = tileSprites[0];
        } else {
            level.board[row, col] = stat;
            displayGrid[row, col].color = Color.white;
            displayGrid[row, col].sprite = tileSprites[stat];
        }
    }

    //use placeStatue with a defined value of stat
    //void place2(int row, int col) { placeStatue(2, row, col); }
    void place3(int row, int col) { placeStatue(3, row, col); }
    void place4(int row, int col) { placeStatue(4, row, col); }
    #endregion

    // register a click on the right sidebar
    void clickRightSidebar(float mouseX, float mouseY) {
        Debug.Log("Sidebar Clicked");
        Debug.Log(IOScript.ExportLevel(level)); 
    }

    // returns a representation of the stored level
    public int[,] getLevelOutput() {
        return this.level.board;
    }
}
