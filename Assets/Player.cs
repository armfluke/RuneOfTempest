using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : MonoBehaviour {

	public int team = 0;
	public int connectionId;
	private NetworkManager NetworkManager;
	private Network network;
	private Generator generator;

	// Use this for initialization
	void Start () {
		this.NetworkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		this.network = GameObject.Find("NetworkManager").GetComponent<Network>();
		this.generator = GameObject.Find("GameMechanic").GetComponent<Generator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(team == 0 && this.NetworkManager.client != null){
			this.network.RequestForTeam();
			
			//this.generator.GenerateUnit();
		}
	}
}
