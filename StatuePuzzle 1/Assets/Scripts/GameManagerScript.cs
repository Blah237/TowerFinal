using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public enum Direction { NORTH, SOUTH, EAST, WEST, NONE }

public enum BoardCodes:int {
	EMPTY = 0,
	WALL = 1,
	PLAYER = 2,
	MIMIC = 3, 
	MIRROR = 4,
	GOAL = 10,
	//FOR NOW, COMBOS ARE > 10. IF THIS CHANGES, MAKE THE CHANGE IN MOVEABLESCRIPT
	PLAYER_ON_GOAL = 12,
	MIMIC_ON_GOAL = 13,
	MIRROR_ON_GOAL = 14, 
    
    SWAP = 20,
    PLAYER_ON_SWAP = 22,
    MIMIC_ON_SWAP = 23,
    MIRROR_ON_SWAP = 24,
	
	BUTTON = 30,
	PLAYER_ON_BUTTON = 32,
	MIMIC_ON_BUTTON = 33,
	MIRROR_ON_BUTTON = 34,
}

[System.Serializable]
public struct coord {
    public int row;
    public int col;
    public coord(int row, int col) {
        this.row = row;
        this.col = col;
    }
    public override string ToString() {
        return "coord(r-" + row + ", c-" + col+")";
    }

	public bool Equals (coord otherCoord) {
		if ((System.Object)otherCoord == null) {
			return false;
		}

		return row == otherCoord.row && col == otherCoord.col; 
	}

	public override int GetHashCode ()
	{
		return row ^ col;
	}
		
}

public class GameManagerScript : MonoBehaviour {

    List<MoveableScript> moveables = new List<MoveableScript>();
    public Stack<DynamicState> dynamicStateStack = new Stack<DynamicState>();
    public PlayerScript player;
    public MimicScript mimic;
    public MirrorScript mirror;
    public GameObject wall;
    public GameObject frontWall;
    public GoalScript goal;
    public LaserScript laser;

    public WinScript winscript;
    public DeathScript deathscript;
    public PauseScript pausescript;
    public RestartLevel restartscript;

    public ButtonToggleScript button;

    public GameObject ground;
    public Camera mainCamera;

    public Text tutorial;
    public GameObject tutorial1;
    public GameObject tutorial3;
    public GameObject tutorial6;
    public GameObject tutorialCanvas;
    public GameObject restartConfirmText;

    public bool win;
    public bool dead;
    public bool showRestartConfirm;

    private int restartScreenTimer = 0;

    List<coord> goalCoords = new List<coord>();
    Dictionary<coord, GoalScript> goalAtCoords = new Dictionary<coord, GoalScript>();
    List<LaserScript> laserList = new List<LaserScript>();

    public GameObject swap;
    public GameObject portal;

    List<coord> swapCoords = new List<coord>();
    Dictionary<coord, ButtonToggleScript> buttonCoords = new Dictionary<coord, ButtonToggleScript>();
    List<coord> portalCoords = new List<coord>();
    Dictionary<coord, coord> portalMap = new Dictionary<coord, coord>();
    HashSet<ButtonToggleScript> buttonsPressed = new HashSet<ButtonToggleScript>();
    HashSet<MoveableScript> needsSwap = new HashSet<MoveableScript>();

    public static Vector2 mapOrigin;

    bool firstStart;

    [SerializeField]
    public static bool inputReady = true;
    public static bool pauseReady = true;
    Direction? inputDir;

    [SerializeField]
    public static string levelName;
    public static int levelNum;

    Level boardState; //Row, Column
                      // East col+, North row+

    /* 0 = empty
     * 1 = wall
     * 2 = player
     * 3 = mimic
     * 4 = mirror
     * 10 = goal
     */

