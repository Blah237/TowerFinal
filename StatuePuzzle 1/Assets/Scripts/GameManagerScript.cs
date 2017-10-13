using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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
    public GameObject goal;
    public GameObject laser; 
	
	public WinScript winscript;
	public DeathScript deathscript;
	public PauseScript pausescript;

	public ButtonToggleScript button;
	
	public GameObject ground;
	public Camera mainCamera; 

	public bool win;
	public bool dead;
    
    List<coord> goalCoords = new List<coord>();
    List<Laser> laserList = new List<Laser>(); 

    public GameObject swap;
    public GameObject portal;

    List<coord> swapCoords = new List<coord>();
	List<coord> buttonCoords = new List<coord>();
    List<coord> portalCoords = new List<coord>();
    Dictionary<coord, coord> portalMap = new Dictionary<coord, coord>();

    public static Vector2 mapOrigin;

	bool firstStart;

    [SerializeField]
    public static bool inputReady = true;
	public static bool pauseReady = true;
    Direction? inputDir;
    
    [SerializeField]
    public static string levelName; 
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
		int levelNum = -1;
		for (int i = 0; i < levelName.Length; i++) {
			if (Int32.TryParse (levelName.Substring (i, 1), out levelNum)) {
				break;
			}
		}
		LoggingManager.instance.RecordLevelStart (levelNum, levelName);

        mapOrigin = new Vector2(-boardState.cols / 2, -boardState.rows / 2);
        int dim = boardState.rows > boardState.cols ? boardState.rows : boardState.cols;
        mainCamera.transform.position = new Vector3(0, 0, -(dim / 2) / Mathf.Tan(Mathf.PI / 6));
        mainCamera.orthographicSize = boardState.rows / 2 + 1;

        int buttonCount = 0;

        //instantiate lasers based on parsed lasers
        if (boardState.lasers != null) {
            foreach (Laser la in boardState.lasers) {
                GameObject l = GameObject.Instantiate(laser);
                l.transform.position = new Vector3(la.startCol + mapOrigin.x - 0.5f, la.startRow + mapOrigin.y + 0.5f, -0.1f);
                l.transform.localScale = new Vector3(1, 1, la.length);
                int rotateDir = la.direction == Direction.NORTH ? -90 : la.direction == Direction.SOUTH ? 90 : la.direction == Direction.EAST ? 0 : 180;
                l.transform.Rotate(rotateDir, 90, 0, Space.World);
                la.gameObject = l; 
                if (la.state == 0) {
                    l.SetActive(false);
                }
                laserList.Add(la);
            }
        }

        //instantiate items based on board
        for (int i = 0; i < boardState.rows; i++) {
            for(int j = 0; j < boardState.cols; j++) {
                if (boardState.board[i, j] == 1) {
                    GameObject w; 
                    if(i == 0 || boardState.board[i-1,j] != 1) {
                        w = GameObject.Instantiate(frontWall); 
                    } else {
                        w = GameObject.Instantiate(wall); 
                    }
                    w.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                } else if (boardState.board[i, j] >= 10 && boardState.board[i, j] < 20) {
                    // goal
                    GameObject c = GameObject.Instantiate(goal);
                    c.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    goalCoords.Add(new coord(i, j));
                } else if (boardState.board[i, j] >= 20 && boardState.board[i, j] < 30)
	            {
		            // swap
		            GameObject c = GameObject.Instantiate(swap);
		            c.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
		            swapCoords.Add(new coord(i, j));
	            } else if (boardState.board[i,j] >= 30 && boardState.board[i,j] < 40) {
	                // button
	                ButtonToggleScript c = GameObject.Instantiate(button);
	                c.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    c.laser = laserList[boardState.buttons[buttonCount]];
                    c.InitButton(); 
	                buttonCoords.Add(new coord(i,j));
	                ButtonManagerScript.buttonCoords.Add(new coord(i,j), c);
                    buttonCount++; 
                } else if (boardState.board[i, j] >= 50 && boardState.board[i, j] < 60) {
                    // portal
                    GameObject p = GameObject.Instantiate(portal);
                    p.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    portalCoords.Add(new coord(i, j));
                } else {
                    GameObject g = GameObject.Instantiate(ground);
                    g.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                }

                if (boardState.board[i, j] % 10 == 2) {
                    player = GameObject.Instantiate(player);
                    player.SetCoords(j, i); 
                    player.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    moveables.Add(player); 
                } else if (boardState.board[i,j] % 10 == 3) {
                    MimicScript m = GameObject.Instantiate(mimic);
                    m.SetCoords(j, i);
                    m.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    moveables.Add(m); 
                } else if (boardState.board[i,j] % 10 == 4) {
                    MirrorScript m = GameObject.Instantiate(mirror);
                    m.SetCoords(j, i);
                    m.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
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

	// Update is called once per frame
	void Update () {

		// TODO: Hacky, but neccessary if we want accurate recording and not a real loading screen
		if (!firstStart) {
			recordDynamicState ();
			firstStart = true;
		}

		if (inputReady) {
			Direction dir = readInput();
			if (dir != Direction.NONE)
			{
				inputReady = false;
				move(dir);
			}
		} else {
			inputReady = getAllDone();
		}
		if (pauseReady && checkPause())
		{
			pausescript.TogglePause();
		}
	}

    bool getAllDone() {
        foreach (MoveableScript m in moveables) {
            if (m.GetIsMoving()) {
                return false;
            }
        }
        return true;
    }

	private void undo() {
		// TODO: Record an undo with logging as an event
		try {
			dynamicStateStack.Pop();
			DynamicState ds = dynamicStateStack.Peek();
			LoggingManager.instance.RecordEvent(LoggingManager.EventCodes.UNDO,ds.toJson());
			int mimicIdx = 0;
			int mirrorIdx = 0;
			List<coord> mimicCoords = ds.mimicPositions.ToList ();
			List<coord> mirrorCoords = ds.mirrorPositions.ToList ();
			foreach (MoveableScript m in moveables) {
				coord prevCoord = m.GetCoords();
				coord undoCoord;
				switch (m.type) {
				case BoardCodes.PLAYER:
					undoCoord = ds.playerPosition;
					m.SetCoords (undoCoord);
					break;
				case BoardCodes.MIMIC:
					undoCoord = mimicCoords [mimicIdx];
					m.SetCoords (undoCoord);
					mimicIdx++;
					break;
				case BoardCodes.MIRROR:
					undoCoord = mirrorCoords [mirrorIdx];
					m.SetCoords (undoCoord);
					mirrorIdx++;
					break;
				default:
					undoCoord = new coord(0,0);
					break;
				}

				if (ButtonManagerScript.buttonCoords.ContainsKey(undoCoord)) {
					ButtonManagerScript.buttonCoords[undoCoord].TogglePressed();
				} else if (ButtonManagerScript.buttonCoords.ContainsKey(prevCoord)) {
					ButtonManagerScript.buttonCoords[prevCoord].TogglePressed();
				}
				m.transform.position = new Vector3(m.GetCoords().col + mapOrigin.x, m.GetCoords().row + mapOrigin.y, 0);
			} 

		} catch (InvalidOperationException) {
			//TODO: Actually display this to the user 
			Debug.Log ("Empty stack, no previous move to undo.");
		}
	}

    Direction readInput() {
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

        foreach (coord c in goalCoords) {
			if (!nppCoords.Contains (c)) {
				return false;
			}
        }
        Debug.Log("VICTORY!");
		LoggingManager.instance.RecordEvent (LoggingManager.EventCodes.LEVEL_COMPLETE);
		LoggingManager.instance.RecordLevelEnd ();
	    winscript.playerWin = true;
        return true;
    }

	bool checkPause()
	{
		return Input.GetKeyDown(KeyCode.P);
	}
		
	public static void setLevelName(string level) {
		levelName = level;
	}

    void move(Direction dir) {

		Dictionary<MoveableScript,coord> desiredCoords = new Dictionary<MoveableScript, coord>(); //where pieces would move without collisions/walls
		Dictionary<MoveableScript,Direction> moveDirections = new Dictionary<MoveableScript, Direction>(); //directions pieces will actually move

		foreach(MoveableScript m in moveables) {
			coord desired = m.GetAttemptedMoveCoords(dir, boardState.board, 1);
			if (boardState.board[desired.row, desired.col] / 10 == 5) {
                desired = portalMap[desired];
            }
            desiredCoords.Add(m,desired);
            
			Direction direction = m.GetAttemptedMoveDirection(dir, boardState.board); 
            //Check for collisions with lasers 
            foreach (Laser laser in laserList) {
                // if laser is active && laser can collide with this moveable 
                if (laser.gameObject.activeInHierarchy && (laser.canCollide & m.collisionMask) > 0) {
	                //if moveable is jumping through a horizontal laser
                    if ((direction == Direction.NORTH && m.GetCoords().row == laser.startRow) ||
		                (direction == Direction.SOUTH && desired.row == laser.startRow)) {
		                if (laser.isBetweenCol(m.GetCoords().col)) {
			                moveDirections[m] = Direction.NONE;
                            desired = m.GetCoords();
                            desiredCoords[m] = desired;
                        }
		            }
                    //if moveable is jumping through a vertical laser 
                    if ((direction == Direction.EAST && desired.col == laser.startCol) ||
	                    (direction == Direction.WEST && m.GetCoords().col == laser.startCol)) {
	                    if (laser.isBetweenRow(m.GetCoords().row)) {
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
				} else if (desiredCoords.ContainsKey(other) && desiredCoords[other].Equals(desired)) {
					moveDirections[other] = Direction.NONE;
					moveDirections[m] = Direction.NONE;
					dead = true;
					deathscript.playerDeath = true;
				}
			}

			// Check for collisions moving through each other
			foreach (MoveableScript other in moveables) {
                if (other == null) {
                    throw new System.NullReferenceException("One Entity in moveables is null!");
                } else if (other == m) {
					continue;
				} else if (desiredCoords.ContainsKey (other) && desiredCoords[other].Equals(m.GetCoords()) && desired.Equals(other.GetCoords())) {
					moveDirections [other] = Direction.NONE;
					moveDirections [m] = Direction.NONE;
				}
			}

			if (!moveDirections.ContainsKey(m)) {
				moveDirections.Add(m, m.GetAttemptedMoveDirection(dir, boardState.board)); 
			}

        }  

		// Check for collisions moving into pieces that couldn't move
		foreach (MoveableScript m in moveables) {
			foreach (MoveableScript other in moveables) {
				if (other == m || other == null) {
					continue;
				} else if (moveDirections[other] == Direction.NONE
					&& other.GetCoords().Equals(desiredCoords[m])) {
					moveDirections [m] = Direction.NONE;
				}
			}
		}

        // make the moves
		if (moveDirections [player] != Direction.NONE) {
            Stack<MoveableScript> toDestroy = new Stack<MoveableScript>();
            foreach (MoveableScript moveable in moveDirections.Keys) {
				//Debug.Log (moveable.name + " " + moveDirections[moveable].ToString() + " after " + moveable.GetAttemptedMoveDirection(dir, boardState));
				moveable.ExecuteMove (moveDirections[moveable], 1, false);

                // check for a swap
                coord c = moveable.GetCoords();
                if (moveDirections[moveable] != Direction.NONE && swapCoords.Contains(c)) {
					if (moveable.type == BoardCodes.MIMIC) {
                        Direction dr = moveDirections[moveable];
                        int dx = dr == Direction.EAST ? 1 : dr == Direction.WEST ? -1 : 0;
                        int dy = dr == Direction.NORTH ? 1 : dr == Direction.SOUTH ? -1 : 0;
                        boardState.board[c.row, c.col] = 24;
                        this.moveables.Remove(moveable);
                        toDestroy.Push(moveable);
                        // Instantiate a Mirror
                        MirrorScript m = GameObject.Instantiate(mirror);
                        m.SetCoords(c.col-dx, c.row-dy);
                        m.transform.position = new Vector3(c.col-dx + mapOrigin.x, c.row-dy + mapOrigin.y, 0);
                        moveables.Add(m);
                        m.ExecuteMove(dr, 1, true);
					} else if (moveable.type == BoardCodes.MIRROR) {
                        Direction dr = moveDirections[moveable];
                        int dx = dr == Direction.EAST ? 1 : dr == Direction.WEST ? -1 : 0;
                        int dy = dr == Direction.NORTH ? 1 : dr == Direction.SOUTH ? -1 : 0;
                        boardState.board[c.row, c.col] = 23;
                        this.moveables.Remove(moveable);
                        toDestroy.Push(moveable);
                        // Instantiate a Mimic
                        MimicScript m = GameObject.Instantiate(mimic);
                        m.SetCoords(c.col-dx, c.row-dy);
                        m.transform.position = new Vector3(c.col-dx + mapOrigin.x, c.row-dy + mapOrigin.y, 0);
                        moveables.Add(m);
                        m.ExecuteMove(dr, 1, true);
                    }
                } else if (moveDirections[moveable] != Direction.NONE && portalCoords.Contains(c)) {
                    coord cp = portalMap[c];
                    // overrides any existing moves
					moveable.EnterPortal(boardState.board, cp);
                }

                //check for button press 
				if (boardState.board[c.row, c.col] >= 30 && boardState.board[c.row, c.col] < 40) {
                    coord buttonCoord = new coord(c.row, c.col);
                    ButtonManagerScript.buttonCoords[buttonCoord].TogglePressed();
                }
            }

            moveDirections.Clear();
            // destroy old objects
            int j = toDestroy.Count;
            for (int i = 0; i < j; i++) {
                MoveableScript ms = toDestroy.Pop();
                GameObject.Destroy(ms.gameObject);
            }
		}

		recordDynamicState ();	
		checkWin ();
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

		foreach (KeyValuePair<coord,ButtonToggleScript> b in ButtonManagerScript.buttonCoords) {
			//TODO: Would really like if laser.state was a bool
			ds.buttonStates.Add (b.Key, b.Value.laser.state);
		}

		dynamicStateStack.Push (ds);
		Debug.Log (ds.toJson ());
		LoggingManager.instance.RecordEvent (LoggingManager.EventCodes.DYNAMIC_STATE, ds.toJson ());
	}
}
