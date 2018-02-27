using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Unit : MonoBehaviour {
	
	public Hexagon position;
	public string unitName;
	public int team;
	public int hp;
	public Status status;
	public string state = "Idle";
	Vector3 targetPosition;
	//RaycastHit hitInfo;
	bool attacking = false;
	Animator animator;
	Vector3 different;
	private int frameMoving;
	private int frameAttacking;
	float diffAngle;
	public RectTransform healthBar;
	public Text healthText;
	public GameMechanic gameMechanic;
	public Player player;
	public Dictionary<string ,GameObject> unitState = new Dictionary<string, GameObject>();

	public float CalculateDifferentAngle(){
		//Calculate rotation angle
		Vector3 targetDir = this.targetPosition - transform.position;
        //  Acute angle [0,180]
        float angle = Vector3.Angle(targetDir, transform.forward);

        //  -Acute angle [180,-179]
        float sign = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(targetDir, transform.forward)));
        float signed_angle = angle * -sign;
		return signed_angle;
	}

	public void Move(Hexagon target){
		this.state  = "Move";

		this.position = target;

		this.targetPosition = GameObject.Find("GameMechanic").GetComponent<GameMechanic>().map.transform.Find(target.x + "," + target.y + "," + target.z)
							.position + new Vector3(0, 0.5f, 0);
		
		this.different = (this.targetPosition - transform.position) / 45;
		
		this.diffAngle = CalculateDifferentAngle() / 15;
		
		this.frameMoving = 0;
		
		//Play moving animation
		this.animator.ResetTrigger("Idle");
		this.animator.SetTrigger("Move");
	}

	private void Moving(){
		this.frameMoving++;
		if(this.frameMoving <= 45){
			if(this.frameMoving == 45){
				//Stop moving and play idle animation
				this.animator.ResetTrigger("Move");
				this.animator.SetTrigger("Idle");
			}
			transform.position += this.different;

			if(this.frameMoving <= 15){
				transform.Rotate(Vector3.up * this.diffAngle, Space.Self);
			}
		}
	}

	public void Attack(Unit target){
		this.state = "Attack";

		this.animator.SetTrigger("Attack");
		this.attacking = true;

		this.targetPosition = GameObject.Find("GameMechanic").GetComponent<GameMechanic>().map.transform.Find(target.position.x + "," + target.position.y + "," + target.position.z).position;
		this.diffAngle = CalculateDifferentAngle() / 15;

		this.frameAttacking = 0;

		target.hp -= this.status.attack;
	}

	private bool isCoroutineExecuting = false;
	/*IEnumerator ExecuteAfterTime(float time){
		//Check if coroutine is already execute
		if(isCoroutineExecuting){
			yield break;
		}
		isCoroutineExecuting = true;
		//Wait For seconds
		yield return new WaitForSeconds(time);
		// Code to execute after the delay

		isCoroutineExecuting = false;
	}
	StartCoroutine(ExecuteAfterTime(3f));*/
	private void Attacking(){
		this.frameAttacking++;
		if(frameAttacking <= 15){
			transform.Rotate(Vector3.up * this.diffAngle, Space.Self);
		}
	}

	public void Defend(){
		this.state = "Defend";
		Debug.Log(this.unitName + " Defend");
	}

	IEnumerator DelayBeforeDie(float time){
		//Check if coroutine is already execute
		if(isCoroutineExecuting){
			yield break;
		}
		isCoroutineExecuting = true;
		//Wait For seconds
		yield return new WaitForSeconds(time);
		// Code to execute after the delay
		GameObject miniMap = GameObject.Find("UserInterface").transform.Find("MiniMap").Find("MiniMap").gameObject;
		GameObject unitImage = miniMap.transform.Find(position.x+","+position.y+","+position.z).Find(this.unitName).gameObject;
		Destroy(unitImage);

		int index = 0;
		if(this.team == this.player.team){
			foreach(Unit unit in this.player.playerUnits){
				if(unit.unitName == this.unitName){
					this.player.playerUnits.RemoveAt(index);
					break;
				}
				index++;
			}
		}

		index = 0;
		foreach(Unit unit in this.gameMechanic.unit){
			if(unit.unitName == this.unitName){
				this.gameMechanic.unit.RemoveAt(index);
				break;
			}
			index++;
		}

		Destroy(gameObject);

		isCoroutineExecuting = false;
	}

	// Use this for initialization
	void Start () {
		this.gameMechanic = GameObject.Find("GameMechanic").GetComponent<GameMechanic>();
		this.player = GameObject.Find("Player").GetComponent<Player>();
		this.animator = transform.Find(this.unitName).GetComponent<Animator>();
		this.healthBar = transform.Find("Health").Find("Background").Find("Foreground").GetComponent<RectTransform>();
		this.healthText = transform.Find("Health").Find("Text").GetComponent<Text>();

		Transform stateObject = transform.Find("Health").Find("State");
		//Get each game object state
		this.unitState["Idle"] = stateObject.Find("Idle").gameObject;
		this.unitState["Move"] = stateObject.Find("Move").gameObject;
		this.unitState["Attack"] = stateObject.Find("Attack").gameObject;
		this.unitState["Defend"] = stateObject.Find("Defend").gameObject;
		this.unitState["Skill"] = stateObject.Find("Skill").gameObject;
		this.unitState["Rest"] = stateObject.Find("Rest").gameObject;
		this.unitState["Die"] = stateObject.Find("Die").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		//Adjust health of unit
		this.healthBar.sizeDelta = new Vector2(((float)this.hp / (float)this.status.maxHp) * 100, this.healthBar.sizeDelta.y);
		this.healthText.text = this.hp + " / " + this.status.maxHp;

		//Moving unit
		Moving();
		//Attacking unit
		Attacking();
		
		//If hp <= 0 unit die
		if(this.hp <= 0){
			this.hp = 0;
			this.animator.SetTrigger("Die");
			StartCoroutine(DelayBeforeDie(4f));
			Debug.Log(this.unitName + " died");
		}

		//Change state sign to current unit state
		foreach(KeyValuePair<string, GameObject> entry in this.unitState){
			if(entry.Value.name == this.state){
				entry.Value.SetActive(true);
			}else{
				entry.Value.SetActive(false);
			}
		}

		//Using raycast to detect character position
		//Shoot raycast to downward
		/*Debug.DrawLine(transform.position, Vector3.down);
		if (Physics.Raycast(transform.position, Vector3.down, out this.hitInfo)){
			//Debug.Log(hitInfo.transform.gameObject.name);
			if(this.hitInfo.transform.parent.name == "Map"){
				hexagon = this.hitInfo.transform.GetComponent<Hexagon>();
				this.position[0] = hexagon.x;
				this.position[1] = hexagon.y;
				this.position[2] = hexagon.z;
				Debug.Log(hexagon.x + "," + hexagon.y + "," + hexagon.z);
			}
			//print("There is something " + hitInfo.distance +"m away from gameobject");
		}*/
	}
}
