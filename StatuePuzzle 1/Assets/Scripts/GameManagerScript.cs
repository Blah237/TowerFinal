using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

public class GameManagerScript : MonoBehaviour {

	List<MoveableScript> moveables = new List<MoveableScript>();
    public PlayerScript player;
    public MimicScript mimic;
    public MirrorScript mirror;
    public GameObject wall;
    public GameObject goal;
    public GameObject laser; 
	
	public WinScript winscript;
	public DeathScript deathscript;
	
	public GameObject ground;
	public Camera mainCamera; 

	public bool win;
	public bool dead;
    
    List<coord> goalCoords = new List<coord>();
    List<Laser> laserList = new List<Laser>(); 

    public GameObject swap;
    List<coord> swapCoords = new List<coord>();


    public Vector2 mapOrigin;

    [SerializeField]
    public static bool inputReady = true;
    Direction? inputDir;
    
	Stack<int[,]> boardStates; //TODO: refactor so that this is a stack of boardcode arrays

    [SerializeField]
    public static string levelName; 
    Level boardState; //Row, Column
    // East col+, North row+

    int[] moveableTypes = new int[] {2,3,4}; 
    /* 0 = empty
     * 1 = wall
     * 2 = player
     * 3 = mimic
     * 4 = mirror
     * 10 = goal
     */

