using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransistor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider collider) {
		Debug.Log ("game object " + collider.gameObject);
		Debug.Log ("hit");
	}
}
