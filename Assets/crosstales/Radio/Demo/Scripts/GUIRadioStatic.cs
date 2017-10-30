using UnityEngine;
using UnityEngine.UI;
using Crosstales.Radio.Util;

namespace Crosstales.Radio.Demo
{
    /// <summary>GUI for a radio player.</summary>
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_demo_1_1_g_u_i_radio_static.html")]
    public class GUIRadioStatic : MonoBehaviour
    {

        #region Variables

        [Header("Settings")]
        /// <summary>'RadioPlayer' from the scene.</summary>
        [Tooltip("'RadioPlayer' from the scene.")]
        public RadioPlayer Player;

        /// <summary>The color for the Play-mode.</summary>
        [Tooltip("The color for the Play-mode.")]
        public Color32 PlayColor = new Color32(0, 255, 0, 64);

        /// <summary>How many times should the radio station restart after an error before giving up (default: 3).</summary>
        [Tooltip("How many times should the radio station restart after an error before giving up (default: 3).")]
        public int Retries = 3;

        [Header("UI Objects")]
        public InputField Name;
        public Text Station;
        public InputField Url;
        public InputField Bitrate;
        public InputField Genere;
        public InputField Rating;
        public Text Format;
        public Text SongTitle;

        public Text Elapsed;

        public GameObject PlayButton;
        public GameObject StopButton;
        public Image MainImage;
/*
        [Header("Behaviour Settings")]
        /// <summary>Play the radio on start on/off (default: false).</summary>
        [Tooltip("Play the radio on start on/off (default: false).")]
        public bool PlayOnStart = false;
*/
        private Color32 color;
        private int invokeDelayCounter = 1;
        private bool isStopped = true;
        private int playtime = 0;

        #endregion


        #region MonoBehaviour methods

        public void Start()
        {
            if (Player != null)
            {
                // Subscribe event listeners
                Player.OnPlaybackStart += onPlayBackStart;
                Player.OnPlaybackEnd += onPlaybackEnd;
                Player.OnAudioStart += onAudioStart;
                Player.OnAudioEnd += onAudioEnd;
                Player.OnAudioPlayTimeUpdate += onAudioPlayTime;
                Player.OnBufferingProgressUpdate += onBufferingProgress;
                Player.OnErrorInfo += onErrorInfo;

                // Fill fields from the Radio component
                Name.text = Player.Station.Name;
                Station.text = Helper.CleanUrl(Player.Station.Station);
                Url.text = Player.Station.Url;
                Genere.text = Player.Station.Genres;
                Bitrate.text = Player.Station.Bitrate.ToString();
                Rating.text = Player.Station.Rating.ToString();
                Format.text = Player.Station.Format.ToString();
            }
            else
            {
                Debug.LogError("'Player' is null!");
            }

            if (Elapsed != null)
            {
                Elapsed.text = Constants.TEXT_STOPPED;
            }

            color = MainImage.color;
/*
            if (PlayOnStart)
            {
                //Invoke("Play", 0.1f);
                Play();
            }
            else
            {
                */
                if (Player != null)
                {
                    PlayButton.SetActive(!Player.isPlayback);
                    StopButton.SetActive(Player.isPlayback);

                    if (Player.isPlayback)
                    {
                        MainImage.color = PlayColor;
                    }
                //}
            }
        }

        public void Update() {
            if (SongTitle != null)
            {
                if (Player != null && Player.isPlayback) {
                    SongTitle.text = Player.RecordInfo.StreamTitle;
                } else {
                    SongTitle.text = string.Empty;
                }
            }
        }

        public void OnDestroy()
        {
            if (Player != null)
            {
                // Unsubscribe event listeners
                Player.OnPlaybackStart -= onPlayBackStart;
                Player.OnPlaybackEnd -= onPlaybackEnd;
                Player.OnAudioStart += onAudioStart;
                Player.OnAudioEnd += onAudioEnd;
                Player.OnAudioPlayTimeUpdate -= onAudioPlayTime;
                Player.OnBufferingProgressUpdate -= onBufferingProgress;
                Player.OnErrorInfo -= onErrorInfo;
            }
        }

        #endregion


        #region Public methods

        public void Play()
        {
            if (Player != null)
            {
                Player.Play();
            }
        }

        public void Stop()
        {
            if (Player != null)
            {
                Player.Stop();
            }
        }

        public void OpenUrl()
        {
            if (Player != null)
            {
                Application.OpenURL(Player.Station.Station);
            }
        }

        public void ChangeVolume(float volume)
        {
            if (Player != null && Player.Source != null)
            {
                Player.Source.volume = volume;
            }
        }

        public void NameChanged(string name)
        {
            if (Player != null)
            {
                Player.Station.Name = name;
            }
        }

        public void StationChanged(string station)
        {
            if (Player != null)
            {
                Player.Station.Station = station;
            }
        }

        public void UrlChanged(string url)
        {
            if (Player != null)
            {
                Player.Station.Url = url;
            }
        }

        public void GenresChanged(string genres)
        {
            if (Player != null)
            {
                Player.Station.Genres = genres;
            }
        }

        public void BitrateChanged(string bitrateString)
        {
            if (Player != null)
            {
                int bitrate;
                if (int.TryParse(bitrateString, out bitrate))
                {
                    Player.Station.Bitrate = Helper.NearestBitrate(bitrate, Player.Station.Format);
                }

                Bitrate.text = Player.Station.Bitrate.ToString();
            }
        }

        public void RatingChanged(string ratingString)
        {
            if (Player != null)
            {
                float rating;
                if (float.TryParse(ratingString, out rating))
                {
                    Player.Station.Rating = Mathf.Clamp(rating, 0f, 5f);
                }

                Rating.text = Player.Station.Rating.ToString();
            }
        }

        public void OpenSpotifyUrl()
        {
            if (Player != null)
            {
                Application.OpenURL(Player.RecordInfo.SpotifyUrl);
            }
        }

        #endregion


        #region Callback methods

        private void onPlayBackStart(Model.RadioStation station)
        {
            PlayButton.SetActive(false);
            StopButton.SetActive(true);

            MainImage.color = PlayColor;
        }

        private void onPlaybackEnd(Model.RadioStation station)
        {
            PlayButton.SetActive(true);
            StopButton.SetActive(false);

            MainImage.color = color;

            if (Elapsed != null)
            {
                Elapsed.text = Constants.TEXT_STOPPED;
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

        private void onAudioPlayTime(Model.RadioStation station, float playtime)
        {
            if (Elapsed != null)
            {
                if ((int)playtime != this.playtime)
                {
                    Elapsed.text = Helper.FormatSecondsToHourMinSec(playtime);

                    this.playtime = (int)playtime;

                    //Debug.Log("Download per second: " + Helper.FormatBytesToHRF((long)(Context.TotalDataSize / Context.TotalPlayTime)));

                    if (playtime > 30f)
                    {
                        invokeDelayCounter = 1;
                    }
                }
            }
        }

        private void onBufferingProgress(Model.RadioStation station, float progress)
        {
            if (Elapsed != null)
            {
                Elapsed.text = Constants.TEXT_BUFFER + progress.ToString(Constants.FORMAT_PERCENT);
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
                    Debug.LogError("Restarting station failed more than " + Retries + " times - giving up!" + System.Environment.NewLine + info);
                }
            }
            else
            {
                Debug.LogError("Could not start the station '" + station.Name + "'! Please try another station. " + System.Environment.NewLine + info);
            }
        }

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