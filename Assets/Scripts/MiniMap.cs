using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class MiniMap : MonoBehaviour {

	private GameObject tile;
	private Hexagon hexagon;
	private Command command;
	private GameMechanic gameMechanic;
	private Unit selectedUnit;
	private GameObject miniMap;
	private GameObject mainGame;

	public void OnMiniMapSelected(){
		Debug.Log(EventSystem.current.currentSelectedGameObject.name);
		this.tile = EventSystem.current.currentSelectedGameObject;
		this.hexagon = EventSystem.current.currentSelectedGameObject.GetComponent<Hexagon>();
		this.selectedUnit = this.gameMechanic.selectedUnit;

		//TODO: Checking if selected tile is appropriate maybe use hilight tile variable
		if(this.command.move == true){
			Hexagon[] range = this.gameMechanic.cube.MovementRange(this.selectedUnit.position, this.selectedUnit.status.range);
			Boolean checkRange = false;
			foreach(Hexagon hex in range){
				if(this.hexagon.x == hex.x && this.hexagon.y == hex.y && this.hexagon.z == hex.z){
					checkRange = true;
					break;
				}
			}

			if(checkRange){
				this.miniMap.SetActive(false);
				this.mainGame.SetActive(true);
				this.command.move = false;
				Move();
			}
		}else if(this.command.attack == true){
			this.miniMap.SetActive(false);
			this.mainGame.SetActive(true);
			this.command.attack = false;
			Attack();
		}else if(this.command.skill == true){
			this.miniMap.SetActive(false);
			this.mainGame.SetActive(true);
			this.command.skill = false;
			Skill();
		}
	}

	private void Move(){
		//TODO: Add checking condition e.g. range
		this.selectedUnit.Move(this.hexagon);
	}

	private Unit SearchUnit(Hexagon position){
		foreach(Unit unit in this.gameMechanic.unit){
			if(unit.position.x == position.x && unit.position.y == position.y && unit.position.z == position.z){
				return unit;
			}
		}
		return null;
	}

	private void Attack(){
		Unit targetUnit = SearchUnit(this.hexagon);
		if(targetUnit){
			//TODO: check if unit is not in the same team
			Debug.Log(targetUnit.name);
			this.selectedUnit.Attack(targetUnit);
		}

	}

	private void Skill(){

	}

	// Use this for initialization
	void Start () {
		this.gameMechanic = gameObject.GetComponent<GameMechanic>();
		this.command = gameObject.GetComponent<Command>();
		Transform userInterface = GameObject.Find("UserInterface").transform;
		this.mainGame = userInterface.Find("MainGame").gameObject;
		this.miniMap = userInterface.Find("MiniMap").gameObject;

		//Attach Hexagon sript and coordinate to each tile
		/*int count = GameObject.Find("UserInterface").transform.Find("MiniMap").Find("MiniMap").childCount;
		for(int i = 0; i < count; i++){
			Transform child = GameObject.Find("UserInterface").transform.Find("MiniMap").Find("MiniMap").transform.GetChild(i);
			string[] coordinate = child.name.Split(',');
			Hexagon hexagon = child.gameObject.AddComponent<Hexagon>();
			hexagon.x = Int32.Parse(coordinate[0]);
			hexagon.y = Int32.Parse(coordinate[1]);
			hexagon.z = Int32.Parse(coordinate[2]);
			//Debug.Log(hexagon.x + " " + hexagon.y + " " + hexagon.z);
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
