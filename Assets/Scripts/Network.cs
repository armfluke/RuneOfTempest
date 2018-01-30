using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class Network : MonoBehaviour {
    
    const short TIMER = 999;
    const short TEAM = 1000;
    const short MOVE = 1001;
    const short ATTACK = 1002;
    const short END_TURN = 1003;
    private bool setting = true;
    public NetworkManager networkManager;
    public GameObject gameMechanic;
    private Network network;
    private Player player;
    private TurnManager turnManager;
    public int team = 1;

    public void OnClientConnected(NetworkMessage msg){
        RequestForTeam();
    }

    public void RequestForTeam(){
        this.networkManager.client.Send(TEAM, new EmptyMessage());
    }

    public void OnServerTeamRequestMessageReceived(NetworkMessage msg){
        TeamAssignMessage message = new TeamAssignMessage();
        message.team = this.team;
        this.team++;
        if(this.team == 5){
            this.team = 1;
        }
        NetworkServer.SendToClient(msg.conn.connectionId, TEAM, message);
    }

    public void OnClientTeamAssigntMessageReceived(NetworkMessage msg){
        TeamAssignMessage message = msg.ReadMessage<TeamAssignMessage>();
        this.player.team = message.team;
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

    public void OnClientTimerMessageReceived(NetworkMessage msg){
        TurnMessage message = msg.ReadMessage<TurnMessage>();
        this.turnManager.time = message.time;
        this.turnManager.currentTeamTurn = message.turn;
    }

    public void OnClientAttackMessageReceived(NetworkMessage msg){
        MoveMessage message = msg.ReadMessage<MoveMessage>();
        
        this.gameMechanic.GetComponent<MiniMap>().Attack(
            new Hexagon((int)message.from.x, (int)message.from.y, (int)message.from.z), 
            new Hexagon((int)message.to.x, (int)message.to.y, (int)message.to.z));
    }

    public void SendTurnMessage(float time, int turn){
        TurnMessage msg = new TurnMessage();
		msg.time = time;
		msg.turn = turn;
		NetworkServer.SendToAll(TIMER ,msg);
    }

    public void SendEndTurnMessage(){
        this.networkManager.client.Send(END_TURN, new EmptyMessage());
    }

    public void OnServerEndTurnMessageReceived(NetworkMessage msg){
        TurnMessage message = new TurnMessage();
        this.turnManager.time = TurnManager.TIME_PER_TURN;
        this.turnManager.currentTeamTurn++;
        if(this.turnManager.currentTeamTurn == 5){
            this.turnManager.currentTeamTurn = 1;
        }

        message.time = this.turnManager.time;
        message.turn = this.turnManager.currentTeamTurn;
        NetworkServer.SendToAll(END_TURN, message);
    }

    public void OnClientEndTurnMessageReceived(NetworkMessage msg){
        TurnMessage message = msg.ReadMessage<TurnMessage>();
        this.turnManager.time = message.time;
        this.turnManager.currentTeamTurn = message.turn;
        if(this.turnManager.currentTeamTurn == 1){
            this.turnManager.turn++;
        }
    }

    void Start(){
        this.gameMechanic = GameObject.Find("GameMechanic");
        this.networkManager = gameObject.GetComponent<NetworkManager>();
        this.player = GameObject.Find("Player").GetComponent<Player>();
        this.turnManager = GameObject.Find("GameMechanic").GetComponent<TurnManager>();
        //Setting server handler
        NetworkServer.RegisterHandler(TEAM, OnServerTeamRequestMessageReceived);
        NetworkServer.RegisterHandler(MOVE, OnServerMoveMessageReceived);
        NetworkServer.RegisterHandler(ATTACK, OnServerAttackMessageReceived);
        NetworkServer.RegisterHandler(END_TURN, OnServerEndTurnMessageReceived);
    }

    void Update(){
        //Register client handler
        if(this.networkManager.client != null && setting){
            setting = false;
            this.networkManager.client.RegisterHandler(MsgType.Connect, OnClientConnected);
            this.networkManager.client.RegisterHandler(TEAM, OnClientTeamAssigntMessageReceived);
            this.networkManager.client.RegisterHandler(MOVE, OnClientMoveMessageReceived);
            this.networkManager.client.RegisterHandler(ATTACK, OnClientAttackMessageReceived);
            this.networkManager.client.RegisterHandler(TIMER, OnClientTimerMessageReceived);
            this.networkManager.client.RegisterHandler(END_TURN, OnClientEndTurnMessageReceived);
        }

    }
    
    // void OnGUI(){
    //     GUI.Label(new Rect(2, 10, 150, 100), this.hexagon.x+","+this.hexagon.y+","+this.hexagon.z); 
    //     GUI.Label(new Rect(2, 30, 150, 100), this.hexagon2.x+","+this.hexagon2.y+","+this.hexagon2.z);
    //     GUI.Label(new Rect(2, 30, 150, 100), "Press B for both");       
    //     GUI.Label(new Rect(2, 50, 150, 100), "Press C for client");
    // }
}

public class TeamRequestMessage: MessageBase {

}

public class TeamAssignMessage: MessageBase {
    //team of player
    public int team;
}

public class MoveMessage: MessageBase {
    //position of unit that made a move
    public Vector3 from;
    //position of target tile that unit move to
    public Vector3 to;
}

public class AttackMessage: MessageBase {
    //position of unit that made an attack
    public Vector3 from;
    //position of unit that get attacked
    public Vector3 to;
}

public class TurnMessage: MessageBase {
    //current time in turn
    public float time;
    //current team turn
    public int turn;
}