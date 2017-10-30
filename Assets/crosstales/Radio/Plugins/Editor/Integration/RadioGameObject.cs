using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorIntegration
{
    /// <summary>Editor component for the "Hierarchy"-menu.</summary>
    public class RadioGameObject : MonoBehaviour
    {

        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/RadioPlayer", false, EditorHelper.GO_ID)]
        private static void AddRadioPlayer()
        {
            EditorHelper.InstantiatePrefab("RadioPlayer");
            GAApi.Event(typeof(RadioGameObject).Name, "Add RadioPlayer");
        }

		[MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/RadioProviderResource", false, EditorHelper.GO_ID + 1)]
		private static void AddRadioProviderResource()
		{
			EditorHelper.InstantiatePrefab("RadioProviderResource");
			GAApi.Event(typeof(RadioGameObject).Name, "Add RadioProviderResource");
		}

		[MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/RadioProviderShoutcast", false, EditorHelper.GO_ID + 2)]
		private static void AddRadioProviderShoutcast()
		{
			EditorHelper.InstantiatePrefab("RadioProviderShoutcast");
			GAApi.Event(typeof(RadioGameObject).Name, "Add RadioProviderShoutcast");
		}

		[MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/RadioProviderURL", false, EditorHelper.GO_ID + 3)]
		private static void AddRadioProviderURL()
		{
			EditorHelper.InstantiatePrefab("RadioProviderURL");
			GAApi.Event(typeof(RadioGameObject).Name, "Add RadioProviderURL");
		}

		[MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/RadioProviderUser", false, EditorHelper.GO_ID + 4)]
		private static void AddRadioProviderUser()
		{
			EditorHelper.InstantiatePrefab("RadioProviderUser");
			GAApi.Event(typeof(RadioGameObject).Name, "Add RadioProviderUser");
		}

        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/RadioManager", false, EditorHelper.GO_ID + 5)]
        private static void AddRadioManager()
        {
            EditorHelper.InstantiatePrefab("RadioManager");
            GAApi.Event(typeof(RadioGameObject).Name, "Add RadioManager");
        }

        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/SimplePlayer", false, EditorHelper.GO_ID + 6)]
        private static void AddSimplePlayer()
        {
            EditorHelper.InstantiatePrefab("SimplePlayer");
            GAApi.Event(typeof(RadioGameObject).Name, "Add SimplePlayer");
        }

        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/Loudspeaker", false, EditorHelper.GO_ID + 7)]
        private static void AddLoudspeaker()
        {
            EditorHelper.InstantiatePrefab("Loudspeaker");
            GAApi.Event(typeof(RadioGameObject).Name, "Add Loudspeaker");
        }

        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/StreamSaver", false, EditorHelper.GO_ID + 8)]
        private static void AddStreamSaver()
        {
            EditorHelper.InstantiatePrefab("StreamSaver");
            GAApi.Event(typeof(RadioGameObject).Name, "Add StreamSaver");
        }

        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/" + Util.Constants.SURVIVOR_SCENE_OBJECT_NAME, false, EditorHelper.GO_ID + 9)]
        private static void AddSurviveSceneSwitch()
        {
            EditorHelper.InstantiatePrefab(Util.Constants.SURVIVOR_SCENE_OBJECT_NAME);
            GAApi.Event(typeof(RadioGameObject).Name, "Add " + Util.Constants.SURVIVOR_SCENE_OBJECT_NAME);
        }

        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/" + Util.Constants.SURVIVOR_SCENE_OBJECT_NAME, true)]
        private static bool AddSurviveSceneSwitchValidator()
        {
            return !EditorHelper.isSurviveSceneSwitchInScene;
        }

        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/" + Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME, false, EditorHelper.GO_ID + 10)]
        private static void AddInternetCheck()
        {
            EditorHelper.InstantiatePrefab(Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME);
            GAApi.Event(typeof(RadioGameObject).Name, "Add " + Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME);
        }

        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/" + Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME, true)]
        private static bool AddInternetCheckValidator()
        {
            return !EditorHelper.isInternetCheckInScene;
        }

        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/" + Util.Constants.PROXY_SCENE_OBJECT_NAME, false, EditorHelper.GO_ID + 11)]
        private static void AddProxy()
        {
            EditorHelper.InstantiatePrefab(Util.Constants.PROXY_SCENE_OBJECT_NAME);
            GAApi.Event(typeof(RadioGameObject).Name, "Add " + Util.Constants.PROXY_SCENE_OBJECT_NAME);
        }

        [MenuItem("GameObject/" + Util.Constants.ASSET_NAME + "/" + Util.Constants.PROXY_SCENE_OBJECT_NAME, true)]
        private static bool AddProxyValidator()
        {
            return !EditorHelper.isProxyInScene;
        }
    }
}
// © 2017 crosstales LLC (https://www.crosstales.com)
