using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorScript : MonoBehaviour {

    public enum clickTileOptions { WALL = 1, PLAYER = 2, MIMIC = 3, MIRROR = 4, GOAL = 10, SWAP = 20, BUTTON = 30 }

    [System.Serializable]
    public struct clickSetter
    {
        public string label;
        public clickTileOptions value;
    }

    [System.Serializable]
    public struct SpriteMapping
    {
        public Sprite sprite;
        public int value;
        public SpriteMapping(Sprite sprite, int value) {
            this.sprite = sprite;
            this.value = value;
        }
    }

    public Level level;
    private Level cacheLevel;

    private float width = .85f;
    private float height = 1f;
    private float sidebarWidth;
    public float padding;
    
    private Image[,] displayGrid;

    //[SerializeField]
    private coord playerCoord = new coord(-1, -1);

    public Image gridSquare;

    public Canvas canvas;

    public List<SpriteMapping> tileSprites;

    public delegate void ClickTileDelegate(int row, int col);
    private ClickTileDelegate clickTile;

    public bool loadedLevel;
    public string levelName;

    [SerializeField]
    private RectTransform gridHolder;
    [SerializeField]
    private RawImage saveButton;
    [SerializeField]
    private RawImage saveAsButton;
    [SerializeField]
    private RawImage resizeButton;
    [SerializeField]
    private RawImage sidebarPiece;
    [SerializeField]
    public List<clickSetter> clickSetters;
    [SerializeField]
    private RawImage namePopUp;
    [SerializeField]
    private RawImage sizePopUp;
    [SerializeField]
    private Text inputText;

	// Use this for initialization
	void Start () {
        // initialize grid
        if (loadedLevel && levelName != null) {
            level = IOScript.ParseLevel(levelName);
        }
        else {
            level.board = new int[level.rows, level.cols];
            fillOutline(level);
        }
        width *= canvas.pixelRect.width;
        sidebarWidth = canvas.pixelRect.width - width;
        height *= canvas.pixelRect.height;

        clickTile = toggleWall;

        // creates entire view
        createAndPositionSquares(level);

        // position sidebar buttons
        saveAsButton.rectTransform.anchoredPosition = new Vector2((width + sidebarWidth/2f), sidebarWidth/4f);
        saveAsButton.rectTransform.sizeDelta = new Vector2(sidebarWidth, sidebarWidth/2f);
        saveAsButton.rectTransform.localScale = Vector2.one;
        saveButton.rectTransform.anchoredPosition = new Vector2((width + sidebarWidth / 2f), 3f*sidebarWidth/4f);
        saveButton.rectTransform.sizeDelta = new Vector2(sidebarWidth, sidebarWidth / 2f);
        saveButton.rectTransform.localScale = Vector2.one;
        resizeButton.rectTransform.anchoredPosition = new Vector2((width + sidebarWidth / 2f), 5f * sidebarWidth / 4f);
        resizeButton.rectTransform.sizeDelta = new Vector2(sidebarWidth, sidebarWidth / 2f);
        resizeButton.rectTransform.localScale = Vector2.one;

        for (int i = 0; i < clickSetters.Count; i++) {
            RawImage button = Instantiate(this.sidebarPiece);
            button.transform.parent = this.canvas.transform;
            positionSidebarPiece(button, clickSetters[i], i, clickSetters.Count, height, sidebarWidth * 3f / 2f);
        }
        /*sidebar.rectTransform.anchoredPosition = new Vector2((width + sidebarWidth / 2f), 2.35f * sidebarWidth);
        sidebar.rectTransform.sizeDelta = new Vector2(sidebarWidth, height - (sidebarWidth * 3f / 2f) - padding);
        sidebar.rectTransform.localScale = Vector2.one;*/
            
        setNamePopUpVisible(false);
        setSizePopUpVisible(false);
    }

    // position square in the correct row/column
    void positionSquare(Image square, int r, int c) {
        square.rectTransform.anchoredPosition = new Vector2(width / level.cols * (c + 0.5f), height / level.rows * (r + 0.5f));
        square.rectTransform.sizeDelta = new Vector2(width / (float)level.cols - padding / 2f, height / (float)level.rows - padding / 2f);
        square.rectTransform.localScale = Vector2.one;
    }

    void positionSidebarPiece(RawImage button, clickSetter setter, int index, int count, float topBarHeight, float bottomBarHeight) {
        float t = index / (float)count;
        button.rectTransform.sizeDelta = new Vector2(sidebarWidth, (topBarHeight - bottomBarHeight)/count);
        button.rectTransform.anchoredPosition = new Vector2(width + sidebarWidth / 2f, topBarHeight * (1f - t) + bottomBarHeight * t - button.rectTransform.sizeDelta.y/2);
        button.rectTransform.localScale = Vector2.one;
        button.GetComponentInChildren<Text>().text = setter.label;
    }

    void createAndPositionSquares(Level level) {
        if (displayGrid != null) {
            for (int r = 0; r < displayGrid.GetLength(0); r++) {
                for (int c = 0; c < displayGrid.GetLength(1); c++) {
                    Destroy(displayGrid[r, c]);
                }
            }
        }
        displayGrid = new Image[level.rows, level.cols];
        playerCoord = new coord(-1, -1);
        // create gridSquares
        for (int r = 0; r < level.rows; r++) {
            for (int c = 0; c < level.cols; c++) {
                Image i = GameObject.Instantiate(gridSquare);
                i.transform.parent = gridHolder.transform;
                positionSquare(i, r, c);
                displayGrid[r, c] = i;
                i.sprite = getTileSprite(level.board[r, c]);
                if (level.board[r, c] % 10 == 2) {
                    if (playerCoord.row == -1 && playerCoord.col == -1) {
                        playerCoord.row = r;
                        playerCoord.col = c;
                    } else {
                        throw new System.Exception("Can't have two players in level!");
                    }
                }
                i.color = level.board[r, c] == 1 ? Color.black : Color.white;
            }
        }
    }

    // Set the outer rows/columns of the grid to be walls
    void fillOutline(Level level) {
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
        if (!namePopUp.isActiveAndEnabled && !sizePopUp.isActiveAndEnabled) {
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
                // s enters swap placement mode
            } else if (Input.GetKeyDown(KeyCode.S)) {
                clickTile = this.placeSwap;
                // if left click this frame, handle click0
            } else if (Input.GetMouseButtonDown(0)) {
                click0(Input.mousePosition.x, Input.mousePosition.y);
            }
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
        foreach(SpriteMapping sm in tileSprites) {
            if(sm.value == val) {
                return sm.sprite;
            }
        }
        return null;
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
        displayGrid[row, col].sprite = getTileSprite(v2);
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

    //places and removes swaps
    void placeSwap(int row, int col) {
        // check if there's a goal
        if (level.board[row, col] >= 20 && level.board[row, col] < 30) {
            level.board[row, col] -= 20;
        } else {
            level.board[row, col] = (level.board[row, col] % 10) + 20;
        }
        displayGrid[row, col].sprite = getTileSprite(level.board[row, col]);
    }

    //places and removes Butons
    void placeButton(int row, int col) {
        // check if there's a goal
        if (level.board[row, col] >= 30 && level.board[row, col] < 40) {
            level.board[row, col] -= 30;
        } else {
            level.board[row, col] = (level.board[row, col] % 10) + 30;
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
        } else if (mouseY < sidebarWidth * 3f / 2f) {
            Debug.Log("Resize Clicked");
            openSizePopUp();
        } else {
            float topHeight = this.height;
            float botHeight = sidebarWidth * 3f / 2f;
            float delt = (topHeight - botHeight) / (float)clickSetters.Count;
            int ind = clickSetters.Count - 1 - (int)((mouseY - (sidebarWidth * 3f / 2f)) / delt);
            setOnClick(clickSetters[ind].value);
            Debug.Log("Sidebar Clicked, ind: " + ind);
        }
        //Debug.Log(IOScript.ExportLevel(level)); 
    }

    void clickSaveAs() {
        openNamePopUp();
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
        Debug.Log(IOScript.ExportLevel(level, levelName));
    }

    public void setLevelName(string newName) {
        levelName = newName;
    }

    // returns a representation of the stored level
    public int[,] getLevelOutput() {
        return this.level.board;
    }

    void setNamePopUpVisible(bool value) {
        this.namePopUp.gameObject.SetActive(value);
    }

    private void openNamePopUp() {
        this.setNamePopUpVisible(true);
        this.inputText.text = levelName;
    }

    public void closeNamePopUp() {
        levelName = this.inputText.text;
        loadedLevel = true;
        this.setNamePopUpVisible(false);
        clickSave();
    }

    void setSizePopUpVisible(bool value) {
        this.sizePopUp.gameObject.SetActive(value);
    }

    public void openSizePopUp() {
        this.setSizePopUpVisible(true);
    }

    public void closeSizePopUp() {
        this.setSizePopUpVisible(false);
    }

    #region add/remove cols/rows
    public void addColLeft() {
        cacheLevel = new Level();
        cacheLevel.rows = level.rows;
        cacheLevel.cols = level.cols + 1;
        cacheLevel.board = new int[cacheLevel.rows, cacheLevel.cols];
        fillOutline(cacheLevel);
        for(int r = 1; r < level.rows - 1; r++) {
            int i = level.board[r, 1];
            i = i != 1 ? 0 : 1;
            cacheLevel.board[r, 1] = i;
        }
        for(int c = 2; c < level.cols; c++) {
            for(int r = 1; r < level.rows; r++) {
                int i = level.board[r, c - 1];
                cacheLevel.board[r, c] = i;
            }
        }
        level = cacheLevel;
        this.createAndPositionSquares(cacheLevel);
    }
    public void addColRight() {
        cacheLevel = new Level();
        cacheLevel.rows = level.rows;
        cacheLevel.cols = level.cols + 1;
        cacheLevel.board = new int[cacheLevel.rows, cacheLevel.cols];
        fillOutline(cacheLevel);
        for (int r = 1; r < level.rows - 1; r++) {
            int i = level.board[r, level.cols - 2];
            i = i != 1 ? 0 : 1;
            cacheLevel.board[r, level.cols - 2] = i;
        }
        for (int c = 1; c < level.cols - 1; c++) {
            for (int r = 1; r < level.rows; r++) {
                int i = level.board[r, c];
                cacheLevel.board[r, c] = i;
            }
        }
        level = cacheLevel;
        this.createAndPositionSquares(cacheLevel);
    }
    public void removeColLeft() {
        if (level.cols <= 3) {
            return;
        }
        cacheLevel = new Level();
        cacheLevel.rows = level.rows;
        cacheLevel.cols = level.cols - 1;
        cacheLevel.board = new int[cacheLevel.rows, cacheLevel.cols];
        fillOutline(cacheLevel);
        for (int c = 1; c < level.cols - 1; c++) {
            for (int r = 1; r < level.rows; r++) {
                int i = level.board[r, c + 1];
                cacheLevel.board[r, c] = i;
            }
        }
        level = cacheLevel;
        this.createAndPositionSquares(cacheLevel);
    }
    public void removeColRight() {
        if(level.cols <= 3) {
            return;
        }
        cacheLevel = new Level();
        cacheLevel.rows = level.rows;
        cacheLevel.cols = level.cols - 1;
        cacheLevel.board = new int[cacheLevel.rows, cacheLevel.cols];
        fillOutline(cacheLevel);
        for (int c = 1; c < level.cols - 2; c++) {
            for (int r = 1; r < level.rows; r++) {
                int i = level.board[r, c];
                cacheLevel.board[r, c] = i;
            }
        }
        level = cacheLevel;
        this.createAndPositionSquares(cacheLevel);
    }
    public void addRowAbove() {
        cacheLevel = new Level();
        cacheLevel.rows = level.rows + 1;
        cacheLevel.cols = level.cols;
        cacheLevel.board = new int[cacheLevel.rows, cacheLevel.cols];
        fillOutline(cacheLevel);
        for (int c = 1; c < level.cols - 1; c++) {
            int i = level.board[level.rows - 2, c];
            i = i != 1 ? 0 : 1;
            cacheLevel.board[level.rows - 2, c] = i;
        }
        for (int r = 1; r < level.rows - 1; r++) {
            for (int c = 1; c < level.cols; c++) {
                int i = level.board[r, c];
                cacheLevel.board[r, c] = i;
            }
        }
        level = cacheLevel;
        this.createAndPositionSquares(cacheLevel);
    }
    public void addRowBelow() {
        cacheLevel = new Level();
        cacheLevel.rows = level.rows + 1;
        cacheLevel.cols = level.cols;
        cacheLevel.board = new int[cacheLevel.rows, cacheLevel.cols];
        fillOutline(cacheLevel);
        for (int c = 1; c < level.cols - 1; c++) {
            int i = level.board[1, c];
            i = i != 1 ? 0 : 1;
            cacheLevel.board[1, c] = i;
        }
        for (int r = 2; r < level.rows; r++) {
            for (int c = 1; c < level.cols; c++) {
                int i = level.board[r - 1, c];
                cacheLevel.board[r, c] = i;
            }
        }
        level = cacheLevel;
        this.createAndPositionSquares(cacheLevel);
    }
    public void removeRowAbove() {
        if (level.rows <= 3) {
            return;
        }
        cacheLevel = new Level();
        cacheLevel.rows = level.rows - 1;
        cacheLevel.cols = level.cols;
        cacheLevel.board = new int[cacheLevel.rows, cacheLevel.cols];
        fillOutline(cacheLevel);
        for (int r = 1; r < level.rows - 2; r++) {
            for (int c = 1; c < level.cols; c++) {
                int i = level.board[r, c];
                cacheLevel.board[r, c] = i;
            }
        }
        level = cacheLevel;
        this.createAndPositionSquares(cacheLevel);
    }
    public void removeRowBelow() {
        if (level.rows <= 3) {
            return;
        }
        cacheLevel = new Level();
        cacheLevel.rows = level.rows - 1;
        cacheLevel.cols = level.cols;
        cacheLevel.board = new int[cacheLevel.rows, cacheLevel.cols];
        fillOutline(cacheLevel);
        for (int r = 1; r < level.rows - 1; r++) {
            for (int c = 1; c < level.cols; c++) {
                int i = level.board[r + 1, c];
                cacheLevel.board[r, c] = i;
            }
        }
        level = cacheLevel;
        this.createAndPositionSquares(cacheLevel);
    }
    #endregion

    #region setClickMethods
    public void setOnClick(int value) {
        this.setOnClick((clickTileOptions)value);
    }

    public void setOnClick(clickTileOptions value) {
        switch (value) {
            case clickTileOptions.WALL:
                clickTile = this.toggleWall;
                break;
            case clickTileOptions.PLAYER:
                clickTile = placePlayer;
                break;
            case clickTileOptions.MIMIC:
                clickTile = place3;
                break;
            case clickTileOptions.MIRROR:
                clickTile = place4;
                break;
            case clickTileOptions.GOAL:
                clickTile = placeGoal;
                break;
            case clickTileOptions.SWAP:
                clickTile = placeSwap;
                break;
            case clickTileOptions.BUTTON:
                clickTile = placeButton;
                break;
            default:
                throw new System.NotImplementedException();
        }
    }
    #endregion
}
