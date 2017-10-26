using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MoveableScript {

	//player specific fields
	private bool isDead = false;

    public bool GetIsDead() {
		return isDead;
	}

	protected override void InitializeType ()
	{
		this.type = BoardCodes.PLAYER;
	}

    public void Celebrate() {
        animator.StopAllAnimations();
        animator.Play("pep", restart: true); 
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
        SetAnimationState(direction);
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
}
