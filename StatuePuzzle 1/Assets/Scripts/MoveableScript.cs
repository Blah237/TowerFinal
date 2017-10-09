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

	public void ExecuteMove(Direction direction, int numSpaces, bool animOnly = false) {

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
    }
}
