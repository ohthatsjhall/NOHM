using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRTK;

public class VinylSnapZone : VRTK_SnapDropZone {

	//public SoundSystem soundSystem;
	public GameObject recordPlayer;
	public bool isOnboarding;

	private GameObject vinylRecord;
	private float spinSpeed;
	public Animator animator;


	protected override void Awake () {
		base.Awake ();

	}

	void Start() {
		//animator = recordPlayer.GetComponent<Animator> ();
	}
		
	protected override void Update() {
		base.Update();
		if (vinylRecord != null) {
			vinylRecord.transform.Rotate (new Vector3 (0.0f, spinSpeed * Time.deltaTime, 0.0f));
		}
	}

	public override void OnObjectEnteredSnapDropZone (SnapDropZoneEventArgs e) {
		base.OnObjectEnteredSnapDropZone (e);
		vinylRecord = e.snappedObject as GameObject;
		Artist artist = vinylRecord.GetComponent<Vinyl> ().artist;
		StartCoroutine (FetchTrack (artist.previewUrl));
	}

	public override void OnObjectSnappedToDropZone (SnapDropZoneEventArgs e) {
		base.OnObjectSnappedToDropZone (e);

		if (isOnboarding) {
			animator.SetBool ("isPlaying", true);
		} else {
			animator.Play ("Playback");
		}

		StartCoroutine (SpinVinyl (e.snappedObject));
		Debug.Log (vinylRecord.GetComponent<Vinyl>().artist.trackName);
		SoundSystem.Instance.PlayOneShot (SoundSystem.Instance.track);
	}

	public override void OnObjectUnsnappedFromDropZone (SnapDropZoneEventArgs e) {
		base.OnObjectUnsnappedFromDropZone (e);

		spinSpeed = 0.0f;

		SoundSystem.Instance.StopAudio();
		if (isOnboarding) {
			animator.SetBool ("isPlaying", false);
		} else {
			animator.Play ("Idle");
		}
	}

	public override void OnObjectExitedSnapDropZone (SnapDropZoneEventArgs e) {
		base.OnObjectExitedSnapDropZone (e);
		e.snappedObject.SetActive (false);
	}

	IEnumerator FetchTrack(string url) {
		WWW www = new WWW (url);
		while(!www.isDone){
			yield return 0;
		}
		SoundSystem.Instance.track = NAudioManager.FromMp3Data(www.bytes);
	}

	IEnumerator SpinVinyl(GameObject currentVinyl) {
		spinSpeed = 55.0f;
		yield return new WaitForSecondsRealtime (30);
		spinSpeed = 0.0f;
		Destroy (currentVinyl);
	}

}