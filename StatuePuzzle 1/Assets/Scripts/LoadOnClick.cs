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
		WinScript.playerWin = false;
		GameManagerScript.levelName = levelName;
		NextLevelScript.currentLevel = levelName;
		SceneManager.LoadScene(1);
	}
}