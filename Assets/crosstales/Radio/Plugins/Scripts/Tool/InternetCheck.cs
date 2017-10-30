using UnityEngine;
using System.Collections;

namespace Crosstales.Radio.Tool
{
    /// <summary>Checks the Internet availabilty.</summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_tool_1_1_internet_check.html")]
    public class InternetCheck : MonoBehaviour
    {

        #region Variables

        /// <summary>Optimized check routines (default: true).</summary>
        [Tooltip("Optimized check routines (default: true)")]
        public bool Optimized = true;

        private const float checkIntervalMin = 4f;
        private const float checkIntervalMax = 10f;

        private const bool runOnStart = true;
        private const bool endlessMode = true;

        private static bool internetAvailable = false;

        private static GameObject go;
        private static InternetCheck instance;
        private static bool initalized = false;
        private static bool loggedOnlyOneInstance = false;

        private bool isRunning = false;

        private float checkTime = 9999f;
        private float checkTimeCounter = 0f;

        private float burstTime = 9999f;
        private const float burstIntervalMin = 1f;
        private const float burstIntervalMax = 3f;
        private float burstTimeCounter = 0f;

        private static float lastCheckTime = 0f;

        #endregion


        #region Properties

        /// <summary>Checks if a Internet connection is available.</summary>
        /// <returns>True if a Internet connection is available.</returns>
        public static bool isInternetAvailable
        {
            get
            {
                if (instance == null)
                {
                    return Application.internetReachability != NetworkReachability.NotReachable;
                }
                else
                {
                    return internetAvailable;
                }
            }
        }

        //// <summary>Last Internet check.</summary>
        public static System.DateTime LastCheck
        {
            get;
            private set;
        }

        //// <summary>Number of Internet checks.</summary>
        public static int CheckCounter
        {
            get;
            private set;
        }

        //// <summary>Downloaded data in bytes.</summary>
        public static int DownloadedData
        {
            get;
            private set;
        }

        #endregion


        #region MonoBehaviour methods

        public void OnEnable()
        {
            if (Util.Helper.isEditorMode || !initalized)
            {
                go = gameObject;

                go.name = Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME;

                instance = this;

                if (runOnStart)
                {
                    StartCoroutine(internetCheck());
                }

                if (!Util.Helper.isEditorMode && Util.Config.DONT_DESTROY_ON_LOAD)
                {
                    DontDestroyOnLoad(transform.root.gameObject);

                    initalized = true;
                }

                if (Util.Constants.DEV_DEBUG)
                    Debug.Log("Using new instance!");
            }
            else
            {
                if (!Util.Helper.isEditorMode && Util.Config.DONT_DESTROY_ON_LOAD && instance != this)
                {
                    if (!loggedOnlyOneInstance)
                    {
                        Debug.LogWarning("Only one active instance of '" + Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME + "' allowed in all scenes!" + System.Environment.NewLine + "This object will now be destroyed.");

                        loggedOnlyOneInstance = true;
                    }

                    Destroy(gameObject, 0.2f);
                }

                if (Util.Constants.DEV_DEBUG)
                    Debug.Log("Using old instance!");
            }
        }

        public void Update()
        {
            if (Util.Helper.isEditorMode)
            {
                if (go != null)
                {
                    go.name = Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME; //ensure name
                }
            }

            if (endlessMode && !isRunning)
            {
                checkTimeCounter += Time.deltaTime;
                burstTimeCounter += Time.deltaTime;

                if (isInternetAvailable)
                {
                    if (checkTimeCounter > checkTime)
                    {
                        if (Util.Constants.DEV_DEBUG)
                            Debug.Log("Normal-Mode!");

                        checkTimeCounter = 0f;
                        burstTimeCounter = 0f;

                        StartCoroutine(internetCheck());
                    }
                }
                else
                {
                    if (burstTimeCounter > burstTime)
                    {
                        if (Util.Constants.DEV_DEBUG)
                            Debug.Log("Burst-Mode!");

                        checkTimeCounter = 0f;
                        burstTimeCounter = 0f;

                        StartCoroutine(internetCheck());
                    }
                }
            }
        }

        public void OnApplicationQuit()
        {
            if (instance != null)
            {
                instance.StopAllCoroutines();
            }
        }

        #endregion


        #region Public methods

        public static void Refresh()
        {
            //NEW
            if (instance != null && !instance.isRunning && lastCheckTime + 1f < Time.realtimeSinceStartup)
            {
                instance.StartCoroutine(instance.internetCheck());
            }
        }

        #endregion


        #region Private methods

