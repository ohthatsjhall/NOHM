using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public struct Artist {

	public int index;
	public int totalCount;
	public int returnedCount;
	public int playbackSeconds;
	public bool isExplicit;
	public string trackName;
	public string artistName;
	public string artistId;
	public string albumName;
	public string albumId;
	public string previewUrl;
	public bool previewAvailable;

	public static List<Artist> ParseJSONData(string json) {
		int count = 0;
		List<Artist> artists = new List<Artist>();

		var data = JSON.Parse (json);

		int totalCount = data["meta"] ["totalCount"].AsInt;
		int returnedCount = data ["meta"] ["returnedCount"].AsInt;

		JSONArray tracks = data ["search"] ["data"] ["tracks"].AsArray;

		foreach (JSONObject track in tracks) {
			int playbackSeconds = track ["playbackSeconds"].AsInt;
			bool isExplicit = track ["isExplicit"].AsBool;
			string trackName = track ["name"].ToString ();
			string artistName = track ["artistName"].ToString ();
			string artistId = track ["artistId"].ToString ();
			string albumName = track ["albumName"].ToString ();
			string albumId = track ["albumId"].ToString ();
			albumId = albumId.Substring(1, albumId.Length - 2);
			string previewUrl = track ["previewURL"].ToString ();
			previewUrl = previewUrl.Substring (1, previewUrl.Length - 2);
			bool previewAvailable = previewUrl != "" ? true : false;

			Artist artist = SingleArtist (
				count,
				totalCount,
				returnedCount,
				playbackSeconds,
				isExplicit,
				trackName,
				artistName,
				artistId,
				albumName,
				albumId,
				previewUrl,
				previewAvailable);

			Debug.Log (GetArtistInfo(artist));
			artists.Add (artist);

			count++;
		}
		return artists;
	}

	// constructor to easily build isntance of Artist
	private static Artist SingleArtist(int index, int totalCount, int returnedCount, int playbackSeconds, 
		bool isExplicit, string trackName, string artistName, string artistId, 
		string albumName, string albumId, string previewUrl, bool previewAvailable) {

		Artist newArtist = new Artist ();
		newArtist.index = index;
		newArtist.totalCount = totalCount;
		newArtist.returnedCount = returnedCount;
		newArtist.playbackSeconds = playbackSeconds;
		newArtist.isExplicit = isExplicit;
		newArtist.trackName = trackName;
		newArtist.artistName = artistName;
		newArtist.albumName = albumName;
		newArtist.albumId = albumId;
		newArtist.previewUrl = previewUrl;
		newArtist.previewAvailable = previewAvailable;
		return newArtist;

	}

	// convienence function to get track details
	public static string GetArtistInfo(Artist artist) {
		string details = "";
		details += "Artist Name: " + artist.artistName + "\n";
		details += "Track Name: " + artist.trackName + "\n";
		details += "Album Name: " + artist.albumName + "\n";
		details += "Preview Available?: " + artist.previewAvailable + "\n";
		return details;
	}

}