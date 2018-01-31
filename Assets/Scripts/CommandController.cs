using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandController : MonoBehaviour {

	private GameMechanic gameMechanic;
	private Unit unit;
	private Button Move;

	// Use this for initialization
	void Start () {
		this.Move = GameObject.Find("UserInterface").transform.Find("UnitDetails").Find("Move").GetComponent<Button>();
		this.gameMechanic = GameObject.Find("GameMechanic").GetComponent<GameMechanic>();
	}
	
	// Update is called once per frame
	void Update () {
		
		this.unit = this.gameMechanic.selectedUnit;

		if(this.unit != null){
			if(this.unit.state == "Move"){
				this.Move.interactable = false;
			}else{
				this.Move.interactable = true;
			}
		}
	}
}
