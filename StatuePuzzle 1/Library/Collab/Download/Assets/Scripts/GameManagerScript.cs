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

    [SerializeField]
	List<MoveableScript> moveables;
    [SerializeField]
    PlayerScript player;

    [SerializeField]
    List<coord> goalCoords;

    public Vector2 mapOrigin;

    [SerializeField]
    bool inputReady = true;
    Direction? inputDir;

    [SerializeField]
    int rows = 8;
    [SerializeField]
    int cols = 7;
    [SerializeField]
	Stack<int[,]> boardStates; //TODO: refactor so that this is a stack of boardcode arrays

    int[,] boardState; //Row, Column
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
		//This is where it should be loaded using Melody's I/O
        boardState = new int[8, 7] { { 0, 0, 0, 1, 0, 0, 0 }, { 0, 0, 1, 3, 1, 0, 0 }, { 0, 1, 0, 0, 0, 1, 0 }, { 0, 1, 0, 2, 0, 1, 0 }, { 1, 0, 0, 0, 0, 0, 1 }, { 1, 0, 10, 4, 10, 0, 1 }, { 1, 0, 0, 1, 0, 0, 1 }, { 0, 1, 1, 0, 1, 1, 0 }};
		boardStates = new Stack<int[,]> ();
		boardStates.Push (boardState);
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
			if (boardState[c.row, c.col] <= 5 ) { //FIVE IS CURRENT COMBO MAX
                return false;
            }
        }
        Debug.Log("VICTORY!");
        return true;
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
					Debug.Log ("Collision: " + m.name + " " + other.name);
					moveDirections[other] = Direction.NONE;
					moveDirections[m] = Direction.NONE;
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
