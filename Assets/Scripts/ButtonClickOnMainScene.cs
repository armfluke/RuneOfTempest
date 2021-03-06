﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClickOnMainScene : MonoBehaviour {

    public void OnClickStartGame(){
        SceneManager.LoadScene("Lobby");
    }

    public void OnClickRules(){
        SceneManager.LoadScene("Rules");
    }

    public void OnClickStory(){
        SceneManager.LoadScene("Story");
    }

    public void OnClickLeaderboard(){
        SceneManager.LoadScene("Leaderboard");
    }

    public void OnClickLogOut(){
        if(PlayerPrefs.HasKey("UserData")){
            PlayerPrefs.DeleteKey("UserData");
        }
        if(GameObject.Find("UserData") != null){
            Destroy(GameObject.Find("UserData"));
        }
        //change to login scene
        SceneManager.LoadScene("Login");
    }

    public void OnClickBackToMainPage(){
        SceneManager.LoadScene("Main");
    }
}
