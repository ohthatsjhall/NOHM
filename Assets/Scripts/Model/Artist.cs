using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;


[Serializable]
public struct Artist { 
	public string artistName;
	public string imageUrl;
	public string trackName;
	public string previewUrl;
	public string albumName;
	public int index;
	public Texture2D artwork;
	public bool previewAvailable;

	public static List<Artist> ParseJsonData(string jsonString) {
		int count = 0;
		List<Artist> artists = new List<Artist>();
		var data = JSON.Parse (jsonString);
		JSONArray items = data ["tracks"] ["items"].AsArray;
		foreach (JSONObject item in items) {
			string trackName = item ["name"].ToString ();
			string previewURL = item ["preview_url"].ToString ();
			string albumName = item ["album"]["name"].ToString ();
			string imageURL = item ["album"] ["images"] [0] ["url"].ToString ();
			bool previewAvailable;
			JSONArray artistsArray = item ["album"] ["artists"].AsArray;
			foreach(JSONObject singleArtist in artistsArray) {
				string artistName = singleArtist ["name"].ToString();
				Artist artist = new Artist();
				artist.artistName = artistName;

				if (previewURL == "null") {
					trackName = "Not Available";
					previewAvailable = false;
				} else {
					previewAvailable = true;
					previewURL = previewURL.Substring (0, previewURL.Length - 1);
					previewURL = previewURL.Substring (1);
				}

				artist.previewAvailable = previewAvailable;
				artist.previewUrl = previewURL;
				artist.trackName = trackName;
				artist.albumName = albumName;
				imageURL = imageURL.Substring (0, imageURL.Length - 1);
				imageURL = imageURL.Substring (1);
				artist.imageUrl = imageURL;
				artist.index = count;
				artists.Add (artist);

				count++;
			}
		}
		return artists;
	}

	private static string RemoveTicksFromUrl(string url) {
		string newUrl = url.Substring(0, url.Length - 1);
		newUrl = url.Substring (1);
		return newUrl;
	}

}
