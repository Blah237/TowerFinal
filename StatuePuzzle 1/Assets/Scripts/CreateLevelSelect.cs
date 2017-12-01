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

	public Sprite completedLevel;
	public Sprite unlockedChallenge;
	public Sprite completedChallenge1;
	public Sprite completedChallenge2;
	public Sprite completedChallenge3;
	public Sprite completedChallenge4;
	public Sprite completedChallenge5;
	

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

	// Wrapper class for deserializing JSON, required for Unity's shit JSON util
	private class LevelList {
		public List<String> levelList;
		public static LevelList CreateFromJson(string json) {
			return JsonUtility.FromJson<LevelList> (json);
		}
	}

	static void BuildList()
	{
		if (LoggingManager.instance.GetABStoredValue () == 0) {
			TextAsset json = Resources.Load ("LevelProgressions/ProgA") as TextAsset;
			levelList = LevelList.CreateFromJson (json.text).levelList;
		} else if (LoggingManager.instance.GetABStoredValue () == 1) {
			TextAsset json = Resources.Load ("LevelProgressions/ProgB") as TextAsset;
			levelList = LevelList.CreateFromJson (json.text).levelList;
		} else if (!LoggingManager.instance.isDebugging) {
			throw new Exception ("PlayerPref for AB testing was not initialized correctly.");
		} else { // Logging manager is debugging, just default to ProgA
			TextAsset json = Resources.Load ("LevelProgressions/ProgA") as TextAsset;
			levelList = LevelList.CreateFromJson (json.text).levelList;
		}
	}

	// Use this for initialization
	void Start()
	{
		if (levelList == null)
		{
			BuildList();
		}

        //getFiles();

        //padX = (1f - width) / 2f * canvas.pixelRect.width;
        //width *= canvas.pixelRect.width;
        //width -= 120; //doing this because Nathaniel told me to 
        //padX += 55;   //doing this because Nathaniel told me to 
        //height *= canvas.pixelRect.height;
		bool reachedLockedLevel = false;
		for (int i = 0; i < 30 - 5; i++)
		{
			{
				GameObject button = GameObject.Find("Level" + (i+1).ToString());
				if (PlayerPrefs.GetInt (levelList [i], 0) == 1) { // 0 for incomplete, 1 for complete
					button.GetComponent<Image> ().sprite = completedLevel;
				} else if (!reachedLockedLevel) {
					//button.GetComponent<Image> ().color = Color.yellow;
					reachedLockedLevel = true;
				} else if (reachedLockedLevel) {
					button.GetComponent<Button>().interactable = false;
				}

				button.GetComponent<LoadOnClick>().levelName = levelList[i];
				//Text text = button.GetComponentInChildren<Text>();
                //text.text = ""+ (i + 1);//levelList[i];
				//text.fontSize = 14;
				//Image bImage = button.GetComponent<Image>();
				//bImage.transform.SetParent(canvas.transform);
				//positionSquare(bImage, i / cols, i % cols);
			}
		}
		for (int ii = 25; ii < 230; ii++)
		{
			GameObject button = GameObject.Find("Level" + (ii+1).ToString());
			if (PlayerPrefs.GetInt (levelList [ii], 0) == 1) { // 0 for incomplete, 1 for complete
				switch (ii + 1)
				{
					case 1 :
						button.GetComponent<Image> ().sprite = completedChallenge1;
						break;
					case 2 :
						button.GetComponent<Image> ().sprite = completedChallenge2;
						break;
					case 3 :
						button.GetComponent<Image> ().sprite = completedChallenge3;
						break;
					case 4 :
						button.GetComponent<Image> ().sprite = completedChallenge4;
						break;
					case 5 :
						button.GetComponent<Image> ().sprite = completedChallenge5;
						break;
						
				}
			} else if (!reachedLockedLevel)
			{
				button.GetComponent<Image>().sprite = unlockedChallenge;
				reachedLockedLevel = true;
			} else if (reachedLockedLevel) {
				button.GetComponent<Button>().interactable = false;
			}

			button.GetComponent<LoadOnClick>().levelName = levelList[ii];
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
		if (levelList == null)
		{
			BuildList();
		}
		return levelList;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
