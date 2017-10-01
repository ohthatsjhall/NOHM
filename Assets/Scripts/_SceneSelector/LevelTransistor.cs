using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.SceneManagement;

public class LevelTransistor : MonoBehaviour {

	[SerializeField] Balloon balloon;
	//[SerializeField] GameObject balloons;

	void OnTriggerEnter(Collider collider) 
	{
		// Balloon balloon = balloons.GetComponentInChildren<Balloon> ();
		LoadLevel (balloon.levelToLoad);
	}

	void LoadLevel(int index)
	{
		VRTK_SDKManager.instance.UnloadSDKSetup ();
		SceneManager.LoadSceneAsync (index);
	}
}
