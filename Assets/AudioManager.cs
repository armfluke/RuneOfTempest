using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {

	public AudioSource audio;

	// Use this for initialization
	void Start () {
		this.audio = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log(SceneManager.GetActiveScene().name);
		if(SceneManager.GetActiveScene().name == "Game"){
			this.audio.Stop();
		}else{
			if(this.audio.isPlaying == false){
				this.audio.Play();
			}
		}
	}
}
