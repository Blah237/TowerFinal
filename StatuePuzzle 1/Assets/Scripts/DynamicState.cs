using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

public class DynamicState
{
	public coord playerPosition;
	public HashSet<coord> mimicPositions;
	public HashSet<coord> mirrorPositions;
	public Dictionary<coord,int> buttonStates;

	public DynamicState() {
		mimicPositions = new HashSet<coord> ();
		mirrorPositions = new HashSet<coord> ();
		buttonStates = new Dictionary<coord,int> ();
	}

	public string toJson() {
		return JsonConvert.SerializeObject (this);
	}
}

