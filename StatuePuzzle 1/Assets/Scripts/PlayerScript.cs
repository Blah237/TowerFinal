using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MoveableScript {

	//player specific fields
	private bool isDead = false;
    public AudioClip victorySound;
    private enum AnimState {
        Idle, 
        Bump, 
        Move, 
        Pep, 
        Transition
    }
    private AnimState animState = AnimState.Idle; 

    public bool GetIsDead() {
		return isDead;
	}

	protected override void InitializeType ()
	{
		this.type = BoardCodes.PLAYER;
	}

    public override void SetAnimationState() {
        if (animator != null && animState != AnimState.Pep) {
            if (isColliding && animState != AnimState.Bump) {
                animator.StopAllAnimations(); 
                animator.Play("bump", restart: true);
                animState = AnimState.Bump;
            }
            else if (isMoving) {
                if (animState == AnimState.Idle) {
                    if (LoggingManager.instance.isDebugging) {
                        Debug.Log("Play Transition");
                    }
                    animator.Play("transition", restart: true);
                    animState = AnimState.Transition; 
                } else if (animState == AnimState.Bump || (animState == AnimState.Transition && !animator.isPlaying("transition"))) {
                    if (LoggingManager.instance.isDebugging) {
                        Debug.Log("Play Move");
                    }
                    animator.Play("move", restart: true, loop: true);
                    animState = AnimState.Move; 
                }
            } else {
                if(animState == AnimState.Move) {
                    animator.StopAllAnimations(); 
                    animator.Play("transition", reverse: true, restart: true);
                    if (LoggingManager.instance.isDebugging) {
                        Debug.Log("Play End Transition");
                    }
                    animState = AnimState.Transition; 
                } else if (animState == AnimState.Transition && !animator.isPlaying("transition")) {
                    if (LoggingManager.instance.isDebugging) {
                        Debug.Log("Play Idle");
                    }
                    animator.Play("idle", restart: true, loop: true);
                    animState = AnimState.Idle; 
                } else if (animState == AnimState.Bump && !animator.isPlaying("bump")) {
                    animator.Play("idle", restart: true, loop: true);
                    animState = AnimState.Idle;
                }
            }
        }
    }

    public void Celebrate() {
        if (LoggingManager.instance.isDebugging) {
            Debug.Log("PEP");
        }
        animator.StopAllAnimations();
        animator.Play("pep", restart: true);
        animState = AnimState.Pep; 
        GetComponentInChildren<ParticleSystem>().Play();
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255); 
    }

	public override coord GetAttemptedMoveCoords (Direction direction, int[,] boardState, int numSpaces)
	{
		switch (direction) {
		case Direction.NORTH:
			if (coords.row >= boardState.GetLength (0) || boardState [coords.row + 1, coords.col] == 1) {
				return coords;
			} else {
				return new coord (coords.row + 1, coords.col); 
			}
		case Direction.SOUTH:
			if (coords.row <= 0 || boardState [coords.row - 1, coords.col] == 1) {
				return coords;
			} else {
				return new coord (coords.row - 1, coords.col);
			}
		case Direction.EAST:
			if (coords.col >= boardState.GetLength (1) || boardState [coords.row, coords.col + 1] == 1) {
				return coords;
			} else {
				return new coord (coords.row, coords.col + 1);
			}
		case Direction.WEST:
			if (coords.col <= 0 || boardState [coords.row, coords.col - 1] == 1) {
				return coords;
			} else {
				return new coord(coords.row, coords.col - 1);
			}
		}

		return coords;
	}

	public override Direction GetAttemptedMoveDirection (Direction direction, int[,] boardState)
	{
        this.direction = direction;
        switch (direction) {
		case Direction.NORTH:
			if (coords.row >= boardState.GetLength (0) || boardState [coords.row + 1, coords.col] == 1) {
				return Direction.NONE;
			} else {
				return direction; 
			}
		case Direction.SOUTH:
			if (coords.row <= 0 || boardState [coords.row - 1, coords.col] == 1) {
				return Direction.NONE;
			} else {
				return direction;
			}
		case Direction.EAST:
			if (coords.col >= boardState.GetLength (1) || boardState [coords.row, coords.col + 1] == 1) {
				return Direction.NONE;
			} else {
				return direction;
			}
		case Direction.WEST:
			if (coords.col <= 0 || boardState [coords.row, coords.col - 1] == 1) {
				return Direction.NONE;
			} else {
				return direction;
			}
		}

		return Direction.NONE;
	}

    public override void startSpin(int degrees, bool willSwap = true) {
        this.willSwap = false;
    }
}
