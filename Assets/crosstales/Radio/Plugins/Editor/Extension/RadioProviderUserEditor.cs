using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorExtension
{
    /// <summary>Custom editor for the 'RadioProviderUser'-class.</summary>
    [CustomEditor(typeof(Provider.RadioProviderUser))]
    public class RadioProviderUserEditor : Editor
    {

        #region Variables

        private Provider.RadioProviderUser script;

        private bool showStations = false;

        #endregion


        #region Editor methods

        public void OnEnable()
        {
            script = (Provider.RadioProviderUser)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorHelper.SeparatorUI();

            if (script.isActiveAndEnabled)
            {
                if (!string.IsNullOrEmpty(script.Entry.Path))
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

                    GUILayout.Space(8);

                    if (Util.Helper.isEditorMode)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button(new GUIContent(" Load", EditorHelper.Icon_Refresh, "Loads all radio stations from the user text-file and resource.")))
                            {
                                script.Load();
                                GAApi.Event(typeof(RadioProviderUserEditor).Name, "Load");
                            }

                            if (script.Stations != null && script.Stations.Count > 0)
                            {
                                if (GUILayout.Button(new GUIContent(" Save", EditorHelper.Icon_Save, "Saves all loaded radio stations as user text-file.")))
                                {
                                    script.Save(script.Entry.FinalPath);

                                    EditorHelper.RefreshAssetDatabase();

                                    GAApi.Event(typeof(RadioProviderUserEditor).Name, "Save");
                                }
                            }

                            if (GUILayout.Button(new GUIContent(" Delete", EditorHelper.Icon_Delete, "Deletes the user text-file.")))
                            {
                                if (EditorUtility.DisplayDialog("Delete user file?", "Delete the user text-file?", "Yes", "No"))
                                {
                                    script.Delete();

                                    EditorHelper.RefreshAssetDatabase();

                                    GAApi.Event(typeof(RadioProviderUserEditor).Name, "Delete");
                                }
                            }
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.Space(8);

                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button(new GUIContent(" Show file", EditorHelper.Icon_Show, "Shows the location of the user text-file in OS file browser.")))
                            {
                                script.ShowFile();
                                GAApi.Event(typeof(RadioProviderUserEditor).Name, "Show file");
                            }

                            if (GUILayout.Button(new GUIContent(" Edit file", EditorHelper.Icon_Edit, "Edits the user text-file with the OS default application.")))
                            {
                                script.EditFile();
                                GAApi.Event(typeof(RadioProviderUserEditor).Name, "Edit file");
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Please add a Path!", MessageType.Warning);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
            }
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)