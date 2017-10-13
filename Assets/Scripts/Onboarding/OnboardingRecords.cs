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
		firstArtist.previewUrl = "http://listen.vo.llnwd.net/g3/5/4/3/4/7/1330574345.mp3";
		firstArtist.albumName = "Slide";
		firstArtist.artistName = "Calvin Harris";
		firstArtist.trackName = "Slide";
		firstArtist.albumId = "alb.253543923";
		firstArtist.index = 0;
		onboardingArtists.Add (firstArtist);

		Artist secondArtist = new Artist ();
		secondArtist.previewUrl = "http://listen.vo.llnwd.net/g3/7/5/9/4/0/1341604957.mp3";
		secondArtist.albumName = "Blonde";
		secondArtist.artistName = "Frank Ocean";
		secondArtist.trackName = "Nikes";
		secondArtist.albumId = "alb.243666117";
		secondArtist.index = 1;
		onboardingArtists.Add (secondArtist);

		Artist thirdArtist = new Artist ();
		thirdArtist.previewUrl = "http://listen.vo.llnwd.net/g3/7/6/9/9/9/1282299967.mp3";
		thirdArtist.albumName = "Channel Orange";
		thirdArtist.artistName = "Frank Ocean";
		thirdArtist.trackName = "Thinkin' Bout You";
		thirdArtist.albumId = "alb.62368630";
		thirdArtist.index = 2;
		onboardingArtists.Add (thirdArtist);

		ShowAlbumArtwork ();
	}

	public void ShowAlbumArtwork() {
		for (var i = 0; i < onboardingArtists.Count; i++) {
			Artist artist = onboardingArtists [i];
			gameObject.transform.GetChild (artist.index).GetComponent<Record> ().artist = artist;
			StartCoroutine(FetchAlbumArtwork(artist.albumId));
		}
	}

	IEnumerator FetchAlbumArtwork(string albumId) {
		string url = NohmConstants.ArtworkBaseURL + albumId + NohmConstants.ArtworkSize500;
		HTTPRequest request = new HTTPRequest (new System.Uri (url), AlbumArtworkCallback);
		request.Send ();
		yield return request;
	}

	void AlbumArtworkCallback(HTTPRequest request, HTTPResponse response) {
		Texture2D artwork = new Texture2D(1, 1);
		artwork.LoadImage(response.Data);
		gameObject.transform.GetChild (count).GetComponent<Renderer> ().material.mainTexture = artwork;
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
