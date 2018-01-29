using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : MonoBehaviour {

	public int team;
	private NetworkManager NetworkManager;
	private Network network;
	private Generator generator;
	private bool checkTeam = true;

	// Use this for initialization
	void Start () {
		this.NetworkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		this.network = GameObject.Find("NetworkManager").GetComponent<Network>();
		this.generator = GameObject.Find("GameMechanic").GetComponent<Generator>();
	}
	
	// Update is called once per frame
	void Update () {

	}
}
