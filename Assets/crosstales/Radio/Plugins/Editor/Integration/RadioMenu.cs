using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorIntegration
{
    /// <summary>Editor component for the "Tools"-menu.</summary>
    public class RadioMenu
    {
        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/RadioPlayer", false, EditorHelper.MENU_ID + 20)]
        private static void AddRadioPlayer()
        {
            EditorHelper.InstantiatePrefab("RadioPlayer");
            GAApi.Event(typeof(RadioMenu).Name, "Add RadioPlayer");
        }

		[MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/RadioProviderResource", false, EditorHelper.MENU_ID + 40)]
		private static void AddRadioProviderResource()
		{
			EditorHelper.InstantiatePrefab("RadioProviderResource");
			GAApi.Event(typeof(RadioMenu).Name, "Add RadioProviderResource");
		}

		[MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/RadioProviderShoutcast", false, EditorHelper.MENU_ID + 50)]
		private static void AddRadioProviderShoutcast()
		{
			EditorHelper.InstantiatePrefab("RadioProviderShoutcast");
			GAApi.Event(typeof(RadioMenu).Name, "Add RadioProviderShoutcast");
		}

		[MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/RadioProviderURL", false, EditorHelper.MENU_ID + 60)]
		private static void AddRadioProviderURL()
		{
			EditorHelper.InstantiatePrefab("RadioProviderURL");
			GAApi.Event(typeof(RadioMenu).Name, "Add RadioProviderURL");
		}

		[MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/RadioProviderUser", false, EditorHelper.MENU_ID + 70)]
		private static void AddRadioProviderUser()
		{
			EditorHelper.InstantiatePrefab("RadioProviderUser");
			GAApi.Event(typeof(RadioMenu).Name, "Add RadioProviderUser");
		}

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/RadioManager", false, EditorHelper.MENU_ID + 90)]
        private static void AddRadioManager()
        {
            EditorHelper.InstantiatePrefab("RadioManager");
            GAApi.Event(typeof(RadioMenu).Name, "Add RadioManager");
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/SimplePlayer", false, EditorHelper.MENU_ID + 110)]
        private static void AddSimplePlayer()
        {
            EditorHelper.InstantiatePrefab("SimplePlayer");
            GAApi.Event(typeof(RadioMenu).Name, "Add SimplePlayer");
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/Loudspeaker", false, EditorHelper.MENU_ID + 130)]
        private static void AddLoudspeaker()
        {
            EditorHelper.InstantiatePrefab("Loudspeaker");
            GAApi.Event(typeof(RadioMenu).Name, "Add Loudspeaker");
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/StreamSaver", false, EditorHelper.MENU_ID + 140)]
        private static void AddStreamSaver()
        {
            EditorHelper.InstantiatePrefab("StreamSaver");
            GAApi.Event(typeof(RadioMenu).Name, "Add StreamSaver");
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/" + Util.Constants.SURVIVOR_SCENE_OBJECT_NAME, false, EditorHelper.MENU_ID + 160)]
        private static void AddSurviveSceneSwitch()
        {
            EditorHelper.InstantiatePrefab(Util.Constants.SURVIVOR_SCENE_OBJECT_NAME);
            GAApi.Event(typeof(RadioMenu).Name, "Add " + Util.Constants.SURVIVOR_SCENE_OBJECT_NAME);
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/" + Util.Constants.SURVIVOR_SCENE_OBJECT_NAME, true)]
        private static bool AddSurviveSceneSwitchValidator()
        {
            return !EditorHelper.isSurviveSceneSwitchInScene;
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/" + Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME, false, EditorHelper.MENU_ID + 180)]
        private static void AddInternetCheck()
        {
            EditorHelper.InstantiatePrefab(Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME);
            GAApi.Event(typeof(RadioMenu).Name, "Add " + Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME);
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/" + Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME, true)]
        private static bool AddInternetCheckValidator()
        {
            return !EditorHelper.isInternetCheckInScene;
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/" + Util.Constants.PROXY_SCENE_OBJECT_NAME, false, EditorHelper.MENU_ID + 190)]
        private static void AddProxy()
        {
            EditorHelper.InstantiatePrefab(Util.Constants.PROXY_SCENE_OBJECT_NAME);
            GAApi.Event(typeof(RadioMenu).Name, "Add " + Util.Constants.PROXY_SCENE_OBJECT_NAME);
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Prefabs/" + Util.Constants.PROXY_SCENE_OBJECT_NAME, true)]
        private static bool AddProxyValidator()
        {
            return !EditorHelper.isProxyInScene;
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Help/Manual", false, EditorHelper.MENU_ID + 600)]
        private static void ShowManual()
        {
            Application.OpenURL(Util.Constants.ASSET_MANUAL_URL);
            GAApi.Event(typeof(RadioMenu).Name, "ASSET_MANUAL_URL");
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Help/API", false, EditorHelper.MENU_ID + 610)]
        private static void ShowAPI()
        {
            Application.OpenURL(Util.Constants.ASSET_API_URL);
            GAApi.Event(typeof(RadioMenu).Name, "ASSET_API_URL");
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Help/Forum", false, EditorHelper.MENU_ID + 620)]
        private static void ShowForum()
        {
            Application.OpenURL(Util.Constants.ASSET_FORUM_URL);
            GAApi.Event(typeof(RadioMenu).Name, "ASSET_FORUM_URL");
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Help/Product", false, EditorHelper.MENU_ID + 630)]
        private static void ShowProduct()
        {
            Application.OpenURL(Util.Constants.ASSET_WEB_URL);
            GAApi.Event(typeof(RadioMenu).Name, "ASSET_WEB_URL");
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Help/Videos/Promo", false, EditorHelper.MENU_ID + 650)]
        private static void ShowVideoPromo()
        {
            Application.OpenURL(Util.Constants.ASSET_VIDEO_PROMO);
            GAApi.Event(typeof(RadioMenu).Name, "ASSET_VIDEO_PROMO");
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Help/Videos/Tutorial", false, EditorHelper.MENU_ID + 660)]
        private static void ShowVideoTutorial()
        {
            Application.OpenURL(Util.Constants.ASSET_VIDEO_TUTORIAL);
            GAApi.Event(typeof(RadioMenu).Name, "ASSET_VIDEO_TUTORIAL");
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Help/Videos/All Videos", false, EditorHelper.MENU_ID + 680)]
        private static void ShowAllVideos()
        {
            Application.OpenURL(Util.Constants.ASSET_SOCIAL_YOUTUBE);
            GAApi.Event(typeof(RadioMenu).Name, "ASSET_SOCIAL_YOUTUBE");
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Help/3rd Party Assets", false, EditorHelper.MENU_ID + 700)]
        private static void Show3rdPartyAV()
        {
            Application.OpenURL(Util.Constants.ASSET_3P_URL);
            GAApi.Event(typeof(RadioMenu).Name, "ASSET_3P_URL");
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/About/Unity AssetStore", false, EditorHelper.MENU_ID + 800)]
        private static void ShowUAS()
        {
            Application.OpenURL(Util.Constants.ASSET_CT_URL);
            GAApi.Event(typeof(RadioMenu).Name, "ASSET_CT_URL");
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/About/" + Util.Constants.ASSET_AUTHOR, false, EditorHelper.MENU_ID + 820)]
        private static void ShowCT()
        {
            Application.OpenURL(Util.Constants.ASSET_AUTHOR_URL);
            GAApi.Event(typeof(RadioMenu).Name, "ASSET_AUTHOR_URL");
        }

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/About/Info", false, EditorHelper.MENU_ID + 840)]
        private static void ShowInfo()
        {
            EditorUtility.DisplayDialog(Util.Constants.ASSET_NAME + " - About",
                "Version: " + Util.Constants.ASSET_VERSION +
                System.Environment.NewLine +
                System.Environment.NewLine +
                "© 2015-2017 by " + Util.Constants.ASSET_AUTHOR +
                System.Environment.NewLine +
                System.Environment.NewLine +
                Util.Constants.ASSET_AUTHOR_URL +
                System.Environment.NewLine, "Ok");

            GAApi.Event(typeof(RadioMenu).Name, "Info");
        }
    }
}
// © 2015-2017 crosstales LLC (https://www.crosstales.com)