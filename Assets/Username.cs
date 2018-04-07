using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Username : MonoBehaviour {

	public Text username;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GameObject userData = GameObject.Find("UserData");
		if(userData != null){
			username.text = userData.GetComponent<UserData>().username;
		}
	}
}
