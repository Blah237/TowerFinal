using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicScript : MoveableScript
{
    
    protected override void InitializeType() {
        this.type = BoardCodes.MIMIC;
        this.direction = Direction.EAST; 
    }

    public override void SetAnimationState() {
        int animateDir = (int)this.direction;
        if (animator != null) {
            if (!animator.isPlaying(animateDir + 4)) {
                if (isColliding) {
                    animateDir += 4;  //direction + 4 will give you the index of the colliding animation 
                    animator.StopAllAnimations(); 
                    animator.Play(animateDir, restart: true);
                } else {
                    animator.StopAllAnimations(); 
                    animator.Play(animateDir, loop: true); 
                } 
            }
        }
    }

    public override coord GetAttemptedMoveCoords(Direction direction, int[,] boardState, int numSpaces) {
        switch (direction) {
            case Direction.SOUTH:
                if (coords.row <= 0 || boardState[coords.row - 1, coords.col] == 1) {
                    return coords;
                } else {
                    return new coord(coords.row - 1, coords.col);
                }
            case Direction.NORTH:
                if (coords.row >= boardState.GetLength(0) || boardState[coords.row + 1, coords.col] == 1) {
                    return coords;
                } else {
                    return new coord(coords.row + 1, coords.col);
                }
            case Direction.WEST:
                if (coords.col <= 0 || boardState[coords.row, coords.col - 1] == 1) {
                    return coords;
                } else {
                    return new coord(coords.row, coords.col - 1);
                }
            case Direction.EAST:
                if (coords.col >= boardState.GetLength(1) || boardState[coords.row, coords.col + 1] == 1) {
                    return coords;
                } else {
                    return new coord(coords.row, coords.col + 1);
                }
        }

        return coords;
    }

    public override Direction GetAttemptedMoveDirection(Direction direction, int[,] boardState) {
        this.direction = direction; 
        SetAnimationState(); 
        switch (direction) {
            case Direction.NORTH:
                if (coords.row <= 0 || boardState[coords.row + 1, coords.col] == 1) {
                    return Direction.NONE;
                } else {
                    return Direction.NORTH;
                }
            case Direction.SOUTH:
                if (coords.row >= boardState.GetLength(0) || boardState[coords.row - 1, coords.col] == 1) {
                    return Direction.NONE;
                } else {
                    return Direction.SOUTH;
                }
            case Direction.EAST:
                if (coords.col <= 0 || boardState[coords.row, coords.col + 1] == 1) {
                    return Direction.NONE;
                } else {
                    return Direction.EAST;
                }
            case Direction.WEST:
                if (coords.col >= boardState.GetLength(1) || boardState[coords.row, coords.col - 1] == 1) {
                    return Direction.NONE;
                } else {
                    return Direction.WEST;
                }
        }

        return Direction.NONE;
    }
}
