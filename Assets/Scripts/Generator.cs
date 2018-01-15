using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Kudan.AR;
using UnityEngine.UI;

public class Generator : MonoBehaviour {

	//Generate map and return map gameobject
	public GameObject GenerateMap(){
		/*Hexagon hexagon;
		string[] coordinate;
		int count;
		Transform child;*/
		
		//Instantiate gameobject to hold map and transform it under correct hierachy
		/*GameObject mapObject = new GameObject();
		mapObject.name = "Map";
		MarkerTransformDriver marker = mapObject.AddComponent<MarkerTransformDriver>();
		marker._expectedId = "Test";
		//mapObject.transform.parent = GameObject.Find("Drivers").transform;*/
		//Instantiate map
		GameObject mapPrefab = (GameObject)Resources.Load("Prefabs/Map", typeof(GameObject));
		GameObject map = (GameObject)Instantiate(mapPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		map.name = "Map";
		map.transform.parent = GameObject.Find("Drivers").transform;
		//map.transform.parent = mapObject.transform;

		//Attach Hexagon sript and coordinate to each tile
		/*count = map.transform.childCount;
		for(int i = 0; i < count; i++){
			child = map.transform.GetChild(i);
			coordinate = child.name.Split(',');
			hexagon = child.gameObject.AddComponent<Hexagon>();
			hexagon.x = Int32.Parse(coordinate[0]);
			hexagon.y = Int32.Parse(coordinate[1]);
			hexagon.z = Int32.Parse(coordinate[2]);
			//Debug.Log(hexagon.x + " " + hexagon.y + " " + hexagon.z);
		}*/

		return map;
	}

	//Generate unit
	public Unit GenerateUnit(string unitType, string unitName, string team, Hexagon position){
		Hexagon tile;
		Vector3 origin = new Vector3(0, 0, 0);	//Default location if unit is not found
		GameObject map;

		//Instantiate gameobject to hold character and transform it under correct hierachy 
		/*GameObject characterObject = new GameObject();
		characterObject.name = unitName;
		MarkerTransformDriver marker = characterObject.AddComponent<MarkerTransformDriver>();
		//Add marker id
		marker._expectedId = unitName;
		characterObject.transform.parent = GameObject.Find("Drivers").transform;*/

		//Get position of tile to instantiate unit
		map = gameObject.GetComponent<GameMechanic>().map;
		foreach(Transform tiles in map.transform){
			tile = tiles.GetComponent<Hexagon>();
			if(tile.x == position.x && tile.y == position.y && tile.z == position.z){
				origin = tile.transform.position + new Vector3(0, 0.5f, 0);
			}
		}

		GameObject unit = (GameObject)Resources.Load("Prefabs/" + unitType, typeof(GameObject));
		unit = (GameObject)Instantiate(unit, origin, Quaternion.Euler(new Vector3(0, 0, 0)));
		/*unit = (GameObject)Instantiate(unit, origin, Quaternion.identity);
		unit.transform.LookAt(GameObject.Find("Main Camera").transform);*/
		unit.name = unitName;
		unit.transform.parent = GameObject.Find("Drivers").transform;
		//unit.transform.parent = characterObject.transform;

		//Add detail of unit
		Unit unitDetails = unit.AddComponent<Unit>();
		unitDetails.position = position;
		unitDetails.team = team;
		unitDetails.status = gameObject.GetComponent<Database>().unitStatus[unitType];
		unitDetails.hp = unitDetails.status.maxHp;
		//Add state of unit
		//Debug.Log(unitDetails.status.maxHp);
		return unitDetails;
	}

	public Image GenerateImage(String imageName, Vector3 position){
		Transform images = GameObject.Find("UserInterface").transform.Find("MiniMap").Find("Images").transform;
		Sprite sprite = Resources.Load<Sprite>("Images/" + imageName);
		GameObject imageObject = new GameObject();
		imageObject.name = imageName;
		imageObject.transform.parent = images;
		imageObject.transform.localPosition = position;
		imageObject.transform.localScale = new Vector3(1, 1, 1);
		Image image = imageObject.AddComponent<Image>();
		image.raycastTarget = false;
		image.sprite = sprite;
		return image;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
