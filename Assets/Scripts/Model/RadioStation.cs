using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NOHM {
	public struct RadioStation {

		public string name;
		public string url;
		public string genre;

		public static RadioStation SingleRadioStation(string name, string url, string genre)
		{
			RadioStation newStation = new RadioStation ();
			newStation.name = name;
			newStation.url = url;
			newStation.genre = genre;

			return newStation;
		}

	}
}