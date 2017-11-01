﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class logList
{
    public List<logEvent> list;
}

[System.Serializable]
public struct logEvent {
    public string user_id;
    public int quest_id;
    public int session_seq_id;
    public int action_id;
    public string action_detail;
}

public class VisualizationManagerScript : MonoBehaviour {

    int frame = -1;
    int maxFrame;
    List<List<DynamicState>> states;
    List<GameObject> frameSprites;
    Dictionary<coord, int> playerCounters = new Dictionary<coord, int>();
    Dictionary<coord, int> mimicCounters = new Dictionary<coord, int>();
    Dictionary<coord, int> mirrorCounters = new Dictionary<coord, int>();
    List<DynamicState> restartsAndQuits = new List<DynamicState>();

    public string levelName;
    public string dataName;
    public Camera mainCamera;

    public GameObject PlayerPrefab;
    public GameObject MimicPrefab;
    public GameObject MirrorPrefab;

    public GameObject wall;
    public GameObject frontWall;
    public GoalScript goal;
    public LaserScript laser;
    public ButtonToggleScript button;
    public GameObject ground;
    public GameObject swap;
    public GameObject portal;

    float playerYOff = 0f;
    float mimicYOff = 0f;
    float mirrorYOff = 0f;

    Vector2 mapOrigin;

    // Use this for initialization
    void Start () {
        playerYOff = PlayerPrefab.GetComponent<MoveableScript>().yOffset;
        mimicYOff = MimicPrefab.GetComponent<MoveableScript>().yOffset;
        mirrorYOff = MirrorPrefab.GetComponent<MoveableScript>().yOffset;

        parseXML(dataName);

        frameSprites = new List<GameObject>();
        foreach(List<DynamicState> l in states) {
            maxFrame = Mathf.Max(maxFrame, l.Count);
        }

        //load level using Melody's I/O
        Level boardState = IOScript.ParseLevel(levelName);


        mapOrigin = new Vector2(-boardState.cols / 2.0f, -boardState.rows / 2.0f);
        mainCamera.orthographicSize = boardState.rows / 2.0f + 1;

        //instantiate lasers based on parsed lasers
        if (boardState.lasers != null) {
            foreach (Laser la in boardState.lasers) {
                LaserScript l = GameObject.Instantiate(laser);
                l.makeLaser(la, mapOrigin);
                //laserList.Add(l);
            }
        }

        //instantiate items based on board
        for (int i = 0; i < boardState.rows; i++) {
            for (int j = 0; j < boardState.cols; j++) {
                if (boardState.board[i, j] == 1) {
                    //wall
                    GameObject w;
                    if (i == 0 || boardState.board[i - 1, j] != 1) {
                        w = GameObject.Instantiate(frontWall);
                    } else {
                        w = GameObject.Instantiate(wall);
                    }
                    w.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                } else if (boardState.board[i, j] >= 10 && boardState.board[i, j] < 20) {
                    // goal
                    GoalScript c = GameObject.Instantiate(goal);
                    c.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    //goalAtCoords.Add(new coord(i, j), c);
                    //goalCoords.Add(new coord(i, j));
                } else if (boardState.board[i, j] >= 20 && boardState.board[i, j] < 30) {
                    // swap
                    GameObject c = GameObject.Instantiate(swap);
                    c.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    //swapCoords.Add(new coord(i, j));
                } else if (boardState.board[i, j] >= 30 && boardState.board[i, j] < 40) {
                    // button
                    ButtonToggleScript c = GameObject.Instantiate(button);
                    c.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    //c.laser = laserList[boardState.buttons[buttonCount]];
                    c.InitButton();
                    //buttonCoords.Add(new coord(i, j), c);
                    //buttonCount++;
                } else if (boardState.board[i, j] >= 50 && boardState.board[i, j] < 60) {
                    // portal
                    GameObject p = GameObject.Instantiate(portal);
                    p.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                    //portalCoords.Add(new coord(i, j));
                } else {
                    // ground 
                    GameObject g = GameObject.Instantiate(ground);
                    g.transform.position = new Vector3(j + mapOrigin.x, i + mapOrigin.y, 0);
                }     
            }
        }

        setFrame(0);
    }
	
