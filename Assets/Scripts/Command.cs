using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Command : MonoBehaviour {
	public bool move = false;
	public bool attack = false;
	public bool defend = false;
	public bool skill = false;
	private GameMechanic gameMechanic;
	private GameObject userInterface;
	private MiniMap miniMap;

	public void Select(){
		if(this.gameMechanic.selectedUnit != null){
			this.userInterface.transform.Find("MainGame").gameObject.SetActive(false);
			this.userInterface.transform.Find("UnitDetails").gameObject.SetActive(true);			
		}
	}

	//When user select back reset all command
	public void Back(){
		this.move = false;
		this.attack = false;
		this.defend = false;
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

	}

	public void Skill(){

	}

	// Use this for initialization
	void Start () {
		this.gameMechanic = gameObject.GetComponent<GameMechanic>();
		this.miniMap = gameObject.GetComponent<MiniMap>();
		this.userInterface = GameObject.Find("UserInterface");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
