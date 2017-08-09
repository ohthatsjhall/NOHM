using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using UnityEngine;
using VRTK;

public class FetchAlbumArtwork : MonoBehaviour {

	public APIManager manager;
	private int count = 0;

	public void ShowAlbumArtwork() {
		foreach (Artist artist in manager.artists) {
			StartCoroutine (FetchNewArtwork (artist.albumId));
		}
	}

	IEnumerator FetchNewArtwork(string albumId) {
		string url = NohmConstants.ArtworkBaseURL + albumId + NohmConstants.ArtworkSize500;
		HTTPRequest request = new HTTPRequest (new System.Uri (url), Callback);
		request.Send ();
		yield return request;
	}

	void Callback(HTTPRequest request, HTTPResponse response) {
		int artistIndex = manager.artists [count].index;
		if (count == artistIndex) {
			gameObject.transform.GetChild (artistIndex).GetComponent<Renderer> ().material.mainTexture = response.DataAsTexture2D;
			gameObject.transform.GetChild (artistIndex).GetComponent<Record> ().artist = manager.artists [artistIndex];
			gameObject.transform.GetChild (artistIndex).GetComponent<Record> ().SetTouchHighlightColorForValidURL (manager.artists [artistIndex].previewAvailable);
	
			if (count >= 19) {
				count = 0;
				return;
			} else {
				count++;
				return;
			}
		} 
	}


}
