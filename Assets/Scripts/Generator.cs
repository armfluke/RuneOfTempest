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
	public GameObject drivers;
	public GameObject[] maps;
	public GameObject[] environments;
	/*private Vector3[] startAngle = new Vector3[]{
		new Vector3(0, 225, 0), new Vector3(0, 135, 0), new Vector3(0, 45, 0), new Vector3(0, 315, 0)
	};*/
	private Vector3[] startAngle = new Vector3[]{
		new Vector3(0, 225, 0), new Vector3(0, 45, 0), new Vector3(0, 135, 0), new Vector3(0, 315, 0)
	};

	// private Hexagon[][] positionForEachTeam = new Hexagon[][]{
	// 	new Hexagon[]{new Hexagon(-6,-1,7), /*new Hexagon(-5,-2,7),*/ new Hexagon(-5,-1,6), new Hexagon(-5,0,5), new Hexagon(-6,1,5), /*new Hexagon(-7,2,5),*/ new Hexagon(-7,1,6)},
	// 	new Hexagon[]{new Hexagon(-1,-6,7), /*new Hexagon(-2,-5,7),*/ new Hexagon(-1,-5,6), new Hexagon(0,-5,5), new Hexagon(1,-6,5), /*new Hexagon(2,-7,5),*/ new Hexagon(1,-7,6)},
	// 	new Hexagon[]{new Hexagon(7,-1,-6), /*new Hexagon(7,-2,-5),*/ new Hexagon(6,-1,-5), new Hexagon(5,0,-5), new Hexagon(5,1,-6), /*new Hexagon(5,2,-7),*/ new Hexagon(6,1,-7)},
	// 	new Hexagon[]{new Hexagon(1,6,-7), /*new Hexagon(2,5,-7),*/ new Hexagon(1,5,-6), new Hexagon(0,5,-5), new Hexagon(-1,6,-5), /*new Hexagon(-2,7,-5),*/ new Hexagon(-1,7,-6)}
	// };
	private Hexagon[][] positionForEachTeam = new Hexagon[][]{
		new Hexagon[]{new Hexagon(-6,-1,7), /*new Hexagon(-5,-2,7),*/ new Hexagon(-5,-1,6), new Hexagon(-5,0,5), new Hexagon(-6,1,5), /*new Hexagon(-7,2,5),*/ new Hexagon(-7,1,6)},
		new Hexagon[]{new Hexagon(7,-1,-6), /*new Hexagon(7,-2,-5),*/ new Hexagon(6,-1,-5), new Hexagon(5,0,-5), new Hexagon(5,1,-6), /*new Hexagon(5,2,-7),*/ new Hexagon(6,1,-7)},
		new Hexagon[]{new Hexagon(-1,-6,7), /*new Hexagon(-2,-5,7),*/ new Hexagon(-1,-5,6), new Hexagon(0,-5,5), new Hexagon(1,-6,5), /*new Hexagon(2,-7,5),*/ new Hexagon(1,-7,6)},
		new Hexagon[]{new Hexagon(1,6,-7), /*new Hexagon(2,5,-7),*/ new Hexagon(1,5,-6), new Hexagon(0,5,-5), new Hexagon(-1,6,-5), /*new Hexagon(-2,7,-5),*/ new Hexagon(-1,7,-6)}
	};

	//Generate map and return map gameobject
	public GameObject GenerateMap(){
		//Instantiate map
		/*GameObject mapPrefab = (GameObject)Resources.Load("Prefabs/Map", typeof(GameObject));
		GameObject map = (GameObject)Instantiate(mapPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		map.name = "Map";
		map.transform.parent = GameObject.Find("Drivers").transform;*/

		System.Random random = new System.Random();
		int mapNumber = random.Next(0, 3);
		
		GameObject map = (GameObject)Instantiate(this.maps[mapNumber], new Vector3(0, 0, 0), Quaternion.identity);
		map.name = "Map";
		map.transform.parent = this.drivers.transform;

		GameObject environment = (GameObject)Instantiate(this.environments[mapNumber], new Vector3(0, 0, 0), Quaternion.identity);
		environment.name = "Environment";
		environment.transform.parent = this.drivers.transform;

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
		GameMechanic gameMechanic = gameObject.GetComponent<GameMechanic>();
		Vector3 origin = Vector3.zero;	//Default location if unit is not found
		//Get position of tile to instantiate unit
		origin = this.drivers.transform.Find("Map").Find(position.x + "," + position.y + "," + position.z)
				.position + new Vector3(0, 0.5f, 0);

		//Create an object to hold unit and health
		GameObject unitObject = new GameObject();
		unitObject.name = unitName;
		unitObject.transform.position = origin;
		unitObject.transform.SetParent(this.drivers.transform);

		//Create unit
		GameObject unit = (GameObject)Resources.Load("Prefabs/Units/" + unitType, typeof(GameObject));
		unit = (GameObject)Instantiate(unit, origin, Quaternion.Euler(Vector3.zero));
		unit.name = unitName;
		unit.transform.SetParent(unitObject.transform);

		//Create health bar
		GameObject health = (GameObject)Resources.Load("Prefabs/Health");
		health = (GameObject)Instantiate(health, origin, Quaternion.Euler(Vector3.zero));
		health.name = "Health";
		health.GetComponent<RectTransform>().position = unit.transform.position + new Vector3(0, 5, 0);
		health.transform.SetParent(unitObject.transform);

		Image healthColor = health.transform.Find("Background").Find("Foreground").GetComponent<Image>();
		Color green = Color.green;
		Color blue = Color.cyan;
		Color yellow = Color.yellow;
		Color magenta = Color.magenta;
		Color white = Color.white;
		//Change color of health bar depending on team
		switch(team){
			case 1:
				healthColor.color = green;
				break;
			case 2:
				healthColor.color = blue;
				break;
			case 3:
				healthColor.color = yellow;
				break;
			case 4:
				healthColor.color = magenta;
				break;
			default:
				healthColor.color = white;
				break;
		}

		//Add state sign to unit
		GameObject state = (GameObject)Resources.Load("Prefabs/State");
		state = (GameObject)Instantiate(state, origin, Quaternion.Euler(Vector3.zero));
		state.name = "State";
		state.GetComponent<RectTransform>().position = health.GetComponent<RectTransform>().position + new Vector3(-1.5f, 0, 0);
		state.transform.SetParent(health.transform);

		//Add detail of unit
		Unit unitDetails = unitObject.AddComponent<Unit>();
		unitDetails.unitName = unitName;
		unitDetails.position = position;
		unitDetails.team = team;
		unitDetails.status = gameObject.GetComponent<Database>().status[unitType];
		unitDetails.hp = unitDetails.status.maxHp;

		GenerateImage(unitType, unitName, team, position);

		gameMechanic.unit.Add(unitDetails);

		return unitDetails;
	}

	public void GenerateImage(String imageName, String name, int team, Hexagon position){
		//Create unit image on minimap
		GameObject miniMap = GameObject.Find("UserInterface").transform.Find("MiniMap").Find("MiniMap").gameObject;
		Sprite sprite = Resources.Load<Sprite>("Images/Units/" + imageName);
		GameObject imageObject = new GameObject();
		imageObject.name = name;
		Transform tile = miniMap.transform.Find(position.x + "," + position.y + "," + position.z);
		Image image = imageObject.AddComponent<Image>();
		image.raycastTarget = false;
		image.sprite = sprite;
		imageObject.transform.SetParent(tile);
		imageObject.GetComponent<RectTransform>().position = tile.GetComponent<RectTransform>().position;
		imageObject.transform.localScale = new Vector3(1, 1, 1);
		imageObject.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
		
		//Create diamonds to identify team on minimap
		sprite = Resources.Load<Sprite>("Images/Diamond");
		GameObject diamondObject = new GameObject();
		diamondObject.name = "Team";
		image = diamondObject.AddComponent<Image>();
		image.raycastTarget = false;
		image.sprite = sprite;
		diamondObject.transform.SetParent(imageObject.transform);
		diamondObject.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 30);
		diamondObject.GetComponent<RectTransform>().position = imageObject.GetComponent<RectTransform>().position + new Vector3(0, -9.5f, 0);
		diamondObject.transform.localScale = new Vector3(1, 1, 1);


		Color green = Color.green;
		Color blue = Color.cyan;
		Color yellow = Color.yellow;
		Color magenta = Color.magenta;
		Color white = Color.white;
		//Change color of diamond depending on team
		switch(team){
			case 1:
				image.color = green;
				break;
			case 2:
				image.color = blue;
				break;
			case 3:
				image.color = yellow;
				break;
			case 4:
				image.color = magenta;
				break;
			default:
				image.color = white;
				break;
		}
	}

	public void GenerateUnitForEachTeam(){
		for(int i = 0; i < GameMechanic.MAX_PLAYER; i++){
			int index = 0;
			foreach(Hexagon position in positionForEachTeam[i]){
				Unit unit = GenerateUnit("Villager", "Unit" + (index+1) + " Team" + (i+1), (i+1), position);
				unit.transform.eulerAngles = startAngle[i];
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
