﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using Newtonsoft.Json;

public class GameMechanic : MonoBehaviour {

	public const int MAX_PLAYER = 1;

	public Cube cube = new Cube();
	Generator generator;
	//public GameObject map;
	//Hexagon[] tiles;
	//private List<GameObject> hilightTiles = new List<GameObject>();
	public List<Unit> unit;
	public Unit selectedUnit; 
	private GameObject unitsButton;
	private TurnManager turnManager;
	private Player player;
	
	public void OnCharacterSelected(){
		Color white = Color.white;
		white.a = 0.5f;
		Color red = Color.red;
		red.a = 0.5f;
		//Debug.Log(EventSystem.current.currentSelectedGameObject.name);
		//Convert button color of previous selected unit to white
		if(this.selectedUnit != null){
			this.unitsButton.transform.Find(this.selectedUnit.name.Split(' ')[0]).GetComponent<Image>().color = white;
		}
		//Convert button color of current selected unit to red
		EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = red;

		//Search for unit
		foreach(Unit unit in this.unit){
			if(unit.unitName == EventSystem.current.currentSelectedGameObject.name + " Team" + this.player.team){
				this.selectedUnit = unit;
				break;
			}else{
				this.selectedUnit = null;
			}
		}
		//this.selectedUnit = GameObject.Find("Drivers").transform.Find(EventSystem.current.currentSelectedGameObject.name + " Team" + this.player.team).GetComponent<Unit>();
	}

	public void StartGame(){
		this.turnManager.currentTeamTurn = 1;
		this.turnManager.time = 90f;
		this.turnManager.turn = 1;


	}

	// Use this for initialization
	void Start () {
		this.unit = new List<Unit>();
		//Get generator
		this.generator = gameObject.GetComponent<Generator>();

		this.unitsButton = GameObject.Find("UserInterface").transform.Find("MainGame").Find("Units").gameObject;

		this.turnManager = gameObject.GetComponent<TurnManager>();

		this.player = GameObject.Find("Player").GetComponent<Player>();

		this.gameObject.GetComponent<Database>().ReadUnitStatus();

		StartGame();

		this.generator.GenerateUnitForEachTeam();
	}

	// Update is called once per frame
	void Update(){

	}
}
