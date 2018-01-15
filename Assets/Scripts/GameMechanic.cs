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

	GameObject SearchTile(int x, int y, int z){
		Transform tile = this.map.transform.Find(x + "," + y + "," + z);
		if(tile){
			return tile.gameObject;
		}
		return null;
	}

	void ChangeTileColor(GameObject tile, Color color){
		tile.GetComponent<Renderer>().material.color = color;
	}

	void UnhilightTiles(){
		int count = this.hilightTiles.Count;
		for(int i = 0; i < count; i++){
			ChangeTileColor(this.hilightTiles[i], Color.white);
		}
		this.hilightTiles.Clear();
	}

	void HilightTiles(Hexagon[] tiles){
		GameObject tile;

		if(this.hilightTiles.Count != 0){
			UnhilightTiles();
		}

		for(int i=0; i<tiles.Length; i++){
			//Debug.Log(tiles[i].x + " " + tiles[i].y + " " + tiles[i].z);
			tile = SearchTile(tiles[i].x, tiles[i].y, tiles[i].z);
			if(tile){
				this.hilightTiles.Add(tile);
				ChangeTileColor(tile, Color.red);
			}
		}
	}

	/*public void OnMiniMapSelected(){
		Debug.Log(EventSystem.current.currentSelectedGameObject.name);
	}*/

	// Use this for initialization
	void Start () {
		//Get generator
		this.generator = gameObject.GetComponent<Generator>();

		this.map = this.generator.GenerateMap();
		
		//HilightTiles(this.cubeHelper.Neighbor(new Hexagon(0, 0, 0)));

		Unit golem = this.generator.GenerateUnit("Golem", "GolemTest", "TestTeam", new Hexagon(0, 0, 0));
		this.unit.Add(golem);
		Unit golem2 = this.generator.GenerateUnit("Golem", "GolemTest2", "TestTeam2", new Hexagon(-4, 3, 1));
		this.unit.Add(golem2);
		this.selectedUnit = unit[0];

		this.generator.GenerateImage("Golem", new Vector3(0, 0, 0));
	}

	// Update is called once per frame
	void Update () {
		//this.unit[0].Move(new Hexagon(-5, 5, 0));

		//Raycast for detecting minimap
		/*if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0)){
			Ray ray;
			if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)){
				ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			}else{
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			}
			RaycastHit hitInfo;
			if(Physics.Raycast(ray, out hitInfo)){
				if(hitInfo.transform.parent.name == "MiniMap"){
					Debug.Log(hitInfo.transform.name);
				}
			}
		}*/
	}
}
