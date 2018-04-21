using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Command : MonoBehaviour {
	public bool move = false;
	public bool attack = false;
	public bool skill = false;
	private GameMechanic gameMechanic;
	private GameObject userInterface;
	private MiniMap miniMap;
	private Network network;
	public GameObject classObject;
	public Skill skillObject;

	public void Select(){
		GameObject unitDetails = this.userInterface.transform.Find("UnitDetails").gameObject;
		Database database = gameObject.GetComponent<Database>();
		Unit unit = this.gameMechanic.selectedUnit;
		if(unit != null && (unit.state == "Idle" || unit.state == "Move")){
			this.userInterface.transform.Find("MainGame").gameObject.SetActive(false);
			unitDetails.SetActive(true);
			//Change unit image to selected unit
			Image unitImage = unitDetails.transform.Find("UnitImage").Find("Image").GetComponent<Image>();
			unitImage.sprite = Resources.Load<Sprite>("Images/Units/" + unit.status.type);
			//Change description to selected unit
			Transform description = unitDetails.transform.Find("Description");
			description.Find("Class").GetComponent<Text>().text = unit.status.type;
			description.Find("Hp").GetComponent<Text>().text = unit.hp + "/" + unit.status.maxHp;
			description.Find("Move").GetComponent<Text>().text = unit.status.move.ToString();
			description.Find("Attack").GetComponent<Text>().text = unit.status.attack.ToString();
			description.Find("Range").GetComponent<Text>().text = unit.status.range.ToString();
			description.Find("Cooldown").GetComponent<Text>().text = unit.cooldown.ToString();
			string skill = "";
			for(int i=0; i<unit.status.skill.Length; i++){
				if(i != 0){
					skill += "\n";
				}
				if(database.skill[unit.status.skill[i]].type == "Active"){
					skill += unit.status.skill[i] + "(" + database.skill[unit.status.skill[i]].type + ")(CD: " + database.skill[unit.status.skill[i]].cooldown + "): " + database.skill[unit.status.skill[i]].description;
				}else{
					skill += unit.status.skill[i] + "(" + database.skill[unit.status.skill[i]].type + "): " + database.skill[unit.status.skill[i]].description;
				}
			}
			unitDetails.transform.Find("Skill").Find("Text").GetComponent<Text>().text = skill;
			
		}
	}

	//When user select back reset all command
	public void Back(){
		this.move = false;
		this.attack = false;
		this.skill = false;
		this.userInterface.transform.Find("UnitDetails").gameObject.SetActive(false);
		this.userInterface.transform.Find("MainGame").gameObject.SetActive(true);
	}

	public void Move(){
		this.move = true;
		this.userInterface.transform.Find("UnitDetails").gameObject.SetActive(false);
		this.userInterface.transform.Find("MiniMap").gameObject.SetActive(true);
		this.miniMap.HilightMiniMapForMoving();
		Debug.Log("Move");
	}

	public void Attack(){
		this.attack = true;
		this.miniMap.HilightMiniMapForAttacking();
		this.userInterface.transform.Find("UnitDetails").gameObject.SetActive(false);
		this.userInterface.transform.Find("MiniMap").gameObject.SetActive(true);
		Debug.Log("Attack");
	}

	public void Defend(){
		this.network.SendDefendCommandMessage(this.gameMechanic.selectedUnit);
		this.userInterface.transform.Find("UnitDetails").gameObject.SetActive(false);
		this.userInterface.transform.Find("MainGame").gameObject.SetActive(true);
	}

	public void Skill(){
		if(this.gameMechanic.selectedUnit.cooldown == 0){
			this.skill = true;
			this.skillObject.Call(this.gameMechanic.selectedUnit.status.skill[0]);
		}
	}

	public void Class(){
		if(this.gameMechanic.selectedUnit.status.availableClass.Length != 0){
			/*Transform foundedClass;
			//Loop to enable button only available class
			foreach(string availableClass in this.gameMechanic.selectedUnit.status.availableClass){
				foundedClass = null;
				foundedClass = this.classObject.transform.Find(availableClass);
				if(foundedClass != null){
					foundedClass.gameObject.GetComponent<Button>().interactable = true;
				}else{
					foundedClass.gameObject.GetComponent<Button>().interactable = false;
				}
			}*/
			foreach(Transform child in this.classObject.transform){
				if(child.name != "Path"){
					bool foundedClass = false;
					foreach(string availableClass in this.gameMechanic.selectedUnit.status.availableClass){
						if(child.name == availableClass || child.name == "Back"){
							foundedClass = true;
						}
					}
					if(foundedClass == true){
						child.GetComponent<Button>().interactable = true;
					}else{
						child.GetComponent<Button>().interactable = false;
					}
				}
			}
		}else{
			foreach(Transform child in this.classObject.transform){
				if(child.name != "Back"){
					child.GetComponent<Button>().interactable = false;
				}
			}
		}

		//swap screen
		this.userInterface.transform.Find("UnitDetails").gameObject.SetActive(false);
		this.userInterface.transform.Find("Class").gameObject.SetActive(true);
	}

	// Use this for initialization
	void Start () {
		this.gameMechanic = gameObject.GetComponent<GameMechanic>();
		this.miniMap = gameObject.GetComponent<MiniMap>();
		this.userInterface = GameObject.Find("UserInterface");
		this.network = GameObject.Find("NetworkManager").GetComponent<Network>();
		this.classObject = this.userInterface.transform.Find("Class").gameObject;
		this.skillObject = gameObject.GetComponent<Skill>();
	}
	
	// Update is called once per frame
	void Update () {
		Database database = gameObject.GetComponent<Database>();
		if(database != null){
			if(this.gameMechanic.selectedUnit != null){
				if(this.gameMechanic.selectedUnit.cooldown != 0 || database.skill[this.gameMechanic.selectedUnit.status.skill[0]].type == "Passive"){
					this.userInterface.transform.Find("UnitDetails").Find("UseSkill").GetComponent<Button>().interactable = false;
				}else{
					this.userInterface.transform.Find("UnitDetails").Find("UseSkill").GetComponent<Button>().interactable = true;
				}
			}
		}
	}
}
