using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardState : MonoBehaviour
{

	public BoardCodes[,] staticDefs;
	public coord playerPosition;
	public HashSet<coord> mimicPositions;
	public HashSet<coord> mirrorPositions;
	public Dictionary<coord,bool> buttonStates;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

