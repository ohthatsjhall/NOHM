using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;
using Crosstales.Radio.EditorTask;

namespace Crosstales.Radio.EditorIntegration
{
    /// <summary>Base class for editor windows.</summary>
    public abstract class ConfigBase : EditorWindow
    {
        #region Variables

        private static string updateText = UpdateCheck.TEXT_NOT_CHECKED;

        private System.Threading.Thread worker;

        private Vector2 scrollPosConfig;
        private Vector2 scrollPosHelp;
        private Vector2 scrollPosAboutUpdate;
        private Vector2 scrollPosAboutReadme;
        private Vector2 scrollPosAboutVersions;

        private static string readme;
        private static string versions;

        private int aboutTab = 0;

        #endregion


        #region Protected methods

        protected void showConfiguration()
        {
            GUI.skin.label.wordWrap = true;

            scrollPosConfig = EditorGUILayout.BeginScrollView(scrollPosConfig, false, false);
            {
                GUILayout.Label("General Settings", EditorStyles.boldLabel);

                Util.Config.ASSET_PATH = EditorGUILayout.TextField(new GUIContent("Asset Path", "Path to the asset inside the Unity-project (default: " + Util.Constants.DEFAULT_ASSET_PATH + ")."), Util.Config.ASSET_PATH);

                Util.Config.DEBUG = EditorGUILayout.Toggle(new GUIContent("Debug", "Enable or disable debug logs (default: " + Util.Constants.DEFAULT_DEBUG + ")"), Util.Config.DEBUG);

                Util.Config.UPDATE_CHECK = EditorGUILayout.BeginToggleGroup(new GUIContent("Update Check", "Enable or disable the update-check (default: " + Util.Constants.DEFAULT_UPDATE_CHECK + ")."), Util.Config.UPDATE_CHECK);
                {
                    EditorGUI.indentLevel++;
                    Util.Config.UPDATE_OPEN_UAS = EditorGUILayout.Toggle(new GUIContent("Open UAS-Site", "Automatically opens the direct link to 'Unity AssetStore' if an update was found (default: " + Util.Constants.DEFAULT_UPDATE_OPEN_UAS + ")."), Util.Config.UPDATE_OPEN_UAS);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndToggleGroup();

                Util.Config.REMINDER_CHECK = EditorGUILayout.Toggle(new GUIContent("Reminder Check", "Enable or disable the reminder-check (default: " + Util.Constants.DEFAULT_REMINDER_CHECK + ")"), Util.Config.REMINDER_CHECK);

                Util.Config.TELEMETRY = EditorGUILayout.Toggle(new GUIContent("Telemetry", "Enable or disable anonymous telemetry data (default: " + Util.Constants.DEFAULT_TELEMETRY + ")"), Util.Config.TELEMETRY);

                Util.Config.PREFAB_AUTOLOAD = EditorGUILayout.Toggle(new GUIContent("Prefab Auto-Load", "Enable or disable auto-loading of the prefabs to the scene (default: " + Util.Constants.DEFAULT_PREFAB_AUTOLOAD + ")."), Util.Config.PREFAB_AUTOLOAD);

                EditorHelper.SeparatorUI();
                GUILayout.Label("UI Settings", EditorStyles.boldLabel);
                Util.Config.HIERARCHY_ICON = EditorGUILayout.Toggle(new GUIContent("Show Hierarchy Icon", "Show hierarchy icon (default: " + Util.Constants.DEFAULT_HIERARCHY_ICON + ")."), Util.Config.HIERARCHY_ICON);

                EditorHelper.SeparatorUI();
                GUILayout.Label("Default Audio Settings", EditorStyles.boldLabel);

                Util.Config.DEFAULT_BITRATE = EditorGUILayout.IntField(new GUIContent("Bitrate", "Default bitrate in kbit/s (default: " + Util.Constants.DEFAULT_DEFAULT_BITRATE + ")."), Util.Config.DEFAULT_BITRATE);
                Util.Config.DEFAULT_CHUNKSIZE = EditorGUILayout.IntField(new GUIContent("Chunk Size", "Default size of the streaming-chunk in KB (default: " + Util.Constants.DEFAULT_DEFAULT_CHUNKSIZE + ")."), Util.Config.DEFAULT_CHUNKSIZE);
                Util.Config.DEFAULT_BUFFERSIZE = EditorGUILayout.IntField(new GUIContent("Buffer Size", "Default size of the local buffer in KB (default: " + Util.Constants.DEFAULT_DEFAULT_BUFFERSIZE + ")."), Util.Config.DEFAULT_BUFFERSIZE);
                Util.Config.DEFAULT_CACHESTREAMSIZE = EditorGUILayout.IntField(new GUIContent("Cache Stream Size", "Default size of the buffer in KB (default: " + Util.Constants.DEFAULT_DEFAULT_CACHESTREAMSIZE + ")."), Util.Config.DEFAULT_CACHESTREAMSIZE);
            }
            EditorGUILayout.EndScrollView();

            validate();
        }

        protected void showHelp()
        {
            scrollPosHelp = EditorGUILayout.BeginScrollView(scrollPosHelp, false, false);
            {
                GUILayout.Label("Resources", EditorStyles.boldLabel);

                //GUILayout.Space(8);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.BeginVertical();
                    {
                        if (GUILayout.Button(new GUIContent(" Manual", EditorHelper.Icon_Manual, "Show the manual.")))
                        {
                            Application.OpenURL(Util.Constants.ASSET_MANUAL_URL);
                            GAApi.Event(typeof(ConfigBase).Name, "ASSET_MANUAL_URL");
                        }

                        GUILayout.Space(6);

                        if (GUILayout.Button(new GUIContent(" Forum", EditorHelper.Icon_Forum, "Visit the forum page.")))
                        {
                            Application.OpenURL(Util.Constants.ASSET_FORUM_URL);
                            GAApi.Event(typeof(ConfigBase).Name, "ASSET_FORUM_URL");
                        }
                    }
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();
                    {

                        if (GUILayout.Button(new GUIContent(" API", EditorHelper.Icon_API, "Show the API.")))
                        {
                            Application.OpenURL(Util.Constants.ASSET_API_URL);
                            GAApi.Event(typeof(ConfigBase).Name, "ASSET_API_URL");
                        }

                        GUILayout.Space(6);

                        if (GUILayout.Button(new GUIContent(" Product", EditorHelper.Icon_Product, "Visit the product page.")))
                        {
                            Application.OpenURL(Util.Constants.ASSET_WEB_URL);
                            GAApi.Event(typeof(ConfigBase).Name, "ASSET_WEB_URL");
                        }
                    }
                    GUILayout.EndVertical();

                }
                GUILayout.EndHorizontal();

                EditorHelper.SeparatorUI();

                GUILayout.Label("Videos", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(new GUIContent(" Promo", EditorHelper.Video_Promo, "View the promotion video on 'Youtube'.")))
                    {
                        Application.OpenURL(Util.Constants.ASSET_VIDEO_PROMO);
                        GAApi.Event(typeof(ConfigBase).Name, "ASSET_VIDEO_PROMO");
                    }

                    if (GUILayout.Button(new GUIContent(" Tutorial", EditorHelper.Video_Tutorial, "View the tutorial video on 'Youtube'.")))
                    {
                        Application.OpenURL(Util.Constants.ASSET_VIDEO_TUTORIAL);
                        GAApi.Event(typeof(ConfigBase).Name, "ASSET_VIDEO_TUTORIAL");
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(6);

                if (GUILayout.Button(new GUIContent(" All Videos", EditorHelper.Icon_Videos, "Visit our 'Youtube'-channel for more videos.")))
                {
                    Application.OpenURL(Util.Constants.ASSET_SOCIAL_YOUTUBE);
                    GAApi.Event(typeof(ConfigBase).Name, "ASSET_SOCIAL_YOUTUBE");
                }

                EditorHelper.SeparatorUI();

                GUILayout.Label("3rd Party Assets", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_PlayMaker, "More information about 'PlayMaker'.")))
                    {
                        Application.OpenURL(Util.Constants.ASSET_3P_PLAYMAKER);
                        GAApi.Event(typeof(ConfigBase).Name, "ASSET_3P_PLAYMAKER");
                    }

                    if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_CompleteSoundSuite, "More information about 'Complete Sound Suite'.")))
                    {
                        Application.OpenURL(Util.Constants.ASSET_3P_SOUND_SUITE);
                        GAApi.Event(typeof(ConfigBase).Name, "ASSET_3P_SOUND_SUITE");
                    }

                    if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_AudioVisualizer, "More information about 'Audio Visualizer'.")))
                    {
                        Application.OpenURL(Util.Constants.ASSET_3P_AUDIO_VISUALIZER);
                        GAApi.Event(typeof(ConfigBase).Name, "ASSET_3P_AUDIO_VISUALIZER");
                    }

                    if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_VisualizerStudio, "More information about 'Visualizer Studio'.")))
                    {
                        Application.OpenURL(Util.Constants.ASSET_3P_VISUALIZER_STUDIO);
                        GAApi.Event(typeof(ConfigBase).Name, "ASSET_3P_VISUALIZER_STUDIO");
                    }
                }
                GUILayout.EndHorizontal();


                GUILayout.Space(6);

                if (GUILayout.Button(new GUIContent(" All Supported Assets", EditorHelper.Icon_3p_Assets, "More information about the all supported assets.")))
                {
                    Application.OpenURL(Util.Constants.ASSET_3P_URL);
                    GAApi.Event(typeof(ConfigBase).Name, "ASSET_3P_URL");
                }
            }
            EditorGUILayout.EndScrollView();

            GUILayout.Space(6);
        }

        protected void showAbout()
        {
            GUILayout.Label(Util.Constants.ASSET_NAME, EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical(GUILayout.Width(60));
                {
                    GUILayout.Label("Version:");

                    GUILayout.Space(12);

                    GUILayout.Label("Web:");

                    GUILayout.Space(2);

                    GUILayout.Label("Email:");

                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.Width(170));
                {
                    GUILayout.Space(0);

                    GUILayout.Label(Util.Constants.ASSET_VERSION);

                    GUILayout.Space(12);

                    EditorGUILayout.SelectableLabel(Util.Constants.ASSET_AUTHOR_URL, GUILayout.Height(16), GUILayout.ExpandHeight(false));

                    GUILayout.Space(2);

                    EditorGUILayout.SelectableLabel(Util.Constants.ASSET_CONTACT, GUILayout.Height(16), GUILayout.ExpandHeight(false));
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                {
                    //GUILayout.Space(0);
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical(GUILayout.Width(64));
                {
                    if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset, "Visit asset website")))
                    {
                        Application.OpenURL(Util.Constants.ASSET_URL);
                        GAApi.Event(typeof(ConfigBase).Name, "ASSET_URL");
                    }

                    if (!Util.Constants.isPro)
                    {
                        if (GUILayout.Button(new GUIContent(" Upgrade", "Upgrade " + Util.Constants.ASSET_NAME + " to the PRO-version")))
                        {
                            Application.OpenURL(Util.Constants.ASSET_PRO_URL);
                            GAApi.Event(typeof(ConfigBase).Name, "ASSET_PRO_URL");
                        }
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("© 2015-2017 by " + Util.Constants.ASSET_AUTHOR);

            EditorHelper.SeparatorUI();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent(" AssetStore", EditorHelper.Logo_Unity, "Visit the 'Unity AssetStore' website.")))
                {
                    Application.OpenURL(Util.Constants.ASSET_CT_URL);
                    GAApi.Event(typeof(ConfigBase).Name, "ASSET_URL");
                }

                if (GUILayout.Button(new GUIContent(" " + Util.Constants.ASSET_AUTHOR, EditorHelper.Logo_CT, "Visit the '" + Util.Constants.ASSET_AUTHOR + "' website.")))
                {
                    Application.OpenURL(Util.Constants.ASSET_AUTHOR_URL);
                    GAApi.Event(typeof(ConfigBase).Name, "ASSET_AUTHOR_URL");
                }
            }
            GUILayout.EndHorizontal();

            EditorHelper.SeparatorUI();

            aboutTab = GUILayout.Toolbar(aboutTab, new string[] { "Readme", "Versions", "Update" });

            if (aboutTab == 2)
            {
                scrollPosAboutUpdate = EditorGUILayout.BeginScrollView(scrollPosAboutUpdate, false, false);
                {
                    Color fgColor = GUI.color;

                    GUI.color = Color.yellow;

                    if (UpdateCheck.Status == UpdateStatus.NO_UPDATE)
                    {
                        GUI.color = Color.green;
                        GUILayout.Label(updateText);
                    }
                    else if (UpdateCheck.Status == UpdateStatus.UPDATE)
                    {
                        GUILayout.Label(updateText);

                        if (GUILayout.Button(new GUIContent(" Download", "Visit the 'Unity AssetStore' to download the latest version.")))
                        {
                            Application.OpenURL(Util.Constants.ASSET_URL);
                            GAApi.Event(typeof(ConfigBase).Name, "UPDATE");
                        }
                    }
                    else if (UpdateCheck.Status == UpdateStatus.UPDATE_PRO)
                    {
                        GUILayout.Label(updateText);

                        if (GUILayout.Button(new GUIContent(" Upgrade", "Upgrade to the PRO-version in the 'Unity AssetStore'.")))
                        {
                            Application.OpenURL(Util.Constants.ASSET_PRO_URL);
                            GAApi.Event(typeof(ConfigBase).Name, "UPGRADE PRO");
                        }
                    }
                    else if (UpdateCheck.Status == UpdateStatus.UPDATE_VERSION)
                    {
                        GUILayout.Label(updateText);

                        if (GUILayout.Button(new GUIContent(" Upgrade", "Upgrade to the newer version in the 'Unity AssetStore'")))
                        {
                            Application.OpenURL(Util.Constants.ASSET_CT_URL);
                            GAApi.Event(typeof(ConfigBase).Name, "UPGRADE");
                        }
                    }
                    else if (UpdateCheck.Status == UpdateStatus.DEPRECATED)
                    {
                        GUILayout.Label(updateText);

                        if (GUILayout.Button(new GUIContent(" More Information", "Visit the 'crosstales'-site for more information.")))
                        {
                            Application.OpenURL(Util.Constants.ASSET_AUTHOR_URL);
                            GAApi.Event(typeof(ConfigBase).Name, "DEPRECATED");
                        }
                    }
                    else
                    {
                        GUI.color = Color.cyan;
                        GUILayout.Label(updateText);
                    }

                    GUI.color = fgColor;
                }
                EditorGUILayout.EndScrollView();

                if (UpdateCheck.Status == UpdateStatus.NOT_CHECKED || UpdateCheck.Status == UpdateStatus.NO_UPDATE)
                {
                    bool isChecking = !(worker == null || (worker != null && !worker.IsAlive));

                    GUI.enabled = Tool.InternetCheck.isInternetAvailable && !isChecking;

                    if (GUILayout.Button(new GUIContent(isChecking ? "Checking... Please wait." : " Check For Update", EditorHelper.Icon_Check, "Checks for available updates of " + Util.Constants.ASSET_NAME)))
                    {
                        worker = new System.Threading.Thread(() => UpdateCheck.UpdateCheckForEditor(out updateText));
                        worker.Start();

                        GAApi.Event(typeof(ConfigBase).Name, "UpdateCheck");
                    }

                    GUI.enabled = true;
                }
            }
            else if (aboutTab == 0)
            {
                if (readme == null)
                {
                    string path = Application.dataPath + Util.Config.ASSET_PATH + "README.txt";

                    try
                    {
                        readme = System.IO.File.ReadAllText(path);
                    }
                    catch (System.Exception)
                    {
                        readme = "README not found: " + path;
                    }
                }

                scrollPosAboutReadme = EditorGUILayout.BeginScrollView(scrollPosAboutReadme, false, false);
                {
                    GUILayout.Label(readme);
                }
                EditorGUILayout.EndScrollView();
            }
            else
            {
                if (versions == null)
                {
                    string path = Application.dataPath + Util.Config.ASSET_PATH + "/Documentation/VERSIONS.txt";

                    try
                    {
                        versions = System.IO.File.ReadAllText(path);
                    }
                    catch (System.Exception)
                    {
                        versions = "VERSIONS not found: " + path;
                    }
                }

                scrollPosAboutVersions = EditorGUILayout.BeginScrollView(scrollPosAboutVersions, false, false);
                {
                    GUILayout.Label(versions);
                }

                EditorGUILayout.EndScrollView();
            }

            EditorHelper.SeparatorUI();

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Social_Facebook, "Follow us on 'Facebook'.")))
                {
                    Application.OpenURL(Util.Constants.ASSET_SOCIAL_FACEBOOK);
                    GAApi.Event(typeof(ConfigBase).Name, "ASSET_SOCIAL_FACEBOOK");
                }

                if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Social_Twitter, "Follow us on 'Twitter'.")))
                {
                    Application.OpenURL(Util.Constants.ASSET_SOCIAL_TWITTER);
                    GAApi.Event(typeof(ConfigBase).Name, "ASSET_SOCIAL_TWITTER");
                }

                if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Social_Linkedin, "Follow us on 'LinkedIn'.")))
                {
                    Application.OpenURL(Util.Constants.ASSET_SOCIAL_LINKEDIN);
                    GAApi.Event(typeof(ConfigBase).Name, "ASSET_SOCIAL_LINKEDIN");
                }

                if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Social_Xing, "Follow us on 'XING'.")))
                {
                    Application.OpenURL(Util.Constants.ASSET_SOCIAL_XING);
                    GAApi.Event(typeof(ConfigBase).Name, "ASSET_SOCIAL_XING");
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6);
        }

        protected static void save()
        {
            Util.Config.Save();

            if (Util.Config.DEBUG)
                Debug.Log("Config data saved");
        }

        #endregion


        #region Private methods

        private static void validate()
        {
            if (Util.Config.DEFAULT_BITRATE <= 0)
            {
                Util.Config.DEFAULT_BITRATE = Util.Constants.DEFAULT_DEFAULT_BITRATE;
            }
            else
            {
                Util.Config.DEFAULT_BITRATE = Util.Helper.NearestMP3Bitrate(Util.Config.DEFAULT_BITRATE); //not ideal, but ok
            }

            if (Util.Config.DEFAULT_CHUNKSIZE <= 0)
            {
                Util.Config.DEFAULT_CHUNKSIZE = Util.Constants.DEFAULT_DEFAULT_CHUNKSIZE;
            }
            else if (Util.Config.DEFAULT_CHUNKSIZE > Util.Config.MAX_CACHESTREAMSIZE)
            {
                Util.Config.DEFAULT_CHUNKSIZE = Util.Config.MAX_CACHESTREAMSIZE;
            }

            if (Util.Config.DEFAULT_BUFFERSIZE <= 0)
            {
                Util.Config.DEFAULT_BUFFERSIZE = Util.Constants.DEFAULT_DEFAULT_BUFFERSIZE;
            }
            else
            {
                if (Util.Config.DEFAULT_BUFFERSIZE < 16)
                {
                    Util.Config.DEFAULT_BUFFERSIZE = 16;
                }

                if (Util.Config.DEFAULT_BUFFERSIZE < Util.Config.DEFAULT_CHUNKSIZE)
                {
                    Util.Config.DEFAULT_BUFFERSIZE = Util.Config.DEFAULT_CHUNKSIZE;
                }
                else if (Util.Config.DEFAULT_BUFFERSIZE > Util.Config.MAX_CACHESTREAMSIZE)
                {
                    Util.Config.DEFAULT_BUFFERSIZE = Util.Config.MAX_CACHESTREAMSIZE;
                }
            }

            if (Util.Config.DEFAULT_CACHESTREAMSIZE <= 0)
            {
                Util.Config.DEFAULT_CACHESTREAMSIZE = Util.Constants.DEFAULT_DEFAULT_CACHESTREAMSIZE;
            }
            else if (Util.Config.DEFAULT_CACHESTREAMSIZE <= Util.Config.DEFAULT_BUFFERSIZE)
            {
                Util.Config.DEFAULT_CACHESTREAMSIZE = Util.Config.DEFAULT_BUFFERSIZE;
            }
            else if (Util.Config.DEFAULT_CACHESTREAMSIZE > Util.Config.MAX_CACHESTREAMSIZE)
            {
                Util.Config.DEFAULT_CACHESTREAMSIZE = Util.Config.MAX_CACHESTREAMSIZE;
            }
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)