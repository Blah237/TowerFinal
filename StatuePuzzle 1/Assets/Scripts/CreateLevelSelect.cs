using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using UnityEditor;
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

	public static string[] levelList;

	// Use this for initialization
	void Start()
	{
		//levelList = new string[5];
		//levelList[0] = "level1";
		//levelList[1] = "level2";
		//levelList[2] = "level3";
		//levelList[3] = "level4";
		//levelList[4] = "level5";
		getFiles();
		width *= canvas.pixelRect.width - 130;
		height *= canvas.pixelRect.height;
		for (int i = 0; i < levelList.Length; i++)
		{
			{
				LoadOnClick button = GameObject.Instantiate(load);
				button.GetComponent<LoadOnClick>().levelName = levelList[i];
				Text text = button.GetComponentInChildren<Text>();
				text.text = levelList[i];
				Image bImage = button.GetComponent<Image>();
				bImage.transform.SetParent(canvas.transform);
				positionSquare(bImage, i / 4, i % 4);
			}
		}
	}


	void getFiles()
	{
		DirectoryInfo levelDirectoryPath = new DirectoryInfo("../StatuePuzzle 1/Assets/Resources/Levels");
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
