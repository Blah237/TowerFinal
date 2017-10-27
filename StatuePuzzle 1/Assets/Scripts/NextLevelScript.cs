using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelScript : MonoBehaviour {
	
	public static string currentLevel;
	
	void Start() { Time.timeScale = 1; }

	private string getNextLevel()
	{
		List<string> levelList = CreateLevelSelect.getLevelList();
		for (int i = 0; i < levelList.Count; i ++) {
			if (levelList[i] == currentLevel) {
				if (i == levelList.Count - 1) {
					currentLevel = levelList[0];
					return levelList[0];
				} else {
					currentLevel = levelList[i + 1];
					return levelList[i + 1];
				}
			}
		}

		return levelList[0];
	}

	public void LoadScene()
	{
		string nextLevel = getNextLevel();
		GameManagerScript.levelName = nextLevel;
		Debug.Log(nextLevel);
		SceneManager.LoadScene(1);
	}
}
