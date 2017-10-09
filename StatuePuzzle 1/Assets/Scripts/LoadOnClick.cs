using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LoadOnClick : MonoBehaviour {

	public GameObject loadingImage;
	public string levelName;

	public void LoadScene(string level)
	{
		int levelNum = -1;
		for (int i = 0; i < levelName.Length; i++) {
			if (Int32.TryParse (levelName.Substring(i,1), out levelNum)) {
				break;
			}
		}
		LoggingManager.instance.RecordLevelStart (levelNum, levelName);

		GameManagerScript.levelName = levelName;
		NextLevelScript.currentLevel = levelName;
		SceneManager.LoadScene(1);
	}
}