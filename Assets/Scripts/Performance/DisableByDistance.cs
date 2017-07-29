using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableByDistance : MonoBehaviour {

	[SerializeField]
	GameObject target;
	[SerializeField]
	float maxDistance;
	[SerializeField]
	int coroutineFrequency;

	void Start () {
		StartCoroutine (DisableAtDistance());
	}

	IEnumerator DisableAtDistance() {
		while (true) {

			foreach (Transform child in transform) {
				
				float distanceSq = (transform.position - target.transform.position).sqrMagnitude;
				if (distanceSq < (maxDistance * maxDistance)) {
					child.gameObject.SetActive (true);
					Debug.Log ("active tho");
				} else {
					child.gameObject.SetActive (false);
					Debug.Log ("NOT ACTIVE");
				}
				for (int i = 0; i < coroutineFrequency; i++) {
					yield return new WaitForEndOfFrame ();
				}
			}
		}
	}

}
