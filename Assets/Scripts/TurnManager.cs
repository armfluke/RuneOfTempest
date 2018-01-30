using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

public class TurnManager : MonoBehaviour {

	public const float TIME_PER_TURN = 90f;
	public int turn = 1;
	public int currentTeamTurn = 1;
	public float time;
	public Text currentTeamTurnText;
	public Text turnText;
	public Text timeText;
	private Network network;

	public void EndTurn(){
		this.network.SendEndTurnMessage();
		
	}

	// Use this for initialization
	void Start () {
		this.network = GameObject.Find("NetworkManager").GetComponent<Network>();

		this.time = TIME_PER_TURN;
	}
	
	// Update is called once per frame
	void Update () {

		this.turnText.text = "Turn: " + this.turn;
		this.currentTeamTurnText.text = "Current team: " + this.currentTeamTurn;
		this.timeText.text = "Time: " + (int)this.time;
		this.time -= Time.deltaTime;
		this.network.SendTurnMessage(this.time, this.currentTeamTurn);

		if(time <= 0){
			EndTurn();
		}
	}
}
