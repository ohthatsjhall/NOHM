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

public class VoiceSpawnerReconfig : Widget {

	public APIManager apiManager;
	public AudioClip sorryClip;
	public List<AudioClip> helpClips;
	public MicrophoneWidget microphone;

	private Conversation m_Conversation = new Conversation();
	private string m_WorkspaceID;
	private bool artistReturned = false;
	private bool m_UseAlternateIntents = true;
	public Context myContext = new Context();
	private int convoIndexTracker = 0;

	[SerializeField]
	private Input m_SpeechInput = new Input("SpeechInput", typeof(SpeechToTextData), "OnSpeechInput");
	private fsSerializer _serializer = new fsSerializer();

	TextToSpeech textToSpeech = new TextToSpeech();
	[HideInInspector]
	public const string welcomeString = "Welcome to Gnome, when you are ready to listen to music, press the app button";

	#region InitAndLifecycle
	//------------------------------------------------------------------------------------------------------------------
	// Initialization and Lifecycle
	//------------------------------------------------------------------------------------------------------------------


	protected override void Start() {
		base.Start();
		m_WorkspaceID = Config.Instance.GetVariableValue("ConversationV1_ID");
		//SendInitialMessage ("I'd like to hear something else");
		microphone.ActivateMicrophone();
	}

	private void OnSpeechInput(Data data) {
		SpeechRecognitionEvent result = ((SpeechToTextData)data).Results;
		if (result != null && result.results.Length > 0) {
			foreach (var res in result.results) {
				foreach (var alt in res.alternatives) {
					if (res.final && alt.confidence > 0.667) {
						string text = alt.transcript;
						Debug.Log ("Result: " + text + " Confidence: " + alt.confidence);
						if (convoIndexTracker == 0) {
							SendInitialMessage (text);
							//m_Conversation.Message (SendInitialMessage, m_WorkspaceID, text);
							microphone.DeactivateMicrophone ();
						} else if(convoIndexTracker == 1){
							MessageRequest messageRequest = new MessageRequest();
							messageRequest.InputText = text;
							messageRequest.alternate_intents = m_UseAlternateIntents;
							messageRequest.ContextData = myContext;

							//  Send the second message
							SendFollowupMessage(messageRequest);
							microphone.DeactivateMicrophone ();
						}

					} else {
						Debug.Log ("Confidence below threshold");
					}
				}
			}
		}
	}

	protected override string GetName()
	{
		return "VoiceSpawnerReconfig";
	}
	#endregion

	#region EventHandlers

	///////////////////////

	private void SendInitialMessage(string input)
	{
		if (string.IsNullOrEmpty (input))
			Debug.Log ("input was empty");

		//  Send initial message to the service
		m_Conversation.Message(OnInitialMessage, m_WorkspaceID, input);
	}



	private void OnInitialMessage(MessageResponse resp, string customData)
	{
		if (resp != null)
		{
			//  Check response here
			//  Convert resp to fsdata
			fsData fsdata = null;
			fsResult r = _serializer.TrySerialize (resp.GetType (), resp, out fsdata);
			if (!r.Succeeded)
				throw new WatsonException (r.FormattedMessages);
			
			MessageResponse messageResponse = new MessageResponse ();
			object obj = messageResponse;
			r = _serializer.TryDeserialize (fsdata, obj.GetType (), ref obj);
			if (!r.Succeeded)
				throw new WatsonException (r.FormattedMessages);
			if (resp != null && (messageResponse.intents.Length > 0 || messageResponse.entities.Length > 0)) {
				string[] values = messageResponse.output.text;
				foreach (string value in values) {
					Debug.Log ("response value: " + value);
					TextToSpeechWithString (value);
				}
			}
			//  Create a message request object with the context
			myContext = resp.context;  // Context of the conversation
			convoIndexTracker++;
			microphone.ActivateMicrophone ();
			//MessageRequest messageRequest = new MessageRequest();
			//messageRequest.InputText = "Primus";
			//messageRequest.alternate_intents = m_UseAlternateIntents;
			//messageRequest.ContextData = myContext;

			//  Send the second message
			//SendFollowupMessage(messageRequest);
		}
		else
		{
			Debug.Log("Message Only: Failed to invoke Message();");
		}
	}


	private void SendFollowupMessage(MessageRequest messageRequest)
	{
		if (messageRequest == null)
			Debug.Log ("message request was null");

		m_Conversation.Message(OnFollowupMessage, m_WorkspaceID, messageRequest);
	}


	private void OnFollowupMessage(MessageResponse resp, string customData)
	{
		if (resp != null)
		{
			foreach (Intent mi in resp.intents) {
				Debug.Log ("Full Request intent: " + mi.intent + ", confidence: " + mi.confidence);
			}

			if (resp.output != null && resp.output.text.Length > 0) {
				foreach (string txt in resp.output.text) {
					Debug.Log ("Full Request output: " + txt);
					TextToSpeechWithString (txt);
				}
			}
		}
		else
		{
			Debug.Log("Full Request: Failed to invoke Message();");
		}
	}

	public void TextToSpeechWithString(string text) {
		textToSpeech.Voice = VoiceType.en_GB_Kate;
		textToSpeech.ToSpeech (text, HandleToSpeechCallback);
	}

	void HandleToSpeechCallback (AudioClip clip) {
		PlayClip (clip);
	}

	private void PlayClip(AudioClip clip) {
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

	///////////////////

}
#endregion