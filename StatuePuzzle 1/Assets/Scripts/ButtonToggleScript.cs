using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ButtonToggleScript : MonoBehaviour
{

	public bool pressed;
	[SerializeField]
	private Material onMaterial;
	[SerializeField]
	private Material offMaterial;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (pressed){
			GetComponent<MeshRenderer>().material = offMaterial;
		} else {
			GetComponent<MeshRenderer>().material = onMaterial;
		}
	}
}
