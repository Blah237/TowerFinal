using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicState
{
	public coord playerPosition;
	public HashSet<coord> mimicPositions;
	public HashSet<coord> mirrorPositions;
	public Dictionary<coord,bool> buttonStates;

	public DynamicState() {
		mimicPositions = new HashSet<coord> ();
		mirrorPositions = new HashSet<coord> ();
		buttonStates = new Dictionary<coord,bool> ();
	}

	public string toJson() {
		return "";
	}
}

