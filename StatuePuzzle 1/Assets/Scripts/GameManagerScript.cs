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
	
	public WinScript winscript;
	public DeathScript deathscript;

	public bool win;
	public bool dead;
    
    List<coord> goalCoords = new List<coord>();

    public Vector2 mapOrigin;

    [SerializeField]
    bool inputReady = true;
    Direction? inputDir;
    
	Stack<int[,]> boardStates; //TODO: refactor so that this is a stack of boardcode arrays

    [SerializeField]
    string levelName; 
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

        //instantiate items based on board
        for(int i = 0; i < boardState.rows; i++) {
            for(int j = 0; j < boardState.cols; j++) {
                if(boardState.board[i, j] == 2 || boardState.board[i,j] == 12) {
                    player = GameObject.Instantiate(player);
                    player.SetCoords(j, i); 
                    player.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    moveables.Add(player); 
                } else if (boardState.board[i,j] == 3 || boardState.board[i, j] == 13) {
                    MimicScript m = GameObject.Instantiate(mimic);
                    m.SetCoords(j, i);
                    m.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    moveables.Add(m); 
                } else if (boardState.board[i,j] == 4 || boardState.board[i, j] == 14) {
                    MirrorScript m = GameObject.Instantiate(mirror);
                    m.SetCoords(j, i);
                    m.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    moveables.Add(m);
                } else if (boardState.board[i,j] == 1) {
                    GameObject w = GameObject.Instantiate(wall);
                    w.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0); 
                }
                if (boardState.board[i,j] >= 10) {
                    GameObject c = GameObject.Instantiate(goal);
                    c.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0); 
                    goalCoords.Add(new coord(i, j)); 
                }
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
	        Debug.Log(boardState.board[c.row,c.col]);
			if (boardState.board[c.row, c.col] !=  13 || boardState.board[c.row, c.col] != 14) { //FIVE IS CURRENT COMBO MAX
                return false;
            }
        }
        //Debug.Log("VICTORY!");
	    winscript.playerWin = true;
        return true;
    }

	void setLevelName(string level) {
		levelName = level;
	}

    void move(Direction dir) {

		int[,] boardState = boardStates.Peek();
		int[,] nextState = new int[boardState.GetLength (0), boardState.GetLength (1)];
		System.Array.Copy (boardState, nextState, boardState.Length);

		Dictionary<MoveableScript,coord> goalCoords = new Dictionary<MoveableScript, coord>();
		Dictionary<MoveableScript,Direction> moveDirections = new Dictionary<MoveableScript, Direction>();

		foreach(MoveableScript m in moveables) {
			coord goal = m.GetAttemptedMoveCoords(dir, boardState, 1);
			goalCoords.Add(m,goal);

			// Check for collisions moving into the same spot
			foreach (MoveableScript other in moveables) {
				if (other == m || other == null) {
					continue;
				//TODO: For these else-ifs we need equals operator for coords, but I'm on a plane without
				//wifi and can't look up how C# operator overloading works :( -Reid

				//Check for moving into same spot
				} else if (goalCoords.ContainsKey(other) && goalCoords[other].row == goal.row && goalCoords[other].col == goal.col) {
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
				} else if (goalCoords.ContainsKey (other)
				           && goalCoords [other].row == m.GetCoords ().row
				           && goalCoords [other].col == m.GetCoords ().col
				           && goal.row == other.GetCoords ().row
				           && goal.col == other.GetCoords ().col) {
					Debug.Log ("Move Through: " + m.name + " " + other.name);
					moveDirections [other] = Direction.NONE;
					moveDirections [m] = Direction.NONE;
				}
			}

			if (!moveDirections.ContainsKey(m)) {
				moveDirections.Add(m, m.GetAttemptedMoveDirection(dir, boardState)); 
			}

        }  

		if (moveDirections [player] != Direction.NONE) {
			foreach (MoveableScript moveable in moveDirections.Keys) {
				//Debug.Log (moveable.name + " " + moveDirections[moveable].ToString() + " after " + moveable.GetAttemptedMoveDirection(dir, boardState));
				moveable.ExecuteMove (moveDirections[moveable], nextState, 1);
			}
		}

		boardStates.Push (nextState);
    }


}
