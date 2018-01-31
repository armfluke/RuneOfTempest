using System.Collections;
using System.Collections.Generic; //using generic list 16.34
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match; //using match info or description
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;

public class JoinGame : MonoBehaviour {

    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    private NetworkManager networkManager;

    public string sceneName;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker(); //make sure matchmaker actually set up before using it 
        }

        Refresh();//refresh room list
    }

    public void Refresh() //when press button refresh 
    {
        //ClearRoomList();
        networkManager.matchMaker.ListMatches(0, 10, "", true, 0, 0, OnMatchList); //multiple room 0, sizepage list 10 first items, "" for matchname filter no need, call back function return as soon as it found a room
        status.text = "Loading...";
    }
    
    public void OnMatchList (bool success, string extendedinfo, List<MatchInfoSnapshot> matchList)//(ListMatchResponse matchList)
    {
        status.text = "";
        // statusText();
        if (matchList == null) //case that error can't look for match
        {
            status.text = "Couldn't find any match.";
            return;
        }
        ClearRoomList(); //clear all of element
        foreach (MatchInfoSnapshot match in matchList) //add for new one
        {
            GameObject _roomListItemGo = Instantiate(roomListItemPrefab);
            _roomListItemGo.transform.SetParent(roomListParent);

            RoomListItem _roomListItem = _roomListItemGo.GetComponent<RoomListItem>();
            if (_roomListItem != null)
            {
                _roomListItem.Setup(match, JoinRoom);
            }
            
            // as well as setting up a callback function that will join the game.
            roomList.Add(_roomListItemGo);
        }
        
        if(roomList.Count == 0) //case not have any room
        {
            status.text = "No rooms created.";
        }
    }

    void ClearRoomList()
    {
        for(int i = 0; i < roomList.Count; i++) //loop through every element in roomlist
        {
            Destroy(roomList[i]);
        }
        roomList.Clear(); //remove it reference
    }

    public void JoinRoom(MatchInfoSnapshot _match)
    {
        Debug.Log("Joining " + _match.name);
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0,networkManager.OnMatchJoined);
        ClearRoomList();
        //DeleteCurrentCanvas();
        //Destroy(GameObject.Find("Canvas"));
        SceneManager.LoadScene(this.sceneName);
        Debug.Log("join leaw ja");
    }

    public void DeleteCurrentCanvas()
    {
        Destroy(transform.gameObject.GetComponentInParent<Canvas>().gameObject);
    }
}
