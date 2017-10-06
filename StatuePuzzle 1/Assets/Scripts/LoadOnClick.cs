using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOnClick : MonoBehaviour {

	public GameObject loadingImage;
	public string levelName;

	public void LoadScene()
	{
		GameManagerScript.levelName = levelName;
		SceneManager.LoadScene(1);
	}
}