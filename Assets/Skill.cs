using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour {

	public Cube cube;
	public GameMechanic gameMechanic;
	public Player player;

	public void Heal(Unit unit){
		Hexagon[] range = cube.MovementRange(unit.position, 1);
		foreach(Hexagon tile in range){
			foreach(Unit character in this.gameMechanic.unit){
				//Check if there is unit in range and on the same team
				if(character.position.Compare(tile) && unit.team == character.team){
					character.hp++;
					if(character.hp > character.status.maxHp){
						character.hp = character.status.maxHp;
					}
				}
			}
		}
	}

	public void Stun(Unit unit){
		Hexagon[] range = cube.MovementRange(unit.position, 1);
		foreach(Hexagon tile in range){
			foreach(Unit character in this.gameMechanic.unit){
				//Check if there is unit in range and on the same team
				if(character.position.Compare(tile) && unit.team == character.team){
					
				}
			}
		}
	}

	// Use this for initialization
	void Start () {
		this.cube = new Cube();
		this.gameMechanic = gameObject.GetComponent<GameMechanic>();
		this.player = GameObject.Find("Player").GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
