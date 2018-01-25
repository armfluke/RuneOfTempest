using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class Network : MonoBehaviour {
    
    const short channal = 1000;
    const short move = 1001;
    private bool setting = true;
    public NetworkManager networkManager;
    public GameObject gameMechanic;
    private Network network;

    public void OnMessageReceived(NetworkMessage msg){
        Test test = msg.ReadMessage<Test>();
        Debug.Log("Message received (conn id): " + test.x);
        Debug.Log("Message received (host id): " + test.host);
    }

    public void MoveUnit(string unitName, Hexagon from, Hexagon to){
        MoveMessage msg = new MoveMessage();
        msg.unitName = unitName;
        msg.fromX = from.x;
        msg.fromY = from.y;
        msg.fromZ = from.z;
        msg.toX = to.x;
        msg.toY = to.y;
        msg.toZ = to.z;
        this.networkManager.client.Send(move, msg);
    }

    public void OnServerMoveMessageReceived(NetworkMessage msg){
        MoveMessage message = msg.ReadMessage<MoveMessage>();
        Debug.Log("Unit Name: "+message.unitName);
        Debug.Log("From: "+message.fromX+","+message.fromY+","+message.fromZ);
        Debug.Log("To: "+message.toX+","+message.toY+","+message.toZ);

        NetworkServer.SendToAll(move, message);
    }

    public void OnClientMoveMessageReceived(NetworkMessage msg){
        MoveMessage message = msg.ReadMessage<MoveMessage>();
        
        this.gameMechanic.GetComponent<MiniMap>().Move(message.unitName, 
                new Hexagon(message.fromX, message.fromY, message.fromZ), 
                new Hexagon(message.toX, message.toY, message.toZ));
    }

    void Start(){
        this.gameMechanic = GameObject.Find("GameMechanic");
        this.networkManager = gameObject.GetComponent<NetworkManager>();
        //Setting server handler
        NetworkServer.RegisterHandler(channal, OnMessageReceived);
        NetworkServer.RegisterHandler(move, OnServerMoveMessageReceived);
    }

    Hexagon hexagon;
    Hexagon hexagon2;
    void Update(){
        this.hexagon = GameObject.Find("Drivers").transform.Find("Unit1").GetComponent<Unit>().position;
        this.hexagon2 = this.gameMechanic.GetComponent<GameMechanic>().unit[0].position;
        // Debug.Log(this.hexagon.x+","+this.hexagon.y+","+this.hexagon.z);
        //Register client handler
        if(this.networkManager.client != null && setting){
            setting = false;
            this.networkManager.client.RegisterHandler(channal, OnMessageReceived);
            this.networkManager.client.RegisterHandler(move, OnClientMoveMessageReceived);
        }

        /*if(Input.GetKeyDown(KeyCode.S)){
            Test test = new Test();
            test.x = this.networkManager.client.connection.connectionId;
            test.host = this.networkManager.client.connection.hostId;
            this.networkManager.client.Send(channal, test);

            Test test2 = new Test();
            test2.x = 5555;
            test2.host = 6666;
            NetworkServer.SendToAll(channal, test2);
        }*/
    }
    
    void OnGUI(){
        GUI.Label(new Rect(2, 10, 150, 100), this.hexagon.x+","+this.hexagon.y+","+this.hexagon.z); 
        GUI.Label(new Rect(2, 30, 150, 100), this.hexagon2.x+","+this.hexagon2.y+","+this.hexagon2.z);
        //GUI.Label(new Rect(2, 30, 150, 100), "Press B for both");       
        //GUI.Label(new Rect(2, 50, 150, 100), "Press C for client");
    }
}

public class MoveMessage: MessageBase {
    public string unitName;
    public int fromX;
    public int fromY;
    public int fromZ;
    public int toX;
    public int toY;
    public int toZ;
}

public class Test: MessageBase {
    public int x;
    public int host;
}