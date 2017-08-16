using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Widgets;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;

using FullSerializer;

#pragma warning disable 414

public class VoiceSpawner : Widget {

	public APIManager apiManager;
	public AudioClip sorryClip;
	public List<AudioClip> helpClips;
	public MicrophoneWidget microphone;

	private Conversation m_Conversation = new Conversation();
	private string m_WorkspaceID;
	private bool artistReturned = false;

	[SerializeField]
	private Input m_SpeechInput = new Input("SpeechInput", typeof(SpeechToTextData), "OnSpeechInput");
	private fsSerializer _serializer = new fsSerializer();

	TextToSpeech textToSpeech = new TextToSpeech();
	private string welcomeString = "Welcome to Gnome, when you are ready to listen to music, press the right trigger";

	#region InitAndLifecycle
	//------------------------------------------------------------------------------------------------------------------
	// Initialization and Lifecycle
	//------------------------------------------------------------------------------------------------------------------

	protected override void Start() {
		base.Start();
		textToSpeech.Voice = VoiceType.en_GB_Kate;
		textToSpeech.ToSpeech (welcomeString, HandleToSpeechCallback);
		m_WorkspaceID = Config.Instance.GetVariableValue("ConversationV1_ID");
	}

	void HandleToSpeechCallback (AudioClip clip) {
		Debug.Log ("ready to play...");
		PlayClip (clip);
	}

	private void PlayClip(AudioClip clip) {
		Debug.Log ("playing");
		if (Application.isPlaying && clip != null) {
			GameObject audioObject = new GameObject("AudioObject");
			AudioSource source = audioObject.AddComponent<AudioSource>();
			source.spatialBlend = 0.0f;
			source.loop = false;
			source.clip = clip;
			source.PlayOneShot (clip);

			GameObject.Destroy(audioObject, clip.length);
		}
	}

	protected override string GetName()
	{
		return "VoiceSpawner";
	}
	#endregion

	#region EventHandlers
	//------------------------------------------------------------------------------------------------------------------
	// Event Handler Functions
	//------------------------------------------------------------------------------------------------------------------

	private void OnSpeechInput(Data data) {
		SpeechRecognitionEvent result = ((SpeechToTextData)data).Results;
		if (result != null && result.results.Length > 0) {
			foreach (var res in result.results) {
				foreach (var alt in res.alternatives) {
					if (res.final && alt.confidence > 0.667) {
						string text = alt.transcript;
						Debug.Log ("Result: " + text + " Confidence: " + alt.confidence);
						m_Conversation.Message (OnMessage, m_WorkspaceID, text);
						microphone.DeactivateMicrophone ();
					} else {
						Debug.Log ("Confidence below threshold");
					}
				}
			}
		}
	}

	public void ActivateSearchArtist() {
		microphone.ActivateMicrophone ();
		m_Conversation.Message(OnMessage, m_WorkspaceID, "I'd like to hear some new music");
	}

	void OnMessage(object resp, string customData) {
		//  Convert resp to fsdata
		fsData fsdata = null;
		fsResult r = _serializer.TrySerialize (resp.GetType (), resp, out fsdata);
		if (!r.Succeeded)
			throw new WatsonException (r.FormattedMessages);

		//  Convert fsdata to MessageResponse
		MessageResponse messageResponse = new MessageResponse ();
		object obj = messageResponse;
		r = _serializer.TryDeserialize (fsdata, obj.GetType (), ref obj);
		if (!r.Succeeded)
			throw new WatsonException (r.FormattedMessages);
		if (resp != null && (messageResponse.intents.Length > 0 || messageResponse.entities.Length > 0)) {
			string[] values = messageResponse.output.text;
			foreach (string value in values) {
				Debug.Log ("response value: " + value);
			}

			Debug.Log ("intents count: " + messageResponse.intents.Length);

			string intent = messageResponse.intents [0].intent;
			Debug.Log ("Intent: " + intent);

			if (intent == "HeyNohm") {
				Debug.Log (messageResponse.output.text [0]);				
				string greeting = messageResponse.output.text [0];
				textToSpeech.ToSpeech (greeting, HandleToSpeechCallback);

				foreach (EntityResponse entity in messageResponse.entities) {
					Debug.Log ("entityType: " + entity.entity + " , value: " + entity.value);
					apiManager.artist = entity.value;
					apiManager.SearchTracksForArtist (entity.value);
					microphone.DeactivateMicrophone ();
				}

			} else {
				Debug.Log ("Different intent path)");
				string greeting = messageResponse.output.text [0];
				textToSpeech.ToSpeech (greeting, HandleToSpeechCallback);
				foreach (EntityResponse entity in messageResponse.entities) {
					Debug.Log ("entityType: " + entity.entity + " , value: " + entity.value);
					apiManager.artist = entity.value;
					apiManager.SearchTracksForArtist (entity.value);
					microphone.DeactivateMicrophone ();
				}
			}
		} else {
			Debug.Log ("else conditional log messages: " + messageResponse.output.log_messages [0]);
			Debug.Log ("else conditional input: " + messageResponse.input.text);
		}
	}
	#endregion
}
