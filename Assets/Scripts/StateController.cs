using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateController : MonoBehaviour {

	public GameMechanic gameMechanic;
	public Player player;
	public Dictionary<string ,GameObject> state = new Dictionary<string, GameObject>();

	// Use this for initialization
	void Start () {
		this.gameMechanic = GameObject.Find("GameMechanic").GetComponent<GameMechanic>();
		this.player = GameObject.Find("Player").GetComponent<Player>();

		//Get each game object state
		this.state["Idle"] = transform.Find("Idle").gameObject;
		this.state["Move"] = transform.Find("Move").gameObject;
		this.state["Attack"] = transform.Find("Attack").gameObject;
		this.state["Defend"] = transform.Find("Defend").gameObject;
		this.state["Skill"] = transform.Find("Skill").gameObject;
		this.state["Rest"] = transform.Find("Rest").gameObject;
		this.state["Die"] = transform.Find("Die").gameObject;
		this.state["Stun"] = transform.Find("Stun").gameObject;
		this.state["Freeze"] = transform.Find("Freeze").gameObject;
		this.state["Stealth"] = transform.Find("Stealth").gameObject;
	}
	
	// Update is called once per frame
	void Update () {

		bool unitFound = false;
		if(this.player.team != 0){
			foreach(Unit unit in this.gameMechanic.unit){
				if(unit.unitName == gameObject.name.Substring(5) + " Team" + this.player.team){
					unitFound = true;
					foreach(KeyValuePair<string, GameObject> entry in this.state){
						if(entry.Value.name == unit.state){
							entry.Value.SetActive(true);
						}else{
							entry.Value.SetActive(false);
						}
					}
					break;
				}
			}

			if(unitFound == false){
				foreach(KeyValuePair<string, GameObject> entry in this.state){
					if(entry.Value.name == "Die"){
						entry.Value.SetActive(true);
					}else{
						entry.Value.SetActive(false);
					}
				}
			}
		}
	}
}