    public void setFrame(int frameNumber) {
        int count = 0;
        if (frameNumber == int.MinValue) {
            if (this.frame == frameNumber) {
                return;
            }
            this.frame = frameNumber;
            Debug.Log("Setting Frame to " + this.frame);

            while (frameSprites.Count > 0) {
                GameObject go = frameSprites[0];
                frameSprites.RemoveAt(0);
                Destroy(go);
            }
            playerCounters.Clear();
            mimicCounters.Clear();
            mirrorCounters.Clear();
            count = 0;
            foreach (DynamicState s in restartsAndQuits) {  
                if (s == null) {
                    continue;
                }
                count++;
                incCounter(s.playerPosition, 1);
                foreach (coord pt in s.mimicPositions) {
                    incCounter(pt, 2);
                }
                foreach (coord pt in s.mirrorPositions) {
                    incCounter(pt, 3);
                }
            }
            foreach (coord pt in playerCounters.Keys) {
                GameObject go = GameObject.Instantiate(PlayerPrefab);
                go.transform.position = new Vector3(pt.col + mapOrigin.x, pt.row + mapOrigin.y + playerYOff, 0);
                SpriteRenderer view = go.GetComponent<SpriteRenderer>();
                view.color = new Color(view.color.r, view.color.g, view.color.b, 2f * playerCounters[pt] / (float)count);
                this.frameSprites.Add(go);
                go.transform.localScale *= .5f;
                go.transform.position += Vector3.right * -.25f;
            }
            foreach (coord pt in mimicCounters.Keys) {
                GameObject go = GameObject.Instantiate(MimicPrefab);
                go.transform.position = new Vector3(pt.col + mapOrigin.x, pt.row + mapOrigin.y + mimicYOff, 0);
                SpriteRenderer view = go.GetComponent<SpriteRenderer>();
                view.color = new Color(view.color.r, view.color.g, view.color.b, 2f * mimicCounters[pt] / (float)count);
                this.frameSprites.Add(go);
                go.transform.localScale *= .5f;
                go.transform.position += Vector3.right * .25f;
            }
            foreach (coord pt in mirrorCounters.Keys) {
                GameObject go = GameObject.Instantiate(MirrorPrefab);
                go.transform.position = new Vector3(pt.col + mapOrigin.x, pt.row + mapOrigin.y + mirrorYOff, 0);
                SpriteRenderer view = go.GetComponent<SpriteRenderer>();
                view.color = new Color(view.color.r, view.color.g, view.color.b, 2f * mirrorCounters[pt] / (float)count);
                this.frameSprites.Add(go);
                go.transform.localScale *= .5f;
                go.transform.position += Vector3.up * .5f;
                go.transform.position += Vector3.right * -.25f;
            }

            return;
        }

        int f = Mathf.Clamp(frameNumber, 0, maxFrame);
        if(this.frame == f) {
            return;
        }
        this.frame = f;
        Debug.Log("Setting Frame to " + this.frame);
        
        while(frameSprites.Count > 0) {
            GameObject go = frameSprites[0];
            frameSprites.RemoveAt(0);
            Destroy(go);
        }
        playerCounters.Clear();
        mimicCounters.Clear();
        mirrorCounters.Clear();
        foreach(List<DynamicState> stateList in states) {
            if(stateList == null) {
                continue;
            }
            if(stateList.Count > frame) {
                DynamicState s = stateList[this.frame];
                if(s == null) {
                    continue;
                }
                count++;
                incCounter(s.playerPosition, 1);
                foreach(coord pt in s.mimicPositions) {
                    incCounter(pt, 2);
                }
                foreach (coord pt in s.mirrorPositions) {
                    incCounter(pt, 3);
                }
            }
        }
        foreach(coord pt in playerCounters.Keys) {
            GameObject go = GameObject.Instantiate(PlayerPrefab);
            go.transform.position = new Vector3(pt.col + mapOrigin.x, pt.row + mapOrigin.y + playerYOff, 0);
            SpriteRenderer view = go.GetComponent<SpriteRenderer>();
            view.color = new Color(view.color.r, view.color.g, view.color.b, 2f* playerCounters[pt] / (float)count);
            this.frameSprites.Add(go);
            go.transform.localScale *= .5f;
            go.transform.position += Vector3.right * -.25f;
        }
        foreach (coord pt in mimicCounters.Keys) {
            GameObject go = GameObject.Instantiate(MimicPrefab);
            go.transform.position = new Vector3(pt.col + mapOrigin.x, pt.row + mapOrigin.y + mimicYOff, 0);
            SpriteRenderer view = go.GetComponent<SpriteRenderer>();
            view.color = new Color(view.color.r, view.color.g, view.color.b, 2f* mimicCounters[pt] / (float)count);
            this.frameSprites.Add(go);
            go.transform.localScale *= .5f;
            go.transform.position += Vector3.right * .25f;
        }
        foreach (coord pt in mirrorCounters.Keys) {
            GameObject go = GameObject.Instantiate(MirrorPrefab);
            go.transform.position = new Vector3(pt.col + mapOrigin.x, pt.row + mapOrigin.y + mirrorYOff, 0);
            SpriteRenderer view = go.GetComponent<SpriteRenderer>();
            view.color = new Color(view.color.r, view.color.g, view.color.b, 2f* mirrorCounters[pt] / (float)count);
            this.frameSprites.Add(go);
            go.transform.localScale *= .5f;
            go.transform.position += Vector3.up * .5f;
            go.transform.position += Vector3.right * -.25f;
        }
    }

