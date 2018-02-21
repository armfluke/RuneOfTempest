using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClickOnMainScene : MonoBehaviour {

    public void OnClickStartGame()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void OnClickRules()
    {
        SceneManager.LoadScene("Rules");
    }

    public void OnClickStory()
    {
        SceneManager.LoadScene("Story");
    }

    public void OnClickLogOut()
    {
        //change to login scene
        //SceneManager.LoadScene("Game");
    }

    public void OnClickBackToMainPage()
    {
        SceneManager.LoadScene("Main");
    }
}
