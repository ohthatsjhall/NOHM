using UnityEngine;
using UnityEngine.UI;
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using Crosstales.Radio.Util;
using Crosstales.Radio.Tool;
using Crosstales.Radio.Demo.Util;

namespace Crosstales.Radio.Demo
{
    /// <summary>Main GUI for all demo scenes.</summary>
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_demo_1_1_g_u_i_main.html")]
    public class GUIMain : MonoBehaviour
    {
        #region Variables

        [Header("Settings")]
        /// <summary>'RadioPlayer' from the scene (optional).</summary>
        [Tooltip("'RadioPlayer' from the scene (optional).")]
        public RadioPlayer Player;

        /// <summary>'RadioManager' from the scene (optional).</summary>
        [Tooltip("'RadioManager' from the scene (optional).")]
        public RadioManager Manager;

        /// <summary>'Orbit'-object from the scene (optional).</summary>
        [Tooltip("'Orbit'-object from the scene (optional).")]
        public Orbit Orbit;

        [Header("UI Objects")]
        public GameObject RadioPanel;

        public Text Name;
        public Text Version;
        public Text Scene;

        public GameObject InternetNotAvailable;
        public GameObject AudioPanel;
        public GameObject FilterPanel;
        public GameObject SpectrumPanel;
        public GameObject Spectrum;
        public GameObject Visuals;
        public Toggle FullscreenToogle;
        public Text DownloadSize;
        public Text ElapsedTotalTime;

        [Header("Scene-Link")]
        public int IndexPreviousScene;
        public int IndexNextScene;

        private float delayCount = 1f;

        #endregion


        #region MonoBehaviour methods

        public void Start()
        {
            //InternetCheck.OnInternetStatusChange += onInternetStatusChange;

            if (FullscreenToogle != null && !Screen.fullScreen)
            {
                FullscreenToogle.isOn = false;
            }

            if (Name != null)
            {
                Name.text = Constants.ASSET_NAME;
            }

            if (Version != null)
            {
                Version.text = Constants.ASSET_VERSION;
            }

            if (DownloadSize != null)
            {
                DownloadSize.text = Helper.FormatBytesToHRF(Context.TotalDataSize);
            }

            if (ElapsedTotalTime != null)
            {
                ElapsedTotalTime.text = Helper.FormatSecondsToHourMinSec(Context.TotalPlayTime);
            }

            if (Scene != null)
            {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
                Scene.text = SceneManager.GetActiveScene().name;
#else
            Scene.text = Application.loadedLevelName;
#endif
            }

            //InternetNotAvailable.SetActive(!Helper.isInternetAvailable);
        }

        public void Update()
        {
            delayCount += Time.deltaTime;

            if (delayCount > 1f)
            {
                delayCount = 0f;

                if (DownloadSize != null)
                {
                    DownloadSize.text = Helper.FormatBytesToHRF(Context.TotalDataSize);
                }

                if (ElapsedTotalTime != null)
                {
                    ElapsedTotalTime.text = Helper.FormatSecondsToHourMinSec(Context.TotalPlayTime);
                }
            }

            InternetNotAvailable.SetActive(!InternetCheck.isInternetAvailable);
        }

        //public void OnDestroy()
        //{
        //    InternetCheck.OnInternetStatusChange -= onInternetStatusChange;
        //}

        //private void onInternetStatusChange(bool isConnected)
        //{
        //    InternetNotAvailable.SetActive(!isConnected);
        //}

        #endregion


        #region Public methods

        public void AudioPanelEnabled(bool val)
        {
            if (AudioPanel != null)
            {
                AudioPanel.SetActive(val);
            }
        }

        public void FilterPanelEnabled(bool val)
        {
            if (FilterPanel != null)
            {
                FilterPanel.SetActive(val);
            }
        }

        public void RadioPanelEnabled(bool val)
        {
            if (RadioPanel != null)
            {
                RadioPanel.SetActive(val);
            }
        }

        public void SpectrumEnabled(bool val)
        {
            if (SpectrumPanel != null)
            {
                SpectrumPanel.SetActive(val);
            }

            if (Spectrum != null)
            {
                Spectrum.SetActive(val);
            }
        }

        public void VisualsEnabled(bool val)
        {
            if (Visuals != null)
            {
                Visuals.SetActive(val);
            }
        }

        public void OrbitEnabled(bool val)
        {
            if (Orbit != null)
            {
                Orbit.enabled = val;
            }
        }

        public void FullscreenEnabled(bool val)
        {
            Screen.fullScreen = val;
        }

        public void OpenAssetURL()
        {
            Application.OpenURL(Constants.ASSET_CT_URL);
        }

        public void OpenCTURL()
        {
            Application.OpenURL(Constants.ASSET_AUTHOR_URL);
        }

        public void Open1FM()
        {
            Application.OpenURL("http://www.1.fm/");
        }

        public void PreviousScene()
        {
            if (Manager != null)
            {
                Manager.StopAll();
            }
            if (Player != null)
            {
                Player.Stop();
            }

            //this.CTInvoke(() => previousScene(), Constants.INVOKE_DELAY);
            Invoke("previousScene", Constants.INVOKE_DELAY);
        }

        public void NextScene()
        {
            if (Manager != null)
            {
                Manager.StopAll();
            }
            if (Player != null)
            {
                Player.Stop();
            }

            //this.CTInvoke(() => nextScene(), Constants.INVOKE_DELAY);
            Invoke("nextScene", Constants.INVOKE_DELAY);
        }

        public void Quit()
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }
        }

        #endregion


        #region Private methods

        private void previousScene()
        {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene(IndexPreviousScene);
#else
         Application.LoadLevel(IndexPreviousScene);
#endif
        }

        private void nextScene()
        {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene(IndexNextScene);
#else
         Application.LoadLevel(IndexNextScene);
#endif

        }

        #endregion
    }
}
// © 2015-2017 crosstales LLC (https://www.crosstales.com)