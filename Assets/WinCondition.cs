using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour {

	public Player player;
	public TurnManager turnManager;
	private int checkTurnChange;
	public GameMechanic gameMechanic;
	public Hexagon[] castlePosition;
	public int[] castleConquerCount = new int[]{0, 0, 0, 0};
	public bool[] checkCastleConquer = new bool[]{false, false, false, false};
	public Unit[] castleConquerUnit = new Unit[4];

	// Use this for initialization
	void Start () {
		this.castlePosition = new Hexagon[]{
			new Hexagon(-7, 0, 7), new Hexagon(0, -7, 7), new Hexagon(7, 0, -7), new Hexagon(0, 7, -7)
		};

		this.player = GameObject.Find("Player").GetComponent<Player>();
		this.gameMechanic = gameObject.GetComponent<GameMechanic>();
		this.turnManager = gameObject.GetComponent<TurnManager>();
		this.checkTurnChange = this.turnManager.turn;
	}
	
	// Update is called once per frame
	void Update () {

		//Check win condition if all unit die
		if(this.player.team != 0 && this.player.playerUnits.Count == 0){
			this.player.status = "Lose";
		}

		//Check win condition if conquer enemy castle for three turn
		//if turn is passed
		if(this.checkTurnChange != this.turnManager.turn){
			this.checkTurnChange = this.turnManager.turn;
			//set check if castle is conquered to false
			for(int i=0; i < this.checkCastleConquer.Length; i++){
				checkCastleConquer[i] = false;
			}
			//loop for each unit in game
			foreach(Unit unit in this.gameMechanic.unit){
				int index = 0;
				//loop to check all castle position
				foreach(Hexagon position in this.castlePosition){
					//check if there is unit on castle position
					if(unit.position.x == position.x && unit.position.y == position.y && unit.position.z == position.z){
						//check if unit on castle position is not the same team
						if((unit.team == 1 && index == 0) || (unit.team == 2 && index == 1) || (unit.team == 3 && index == 2) || (unit.team == 4 && index == 3)){
							break;
						}
						//castle is conquered
						this.checkCastleConquer[index] = true;

						//increment conquer count
						this.castleConquerCount[index]++;
						//if conquer count == 3 set status of player of that castle to Lose
						if(castleConquerCount[index] == 3 && this.player.castlePosition.x == position.x
							&& this.player.castlePosition.y == position.y && this.player.castlePosition.z == position.z){
								this.player.status = "Lose";
						}
						break;
					}
					index++;
				}
			}
			for(int i=0; i < this.checkCastleConquer.Length; i++){
				if(checkCastleConquer[i] == false){
					castleConquerCount[i] = 0;
				}
			}
		}
	}

	void OnGUI(){
		GUI.Label(new Rect(2, 30, 150, 100), "Conquer count team1: " + this.castleConquerCount[0]);
        GUI.Label(new Rect(2, 50, 150, 100), "Conquer count team2: " + this.castleConquerCount[1]);       
        GUI.Label(new Rect(2, 70, 150, 100), "Conquer count team3: " + this.castleConquerCount[2]);
		GUI.Label(new Rect(2, 90, 150, 100), "Conquer count team4: " + this.castleConquerCount[3]);
	}
}
