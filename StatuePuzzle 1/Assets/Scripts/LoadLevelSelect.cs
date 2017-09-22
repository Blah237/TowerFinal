using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelSelect : MonoBehaviour {

	public void LoadScene(int level)
	{
		Debug.Log("Load Level Select");
		SceneManager.LoadScene(0);
	}
}