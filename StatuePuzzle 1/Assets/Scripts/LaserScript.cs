using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Laser {
    public int startRow; //The top of the box where the laser starts from. 
    public int startCol; //The left side of the box where the laser starts from. 
    public Direction direction; //Can be NORTH, SOUTH, EAST, or WEST, mimics Direction enum 
    public int length; //How many squares the laser covers
    public int state; //0 is off, 1 is on 
    /**If type is a BoardCode of a moveable, then that character can go through the laser
        Otherwise, no characters can go through the laser**/ 
    public BoardCodes type; 
    public GameObject gameObject; 

    public bool isBetweenRow(int coord) {
        if (direction == Direction.NORTH) {
            return (startRow < coord && coord <= startRow + length);
        }
        if (direction == Direction.SOUTH) {
            return (startRow - length < coord && coord <= startRow);
        }
        return false;
    }

    public bool isBetweenCol(int coord) {
        if (direction == Direction.EAST) {
            return (startCol <= coord && coord < startCol + length);
        }
        if (direction == Direction.WEST) {
            return (startCol - length <= coord && coord < startCol);
        }
        return false;
    }
}