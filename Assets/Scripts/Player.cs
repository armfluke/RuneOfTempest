using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : MonoBehaviour {

	public int team = 0;
	public List<Unit> playerUnits;
	public string status = "Alive";
	public Hexagon castlePosition;
	private bool checkLose = false;
	public GameMechanic gameMechanic;
	public Network network;
	public GameObject mainGame;

	// Use this for initialization
	void Start () {
		this.gameMechanic = GameObject.Find("GameMechanic").GetComponent<GameMechanic>();
		this.network = GameObject.Find("NetworkManager").GetComponent<Network>();
		this.mainGame = GameObject.Find("UserInterface").transform.Find("MainGame").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if(this.status == "Lose" && this.checkLose == false){
			this.checkLose = true;
			for(int i=0; i < this.playerUnits.Count; i++){
				this.playerUnits[i].hp = 0;
			}

			this.network.SendLoseStatusMessage(this.team);
			//TODO: Add all player unit hp to 0 disable userinterface show lose and back button
			//maybe send status to server
			if(this.status == "Lose"){
				this.mainGame.SetActive(false);
			}
		}
	}

	void OnGUI(){
        GUI.Label(new Rect(2, 10, 150, 100), "Team:"+this.team);
	}
}
