using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelSelect : MonoBehaviour {

	public GameObject loadingImage;

	public void LoadScene(int level)
	{
		SceneManager.LoadScene(0);
	}
}