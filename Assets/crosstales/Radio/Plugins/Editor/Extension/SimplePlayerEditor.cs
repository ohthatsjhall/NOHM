using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorExtension
{
    /// <summary>Custom editor for the 'SimplePlayer'-class.</summary>
    [InitializeOnLoad]
    [CustomEditor(typeof(SimplePlayer))]
    public class SimplePlayerEditor : Editor
    {

        #region Variables

        private SimplePlayer script;

        private bool showStations = false;
        private bool showRecords = false;

        public delegate void StopPlayback();

        public static event StopPlayback OnStopPlayback;

        #endregion


        #region Static constructor

        static SimplePlayerEditor()
        {
            EditorApplication.update += onEditorUpdate;
        }

        #endregion


        #region Editor methods

        public void OnEnable()
        {
            script = (SimplePlayer)target;

            if (Util.Helper.isEditorMode)
            {
                OnStopPlayback += stopRadio;
            }
        }

        public void OnDisable()
        {
            if (Util.Helper.isEditorMode)
            {

                stopRadio();

                OnStopPlayback -= stopRadio;
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
                if (script.Player != null && script.Manager != null)
                {
                    GUILayout.Label("Data", EditorStyles.boldLabel);

                    if (script.Manager == null)
                    {
                        showStations = EditorGUILayout.Foldout(showStations, "Stations (" + script.Stations.Count + ")");
                    }
                    else
                    {
                        showStations = EditorGUILayout.Foldout(showStations, "Stations (" + script.Stations.Count + "/" + script.Manager.Stations.Count + ")");
                    }

                    if (showStations)
                    {
                        EditorGUI.indentLevel++;

                        foreach (Model.RadioStation station in script.Stations)
                        {

                            EditorGUILayout.SelectableLabel(station.ToShortString(), GUILayout.Height(16), GUILayout.ExpandHeight(false));
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
                                if (script.Manager != null)
                                {
                                    if (EditorUtility.DisplayDialog("Load all stations?", "Loading all stations of this manager can freeze the Unity Editor for quite a long time. Would you load the data now?", "Load", "Abort"))
                                    {
                                        script.Manager.Load();

                                        GAApi.Event(typeof(SimplePlayerEditor).Name, "Load");
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning("'Manager' is null - please add one in the Inspector!");
                                }
                            }

                            if (script.Stations != null && script.Stations.Count > 0)
                            {
                                if (GUILayout.Button(new GUIContent(" Save", EditorHelper.Icon_Save, "Saves all loaded radio stations as a text-file with streams.")))
                                {
                                    string path = EditorUtility.SaveFilePanelInProject("Save text-file", "Radios.txt", "txt", "Please enter a file name to save the radio stations to");
                                    if (path.Length != 0)
                                    {
                                        if (script.Filter.isFiltering)
                                        {
                                            script.Manager.Save(path, script.Filter);
                                        }
                                        else
                                        {
                                            script.Manager.Save(path);
                                        }

                                        EditorHelper.RefreshAssetDatabase();

                                        GAApi.Event(typeof(SimplePlayerEditor).Name, "Save");
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
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button(new GUIContent(" Previous", EditorHelper.Icon_Previous, "Plays the previous radio station.")))
                            {
                                script.Previous(script.PlayRandom);

                                GAApi.Event(typeof(SimplePlayerEditor).Name, "Previous");
                            }

                            GUI.enabled = script.isPlayback;
                            if (GUILayout.Button(new GUIContent(" Stop", EditorHelper.Icon_Stop, "Stops the radio station.")))
                            {
                                script.Stop();

                                GAApi.Event(typeof(SimplePlayerEditor).Name, "Stop");
                            }
                            GUI.enabled = true;

                            if (GUILayout.Button(new GUIContent(" Next", EditorHelper.Icon_Next, "Plays the next radio station.")))
                            {
                                script.Next(script.PlayRandom);

                                GAApi.Event(typeof(SimplePlayerEditor).Name, "Next");
                            }
                        }
                        GUILayout.EndHorizontal();

                        if (script.isPlayback)
                        {
                            GUILayout.Space(8);

                            GUILayout.BeginHorizontal();
                            {
                                if (script.isBuffering)
                                {
                                    GUILayout.Label("Buffering station '" + script.Station.Name + "':");

                                    GUI.skin.label.alignment = TextAnchor.MiddleRight;
                                    GUILayout.Label(script.BufferProgress.ToString(Util.Constants.FORMAT_PERCENT));
                                    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                                }
                                else
                                {
                                    GUILayout.Label(script.Station.Name);

                                    GUI.skin.label.alignment = TextAnchor.MiddleRight;
                                    GUILayout.Label(Util.Helper.FormatSecondsToHourMinSec(script.Source != null ? script.Source.time : 0f));
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

                    if (script.Player != null && script.isPlayback)
                    {
                        EditorHelper.SeparatorUI();

                        GUILayout.Label("Station Information", EditorStyles.boldLabel);

                        GUILayout.Label("Name:\t\t" + script.Station.Name);
                        GUILayout.Label("Genres:\t\t" + script.Station.Genres);
                        GUILayout.Label("Format:\t\t" + script.Station.Format);
                        GUILayout.Label("Bitrate:\t\t" + script.Station.Bitrate + "kb/s");
                        GUILayout.Label("Rating:\t\t" + script.Station.Rating);

                        if (!Util.Helper.isEditorMode && !script.Player.LegacyMode)
                        {
                            GUILayout.Label("Current Record:", EditorStyles.boldLabel);

                            if (!string.IsNullOrEmpty(script.RecordInfo.Info))
                            {
                                GUILayout.Label("Title:\t\t" + script.RecordInfo.Title);
                                GUILayout.Label("Artist:\t\t" + script.RecordInfo.Artist);
                            }

                            GUILayout.Label("Time:\t\t" + Util.Helper.FormatSecondsToHourMinSec(script.RecordPlayTime));

                            GUILayout.Space(6);

                            showRecords = EditorGUILayout.Foldout(showRecords, "Played records (" + script.Station.PlayedRecords.Count + ")");
                            if (showRecords)
                            {
                                EditorGUI.indentLevel++;

                                foreach (Model.RecordInfo ri in script.Station.PlayedRecords)
                                {
                                    EditorGUILayout.SelectableLabel(ri.ToShortString(), GUILayout.Height(16), GUILayout.ExpandHeight(false));
                                }

                                EditorGUI.indentLevel--;
                            }

                            GUILayout.Space(6);
                        }

                        GUILayout.Label("Stats:", EditorStyles.boldLabel);

                        if (!Util.Helper.isEditorMode)
                            GUILayout.Label("Total time:\t\t" + Util.Helper.FormatSecondsToHourMinSec(script.Station.TotalPlayTime));

                        if (script.Station != null)
                        {
                            GUILayout.Label("Total download:\t" + Util.Helper.FormatBytesToHRF(script.Station.TotalDataSize));
                            GUILayout.Label("Total requests:\t" + script.Station.TotalDataRequests);
                        }
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
                    EditorGUILayout.HelpBox("Please add a Player and a Manager!", MessageType.Warning);
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

        private void stopRadio()
        {
            script.Stop();
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)