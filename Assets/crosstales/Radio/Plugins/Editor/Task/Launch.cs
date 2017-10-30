using UnityEngine;
using UnityEditor;

namespace Crosstales.Radio.EditorTask
{
    /// <summary>Show the configuration window on the first launch.</summary>
    [InitializeOnLoad]
    public static class Launch
    {

        #region Constructor

        static Launch()
        {
            bool launched = EditorPrefs.GetBool(Util.Constants.KEY_LAUNCH);
            //bool launched = false;
 
            if (!launched) {
                EditorIntegration.ConfigWindow.ShowWindow(4);
                EditorPrefs.SetBool(Util.Constants.KEY_LAUNCH, true);
            }
        }

        #endregion
    }
}
// © 2017 crosstales LLC (https://www.crosstales.com)