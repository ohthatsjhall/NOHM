using UnityEngine;
using UnityEditor;

namespace Crosstales.Radio.EditorUtil
{
    /// <summary>Editor helper class.</summary>
    public static class EditorHelper
    {

        #region Static variables

        /// <summary>Start index inside the "GameObject"-menu.</summary>
        //public const int GO_ID = 25;
        public const int GO_ID = 20;

        /// <summary>Start index inside the "Tools"-menu.</summary>
        public const int MENU_ID = 11801; // 1, R = 18, A = 01

        private static Texture2D logo_asset;
        private static Texture2D logo_asset_small;
        private static Texture2D logo_ct;
        private static Texture2D logo_unity;

        private static Texture2D icon_save;
        private static Texture2D icon_reset;
        private static Texture2D icon_plus;
        private static Texture2D icon_minus;
        private static Texture2D icon_play;
        private static Texture2D icon_stop;
        private static Texture2D icon_next;
        private static Texture2D icon_previous;
        private static Texture2D icon_refresh;
        private static Texture2D icon_delete;
        private static Texture2D icon_edit;
        private static Texture2D icon_show;

        private static Texture2D icon_clear;

        private static Texture2D icon_manual;
        private static Texture2D icon_api;
        private static Texture2D icon_forum;
        private static Texture2D icon_product;

        private static Texture2D icon_check;

        private static Texture2D social_Facebook;
        private static Texture2D social_Twitter;
        private static Texture2D social_Youtube;
        private static Texture2D social_Linkedin;
        private static Texture2D social_Xing;

        private static Texture2D video_promo;
        private static Texture2D video_tutorial;

        private static Texture2D icon_videos;

        private static Texture2D store_AudioVisualizer;
        private static Texture2D store_CompleteSoundSuite;
        private static Texture2D store_PlayMaker;
        private static Texture2D store_VisualizerStudio;

        private static Texture2D icon_3p_assets;

        #endregion


        #region Static properties

        public static Texture2D Logo_Asset
        {
            get
            {
                if (Util.Constants.isPro)
                {
                    return loadImage(ref logo_asset, "logo_asset_pro.png");
                }
                else
                {
                    return loadImage(ref logo_asset, "logo_asset.png");
                }
            }
        }

        public static Texture2D Logo_Asset_Small
        {
            get
            {
                if (Util.Constants.isPro)
                {
                    return loadImage(ref logo_asset_small, "logo_asset_small_pro.png");
                }
                else
                {
                    return loadImage(ref logo_asset_small, "logo_asset_small.png");
                }
            }
        }

        public static Texture2D Logo_CT
        {
            get
            {
                return loadImage(ref logo_ct, "logo_crosstales.png");
            }
        }

        public static Texture2D Logo_Unity
        {
            get
            {
                return loadImage(ref logo_unity, "logo_Unity.png");
            }
        }

        public static Texture2D Icon_Save
        {
            get
            {
                return loadImage(ref icon_save, "icon_save.png");
            }
        }

        public static Texture2D Icon_Reset
        {
            get
            {
                return loadImage(ref icon_reset, "icon_reset.png");
            }
        }

        public static Texture2D Icon_Plus
        {
            get
            {
                return loadImage(ref icon_plus, "icon_plus.png");
            }
        }

        public static Texture2D Icon_Minus
        {
            get
            {
                return loadImage(ref icon_minus, "icon_minus.png");
            }
        }

        public static Texture2D Icon_Play
        {
            get
            {
                return loadImage(ref icon_play, "icon_play.png");
            }
        }

        public static Texture2D Icon_Stop
        {
            get
            {
                return loadImage(ref icon_stop, "icon_stop.png");
            }
        }

        public static Texture2D Icon_Next
        {
            get
            {
                return loadImage(ref icon_next, "icon_next.png");
            }
        }

        public static Texture2D Icon_Previous
        {
            get
            {
                return loadImage(ref icon_previous, "icon_previous.png");
            }
        }

        public static Texture2D Icon_Refresh
        {
            get
            {
                return loadImage(ref icon_refresh, "icon_refresh.png");
            }
        }

        public static Texture2D Icon_Delete
        {
            get
            {
                return loadImage(ref icon_delete, "icon_delete.png");
            }
        }

        public static Texture2D Icon_Edit
        {
            get
            {
                return loadImage(ref icon_edit, "icon_edit.png");
            }
        }

        public static Texture2D Icon_Show
        {
            get
            {
                return loadImage(ref icon_show, "icon_show.png");
            }
        }

        public static Texture2D Icon_Clear
        {
            get
            {
                return loadImage(ref icon_clear, "icon_clear.png");
            }
        }

        public static Texture2D Icon_Manual
        {
            get
            {
                return loadImage(ref icon_manual, "icon_manual.png");
            }
        }

        public static Texture2D Icon_API
        {
            get
            {
                return loadImage(ref icon_api, "icon_api.png");
            }
        }

        public static Texture2D Icon_Forum
        {
            get
            {
                return loadImage(ref icon_forum, "icon_forum.png");
            }
        }

        public static Texture2D Icon_Product
        {
            get
            {
                return loadImage(ref icon_product, "icon_product.png");
            }
        }

