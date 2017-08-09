using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine;
using BestHTTP;
using SimpleJSON;

public class APIManager : MonoBehaviour {

	[HideInInspector]
	public List<Artist> artists;

	[SerializeField]
	GameObject Records;
	public string artist;

	public void SearchTracksForArtist(string artist) {
		string artistString = artist.Replace (" ", "+");
		HTTPRequest request = new HTTPRequest (new System.Uri (NohmConstants.BaseURL + NohmConstants.SearchEndpoint + artistString + NohmConstants.TrackSearch), RequestCallback);
		request.Send ();
	}

	void RequestCallback(HTTPRequest request, HTTPResponse response) {
		Debug.Log ("response: " + response.DataAsText);
		artists = Artist.ParseJSONData (response.DataAsText);
		Records.gameObject.SetActive (true); //uncomment after testing
		Records.GetComponent<FetchAlbumArtwork> ().ShowAlbumArtwork();
	}
		
}
