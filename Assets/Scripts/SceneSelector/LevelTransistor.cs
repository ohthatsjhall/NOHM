using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.SceneManagement;

public class LevelTransistor : MonoBehaviour {

	void OnTriggerEnter(Collider collider) 
	{
		if (collider.gameObject.tag == "Balloon") 
		{
			int levelToLoad = collider.gameObject.GetComponent<Balloon> ().levelToLoad;
			Debug.Log ("Level To Load: " + levelToLoad);
			LoadLevel (levelToLoad);
		}
	}

	void LoadLevel(int index)
	{
		VRTK_SDKManager.instance.UnloadSDKSetup ();
		SceneManager.LoadSceneAsync (index);
	}
}
