using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveableScript : MonoBehaviour {

	[SerializeField]
	private bool isMoving; 
	public float speed = 1f; 
	[SerializeField]
	private Direction direction;
	[SerializeField]
	private float distanceToMove; 
	//current statue position
	[SerializeField]
	protected coord coords;

    public int collisionMask; 
	public BoardCodes type { get; protected set;}

	// Use this for initialization
	void Start () {
		InitializeType ();
	}

	protected abstract void InitializeType ();

	public bool GetIsMoving() {
		return isMoving; 
	}

	public coord GetCoords() {
		return coords;
	}

	public void SetCoords(int col, int row) {
		coords.col = col;
		coords.row = row;
	}

	private void Update() {
		if(isMoving) {
			float dt = Time.deltaTime; 
			int y = direction == Direction.NORTH ? 1 : (direction == Direction.SOUTH ? -1 : 0);
			int x = direction == Direction.EAST ? 1 : (direction == Direction.WEST ? -1 : 0);

			float distance = dt * speed;
			distanceToMove -= distance; 
			if (distanceToMove <= 0) {
				isMoving = false;
				distance += distanceToMove;
				distanceToMove = 0; 
			}
			transform.Translate(new Vector3(x * distance, y * distance, 0), Space.World);       
		}
	}

	//TODO: I think Unity is encouraging bad design here, but models should NOT be getting boardsate
	public abstract coord GetAttemptedMoveCoords (Direction direction, int[,] boardState, int numSpaces);

	public abstract Direction GetAttemptedMoveDirection (Direction direction, int[,] boardState);

	public void ExecuteMove(Direction direction, int[,] boardState, int numSpaces, bool animOnly = false) {

		//TODO: Make this assert more robust so it doesn't just check every overlap possibility
		//(currently the only overlap possibility is a goal)
		//TODO: Actually going to comment this assert out entirely temporarily, another thing we need
		//to add for robustness is that when one piece moves into a space that another piece was just in,
		//the sum is temporarily off until it gets subtracted by the later moving piece, which triggers
		//the assert. 

//		Debug.Assert(boardState[coords.row,coords.col] == (int) type ||
//			boardState[coords.row,coords.col] > 5,
//			"Expected " + coords.ToString() + " to be " + (int) type + " but was " + boardState[coords.row,coords.col]);

		//TODO: animate 

		//translate according to directions 
		isMoving = true;
		distanceToMove = numSpaces;
		this.direction = direction;
		//Debug.Log(this.name + " moving " + direction.ToString());

		//update now empty space
		coord oldCoords = coords;

		//change statue position
		switch (direction) {
		case Direction.NORTH:
			coords.row += numSpaces;
			break;
		case Direction.SOUTH:
			coords.row -= numSpaces;
			break;
		case Direction.EAST:
			coords.col += numSpaces;
			break;
		case Direction.WEST:
			coords.col -= numSpaces;
			break;
		}

        //update board
        if (!animOnly) {
            boardState[oldCoords.row, oldCoords.col] = boardState[oldCoords.row, oldCoords.col] - (int)type;
            boardState[coords.row, coords.col] = (int)type + boardState[coords.row, coords.col];
        }
    }
}
