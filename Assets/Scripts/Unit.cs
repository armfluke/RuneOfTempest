using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
	
	public Hexagon position;
	public string team;
	public int hp;
	public Status status;
	public string state;
	Hexagon hexagon;
	float speed = 3f;
	Vector3 targetPosition;
	RaycastHit hitInfo;
	bool moving = false;
	bool attacking = false;
	Animator animator;

	public void Move(Hexagon target){
		this.position = target;
		this.targetPosition = GameObject.Find("Drivers").transform.Find("Map").Find(target.x + "," + target.y + "," + target.z).position;
		this.targetPosition[1] += 0.5f;
		this.moving = true;
	}

	private void Moving(){
		//Play moving animation
		if(this.moving && !this.animator.GetCurrentAnimatorStateInfo(0).IsName("Move")){
			this.animator.ResetTrigger("Idle");
			this.animator.SetTrigger("Move");
		}else if(this.moving == false && !this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")){
			this.animator.SetTrigger("Idle");
			this.animator.ResetTrigger("Move");
		}

		//Check if unit is moving
		if(this.moving){
			//Rotate unit toward target
			Quaternion target = Quaternion.LookRotation(this.targetPosition - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * speed);

			//Moving unit by transform its position
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, this.targetPosition, step);
		}
		
		//Check if unit is reached its destination
		if(Mathf.Abs(transform.position.x - this.targetPosition.x) < 0.1 && Mathf.Abs(transform.position.z - this.targetPosition.z) < 0.1){
			//Stop moving
			this.moving = false;
			//Rotate unit back to its origin
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * this.speed);

		}
	}

	public void Attack(Unit target){
		this.animator.SetTrigger("Attack");
		this.attacking = true;
		target.hp -= this.status.attack;
		this.targetPosition = GameObject.Find("Drivers").transform.Find("Map").Find(target.position.x + "," + target.position.y + "," + target.position.z).position;
		//Checking if unit died
	}

	private bool isCoroutineExecuting = false;
	IEnumerator ExecuteAfterTime(float time){
		//Check if coroutine is already execute
		if(isCoroutineExecuting){
			yield break;
		}
		isCoroutineExecuting = true;
		//Wait For seconds
		yield return new WaitForSeconds(time);
		// Code to execute after the delay
		isCoroutineExecuting = false;
		this.attacking = false;
	}

	private void Attacking(){

		if(this.attacking){
			//Rotate unit toward target
			Quaternion targetPosition = Quaternion.LookRotation(this.targetPosition - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetPosition, Time.deltaTime * speed);

			//Stop attacking after some seconds
			StartCoroutine(ExecuteAfterTime(3f));
		}else{
			//Rotate unit back to its origin
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * this.speed);
		}
	}

	// Use this for initialization
	void Start () {
		this.targetPosition = transform.position;
		this.animator = transform.gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

		//Moving unit
		Moving();
		//Attacking unit
		Attacking();

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