    // Use this for initialization
    void Start() {
        //load level using Melody's I/O
        boardState = IOScript.ParseLevel(levelName); 
        boardStates = new Stack<int[,]> ();
		boardStates.Push (boardState.board);

        mapOrigin = new Vector2(-boardState.cols / 2, -boardState.rows / 2);
        int dim = boardState.rows > boardState.cols ? boardState.rows : boardState.cols;
        mainCamera.transform.position = new Vector3(0, 0, -(dim / 2) / Mathf.Tan(Mathf.PI / 6)); 
        
        //instantiate items based on board
        for(int i = 0; i < boardState.rows; i++) {
            for(int j = 0; j < boardState.cols; j++) {
                if (boardState.board[i, j] == 1) {
                    GameObject w = GameObject.Instantiate(wall);
                    w.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                } else if (boardState.board[i, j] >= 10 && boardState.board[i, j] < 20) {
                    // goal
                    GameObject c = GameObject.Instantiate(goal);
                    c.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    goalCoords.Add(new coord(i, j));
                } else if (boardState.board[i, j] >= 20 && boardState.board[i, j] < 30) {
                    // swap
                    GameObject c = GameObject.Instantiate(swap);
                    c.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    swapCoords.Add(new coord(i, j));
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

        //instantiate lasers based on parsed lasers
        if (boardState.lasers != null) {
            foreach (Laser la in boardState.lasers) {
                GameObject l = GameObject.Instantiate(laser);
                l.transform.position = new Vector3(la.startCol + mapOrigin.x - 0.5f, la.startRow + mapOrigin.y + 0.5f, -0.1f);
                l.transform.localScale = new Vector3(1, 1, la.length);
                //TODO: implement button control of laser based on ID of laser
                int rotateDir = la.direction == Direction.NORTH ? -90 : la.direction == Direction.SOUTH ? 90 : la.direction == Direction.EAST ? 0 : 180;
                l.transform.Rotate(rotateDir, 90, 0, Space.World);
                if (la.state == 0) {
                    l.SetActive(false);
                }
                laserList.Add(la); 
            }
        }
	}

	// Update is called once per frame
	void Update () {
        if (inputReady) {
            Direction dir = readInput();
            if(dir != Direction.NONE) {
                inputReady = false;
                move(dir);
            }
            checkWin();
        } else {
            inputReady = getAllDone();
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

    Direction readInput() {
        if(Input.GetAxis("Horizontal") >= 1f) {
            return Direction.EAST;
        } else if(Input.GetAxis("Horizontal") <= -1f) {
            return Direction.WEST;
        } else if(Input.GetAxis("Vertical") >= 1f) {
            return Direction.NORTH;
        } else if(Input.GetAxis("Vertical") <= -1f) {
            return Direction.SOUTH;
        } else {
            return Direction.NONE;
        }
    }

    bool checkWin() {
        foreach (coord c in goalCoords) {
			if (boardStates.Peek()[c.row, c.col] !=  13 && boardStates.Peek()[c.row, c.col] != 14) { //FIVE IS CURRENT COMBO MAX
				//Debug.Log(boardStates.Peek()[c.row,c.col]);
                return false;
            }
        }
        //Debug.Log("VICTORY!");
	    winscript.playerWin = true;
        return true;
    }



	public static void setLevelName(string level) {
		levelName = level;
	}

    void move(Direction dir) {

		int[,] boardState = boardStates.Peek();
		int[,] nextState = new int[boardState.GetLength (0), boardState.GetLength (1)];
		System.Array.Copy (boardState, nextState, boardState.Length);

		Dictionary<MoveableScript,coord> goalCoordMap = new Dictionary<MoveableScript, coord>();
		Dictionary<MoveableScript,Direction> moveDirections = new Dictionary<MoveableScript, Direction>();

		foreach(MoveableScript m in moveables) {
			coord goal = m.GetAttemptedMoveCoords(dir, boardState, 1);
            Direction direction = m.GetAttemptedMoveDirection(dir, boardState); 
			goalCoordMap.Add(m,goal);

            //Check for collisions with lasers 
            foreach (Laser laser in laserList) {
                // if laser is active && laser can collide with this moveable 
                if (laser.state == 1 && (laser.canCollide & m.collisionMask) > 0) {
                    //if moveable is jumping through a horizontal laser
                    if ((direction == Direction.NORTH && m.GetCoords().row == laser.startRow) ||
                       (direction == Direction.SOUTH && goal.row == laser.startRow)) {
                        if (laser.isBetweenCol(m.GetCoords().col)) {
                            moveDirections[m] = Direction.NONE;
                        }
                    }
                    //if moveable is jumping through a vertical laser 
                    if ((direction == Direction.EAST && goal.col == laser.startCol) ||
                        (direction == Direction.WEST && m.GetCoords().col == laser.startCol)) {
                        if (laser.isBetweenRow(m.GetCoords().row)) {
                            moveDirections[m] = Direction.NONE;
                        }
                    }
                }
            }

            if (!moveDirections.ContainsKey(m)) {
			    // Check for collisions moving into the same spot
			    foreach (MoveableScript other in moveables) {
				    if (other == m || other == null) {
					    continue;
				    //TODO: For these else-ifs we need equals operator for coords, but I'm on a plane without
				    //wifi and can't look up how C# operator overloading works :( -Reid

				    //Check for moving into same spot
				    } else if (goalCoordMap.ContainsKey(other) && goalCoordMap[other].row == goal.row && goalCoordMap[other].col == goal.col) {
					    Debug.Log ("Collision at " + goal.row + " " + goal.col + ": " + m.name + " " + other.name);
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
				    } else if (goalCoordMap.ContainsKey (other)
				               && goalCoordMap[other].row == m.GetCoords ().row
				               && goalCoordMap[other].col == m.GetCoords ().col
				               && goal.row == other.GetCoords ().row
				               && goal.col == other.GetCoords ().col) {
					    Debug.Log ("Move Through: " + m.name + " " + other.name);
					    moveDirections [other] = Direction.NONE;
					    moveDirections [m] = Direction.NONE;
				    }
			    }
            }

            if (!moveDirections.ContainsKey(m)) {
				moveDirections.Add(m, m.GetAttemptedMoveDirection(dir, boardState)); 
			}

        }  

		// Check for collisions moving into pieces that couldn't move
		foreach (MoveableScript m in moveables) {
			foreach (MoveableScript other in moveables) {
				if (other == m || other == null) {
					continue;
				} else if (moveDirections[other] == Direction.NONE
					&& other.GetCoords().row == goalCoordMap[m].row 
					&& other.GetCoords().col == goalCoordMap[m].col) {
					moveDirections [m] = Direction.NONE;
				}
			}
		}

        // make the moves
		if (moveDirections [player] != Direction.NONE) {
            Stack<MoveableScript> toDestroy = new Stack<MoveableScript>();
            foreach (MoveableScript moveable in moveDirections.Keys) {
				//Debug.Log (moveable.name + " " + moveDirections[moveable].ToString() + " after " + moveable.GetAttemptedMoveDirection(dir, boardState));
				moveable.ExecuteMove (moveDirections[moveable], nextState, 1);

                // check for a swap
                coord c = moveable.GetCoords();
                if (moveDirections[moveable] != Direction.NONE && swapCoords.Contains(c)) {
                    if (nextState[c.row, c.col] == 23) {
                        Direction dr = moveDirections[moveable];
                        int dx = dr == Direction.EAST ? 1 : dr == Direction.WEST ? -1 : 0;
                        int dy = dr == Direction.NORTH ? 1 : dr == Direction.SOUTH ? -1 : 0;
                        nextState[c.row, c.col] = 24;
                        this.moveables.Remove(moveable);
                        toDestroy.Push(moveable);
                        // Instantiate a Mirror
                        MirrorScript m = GameObject.Instantiate(mirror);
                        m.SetCoords(c.col-dx, c.row-dy);
                        m.transform.position = new Vector3(c.col-dx + mapOrigin.x, c.row-dy + mapOrigin.y, 0);
                        moveables.Add(m);
                        m.ExecuteMove(dr, nextState, 1, true);
                    } else if (nextState[c.row, c.col] == 24) {
                        Direction dr = moveDirections[moveable];
                        int dx = dr == Direction.EAST ? 1 : dr == Direction.WEST ? -1 : 0;
                        int dy = dr == Direction.NORTH ? 1 : dr == Direction.SOUTH ? -1 : 0;
                        nextState[c.row, c.col] = 23;
                        this.moveables.Remove(moveable);
                        toDestroy.Push(moveable);
                        // Instantiate a Mimic
                        MimicScript m = GameObject.Instantiate(mimic);
                        m.SetCoords(c.col-dx, c.row-dy);
                        m.transform.position = new Vector3(c.col-dx + mapOrigin.x, c.row-dy + mapOrigin.y, 0);
                        moveables.Add(m);
                        m.ExecuteMove(dr, nextState, 1, true);
                    }
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

		boardStates.Push (nextState);
    }


}
