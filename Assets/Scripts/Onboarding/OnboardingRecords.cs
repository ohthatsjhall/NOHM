using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;

public class OnboardingRecords : MonoBehaviour {

	private List<Artist> onboardingArtists;
	private int count = 0;
	private Animator recordAnimator;

	// Use this for initialization
	void Start () {
		recordAnimator = GetComponent<Animator> ();
		AddArtistData ();
	}

	private void AddArtistData() {
		onboardingArtists = new List<Artist>();
		Artist firstArtist = new Artist ();
		firstArtist.previewUrl = "https://p.scdn.co/mp3-preview/d3e8d7ced6e0f3844b8c0309a7d4baad511a3518?cid=8897482848704f2a8f8d7c79726a70d4";
		firstArtist.albumName = "Slide";
		firstArtist.artistName = "Calvin Harris";
		firstArtist.trackName = "Slide";
		firstArtist.imageUrl = "https://i.scdn.co/image/24a63c5c743b4376b7b7d570cf4a83ea017382fd";
		firstArtist.index = 0;
		onboardingArtists.Add (firstArtist);

		Artist secondArtist = new Artist ();
		secondArtist.previewUrl = "https://p.scdn.co/mp3-preview/be2bc4aadde588c1a381e878497db1a165706b18?cid=8897482848704f2a8f8d7c79726a70d4";
		secondArtist.albumName = "Blonde";
		secondArtist.artistName = "Frank Ocean";
		secondArtist.trackName = "Nikes";
		secondArtist.imageUrl = "https://i.scdn.co/image/e22f959dae6f088b9c6614c4f777efacaf3544f1";
		secondArtist.index = 1;
		onboardingArtists.Add (secondArtist);

		Artist thirdArtist = new Artist ();
		thirdArtist.previewUrl = "https://p.scdn.co/mp3-preview/43abb77afc35b8eced064a1b062921a13f5c7b15?cid=8897482848704f2a8f8d7c79726a70d4";
		thirdArtist.albumName = "Blonde";
		thirdArtist.artistName = "Frank Ocean";
		thirdArtist.trackName = "Nights";
		thirdArtist.imageUrl = "https://i.scdn.co/image/b9aa26b76e088da60aa9adf4d30fa39b98075a0d";
		thirdArtist.index = 2;
		onboardingArtists.Add (thirdArtist);

		ShowAlbumArtwork ();
	}

	public void ShowAlbumArtwork() {
		for (var i = 0; i < onboardingArtists.Count; i++) {
			Artist artist = onboardingArtists [i];
			gameObject.transform.GetChild (artist.index).GetComponent<Record> ().artist = artist;
			StartCoroutine(FetchAlbumArtwork(artist.imageUrl));
		}
	}

	IEnumerator FetchAlbumArtwork(string url) {
		HTTPRequest request = new HTTPRequest (new System.Uri (url), AlbumArtworkCallback);
		request.Send ();
		yield return request;
	}

	void AlbumArtworkCallback(HTTPRequest request, HTTPResponse response) {
		gameObject.transform.GetChild (count).GetComponent<Renderer> ().material.mainTexture = response.DataAsTexture2D;
		count++;
	}

	public void AnimateRecords(bool fetchCompleted) {
		if (fetchCompleted) {
			foreach (Transform child in transform) {
				child.gameObject.SetActive (true);
			}
			recordAnimator.SetBool ("recordsShown", true);
		}
	}

}