        private IEnumerator internetCheck()
        {
            if (!isRunning)
            {
                isRunning = true;
                CheckCounter++;

                WWW www;
                bool available = false;


                if (Util.Helper.isWebPlatform || Util.Helper.isEditorMode)
                {
                    available = Application.internetReachability != NetworkReachability.NotReachable;
                }
                else
                {
                    if ((Optimized || Util.Helper.isWindowsBasedPlatform) && !Util.Helper.isAppleBasedPlatform)
                    {
                        if (Util.Config.DEBUG)
                            Debug.Log("Testing the Internet availability for Optimized/Windows-based systems: " + System.DateTime.Now);

                        using (www = new WWW(Util.Constants.INTERNET_CHECK_URL_WINDOWS))
                        {
                            do
                            {
                                yield return www;
                            } while (!www.isDone);

                            if (string.IsNullOrEmpty(www.error))
                            {
                                if (Util.Constants.DEV_DEBUG)
                                    Debug.Log("Content for Windows-based systems: " + www.text);

                                available = !string.IsNullOrEmpty(www.text) && www.text.CTEquals("Microsoft NCSI");
                                DownloadedData += www.bytesDownloaded;
                            }
                            else
                            {
                                if (Util.Constants.DEV_DEBUG)
                                    Debug.LogError("Error getting content for Windows-based systems: " + www.error);
                            }
                        }
                    }

                    if (Util.Helper.isAppleBasedPlatform)
                    {
                        if (Util.Config.DEBUG)
                            Debug.Log("Testing the Internet availability for Apple-based systems: " + System.DateTime.Now);

                        using (www = new WWW(Util.Constants.INTERNET_CHECK_URL_APPLE))
                        {
                            do
                            {
                                yield return www;
                            } while (!www.isDone);

                            if (string.IsNullOrEmpty(www.error))
                            {
                                if (Util.Constants.DEV_DEBUG)
                                    Debug.Log("Content for Apple-based systems: " + www.text);

                                available = !string.IsNullOrEmpty(www.text) && www.text.CTContains("<TITLE>Success</TITLE>");
                                DownloadedData += www.bytesDownloaded;
                            }
                            else
                            {
                                if (Util.Constants.DEV_DEBUG)
                                    Debug.LogError("Error getting content for Apple-based systems: " + www.error);
                            }
                        }
                    }

                    // default check
                    if (!available)
                    {
                        if (Util.Config.DEBUG)
                            Debug.Log("Testing with default Internet availability: " + System.DateTime.Now);

                        using (www = new WWW(Util.Constants.INTERNET_CHECK_URL))
                        {
                            do
                            {
                                yield return www;
                            } while (!www.isDone);

                            if (string.IsNullOrEmpty(www.error))
                            {
                                if (Util.Constants.DEV_DEBUG)
                                    Debug.Log("Content for default: " + www.text);

                                available = !string.IsNullOrEmpty(www.text) && www.text.CTContains("<TITLE>Lorem Ipsum</TITLE>");
                                DownloadedData += www.bytesDownloaded;
                            }
                            else
                            {
                                if (Util.Constants.DEV_DEBUG)
                                    Debug.LogError("Error getting content for default: " + www.error);
                            }
                        }
                    }

                    // fallback check
                    if (!available)
                    {
                        if (Util.Config.DEBUG)
                            Debug.Log("Testing with fallback Internet availability: " + System.DateTime.Now);

                        using (www = new WWW(Util.Constants.INTERNET_CHECK_URL_FALLBACK))
                        {
                            do
                            {
                                yield return www;
                            } while (!www.isDone);

                            if (string.IsNullOrEmpty(www.error))
                            {
                                if (Util.Constants.DEV_DEBUG)
                                    Debug.Log("Content for fallback: " + www.text);

                                available = !string.IsNullOrEmpty(www.text) && www.text.CTEquals("crosstales rulez!");
                                DownloadedData += www.bytesDownloaded;
                            }
                            else
                            {
                                if (Util.Constants.DEV_DEBUG)
                                    Debug.LogError("Error getting content for fallback: " + www.error);
                            }
                        }
                    }
                }

                internetAvailable = available;

                checkTime = Random.Range(checkIntervalMin, checkIntervalMax);
                burstTime = Random.Range(burstIntervalMin, burstIntervalMax);
                LastCheck = System.DateTime.Now;
                lastCheckTime = Time.realtimeSinceStartup;

                isRunning = false;
            }
            else
            {
                Debug.LogError("Internet check already running!");
            }
        }

        #endregion

    }
}
// © 2017 crosstales LLC (https://www.crosstales.com)