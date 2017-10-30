using UnityEngine;

namespace Crosstales.Radio.Tool
{
    /// <summary>
    /// Saves the streams of a RadioPlayer as audio files in the WAV-format.
    /// NOTE: Copyright laws for music are VERY STRICT and MUST BE respected! If you save music, make sure YOU have the RIGHT to do so! crosstales LLC denies any responsibility for YOUR actions with this tool - use it at your OWN RISK!
    /// For more, see  https://en.wikipedia.org/wiki/Radio_music_ripping and the rights applying to your country.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [HelpURL("https://crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_tool_1_1_stream_saver.html")]
    public class StreamSaver : MonoBehaviour
    {
        #region Variables

        /// <summary>Origin RadioPlayer.</summary>
        [Tooltip("Origin RadioPlayer.")]
        public RadioPlayer Player;

        /// <summary>Silence the origin (default: true).</summary>
        [Tooltip("Silence the origin (default: true).")]
        public bool SilenceSource = true;

        /// <summary>Output path for the audio files.</summary>
        [Tooltip("Output path for the audio files.")]
        public string OutputPath;

        /// <summary>Record delay in seconds before start saving the audio (default: 0).</summary>
        [Tooltip("Record delay in seconds before start saving the audio (default: 0).")]
        [Range(0f, 20f)]
        public float RecordStartDelay = 0f;

        /// <summary>Record delay in seconds before stop saving the audio (default: 0).</summary>
        [Tooltip("Record delay in seconds before stop saving the audio (default: 0).")]
        [Range(0f, 20f)]
        public float RecordStopDelay = 0f;

        private System.IO.FileStream fileStream;

        private const int HEADER_SIZE = 44; //default for uncompressed wav
        private const float RESCALE_FACTOR = 32767f;

        private AudioSource audioSource;

        private bool recOutput;
        private bool stopped = true;

        private int dataPosition = 0;

        private string fileName;

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
                Debug.LogWarning("No 'Player' added to the StreamSaver!");
            }
            else
            {
                Player.CaptureDataStream = true;
                Player.LegacyMode = false;
            }

            if (string.IsNullOrEmpty(OutputPath))
            {
                Debug.LogWarning("No 'OutputPath' added to the StreamSaver, saving in the project root!");
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

        public void OnEnable()
        {
            if (Player != null) {
                Player.OnAudioEnd += onAudioEnd;
                Player.OnNextRecordChange += onNextRecordChange;
            }
        }

        public void OnDisable()
        {
            if (Player != null) {
                Player.OnAudioEnd -= onAudioEnd;
                Player.OnNextRecordChange -= onNextRecordChange;
            }

            closeFile();

            audioSource.Stop();
            audioSource.clip = null;
            stopped = true;
        }

        #endregion


        #region Private methods

        private void openFile()
        {
            if (Util.Config.DEBUG)
                Debug.Log("openFile: " + fileName);

            if (fileStream != null && fileStream.CanWrite)
            {
                closeFile();
            }

            try
            {
                fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
                byte emptyByte = 0;

                for (int ii = 0; ii < HEADER_SIZE; ii++) //preparing the header
                {
                    fileStream.WriteByte(emptyByte);
                }

                recOutput = true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Could not open file '" + fileName + "': " + ex);
            }
        }

        private void convertAndWrite(float[] dataSource)
        {
            if (fileStream != null && fileStream.CanWrite)
            {
                short[] intData = new short[dataSource.Length];
                byte[] bytesData = new byte[dataSource.Length * 2];
                System.Byte[] byteArr = new System.Byte[2];

                for (int ii = 0; ii < dataSource.Length; ii++)
                {
                    intData[ii] = (short)(dataSource[ii] * RESCALE_FACTOR);
                    byteArr = System.BitConverter.GetBytes(intData[ii]);
                    byteArr.CopyTo(bytesData, ii * 2);
                }

                try
                {
                    fileStream.Write(bytesData, 0, bytesData.Length);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Could write to file '" + fileName + "': " + ex);
                }
            }
        }

        private void closeFile()
        {
            if (Util.Config.DEBUG)
                Debug.Log("closeFile");

            recOutput = false;

            if (fileStream != null && fileStream.CanWrite)
            {
                try
                {
                    fileStream.Seek(0, System.IO.SeekOrigin.Begin);

                    byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
                    fileStream.Write(riff, 0, 4);

                    byte[] chunkSize = System.BitConverter.GetBytes(fileStream.Length - 8);
                    fileStream.Write(chunkSize, 0, 4);

                    byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
                    fileStream.Write(wave, 0, 4);

                    byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
                    fileStream.Write(fmt, 0, 4);

                    byte[] subChunk1 = System.BitConverter.GetBytes(16);
                    fileStream.Write(subChunk1, 0, 4);

                    ushort one = 1;

                    byte[] audioFormat = System.BitConverter.GetBytes(one);
                    fileStream.Write(audioFormat, 0, 2);

                    byte[] numChannels = System.BitConverter.GetBytes(Player.Channels);
                    fileStream.Write(numChannels, 0, 2);

                    byte[] sampleRate = System.BitConverter.GetBytes(Player.SampleRate); //HZ?
                    fileStream.Write(sampleRate, 0, 4);

                    byte[] byteRate = System.BitConverter.GetBytes(Player.SampleRate * Player.Channels * 2);

                    fileStream.Write(byteRate, 0, 4);

                    ushort blockAlign = (ushort)(Player.Channels * 2);
                    fileStream.Write(System.BitConverter.GetBytes(blockAlign), 0, 2);

                    ushort bps = 16;
                    byte[] bitsPerSample = System.BitConverter.GetBytes(bps);
                    fileStream.Write(bitsPerSample, 0, 2);

                    byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
                    fileStream.Write(datastring, 0, 4);

                    byte[] subChunk2 = System.BitConverter.GetBytes(fileStream.Length - HEADER_SIZE);
                    fileStream.Write(subChunk2, 0, 4);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Could write header for file '" + fileName + "': " + ex);
                }
                finally
                {
                    try
                    {
                        fileStream.Close();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("Could close file '" + fileName + "': " + ex);
                    }
                }
            }
        }

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

                    if (recOutput)
                        convertAndWrite(floats);

                    dataPosition += count;
                }
            }
            else
            {
                System.Buffer.BlockCopy(new float[data.Length], 0, data, 0, data.Length * 4);
            }
        }

        #endregion


        #region Callback methods

        private void onAudioEnd(Model.RadioStation station)
        {
            if (Util.Config.DEBUG)
                Debug.Log("onAudioEnd");

            closeFile();
        }

        private void onNextRecordChange(Model.RadioStation station, Model.RecordInfo nextRecord, float delay)
        {
            if (Util.Config.DEBUG)
                Debug.Log("onNextRecordChange: " + delay);

            if (delay > 0f)
            {
                Invoke("closeFile", delay - RecordStopDelay - 0.2f);
            }

            fileName = OutputPath + nextRecord.Artist + " - " + nextRecord.Title + ".wav";
            Invoke("openFile", delay + RecordStartDelay + 0.2f);
        }

        #endregion
    }
}
// Copyright 2017 www.crosstales.com