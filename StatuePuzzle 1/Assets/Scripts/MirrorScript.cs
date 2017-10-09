using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorScript : MoveableScript {

	protected override void InitializeType ()
	{
		this.type = BoardCodes.MIRROR;
        this.collisionMask = 1; 
	}

	public override coord GetAttemptedMoveCoords (Direction direction, int[,] boardState, int numSpaces)
	{
		switch (direction) {
		case Direction.SOUTH:
			if (coords.row >= boardState.GetLength (0) || boardState [coords.row + 1, coords.col] == 1) {
				return coords;
			} else {
				return new coord (coords.row + 1, coords.col); 
			}
		case Direction.NORTH:
			if (coords.row <= 0 || boardState [coords.row - 1, coords.col] == 1) {
				return coords;
			} else {
				return new coord (coords.row - 1, coords.col);
			}
		case Direction.WEST:
			if (coords.col >= boardState.GetLength (1) || boardState [coords.row, coords.col + 1] == 1) {
				return coords;
			} else {
				return new coord(coords.row, coords.col + 1);
			}
		case Direction.EAST:
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
		switch (direction) {
		case Direction.NORTH:
			if (coords.row >= boardState.GetLength (0) || boardState [coords.row - 1, coords.col] == 1) {
				return Direction.NONE;
			} else {
				return Direction.SOUTH; 
			}
		case Direction.SOUTH:
			if (coords.row <= 0 || boardState [coords.row + 1, coords.col] == 1) {
				return Direction.NONE;
			} else {
				return Direction.NORTH;
			}
		case Direction.EAST:
			if (coords.col >= boardState.GetLength (1) || boardState [coords.row, coords.col - 1] == 1) {
				return Direction.NONE;
			} else {
				return Direction.WEST;
			}
		case Direction.WEST:
			if (coords.col <= 0 || boardState [coords.row, coords.col + 1] == 1) {
				return Direction.NONE;
			} else {
				return Direction.EAST;
			}
		}

		return Direction.NONE;
	}
}