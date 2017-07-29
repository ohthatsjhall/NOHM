using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;

public static class AlbumArtwork {

	public static Texture2D albumArtwork;

	public static void FetchArtworkAtUrl(string imageUrl) {
		FetchArtworkForAlbum (imageUrl);
	}

	private static void FetchArtworkForAlbum(string url) {
		HTTPRequest request = new HTTPRequest (new System.Uri (url), Callback);
		request.Send ();
	}

	private static void Callback(HTTPRequest request, HTTPResponse response) {
		albumArtwork = response.DataAsTexture2D;
	}
}
