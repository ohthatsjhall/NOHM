using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorExtension
{
    /// <summary>Custom editor for the 'RadioPlayer'-class.</summary>
    [InitializeOnLoad]
    [CustomEditor(typeof(RadioPlayer))]
    public class RadioPlayerEditor : Editor
    {

        #region Variables

        public delegate void StopPlayback();
        public static event StopPlayback OnStopPlayback;

        private RadioPlayer script;

        private int channels = 2;
        private int sampleRate = 44100;

        private bool showRecords = false;

        #endregion


        #region Static constructor

        static RadioPlayerEditor()
        {
            EditorApplication.update += onEditorUpdate;
            EditorApplication.hierarchyWindowItemOnGUI += hierarchyItemCB;
        }

        #endregion


        #region Editor methods

        public void OnEnable()
        {
            script = (RadioPlayer)target;

            if (Util.Helper.isEditorMode)
            {
                OnStopPlayback += stop;
            }
        }

        public void OnDisable()
        {
            if (Util.Helper.isEditorMode)
            {
                stop();

                OnStopPlayback -= stop;
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (Util.Constants.DEV_DEBUG)
            {
                GUILayout.Label("BufferSize:\t" + Util.Helper.FormatBytesToHRF(script.CurrentBufferSize));
                GUILayout.Label("Download speed:\t" + Util.Helper.FormatBytesToHRF(script.CurrentDownloadSpeed) + "/s");
            }

            EditorHelper.SeparatorUI();

            if (script.isActiveAndEnabled)
            {

                if (!string.IsNullOrEmpty(script.Station.Url))
                {
                    GUILayout.Label("Test-Drive", EditorStyles.boldLabel);

                    if (Util.Helper.isEditorMode)
                    {
                        if (script.isPlayback && script.Source != null)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                if (script.isBuffering)
                                {
                                    GUILayout.Label("Buffering:");

                                    GUI.skin.label.alignment = TextAnchor.MiddleRight;
                                    GUILayout.Label(script.BufferProgress.ToString(Util.Constants.FORMAT_PERCENT));
                                    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                                }
                                else
                                {
                                    GUILayout.Label("Playing:");

                                    GUI.skin.label.alignment = TextAnchor.MiddleRight;
                                    GUILayout.Label(Util.Helper.FormatSecondsToHourMinSec(script.Source.time));
                                    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                                }
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(8);

                            if (GUILayout.Button(new GUIContent(" Stop", EditorHelper.Icon_Stop, "Stops the radio station.")))
                            {
                                stop();
                                GAApi.Event(typeof(RadioPlayerEditor).Name, "Stop");
                            }
                        }
                        else
                        {
                            channels = EditorGUILayout.IntSlider(new GUIContent("Channels", "Audio channels of the station (default: 2)."), channels, 1, 2);
                            sampleRate = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Sample Rate", "Audio sample rate (default: 44100)."), sampleRate), 8000, 192000);

                            GUILayout.Space(8);

                            if (GUILayout.Button(new GUIContent(" Play", EditorHelper.Icon_Play, "Plays the radio station.")))
                            {
                                script.PlayInEditor(channels, sampleRate);
                                GAApi.Event(typeof(RadioPlayerEditor).Name, "Play");
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
                    }

                    EditorHelper.SeparatorUI();

                    if (script.Station != null)
                    {
                        GUILayout.Label("Station Information", EditorStyles.boldLabel);

                        if (!Util.Helper.isEditorMode && !script.LegacyMode)
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
                        GUILayout.Label("Total download:\t" + Util.Helper.FormatBytesToHRF(script.Station.TotalDataSize));
                        GUILayout.Label("Total requests:\t" + script.Station.TotalDataRequests);
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
                    EditorGUILayout.HelpBox("Please add an URL for the radio station!", MessageType.Warning);
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

        private void stop()
        {
            if (script != null)
            {
                script.Stop();
            }
        }

        private static void hierarchyItemCB(int instanceID, Rect selectionRect)
        {
            if (Util.Config.HIERARCHY_ICON)
            {
                GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

                if (go != null && go.GetComponent<RadioPlayer>())
                {
                    Rect r = new Rect(selectionRect);
                    r.x = r.width - 4;

                    //Debug.Log("HierarchyItemCB: " + r);

                    GUI.Label(r, EditorHelper.Logo_Asset_Small);
                }
            }
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)