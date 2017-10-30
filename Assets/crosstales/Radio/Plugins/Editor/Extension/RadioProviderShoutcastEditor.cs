using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorExtension
{
    /// <summary>Custom editor for the 'RadioProviderShoutcast'-class.</summary>
    [CustomEditor(typeof(Provider.RadioProviderShoutcast))]
    public class RadioProviderShoutcastEditor : BaseRadioProviderEditor
    {
        #region Variables

        private Provider.RadioProviderShoutcast _script;

        #endregion


        #region Editor methods

        public override void OnEnable()
        {
            base.OnEnable();
            _script = (Provider.RadioProviderShoutcast)target;
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