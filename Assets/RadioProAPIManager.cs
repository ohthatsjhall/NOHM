using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NOHM;
using Crosstales.Radio;

public class RadioProAPIManager : MonoBehaviour {

	[HideInInspector]
	public List<RadioStation> radioStations;

	public RadioPlayer radioPlayer;

	// Use this for initialization
	void Start () {
		radioStations = LoadRadioStations ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private List<RadioStation> LoadRadioStations()
	{
		List<RadioStation> tempStations = new List<RadioStation>();

		RadioStation amsterdamTrance = RadioStation.SingleRadioStation ("Amsterdam Trance", "http://185.33.21.112:11029/", "trance");
		RadioStation reggaeTradeRadio = RadioStation.SingleRadioStation ("Reggaetrade Radio", "http://185.33.22.15:11202/", "reggae");
		tempStations.Add (amsterdamTrance);
		tempStations.Add (reggaeTradeRadio);

		return tempStations;
	}
}
