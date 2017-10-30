using UnityEngine;
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace Crosstales.Radio.Demo.Util
{
    /// <summary>Very simple scene switcher.</summary>
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_demo_1_1_util_1_1_scene_switcher.html")]
    public class SceneSwitcher : MonoBehaviour
    {

        public int Index = 0;

        /// <summary>Switches the scene to the given index.</summary>
        public void Switch()
        {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene(Index);
#else
         Application.LoadLevel(Index);
#endif
        }
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)