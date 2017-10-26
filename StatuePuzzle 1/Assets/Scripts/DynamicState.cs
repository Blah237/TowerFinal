using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicState
{
	public coord playerPosition;
	public List<coord> mimicPositions;
	public List<coord> mirrorPositions;
	public List<coord> activeButtons;

	public DynamicState() {
		mimicPositions = new List<coord> ();
		mirrorPositions = new List<coord> ();
		activeButtons = new List<coord>();
	}

	public string toJson() {
		return JsonUtility.ToJson (this);
	}
}

