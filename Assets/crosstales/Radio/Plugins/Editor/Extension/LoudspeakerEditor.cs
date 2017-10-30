using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorExtension
{
    /// <summary>Custom editor for the 'Loudspeaker'-class.</summary>
    [CustomEditor(typeof(Tool.Loudspeaker))]
    public class LoudspeakerEditor : Editor
    {

        #region Variables

        private Tool.Loudspeaker script;

        #endregion


        #region Editor methods

        public void OnEnable()
        {
            script = (Tool.Loudspeaker)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            //EditorHelper.SeparatorUI();

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
            }
            else
            {
                EditorHelper.SeparatorUI();
                EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
            }
        }
        
        #endregion

    }
}
// © 2017 crosstales LLC (https://www.crosstales.com)