        public static Texture2D Icon_Check
        {
            get
            {
                return loadImage(ref icon_check, "icon_check.png");
            }
        }

        public static Texture2D Social_Facebook
        {
            get
            {
                return loadImage(ref social_Facebook, "social_Facebook.png");
            }
        }

        public static Texture2D Social_Twitter
        {
            get
            {
                return loadImage(ref social_Twitter, "social_Twitter.png");
            }
        }

        public static Texture2D Social_Youtube
        {
            get
            {
                return loadImage(ref social_Youtube, "social_Youtube.png");
            }
        }

        public static Texture2D Social_Linkedin
        {
            get
            {
                return loadImage(ref social_Linkedin, "social_Linkedin.png");
            }
        }

        public static Texture2D Social_Xing
        {
            get
            {
                return loadImage(ref social_Xing, "social_Xing.png");
            }
        }

        public static Texture2D Video_Promo
        {
            get
            {
                return loadImage(ref video_promo, "video_promo.png");
            }
        }

        public static Texture2D Video_Tutorial
        {
            get
            {
                return loadImage(ref video_tutorial, "video_tutorial.png");
            }
        }

        public static Texture2D Icon_Videos
        {
            get
            {
                return loadImage(ref icon_videos, "icon_videos.png");
            }
        }

        public static Texture2D Store_AudioVisualizer
        {
            get
            {
                return loadImage(ref store_AudioVisualizer, "store_AudioVisualizer.png");
            }
        }

        public static Texture2D Store_CompleteSoundSuite
        {
            get
            {
                return loadImage(ref store_CompleteSoundSuite, "store_CompleteSoundSuite.png");
            }
        }

        public static Texture2D Store_PlayMaker
        {
            get
            {
                return loadImage(ref store_PlayMaker, "store_PlayMaker.png");
            }
        }

        public static Texture2D Store_VisualizerStudio
        {
            get
            {
                return loadImage(ref store_VisualizerStudio, "store_VisualizerStudio.png");
            }
        }

        public static Texture2D Icon_3p_Assets
        {
            get
            {
                return loadImage(ref icon_3p_assets, "icon_3p_assets.png");
            }
        }

        #endregion


        #region Static methods

        /// <summary>Shows a separator-UI.</summary>
        /// <param name="space">Space in pixels between the component and the seperator line (default: 10, optional).</param>
        public static void SeparatorUI(int space = 10)
        {
            GUILayout.Space(space);
            GUILayout.Box(string.Empty, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
        }

        /// <summary>Refreshes the asset database.</summary>
        public static void RefreshAssetDatabase()
        {
            if (Util.Helper.isEditorMode)
            {
                //Debug.Log("Refresh AssetDatabase");
                AssetDatabase.Refresh();
            }
        }

        /// <summary>Instantiates a prefab.</summary>
        /// <param name="prefabName">Name of the prefab.</param>
        public static void InstantiatePrefab(string prefabName)
        {
            PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath("Assets" + Util.Config.PREFAB_PATH + prefabName + ".prefab", typeof(GameObject)));
        }

        /// <summary>Checks if the 'InternetCheck'-prefab is in the scene.</summary>
        /// <returns>True if the 'InternetCheck'-prefab is in the scene.</returns>
        public static bool isInternetCheckInScene
        {
            get
            {
                return GameObject.Find(Util.Constants.INTERNETCHECK_SCENE_OBJECT_NAME) != null;
            }
        }

        /// <summary>Checks if the 'Proxy'-prefab is in the scene.</summary>
        /// <returns>True if the 'Proxy'-prefab is in the scene.</returns>
        public static bool isProxyInScene
        {
            get
            {
                return GameObject.Find(Util.Constants.PROXY_SCENE_OBJECT_NAME) != null;
            }
        }

        /// <summary>Checks if the 'SurviveSceneSwitch'-prefab is in the scene.</summary>
        /// <returns>True if the 'SurviveSceneSwitch'-prefab is in the scene.</returns>
        public static bool isSurviveSceneSwitchInScene
        {
            get
            {
                return GameObject.Find(Util.Constants.SURVIVOR_SCENE_OBJECT_NAME) != null;
            }
        }

        /// <summary>Loads an image as Texture2D from 'Editor Default Resources'.</summary>
        /// <param name="logo">Logo to load.</param>
        /// <param name="fileName">Name of the image.</param>
        /// <returns>Image as Texture2D from 'Editor Default Resources'.</returns>
        private static Texture2D loadImage(ref Texture2D logo, string fileName)
        {
            if (logo == null)
            {
                //logo = (Texture2D)Resources.Load(fileName, typeof(Texture2D));

#if radio_ignore_setup
                logo = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets" + Util.Config.ASSET_PATH + "Icons/" + fileName, typeof(Texture2D));
#else
                logo = (Texture2D)EditorGUIUtility.Load("Radio/" + fileName);
#endif

                if (logo == null)
                {
                    Debug.LogWarning("Image not found: " + fileName);
                }
            }

            return logo;
        }

        #endregion
    }
}
// © 2016-2017 crosstales LLC (https://www.crosstales.com)