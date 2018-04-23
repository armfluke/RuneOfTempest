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
    public string tempPrint = "";
    public Text printText;


    public Task<DataSnapshot> ReadData()
    {
        return FirebaseDatabase.DefaultInstance.GetReference("UserData").GetValueAsync();
    }

    // Use this for initialization
    void Start () {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://rune-of-tempest.firebaseio.com/");

        // Get the root reference location of the database.
        this.reference = FirebaseDatabase.DefaultInstance.RootReference;

        //initiate auth
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

    }

    // Update is called once per frame
    void Update () {
		
	}
}
