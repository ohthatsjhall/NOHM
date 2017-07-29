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

	private string token;
	private const string baseURL = "https://api.spotify.com/v1/search?q=";
	private const string clientId = "cb21eafcffa6497098c8cc3ac940abe2";
	private const string clientSecret = "7a765bcc6a644d67904b6bab1546b31b";
	private const string authorizationUrl = "https://accounts.spotify.com/api/token";
	public string artist;

	public void SearchTracksForArtist(string artist) {
		string artistString = artist.Replace (" ", "+");
		HTTPRequest request = new HTTPRequest(new System.Uri(baseURL + artistString + "&type=track"), RequestCallback);
		token = token.Substring (0, token.Length - 1);
		token = token.Substring (1);
		request.AddHeader ("Authorization", "Bearer " + token);
		request.Send ();
	}

	void RequestCallback(HTTPRequest request, HTTPResponse response) {
		Debug.Log ("response: " + response.DataAsText);
		artists = Artist.ParseJsonData (response.DataAsText);
		Records.gameObject.SetActive (true); //uncomment after testing
		Records.GetComponent<FetchAlbumArtwork> ().ShowAlbumArtwork();
	}

	public void FindArtistOrTrack() {
		GetOAuthToken (authorizationUrl, clientId, clientSecret);
	}
		
	private void GetOAuthToken(string urlString, string clientId, string clientSecret) {
		string baseEncodedString = Encode (clientId, clientSecret);
		HTTPRequest request = new HTTPRequest (new System.Uri (urlString), AuthCallback);
		request.MethodType = HTTPMethods.Post;
		request.AddField ("grant_type", "client_credentials");
		request.AddHeader ("Authorization", "Basic " + baseEncodedString);
		request.Send ();
	}

	void AuthCallback(HTTPRequest request, HTTPResponse response) {
		var data = JSON.Parse (response.DataAsText);
		token = data ["access_token"].ToString ();
		SearchTracksForArtist (artist);
	}

	private string Encode(string clientId, string clientSecret) {
		string combined = clientId + ":" + clientSecret;
		byte[] credentialsToEncode = Encoding.UTF8.GetBytes (combined);
		string encodedCredentials = Convert.ToBase64String (credentialsToEncode);
		return encodedCredentials;
	}
}
