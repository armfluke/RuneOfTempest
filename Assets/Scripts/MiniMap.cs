using System.Collections;
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

	public void Back(){
		GameObject userInterface = GameObject.Find("UserInterface");
		userInterface.transform.Find("MiniMap").gameObject.SetActive(false);
		userInterface.transform.Find("UnitDetails").gameObject.SetActive(true);		
		//TODO: reset all command when back
		if(this.command.move){
			ResetMiniMapHilightForMoving();
		}else if(this.command.attack){
			ResetMiniMapHilightForAttacking();
		}
	}

	public void OnMiniMapSelected(){
		GameObject userInterface = GameObject.Find("UserInterface");
		GameObject mainGame = userInterface.transform.Find("MainGame").gameObject;
		GameObject miniMap = userInterface.transform.Find("MiniMap").gameObject;
		Debug.Log(EventSystem.current.currentSelectedGameObject.name);
		this.tile = EventSystem.current.currentSelectedGameObject;
		this.hexagon = EventSystem.current.currentSelectedGameObject.GetComponent<Hexagon>();
		this.selectedUnit = this.gameMechanic.selectedUnit;

		if(this.command.move == true){
			Hexagon[] range = this.gameMechanic.cube.MovementRange(this.selectedUnit.position, this.selectedUnit.status.move);
			bool checkRange = false;
			foreach(Hexagon hex in range){
				if(this.hexagon.x == hex.x && this.hexagon.y == hex.y && this.hexagon.z == hex.z){
					checkRange = true;
					break;
				}
			}
			
			if(checkRange && !unitChecking(this.hexagon)){
				ResetMiniMapHilightForMoving();
				miniMap.SetActive(false);
				mainGame.SetActive(true);
				this.command.move = false;
				Move();
			}
		}else if(this.command.attack == true){
			Unit targetUnit = SearchUnit(this.hexagon);
			if(targetUnit && targetUnit != this.selectedUnit){
				//TODO: check if unit is not in the same team**********************************
				Debug.Log(targetUnit.name);
				miniMap.SetActive(false);
				mainGame.SetActive(true);
				this.command.attack = false;
				Attack(targetUnit);
			}
		}else if(this.command.skill == true){
			miniMap.SetActive(false);
			mainGame.SetActive(true);
			this.command.skill = false;
			Skill();
		}
	}

	private bool unitChecking(Hexagon hexagon){
		foreach(Unit unit in this.gameMechanic.unit){
			if(unit.position.x == hexagon.x && unit.position.y == hexagon.y && unit.position.z == hexagon.z){
				return true;
			}
		}
		return false;
	}

	public void HilightMiniMapForMoving(){
		GameObject userInterface = GameObject.Find("UserInterface");
		Transform miniMap = userInterface.transform.Find("MiniMap").Find("MiniMap");
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
		GameObject userInterface = GameObject.Find("UserInterface");
		Transform miniMap = userInterface.transform.Find("MiniMap").Find("MiniMap");
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

	private void Move(){
		GameObject userInterface = GameObject.Find("UserInterface");
		Transform miniMap = userInterface.transform.Find("MiniMap").Find("MiniMap");
		Hexagon position = this.selectedUnit.position;
		Transform unitImage = miniMap.transform.Find(position.x + "," + position.y + "," + position.z)
							.Find(this.selectedUnit.name).transform;
		unitImage.parent = this.tile.transform;
		unitImage.GetComponent<RectTransform>().position = this.tile.GetComponent<RectTransform>().position;
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

	public void HilightMiniMapForAttacking(){
		//TODO: Check team
		GameObject userInterface = GameObject.Find("UserInterface");
		Transform miniMap = userInterface.transform.Find("MiniMap").Find("MiniMap");
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
		GameObject userInterface = GameObject.Find("UserInterface");
		Transform miniMap = userInterface.transform.Find("MiniMap").Find("MiniMap");
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

	private void Attack(Unit targetUnit){
		this.selectedUnit.Attack(targetUnit);

	}

	private void Skill(){

	}

	// Use this for initialization
	void Start () {
		this.gameMechanic = gameObject.GetComponent<GameMechanic>();
		this.command = gameObject.GetComponent<Command>();

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
