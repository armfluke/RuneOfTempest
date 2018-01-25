using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TurnManager : MonoBehaviour {

	public const float TIME_PER_TURN = 90f;
	public int turn = 1;
	public int currentTeamTurn = 1;
	public float time;
	public Text currentTeamTurnText;
	public Text turnText;
	public Text timeText;

	public void EndTurn(){
		this.time = TIME_PER_TURN;
		this.currentTeamTurn++;
		if(this.currentTeamTurn == 5){
			this.currentTeamTurn = 1;
			turn++;
		}
	}

	// Use this for initialization
	void Start () {
		this.time = TIME_PER_TURN;
	}
	
	// Update is called once per frame
	void Update () {
		this.turnText.text = "Turn: " + this.turn;
		this.currentTeamTurnText.text = "Current team: " + this.currentTeamTurn;
		this.timeText.text = "Time: " + (int)this.time;
		this.time -= Time.deltaTime;
		if(time <= 0){
			EndTurn();
		}
	}
}
