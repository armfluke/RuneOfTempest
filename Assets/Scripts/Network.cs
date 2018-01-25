using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class Network : MonoBehaviour {
    
    const short CHANNAL = 999;
    const short TEAM = 1000;
    const short MOVE = 1001;
    const short ATTACK = 1002;
    private bool setting = true;
    public NetworkManager networkManager;
    public GameObject gameMechanic;
    private Network network;
    private Player player;
    public int team = 1;

    public void RequestForTeam(){
        //TeamRequestMessage msg = new TeamRequestMessage();
        this.networkManager.client.Send(TEAM, new EmptyMessage());
    }

    public void OnServerTeamRequestMessageReceived(NetworkMessage msg){
        TeamAssignMessage message = new TeamAssignMessage();
        message.connectionId = msg.conn.connectionId;
        message.team = this.team;
        this.team++;
        if(this.team == 5){
            this.team = 1;
        }
        NetworkServer.SendToClient(msg.conn.connectionId, TEAM, message);
    }

    public void OnClientTeamAssigntMessageReceived(NetworkMessage msg){
        TeamAssignMessage message = msg.ReadMessage<TeamAssignMessage>();
        this.player.connectionId = message.connectionId;
        this.player.team = message.team;
    }

    public void OnMessageReceived(NetworkMessage msg){
        Test test = msg.ReadMessage<Test>();
        Debug.Log("Message received (conn id): " + test.x);
        Debug.Log("Message received (host id): " + test.host);
    }

    public void MoveUnit(Hexagon from, Hexagon to){
        MoveMessage msg = new MoveMessage();
        msg.from = new Vector3(from.x, from.y, from.z);
        msg.to = new Vector3(to.x, to.y, to.z);
        this.networkManager.client.Send(MOVE, msg);
    }

    public void AttackUnit(Hexagon from, Hexagon to){
        MoveMessage msg = new MoveMessage();
        msg.from = new Vector3(from.x, from.y, from.z);
        msg.to = new Vector3(to.x, to.y, to.z);
        this.networkManager.client.Send(ATTACK, msg);
    }

    public void OnServerMoveMessageReceived(NetworkMessage msg){
        MoveMessage message = msg.ReadMessage<MoveMessage>();
        Debug.Log("Move unit | From: "+message.from.x+","+message.from.y+","+message.from.z+"|"+"To: "+message.to.x+","+message.to.y+","+message.to.z);

        NetworkServer.SendToAll(MOVE, message);
    }

    public void OnServerAttackMessageReceived(NetworkMessage msg){
        MoveMessage message = msg.ReadMessage<MoveMessage>();
        Debug.Log("Attack unit | From: "+message.from.x+","+message.from.y+","+message.from.z+"|"+"To: "+message.to.x+","+message.to.y+","+message.to.z);

        NetworkServer.SendToAll(ATTACK, message);
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
        this.player = GameObject.Find("Player").GetComponent<Player>();
        //Setting server handler
        NetworkServer.RegisterHandler(CHANNAL, OnMessageReceived);
        NetworkServer.RegisterHandler(TEAM, OnServerTeamRequestMessageReceived);
        NetworkServer.RegisterHandler(MOVE, OnServerMoveMessageReceived);
        NetworkServer.RegisterHandler(ATTACK, OnServerAttackMessageReceived);

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
            this.networkManager.client.RegisterHandler(CHANNAL, OnMessageReceived);
            this.networkManager.client.RegisterHandler(TEAM, OnClientTeamAssigntMessageReceived);
            this.networkManager.client.RegisterHandler(MOVE, OnClientMoveMessageReceived);
            this.networkManager.client.RegisterHandler(ATTACK, OnClientAttackMessageReceived);
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
        //GUI.Label(new Rect(2, 10, 150, 100), this.hexagon.x+","+this.hexagon.y+","+this.hexagon.z); 
        //GUI.Label(new Rect(2, 30, 150, 100), this.hexagon2.x+","+this.hexagon2.y+","+this.hexagon2.z);
        //GUI.Label(new Rect(2, 30, 150, 100), "Press B for both");       
        //GUI.Label(new Rect(2, 50, 150, 100), "Press C for client");
    }
}

public class TeamRequestMessage: MessageBase {

}

public class TeamAssignMessage: MessageBase {
    public int team;
    public int connectionId;
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