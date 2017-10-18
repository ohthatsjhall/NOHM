using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System.Collections;
using FullSerializer;
using System.Collections.Generic;
using BestHTTP;
using System.Text;

public class ConversationManager : MonoBehaviour {

	private string _username = "e69f1eea-2731-48eb-accc-6113ffe4d420";
	private string _password = "6LxxkQxkXOcU";
	private string _url = "https://gateway.watsonplatform.net/conversation/api";
	private string _workspaceId = "caa0c1b5-e0e4-4873-aed8-970092de70e3";

	private Conversation _conversation;
	private string _conversationVersionDate = "2017-05-26";

	TextToSpeechManager _textToSpeech;
	NohmWatsonManager _nohmWatsonManager;

	// private string[] _questionArray = { "I'd like to hear new music", "Frank Ocean"};
	// private string[] _questionArray;
	List<string> newQuestionArray = new List<string>();
	private fsSerializer _serializer = new fsSerializer();
	private Dictionary<string, object> _context = null;
	private int _questionCount = -1;
	private bool _waitingForResponse = true;

	void Start()
	{
		LogSystem.InstallDefaultReactors();

		//  Create credential and instantiate service
		Credentials credentials = new Credentials(_username, _password, _url);

		_conversation = new Conversation(credentials);
		_conversation.VersionDate = _conversationVersionDate;
		_nohmWatsonManager = GetComponent<NohmWatsonManager>();

		//        Runnable.Run(Examples());
	}

	private IEnumerator Examples()
	{
		if (!_conversation.Message(OnMessage, _workspaceId, "Hello"))
			Log.Debug("ExampleConversation", "Failed to message!");

		while (_waitingForResponse)
			yield return null;

		_waitingForResponse = true;
		_questionCount++;

		AskQuestion();
		while (_waitingForResponse)
			yield return null;

		_questionCount++;

		_waitingForResponse = true;

		AskQuestion();
		while (_waitingForResponse)
			yield return null;
		_questionCount++;

		_waitingForResponse = false;

		//        AskQuestion();
		//        while (_waitingForResponse)
		//            yield return null;
		//        _questionCount++;
		//
		//        _waitingForResponse = true;
		//
		//        AskQuestion();
		//        while (_waitingForResponse)
		//            yield return null;

		Log.Debug("ExampleConversation", "Conversation examples complete.");
	}

	public void AskQuestion()
	{
		Debug.Log("newQuestionArray first index: " + newQuestionArray[0]);

		MessageRequest messageRequest = new MessageRequest()
		{

			input = new Dictionary<string, object>()
			{
				{ "text", newQuestionArray[0] }
			},
			context = _context
		};

		// _conversation.Message (OnMessage, _workspaceId, messageRequest);

		newQuestionArray.Clear ();

		if (!_conversation.Message(OnMessage, _workspaceId, messageRequest))
			Log.Debug("ExampleConversation", "Failed to message!");
	}

	private void OnMessage (object resp, string data)
	{
		Log.Debug ("ExampleConversation", "Conversation: Message Response: {0}", data);

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

		Debug.Log ("TRIGGERERDDDDD");

		if (resp != null ) {
			string[] values = messageResponse.output.text;
			foreach (string value in values) {
				Debug.Log ("response value: " + value);

				bool isFinal = (bool)messageResponse.context ["isFinal"];
				if (isFinal) {
					if (messageResponse.entities.Length > 0) {
						foreach (var entity in messageResponse.entities) {
							Debug.Log ("entity type: " + entity + "\nvalue: " + entity.value);
							string artist = entity.value;
							Debug.Log ("artist: " + artist);
							_nohmWatsonManager.SearchForArtist (artist);
						}
					}
					_nohmWatsonManager.StopRecording ();
					messageResponse.context.Clear ();
				}	

				if (messageResponse.intents.Length > 0) {
					foreach (var intent in messageResponse.intents) {
						string intentValue = intent.intent;
						if (intentValue == "Confirmation" && isFinal) {
							string unknownArtist = (string)messageResponse.context ["artistSearch"];
							AddUnknownArtistToEntity (unknownArtist, NohmConstants.AddArtistURL);
						}
					}
				}

				_nohmWatsonManager.SayString (value);
			}
		}

		//  Set context for next round of messaging
		object _tempContext = null;
		(resp as Dictionary<string, object>).TryGetValue("context", out _tempContext);

		if (_tempContext != null)
			_context = _tempContext as Dictionary<string, object>;
		else
			Log.Debug("ExampleConversation", "Failed to get context");
		_waitingForResponse = false;
	}

	public void SetQuestions (string questionString) {
		newQuestionArray.Add(questionString);
		AskQuestion();
	}

	private void AddUnknownArtistToEntity(string unknownArtist, string url) {

		string requestJson = "{{\"metadata\": {{\"property\": \"{0}\"}}}}";
		string requestString = string.Format (requestJson, unknownArtist);

		HTTPRequest request = new HTTPRequest (new System.Uri (url), AddArtistCallback);
		request.MethodType = HTTPMethods.Post;
		request.AddHeader ("Content-Type", "application/json");
		request.AddHeader ("Authorization", "Basic ZTY5ZjFlZWEtMjczMS00OGViLWFjY2MtNjExM2ZmZTRkNDIwOjZMeHhrUXhrWE9jVQ==");
		request.RawData = Encoding.UTF8.GetBytes (requestString);
		request.Send ();
		//request.AddField ("entity", "UnknownArtists");
		//request.AddField ("metadata", "");
	}

	private void AddArtistCallback(HTTPRequest request, HTTPResponse response) {
		Debug.Log ("response: " + response.DataAsText);
	}

}