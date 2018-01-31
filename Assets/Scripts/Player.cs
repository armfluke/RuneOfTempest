using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : MonoBehaviour {

	public int team = 0;
	public int connectionId;
	public List<Unit> playerUnits;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI(){
        GUI.Label(new Rect(2, 10, 150, 100), "Team:"+this.team);
	}
}
