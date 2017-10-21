using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Balloon))]
public class LevelTransistionEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector ();
		if (GUILayout.Button ("Clear Player Prefs"))
			PlayerPrefs.DeleteKey ("OnboardingCompleted");
	}
}
