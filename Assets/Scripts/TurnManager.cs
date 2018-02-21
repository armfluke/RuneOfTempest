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
	private Player player;
	private GameObject mainGame;


	public void EndTurn(){
		this.network.SendEndTurnMessage();
	}

	// Use this for initialization
	void Start () {
		this.network = GameObject.Find("NetworkManager").GetComponent<Network>();
		this.player = GameObject.Find("Player").GetComponent<Player>();
		this.mainGame = GameObject.Find("UserInterface").transform.Find("MainGame").gameObject;

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

		if(this.player.status == "Lose" && this.player.team == this.currentTeamTurn){
			EndTurn();
		}

	}
}
