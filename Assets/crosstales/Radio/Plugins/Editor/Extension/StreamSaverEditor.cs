using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorExtension
{
    /// <summary>Custom editor for the 'StreamSaver'-class.</summary>
    [CustomEditor(typeof(Tool.StreamSaver))]
    public class StreamSaverEditor : Editor
    {

        #region Variables

        private Tool.StreamSaver script;

        #endregion


        #region Editor methods

        public void OnEnable()
        {
            script = (Tool.StreamSaver)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            /*
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.SelectableLabel(script.OutputPath);

                if (GUILayout.Button(new GUIContent(" Select", EditorHelper.Icon_Refresh, "Select path for the cache"))) //Icon_Folder
                {
                    string path = EditorUtility.OpenFolderPanel("Select path for the audio files", script.OutputPath, "");

                    if (!string.IsNullOrEmpty(path))
                    {
                        script.OutputPath = path + (Util.Helper.isWindowsPlatform ? "\\" : "/");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            */

            EditorHelper.SeparatorUI();

            if (script.isActiveAndEnabled)
            {
				if (script.Player != null)
				{
					//TODO add stuff if needed
				}
				else
				{
					EditorHelper.SeparatorUI();
					EditorGUILayout.HelpBox("Please add a Player!", MessageType.Warning);
				}

                EditorGUILayout.HelpBox("Copyright laws for music are VERY STRICT and MUST BE respected!" + System.Environment.NewLine + "If you save music, make sure YOU have the RIGHT to do so! " + System.Environment.NewLine + "crosstales LLC denies any responsibility for YOUR actions with this tool - use it at your OWN RISK!" + System.Environment.NewLine + System.Environment.NewLine + "For more, see 'https://en.wikipedia.org/wiki/Radio_music_ripping' and the rights applying to your country.", MessageType.Error);
            }
            else
            {
                EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
            }
        }
        #endregion

    }
}
// © 2017 crosstales LLC (https://www.crosstales.com)