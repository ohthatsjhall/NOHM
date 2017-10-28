using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Widgets;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;

public class SpeechToTextManager : MonoBehaviour {

	private string username = "4cae95ac-77e7-44cd-a1fa-8ae159a06ba7";
	private string password = "EqGfHpod6Wsb";
	private string url = "https://stream.watsonplatform.net/speech-to-text/api";

	private int _recordingRoutine = 0;
	private string _microphoneID = null;
	private AudioClip _recording = null;
	private int _recordingBufferSize = 2;
	private int _recordingHZ = 22050;

	private SpeechToText _speechToText;
	private NohmWatsonManager nohmWatsonManager;

	void Start()
	{
		LogSystem.InstallDefaultReactors();

		//  Create credential and instantiate service
		Credentials credentials = new Credentials(username, password, url);

		_speechToText = new SpeechToText(credentials);
		nohmWatsonManager = GetComponent<NohmWatsonManager>();

		// Active = true;

		// StartRecording();
	}

	public bool Active
	{
		get { return _speechToText.IsListening; }
		set
		{
			if (value && !_speechToText.IsListening)
			{
				_speechToText.DetectSilence = true;
				_speechToText.EnableWordConfidence = false;
				_speechToText.EnableTimestamps = false;
				_speechToText.SilenceThreshold = 0.03f;
				_speechToText.MaxAlternatives = 1;
				_speechToText.EnableContinousRecognition = true;
				_speechToText.EnableInterimResults = true;
				_speechToText.OnError = OnError;
				_speechToText.StartListening(OnRecognize);
				List<string> keywords = new List<string>();
				keywords.Add("hello");
				_speechToText.KeywordsThreshold = 0.5f;
				_speechToText.Keywords = keywords.ToArray();
			}
			else if (!value && _speechToText.IsListening)
			{
				_speechToText.StopListening();
			}
		}
	}

	public void StartRecording()
	{
		Active = true;
		if (_recordingRoutine == 0)
		{
			UnityObjectUtil.StartDestroyQueue();
			_recordingRoutine = Runnable.Run(RecordingHandler());
		}
	}

	public void StopRecording()
	{
		Active = false;
		if (_recordingRoutine != 0)
		{
			Microphone.End(_microphoneID);
			Runnable.Stop(_recordingRoutine);
			_recordingRoutine = 0;
		}
	}

	private void OnError(string error)
	{
		Active = false;

		Log.Debug("ExampleStreaming", "Error! {0}", error);
	}

	private IEnumerator RecordingHandler()
	{
		Log.Debug("ExampleStreaming", "devices: {0}", Microphone.devices);
		_recording = Microphone.Start(_microphoneID, true, _recordingBufferSize, _recordingHZ);
		yield return null;      // let _recordingRoutine get set..

		if (_recording == null)
		{
			StopRecording();
			yield break;
		}

		bool bFirstBlock = true;
		int midPoint = _recording.samples / 2;
		float[] samples = null;

		while (_recordingRoutine != 0 && _recording != null)
		{
			int writePos = Microphone.GetPosition(_microphoneID);
			if (writePos > _recording.samples || !Microphone.IsRecording(_microphoneID))
			{
				Log.Error("MicrophoneWidget", "Microphone disconnected.");

				StopRecording();
				yield break;
			}

			if ((bFirstBlock && writePos >= midPoint)
				|| (!bFirstBlock && writePos < midPoint))
			{
				// front block is recorded, make a RecordClip and pass it onto our callback.
				samples = new float[midPoint];
				_recording.GetData(samples, bFirstBlock ? 0 : midPoint);

				AudioData record = new AudioData();
				record.MaxLevel = Mathf.Max(samples);
				record.Clip = AudioClip.Create("Recording", midPoint, _recording.channels, _recordingHZ, false);
				record.Clip.SetData(samples, 0);

				_speechToText.OnListen(record);

				bFirstBlock = !bFirstBlock;
			}
			else
			{
				// calculate the number of samples remaining until we ready for a block of audio, 
				// and wait that amount of time it will take to record.
				int remaining = bFirstBlock ? (midPoint - writePos) : (_recording.samples - writePos);
				float timeRemaining = (float)remaining / (float)_recordingHZ;

				yield return new WaitForSeconds(timeRemaining);
			}

		}

		yield break;
	}

	private void OnRecognize (SpeechRecognitionEvent result)
	{
		if (result != null && result.results.Length > 0) {
			foreach (var res in result.results) {
				foreach (var alt in res.alternatives) {
					string text = alt.transcript;
					Log.Debug ("ExampleStreaming", string.Format ("{0} ({1}, {2:0.00})\n", text, res.final ? "Final" : "Interim", alt.confidence));
					if (res.final && alt.confidence > 0.75) {
						Debug.Log("text after final check: " + text);


						if (nohmWatsonManager.buildIndex == 1)
							OnRecognizeOnboardingArtist (text);
						else
							nohmWatsonManager.RecognizeQuestion (text);


					}
				}

				if (res.keywords_result != null && res.keywords_result.keyword != null)
				{
					foreach (var keyword in res.keywords_result.keyword)
					{
						Log.Debug("ExampleSpeechToText", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
					}
				}
			}
		}
	}


	private void OnRecognizeOnboardingArtist(string text) 
	{
		string resultText = text.ToLower ();
		if ((resultText.Contains ("frank")) || (resultText.Contains ("ocean"))) {
			nohmWatsonManager.RecognizeQuestion (text);
			nohmWatsonManager.tutorialManager.ClearCanvas ();
		} else {
			Debug.Log ("didnt say Frank Ocean");
			nohmWatsonManager.SayString ("try again by searching for the artist, Frank Ocean");
		}
		nohmWatsonManager.StopRecording ();
	}

}
