using UnityEngine;
using UnityEngine.UI;
using Crosstales.Radio.Util;
using Crosstales.Radio.Model;

namespace Crosstales.Radio.Demo
{
    /// <summary>GUI for a very simple radio player.</summary>
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_demo_1_1_g_u_i_play_radio.html")]
    public class GUIPlayRadio : MonoBehaviour
    {

        #region Variables

        [Header("Settings")]
        /// <summary>'SimplePlayer' from the scene.</summary>
        [Tooltip("'SimplePlayer' from the scene.")]
        public SimplePlayer Player;

        /// <summary>The color for the Play-mode.</summary>
        [Tooltip("The color for the Play-mode.")]
        public Color32 PlayColor = new Color32(0, 255, 0, 64);

        /// <summary>How many times should the radio station restart after an error before giving up (default: 3).</summary>
        [Tooltip("How many times should the radio station restart after an error before giving up (default: 3).")]
        public int Retries = 3;
/*
        /// <summary>Plays the stations in random order (default: false).</summary>
        [Tooltip("Plays the stations in random order (default: false).")]
        public bool PlayRandom = true;
*/
        [Header("UI Objects")]
        public GameObject PlayButton;
        public GameObject StopButton;
        public Image MainImage;
        public Text Station;
        public Text ElapsedTime;
        public Text ErrorText;
        public Text ElapsedRecordTime;
        public Text RecordTitle;
        public Text RecordArtist;
        public Text DownloadSizeStation;
        public Text ElapsedStationTime;
        public Text NextRecordTitle;
        public Text NextRecordArtist;
        public Text NextRecordDelay;

        //      [Header("Behaviour Settings")]
        //      [Tooltip("Play the radio on start (default: off).")]
        //      public bool PlayOnStart = false;

        private int invokeDelayCounter = 1;
        private bool isStopped = true;
        private Color32 color;
        private int playtime = 0;
        private int recordPlaytime = 0;

        private RecordInfo currentRecord;
        private RecordInfo nextRecord;

        #endregion


        #region MonoBehaviour methods

        public void Start()
        {
            if (Player != null && Player.Player != null)
            {
                // Subscribe event listeners
                Player.OnPlaybackStart += onPlaybackStart;
                Player.OnPlaybackEnd += onPlaybackEnd;
                Player.OnAudioStart += onAudioStart;
                Player.OnAudioEnd += onAudioEnd;
                Player.OnAudioPlayTimeUpdate += onAudioPlayTimeUpdate;
                Player.OnBufferingProgressUpdate += onBufferingProgressUpdate;
                Player.OnErrorInfo += onErrorInfo;
                Player.OnRecordChange += onRecordChange;
                Player.OnRecordPlayTimeUpdate += onRecordPlayTimeUpdate;
                Player.OnNextRecordChange += onNextRecordChange;
                Player.OnNextRecordDelayUpdate += onNextRecordDelayUpdate;

                ErrorText.text = string.Empty;
            }
            else
            {
                string msg = "'Player' is null!";
                ErrorText.text = msg;
                Debug.LogError(msg);
            }

            if (ElapsedTime != null)
            {
                ElapsedTime.text = Constants.TEXT_STOPPED;
            }

            Station.text = Constants.TEXT_QUESTIONMARKS;
            color = MainImage.color;

            //         if (PlayOnStart) {
            //            //Invoke("Play", 0.1f);
            //            Play();
            //         } else {
            if (Player != null)
            {
                PlayButton.SetActive(!Player.isPlayback);
                StopButton.SetActive(Player.isPlayback);

                if (Player.isPlayback)
                {
                    MainImage.color = PlayColor;
                }
            }
            //         }

            onPlaybackEnd(null); //initalize GUI-components
        }

        public void Update()
        {
            if (Time.frameCount % 30 == 0 && Player != null && Player.Station != null && Player.isPlayback)
            {
                Station.text = Player.Station.Name;

                if (nextRecord != null && nextRecord.Equals(currentRecord))
                {
                    if (NextRecordTitle != null)
                    {
                        NextRecordTitle.text = string.Empty;
                    }

                    if (NextRecordArtist != null)
                    {
                        NextRecordArtist.text = string.Empty;
                    }
                }
            }
        }

        public void OnDestroy()
        {
            if (Player != null && Player.Player != null)
            {
                // Unsubscribe event listeners
                Player.OnPlaybackStart -= onPlaybackStart;
                Player.OnPlaybackEnd -= onPlaybackEnd;
                Player.OnAudioStart -= onAudioStart;
                Player.OnAudioEnd -= onAudioEnd;
                Player.OnAudioPlayTimeUpdate -= onAudioPlayTimeUpdate;
                Player.OnBufferingProgressUpdate -= onBufferingProgressUpdate;
                Player.OnErrorInfo -= onErrorInfo;
                Player.OnRecordChange -= onRecordChange;
                Player.OnRecordPlayTimeUpdate -= onRecordPlayTimeUpdate;
                Player.OnNextRecordChange -= onNextRecordChange;
                Player.OnNextRecordDelayUpdate -= onNextRecordDelayUpdate;
            }
        }

        #endregion


        #region Public methods

        public void Play()
        {
            if (Player != null)
            {
                ErrorText.text = string.Empty;

                Player.Play(Player.PlayRandom);

                if (Player.Station != null)
                {
                    Station.text = Player.Station.Name;
                }
            }
        }

        public void Stop()
        {
            if (Player != null)
            {
                Player.Stop();
            }

            Station.text = Constants.TEXT_QUESTIONMARKS;
        }

