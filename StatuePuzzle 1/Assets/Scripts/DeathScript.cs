using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScript : MonoBehaviour
{

	public bool playerDeath;
	public float restartDelay = 1f;

	private Animator anim;
	private float restartTimer;
	
	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (playerDeath) {
			anim.SetTrigger("Dead");
			restartTimer += Time.deltaTime;
			GameManagerScript.inputReady = false;

			if (restartTimer >= restartDelay) {
				SceneManager.LoadScene(1);
			}
			
		}
	}
}
