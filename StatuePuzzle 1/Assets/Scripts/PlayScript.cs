using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayScript : MonoBehaviour
{
	
	private Animator anim;

	private void Start()
	{
		anim = GetComponentInParent<Animator>();
	}

	public void LoadScene()
	{
		for (int i = 0; i < CreateLevelSelect.getLevelList().Count; i++)
		{
			{
				if (PlayerPrefs.GetInt(CreateLevelSelect.getLevelList()[i], 0) == 0) // 0 for incomplete, 1 for complete
				{
					anim.SetTrigger("Zoom");
					GameManagerScript.levelName = CreateLevelSelect.getLevelList()[i];
					NextLevelScript.currentLevel = CreateLevelSelect.getLevelList()[i];
					Invoke("LoadSceneHelper", 3.5f);
					return;
				}
			}
		}
		GameManagerScript.levelName = CreateLevelSelect.getLevelList()[0];
		NextLevelScript.currentLevel = CreateLevelSelect.getLevelList()[0];
		SceneManager.LoadScene(1);
	}

	private void LoadSceneHelper()
	{
		SceneManager.LoadScene(1);
	}
}
