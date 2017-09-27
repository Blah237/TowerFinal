using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorScript : MonoBehaviour {

    public Level level; 

    public float width;
    public float height;
    private float sidebarWidth;
    public float padding;
    
    private Image[,] displayGrid;

    [SerializeField]
    private coord playerCoord = new coord(-1, -1);

    public Image gridSquare;

    public Canvas canvas;

    public List<Sprite> tileSprites;

    private delegate void ClickTileDelegate(int row, int col);
    private ClickTileDelegate clickTile;

    public bool loadedLevel;
    public string levelName;

    [SerializeField]
    private RawImage saveButton;
    [SerializeField]
    private RawImage saveAsButton;

	// Use this for initialization
	void Start () {
        // initialize grid
        if (loadedLevel && levelName != null) {
            level = IOScript.ParseLevel(levelName);
        }
        else {
            level.board = new int[level.rows, level.cols];
            fillOutline();
        }
        displayGrid = new Image[level.rows, level.cols];
        width *= canvas.pixelRect.width;
        sidebarWidth = canvas.pixelRect.width - width;
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
                if (level.board[r, c] % 10 == 2) {
                    if (playerCoord.row == -1 && playerCoord.col == -1) {
                        playerCoord.row = r;
                        playerCoord.col = c;
                    } else {
                        throw new System.Exception("Can't have two players in initial level!");
                    }
                }
                i.color = level.board[r, c] == 1 ? Color.black : Color.white;
            }
        }

        // position sidebar buttons
        saveAsButton.rectTransform.anchoredPosition = new Vector2((width + sidebarWidth/2f), sidebarWidth/4f);
        saveAsButton.rectTransform.sizeDelta = new Vector2(sidebarWidth, sidebarWidth/2f);
        saveAsButton.rectTransform.localScale = Vector2.one;
        saveButton.rectTransform.anchoredPosition = new Vector2((width + sidebarWidth / 2f), 3f*sidebarWidth/4f);
        saveButton.rectTransform.sizeDelta = new Vector2(sidebarWidth, sidebarWidth / 2f);
        saveButton.rectTransform.localScale = Vector2.one;
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
            // g enters goal placement mode
        } else if (Input.GetKeyDown(KeyCode.G)) {
            clickTile = this.placeGoal;
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

    // returns the tile sprite for this title
    Sprite getTileSprite(int val) {
        if (val >= tileSprites.Count || val < 0) {
            return null;
        }
        return tileSprites[val];
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
        if (level.board[row, col] % 10 == 0) {
            int r = playerCoord.row;
            int c = playerCoord.col;

            if (r != -1 && c != -1) {
                level.board[r, c] = (level.board[r, c] / 10) * 10;
                displayGrid[r, c].color = Color.white;
                displayGrid[r, c].sprite = getTileSprite(level.board[r, c]);
            }
            playerCoord.row = row;
            playerCoord.col = col;
            level.board[row, col] = (level.board[row, col] / 10) * 10 + 2;
            displayGrid[row, col].sprite = getTileSprite(level.board[row, col]);
        }
    }

    //places and removes goals
    void placeGoal(int row, int col) {
        // check if there's a goal
        if (level.board[row, col] >= 10 && level.board[row, col] < 20) {
            level.board[row, col] -= 10;
        } else {
            level.board[row, col] = (level.board[row, col] % 10) + 10;
        }
        displayGrid[row, col].sprite = getTileSprite(level.board[row, col]);
    }

    //places any kind of statue
    void placeStatue(int stat, int row, int col) {
        if (level.board[row, col] % 10 == stat) {
            level.board[row, col] = (level.board[row, col] / 10) * 10;
            displayGrid[row, col].sprite = getTileSprite(level.board[row, col]);
        } else {
            level.board[row, col] = (level.board[row, col] / 10) * 10 + stat;
            displayGrid[row, col].color = Color.white;
            displayGrid[row, col].sprite = getTileSprite(level.board[row, col]);
        }
    }

    //use placeStatue with a defined value of stat
    //void place2(int row, int col) { placeStatue(2, row, col); }
    void place3(int row, int col) { placeStatue(3, row, col); }
    void place4(int row, int col) { placeStatue(4, row, col); }
    #endregion

    // register a click on the right sidebar
    void clickRightSidebar(float mouseX, float mouseY) {
        if(mouseY < sidebarWidth / 2f) {
            Debug.Log("SaveAs Clicked");
            clickSaveAs();
        }
        else if (mouseY < sidebarWidth) {
            Debug.Log("Save Clicked");
            clickSave();
        } else {
            Debug.Log("Sidebar Clicked");
        }
        //Debug.Log(IOScript.ExportLevel(level)); 
    }

    void clickSaveAs() {
        // define level name

        // TODO make this involve inputting a level name.
        levelName = "FIXME";
        loadedLevel = true;
        clickSave();
    }

    void clickSave() {
        if (!loadedLevel) {
            clickSaveAs();
            return;   
        }
        save(this.levelName);
    }

    void save(string fileName) {
        // TODO mix me with IO
        Debug.Log("Saving Level...");
        Debug.Log(IOScript.ExportLevel(level));
    }

    // returns a representation of the stored level
    public int[,] getLevelOutput() {
        return this.level.board;
    }
}
