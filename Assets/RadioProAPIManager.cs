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

		// Hip-Hop

		RadioStation jamzRadio = RadioStation.SingleRadioStation("Jamz Radio", "http://185.33.21.112:11142", "hip hop");
		tempStations.Add (jamzRadio);

		RadioStation breakzFM = RadioStation.SingleRadioStation("Breakz FM", "http://144.76.138.169:8200/stream48/", "hip hop");
		tempStations.Add (breakzFM);

		RadioStation hipHopRequest = RadioStation.SingleRadioStation("Hip Hop Request", "http://144.76.138.169:8200/stream48/", "hip hop");
		tempStations.Add (hipHopRequest);

		RadioStation fluid = RadioStation.SingleRadioStation("Fluid", "http://ice1.somafm.com/fluid-128-mp3", "hip hop");
		tempStations.Add (fluid);

		// House

		RadioStation houseTime = RadioStation.SingleRadioStation("House Time", "http://listen.housetime.fm/tunein-mp3-asx", "house");
		tempStations.Add (houseTime);

		RadioStation beatBlender = RadioStation.SingleRadioStation("Beat Blender", "http://ice1.somafm.com/beatblender-128-mp3", "house");
		tempStations.Add (beatBlender);

		RadioStation house = RadioStation.SingleRadioStation("House", "http://house.mthn.net:8500/", "house");
		tempStations.Add (house);

		RadioStation electoradio = RadioStation.SingleRadioStation("Electoradio.fm", "http://stream.electroradio.fm/192k/", "house");
		tempStations.Add (electoradio);

		// Reggae

		RadioStation reggaeTradeRadio = RadioStation.SingleRadioStation ("Reggaetrade Radio", "http://185.33.22.15:11202/", "reggae");
		tempStations.Add (reggaeTradeRadio);

		RadioStation ambianceRadio = RadioStation.SingleRadioStation ("Ambiance Radio", "http://streaming.radionomy.com/Ambiance-Reggae", "reggae");
		tempStations.Add (ambianceRadio);

		RadioStation seneethiopia = RadioStation.SingleRadioStation ("Seneethiopia", "http://streaming.radionomy.com/SeneEthiopia", "reggae");
		tempStations.Add (seneethiopia);

		RadioStation jointRadio = RadioStation.SingleRadioStation ("Joint Radio", "http://star.jointil.net/proxy/jrn_reggae?mp=/stream/", "reggae");
		tempStations.Add (jointRadio);

		RadioStation reggaenation = RadioStation.SingleRadioStation ("Reggaenation", "http://streaming.radionomy.com/ReggaeNation", "reggae");
		tempStations.Add (reggaenation);

		// Trance

		RadioStation amsterdamTrance = RadioStation.SingleRadioStation ("Amsterdam Trance", "http://185.33.21.112:11029/", "trance");
		tempStations.Add (amsterdamTrance);


		return tempStations;
	}
}
