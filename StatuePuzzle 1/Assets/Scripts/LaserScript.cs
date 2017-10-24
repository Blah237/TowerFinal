using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Laser {
    public int startRow; //The top of the box where the laser starts from. 
    public int startCol; //The left side of the box where the laser starts from. 
    public bool isHorizontal; //1 for horizontal, 0 for vertical. laser always grows up or left 
    public int length; //How many squares the laser covers
    public bool isActive; //0 is off, 1 is on 
    /**If type is a BoardCode of a moveable, then that character can go through the laser
        Otherwise, no characters can go through the laser**/ 
    public BoardCodes type; 

    public bool isBetweenRow(int coord) {
        if (isHorizontal) {
            return (startRow < coord && coord <= startRow + length);
        }
        return false;
    }

    public bool isBetweenCol(int coord) {
        if (!isHorizontal) {
            return (startCol <= coord && coord < startCol + length);
        }
        return false;
    }
}

public class LaserScript : MonoBehaviour {

    public Laser data;
    public GameObject horizontalLaser;
    public GameObject verticalLaser; 

    public SpriteMask mask;
    public Sprite start;
    public Sprite middle;
    public Sprite end; 

    public GameObject[] gameObjects;

    private float spriteWidth = 2;
    private float spriteHeight = 2; 


    public void makeLaser(Laser la, Vector2 mapOrigin) {
        data = la; 
        GameObject lasertype; 
        //horizontal or vertical?
        if (data.isHorizontal) {
            lasertype = horizontalLaser; 

            //determine the number of objects needed 
            int num2Objects = data.length / 2;
            int num1Objects = data.length % 2;
            gameObjects = new GameObject[num2Objects + num1Objects];

            //instantiate them with position, rotation, size, etc. 
            for (int i = 0; i < num2Objects; i++) {
                GameObject g = GameObject.Instantiate(lasertype);
                g.transform.position = new Vector3(data.startCol + (i * spriteWidth) + mapOrigin.x - 0.5f, data.startRow + mapOrigin.y + 0.5f, -0.1f);
                if (!data.isActive) {
                    g.SetActive(false);
                }
                gameObjects[i] = g;

            }

            for (int i = 0; i < num1Objects; i++) {
                GameObject g = GameObject.Instantiate(lasertype);
                g.transform.position = new Vector3(data.startCol + ((i + num2Objects) * spriteWidth) + mapOrigin.x - 0.5f, data.startRow + mapOrigin.y + 0.5f, -0.1f);
                if (!data.isActive) {
                    g.SetActive(false);
                }
                gameObjects[gameObjects.Length - 1] = g;
            }

        } else {
            //vertical laser
        }
    }

    public void ToggleActive() {
        data.isActive = !data.isActive; 
        foreach (GameObject g in gameObjects) {
            g.SetActive(!g.activeInHierarchy);
        }
    }
}