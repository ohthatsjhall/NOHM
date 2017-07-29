using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RecordPlayer : MonoBehaviour {

	public APIManager manager;
	public GameObject albums;
	private AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
	}

	public void PlayTrack() {
		audioSource.PlayOneShot (SoundSystem.Instance.track);
		//audioSource.PlayOneShot (SoundSystem.Instance.vinylEffect);
	}

	/*
	void OnTriggerEnter(Collider collider) {
		string colliderObject = collider.gameObject.name;
		if (colliderObject == "Body") {
			if (!albums.activeInHierarchy) {
				albums.SetActive (true);
			}
		}
	}
	*/
}
