using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class Network : MonoBehaviour {
    
    const short channal = 1000;
    const short move = 1001;
    const short attack = 1002;
    private bool setting = true;
    public NetworkManager networkManager;
    public GameObject gameMechanic;
    private Network network;

    public void OnMessageReceived(NetworkMessage msg){
        Test test = msg.ReadMessage<Test>();
        Debug.Log("Message received (conn id): " + test.x);
        Debug.Log("Message received (host id): " + test.host);
    }

    public void MoveUnit(Hexagon from, Hexagon to){
        MoveMessage msg = new MoveMessage();
        msg.from = new Vector3(from.x, from.y, from.z);
        msg.to = new Vector3(to.x, to.y, to.z);
        this.networkManager.client.Send(move, msg);
    }

    public void AttackUnit(Hexagon from, Hexagon to){
        MoveMessage msg = new MoveMessage();
        msg.from = new Vector3(from.x, from.y, from.z);
        msg.to = new Vector3(to.x, to.y, to.z);
        this.networkManager.client.Send(attack, msg);
    }

    public void OnServerMoveMessageReceived(NetworkMessage msg){
        MoveMessage message = msg.ReadMessage<MoveMessage>();
        Debug.Log("Move unit | From: "+message.from.x+","+message.from.y+","+message.from.z+"|"+"To: "+message.to.x+","+message.to.y+","+message.to.z);

        NetworkServer.SendToAll(move, message);
    }

    public void OnServerAttackMessageReceived(NetworkMessage msg){
        MoveMessage message = msg.ReadMessage<MoveMessage>();
        Debug.Log("Attack unit | From: "+message.from.x+","+message.from.y+","+message.from.z+"|"+"To: "+message.to.x+","+message.to.y+","+message.to.z);

        NetworkServer.SendToAll(attack, message);
    }

    public void OnClientMoveMessageReceived(NetworkMessage msg){
        MoveMessage message = msg.ReadMessage<MoveMessage>();
        
        this.gameMechanic.GetComponent<MiniMap>().Move( 
            new Hexagon((int)message.from.x, (int)message.from.y, (int)message.from.z), 
            new Hexagon((int)message.to.x, (int)message.to.y, (int)message.to.z));
    }

    public void OnClientAttackMessageReceived(NetworkMessage msg){
        MoveMessage message = msg.ReadMessage<MoveMessage>();
        
        this.gameMechanic.GetComponent<MiniMap>().Attack(
            new Hexagon((int)message.from.x, (int)message.from.y, (int)message.from.z), 
            new Hexagon((int)message.to.x, (int)message.to.y, (int)message.to.z));
    }

    void Start(){
        this.gameMechanic = GameObject.Find("GameMechanic");
        this.networkManager = gameObject.GetComponent<NetworkManager>();
        //Setting server handler
        NetworkServer.RegisterHandler(channal, OnMessageReceived);
        NetworkServer.RegisterHandler(move, OnServerMoveMessageReceived);
        NetworkServer.RegisterHandler(attack, OnServerAttackMessageReceived);
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
            this.networkManager.client.RegisterHandler(attack, OnClientAttackMessageReceived);
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
    public Vector3 from;
    public Vector3 to;
}

public class AttackMessage: MessageBase {
    public Vector3 from;
    public Vector3 to;
}

public class Test: MessageBase {
    public int x;
    public int host;
}