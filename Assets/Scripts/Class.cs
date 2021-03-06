﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Class : MonoBehaviour {

	private Generator generator;
	private Player player;
	private Network network;
	private GameMechanic gameMechanic;
	private GameObject classTree;
	private GameObject mainGame;
	private Database database;
	private TurnManager turnManager;

	/*T CopyComponent<T>(T original, GameObject destination) where T : Component {
		System.Type type = original.GetType();
		Component copy = destination.AddComponent(type);
		System.Reflection.FieldInfo[] fields = type.GetFields();
		foreach (System.Reflection.FieldInfo field in fields){
			field.SetValue(copy, field.GetValue(original));
		}
		return copy as T;
	}*/

	public void ClassChangeClicked(){
		if(this.player.cost >= this.database.status[EventSystem.current.currentSelectedGameObject.name].cost){
			this.player.cost -= this.database.status[EventSystem.current.currentSelectedGameObject.name].cost;
			this.classTree.SetActive(false);
			this.mainGame.SetActive(true);
			this.network.SendClassChangeMessage(this.gameMechanic.selectedUnit.unitName, EventSystem.current.currentSelectedGameObject.name);
		}
	}

	public void ClassChange(string unitName, string nextClass){
		Unit unit = null; 
		foreach(Unit character in this.gameMechanic.unit){
			if(character.unitName == unitName){
				unit = character;
			}
		}
		Unit newUnit = this.generator.GenerateUnit(nextClass, unit.unitName, unit.team, unit.position);
		if(this.player.team == this.turnManager.currentTeamTurn){
			this.player.playerUnits.Add(newUnit);
		}
		newUnit.state = unit.state;
		this.gameMechanic.selectedUnit = newUnit;
		unit.Die();
	}

	// Use this for initialization
	void Start () {
		GameObject userInterface;
		this.gameMechanic = gameObject.GetComponent<GameMechanic>();
		this.generator = gameObject.GetComponent<Generator>();
		this.player = GameObject.Find("Player").GetComponent<Player>();
		this.network = GameObject.Find("NetworkManager").GetComponent<Network>();
		userInterface = this.classTree = GameObject.Find("UserInterface");
		this.classTree = userInterface.transform.Find("Class").gameObject;
		this.mainGame = userInterface.transform.Find("MainGame").gameObject;
		this.database = gameObject.GetComponent<Database>();
		this.turnManager = gameObject.GetComponent<TurnManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
