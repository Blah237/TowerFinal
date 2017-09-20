using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;

[Serializable]
public class Level
{
    public int rows;
    public int cols;
    public int[] flatBoard;
    public int[,] board; 

    public void MakeFlatBoard() {
        flatBoard = new int[rows * cols]; 
        for(int i = 0; i < rows; i++) {
            for(int j = 0; j < cols; j++) {
                flatBoard[cols * i + j] = board[i, j]; 
            }
        }
    }

    public void Make2DBoard() {
        board = new int[rows, cols]; 
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                board[i, j] = flatBoard[cols * i + j]; 
            }
        }
    }
}

public abstract class IOScript : MonoBehaviour {

    private static string levelDirectory = "Levels/"; 

    public static Level ParseLevel(string levelName) {
        TextAsset level = Resources.Load(levelDirectory + levelName) as TextAsset;
        Level l = JsonUtility.FromJson<Level>(level.text);
        l.Make2DBoard();
        return l; 
    }

    public static String ExportLevel(Level level) {
        level.MakeFlatBoard();

        int levelNum = 1;
        string json = JsonUtility.ToJson(level);
        string path = "Assets/Resources/Levels/"; 

        while (File.Exists(path + "level" + levelNum + ".json")) {
            levelNum++; 
        }

        File.WriteAllText(path + "level" + levelNum + ".json", json);
        return json; 
    }
}
