﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Status : MonoBehaviour {
	[JsonProperty("name")]
	public string unitName;
	[JsonProperty("hp")]
	public int maxHp;
	[JsonProperty("attack")]
	public int attack;
	[JsonProperty("range")]
	public int range;
	[JsonProperty("move")]
	public int move;
	[JsonProperty("cost")]
	public int cost;
	[JsonProperty("skill")]
	public string[] skill;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
