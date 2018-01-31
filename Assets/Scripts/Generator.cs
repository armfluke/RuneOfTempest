using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

public class Generator : MonoBehaviour {

	private Player player;
	private int countTeam = 0;

	private Hexagon[][] positionForEachTeam = new Hexagon[][]{
		new Hexagon[]{new Hexagon(-6,-1,7), new Hexagon(-5,-2,7), new Hexagon(-5,-1,6), new Hexagon(-5,0,5), new Hexagon(-6,1,5), new Hexagon(-7,2,5), new Hexagon(-7,1,6)},
		new Hexagon[]{new Hexagon(-1,-6,7), new Hexagon(-2,-5,7), new Hexagon(-1,-5,6), new Hexagon(0,-5,5), new Hexagon(1,-6,5), new Hexagon(2,-7,5), new Hexagon(1,-7,6)},
		new Hexagon[]{new Hexagon(7,-1,-6), new Hexagon(7,-2,-5), new Hexagon(6,-1,-5), new Hexagon(5,0,-5), new Hexagon(5,1,-6), new Hexagon(5,2,-7), new Hexagon(6,1,-7)},
		new Hexagon[]{new Hexagon(1,6,-7), new Hexagon(2,5,-7), new Hexagon(1,5,-6), new Hexagon(0,5,-5), new Hexagon(-1,6,-5), new Hexagon(-2,7,-5), new Hexagon(-1,7,-6)}
	};

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
	public Unit GenerateUnit(string unitType, string unitName, int team, Hexagon position){
		Vector3 origin = new Vector3(0, 0, 0);	//Default location if unit is not found

		//Get position of tile to instantiate unit
		origin = GameObject.Find("Drivers").transform.Find("Map").Find(position.x + "," + position.y + "," + position.z)
				.position + new Vector3(0, 0.5f, 0);

		GameObject unit = (GameObject)Resources.Load("Prefabs/" + unitType, typeof(GameObject));
		unit = (GameObject)Instantiate(unit, origin, Quaternion.Euler(new Vector3(0, 0, 0)));
		unit.name = unitName;
		unit.transform.parent = GameObject.Find("Drivers").transform;

		//Add detail of unit
		Unit unitDetails = unit.AddComponent<Unit>();
		unitDetails.unitName = unitName;
		unitDetails.position = position;
		unitDetails.team = team;
		unitDetails.status = gameObject.GetComponent<Database>().unitStatus[unitType];
		unitDetails.hp = unitDetails.status.maxHp;

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

	public void GenerateUnitForEachTeam(){
		GameMechanic gameMechanic = gameObject.GetComponent<GameMechanic>();
		for(int i = 0; i < positionForEachTeam.Length; i++){
			int index = 0;
			foreach(Hexagon position in positionForEachTeam[i]){
				Unit unit = GenerateUnit("Golem", "Unit" + (index+1) + " Team" + (i+1), (i+1), position);
				gameMechanic.unit.Add(unit);
				index++;
			}
		}


	}

	/*void OnGUI(){
        GUI.Label(new Rect(2, 10, 150, 100), ""+NetworkServer.connections.Count);
	}*/

	// Use this for initialization
	void Start () {
		this.player = GameObject.Find("Player").GetComponent<Player>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
