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

    public float padding;
	public Canvas canvas;

	public int cols;
	public int rows;

	public static List<string> levelList;
	public static Dictionary<string, bool> buttonMap = null;

	// Use this for initialization
	void Start() {
		levelList = new List<string>();
		levelList.Add("01level1");
        levelList.Add("02level2");
        levelList.Add("03level3");
        levelList.Add("04blockLevel");
        levelList.Add("08level4");
        levelList.Add("09level5");
        levelList.Add("21smallSwap");
        levelList.Add("22SwapMaze");
        levelList.Add("23SwapTest");
        levelList.Add("31dumbPortalTutorial");
        levelList.Add("32portal2");
        levelList.Add("33portalSwap");
        levelList.Add("34CircleWithPortals");
        levelList.Add("PortalLinkTest");
        levelList.Add("LaserTest");
        levelList.Add("lasertest2");
        levelList.Add("portalBugExhaustiveTest");
        
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
		int incrementer = 0;
			foreach (FileInfo file in fileInfo)
			{
				if (file.Extension != ".meta")
				{
					incrementer++;
				}
			}
		levelList = new List<string>();
		incrementer = 0;
		foreach (FileInfo file in fileInfo)
		{
			if (file.Extension != ".meta")
			{
				levelList.Add(Path.GetFileNameWithoutExtension(file.Name));
				incrementer++;
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
		square.rectTransform.anchoredPosition = new Vector2(width / cols * (c + 0.5f) + padX, height - (height / rows * (r + 0.5f)));
		square.rectTransform.sizeDelta = new Vector2(width / (float)cols - padding / 2f, height / (float)rows - padding / 2f);
		square.rectTransform.localScale = Vector2.one;
	}

	public static string[] getLevelList() {
		return levelList.ToArray();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