    // Use this for initialization
    void Start() {

		firstStart = false;

        //load level using Melody's I/O
        boardState = IOScript.ParseLevel(levelName);

        // TODO: Below the way we get the index is DISGUSTING, this whole shitshow needs refactored
        levelNum = CreateLevelSelect.levelList.IndexOf(levelName);
        LoggingManager.instance.RecordLevelStart(levelNum, levelName);

        mapOrigin = new Vector2(-boardState.cols / 2.0f, -boardState.rows / 2.0f);
        mainCamera.orthographicSize = boardState.rows / 2.0f + 1;
        tutorial.text = boardState.tutorial;
        tutorial.enabled = true;
        if (levelNum == 0) {
            tutorial1.SetActive(true);
        } else if (levelNum == 5) {
            tutorial6.SetActive(true);
        }


        int buttonCount = 0;

        //instantiate lasers based on parsed lasers
        if (boardState.lasers != null) {
            foreach (Laser la in boardState.lasers) {
                LaserScript l = GameObject.Instantiate(laser);
                l.makeLaser(la, mapOrigin);
                laserList.Add(l);
            }
        }

        //instantiate items based on board
        for (int i = 0; i < boardState.rows; i++) {
            for (int j = 0; j < boardState.cols; j++) {
                if (boardState.board[i, j] == 1) {
                    //wall
                    GameObject w;
                    if (i == 0 || boardState.board[i - 1, j] != 1) {
                        w = GameObject.Instantiate(frontWall);
                    } else {
                        w = GameObject.Instantiate(wall);
                    }
                    w.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                } else if (boardState.board[i, j] >= 10 && boardState.board[i, j] < 20) {
                    // goal
                    GoalScript c = GameObject.Instantiate(goal);
                    c.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    goalAtCoords.Add(new coord(i, j), c);
                    goalCoords.Add(new coord(i, j));
                } else if (boardState.board[i, j] >= 20 && boardState.board[i, j] < 30) {
                    // swap
                    GameObject c = GameObject.Instantiate(swap);
                    c.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    swapCoords.Add(new coord(i, j));
                } else if (boardState.board[i, j] >= 30 && boardState.board[i, j] < 40) {
                    // button
                    ButtonToggleScript c = GameObject.Instantiate(button);
                    c.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    c.laser = laserList[boardState.buttons[buttonCount]];
                    c.InitButton();
                    buttonCoords.Add(new coord(i, j), c);
                    buttonCount++;
                } else if (boardState.board[i, j] >= 50 && boardState.board[i, j] < 60) {
                    // portal
                    GameObject p = GameObject.Instantiate(portal);
                    p.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    portalCoords.Add(new coord(i, j));
                } else {
                    // ground 
                    GameObject g = GameObject.Instantiate(ground);
                    g.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                }

                if (boardState.board[i, j] % 10 == 2) {
                    // player
                    player = GameObject.Instantiate(player);
                    player.SetCoords(j, i);
                    player.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y + player.yOffset, -0.2f);
                    moveables.Add(player);
                } else if (boardState.board[i, j] % 10 == 3) {
                    // mimic
                    MimicScript m = GameObject.Instantiate(mimic);
                    m.SetCoords(j, i);
                    m.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y + m.yOffset, -0.2f);
                    moveables.Add(m);
                } else if (boardState.board[i, j] % 10 == 4) {
                    // mirror
                    MirrorScript m = GameObject.Instantiate(mirror);
                    m.SetCoords(j, i);
                    m.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y + m.yOffset, -0.2f);
                    moveables.Add(m);
                }
            }
        }

        if (boardState.portals != null) {
            for (int i = 0; i < boardState.portals.Length; i++) {
                portalMap.Add(portalCoords[i], portalCoords[boardState.portals[i]]);
            }
        }
    }

    void updateLoopingSounds() {

        if (pausescript.paused) {
            return;
        }

        // Looping sound effects
        int mirrorsOnGoal = 0;
        int mimicsOnGoal = 0;
        foreach (MoveableScript m in moveables) {
            if (m.type == BoardCodes.MIMIC && goalCoords.Contains(m.GetCoords())) {
                mimicsOnGoal++;
            } else if (m.type == BoardCodes.MIRROR && goalCoords.Contains(m.GetCoords())) {
                mirrorsOnGoal++;
            }
        }

        AudioManagerScript.instance.setMimicPlayingCount(mimicsOnGoal);
        AudioManagerScript.instance.setMirrorPlayingCount(mirrorsOnGoal);
    }

    // Update is called once per frame
    void Update() {

        // TODO: Hacky, but neccessary if we want accurate recording and not a real loading screen
        if (!firstStart) {
            recordDynamicState();
            firstStart = true;
        }

        updateLoopingSounds();

        CheckSwaps();

        if (inputReady) {
            foreach (ButtonToggleScript button in buttonsPressed) {
                button.TogglePressed();
            }
            buttonsPressed.Clear();
            foreach (MoveableScript m in needsSwap) {
                PerformSwap(m);
            }
            needsSwap.Clear();
            if (!WinScript.playerWin) {
                checkWin();
                checkFailState();
            }
            Direction dir = readInput();
            if (dir != Direction.NONE && !WinScript.playerWin) {
                inputReady = false;
                move(dir);
            }
        } else {
            inputReady = (!WinScript.playerWin && getAllDone());
            pauseReady = (!WinScript.playerWin);
        }
        if (pauseReady && checkPause()) {
            pausescript.TogglePause();
            tutorial.enabled = !tutorial.enabled;
            tutorialCanvas.SetActive(!tutorialCanvas.activeInHierarchy);
        } else if (pauseReady && Input.GetKeyDown(KeyCode.Escape)) {
            if (pausescript.paused) {
                LoadLevelSelect.LoadSceneEscFromPause();
            } else {
                pausescript.TogglePause();
                tutorial.enabled = !tutorial.enabled;
                tutorialCanvas.SetActive(!tutorialCanvas.activeInHierarchy);
            }
        }
        if (pausescript.paused) {
            showRestartConfirm = false;
            restartConfirmText.SetActive(false); 
            inputReady = false;
        }
        handleRestart();
    }

    bool getAllDone() {
        foreach (MoveableScript m in moveables) {
            if (!m.GetIsDone()) {
                return false;
            }
        }
        return true;
    }

    public void resume() {
        pausescript.TogglePause();
        tutorial.enabled = !tutorial.enabled;
        tutorialCanvas.SetActive(!tutorialCanvas.activeInHierarchy);
    }

    private void undo() {
        // TODO: Record an undo with logging as an event
        if (dynamicStateStack.Count <= 1) {
            return;
        }
        try {
            dynamicStateStack.Pop();
            DynamicState ds = dynamicStateStack.Peek();
            LoggingManager.instance.RecordEvent(LoggingManager.EventCodes.UNDO, ds.toJson());
            //int mimicIdx = 0;
            //int mirrorIdx = 0;
            List<coord> mimicCoords = ds.mimicPositions.ToList();
            List<coord> mirrorCoords = ds.mirrorPositions.ToList();
            while (moveables.Count > 0) {
                MoveableScript ms = moveables[0];
                moveables.RemoveAt(0);
                if (ms is PlayerScript) {
                    continue;
                }
                Destroy(ms.gameObject);
            }
            player.SetCoords(ds.playerPosition.col, ds.playerPosition.row);
            player.transform.position = new Vector3(player.GetCoords().col + mapOrigin.x, player.GetCoords().row + mapOrigin.y + player.yOffset);
            moveables.Add(player);
            foreach (coord c in mimicCoords) {
                MimicScript m = GameObject.Instantiate(mimic);
                m.SetCoords(c.col, c.row);
                m.transform.position = new Vector3(c.col + mapOrigin.x, c.row + mapOrigin.y + m.yOffset, -0.2f);
                moveables.Add(m);
            }
            foreach (coord c in mirrorCoords) {
                MirrorScript m = GameObject.Instantiate(mirror);
                m.SetCoords(c.col, c.row);
                m.transform.position = new Vector3(c.col + mapOrigin.x, c.row + mapOrigin.y + m.yOffset, -0.2f);
                moveables.Add(m);
            }
            foreach (coord c in buttonCoords.Keys) {
                if (buttonCoords[c].laser.data.isActive != ds.activeButtons.Contains(c)) {
                    buttonCoords[c].TogglePressed();
                }
            }

        } catch (InvalidOperationException) {
            //TODO: Actually display this to the user 
            Debug.Log("Empty stack, no previous move to undo.");
        }
    }

    Direction readInput() {
        if (WinScript.playerWin || !getAllDone()) {
            return Direction.NONE;
        }
		if (Input.GetAxis ("Horizontal") >= 1f) {
			return Direction.EAST;
		} else if (Input.GetAxis ("Horizontal") <= -1f) {
			return Direction.WEST;
		} else if (Input.GetAxis ("Vertical") >= 1f) {
			return Direction.NORTH;
		} else if (Input.GetAxis ("Vertical") <= -1f) {
			return Direction.SOUTH;
		} else if (Input.GetKeyDown (KeyCode.Z)) {
			Debug.Log ("Undoing");
			undo ();
			return Direction.NONE;
		} else {
            return Direction.NONE;
        }
    }

    bool checkWin() {

		HashSet<coord> nppCoords = new HashSet<coord> ();
		foreach (MoveableScript m in moveables) {
			if (m.type != BoardCodes.PLAYER) {
				nppCoords.Add (m.GetCoords());
			}
		}
        bool didWin = true; 
        foreach (GoalScript g in goalAtCoords.Values) {
            g.isWin = false; 
        }
        foreach (coord c in goalCoords) {
			if (!nppCoords.Contains (c)) {
				didWin = false;
			} else {
                goalAtCoords[c].isWin = true; 
            }
        }
        foreach (GoalScript g in goalAtCoords.Values) {
            g.ToggleParticles(); 
        }
        if (!didWin) {
            return false; 
        }
        Debug.Log("VICTORY!");
		//AudioManagerScript.instance.mirrorGoal.loop = false;
		//AudioManagerScript.instance.mimicGoal.loop = false;
		LoggingManager.instance.RecordEvent (LoggingManager.EventCodes.LEVEL_COMPLETE, "Level complete");
		LoggingManager.instance.RecordLevelEnd ();
        player.Celebrate();
        //AudioManagerScript.instance.soundFx.PlayOneShot(player.victorySound);
        inputReady = false; 
	    WinScript.playerWin = true;
        WinScript.newChallengeUnlock = false;
        Debug.Log("Won Level " + levelNum);
        if (CreateLevelSelect.challengeUnlocks.ContainsKey(levelNum)) {
            int chal = CreateLevelSelect.challengeUnlocks[levelNum];
            if(PlayerPrefs.GetInt("chal_"+chal+"_unlocked", 0) != 1) {
                PlayerPrefs.SetInt("chal_" + chal + "_unlocked", 1);
                WinScript.newChallengeUnlock = true;
                Debug.Log("Challenge " + chal + " Unlocked!");
            }
        }
	    pauseReady = false;
        tutorial.enabled = false;
        tutorial1.SetActive(false);
        tutorial6.SetActive(false);
        return true;
    }

    void checkFailState() {
        if(levelName == "tutorial3") {
            if (moveables[1].GetCoords().col - 1 == moveables[0].GetCoords().col) {
                tutorial3.SetActive(true);
            } else {
                tutorial3.SetActive(false); 
            }
        }
    }

	bool checkPause()
	{
		return Input.GetKeyDown(KeyCode.P);
	}

	bool checkRestart()
	{
		return Input.GetKeyDown(KeyCode.R);
	}

	void handleRestart()
	{
		if (showRestartConfirm && pausescript.unpaused)
		{
			if (checkRestart())
			{
				showRestartConfirm = false;
                restartConfirmText.SetActive(false); 
				restartscript.LoadScene();
			} else if (Input.anyKeyDown)
			{
				showRestartConfirm = false;
                restartConfirmText.SetActive(false); 
			}
		}
		else if (checkRestart() && pausescript.unpaused)
		{
			showRestartConfirm = true;
            restartConfirmText.SetActive(true); 
		}
	}

	public static void setLevelName(string level) {
		levelName = level;
	}

    void move(Direction dir) {

		Dictionary<MoveableScript,coord> desiredCoords = new Dictionary<MoveableScript, coord>(); //where pieces would move without collisions/walls
		Dictionary<MoveableScript,Direction> moveDirections = new Dictionary<MoveableScript, Direction>(); //directions pieces will actually move
		Dictionary<MoveableScript, int> collided = new Dictionary<MoveableScript, int>();
        Dictionary<MoveableScript, coord> blockingCoords = new Dictionary<MoveableScript, coord>();

        foreach (MoveableScript m in moveables) {
            Direction direction = m.GetAttemptedMoveDirection(dir, boardState.board);

            coord desired = m.GetAttemptedMoveCoords(dir, boardState.board, 1);
            if (boardState.board[desired.row, desired.col] / 10 == 5) {
                blockingCoords.Add(m, desired);
                desired = portalMap[desired];
            }
            desiredCoords.Add(m, desired);

            //Check for collisions with lasers 
            foreach (LaserScript laser in laserList) {
                // if laser is active && laser can collide with this moveable 
                if (laser.data.isActive && laser.data.type != m.type) {
                    //if moveable is jumping through a horizontal laser
                    if ((direction == Direction.NORTH && m.GetCoords().row == laser.data.startRow) ||
                        (direction == Direction.SOUTH && desired.row == laser.data.startRow)) {
                        if (laser.data.isBetweenCol(m.GetCoords().col)) {
                            moveDirections[m] = Direction.NONE;
                            desired = m.GetCoords();
                            desiredCoords[m] = desired;
                        }
                    }
                    //if moveable is jumping through a vertical laser 
                    if ((direction == Direction.EAST && desired.col == laser.data.startCol) ||
                        (direction == Direction.WEST && m.GetCoords().col == laser.data.startCol)) {
                        if (laser.data.isBetweenRow(m.GetCoords().row)) {
                            moveDirections[m] = Direction.NONE;
                            desired = m.GetCoords();
                            desiredCoords[m] = desired;
                        }
                    }
                }
            }

            // Check for collisions moving into the same spot
            foreach (MoveableScript other in moveables) {
				if (other == m || other == null) {
					continue;

				//Check for moving into same spot
				} else if ((desiredCoords.ContainsKey(other) && desiredCoords[other].Equals(desired)) // classic move into blocked
                    || (desiredCoords.ContainsKey(other) && blockingCoords.ContainsKey(m) && desiredCoords[other].Equals(blockingCoords[m])) // other is moving into my blocker
                    || (blockingCoords.ContainsKey(other) && blockingCoords[other].Equals(desired)) // I am moving into other's blocker
                    || (blockingCoords.ContainsKey(other) && blockingCoords.ContainsKey(m) && blockingCoords[m].Equals(blockingCoords[other]))) { //we share a blocker
					moveDirections[other] = Direction.NONE;
					moveDirections[m] = Direction.NONE;
					dead = true;
					deathscript.playerDeath = true;
                    if (desiredCoords.ContainsKey(player) && !desiredCoords[player].Equals(player.GetCoords()) || m.type == BoardCodes.PLAYER || other.type == BoardCodes.PLAYER) {
                        collided.Add(m, 2);
                        collided.Add(other, 2);
                    }
				}
			}

			// Check for collisions moving through each other
			foreach (MoveableScript other in moveables) {
                if (other == null) {
                    throw new System.NullReferenceException("One Entity in moveables is null!");
                } else if (other == m) {
					continue;
				} else if ((desiredCoords.ContainsKey (other) && desiredCoords[other].Equals(m.GetCoords()) && desired.Equals(other.GetCoords())) // classic close collision, other wants my space, I want other's space
                    || (desiredCoords.ContainsKey(other) && desiredCoords[other].Equals(m.GetCoords()) && blockingCoords.ContainsKey(m) && blockingCoords[m].Equals(other.GetCoords())) // other wants my current space, other is in my blocker space
                    || (blockingCoords.ContainsKey(other) && blockingCoords[other].Equals(m.GetCoords()) && desiredCoords[m].Equals(other.GetCoords()))) { // other is blocked by me, I want other's space
					moveDirections [other] = Direction.NONE;
					moveDirections [m] = Direction.NONE;
				}
			}

			if (!moveDirections.ContainsKey(m)) {
				moveDirections.Add(m, m.GetAttemptedMoveDirection(dir, boardState.board)); 
			}

        }  

		// Check for collisions moving into pieces that couldn't move
		for (int i = 0; i < moveables.Count; i++) {
			foreach (MoveableScript m in moveables) {
				foreach (MoveableScript other in moveables) {
					if (other == m || other == null) {
						continue;
					} else if (moveDirections [other] == Direction.NONE
					          && (other.GetCoords ().Equals (desiredCoords [m]) || (blockingCoords.ContainsKey (m) && other.GetCoords ().Equals (blockingCoords [m])))) {
						moveDirections [m] = Direction.NONE;
						collided [m] = 1;
						collided [other] = 1;
						//Debug.Log ("Couldn't move" + ":" + other.GetCoords ());
					} else {
						//Debug.Log (moveDirections [other] + ":" + other.GetCoords ());
					}
				}
			}
		}
   
        // make the moves
		if (moveDirections [player] != Direction.NONE) {
            Stack<MoveableScript> toDestroy = new Stack<MoveableScript>();
            foreach (MoveableScript moveable in moveDirections.Keys) {
				moveable.ExecuteMove (moveDirections[moveable], 1, false);
				if (moveDirections [moveable] == Direction.NONE) {
					AudioManagerScript.instance.soundFx.PlayOneShot (moveable.collideSound, .5f);
				}

                // check for a swap
                coord c = moveable.GetCoords();
                if (moveDirections[moveable] != Direction.NONE && swapCoords.Contains(c)) {
                    needsSwap.Add(moveable); 
                } else if (moveDirections[moveable] != Direction.NONE && portalCoords.Contains(c)) {
                    coord cp = portalMap[c];
                    // overrides any existing moves
					moveable.EnterPortal(boardState.board, cp);
                }

                //check for button press 
				if (boardState.board[c.row, c.col] >= 30 && boardState.board[c.row, c.col] < 40) {
                    coord buttonCoord = new coord(c.row, c.col);
                    buttonsPressed.Add(buttonCoords[buttonCoord]);
                }
			} 

            moveDirections.Clear();
            // destroy old objects
            int j = toDestroy.Count;
            for (int i = 0; i < j; i++) {
                MoveableScript ms = toDestroy.Pop();
                GameObject.Destroy(ms.gameObject);
            }
		} else {
            if (!collided.ContainsKey(player)) {
                collided.Add(player, 1);
            }
        }

		foreach (MoveableScript m in collided.Keys) {
            m.ExecuteMove(Direction.NONE, collided[m], false);
			AudioManagerScript.instance.soundFx.PlayOneShot (m.collideSound, .5f);
		}

		recordDynamicState ();	
    }

    private void PerformSwap(MoveableScript moveable) {
		moveable.startSpin (270);
    }

	private void CheckSwaps () {

		List<MoveableScript> toDelete = new List<MoveableScript> ();
		List<MoveableScript> toAdd = new List<MoveableScript> ();
		foreach (MoveableScript moveable in moveables) {
			Direction dr = moveable.direction;
			int row = moveable.GetCoords().row;
			int col = moveable.GetCoords().col; 
			if (moveable.shouldSwap && moveable.type == BoardCodes.MIMIC) {
                inputReady = false;
				boardState.board [row, col] = 24;
				// Instantiate a Mirror
				MirrorScript m = GameObject.Instantiate (mirror);
				m.SetCoords (col, row);
                m.transform.Rotate(0f, -90f, 0f);
                m.startSpin(90, false);
				toAdd.Add (m);
				toDelete.Add (moveable);
			} else if (moveable.shouldSwap && moveable.type == BoardCodes.MIRROR) {
                inputReady = false;
                boardState.board [row, col] = 23;
				// Instantiate a Mimic
				MimicScript m = GameObject.Instantiate (mimic);
				m.SetCoords (col, row);
                m.transform.Rotate(0f, -90f, 0f);
                m.startSpin(90, false);
                toAdd.Add (m);
				toDelete.Add (moveable);
			}
		}

		foreach (MoveableScript moveable in toDelete) {
			this.moveables.Remove (moveable);
			GameObject.Destroy (moveable.gameObject);
		}

		foreach (MoveableScript m in toAdd) {
			int col = m.GetCoords ().col;
			int row = m.GetCoords ().row;
			m.transform.position = new Vector3 (col + mapOrigin.x, row + mapOrigin.y + m.yOffset, 0);
			moveables.Add (m);
		}
	}

	private void recordDynamicState() {
		DynamicState ds = new DynamicState ();
		foreach (MoveableScript m in moveables) {
			switch (m.type) {
			case BoardCodes.PLAYER:
				ds.playerPosition = m.GetCoords ();
				break;
			case BoardCodes.MIMIC:
				ds.mimicPositions.Add (m.GetCoords ());
				break;
			case BoardCodes.MIRROR:
				ds.mirrorPositions.Add (m.GetCoords ());
				break;
			}
		}

		foreach (coord b in buttonCoords.Keys) {
			if (buttonCoords[b].laser.data.isActive) {
				ds.activeButtons.Add(b);
			}
		}

		dynamicStateStack.Push (ds);
		//Debug.Log (ds.toJson ());
		LoggingManager.instance.RecordEvent (LoggingManager.EventCodes.DYNAMIC_STATE, ds.toJson ());
	}
}
