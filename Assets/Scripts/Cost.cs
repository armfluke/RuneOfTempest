using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Cost : MonoBehaviour {

	public Database database;

	// Use this for initialization
	void Start () {
		this.database = GameObject.Find("GameMechanic").GetComponent<Database>();
		GetComponent<Text>().text = this.database.status[transform.parent.name].cost.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
