using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	DatabaseReference reference;
	public int team = 0;
	public int cost = 5;
	public List<Unit> playerUnits;
	public string status = "Alive";
	public Hexagon castlePosition;
	private bool checkLose = false;
	public GameMechanic gameMechanic;
	public Network network;
	public GameObject mainGame;
	public Text teamText;
	public Text costText;
	public UserData userData;
	public WinCondition winCondition;

	// Use this for initialization
	void Start () {
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://rune-of-tempest.firebaseio.com/");
		this.reference = FirebaseDatabase.DefaultInstance.RootReference;

		this.gameMechanic = GameObject.Find("GameMechanic").GetComponent<GameMechanic>();
		this.network = GameObject.Find("NetworkManager").GetComponent<Network>();
		this.mainGame = GameObject.Find("UserInterface").transform.Find("MainGame").gameObject;
		this.userData = GameObject.Find("UserData").GetComponent<UserData>();
		this.winCondition = GameObject.Find("GameMechanic").GetComponent<WinCondition>();
	}
	
	// Update is called once per frame
	void Update () {
		if(this.status == "Lose" && this.checkLose == false){
			this.checkLose = true;
			for(int i=0; i < this.playerUnits.Count; i++){
				this.playerUnits[i].hp = 0;
			}

			this.network.SendLoseStatusMessage(this.team);
			this.mainGame.SetActive(false);

			//Update score
			FirebaseDatabase.DefaultInstance.GetReference("UserData").GetValueAsync().ContinueWith(task => {
				if (task.IsFaulted){
					// Handle the error...
					Debug.Log("Error to read data from firebase database");
				}else if(task.IsCompleted){
					int count = 0;
					foreach(bool loseStatus in this.winCondition.playerLoseStatus){
						if(loseStatus == true){
							count++;
						}
					}
					int additionalScore = 0;
					switch(count){
						case 1:
							additionalScore = -2;
							break;
						case 2:
							additionalScore = -1;
							break;
						case 3:
							additionalScore = 1;
							break;
						default:
							additionalScore = 0;
							break;
					}
					DataSnapshot snapshot = task.Result;
					IDictionary data = (IDictionary)snapshot.Value;
					data = ((IDictionary)data[this.userData.username]);
					UserInformation newData = new UserInformation(data["username"].ToString(), data["password"].ToString(), data["email"].ToString(), Int32.Parse(data["score"].ToString()) + additionalScore);
					string json = JsonUtility.ToJson(newData);
					Debug.Log(json);
					this.reference.Child("UserData").Child(this.userData.username).SetRawJsonValueAsync(json);
				}
			});
		}

		//Add status to game status
		this.teamText.text = this.team.ToString();
		this.costText.text = this.cost.ToString();
	}

	/*void OnGUI(){
        GUI.Label(new Rect(2, 10, 150, 100), "Team:"+this.team);
		GUI.Label(new Rect(102, 10, 150, 100), "Cost:"+this.cost);
	}*/
}
