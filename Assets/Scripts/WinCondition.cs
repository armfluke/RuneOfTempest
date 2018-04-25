using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Networking.Match;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using Newtonsoft.Json;
using System;

public class WinCondition : MonoBehaviour {

	public DatabaseReference reference;
	public Player player;
	private TurnManager turnManager;
	private int checkTurnChange;
	private GameMechanic gameMechanic;
	/*private Hexagon[] castlePosition = new Hexagon[]{
		new Hexagon(-7, 0, 7), new Hexagon(0, -7, 7), new Hexagon(7, 0, -7), new Hexagon(0, 7, -7)
	};*/
	private Hexagon[] castlePosition = new Hexagon[]{
		new Hexagon(-7, 0, 7), new Hexagon(7, 0, -7), new Hexagon(0, -7, 7), new Hexagon(0, 7, -7)
	};
	private NetworkManager networkManager;
	public int[] castleConquerCount = new int[]{0, 0, 0, 0};
	public bool[] checkCastleConquer = new bool[]{false, false, false, false};
	public Unit[] castleConquerUnit = new Unit[4];
	public bool[] playerLoseStatus = new bool[]{false, false, false, false};
	public Text[] conquerCount;
	public UserData userData;

	public void OnLobbyClicked(){
		MatchInfo matchInfo = this.networkManager.matchInfo;
		//this.networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, this.networkManager.OnDropConnection);
		this.networkManager.StopHost();

		Destroy(GameObject.Find("NetworkManager"));
		SceneManager.LoadScene("Lobby");
	}

	public void Win(int team){
		GameObject win = GameObject.Find("UserInterface").transform.Find("Win").gameObject;
        win.transform.Find("WinText").GetComponent<Text>().text = "Player Team " + team + " Win!!!";
		if(this.player.team == team){
			//Update score to database
			FirebaseDatabase.DefaultInstance.GetReference("UserData").GetValueAsync().ContinueWith(task => {
				if (task.IsFaulted){
					// Handle the error...
					Debug.Log("Error to read data from firebase database");
				}else if(task.IsCompleted){
					DataSnapshot snapshot = task.Result;
					IDictionary data = (IDictionary)snapshot.Value;
					data = ((IDictionary)data[this.userData.username]);
					UserInformation newData = new UserInformation(data["username"].ToString(), data["password"].ToString(), data["email"].ToString(), Int32.Parse(data["score"].ToString()) + 2);
					string json = JsonUtility.ToJson(newData);
					Debug.Log(json);
					this.reference.Child("UserData").Child(this.userData.username).SetRawJsonValueAsync(json);
				}
			});
		}

		win.SetActive(true);
	}

	// Use this for initialization
	void Start () {
		/*this.castlePosition = new Hexagon[]{
			new Hexagon(-7, 0, 7), new Hexagon(0, -7, 7), new Hexagon(7, 0, -7), new Hexagon(0, 7, -7)
		};*/
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://rune-of-tempest.firebaseio.com/");
		this.reference = FirebaseDatabase.DefaultInstance.RootReference;
		
		this.networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
		this.player = GameObject.Find("Player").GetComponent<Player>();
		this.gameMechanic = gameObject.GetComponent<GameMechanic>();
		this.turnManager = gameObject.GetComponent<TurnManager>();
		this.checkTurnChange = this.turnManager.turn;
		this.userData = GameObject.Find("UserData").GetComponent<UserData>();

	}
	
	// Update is called once per frame
	void Update () {

		//Check win condition if all unit die
		if(this.player.team != 0 && this.player.playerUnits.Count == 0){
			this.player.status = "Lose";
		}

		//Check win condition if conquer enemy castle for three turn
		//if turn is passed
		if(this.checkTurnChange != this.turnManager.turn){
			this.checkTurnChange = this.turnManager.turn;
			//set check if castle is conquered to false
			for(int i=0; i < this.checkCastleConquer.Length; i++){
				checkCastleConquer[i] = false;
			}
			//loop for each unit in game
			foreach(Unit unit in this.gameMechanic.unit){
				int index = 0;
				//loop to check all castle position
				foreach(Hexagon position in this.castlePosition){
					//check if there is unit on castle position
					if(unit.position.x == position.x && unit.position.y == position.y && unit.position.z == position.z){
						//check if unit on castle position is not the same team
						if((unit.team == 1 && index == 0) || (unit.team == 2 && index == 1) || (unit.team == 3 && index == 2) || (unit.team == 4 && index == 3)){
							break;
						}
						//castle is conquered
						this.checkCastleConquer[index] = true;

						//increment conquer count
						this.castleConquerCount[index]++;
						//if conquer count == 2 set status of player of that castle to Lose
						if(castleConquerCount[index] == 2 && this.player.castlePosition.x == position.x
							&& this.player.castlePosition.y == position.y && this.player.castlePosition.z == position.z){
								this.player.status = "Lose";
						}
						break;
					}
					index++;
				}
			}

			for(int i=0; i < this.checkCastleConquer.Length; i++){
				if(checkCastleConquer[i] == false){
					castleConquerCount[i] = 0;
				}
			}
		}

		//check winning(Only one team left)
		int countLose = 0;
		int teamWin = 0;
		foreach(bool loseStatus in this.playerLoseStatus){
			if(loseStatus){
				countLose++;
				if(countLose == GameMechanic.MAX_PLAYER - 1){
					//Search for winning team
					for(int i=0; i<this.playerLoseStatus.Length;i++){
						if(this.playerLoseStatus[i] == false){
							teamWin = i + 1;
							break;
						}
					}
					Win(teamWin);
				}
			}
		}

		for(int i = 0; i < conquerCount.Length; i++){
			conquerCount[i].text = this.castleConquerCount[i].ToString();
		}
	}

	/*void OnGUI(){
		GUI.Label(new Rect(2, 30, 150, 100), "Conquer count team1: " + this.castleConquerCount[0]);
        GUI.Label(new Rect(2, 50, 150, 100), "Conquer count team2: " + this.castleConquerCount[1]);       
        GUI.Label(new Rect(2, 70, 150, 100), "Conquer count team3: " + this.castleConquerCount[2]);
		GUI.Label(new Rect(2, 90, 150, 100), "Conquer count team4: " + this.castleConquerCount[3]);
	}*/
}
