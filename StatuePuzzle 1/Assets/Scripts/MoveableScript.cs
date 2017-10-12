using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveableScript : MonoBehaviour {

    [SerializeField]
    private bool isMoving;
    [SerializeField]
    private bool isColliding;
    public float speed = 1f;
    public float cSpeed = 0.7f;
    public float collideFactor; 
    [SerializeField]
    protected Direction direction;
    [SerializeField]
    private float distanceToMove;
    private float totalDistance;
    //current statue position
    [SerializeField]
    protected coord coords;
    [SerializeField]
    public float yOffset; 

    protected Animation2DManager animator;

    public int collisionMask;
    public BoardCodes type { get; protected set; }

    // Use this for initialization
    void Start() {
        InitializeType();
        animator = GetComponent<Animation2DManager>();
    }

    protected abstract void InitializeType();

    public bool GetIsMoving() {
        return isMoving;
    }

    public bool GetIsColliding() {
        return isColliding; 
    }

	public coord GetCoords() {
		return coords;
	}

	public void SetCoords(int col, int row) {
		coords.col = col;
		coords.row = row;
	}

	public void SetCoords(coord c) {
		coords = c;
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
                SetAnimationState(direction); 
                //snap to correct place for portals
                Vector3 endPos = new Vector3(coords.col + GameManagerScript.mapOrigin.x, coords.row + GameManagerScript.mapOrigin.y + yOffset, this.transform.position.z);
                this.transform.position = endPos;
			}
			transform.Translate(new Vector3(x * distance, y * distance, 0), Space.World);       
		}
        if(isColliding) {
            float dt = Time.deltaTime;
            int y = direction == Direction.NORTH ? 1 : (direction == Direction.SOUTH ? -1 : 0);
            int x = direction == Direction.EAST ? 1 : (direction == Direction.WEST ? -1 : 0);
            
            float distance = dt * cSpeed;
            distanceToMove -= distance;
            if(distanceToMove < 0.5 * totalDistance) {
                y *= -1;
                x *= -1; 
            }
            if (distanceToMove <= 0) {
                isColliding = false;
                SetAnimationState(direction);
                Vector3 endPos = new Vector3(coords.col + GameManagerScript.mapOrigin.x, coords.row + GameManagerScript.mapOrigin.y + yOffset, this.transform.position.z);
                this.transform.position = endPos;
            }
            else {
                transform.Translate(new Vector3(x * distance, y * distance, 0), Space.World);
            }
        }
	}

	//TODO: I think Unity is encouraging bad design here, but models should NOT be getting boardsate
	public abstract coord GetAttemptedMoveCoords (Direction direction, int[,] boardState, int numSpaces);

	public abstract Direction GetAttemptedMoveDirection (Direction direction, int[,] boardState);

	public void ExecuteMove(Direction direction, int numSpaces, bool animOnly = false) {

        distanceToMove = numSpaces;
        //TODO if your first direction is NONE, things get weird 
        if (direction == Direction.NONE) {
            isColliding = true;
            distanceToMove /= collideFactor;
            totalDistance = distanceToMove; 
        } else {
            isMoving = true;
            this.direction = direction;
        }

        SetAnimationState(direction);
        //Debug.Log(this.name + " moving " + direction.ToString());

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

    public void EnterPortal(int[,] boardState, coord portalCoords) {
        coords = portalCoords;
    }

    public void SetAnimationState(Direction direction) {
        int animateDir = (int)this.direction;
        if (isColliding) {
            animateDir += 4;  //direction + 4 will give you the index of the colliding animation 
        }
        if (animator != null) { 
            animator.StopAllAnimations();
            animator.Play(animateDir, loop: true);
        }
    }
}
