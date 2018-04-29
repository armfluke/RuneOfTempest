using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Text;




public class Leaderboard : MonoBehaviour {

    public DatabaseReference reference;
    public static int Row = 50;
    public static int Column = 2;
    public string[,] arrayRanking = new string[Row, Column];
    private string tempPrint = "";
    public Text printText;


    public Task<DataSnapshot> ReadData()
    {
        return FirebaseDatabase.DefaultInstance.GetReference("UserData").GetValueAsync();
    }

    public void GetUserLeaderboard()
    {
        //read data
        ReadData().ContinueWith(task => {
            if (task.IsFaulted) {
                // Handle the error...
                Debug.Log("Error to read data from firebase database");

            }else if (task.IsCompleted){
                DataSnapshot snapshot = task.Result;
                //read all key
                IDictionary test = (IDictionary)snapshot.Value;
                Dictionary<string, int> sortArray = new Dictionary<string, int>();

                foreach (string key in test.Keys){
                    sortArray[key] = Int32.Parse(snapshot.Child(key).Child("score").GetValue(true).ToString());
                }

                sortArray = sortArray.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                foreach(string key in sortArray.Keys){
                    tempPrint += key + " " + sortArray[key] + "\n";
                }

                printText.text = tempPrint;
            }
            GameObject.Find("LoginRegisterCanvas").transform.Find("ErrorLoginPanel").gameObject.SetActive(true);
        });

    }

    // Use this for initialization
    void Start () {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://rune-of-tempest.firebaseio.com/");

        // Get the root reference location of the database.
        this.reference = FirebaseDatabase.DefaultInstance.RootReference;
        GetUserLeaderboard();

    }

    // Update is called once per frame
    void Update () {
		
	}
}
