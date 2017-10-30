using UnityEditor;
using UnityEngine;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorIntegration
{
    /// <summary>Editor window extension.</summary>
    [InitializeOnLoad]
    public class ConfigWindow : ConfigBase
    {

        #region Variables

        private int tab = 0;
        private int lastTab = 0;
        private GameObject go;
        private RadioPlayer rp;

        private string tdUrl = "http://185.33.21.112:11010";

        private Model.Enum.AudioFormat[] formats = { Model.Enum.AudioFormat.MP3, Model.Enum.AudioFormat.OGG };
        private int formatIndex;

        private int channels = 2;
        private int sampleRate = 44100;

        private Vector2 scrollPosPrefabs;
        private Vector2 scrollPosTD;

        //private bool showRecords = false;

        public delegate void StopPlayback();
        public static event StopPlayback OnStopPlayback;

        #endregion


        #region Static constructor

        static ConfigWindow()
        {
            EditorApplication.update += onEditorUpdate;
        }

        #endregion


        #region EditorWindow methods

        [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Configuration...", false, EditorHelper.MENU_ID + 1)]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ConfigWindow));
        }

        public static void ShowWindow(int tab)
        {
            ConfigWindow window = EditorWindow.GetWindow(typeof(ConfigWindow)) as ConfigWindow;
            window.tab = tab;
        }

        public void OnEnable()
        {
            //base.OnEnable();

            titleContent = new GUIContent(Util.Constants.ASSET_NAME, EditorHelper.Logo_Asset_Small);

            OnStopPlayback += removeRadio;
        }

        public void OnDisable()
        {
            removeRadio();

            OnStopPlayback -= removeRadio;
        }

        public void OnInspectorUpdate()
        {
            //if (tab == 2)
            //{
            // This will only get called 10 times per second.
            Repaint();
            //}
        }

        public void OnGUI()
        {
            tab = GUILayout.Toolbar(tab, new string[] { "Config", "Prefabs", "TD", "Help", "About" });

            if (tab != lastTab)
            {
                lastTab = tab;
                GUI.FocusControl(null);
            }

            if (tab == 0)
            {
                showConfiguration();

                EditorHelper.SeparatorUI();

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(new GUIContent(" Save", EditorHelper.Icon_Save, "Saves the configuration settings for this project")))
                    {
                        save();

                        GAApi.Event(typeof(ConfigWindow).Name, "Save configuration");
                    }

                    if (GUILayout.Button(new GUIContent(" Reset", EditorHelper.Icon_Reset, "Resets the configuration settings for this project.")))
                    {
                        if (EditorUtility.DisplayDialog("Reset configuration?", "Reset the configuration of " + Util.Constants.ASSET_NAME + "?", "Yes", "No"))
                        {
                            Util.Config.Reset();
                            save();

                            GAApi.Event(typeof(ConfigWindow).Name, "Reset configuration");
                        }
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(6);
            }
            else if (tab == 1)
            {
                showPrefabs();

                //Repaint();
            }
            else if (tab == 2)
            {
                showTestDrive();

                //Repaint();
            }
            else if (tab == 3)
            {
                showHelp();

                //Repaint();
            }
            else
            {
                showAbout();
            }
        }

        #endregion


        #region Private methods

        private static void onEditorUpdate()
        {
            if (EditorApplication.isCompiling || EditorApplication.isPlaying || BuildPipeline.isBuildingPlayer)
            {
                onStopPlayback();
            }
        }

        private static void onStopPlayback()
        {
            if (OnStopPlayback != null)
            {
                OnStopPlayback();
            }
        }

        private void showPrefabs()
        {
            scrollPosPrefabs = EditorGUILayout.BeginScrollView(scrollPosPrefabs, false, false);
            {
                GUILayout.Label("Available Prefabs", EditorStyles.boldLabel);

                GUILayout.Space(6);

                GUILayout.Label("RadioPlayer");

                if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'RadioPlayer'-prefab to the scene.")))
                {
                    EditorHelper.InstantiatePrefab("RadioPlayer");
                    GAApi.Event(typeof(ConfigWindow).Name, "Add RadioPlayer");
                }

				EditorHelper.SeparatorUI();

				GUILayout.Label("RadioProviderResource");

				if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'RadioProviderResource'-prefab to the scene.")))
				{
					EditorHelper.InstantiatePrefab("RadioProviderResource");
					GAApi.Event(typeof(ConfigWindow).Name, "Add RadioProviderResource");
				}

				GUILayout.Space(6);

				GUILayout.Label("RadioProviderShoutcast");

				if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'RadioProviderShoutcast'-prefab to the scene.")))
				{
					EditorHelper.InstantiatePrefab("RadioProviderShoutcast");
					GAApi.Event(typeof(ConfigWindow).Name, "Add RadioProviderShoutcast");
				}

				GUILayout.Space(6);

				GUILayout.Label("RadioProviderURL");

				if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'RadioProviderURL'-prefab to the scene.")))
				{
					EditorHelper.InstantiatePrefab("RadioProviderURL");
					GAApi.Event(typeof(ConfigWindow).Name, "Add RadioProviderURL");
				}

				GUILayout.Space(6);
				GUILayout.Label("RadioProviderUser");

				if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'RadioProviderUser'-prefab to the scene.")))
				{
					EditorHelper.InstantiatePrefab("RadioProviderUser");
					GAApi.Event(typeof(ConfigWindow).Name, "Add RadioProviderUser");
				}

				EditorHelper.SeparatorUI();

                GUILayout.Label("RadioManager");

                if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'RadioManager'-prefab to the scene.")))
                {
                    EditorHelper.InstantiatePrefab("RadioManager");
                    GAApi.Event(typeof(ConfigWindow).Name, "Add RadioManager");
                }

                EditorHelper.SeparatorUI();

                GUILayout.Label("SimplePlayer");

                if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'SimplePlayer'-prefab to the scene.")))
                {
                    EditorHelper.InstantiatePrefab("SimplePlayer");
                    GAApi.Event(typeof(ConfigWindow).Name, "Add SimplePlayer");
                }

                EditorHelper.SeparatorUI();

                GUILayout.Label("Loudspeaker");

                if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'Loudspeaker'-prefab to the scene.")))
                {
                    EditorHelper.InstantiatePrefab("Loudspeaker");
                    GAApi.Event(typeof(ConfigWindow).Name, "Add Loudspeaker");
                }

                GUILayout.Space(6);

                GUILayout.Label("StreamSaver");

                if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'StreamSaver'-prefab to the scene.")))
                {
                    EditorHelper.InstantiatePrefab("StreamSaver");
                    GAApi.Event(typeof(ConfigWindow).Name, "Add StreamSaver");
                }

                if (!EditorHelper.isSurviveSceneSwitchInScene)
                {
                    EditorHelper.SeparatorUI();

                    GUILayout.Label(Util.Constants.SURVIVOR_SCENE_OBJECT_NAME);
                    if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a '" + Util.Constants.SURVIVOR_SCENE_OBJECT_NAME + "'-prefab to the scene.")))
                    {
                        EditorHelper.InstantiatePrefab(Util.Constants.SURVIVOR_SCENE_OBJECT_NAME);
                        GAApi.Event(typeof(ConfigWindow).Name, "Add " + Util.Constants.SURVIVOR_SCENE_OBJECT_NAME);
                    }
                }

                if (!EditorHelper.isInternetCheckInScene)
                {
                    EditorHelper.SeparatorUI();

                    GUILayout.Label(Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME);
                    if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a '" + Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME + "'-prefab to the scene.")))
                    {
                        EditorHelper.InstantiatePrefab(Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME);
                        GAApi.Event(typeof(ConfigWindow).Name, "Add " + Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME);
                    }
                }

                if (!EditorHelper.isProxyInScene)
                {
                    if (EditorHelper.isInternetCheckInScene)
                    {
                        EditorHelper.SeparatorUI();
                    } else
                    {
                        GUILayout.Space(6);
                    }

                    GUILayout.Label(Util.Constants.PROXY_SCENE_OBJECT_NAME);
                    if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a '" + Util.Constants.PROXY_SCENE_OBJECT_NAME + "'-prefab to the scene.")))
                    {
                        EditorHelper.InstantiatePrefab(Util.Constants.PROXY_SCENE_OBJECT_NAME);
                        GAApi.Event(typeof(ConfigWindow).Name, "Add " + Util.Constants.PROXY_SCENE_OBJECT_NAME);
                    }
                }

                GUILayout.Space(6);
            }
            EditorGUILayout.EndScrollView();
        }

        private void showTestDrive()
        {
            if (Util.Helper.isEditorMode)
            {
                scrollPosTD = EditorGUILayout.BeginScrollView(scrollPosTD, false, false);
                {
                    GUILayout.Label("Test-Drive", EditorStyles.boldLabel);

                    tdUrl = EditorGUILayout.TextField(new GUIContent("URL", "URL of the radio station."), tdUrl);

                    formatIndex = EditorGUILayout.Popup("Format", formatIndex, System.Array.ConvertAll(formats, x => x.ToString()));
                    channels = EditorGUILayout.IntSlider(new GUIContent("Channels", "Audio channels of the station (default: 2)."), channels, 1, 2);
                    sampleRate = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Sample Rate", "Audio sample rate (default: 44100)."), sampleRate), 8000, 192000);

                    if (rp != null && rp.Station != null)
                    {
                        EditorHelper.SeparatorUI();

                        GUILayout.Label("Station Information", EditorStyles.boldLabel);

                        //                      if (rp.isPlayback && !rp.isLegacyMode)
                        //                        {
                        //                            GUILayout.Label("Current Record:", EditorStyles.boldLabel);
                        //
                        //                            if (!string.IsNullOrEmpty(rp.RecordInfo.Info))
                        //                            {
                        //                                GUILayout.Label("Title:\t\t" + rp.RecordInfo.Title);
                        //                                GUILayout.Label("Artist:\t\t" + rp.RecordInfo.Artist);
                        //                            }
                        //
                        //                            GUILayout.Space(6);
                        //                        }

                        GUILayout.Label("Stats:", EditorStyles.boldLabel);

                        GUILayout.Label("Total download:\t" + Util.Helper.FormatBytesToHRF(rp.Station.TotalDataSize));

                        //                        GUILayout.Space(6);
                        //
                        //                        showRecords = EditorGUILayout.Foldout(showRecords, "Played records (" + rp.Station.PlayedRecords.Count + ")");
                        //                        if (showRecords)
                        //                        {
                        //                            EditorGUI.indentLevel++;
                        //
                        //                            foreach (Model.RecordInfo ri in rp.Station.PlayedRecords)
                        //                            {
                        //                                EditorGUILayout.SelectableLabel(ri.ToShortString(), GUILayout.Height(16), GUILayout.ExpandHeight(false));
                        //                            }
                        //
                        //                            EditorGUI.indentLevel--;
                        //                        }

                    }

                    EditorHelper.SeparatorUI();

                    GUILayout.Label("Global Information", EditorStyles.boldLabel);

                    //GUILayout.Label("Total time:\t\t" + Util.Helper.FormatSecondsToHourMinSec(Util.Context.TotalPlayTime));
                    GUILayout.Label("Total download:\t" + Util.Helper.FormatBytesToHRF(Util.Context.TotalDataSize));
                    GUILayout.Label("Total requests:\t" + Util.Context.TotalDataRequests);
                }
                EditorGUILayout.EndScrollView();
                EditorHelper.SeparatorUI();

                if (rp != null && rp.Source != null && rp.isPlayback)
                {
                    if (GUILayout.Button(new GUIContent(" Stop", EditorHelper.Icon_Stop, "Stops the radio station.")))
                    {
                        removeRadio();
                    }
                    else
                    {
                        GUILayout.Space(8);

                        GUILayout.BeginHorizontal();
                        {
                            if (rp.isBuffering)
                            {
                                GUILayout.Label("Buffering:");

                                GUI.skin.label.alignment = TextAnchor.MiddleRight;
                                GUILayout.Label(rp.BufferProgress.ToString(Util.Constants.FORMAT_PERCENT));
                                GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                            }
                            else
                            {
                                GUILayout.Label("Playing:");

                                GUI.skin.label.alignment = TextAnchor.MiddleRight;
                                GUILayout.Label(Util.Helper.FormatSecondsToHourMinSec(rp.Source != null ? rp.Source.time : 0f));
                                GUI.skin.label.alignment = TextAnchor.MiddleLeft;

                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(tdUrl))
                    {
                        if (GUILayout.Button(new GUIContent(" Play", EditorHelper.Icon_Play, "Plays the radio station.")))
                        {
                            //removeRadio();
                            addRadio();

                            rp.Station = new Model.RadioStation("TD-Radio", tdUrl, formats[formatIndex]);

                            rp.PlayInEditor(channels, sampleRate);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Please add an URL for the radio station!", MessageType.Warning);
                    }
                }

                GUILayout.Space(6);
            }
            else
            {
                EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
            }
        }

        private void removeRadio()
        {
            if (rp != null)
            {
                rp.Stop();
                rp = null;
            }

            if (go != null)
            {
                DestroyImmediate(go);
                go = null;
            }
        }

        private void addRadio()
        {
            go = EditorUtility.CreateGameObjectWithHideFlags("invisibleRadioPlayer", /*HideFlags.DontUnloadUnusedAsset | */ HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy);

            rp = go.AddComponent<RadioPlayer>();
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)