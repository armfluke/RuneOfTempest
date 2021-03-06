﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;*/
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

public class Database : MonoBehaviour {
	//public DatabaseReference reference;
	public Dictionary<string, Status> status;
	public Dictionary<string, SkillStatus> skill;

	public void ReadUnitStatus(){
		TextAsset file = Resources.Load<TextAsset>("Files/Units");
		this.status = JsonConvert.DeserializeObject<Dictionary<string, Status>>(file.ToString());
	}

	public void ReadSkillStatus(){
		TextAsset file = Resources.Load<TextAsset>("Files/Skills");
		this.skill = JsonConvert.DeserializeObject<Dictionary<string, SkillStatus>>(file.ToString());
	}

	public Status GetUnitStatus(string unit){
		return this.status[unit];
	}

	/*public Task<DataSnapshot> ReadAll(string query){
		return FirebaseDatabase.DefaultInstance.GetReference(query).GetValueAsync();
	}

	public Task<DataSnapshot> Read(string query, string child){
		return FirebaseDatabase.DefaultInstance.GetReference(query).Child(child).GetValueAsync();
	}*/

	// Use this for initialization
	void Start () {
		//Read unit status from file
		//ReadUnitStatus();

		// Set up the Editor before calling into the realtime database.
		//FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://rune-of-tempest.firebaseio.com/");

		// Get the root reference location of the database.
		//this.reference = FirebaseDatabase.DefaultInstance.RootReference;

		//Write data
		/*string json = JsonUtility.ToJson(new Hexagon(0, 1, 2));
		Debug.Log(json);
   		this.reference.Child("test").SetRawJsonValueAsync(json);*/

		//Read data
		/*FirebaseDatabase.DefaultInstance.GetReference(query).GetValueAsync().ContinueWith(task => {
			if (task.IsFaulted) {
				// Handle the error...
			}else if (task.IsCompleted) {
				// Do something with snapshot...
				DataSnapshot snapshot = task.Result;
				Debug.Log(snapshot.ChildrenCount);
				//Debug.Log(snapshot.Child("x").GetValue(true).ToString());
				Debug.Log(((Dictionary<string, Object>)snapshot.Value)["y"]);
			}
		});*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
