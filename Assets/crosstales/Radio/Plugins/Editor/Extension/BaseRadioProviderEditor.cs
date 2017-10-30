using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorExtension
{
    /// <summary>Base-class for custom editors of children of the 'BaseRadioProvider'-class.</summary>
    public abstract class BaseRadioProviderEditor : Editor
    {

        #region Variables

        private Provider.BaseRadioProvider script;

        private bool showStations = false;

        #endregion


        #region Editor methods

        public virtual void OnEnable()
        {
            script = (Provider.BaseRadioProvider)target;
        }

        //        public override void OnInspectorGUI()
        //        {
        //            DrawDefaultInspector();
        //
        //            EditorHelper.SeparatorUI();
        //
        //            if (script.isActiveAndEnabled)
        //            {
        //              showData ();
        //          }
        //            else
        //            {
        //              EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
        //            }
        //        }

        #endregion

        protected void showData()
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
                if (GUILayout.Button(new GUIContent(" Load", EditorHelper.Icon_Refresh, "Loads all radio stations from the given sources.")))
                {
                    if (EditorUtility.DisplayDialog("Load all stations?", "Loading all stations of this provider can freeze the Unity Editor for quite a long time. Would you load the data now?", "Load", "Abort"))
                    {
                        script.Load();
                        GAApi.Event(typeof(BaseRadioProviderEditor).Name, "Load");
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

                            GAApi.Event(typeof(BaseRadioProviderEditor).Name, "Save");
                        }
                    }
                }
                //                }
                //                else
                //                {
                //                  EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
            }
        }
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)