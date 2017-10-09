using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LoadOnClick : MonoBehaviour {

	public GameObject loadingImage;

	public void LoadScene(string level)
	{
		int levelNum = -1;
		for (int i = 0; i < level.Length; i++) {
			if (Int32.TryParse (level.Substring(i,1), out levelNum)) {
				break;
			}
		}
		LoggingManager.instance.RecordLevelStart (levelNum, level);

		GameManagerScript.levelName = level;
		SceneManager.LoadScene(1);
	}
}