using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Kudan.AR;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using Newtonsoft.Json;

public class GameMechanic : MonoBehaviour {

	public Cube cube = new Cube();
	Generator generator;
	public GameObject map;
	//Hexagon[] tiles;
	private List<GameObject> hilightTiles = new List<GameObject>();
	public List<Unit> unit = new List<Unit>();
	public Unit selectedUnit; 
	public GameObject unitsButton;
	
	public void OnCharacterSelected(){
		Color white = Color.white;
		white.a = 0.5f;
		Color red = Color.red;
		red.a = 0.5f;
		//Debug.Log(EventSystem.current.currentSelectedGameObject.name);
		//Convert button color of previous selected unit to white
		if(this.selectedUnit != null){
			this.unitsButton.transform.Find(this.selectedUnit.name).GetComponent<Image>().color = white;
		}
		//Convert button color of current selected unit to red
		EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = red;
		this.selectedUnit = GameObject.Find("Drivers").transform.Find(EventSystem.current.currentSelectedGameObject.name).GetComponent<Unit>();
	}

	// Use this for initialization
	void Start () {
		//Get generator
		this.generator = gameObject.GetComponent<Generator>();

		this.unitsButton = GameObject.Find("UserInterface").transform.Find("MainGame").Find("Units").gameObject;

		//this.map = this.generator.GenerateMap();
		this.map = GameObject.Find("Drivers").transform.Find("Map").gameObject;

		this.gameObject.GetComponent<Database>().ReadUnitStatus();

		Unit golem = this.generator.GenerateUnit("Golem", "Unit1", 1, new Hexagon(0, 0, 0));
		this.unit.Add(golem);
		Unit golem2 = this.generator.GenerateUnit("Golem", "Unit2", 2, new Hexagon(-4, 3, 1));
		this.unit.Add(golem2);

		//At start game select first unit as a selected unit
		/*this.selectedUnit = unit[0];
		Color red = Color.red;
		red.a = 0.5f;
		this.unitsButton.transform.Find(this.selectedUnit.name).GetComponent<Image>().color = red;*/
	}

	// Update is called once per frame
	void Update(){

	}
}
