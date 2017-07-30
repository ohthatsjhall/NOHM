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
	private Animator pointLightAnimator;

	[SerializeField]
	private Input m_SpeechInput = new Input("SpeechInput", typeof(SpeechToTextData), "OnSpeechInput");
	private fsSerializer _serializer = new fsSerializer();

	TextToSpeech textToSpeech = new TextToSpeech();

	#region InitAndLifecycle
	//------------------------------------------------------------------------------------------------------------------
	// Initialization and Lifecycle
	//------------------------------------------------------------------------------------------------------------------

	protected override void Start() {
		base.Start();
		pointLightAnimator = onboardingManager.pointLight.GetComponent<Animator> ();
		textToSpeech.Voice = VoiceType.en_GB_Kate;
		//textToSpeech.ToSpeech (welcomeString, HandleToSpeechCallback);
		m_WorkspaceID = Config.Instance.GetVariableValue("ConversationV1_ID");
		m_Conversation.Message (OnMessage, m_WorkspaceID, "LastMoonOnboardingForNohm");
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
						string resultText = text.ToLower ();
						if ((resultText.Contains ("frank")) || (resultText.Contains ("ocean"))) {
							m_Conversation.Message (OnMessage, m_WorkspaceID, text);
							currentStep = 2;
							ClearCanvas ();
						} else {
							// Error Handling to make the user say 
							Debug.Log ("didnt say Frank Ocean");
							textToSpeech.ToSpeech ("Say Frank Ocean you silly ass donk", HandleToSpeechCallback);
						}
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

			if (intent == "LastMoonOnboardingForNohm" && currentStep == 0) {
				
				StartCoroutine (DelayMethod (5.0f, values));
				currentStep++;

			} else if (intent == "HeyNohm" && currentStep == 1) {

				StartCoroutine (DelayMethod (0.0f, values));

			} else if (intent == "OnboardingGrabRecord" && currentStep == 2) {
				onboardingManager.rightController.GetComponent<OnboardingRightControllerListener> ().enabled = false;
				onboardingManager.microphone.DeactivateMicrophone ();
				onboardingManager.records.GetComponent<OnboardingRecords> ().AnimateRecords (currentStep == 2);
				pointLightAnimator.SetInteger ("Stage", 1);
				StartCoroutine (DelayMethod (8.0f, values));
				currentStep++;
			} else if (intent == "OnboardingClose" && currentStep == 3){
				pointLightAnimator.SetInteger ("Stage", 2);
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
					//onboardingManager.microphone.ActivateMicrophone ();
					onboardingManager.rightController.GetComponent<OnboardingRightControllerListener>().enabled = true;
				}	

			} else if (currentStep == 3) {
				onboardingManager.recordPlayer.SetActive (true);

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
		
	private void LoadLevelWithSteam() {
		VRTK_SDKManager.instance.UnloadSDKSetup ();
		SteamVR_LoadLevel.Begin ("Vinyl");
	}

	public void OnboardingTriggerPressed() {
		onboardingManager.microphone.ActivateMicrophone ();
		m_Conversation.Message(OnMessage, m_WorkspaceID, "Hey Nohm");
	}

	private void ClearCanvas() {
		onboardingManager.worldSpaceCanvas.GetComponentInChildren<Text> ().text = "";
	}

}
	
	#endregion

