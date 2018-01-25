using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Kudan.AR;
using UnityEngine.UI;
using Newtonsoft.Json;

public class Generator : MonoBehaviour {

	//Generate map and return map gameobject
	public GameObject GenerateMap(){
		//Instantiate map
		GameObject mapPrefab = (GameObject)Resources.Load("Prefabs/Map", typeof(GameObject));
		GameObject map = (GameObject)Instantiate(mapPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		map.name = "Map";
		map.transform.parent = GameObject.Find("Drivers").transform;
		//map.transform.parent = mapObject.transform;

		//Attach Hexagon sript and coordinate to each tile ***For adding new tile
		/*int count = map.transform.childCount;
		for(int i = 0; i < count; i++){
			Transform child = map.transform.GetChild(i);
			String[] coordinate = child.name.Split(',');
			Hexagon hexagon = child.gameObject.AddComponent<Hexagon>();
			hexagon.x = Int32.Parse(coordinate[0]);
			hexagon.y = Int32.Parse(coordinate[1]);
			hexagon.z = Int32.Parse(coordinate[2]);
			//Debug.Log(hexagon.x + " " + hexagon.y + " " + hexagon.z);
		}*/

		return map;
	}

	//Generate unit
	public Unit GenerateUnit(string unitType, string unitName, string team, Hexagon position){
		Vector3 origin = new Vector3(0, 0, 0);	//Default location if unit is not found

		//Instantiate gameobject to hold character and transform it under correct hierachy 
		/*GameObject characterObject = new GameObject();
		characterObject.name = unitName;
		MarkerTransformDriver marker = characterObject.AddComponent<MarkerTransformDriver>();
		//Add marker id
		marker._expectedId = unitName;
		characterObject.transform.parent = GameObject.Find("Drivers").transform;*/

		//Get position of tile to instantiate unit
		origin = gameObject.GetComponent<GameMechanic>().map.transform.Find(position.x + "," + position.y + "," + position.z)
				.position + new Vector3(0, 0.5f, 0);

		GameObject unit = (GameObject)Resources.Load("Prefabs/" + unitType, typeof(GameObject));
		unit = (GameObject)Instantiate(unit, origin, Quaternion.Euler(new Vector3(0, 0, 0)));
		/*unit = (GameObject)Instantiate(unit, origin, Quaternion.identity);
		unit.transform.LookAt(GameObject.Find("Main Camera").transform);*/
		unit.name = unitName;
		unit.transform.parent = GameObject.Find("Drivers").transform;
		//unit.transform.parent = characterObject.transform;

		//Add detail of unit
		Unit unitDetails = unit.AddComponent<Unit>();
		unitDetails.unitName = unitName;
		unitDetails.position = position;
		unitDetails.team = team;
		unitDetails.status = gameObject.GetComponent<Database>().unitStatus[unitType];
		unitDetails.hp = unitDetails.status.maxHp;
		//TODO: Add state of unit

		GenerateImage(unitType, unitName, position);
		return unitDetails;
	}

	public Image GenerateImage(String imageName, String name, Hexagon position){
		GameObject miniMap = GameObject.Find("UserInterface").transform.Find("MiniMap").Find("MiniMap").gameObject;
		Sprite sprite = Resources.Load<Sprite>("Images/" + imageName);
		GameObject imageObject = new GameObject();
		imageObject.name = name;
		Transform tile = miniMap.transform.Find(position.x + "," + position.y + "," + position.z);
		Image image = imageObject.AddComponent<Image>();
		image.raycastTarget = false;
		image.sprite = sprite;
		imageObject.transform.SetParent(tile);
		imageObject.GetComponent<RectTransform>().position = tile.GetComponent<RectTransform>().position;
		imageObject.transform.localScale = new Vector3(1, 1, 1);
		return image;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
