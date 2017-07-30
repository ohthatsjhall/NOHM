using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using IBM.Watson.DeveloperCloud.Widgets;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using VRTK;

using FullSerializer;

#pragma warning disable 414

public class OnboardingVoiceSpawner : Widget {

	public OnboardingManager onboardingManager;
	public AudioClip sorryClip;
	public List<AudioClip> helpClips;

	private Conversation m_Conversation = new Conversation();
	private string m_WorkspaceID;
	private bool artistReturned = false;
	private int currentStep = 0;

	private VRTK_ControllerHighlighter highligher;
	private Color highlightColor = Color.yellow;
	private Color pulseColor = Color.black;
	private Color currentPulseColor;
	private float pulseTimer = 0.75f;

	[SerializeField]
	private Input m_SpeechInput = new Input("SpeechInput", typeof(SpeechToTextData), "OnSpeechInput");
	private fsSerializer _serializer = new fsSerializer();

	TextToSpeech textToSpeech = new TextToSpeech();
	//private string welcomeString = "Welcome to Gnome, when you are ready to listen to music, just say, hey gnome";

	#region InitAndLifecycle
	//------------------------------------------------------------------------------------------------------------------
	// Initialization and Lifecycle
	//------------------------------------------------------------------------------------------------------------------

	protected override void Start() {
		base.Start();
		textToSpeech.Voice = VoiceType.en_GB_Kate;
		//textToSpeech.ToSpeech (welcomeString, HandleToSpeechCallback);
		m_WorkspaceID = Config.Instance.GetVariableValue("ConversationV1_ID");
		m_Conversation.Message (OnMessage, m_WorkspaceID, "Onboarding");
	}

	private void Update(){
		if (currentStep == 2) {
			m_Conversation.Message (OnMessage, m_WorkspaceID, "OnboardingGrabRecord");
		} else if (SoundSystem.Instance.GetComponent<AudioSource>().isPlaying && currentStep == 3) {
			m_Conversation.Message (OnMessage, m_WorkspaceID, "OnboardingClose");
		}
	}

	void HandleToSpeechCallback (AudioClip clip) {
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
			source.PlayOneShot(clip);

			GameObject.Destroy(audioObject, clip.length);
		}
	}

	protected override string GetName()
	{
		return "OnboardingVoiceSpawner";
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

						Debug.Log("Result: " + text + " Confidence: " + alt.confidence);
						m_Conversation.Message(OnMessage, m_WorkspaceID, text);
					}
				}
			}
		}
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
			string intent = messageResponse.intents [0].intent;
			Debug.Log ("Intent: " + intent);

			if (intent == "Onboarding" && currentStep == 0) {
				
				StartCoroutine (DelayMethod (5.0f, values));
				currentStep++;

			} else if (intent == "HeyNohm" && currentStep == 1) {

				StartCoroutine (DelayMethod (0.0f, values));
				currentStep++;
				onboardingManager.records.GetComponent<OnboardingRecords> ().AnimateRecords (currentStep == 2);
			} else if (intent == "OnboardingGrabRecord" && currentStep == 2) {
				StartCoroutine (DelayMethod (8.0f, values));
				currentStep++;
			} else if (intent == "OnboardingClose" && currentStep == 3){

				currentStep++;
				StartCoroutine(DelayMethod(20.0f, values));
			}

		} else {
			Debug.Log ("Failed to invoke OnMessage();");
		}
	}

	IEnumerator DelayMethod(float delay, string[] values) {
		for (var i = 0; i < values.Length; i++) {
			yield return new WaitForSeconds (delay);
			if (currentStep == 1) {
				if (i == values.Length - 1) {
					onboardingManager.microphone.ActivateMicrophone();
				}

			} else if (currentStep == 3) {
				onboardingManager.recordPlayer.SetActive (true);
				onboardingManager.pointLight.range = 19.9f;
				onboardingManager.leftController.GetComponent<OnboardingTooltips> ().enabled = true;
				onboardingManager.rightController.GetComponent<OnboardingTooltips> ().enabled = true;
				// animate point light
			} else {
				onboardingManager.leftController.GetComponent<OnboardingTooltips> ().enabled = false;
				onboardingManager.rightController.GetComponent<OnboardingTooltips> ().enabled = false;
			}
			textToSpeech.ToSpeech (values[i], HandleToSpeechCallback);
			if (currentStep == 4) {
				LoadLevelWithSteam ();
			}
			Debug.Log ("value post" + values[i]);
			onboardingManager.worldSpaceCanvas.GetComponentInChildren<Text> ().text = values[i];
		}
	}

	private void TransitionScene() {
		LoadingOverlay overlay = GameObject.Find ("LoadingOverlay").gameObject.GetComponent<LoadingOverlay> ();
		overlay.FadeOut ();
		StartCoroutine (LoadVinylSceneAsync());
	}

	IEnumerator LoadVinylSceneAsync() {
		VRTK.VRTK_SDKManager.instance.UnloadSDKSetup ();
		AsyncOperation async = SceneManager.LoadSceneAsync (1);
		yield return async;
		Debug.Log("Loading complete");
	}

	private void LoadNextScene() {
		//VRTK.VRTK_SDKManager.instance.UnloadSDKSetup ();
		int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
		SceneManager.LoadScene (nextSceneIndex, LoadSceneMode.Single);
	}

	private void LoadLevelWithSteam() {
		VRTK.VRTK_SDKManager.instance.UnloadSDKSetup ();
		SteamVR_LoadLevel.Begin ("Vinyl");
	}

}
	
	#endregion

