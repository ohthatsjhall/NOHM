using UnityEngine;

namespace Crosstales.Radio.Tool
{
    /// <summary>Loudspeaker for a RadioPlayer.</summary>
    [RequireComponent(typeof(AudioSource))]
    [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_tool_1_1_loudspeaker.html")]
    public class Loudspeaker : MonoBehaviour
    {

        #region Variables

        /// <summary>Origin RadioPlayer.</summary>
        [Tooltip("Origin RadioPlayer.")]
        public RadioPlayer Player;

        /// <summary>Synchronize with the origin (default: false).</summary>
        //[Tooltip("Synchronize with the origin (default: false).")]
        //public bool Synchronized = false;

        /// <summary>Silence the origin (default: true).</summary>
        [Tooltip("Silence the origin (default: true).")]
        public bool SilenceSource = true;

        private AudioSource audioSource;

        private bool stopped = true;

        private int dataPosition = 0;

        #endregion


        #region Properties

        /// <summary>Silence the origin (main use is for UI).</summary>
        public bool isSilenceSource
        {
            get
            {
                return SilenceSource;
            }

            set
            {
                SilenceSource = value;
            }
        }

        #endregion


        #region MonoBehaviour methods

        public void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.Stop(); //always stop the AudioSource at startup
        }

        public void Start()
        {
            if (Player == null)
            {
                Debug.LogWarning("No 'Player' added to the Loudspeaker!");
            }
            else
            {
                Player.CaptureDataStream = true;
            }

        }

        public void Update()
        {

            if (Player != null && Player.isAudioPlaying)
            {
                if (stopped)
                {
                    stopped = false;

                    AudioClip myClip = AudioClip.Create(Player.Station.Name, int.MaxValue, Player.Channels, Player.SampleRate, true, readPCMData);

                    audioSource.clip = myClip;

                    audioSource.Play();

                    if (SilenceSource)
                    {
                        Player.Silence();
                    }
                }

                //if (Synchronized)
                //{
                //    audioSource.timeSamples = Player.Source.timeSamples;
                //}
            }
            else
            {
                if (!stopped)
                {
                    audioSource.Stop();
                    audioSource.clip = null;
                    stopped = true;
                    dataPosition = 0;
                }
            }
        }

        public void OnDisable()
        {
            audioSource.Stop();
            audioSource.clip = null;
            stopped = true;
        }

        #endregion


        #region Private methods

        private void readPCMData(float[] data)
        {


            if (Player.isAudioPlaying && Player.DataStream != null)
            {
                byte[] buffer = new byte[data.Length * 2];

                int count;

                Player.DataStream.Position = dataPosition;

                if ((count = Player.DataStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    float[] floats = Util.Helper.ConvertByteArrayToFloatArray(buffer, count);
                    System.Buffer.BlockCopy(floats, 0, data, 0, data.Length * 4);

                    dataPosition += count;
                }
            }
            else
            {
                System.Buffer.BlockCopy(new float[data.Length], 0, data, 0, data.Length * 4);
            }
        }

        #endregion
    }
}
// © 2017 crosstales LLC (https://www.crosstales.com)