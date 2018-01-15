using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command : MonoBehaviour {
	public bool move = false;
	public bool attack = false;
	public bool defend = false;
	public bool skill = false;

	//When user select back reset all command
	public void Back(){
		this.move = false;
		this.attack = false;
		this.defend = false;
		this.skill = false;
	}

	public void Move(){
		//TODO:
		//Check range of unit
		//create unit picture no minimap
		this.move = true;
		Debug.Log("Move");
	}

	public void Attack(){
		//TODO:
		//Check range of unit
		//create unit picture on minimap
		this.attack = true;
		Debug.Log("Attack");
	}

	public void Defend(){

	}

	public void Skill(){

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