    public void incCounter(coord point, int type) {
        Dictionary<coord, int> dict;
        switch (type) {
            case 1: // player
                dict = playerCounters;
                break;
            case 2: // mimic
                dict = mimicCounters;
                break;
            case 3: // mirror
                dict = mirrorCounters;
                break;
            default:
                throw new System.NotImplementedException();
        }
        if (dict.ContainsKey(point)) {
            dict[point]++;
        } else {
            dict.Add(point, 1);
        }
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            setFrame(this.frame - 1);
        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            setFrame(this.frame + 1);
        } else if (Input.GetKeyDown(KeyCode.R)) {
            setFrame(int.MinValue);
        }
	}

    void parseXML(string fileName) {
        // user, quest, session_seq, action_id, action_detail
        string dataDirectory = "Data/";
        states = new List<List<DynamicState>>();
        restartsAndQuits = new List<DynamicState>();
        TextAsset data = Resources.Load(dataDirectory + fileName) as TextAsset;
        logList logs = JsonUtility.FromJson<logList>(data.text);
        List<logEvent> table = logs.list;
        string userID = "";
        int seqOffset = 0;
        int lastTurn = -1;
        int userCount = -1;
        foreach(logEvent row in table) {
            //ary = row.ItemArray;
            if (row.action_id == 4) {
                continue;
            }
            if (row.action_id == 1) {
                lastTurn++;
                continue;
            }
            if(row.action_id == 3) {
                restartsAndQuits.Add(states[userCount][states[userCount].Count - 1]);
                continue;
            }
            if (row.action_id == 2) {
                restartsAndQuits.Add(states[userCount][states[userCount].Count - 1]);
                userID = "";
                continue;
            }
            if (row.user_id != userID) {
                userID = row.user_id;
                seqOffset = row.session_seq_id;
                lastTurn = seqOffset;
                userCount++;
                states.Add(new List<DynamicState>());
                states[userCount].Add(DynamicState.fromJson(row.action_detail));
            } else {
                while(row.session_seq_id > lastTurn + 1) {
                    states[userCount].Add(null);
                    lastTurn++;
                }
                states[userCount].Add(DynamicState.fromJson(row.action_detail));
                lastTurn++;
            }
        }
        table = null;
    }
}