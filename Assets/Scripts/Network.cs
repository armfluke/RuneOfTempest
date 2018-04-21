using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Network : MonoBehaviour {
    
    const short TIMER = 999;
    const short TEAM = 1000;
    const short MOVE = 1001;
    const short ATTACK = 1002;
    const short END_TURN = 1003;
    const short DEFEND = 1004;
    const short LOSE = 1005;
    const short CLASS_CHANGE = 1006;
    const short SKILL = 1007;
    public bool setting = true;
    public NetworkManager networkManager;
    public int team = 1;

    /*public void OnClientConnected(NetworkMessage msg){
        RequestForTeam();
    }*/

    public void RequestForTeam(){
        this.networkManager.client.Send(TEAM, new EmptyMessage());
    }

    public void OnServerTeamRequestMessageReceived(NetworkMessage msg){
        TeamAssignMessage message = new TeamAssignMessage();
        message.team = this.team;
        this.team++;
        if(this.team == GameMechanic.MAX_PLAYER + 1){
            this.team = 1;
        }
        NetworkServer.SendToClient(msg.conn.connectionId, TEAM, message);
    }

    public void OnClientTeamAssigntMessageReceived(NetworkMessage msg){
        GameObject gameMechanic = GameObject.Find("GameMechanic");
        Player player = GameObject.Find("Player").GetComponent<Player>();
        TurnManager turnManager = GameObject.Find("GameMechanic").GetComponent<TurnManager>();
        GameObject mainGame = GameObject.Find("UserInterface").transform.Find("MainGame").gameObject;

        TeamAssignMessage message = msg.ReadMessage<TeamAssignMessage>();
        player.team = message.team;
        //Assign castle positon for each player team
        if(player.team == 1){
            player.castlePosition = new Hexagon(-7, 0, 7);
        }else if(player.team == 2){
            player.castlePosition = new Hexagon(0, -7, 7);
        }else if(player.team == 3){
            player.castlePosition = new Hexagon(7, 0, -7);
        }else if(player.team == 4){
            player.castlePosition = new Hexagon(0, 7, -7);
        }

        List<Unit> units = gameMechanic.GetComponent<GameMechanic>().unit;

        foreach(Unit unit in units){
            if(unit.team == player.team){
                player.playerUnits.Add(unit);
            }
        }

        if(turnManager.currentTeamTurn != player.team){
			mainGame.SetActive(false);
		}else{
			mainGame.SetActive(true);
		}
    }

    public void MoveUnit(Hexagon from, Hexagon to){
        MoveMessage msg = new MoveMessage();
        msg.from = from.ToVector3();
        msg.to = to.ToVector3();
        this.networkManager.client.Send(MOVE, msg);
    }

    public void AttackUnit(Hexagon from, Hexagon to){
        MoveMessage msg = new MoveMessage();
        msg.from = from.ToVector3();
        msg.to = to.ToVector3();
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
        GameObject gameMechanic = GameObject.Find("GameMechanic");
        
        MoveMessage message = msg.ReadMessage<MoveMessage>();
        
        gameMechanic.GetComponent<MiniMap>().Move(Hexagon.ToHexagon(message.from), Hexagon.ToHexagon(message.to));
    }

    public void OnClientTimerMessageReceived(NetworkMessage msg){
        TurnManager turnManager = GameObject.Find("GameMechanic").GetComponent<TurnManager>();

        TurnMessage message = msg.ReadMessage<TurnMessage>();
        turnManager.time = message.time;
        turnManager.currentTeamTurn = message.turn;
    }

    public void OnClientAttackMessageReceived(NetworkMessage msg){
        GameObject gameMechanic = GameObject.Find("GameMechanic");

        MoveMessage message = msg.ReadMessage<MoveMessage>();
        
        gameMechanic.GetComponent<MiniMap>().Attack(Hexagon.ToHexagon(message.from), Hexagon.ToHexagon(message.to));
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
        TurnManager turnManager = GameObject.Find("GameMechanic").GetComponent<TurnManager>();

        TurnMessage message = new TurnMessage();
        turnManager.time = TurnManager.TIME_PER_TURN;
        turnManager.currentTeamTurn++;
        if(turnManager.currentTeamTurn == GameMechanic.MAX_PLAYER + 1){
            turnManager.currentTeamTurn = 1;
            
        }

        message.time = turnManager.time;
        message.turn = turnManager.currentTeamTurn;
        NetworkServer.SendToAll(END_TURN, message);
    }

    public void OnClientEndTurnMessageReceived(NetworkMessage msg){
        GameObject gameMechanic = GameObject.Find("GameMechanic");
        Player player = GameObject.Find("Player").GetComponent<Player>();
        TurnManager turnManager = GameObject.Find("GameMechanic").GetComponent<TurnManager>();
        GameObject userInterface = GameObject.Find("UserInterface");

        TurnMessage message = msg.ReadMessage<TurnMessage>();
        turnManager.time = message.time;
        turnManager.currentTeamTurn = message.turn;

        Color white = Color.white;
		white.a = 0.5f;
        GameMechanic mechanic = gameMechanic.GetComponent<GameMechanic>();
        if(mechanic.selectedUnit != null){
            mechanic.unitsButton.transform.Find(mechanic.selectedUnit.name.Split(' ')[0]).GetComponent<Image>().color = white;
            mechanic.selectedUnit = null;
        }
        
        //Reset state of unit to idle
        List<Unit> units = gameMechanic.GetComponent<GameMechanic>().unit;
        foreach(Unit unit in units){
            if(unit.team == turnManager.currentTeamTurn){
                unit.state = "Idle";
                unit.cooldown--;
                if(unit.cooldown < 0){
                    unit.cooldown = 0;
                }
            }
        }

        //Check if all player get a turn then loop to first player
        if(turnManager.currentTeamTurn == 1){
            turnManager.turn++;
            //Increase cost of player each turn
            player.cost++;

            //Havesting skill(Increase cost by 1 per each villager)
            foreach(Unit unit in player.playerUnits){
                if(unit.status.type == "Villager"){
                    player.cost++;
                }
            }

            //reduce cooldown of skill
            foreach(Unit unit in units){
                unit.cooldown--;
                if(unit.cooldown < 0){
                    unit.cooldown = 0;
                }
            }
        }
        
        //if not player turn disable game user interface of player
        if(turnManager.currentTeamTurn != player.team){
			userInterface.transform.Find("MainGame").gameObject.SetActive(false);
            userInterface.transform.Find("UnitDetails").gameObject.SetActive(false);
            userInterface.transform.Find("Class").gameObject.SetActive(false);
            userInterface.transform.Find("MiniMap").gameObject.SetActive(false);
		}else{
			userInterface.transform.Find("MainGame").gameObject.SetActive(true);
        }
    }

    public void SendDefendCommandMessage(Unit unit){
        DefendCommandMessage msg = new DefendCommandMessage();
        msg.name = unit.unitName;
        msg.team = unit.team;
        this.networkManager.client.Send(DEFEND, msg);
    }

    public void OnServerDefendCommandReceived(NetworkMessage msg){
        DefendCommandMessage message = msg.ReadMessage<DefendCommandMessage>();
        NetworkServer.SendToAll(DEFEND, message);
    }

    public void OnClientDefendCommandMessageReceived(NetworkMessage msg){
        DefendCommandMessage message = msg.ReadMessage<DefendCommandMessage>();
        Unit unit = GameObject.Find("Drivers").transform.Find(message.name).GetComponent<Unit>();
        unit.Defend();
    }

    public void SendLoseStatusMessage(int team){
        LoseStatusMessage msg = new LoseStatusMessage();
        msg.team = team;
        this.networkManager.client.Send(LOSE, msg);
    }

    public void OnServerLoseStatusMessageReceived(NetworkMessage msg){
        LoseStatusMessage message = msg.ReadMessage<LoseStatusMessage>();
        NetworkServer.SendToAll(LOSE, message);
    }

    public void OnClientLoseStatusMessageReceived(NetworkMessage msg){
        GameMechanic gameMechanic = GameObject.Find("GameMechanic").GetComponent<GameMechanic>();
        WinCondition winCondition = GameObject.Find("GameMechanic").GetComponent<WinCondition>();
        LoseStatusMessage message = msg.ReadMessage<LoseStatusMessage>();
        for(int i = 0; i < gameMechanic.unit.Count; i++){
            if(gameMechanic.unit[i].team == message.team){
                gameMechanic.unit[i].hp = 0;
            }
        }

        winCondition.playerLoseStatus[message.team - 1] = true;
    }

    public void SendClassChangeMessage(string unitName, string nextClass){
        ClassChangeMessage message = new ClassChangeMessage();
        message.unitName = unitName;
        message.nextClass = nextClass;
        this.networkManager.client.Send(CLASS_CHANGE, message);
    }

    public void OnServerLoseClassChangeMessageReceived(NetworkMessage msg){
        ClassChangeMessage message = msg.ReadMessage<ClassChangeMessage>();
        NetworkServer.SendToAll(CLASS_CHANGE, message);
    }

    public void OnClientClassChangeMessageReceived(NetworkMessage msg){
        ClassChangeMessage message = msg.ReadMessage<ClassChangeMessage>();
        GameObject.Find("GameMechanic").GetComponent<Class>().ClassChange(message.unitName, message.nextClass);
    }

    public void SendSkillMessage(string skill, string unitName, Hexagon target){
        SkillMessage msg = new SkillMessage();
        msg.skill = skill;
        msg.unitName = unitName;
        msg.target = target.ToVector3();
        this.networkManager.client.Send(SKILL, msg);
    }

    public void OnServerSkillMessageReceived(NetworkMessage msg){
        SkillMessage message = msg.ReadMessage<SkillMessage>();
        NetworkServer.SendToAll(SKILL, message);
    }

    public void OnClientSkillMessageReceived(NetworkMessage msg){
        Skill skill = GameObject.Find("GameMechanic").GetComponent<Skill>();
        SkillMessage message = msg.ReadMessage<SkillMessage>();
        skill.Cast(message.skill, message.unitName, Hexagon.ToHexagon(message.target));
    }

    void Start(){
        this.networkManager = gameObject.GetComponent<NetworkManager>();
        //Setting server handler
        NetworkServer.RegisterHandler(TEAM, OnServerTeamRequestMessageReceived);
        NetworkServer.RegisterHandler(MOVE, OnServerMoveMessageReceived);
        NetworkServer.RegisterHandler(ATTACK, OnServerAttackMessageReceived);
        NetworkServer.RegisterHandler(END_TURN, OnServerEndTurnMessageReceived);
        NetworkServer.RegisterHandler(DEFEND, OnServerDefendCommandReceived);
        NetworkServer.RegisterHandler(LOSE, OnServerLoseStatusMessageReceived);
        NetworkServer.RegisterHandler(CLASS_CHANGE, OnServerLoseClassChangeMessageReceived);
        NetworkServer.RegisterHandler(SKILL, OnServerSkillMessageReceived);
    }

    void Update(){
        //Register client handler
        //Debug.Log(NetworkServer.connections.Count);
        if(this.networkManager.client != null && this.networkManager.client.isConnected && this.setting){
            this.setting = false;
            //Debug.Log("Register Client Handler");
            RequestForTeam();
            //this.networkManager.client.RegisterHandler(MsgType.Connect, OnClientConnected);
            this.networkManager.client.RegisterHandler(TEAM, OnClientTeamAssigntMessageReceived);
            this.networkManager.client.RegisterHandler(MOVE, OnClientMoveMessageReceived);
            this.networkManager.client.RegisterHandler(ATTACK, OnClientAttackMessageReceived);
            this.networkManager.client.RegisterHandler(TIMER, OnClientTimerMessageReceived);
            this.networkManager.client.RegisterHandler(END_TURN, OnClientEndTurnMessageReceived);
            this.networkManager.client.RegisterHandler(DEFEND, OnClientDefendCommandMessageReceived);
            this.networkManager.client.RegisterHandler(LOSE, OnClientLoseStatusMessageReceived);
            this.networkManager.client.RegisterHandler(CLASS_CHANGE, OnClientClassChangeMessageReceived);
            this.networkManager.client.RegisterHandler(SKILL, OnClientSkillMessageReceived);
        }

    }
    
    // void OnGUI(){
    //     GUI.Label(new Rect(2, 10, 150, 100), this.hexagon.x+","+this.hexagon.y+","+this.hexagon.z); 
    //     GUI.Label(new Rect(2, 30, 150, 100), this.hexagon2.x+","+this.hexagon2.y+","+this.hexagon2.z);
    //     GUI.Label(new Rect(2, 30, 150, 100), "Press B for both");       
    //     GUI.Label(new Rect(2, 50, 150, 100), "Press C for client");
    // }
}

public class SkillMessage: MessageBase {
    public string skill;
    public string unitName;
    public Vector3 target;
}

public class ClassChangeMessage: MessageBase {
    public string unitName;
    public string nextClass;
}

public class LoseStatusMessage: MessageBase {
    public int team;
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

public class DefendCommandMessage: MessageBase {
    public string name;
    public int team;
}