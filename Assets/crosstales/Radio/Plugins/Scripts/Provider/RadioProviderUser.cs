using UnityEngine;

namespace Crosstales.Radio.Provider
{
    /// <summary>Provider for users of Radio. This enables the possibility to manage the desired stations with a given inital set of stations.</summary>
    //[ExecuteInEditMode]
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_provider_1_1_radio_provider_user.html")]
    public class RadioProviderUser : BaseRadioProvider
    {

        #region Variables

        [Header("Save Behaviour")]
        /// <summary>Calls 'Save' OnDisable (default: true).</summary>
        [Tooltip("Calls 'Save' OnDisable (default: true).")]
        public bool SaveOnDisable = true;


        [Header("Source Settings")]
        /// <summary>User radio station entry.</summary>
        [Tooltip("User radio station entry.")]
        public Model.Entry.RadioEntryUser Entry; // = new RadioEntryUser();

        #endregion


        #region Properties

        public override System.Collections.Generic.List<Model.Entry.BaseRadioEntry> RadioEntries
        {
            get
            {
                return new System.Collections.Generic.List<Model.Entry.BaseRadioEntry> { Entry };
            }
        }

        #endregion


        #region MonoBehaviour methods

        //public override void Awake()
        //{
        //    base.Awake();
        //}

        public void OnDisable()
        {
            if (!Util.Helper.isEditorMode)
            {
                if (SaveOnDisable)
                {
                    Save(Entry.FinalPath);
                }
            }
        }

        public override void OnValidate()
        {

            //Debug.Log("OnValidate called: " + Entry);

            if (Entry != null)
            {
                if (!Entry.isInitalized)
                {
                    Entry.LoadOnlyOnce = true;
                }
            }

            base.OnValidate();
        }

        #endregion


        #region Public methods

        /// <summary>Deletes the user text-file.</summary>
        public void Delete()
        {
            if (System.IO.File.Exists(Entry.FinalPath))
            {
                try
                {
                    System.IO.File.Delete(Entry.FinalPath);
                }
                catch (System.IO.IOException ex)
                {
                    Debug.LogError("Could not delete file: " + Entry.FinalPath + System.Environment.NewLine + ex);
                }
            }
        }

        /// <summary>Shows the location of the user text-file in OS file browser.</summary>
        public void ShowFile()
        {
            if (Util.Helper.isStandalonePlatform)
            {
                string path = System.IO.Path.GetDirectoryName(Entry.FinalPath);

                if (Util.Helper.isWindowsPlatform)
                {
                    path += Util.Constants.PATH_DELIMITER_WINDOWS;
                }
                else
                {
                    path += Util.Constants.PATH_DELIMITER_UNIX;
                }

                //Debug.Log(path);

                System.Diagnostics.Process.Start(path);
            }
            else
            {
                Debug.LogWarning("'ShowFile' is not supported on your platform!");
            }
        }

        /// <summary>Edits the user text-file with the OS default application.</summary>
        public void EditFile()
        {
            if (Util.Helper.isStandalonePlatform)
            {
                string file = Entry.FinalPath;

                if (System.IO.File.Exists(file))
                {
                    if (Util.Helper.isMacOSPlatform)
                    {
                        using (System.Diagnostics.Process process = new System.Diagnostics.Process())
                        {

                            process.StartInfo.FileName = "open";
                            process.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(Entry.FinalPath) + Util.Constants.PATH_DELIMITER_UNIX;
                            process.StartInfo.Arguments = "-t " + System.IO.Path.GetFileName(file);

                            process.Start();
                        }
                    }
                    else
                    {
                        System.Diagnostics.Process.Start(file);
                    }
                }
            }
            else
            {
                Debug.LogWarning("'EditFile' is not supported on your platform!");
            }
        }

        #endregion


        #region Private methods

        protected override void init()
        {
            base.init();

            if (Entry != null && Entry.EnableSource)
            {
                if (!string.IsNullOrEmpty(Entry.FinalPath) && System.IO.File.Exists(Entry.FinalPath))
                {
                    StartCoroutine(loadWeb(addCoRoutine(), new Model.Entry.RadioEntryURL(Entry, Util.Constants.PREFIX_FILE + Entry.FinalPath, Model.Enum.DataFormatURL.Text), true));
                }

                if (Entry.Resource != null)
                {
                    if (!Entry.LoadOnlyOnce || (Entry.LoadOnlyOnce && !System.IO.File.Exists(Entry.FinalPath)))
                    {
                        StartCoroutine(loadResource(addCoRoutine(), new Model.Entry.RadioEntryResource(Entry, Entry.Resource, Entry.DataFormat, Entry.ReadNumberOfStations), true));

                        if (!System.IO.File.Exists(Entry.FinalPath))
                        { //always store file first
                            //this.CTInvoke(() => save(), 3f);
                            Invoke("save", 2f);
                        }
                    }
                }
            }
        }

        private void save()
        {
            Save(Entry.FinalPath);
        }

        #endregion


        #region Editor-only methods

#if UNITY_EDITOR

        protected override void initInEditor()
        {
            if (Util.Helper.isEditorMode)
            {
                base.initInEditor();

                if (Entry != null && Entry.EnableSource)
                {
                    if (!string.IsNullOrEmpty(Entry.FinalPath) && System.IO.File.Exists(Entry.FinalPath))
                    {
                        loadWebInEditor(new Model.Entry.RadioEntryURL(Entry, Util.Constants.PREFIX_FILE + Entry.FinalPath, Model.Enum.DataFormatURL.Text), true);
                    }

                    if (Entry.Resource != null)
                    {
                        if (!Entry.LoadOnlyOnce || (Entry.LoadOnlyOnce && !System.IO.File.Exists(Entry.FinalPath)))
                        {

                            //                        Debug.Log("File: " + File.Exists(Entry.FinalPath));
                            //                        Debug.Log("Load: " + Entry.LoadOnlyOnce);

                            loadResourceInEditor(new Model.Entry.RadioEntryResource(Entry, Entry.Resource, Entry.DataFormat, Entry.ReadNumberOfStations), true);
                        }
                    }
                }
            }
        }

#endif

        #endregion

    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)