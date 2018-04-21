using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Skill : MonoBehaviour {

	private Cube cube;
	private GameMechanic gameMechanic;
	private Player player;
	private MiniMap miniMap;
	private GameObject unitDetails;
	private GameObject miniMapObject;
	private Database database;
	private Command command;
	private Network network;
	//Used to indicate which skill is selected
	public string currentSkill;

	private Unit SearchUnit(Hexagon position){
		foreach(Unit unit in this.gameMechanic.unit){
			if(unit.position.Compare(position)){
				return unit;
			}
		}
		return null;
	}

	public void Call(string skill){
		this.currentSkill = skill;
		if(this.database.skill[skill].type == "Active" && skill != "Stealth" && skill != "Heal"){	//Add more skill here
			this.unitDetails.SetActive(false);
			this.miniMap.HilightMiniMapForSkill(this.database.skill[skill].range);
			this.miniMapObject.SetActive(true);
		}else if(skill == "Stealth"){
			this.network.SendSkillMessage(skill, this.gameMechanic.selectedUnit.name, new Hexagon(0, 0, 0));
		}else if(skill == "Heal"){
			this.network.SendSkillMessage(skill, this.gameMechanic.selectedUnit.name, new Hexagon(0, 0, 0));
		}
	}

	public void Cast(string skill, string unitName, Hexagon target){
		Unit unit = null;
		foreach(Unit character in this.gameMechanic.unit){
			if(character.unitName == unitName){
				unit = character;
			}
		}

		unit.state = "Skill";
		unit.cooldown = this.database.skill[skill].cooldown;
		Debug.Log(unitName + " use skill: " +skill);
		switch(skill){
			case "KnightSpirit":
				KnightSpirit(SearchUnit(target));
				break;
			case "Stealth":
				Stealth(unit);
				break;
			case "DoubleAttack":
				DoubleAttack(unit, SearchUnit(target));
				break;
			case "Freeze":
				Freeze(SearchUnit(target));
				break;
			case "Heal":
				Heal(unit);
				break;
			case "Slam":
				Slam(unit);
				break;
			case "DragonBreath":
				DragonBreath(unit, target);
				break;
			default:
				Debug.Log("Error!!! " + skill + " is not found");
				break;
		}
	}

	public void DragonBreath(Unit unit, Hexagon target){
		Hexagon[] range = this.cube.MovementRange(target, 1);
		Unit targetUnit = null;
		foreach(Hexagon tile in range){ 
			targetUnit = SearchUnit(tile);
			if(targetUnit != null && targetUnit.team != unit.team){
				if(targetUnit.state == "Defend"){
					targetUnit.hp -= this.database.skill["DragonBreath"].damage - 1;
				}else{
					targetUnit.hp -= this.database.skill["DragonBreath"].damage;
				}
			}
		}
	}

	public void DoubleAttack(Unit unit, Unit target){
		unit.Attack(target);
		unit.state = "Idle";
	}

	public void KnightSpirit(Unit target){
		if(target.state == "Defend"){
			target.hp -= this.database.skill["KnightSpirit"].damage -1;
		}else{
			target.hp -= this.database.skill["KnightSpirit"].damage;
		}
	}

	public void Stealth(Unit unit){
		unit.state = "Stealth";
	}

	public void Heal(Unit unit){
		Hexagon[] range = cube.MovementRange(unit.position, 1);
		foreach(Hexagon tile in range){
			foreach(Unit character in this.gameMechanic.unit){
				//Check if there is unit in range and on the same team
				if(character.position.Compare(tile) && unit.team == character.team){
					character.hp += this.database.skill["Heal"].damage;
					if(character.hp > character.status.maxHp){
						character.hp = character.status.maxHp;
					}
				}
			}
		}
	}

	public void Slam(Unit unit){
		Hexagon[] range = cube.MovementRange(unit.position, 1);
		foreach(Hexagon tile in range){
			foreach(Unit character in this.gameMechanic.unit){
				//Check if there is unit in range and aren't on the same team
				if(character.position.Compare(tile) && unit.team != character.team){
					character.state = "Stun";
				}
			}
		}
	}

	public void Freeze(Unit targetUnit){
		targetUnit.state = "Freeze";
		if(targetUnit.state == "Defend"){
			targetUnit.hp -= this.database.skill["KnightSpirit"].damage - 1;
		}else{
			targetUnit.hp -= this.database.skill["Freeze"].damage;
		}
	}

	// Use this for initialization
	void Start () {
		this.cube = new Cube();
		this.gameMechanic = gameObject.GetComponent<GameMechanic>();
		this.player = GameObject.Find("Player").GetComponent<Player>();
		this.miniMap = gameObject.GetComponent<MiniMap>();
		this.database = gameObject.GetComponent<Database>();
		this.command = gameObject.GetComponent<Command>();
		this.network = GameObject.Find("NetworkManager").GetComponent<Network>();
		GameObject userInterface = GameObject.Find("UserInterface");
		this.unitDetails = userInterface.transform.Find("UnitDetails").gameObject;
		this.miniMapObject = userInterface.transform.Find("MiniMap").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
