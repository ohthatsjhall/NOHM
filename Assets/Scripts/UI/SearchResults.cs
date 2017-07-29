using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hover.InterfaceModules.Key;
using UnityEngine.UI;

public class SearchResults : MonoBehaviour {

	public GameObject keyboard;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		string searchText = GetComponent<HoverkeyTextInput> ().TextInput;
		keyboard.GetComponentInChildren<Text> ().text = searchText;
	}
}
