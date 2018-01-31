using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {

	Unit unit;
	Player player;
	Button button;
	GameObject drivers;
	GameMechanic gameMechanic;

	// Use this for initialization
	void Start () {
		this.gameMechanic = GameObject.Find("GameMechanic").GetComponent<GameMechanic>();
		this.player = GameObject.Find("Player").GetComponent<Player>();
		this.button = gameObject.GetComponent<Button>();
	}
	
	// Update is called once per frame
	void Update () {
		if(this.player.team != 0){
			foreach(Unit unit in this.gameMechanic.unit){
				if(unit.unitName == gameObject.name + " Team" + this.player.team){
					this.unit = unit;
					break;
				}else{
					this.unit = null;
				}
			}

			/*if(this.drivers != null){
				Transform unitObject = this.drivers.transform.Find(gameObject.name + " Team" + this.player.team);
				if(unitObject != null){
					this.unit = unitObject.GetComponent<Unit>();
				}else{
					this.unit = null;
				}
			}*/
		}

		if(this.unit == null){
			this.button.interactable = false;
		}else{
			if(this.unit.state == "Idle" || this.unit.state == "Move"){
				this.button.interactable = true;
			}else{
				this.button.interactable = false;
			}
		}
	}
}
