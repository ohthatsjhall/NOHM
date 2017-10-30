using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorExtension
{
    /// <summary>Custom editor for the 'RadioPlayer'-class.</summary>
    [InitializeOnLoad]
    [CustomEditor(typeof(RadioManager))]
    public class RadioManagerEditor : Editor
    {

        #region Variables

        private bool isRandom = false;
        private int channels = 2;
        private int sampleRate = 44100;

        private GameObject go;
        private RadioPlayer rp;
        private RadioManager script;

        private bool showStations = false;
        private bool showPlayers = false;
        private bool showRecords = false;

        public delegate void StopPlayback();
        public static event StopPlayback OnStopPlayback;

        #endregion


        #region Static constructor

        static RadioManagerEditor()
        {
            EditorApplication.update += onEditorUpdate;
        }

        #endregion


        #region Editor methods

        public void OnEnable()
        {
            script = (RadioManager)target;

            if (Util.Helper.isEditorMode)
            {
                OnStopPlayback += removeRadio;
            }
        }

        public void OnDisable()
        {
            if (Util.Helper.isEditorMode)
            {
                removeRadio();

                OnStopPlayback -= removeRadio;
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorHelper.SeparatorUI();

            if (script.isActiveAndEnabled)
            {
                if (script.Providers != null && script.Providers.Length > 0)
                {
                    GUILayout.Label("Data", EditorStyles.boldLabel);

                    showStations = EditorGUILayout.Foldout(showStations, "Stations (" + script.CountStations() + "/" + script.Stations.Count + ")");
                    if (showStations)
                    {
                        EditorGUI.indentLevel++;

                        foreach (Model.RadioStation station in script.StationsByName())
                        {
                            EditorGUILayout.SelectableLabel(station.ToShortString(), GUILayout.Height(16), GUILayout.ExpandHeight(false));
                        }

                        EditorGUI.indentLevel--;
                    }

                    showPlayers = EditorGUILayout.Foldout(showPlayers, "Players (" + script.CountPlayers() + "/" + script.Players.Count + ")");
                    if (showPlayers)
                    {
                        EditorGUI.indentLevel++;

                        foreach (RadioPlayer player in script.Players)
                        {
                            EditorGUILayout.SelectableLabel(player.ToShortString(), GUILayout.Height(16), GUILayout.ExpandHeight(false));
                        }

                        EditorGUI.indentLevel--;
                    }

                    GUILayout.Space(8);

                    if (Util.Helper.isEditorMode)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button(new GUIContent(" Load", EditorHelper.Icon_Refresh, "Loads all radio stations from the given providers.")))
                            {
                                if (script.Providers != null)
                                {
                                    if (EditorUtility.DisplayDialog("Load all stations?", "Loading all stations of this manager can freeze the Unity Editor for quite a long time. Would you load the data now?", "Load", "Abort"))
                                    {
                                        foreach (Provider.BaseRadioProvider _rp in script.Providers)
                                        {
                                            if (_rp != null && _rp.isActiveAndEnabled)
                                            {
                                                _rp.Load();
                                            }
                                        }

                                        script.Load();

                                        GAApi.Event(typeof(RadioManagerEditor).Name, "Load");
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning("'Providers' is null - please add at least one provider in the Inspector!");
                                }
                            }

                            if (script.Stations != null && script.Stations.Count > 0)
                            {
                                if (GUILayout.Button(new GUIContent(" Save", EditorHelper.Icon_Save, "Saves all loaded radio stations as a text-file with streams.")))
                                {
                                    string path = EditorUtility.SaveFilePanelInProject("Save text-file", "Radios.txt", "txt", "Please enter a file name to save the radio stations to");
                                    if (path.Length != 0)
                                    {

                                        script.Save(path);

                                        EditorHelper.RefreshAssetDatabase();

                                        GAApi.Event(typeof(RadioManagerEditor).Name, "Save");
                                    }
                                }
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    

                    EditorHelper.SeparatorUI();

                    GUILayout.Label("Test-Drive", EditorStyles.boldLabel);

                    if (Util.Helper.isEditorMode)
                    {
                        isRandom = EditorGUILayout.Toggle(new GUIContent("Play Random Station", "Enable or disable play random radio stations (default: false)."), isRandom);

                        GUILayout.Space(8);

                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button(new GUIContent(" Previous", EditorHelper.Icon_Previous, "Plays the previous radio station.")))
                            {
                                removeRadio();
                                addRadio();

                                if (rp != null)
                                {
                                    rp.Station = script.PreviousStation(isRandom);

                                    rp.PlayInEditor(channels, sampleRate);
                                }

                                GAApi.Event(typeof(RadioManagerEditor).Name, "Previous");
                            }

                            GUI.enabled = rp != null && rp.Source != null && rp.isPlayback;

                            if (GUILayout.Button(new GUIContent(" Stop", EditorHelper.Icon_Stop, "Stops the radio station.")))
                            {
                                removeRadio();

                                GAApi.Event(typeof(RadioManagerEditor).Name, "Stop");
                            }
                            GUI.enabled = true;

                            if (GUILayout.Button(new GUIContent(" Next", EditorHelper.Icon_Next, "Plays the next radio station.")))
                            {
                                removeRadio();
                                addRadio();

                                if (rp != null)
                                {
                                    rp.Station = script.NextStation(isRandom);

                                    rp.PlayInEditor(channels, sampleRate);
                                }

                                GAApi.Event(typeof(RadioManagerEditor).Name, "Next");
                            }
                        }
                        GUILayout.EndHorizontal();

                        if (rp != null && rp.Source != null && rp.isPlayback)
                        {
                            GUILayout.Space(8);

                            GUILayout.BeginHorizontal();
                            {
                                if (rp.isBuffering)
                                {
                                    GUILayout.Label("Buffering station '" + rp.Station.Name + "':");

                                    GUI.skin.label.alignment = TextAnchor.MiddleRight;
                                    GUILayout.Label(rp.BufferProgress.ToString(Util.Constants.FORMAT_PERCENT));
                                    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                                }
                                else
                                {
                                    GUILayout.Label(rp.Station.Name);

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
                        EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
                    }

                    if (Util.Helper.isEditorMode && rp != null && rp.Station != null)
                    {
                        EditorHelper.SeparatorUI();

                        GUILayout.Label("Station Information", EditorStyles.boldLabel);

                        GUILayout.Label("Name:\t\t" + rp.Station.Name);
                        GUILayout.Label("Genres:\t\t" + rp.Station.Genres);
                        GUILayout.Label("Format:\t\t" + rp.Station.Format);
                        GUILayout.Label("Bitrate:\t\t" + rp.Station.Bitrate + "kb/s");
                        GUILayout.Label("Rating:\t\t" + rp.Station.Rating);

                        if (!Util.Helper.isEditorMode && !rp.LegacyMode)
                        {
                            GUILayout.Label("Current Record:", EditorStyles.boldLabel);

                            if (!string.IsNullOrEmpty(rp.RecordInfo.Info))
                            {
                                GUILayout.Label("Title:\t\t" + rp.RecordInfo.Title);
                                GUILayout.Label("Artist:\t\t" + rp.RecordInfo.Artist);
                            }

                            GUILayout.Label("Time:\t\t" + Util.Helper.FormatSecondsToHourMinSec(rp.RecordPlayTime));

                            GUILayout.Space(6);

                            showRecords = EditorGUILayout.Foldout(showRecords, "Played records (" + rp.Station.PlayedRecords.Count + ")");
                            if (showRecords)
                            {
                                EditorGUI.indentLevel++;

                                foreach (Model.RecordInfo ri in rp.Station.PlayedRecords)
                                {
                                    EditorGUILayout.SelectableLabel(ri.ToShortString(), GUILayout.Height(16), GUILayout.ExpandHeight(false));
                                }

                                EditorGUI.indentLevel--;
                            }

                            GUILayout.Space(6);
                        }

                        GUILayout.Label("Stats:", EditorStyles.boldLabel);

                        GUILayout.Label("Total download:\t" + Util.Helper.FormatBytesToHRF(rp.Station.TotalDataSize));
                        GUILayout.Label("Total requests:\t" + rp.Station.TotalDataRequests);
                    }

                    EditorHelper.SeparatorUI();

                    GUILayout.Label("Global Information", EditorStyles.boldLabel);

                    if (!Util.Helper.isEditorMode)
                        GUILayout.Label("Total time:\t\t" + Util.Helper.FormatSecondsToHourMinSec(Util.Context.TotalPlayTime));
                    GUILayout.Label("Total download:\t" + Util.Helper.FormatBytesToHRF(Util.Context.TotalDataSize));
                    GUILayout.Label("Total requests:\t" + Util.Context.TotalDataRequests);
                }
                else
                {
                    EditorGUILayout.HelpBox("Please add a Provider!", MessageType.Warning);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
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
            go = EditorUtility.CreateGameObjectWithHideFlags("invisibleRadioPlayer", /* HideFlags.DontUnloadUnusedAsset | */ HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy);

            rp = go.AddComponent<RadioPlayer>();
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)