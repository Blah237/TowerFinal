using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayScript : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void LoadScene()
	{
		for (int i = 0; i < CreateLevelSelect.getLevelList().Count; i++)
		{
			{
				if (PlayerPrefs.GetInt(CreateLevelSelect.getLevelList()[i], 0) == 0) // 0 for incomplete, 1 for complete
				{
					GameManagerScript.levelName = CreateLevelSelect.getLevelList()[i];
					SceneManager.LoadScene(1);
					return;
				}
			}
		}
		GameManagerScript.levelName = CreateLevelSelect.getLevelList()[0];
		SceneManager.LoadScene(1);
	}
}
