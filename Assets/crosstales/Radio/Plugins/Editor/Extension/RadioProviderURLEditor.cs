using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorExtension
{
    /// <summary>Custom editor for the 'RadioProviderURL'-class.</summary>
    [CustomEditor(typeof(Provider.RadioProviderURL))]
    public class RadioProviderURLEditor : BaseRadioProviderEditor
    {
        #region Variables

        private Provider.RadioProviderURL _script;

        #endregion


        #region Editor methods

        public override void OnEnable()
        {
            base.OnEnable();
            _script = (Provider.RadioProviderURL)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorHelper.SeparatorUI();

            if (_script.isActiveAndEnabled)
            {
                if (_script.Entries != null && _script.Entries.Count > 0)
                {
                    showData();
                }
                else
                {
                    EditorGUILayout.HelpBox("Please add an Entry!", MessageType.Warning);
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