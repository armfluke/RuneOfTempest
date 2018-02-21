using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class HostGame : MonoBehaviour {

    [SerializeField] //field that inspectator can change 
    private uint roomSize = 4; //maximum player //uint smaller than int

    private string roomName; //input room name

    private NetworkManager networkManager;

    public string sceneName;

    private void Start()
    {
        networkManager = NetworkManager.singleton;
        //edit
        /*var input = gameObject.GetComponent<InputField>();
        var se = new InputField.SubmitEvent();
        se.AddListener(SetRoomName);
        input.onEndEdit = se;*/
        //
        if (networkManager.matchMaker == null) //make sure matchmaker enable
        {
            networkManager.StartMatchMaker(); //if not enable start it
        }
    }

    public void SetRoomName (string _name)//public so can be change if its not a host or edit script
    {
        roomName = _name; //input field will be a room name
        //roomName = "eiei";
        Debug.Log("here is room name" + roomName);
    }

    public void CreateRoom() //call from client
    {
        if (roomName != "" && roomName != null)
        {
            Debug.Log("Creating Room: " + roomName + "with room for" + roomSize + "players");
            Debug.Log("here is room name: " + roomName);
            //create room
            networkManager.StartMatchMaker();
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0, networkManager.OnMatchCreate);  //put all parameter you want player to input and set it true. Forth one is password can set and player play input it as same as roomName
            //last one is The callback to be called when this function completes. This will be called regardless of whether the function succeeds or fails.
            /*ChangeScene change = new ChangeScene();
            change.changeToScene();*/
            SceneManager.LoadScene(this.sceneName);
            //Destroy(GameObject.Find("Canvas"));
            
        }
        else
        {
            Debug.Log("Room name is null");
        }
    }

    /*public class ChangeScene : MonoBehaviour
    {

        public void changeToScene(int changeTheScene)
        {
            
        }

    }*/



}
