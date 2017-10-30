﻿using UnityEngine;

namespace Crosstales.Radio.Demo.Util
{
    /// <summary>Enables or disable game objects for a given platform.</summary>
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_demo_1_1_util_1_1_platform_controller.html")]
    public class PlatformController : MonoBehaviour
    {

        #region Variables

        [Header("Configuration")]

        ///<summary>Selected platforms for the controller.</summary>
        [Tooltip("Selected platforms for the controller.")]
        public System.Collections.Generic.List<Platform> Platforms;

        ///<summary>Enable or disable the 'Objects' for the selected 'Platforms' (default: true).</summary>
        [Tooltip("Enable or disable the 'Objects' for the selected 'Platforms' (default: true).")]
        public bool Active = true;


        [Header("Objects")]
        ///<summary>Selected objects for the controller.</summary>
        [Tooltip("Selected objects for the controller.")]
        public GameObject[] Objects;

        private Platform currentPlatform;

        #endregion


        #region MonoBehaviour methods

        public void Start()
        {
            if (Radio.Util.Helper.isWindowsPlatform)
            {
                currentPlatform = Platform.Windows;
            }
            else if (Radio.Util.Helper.isMacOSPlatform)
            {
                currentPlatform = Platform.OSX;
            }
            else if (Radio.Util.Helper.isAndroidPlatform)
            {
                currentPlatform = Platform.Android;
            }
            else if (Radio.Util.Helper.isIOSPlatform)
            {
                currentPlatform = Platform.IOS;
            }
            else if (Radio.Util.Helper.isWSAPlatform)
            {
                currentPlatform = Platform.WSA;
            }
            else if (Radio.Util.Helper.isWebPlatform) 
            {
                currentPlatform = Platform.Web;
            }
            else //should never happen - fallback setting
            {
                currentPlatform = Platform.Unsupported;
            }

            //Debug.Log (currentPlatform);

            bool active = Platforms.Contains(currentPlatform) ? Active : !Active;

            foreach (GameObject go in Objects)
            {
                if (go != null) {
                    go.SetActive(active);
                }
            }
        }

        #endregion

    }


    #region Enumeration

    /// <summary>All available platforms.</summary>
    public enum Platform
    {
        OSX,
        Windows,
        IOS,
        Android,
        WSA,
        MaryTTS,
        Web,
        Unsupported
    }

    #endregion
}
// © 2017 crosstales LLC (https://www.crosstales.com)