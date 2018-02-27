using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class : MonoBehaviour {

	Generator generator;
	Player player;

	T CopyComponent<T>(T original, GameObject destination) where T : Component {
		System.Type type = original.GetType();
		Component copy = destination.AddComponent(type);
		System.Reflection.FieldInfo[] fields = type.GetFields();
		foreach (System.Reflection.FieldInfo field in fields){
			field.SetValue(copy, field.GetValue(original));
		}
		return copy as T;
	}

	public void ClassChange(Unit unit, string nextClass){
		Unit newUnit = this.generator.GenerateUnit(nextClass, unit.unitName, unit.team, unit.position);
		this.player.playerUnits.Add(newUnit);
		newUnit.state = unit.state;
		unit.hp = 0;
	}

	// Use this for initialization
	void Start () {
		this.generator = gameObject.GetComponent<Generator>();
		this.player = GameObject.Find("Player").GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
