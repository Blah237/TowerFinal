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
        if (!isHorizontal) {
            return (startRow < coord && coord <= startRow + length);
        }
        return false;
    }

    public bool isBetweenCol(int coord) {
        if (isHorizontal) {
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
    public Sprite end;

    public Sprite mimicGradient;
    public Sprite mirrorGradient; 

    public GameObject[] gameObjects;

    private float spriteWidth = 2;
    private float spriteHeight = 2;
    [SerializeField]
    private float maskOffset; 


    public void makeLaser(Laser la, Vector2 mapOrigin) {
        data = la; 
        GameObject lasertype;

        //determine the number of objects needed 
        int num2Objects = data.length / 2;
        int num1Objects = data.length % 2;
        gameObjects = new GameObject[num2Objects + num1Objects];

        //horizontal or vertical?
        if (data.isHorizontal) {
            lasertype = horizontalLaser;
            this.gameObject.transform.position = new Vector3(1 + data.startCol + mapOrigin.x - 0.5f, data.startRow + mapOrigin.y + 0.63f, -0.1f);

            //instantiate them with position, rotation, size, etc. 
            for (int i = 0; i < num2Objects; i++) {
                GameObject g = GameObject.Instantiate(lasertype);
                if(data.type == BoardCodes.MIMIC) {
                    g.GetComponent<SpriteRenderer>().sprite = mimicGradient; 
                }
                if(data.type == BoardCodes.MIRROR) {
                    g.GetComponent<SpriteRenderer>().sprite = mirrorGradient; 
                }

                g.transform.parent = this.gameObject.transform;
                g.transform.localScale = new Vector3(1, 1, 1);
                g.transform.localPosition = new Vector3(0, 0, 0);
                g.transform.Translate(i * spriteWidth, 0, 0, Space.World);

                SpriteMask m = GameObject.Instantiate(mask);
                if (i == 0) {
                    m.sprite = start;
                }
                SpriteMask m2 = GameObject.Instantiate(mask);
                if (i == num2Objects - 1 && num1Objects == 0) {
                    m2.sprite = end;
                }
                m.transform.parent = g.transform;
                m2.transform.parent = g.transform;

                m.transform.localPosition = new Vector3(-maskOffset, 0, 0);
                m.transform.localScale = new Vector3(1, 1, 1);
                m2.transform.localPosition = new Vector3(maskOffset, 0, 0);
                m2.transform.localScale = new Vector3(1, 1, 1);

                if (!data.isActive) {
                    g.SetActive(false);
                }
                gameObjects[i] = g;

            }

            if (num1Objects == 1) {
                GameObject g = GameObject.Instantiate(lasertype);
                if (data.type == BoardCodes.MIMIC) {
                    g.GetComponent<SpriteRenderer>().sprite = mimicGradient;
                }
                if (data.type == BoardCodes.MIRROR) {
                    g.GetComponent<SpriteRenderer>().sprite = mirrorGradient;
                }

                g.transform.parent = this.gameObject.transform;
                g.transform.localScale = new Vector3(1, 1, 1);
                g.transform.localPosition = new Vector3(0, 0, 0);
                g.transform.Translate(num2Objects * spriteWidth, 0, 0, Space.World);

                SpriteMask m = GameObject.Instantiate(mask);
                m.sprite = end;
                m.transform.parent = g.transform;

                m.transform.localPosition = new Vector3(-maskOffset, 0, 0);
                m.transform.localScale = new Vector3(1, 1, 1);
                if (!data.isActive) {
                    g.SetActive(false);
                }
                gameObjects[gameObjects.Length - 1] = g;
            }

        }
        else {
            //vertical laser
            lasertype = verticalLaser;
            this.gameObject.transform.position = new Vector3(data.startCol + mapOrigin.x - 0.5f, 1 + data.startRow + mapOrigin.y + 0.63f, -0.1f);

            for (int i = 0; i < num2Objects; i++) {
                GameObject g = GameObject.Instantiate(lasertype);
                if (data.type == BoardCodes.MIMIC) {
                    g.GetComponent<SpriteRenderer>().sprite = mimicGradient;
                }
                if (data.type == BoardCodes.MIRROR) {
                    g.GetComponent<SpriteRenderer>().sprite = mirrorGradient;
                }

                g.transform.parent = this.gameObject.transform;
                g.transform.localScale = new Vector3(1, 0.1f, 1);
                g.transform.localPosition = new Vector3(0, 0, 0);
                g.transform.Translate(0, i * spriteWidth, 0, Space.World);

                if (!data.isActive) {
                    g.SetActive(false);
                }
                gameObjects[i] = g;
            }

            if (num1Objects == 1) {
                GameObject g = GameObject.Instantiate(lasertype);
                if (data.type == BoardCodes.MIMIC) {
                    g.GetComponent<SpriteRenderer>().sprite = mimicGradient;
                }
                if (data.type == BoardCodes.MIRROR) {
                    g.GetComponent<SpriteRenderer>().sprite = mirrorGradient;
                }

                g.transform.parent = this.gameObject.transform;
                g.transform.localScale = new Vector3(1, 0.1f, 1);
                g.transform.localPosition = new Vector3(0, -maskOffset, 0);
                g.transform.Translate(0, num2Objects * spriteWidth, 0, Space.World);

                g.GetComponent<SpriteRenderer>().size -= new Vector2(2.56f, 0);

                if (!data.isActive) {
                    g.SetActive(false);
                }
                gameObjects[gameObjects.Length - 1] = g;
            }
        }
    }

    public void ToggleActive() {
        data.isActive = !data.isActive; 
        foreach (GameObject g in gameObjects) {
            g.SetActive(!g.activeInHierarchy);
        }
    }
}