using UnityEngine;
using UnityEngine.UI;
using Crosstales.Radio.Model;
using Crosstales.Radio.Util;
using Crosstales.Radio.Tool;

namespace Crosstales.Radio.Demo
{
    /// <summary>GUI for a very simple normal/random radio station player.</summary>
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_demo_1_1_g_u_i_play_random_station.html")]
    public class GUIPlayRandomStation : MonoBehaviour
    {

        #region Variables

        [Header("Settings")]
        /// <summary>'SimplePlayer' from the scene.</summary>
        [Tooltip("'SimplePlayer' from the scene.")]
        public SimplePlayer Player;
/*
        /// <summary>Random order the stations (default: true).</summary>
        [Tooltip("Random order the stations (default: true).")]
        public bool Random = true;
*/
        /// <summary>The color for the Play-mode.</summary>
        [Tooltip("The color for the Play-mode.")]
        public Color32 PlayColor = new Color32(0, 255, 0, 64);

        [Header("UI Objects")]
        public Button NextButton;
        public Button PreviousButton;
        public Button PlayButton;
        public Button StopButton;
        public Image MainImage;
        public Text Station;
        public Text ElapsedTime;
        public Text StationsNumberText;
        public Text ErrorText;
        public Text ElapsedRecordTime;
        public Text RecordTitle;
        public Text RecordArtist;
        public Text DownloadSizeStation;
        public Text ElapsedStationTime;
        public Text NextRecordTitle;
        public Text NextRecordArtist;
        public Text NextRecordDelay;

        private Color32 color;
        private bool isDirectionNext = true;
        private bool isStopped = true;
        private int playtime = 0;
        private int recordPlaytime = 0;

        private RadioFilter filter = new RadioFilter();

        private int invokeDelayCounter = 1;

        private RecordInfo currentRecord;
        private RecordInfo nextRecord;

        #endregion


        #region MonoBehaviour methods

        public void Start()
        {
            color = MainImage.color;
            ElapsedTime.text = "Loading...";

            if (PlayButton != null)
            {
                PlayButton.interactable = false;
            }

            if (NextButton != null)
            {
                NextButton.interactable = false;
            }

            if (PreviousButton != null)
            {
                PreviousButton.interactable = false;
            }

            if (StopButton != null)
            {
                StopButton.interactable = false;
            }

            if (Player != null && Player.Player != null)
            {
                // Subscribe event listeners
                Player.OnPlaybackStart += onPlaybackStart;
                Player.OnPlaybackEnd += onPlaybackEnd;
                Player.OnAudioPlayTimeUpdate += onAudioPlayTimeUpdate;
                Player.OnBufferingProgressUpdate += onBufferingProgressUpdate;
                Player.OnErrorInfo += onErrorInfo;
                Player.OnProviderReady += onProviderReady;
                Player.OnRecordChange += onRecordChange;
                Player.OnRecordPlayTimeUpdate += onRecordPlayTimeUpdate;
                Player.OnNextRecordChange += onNextRecordChange;
                Player.OnNextRecordDelayUpdate += onNextRecordDelayUpdate;

                if (ErrorText != null)
                {
                    ErrorText.text = string.Empty;
                }
            }
            else
            {
                string msg = "'Player' is null!";

                Debug.LogError(msg);

                if (ErrorText != null)
                {
                    ErrorText.text = msg;
                }
            }

            onPlaybackEnd(null); //initalize GUI-components
        }

