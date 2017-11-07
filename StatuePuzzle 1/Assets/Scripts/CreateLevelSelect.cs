using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class CreateLevelSelect : MonoBehaviour {

	public LoadOnClick load;

	public float width;
	public float height;
    private float padX;
	private float padY = 50.0f;

    public float padding;
	public Canvas canvas;

	public int cols;
	public int rows;

	private static int CANVAS_WIDTH_OFFSET = 0;

	public static List<String> levelList;
	public static Dictionary<string, bool> buttonMap = null;

	// Use this for initialization
	void Start()
	{
		
		if (LoggingManager.instance.GetABStoredValue() == 0) {
			//Do option A
			levelList = new List<string> {
				//mimic
				"01level1",
				"tutorial1",
				//blocking
				"tutorial3",
				"tutorial5",
				"04blockLevel",
				//mirror
				"02level2",
				"tutorial2",
				//mirror + blocking 
				"tutorial4",
				//collision
				"03level3",
				//blocking + mirror + mimic
				"08level4",

				//swap 
				"tutorial6",
				"22SwapMaze",
				"21smallSwap",
				"23SwapTest",

				//portal 
				"31dumbPortalTutorial",
				"32portal2",
				//portal + swap 
				"33portalSwap",

				//lasers 
				"tutorial7",
				"tutorial8",

				//many floaty bois
				"09level5",
				"34CircleWithPortals"
			};
		} else if (LoggingManager.instance.GetABStoredValue() == 1) {
			//Do option B
			levelList = new List<string> {
				//mimic
				"tutorial1",
				"01level1",
				//blocking
				"tutorial3",
				"tutorial5",
				"04blockLevel",
				//mirror
				"02level2",
				"tutorial2",
				//mirror + blocking 
				"tutorial4",
				//collision
				"03level3",
				//blocking + mirror + mimic
				"08level4",

				//swap 
				"tutorial6",
				"22SwapMaze",
				"21smallSwap",
				"23SwapTest",

				//portal 
				"31dumbPortalTutorial",
				"32portal2",
				//portal + swap 
				"33portalSwap",

				//lasers 
				"tutorial7",
				"tutorial8",

				//many floaty bois
				"09level5",
				"34CircleWithPortals"
			};
		} else {
			throw new Exception ("PlayerPref for AB testing was not initialized correctly.");
		}

        //getFiles();
        if (buttonMap == null) {
			buttonMap = new Dictionary<string, bool>();
			for (int i = 0; i < levelList.Count; i++)
			{
				buttonMap.Add(levelList[i], false);
			}
		}

        padX = (1f - width) / 2f * canvas.pixelRect.width;
        width *= canvas.pixelRect.width;
		height *= canvas.pixelRect.height;
		for (int i = 0; i < levelList.Count; i++)
		{
			{
				LoadOnClick button = GameObject.Instantiate(load);
				if (buttonMap[levelList[i]])
				{
					button.GetComponent<Image>().color = Color.green;
				}
				button.GetComponent<LoadOnClick>().levelName = levelList[i];
				Text text = button.GetComponentInChildren<Text>();
                text.text = ""+ (i + 1);//levelList[i];
				text.fontSize = 16;
				Image bImage = button.GetComponent<Image>();
				bImage.transform.SetParent(canvas.transform);
				positionSquare(bImage, i / cols, i % cols);
			}
		}
	}


	void getFiles() {
        DirectoryInfo levelDirectoryPath = new DirectoryInfo(Application.dataPath + "/Resources/Levels");
        FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.*", SearchOption.AllDirectories);
		levelList = new List<string>();
		foreach (FileInfo file in fileInfo)
		{
			if (file.Extension != ".meta")
			{
				levelList.Add(Path.GetFileNameWithoutExtension(file.Name));
			}
		}
	}

    public static long DirCount(DirectoryInfo d) {
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
		square.rectTransform.anchoredPosition = new Vector2(width / cols * (c + 0.5f) + padX, height - (height / rows * (r + 0.5f)) + padY);
		square.rectTransform.sizeDelta = new Vector2(width / (float)cols - padding / 2f, height / (float)rows - padding / 2f);
		square.rectTransform.localScale = Vector2.one;
	}

	public static List<string> getLevelList()
	{
		return levelList;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
