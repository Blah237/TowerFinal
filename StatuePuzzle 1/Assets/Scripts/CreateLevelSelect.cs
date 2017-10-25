using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class CreateLevelSelect : MonoBehaviour
{

	public LoadOnClick load;
	public float width;
	public float height;

	public float padding;
	public Canvas canvas;

	public int cols;
	public int rows;

	private static int CANVAS_WIDTH_OFFSET = 0;

	public static string[] levelList;
	public static Dictionary<string, bool> buttonMap = null;

	// Use this for initialization
	void Start()
	{
		levelList = new string[17];
		levelList[0] = "01level1";
		levelList[1] = "02level2";
		levelList[2] = "03level3";
		levelList[3] = "04blockLevel";
		levelList[4] = "08level4";
        levelList[5] = "09level5";
        levelList[6] = "21smallSwap";
        levelList[7] = "22SwapMaze";
        levelList[8] = "23SwapTest";
        levelList[9] = "31dumbPortalTutorial";
        levelList[10] = "32portal2";
        levelList[11] = "33portalSwap";
        levelList[12] = "34CircleWithPortals";
        levelList[13] = "PortalLinkTest";
        levelList[14] = "LaserTest";
        levelList[15] = "lasertest2"; 
        levelList[16] = "portalBugExhaustiveTest";
        
        //getFiles();
		if (buttonMap == null)
		{
			buttonMap = new Dictionary<string, bool>();
			for (int i = 0; i < levelList.Length; i++)
			{
				buttonMap.Add(levelList[i], false);
			}
		}
		
		width *= canvas.pixelRect.width - CANVAS_WIDTH_OFFSET;
		height *= canvas.pixelRect.height;
		for (int i = 0; i < levelList.Length; i++)
		{
			{
				LoadOnClick button = GameObject.Instantiate(load);
				if (buttonMap[levelList[i]])
				{
					button.GetComponent<Image>().color = Color.green;
				}
				button.GetComponent<LoadOnClick>().levelName = levelList[i];
				Text text = button.GetComponentInChildren<Text>();
				text.text = levelList[i];
				text.fontSize = 16;
				Image bImage = button.GetComponent<Image>();
				bImage.transform.SetParent(canvas.transform);
				positionSquare(bImage, i / 4, i % 4);
			}
		}
	}


	void getFiles()
	{
        DirectoryInfo levelDirectoryPath = new DirectoryInfo(Application.dataPath + "/Resources/Levels");
        FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.*", SearchOption.AllDirectories);
		int incrementer = 0;
			foreach (FileInfo file in fileInfo)
			{
				if (file.Extension != ".meta")
				{
					incrementer++;
				}
			}
		levelList = new string[incrementer];
		incrementer = 0;
		foreach (FileInfo file in fileInfo)
		{
			if (file.Extension != ".meta")
			{
				levelList[incrementer] = Path.GetFileNameWithoutExtension(file.Name);
				incrementer++;
			}
		}
	}

public static long DirCount(DirectoryInfo d)
	{
		long i = 0;
		// Add file sizes.
		FileInfo[] fis = d.GetFiles();
		foreach (FileInfo fi in fis)
		{
			if (fi.Extension.Contains("json"))
				i++;
		}
		return i;
	}
	
	void positionSquare(Image square, int r, int c) {
		square.rectTransform.anchoredPosition = new Vector2(width / cols * (c + 0.5f), height - (height / rows * (r + 0.5f)));
		square.rectTransform.sizeDelta = new Vector2(width / (float)cols - padding / 2f, height / (float)rows - padding / 2f);
		square.rectTransform.localScale = Vector2.one;
	}

	public static string[] getLevelList()
	{
		return levelList;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
