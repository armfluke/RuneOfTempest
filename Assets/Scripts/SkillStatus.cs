using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class SkillStatus : MonoBehaviour {
	[JsonProperty("name")]
	public string skillName;
    [JsonProperty("type")]
    public string type;
    [JsonProperty("range")]
    public int range;
    [JsonProperty("damage")]
    public int damage;
    [JsonProperty("cooldown")]
    public int cooldown;
    [JsonProperty("description")]
    public string description;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