        public void Update()
        {
            if (Time.frameCount % 20 == 0 && Player != null && Player.Manager != null)
            {
                if (StationsNumberText != null)
                {
                    StationsNumberText.text = Player.Manager.CountStations(filter).ToString();
                }

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

        void OnDestroy()
        {
            if (Player != null && Player.Player != null)
            {
                // Unsubscribe event listeners
                Player.OnPlaybackStart -= onPlaybackStart;
                Player.OnPlaybackEnd -= onPlaybackEnd;
                Player.OnAudioPlayTimeUpdate -= onAudioPlayTimeUpdate;
                Player.OnBufferingProgressUpdate -= onBufferingProgressUpdate;
                Player.OnErrorInfo -= onErrorInfo;
                Player.OnProviderReady -= onProviderReady;
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
            if (InternetCheck.isInternetAvailable)
            {
                isDirectionNext = true;

                if (Player != null)
                {
                    isStopped = false;

                    Player.Play(Player.PlayRandom, filter);
                }
            }
            else
            {
                Debug.LogError("No internet connection available! Can't play (stream) any stations!");
            }
        }
        
        public void Next()
        {
            if (InternetCheck.isInternetAvailable)
            {
                isDirectionNext = true;

                if (Player != null)
                {
                    isStopped = false;

                    Player.Next(Player.PlayRandom, filter);
                }
            }
            else
            {
                Debug.LogError("No internet connection available! Can't play (stream) any stations!");
            }
        }
        
        public void Previous()
        {
            if (InternetCheck.isInternetAvailable)
            {
                isDirectionNext = false;

                if (Player != null)
                {
                    isStopped = false;

                    Player.Previous(Player.PlayRandom, filter);
                }
            }
            else
            {
                Debug.LogError("No internet connection available! Can't play (stream) any stations!");
            }
        }

        public void Stop()
        {
            isStopped = true;

            stop();
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
/*
        public void RandomPlay(bool random)
        {
            Random = random;
        }
*/
        public void FilterGenre(string filter)
        {
            this.filter.Genres = filter;
        }

        public void FilterRatingMin(string rating)
        {
            float.TryParse(rating, out filter.RatingMin);
        }

        public void FilterRatingMax(string rating)
        {
            float.TryParse(rating, out filter.RatingMax);
        }

        #endregion


        #region Callback methods

        private void onPlaybackStart(RadioStation station)
        {
            MainImage.color = PlayColor;

            if (PlayButton != null)
            {
                PlayButton.interactable = false;
            }
            
            if (StopButton != null)
            {
                StopButton.interactable = true;
            }
            
            if (ErrorText != null)
            {
                ErrorText.text = string.Empty;
            }

            if (Player.Station != null)
            {
                Station.text = station.Name;
            }
        }

        private void onPlaybackEnd(RadioStation station)
        {
            MainImage.color = color;
            Station.text = Constants.TEXT_QUESTIONMARKS;

            if (PlayButton != null)
            {
                PlayButton.interactable = true;
            }
            
            if (StopButton != null)
            {
                StopButton.interactable = false;
            }

            
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
        }

        private void onAudioPlayTimeUpdate(RadioStation station, float playtime)
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

        private void onBufferingProgressUpdate(RadioStation station, float progress)
        {
            if (ElapsedTime != null)
            {
                ElapsedTime.text = Constants.TEXT_BUFFER + progress.ToString(Constants.FORMAT_PERCENT);
            }
        }

        private void onErrorInfo(RadioStation station, string info)
        {
            stop();

            if (!isStopped && InternetCheck.isInternetAvailable && Player != null && Player.Manager != null && Player.Manager.CountStations(filter) > 1)
            {
                if (isDirectionNext)
                {
                    Debug.LogError("Error occoured => Playing next random station." + System.Environment.NewLine + info);

                    //this.CTInvoke(() => next(), Constants.INVOKE_DELAY * invokeDelayCounter);
                    Invoke("next", Constants.INVOKE_DELAY * invokeDelayCounter);
                    invokeDelayCounter++;
                }
                else
                {
                    Debug.LogError("Error occoured => Playing previous random station." + System.Environment.NewLine + info);

                    //this.CTInvoke(() => previous(), Constants.INVOKE_DELAY * invokeDelayCounter);
                    Invoke("previous", Constants.INVOKE_DELAY * invokeDelayCounter);
                    invokeDelayCounter++;
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

        private void onProviderReady()
        {
            if (NextButton != null)
            {
                NextButton.interactable = true;
            }

            if (PreviousButton != null)
            {
                PreviousButton.interactable = true;
            }

            if (PlayButton != null)
            {
                PlayButton.interactable = true;
            }

            if (ElapsedTime != null)
            {
                ElapsedTime.text = Constants.TEXT_STOPPED;
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

        private void stop()
        {
            if (Player != null)
            {
                Player.Stop();
            }
        }

        private void next()
        {
            if (!isStopped)
            {
                Next();
            }
        }

        private void previous()
        {
            if (!isStopped)
            {
                Previous();
            }
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)