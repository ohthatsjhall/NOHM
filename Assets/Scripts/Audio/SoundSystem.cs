using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour {

	public static SoundSystem Instance = null;

	public AudioClip track;
	public AudioClip vinylEffect;
	private AudioSource soundAudio;
	private AudioSource vinylTrack;

	// Use this for initialization
	void Start () {

		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy(gameObject);
		}

		AudioSource[] sources = GetComponents<AudioSource>();
		foreach (AudioSource source in sources) {
			if (source.clip == null) {
				soundAudio = source;
			} else {
				vinylTrack = source;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayOneShot(AudioClip clip) {
		soundAudio.PlayOneShot (clip);
		vinylTrack.Play ();
	}

	public void StopAudio() {
		if (soundAudio.isPlaying) {
			soundAudio.Stop();
		}
		vinylTrack.Stop ();
	}
}
