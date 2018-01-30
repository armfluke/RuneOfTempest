using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Unit : MonoBehaviour {
	
	public Hexagon position;
	public string unitName;
	public int team;
	public int hp;
	public Status status;
	public string state;
	Hexagon hexagon;
	Vector3 targetPosition;
	RaycastHit hitInfo;
	bool attacking = false;
	Animator animator;
	Vector3 different;
	private int frameMoving;
	private int frameAttacking;
	float diffAngle;

	public float calculateDifferentAngle(){
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
		this.position = target;
		this.targetPosition = GameObject.Find("Drivers").transform.Find("Map").Find(target.x + "," + target.y + "," + target.z)
							.position + new Vector3(0, 0.5f, 0);
		
		this.different = (this.targetPosition - transform.position) / 45;
		
		this.diffAngle = calculateDifferentAngle() / 15;
		
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
		this.animator.SetTrigger("Attack");
		this.attacking = true;

		this.targetPosition = GameObject.Find("Drivers").transform.Find("Map").Find(target.position.x + "," + target.position.y + "," + target.position.z).position;
		this.diffAngle = calculateDifferentAngle() / 15;

		this.frameAttacking = 0;

		target.hp -= this.status.attack;
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
	//StartCoroutine(ExecuteAfterTime(3f));

	private void Attacking(){
		this.frameAttacking++;
		if(frameAttacking <= 15){
			transform.Rotate(Vector3.up * this.diffAngle, Space.Self);
		}
	}

	// Use this for initialization
	void Start () {
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
