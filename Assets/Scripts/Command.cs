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

	public void Select(){
		if(this.gameMechanic.selectedUnit != null && (this.gameMechanic.selectedUnit.state == "Idle" || this.gameMechanic.selectedUnit.state == "Move")){
			this.userInterface.transform.Find("MainGame").gameObject.SetActive(false);
			this.userInterface.transform.Find("UnitDetails").gameObject.SetActive(true);			
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

	}

	public void Class(){
		if(this.gameMechanic.selectedUnit.status.availableClass.Length != 0){
			Transform foundedClass;
			//Loop to enable button only available class
			foreach(string availableClass in this.gameMechanic.selectedUnit.status.availableClass){
				foundedClass = null;
				foundedClass = this.classObject.transform.Find(availableClass);
				if(foundedClass != null){
					foundedClass.gameObject.GetComponent<Button>().interactable = true;
				}else{
					foundedClass.gameObject.GetComponent<Button>().interactable = false;
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
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
