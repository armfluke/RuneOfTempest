﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class MiniMap : MonoBehaviour {

	private GameObject tile;
	private Hexagon hexagon;
	private Command command;
	private GameMechanic gameMechanic;
	private Unit selectedUnit;
	private Network network;
	GameObject userInterface;
	GameObject miniMap;
	GameObject mainGame;

	public void Back(){
		this.userInterface.transform.Find("MiniMap").gameObject.SetActive(false);
		this.userInterface.transform.Find("UnitDetails").gameObject.SetActive(true);		
		//TODO: reset all command when back
		if(this.command.move){
			ResetMiniMapHilightForMoving();
			this.command.move = false;
		}else if(this.command.attack){
			ResetMiniMapHilightForAttacking();
			this.command.attack = false;
		}
	}

	//function to check if range is valid
	public bool CheckRange(Hexagon[] range){
		foreach(Hexagon hex in range){
			if(this.hexagon.x == hex.x && this.hexagon.y == hex.y && this.hexagon.z == hex.z){
				return true;
			}
		}
		return false;
	}

	public void OnMiniMapSelected(){

		bool checkRange;
		Hexagon[] range;
		
		Debug.Log(EventSystem.current.currentSelectedGameObject.name);
		this.tile = EventSystem.current.currentSelectedGameObject;
		this.hexagon = EventSystem.current.currentSelectedGameObject.GetComponent<Hexagon>();
		this.selectedUnit = this.gameMechanic.selectedUnit;

		if(this.command.move == true){
			range = this.gameMechanic.cube.MovementRange(this.selectedUnit.position, this.selectedUnit.status.move);
			checkRange = CheckRange(range);
			
			if(checkRange && !UnitChecking(this.hexagon)){
				ResetMiniMapHilightForMoving();
				this.miniMap.SetActive(false);
				this.mainGame.SetActive(true);
				this.command.move = false;
				
				this.network.MoveUnit(this.selectedUnit.position, this.hexagon);
				//Move();
			}
		}else if(this.command.attack == true){
			range = this.gameMechanic.cube.MovementRange(this.selectedUnit.position, this.selectedUnit.status.range);
			checkRange = CheckRange(range);

			Unit targetUnit = SearchUnit(this.hexagon);
			if(targetUnit && checkRange && targetUnit.team != this.selectedUnit.team){
				ResetMiniMapHilightForAttacking();
				Debug.Log("Attack "+targetUnit.name);
				this.miniMap.SetActive(false);
				this.mainGame.SetActive(true);
				this.command.attack = false;


				this.network.AttackUnit(this.selectedUnit.position, targetUnit.position);
				//Attack(targetUnit);
			}
		}else if(this.command.skill == true){
			this.miniMap.SetActive(false);
			this.mainGame.SetActive(true);
			this.command.skill = false;
			Skill();
		}
	}

	private bool UnitChecking(Hexagon hexagon){
		foreach(Unit unit in this.gameMechanic.unit){
			if(unit.position.x == hexagon.x && unit.position.y == hexagon.y && unit.position.z == hexagon.z){
				return true;
			}
		}
		return false;
	}

	public void HilightMiniMapForMoving(){
		Transform miniMap = this.miniMap.transform.Find("MiniMap");
		Color green = Color.green;
		green.a = 0.65f;
		Color blue = Color.blue;
		blue.a = 0.65f;

		Hexagon[] range = this.gameMechanic.cube.MovementRange(this.gameMechanic.selectedUnit.position, this.gameMechanic.selectedUnit.status.move);
		foreach(Hexagon tile in range){
			Transform hexagon = miniMap.transform.Find(tile.x + "," + tile.y + "," + tile.z);
			if(hexagon){
				hexagon.Find("Hilight").GetComponent<Image>().color = green;
			}
		}

		miniMap.transform.Find(this.gameMechanic.selectedUnit.position.x + "," + this.gameMechanic.selectedUnit.position.y + "," + this.gameMechanic.selectedUnit.position.z)
			.Find("Hilight").GetComponent<Image>().color = blue;
	}
	
	public void ResetMiniMapHilightForMoving(){
		Transform miniMap = this.miniMap.transform.Find("MiniMap");
		Color white = Color.white;
		white.a = 0.275f;

		Hexagon[] range = this.gameMechanic.cube.MovementRange(this.gameMechanic.selectedUnit.position, this.gameMechanic.selectedUnit.status.move);
		foreach(Hexagon tile in range){
			Transform hexagon = miniMap.Find(tile.x + "," + tile.y + "," + tile.z);
			if(hexagon){
				hexagon.Find("Hilight").GetComponent<Image>().color = white;
			}
		}
	}

	public void Move(Hexagon from, Hexagon to){
		Unit unit = SearchUnit(from);

		//Move unit image on minimap
		Transform miniMap = this.miniMap.transform.Find("MiniMap");
		Transform unitImage = miniMap.transform.Find(unit.position.x + "," + unit.position.y + "," + unit.position.z)
							.Find(unit.name).transform;

		Transform targetTile = miniMap.transform.Find(to.x + "," + to.y + "," + to.z);
		unitImage.SetParent(targetTile.transform);
		unitImage.GetComponent<RectTransform>().position = targetTile.GetComponent<RectTransform>().position;

		unit.Move(to);

		//GameObject.Find("Drivers").transform.Find(unitName).GetComponent<Unit>().Move(to);

		/*foreach(Unit unit in this.gameMechanic.unit){
			Debug.Log(unit.position.x+","+unit.position.y+","+unit.position.z+"||"+from.x+","+from.y+","+from.z+"||"+(unit.position.x == from.x && unit.position.x == from.y && unit.position.x == from.z));
			if(unit.position.x == from.x && unit.position.x == from.y && unit.position.x == from.z){
				Debug.Log("Unit found: "+unit.name);
				unit.Move(to);
				break;
			}
		}*/
	}

	/*private void Move(){
		GameObject userInterface = GameObject.Find("UserInterface");
		Transform miniMap = userInterface.transform.Find("MiniMap").Find("MiniMap");
		Hexagon position = this.selectedUnit.position;
		Transform unitImage = miniMap.transform.Find(position.x + "," + position.y + "," + position.z)
							.Find(this.selectedUnit.name).transform;
		unitImage.parent = this.tile.transform;
		unitImage.GetComponent<RectTransform>().position = this.tile.GetComponent<RectTransform>().position;
		this.selectedUnit.Move(this.hexagon);
	}*/

	private Unit SearchUnit(Hexagon position){
		foreach(Unit unit in this.gameMechanic.unit){
			if(unit.position.x == position.x && unit.position.y == position.y && unit.position.z == position.z){
				return unit;
			}
		}
		return null;
	}

	public void HilightMiniMapForAttacking(){
		//TODO: Check team
		Transform miniMap = this.miniMap.transform.Find("MiniMap");
		Color red = Color.red;
		red.a = 0.65f;
		Color blue = Color.blue;
		blue.a = 0.65f;

		Hexagon[] range = this.gameMechanic.cube.MovementRange(this.gameMechanic.selectedUnit.position, this.gameMechanic.selectedUnit.status.range);
		foreach(Hexagon tile in range){
			Transform hexagon = miniMap.transform.Find(tile.x + "," + tile.y + "," + tile.z);
			if(hexagon){
				hexagon.Find("Hilight").GetComponent<Image>().color = red;
			}
		}

		miniMap.transform.Find(this.gameMechanic.selectedUnit.position.x + "," + this.gameMechanic.selectedUnit.position.y + "," + this.gameMechanic.selectedUnit.position.z)
			.Find("Hilight").GetComponent<Image>().color = blue;
	}

	public void ResetMiniMapHilightForAttacking(){
		Transform miniMap = this.miniMap.transform.Find("MiniMap");
		Color white = Color.white;
		white.a = 0.275f;

		Hexagon[] range = this.gameMechanic.cube.MovementRange(this.gameMechanic.selectedUnit.position, this.gameMechanic.selectedUnit.status.range);
		foreach(Hexagon tile in range){
			Transform hexagon = miniMap.Find(tile.x + "," + tile.y + "," + tile.z);
			if(hexagon){
				hexagon.Find("Hilight").GetComponent<Image>().color = white;
			}
		}
	}

	public void Attack(Hexagon from, Hexagon to){
		Unit unit = SearchUnit(from);
		Unit targetUnit = SearchUnit(to);
		unit.Attack(targetUnit);
	}

	/*private void Attack(Unit targetUnit){
		this.selectedUnit.Attack(targetUnit);
	}*/

	private void Skill(){

	}

	// Use this for initialization
	void Start () {
		this.gameMechanic = gameObject.GetComponent<GameMechanic>();
		this.command = gameObject.GetComponent<Command>();
		this.network = GameObject.Find("NetworkManager").GetComponent<Network>();
		this.userInterface = GameObject.Find("UserInterface");
		this.mainGame = userInterface.transform.Find("MainGame").gameObject;
		this.miniMap = userInterface.transform.Find("MiniMap").gameObject;

		//Attach Hexagon sript and coordinate to each tile ***For adding new tile
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

		//Add hilight to mimap tile ***use when creting new minimap
		/*GameObject hilightPrefab = (GameObject)Resources.Load("Prefabs/Hilight", typeof(GameObject));
		Transform miniMap = this.miniMap.transform.Find("MiniMap");
		Color white = Color.white;
		white.a = 0.275f;
		foreach(Transform tile in miniMap){
			//GameObject.Destroy(tile.Find("Hilight").gameObject);
			GameObject hilight = (GameObject)Instantiate(hilightPrefab, new Vector3(0, 0, 0), Quaternion.identity);
			hilight.transform.parent = tile;
			hilight.name = "Hilight";
			hilight.transform.localScale = new Vector3(1, 1, 1);
			hilight.transform.Rotate(0, 0, 90f);
			hilight.GetComponent<RectTransform>().position = tile.GetComponent<RectTransform>().position - new Vector3(0.75f, 0, 0);
			hilight.GetComponent<Image>().color = white;
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
