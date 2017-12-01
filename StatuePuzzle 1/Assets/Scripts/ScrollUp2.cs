using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUp2 : MonoBehaviour {

	private Animator anim;
	
	// Use this for initialization
	void Start () {
		anim = GetComponentInParent<Animator>();
	}

	public void ScrollUp()
	{
		anim.SetTrigger("ScrollUp2");
	}
}
