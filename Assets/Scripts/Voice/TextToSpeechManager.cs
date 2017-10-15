using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;

public class TextToSpeechManager : MonoBehaviour {

	private string _username = "5b3be75c-26c7-4f21-a5af-63f38a6cc259";
	private string _password = "v1XVUon08c2d";
	private string _url = "https://stream.watsonplatform.net/text-to-speech/api";
	// private string _testString = "Zach, you have the most bones I have ever seen...and your gnar levels are off the charts";

	TextToSpeech _textToSpeech;

	void Start() {
		LogSystem.InstallDefaultReactors();

		//  Create credential and instantiate service
		Credentials credentials = new Credentials(_username, _password, _url);

		_textToSpeech = new TextToSpeech(credentials);
		_textToSpeech.Voice = VoiceType.en_GB_Kate;
		//_textToSpeech.ToSpeech(_testString, HandleToSpeechCallback, true);
	}

	public void Say (string speechString) {
		_textToSpeech.ToSpeech(speechString, HandleToSpeechCallback, true);
	}

	void HandleToSpeechCallback(AudioClip clip, string customData) {
		PlayClip(clip);
	}

	private void PlayClip(AudioClip clip)
	{
		if (Application.isPlaying && clip != null)
		{
			GameObject audioObject = new GameObject("AudioObject");
			AudioSource source = audioObject.AddComponent<AudioSource>();
			source.spatialBlend = 0.0f;
			source.loop = false;
			source.clip = clip;
			source.Play();

			Destroy(audioObject, clip.length);

			//            _synthesizeTested = true;
		}
	}
}