using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VinylManager : MonoBehaviour {

	public NohmWatsonManager nohmWatsonManager;
	public APIManager apiManager;

	private string welcomeString = "Hello Dad, Welcome to Nohm Vinyl you dad";

	// Use this for initialization
	void Start () {
		nohmWatsonManager.SayString (welcomeString);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