        public void OpenUrl()
        {
            if (Player != null && Player.Player != null)
            {
                Application.OpenURL(Player.Player.Station.Station);
            }
        }

        public void OpenSpotifyUrl()
        {
            if (Player != null && Player.Player != null)
            {
                Application.OpenURL(Player.Player.RecordInfo.SpotifyUrl);
            }
        }

        #endregion


        #region Callback methods

        private void onPlaybackStart(Model.RadioStation station)
        {
            PlayButton.SetActive(false);
            StopButton.SetActive(true);

            MainImage.color = PlayColor;
        }

        private void onPlaybackEnd(Model.RadioStation station)
        {

            if (ElapsedTime != null)
            {
                ElapsedTime.text = Constants.TEXT_STOPPED;
            }

            if (ElapsedRecordTime != null)
            {
                ElapsedRecordTime.text = Helper.FormatSecondsToHourMinSec(0f);
            }

            if (ElapsedStationTime != null)
            {
                ElapsedStationTime.text = Helper.FormatSecondsToHourMinSec(0f);
            }

            if (DownloadSizeStation != null)
            {
                DownloadSizeStation.text = Helper.FormatBytesToHRF(0);
            }

            if (RecordTitle != null)
            {
                RecordTitle.text = string.Empty;
            }

            if (RecordArtist != null)
            {
                RecordArtist.text = string.Empty;
            }

            if (NextRecordTitle != null)
            {
                NextRecordTitle.text = string.Empty;
            }

            if (NextRecordArtist != null)
            {
                NextRecordArtist.text = string.Empty;
            }

            if (NextRecordDelay != null)
            {
                NextRecordDelay.text = string.Empty;
            }

            PlayButton.SetActive(true);
            StopButton.SetActive(false);

            MainImage.color = color;

            if (ElapsedTime != null)
            {
                ElapsedTime.text = Constants.TEXT_STOPPED;
            }
        }

        private void onAudioStart(Model.RadioStation station)
        {
            isStopped = false;
        }

        private void onAudioEnd(Model.RadioStation station)
        {
            isStopped = true;
        }

        private void onAudioPlayTimeUpdate(Model.RadioStation station, float playtime)
        {
            if ((int)playtime != this.playtime)
            {
                if (ElapsedTime != null)
                {
                    ElapsedTime.text = Helper.FormatSecondsToHourMinSec(playtime);
                }

                if (DownloadSizeStation != null)
                {
                    DownloadSizeStation.text = Helper.FormatBytesToHRF(station.TotalDataSize);
                }

                if (ElapsedStationTime != null)
                {
                    ElapsedStationTime.text = Helper.FormatSecondsToHourMinSec(station.TotalPlayTime);
                }

                //Debug.Log("Download per second: " + Helper.FormatBytesToHRF((long)(Context.TotalDataSize / Context.TotalPlayTime)));

                if (playtime > 30f)
                {
                    invokeDelayCounter = 1;
                }

                this.playtime = (int)playtime;
            }
        }

        private void onBufferingProgressUpdate(Model.RadioStation station, float progress)
        {
            if (ElapsedTime != null)
            {
                ElapsedTime.text = Constants.TEXT_BUFFER + progress.ToString(Constants.FORMAT_PERCENT);
            }
        }

        private void onErrorInfo(Model.RadioStation station, string info)
        {
            Stop();
            onPlaybackEnd(station);

            if (!isStopped)
            {
                if (invokeDelayCounter < Retries)
                {
                    Debug.LogWarning("Error occoured -> Restarting station." + System.Environment.NewLine + info);
                    //this.CTInvoke(() => play(), Constants.INVOKE_DELAY * invokeDelayCounter);
                    Invoke("play", Constants.INVOKE_DELAY * invokeDelayCounter);
                    
                    invokeDelayCounter++;
                }
                else
                {
                    string msg = "Restarting station failed more than " + Retries + " times - giving up!" + System.Environment.NewLine + info;
                    ErrorText.text = msg;
                    Debug.LogError(msg);
                }
            }
            else
            {
                if (ErrorText != null)
                {
                    ErrorText.text = info;
                }
            }
        }

        private void onRecordChange(Model.RadioStation station, RecordInfo record)
        {
            currentRecord = record;

            if (RecordTitle != null)
            {
                RecordTitle.text = record.Title;
            }

            if (RecordArtist != null)
            {
                RecordArtist.text = record.Artist;
            }

            if (NextRecordDelay != null)
            {
                NextRecordDelay.text = string.Empty;
            }
        }

        private void onRecordPlayTimeUpdate(RadioStation station, RecordInfo record, float playtime)
        {

            if ((int)playtime != recordPlaytime)
            {
                recordPlaytime = (int)playtime;

                if (ElapsedRecordTime != null)
                {
                    ElapsedRecordTime.text = Helper.FormatSecondsToHourMinSec(playtime);
                }
            }
        }

        private void onNextRecordChange(Model.RadioStation station, RecordInfo record, float delay)
        {
            nextRecord = record;

            if (NextRecordTitle != null)
            {
                NextRecordTitle.text = record.Title;
            }

            if (NextRecordArtist != null)
            {
                NextRecordArtist.text = record.Artist;
            }
        }

        private void onNextRecordDelayUpdate(Model.RadioStation station, RecordInfo record, float delay)
        {
            if (NextRecordDelay != null)
            {
                NextRecordDelay.text = delay.ToString("#0.0");
            }
        }

        #endregion


        #region Private methods

        private void play()
        {
            if (!isStopped)
            {
                Play();
            }
        }

        #endregion
    }
}
// © 2015-2017 crosstales LLC (https://www.crosstales.com)