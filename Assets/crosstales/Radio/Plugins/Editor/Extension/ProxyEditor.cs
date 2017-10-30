using UnityEngine;
using UnityEditor;
using Crosstales.Radio.EditorUtil;

namespace Crosstales.Radio.EditorExtension
{
    /// <summary>Custom editor for the 'Proxy'-class.</summary>
    [CustomEditor(typeof(Tool.Proxy))]
    public class ProxyEditor : Editor
    {

        #region Variables

        private Tool.Proxy script;

        #endregion


        #region Editor methods

        public void OnEnable()
        {
            script = (Tool.Proxy)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorHelper.SeparatorUI();

            if (script.isActiveAndEnabled)
            {
                if (Util.Helper.isEditorMode)
                {
                    GUILayout.Label("HTTP-Proxy:", EditorStyles.boldLabel);

                    if (Tool.Proxy.hasHTTPProxy)
                    {
                        if (GUILayout.Button(new GUIContent(" Disable", EditorHelper.Icon_Minus, "Disable HTTP-Proxy.")))
                        {
                            script.DisableHTTPProxy();
                            GAApi.Event(typeof(ProxyEditor).Name, "Disable HTTP-Proxy");
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(new GUIContent(" Enable", EditorHelper.Icon_Plus, "Enable HTTP-Proxy.")))
                        {
                            script.EnableHTTPProxy();
                            GAApi.Event(typeof(ProxyEditor).Name, "Enable HTTP-Proxy");
                        }
                    }

                    GUILayout.Space(8);

                    GUILayout.Label("HTTPS-Proxy:", EditorStyles.boldLabel);

                    if (Tool.Proxy.hasHTTPSProxy)
                    {
                        if (GUILayout.Button(new GUIContent(" Disable", EditorHelper.Icon_Minus, "Disable HTTPS-Proxy.")))
                        {
                            script.DisableHTTPSProxy();
                            GAApi.Event(typeof(ProxyEditor).Name, "Disable HTTPS-Proxy");
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(new GUIContent(" Enable", EditorHelper.Icon_Plus, "Enable HTTPS-Proxy.")))
                        {
                            script.EnableHTTPSProxy();
                            GAApi.Event(typeof(ProxyEditor).Name, "Enable HTTPS-Proxy");
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
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
// © 2017 crosstales LLC (https://www.crosstales.com)