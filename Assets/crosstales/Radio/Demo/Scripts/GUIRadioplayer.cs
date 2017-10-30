using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Crosstales.Radio.Model;

namespace Crosstales.Radio.Demo
{
    /// <summary>GUI for multiple radio players.</summary>
    [HelpURL("https://www.crosstales.com/media/data/assets/radio/api/class_crosstales_1_1_radio_1_1_demo_1_1_g_u_i_radioplayer.html")]
    public class GUIRadioplayer : MonoBehaviour
    {

        #region Variables

        [Header("Settings")]
        /// <summary>'RadioManager' from the scene.</summary>
        [Tooltip("'RadioManager' from the scene.")]
        public RadioManager Manager;

        /// <summary>Prefab for the radio list.</summary>
        [Tooltip("Prefab for the radio list.")]
        public GameObject ItemPrefab;

        [Header("UI Objects")]
        public GameObject Target;
        public GameObject OrderPanel;
        public Scrollbar Scroll;
        public int ColumnCount = 1;
        public Vector2 SpaceWidth = new Vector2(8, 8);
        public Vector2 SpaceHeight = new Vector2(8, 8);
        public Color32 EvenColor = new Color32(242, 236, 224, 128);
        public Color32 OddColor = new Color32(128, 128, 128, 128);
        public Text StationCounter;

        //private bool orderByName = true;
        private bool orderByNameDesc = false;
        private bool orderByNameStandard = true;
        private bool orderByStation = false;
        private bool orderByStationDesc = false;
        private bool orderByStationStandard = true;
        private bool orderByUrl = false;
        private bool orderByUrlDesc = false;
        private bool orderByUrlStandard = true;
        private bool orderByFormat = false;
        private bool orderByFormatDesc = false;
        private bool orderByFormatStandard = false;
        private bool orderByBitrate = false;
        private bool orderByBitrateDesc = false;
        private bool orderByBitrateStandard = false;
        private bool orderByGenre = false;
        private bool orderByGenreDesc = false;
        private bool orderByGenreStandard = true;
        private bool orderByRating = false;
        private bool orderByRatingDesc = false;
        private bool orderByRatingStandard = false;

        private bool isSorting = false;

        private RadioFilter filter = new RadioFilter();

        #endregion


        #region MonoBehaviour methods

        public void Start()
        {
            if (Manager != null)
            {
                Manager.OnProviderReady += onProviderReady;

                StationCounter.text = "Loading...";
            }
            else
            {
                Debug.LogError("'Manager' is null!");

                StationCounter.text = "Error";
            }
        }

        public void Update()
        {
            if (Time.frameCount % 10 == 0)
            { //don't stress
                OrderPanel.SetActive(isSorting);
            }
        }

        public void OnDestroy()
        {
            if (Manager != null)
            {
                Manager.OnProviderReady -= onProviderReady;
            }
        }

        #endregion


        #region Public methods

        public void FilterName(string filter)
        {
            this.filter.Name = filter;

            OrderByName();
        }

        public void FilterStation(string filter)
        {
            this.filter.Station = filter;

            OrderByStation();
        }

        public void FilterUrl(string filter)
        {
            this.filter.Url = filter;

            OrderByUrl();
        }

        public void FilterBitrateMin(string bitrate)
        {
            if (int.TryParse(bitrate, out filter.BitrateMin))
            {
                OrderByBitrate();
            }
        }

        public void FilterBitrateMax(string bitrate)
        {
            if (int.TryParse(bitrate, out filter.BitrateMax))
            {
                OrderByBitrate();
            }
        }

        public void FilterGenre(string filter)
        {
            this.filter.Genres = filter;

            OrderByGenre();
        }

        public void FilterRatingMin(string rating)
        {
            if (float.TryParse(rating, out filter.RatingMin))
            {
                OrderByRating();
            }
        }

        public void FilterRatingMax(string rating)
        {
            if (float.TryParse(rating, out filter.RatingMax))
            {
                OrderByRating();
            }
        }

        public void FilterFormat(string filter)
        {
            this.filter.Format = filter;

            OrderByFormat();
        }

        public void OrderByName()
        {
            if (!isSorting)
            {
                resetOrderBy();

                if (orderByNameStandard)
                {
                    //            orderByName = true;
                    //            orderByNameDesc = false;

                    //               orderByName = false;
                    orderByNameDesc = true;

                    orderByNameStandard = false;
                }
                else
                {

                    //               orderByName = true;
                    orderByNameDesc = false;
                    //            orderByName = false;
                    //            orderByNameDesc = true;
                    orderByNameStandard = true;
                }

                StartCoroutine(buildRadioPlayerList());
            }
        }

        public void OrderByStation()
        {
            if (!isSorting)
            {
                resetOrderBy();

                if (orderByStationStandard)
                {
                    orderByStation = true;
                    orderByStationDesc = false;
                    orderByStationStandard = false;
                }
                else
                {
                    orderByStation = false;
                    orderByStationDesc = true;
                    orderByStationStandard = true;
                }

                StartCoroutine(buildRadioPlayerList());
            }
        }

        public void OrderByUrl()
        {
            if (!isSorting)
            {
                resetOrderBy();

                if (orderByUrlStandard)
                {
                    orderByUrl = true;
                    orderByUrlDesc = false;
                    orderByUrlStandard = false;
                }
                else
                {
                    orderByUrl = false;
                    orderByUrlDesc = true;
                    orderByUrlStandard = true;
                }

                StartCoroutine(buildRadioPlayerList());
            }
        }

        public void OrderByFormat()
        {
            if (!isSorting)
            {
                resetOrderBy();

                if (orderByFormatStandard)
                {
                    orderByFormat = true;
                    orderByFormatDesc = false;
                    orderByFormatStandard = false;
                }
                else
                {
                    orderByFormat = false;
                    orderByFormatDesc = true;
                    orderByFormatStandard = true;
                }

                StartCoroutine(buildRadioPlayerList());
            }
        }

        public void OrderByBitrate()
        {
            if (!isSorting)
            {
                resetOrderBy();

                if (orderByBitrateStandard)
                {
                    orderByBitrate = true;
                    orderByBitrateDesc = false;
                    orderByBitrateStandard = false;
                }
                else
                {
                    orderByBitrate = false;
                    orderByBitrateDesc = true;
                    orderByBitrateStandard = true;
                }

                StartCoroutine(buildRadioPlayerList());
            }
        }

        public void OrderByGenre()
        {
            if (!isSorting)
            {
                resetOrderBy();

                if (orderByGenreStandard)
                {
                    orderByGenre = true;
                    orderByGenreDesc = false;
                    orderByGenreStandard = false;
                }
                else
                {
                    orderByGenre = false;
                    orderByGenreDesc = true;
                    orderByGenreStandard = true;
                }

                StartCoroutine(buildRadioPlayerList());
            }
        }

        public void OrderByRating()
        {
            if (!isSorting)
            {
                resetOrderBy();

                if (orderByRatingStandard)
                {
                    orderByRating = true;
                    orderByRatingDesc = false;
                    orderByRatingStandard = false;
                }
                else
                {
                    orderByRating = false;
                    orderByRatingDesc = true;
                    orderByRatingStandard = true;
                }

                StartCoroutine(buildRadioPlayerList());
            }
        }

        #endregion


        #region Callback methods

        private void onProviderReady()
        {
            StartCoroutine(buildRadioPlayerList());
        }

        #endregion


        #region Private methods

        private IEnumerator buildRadioPlayerList()
        {
            if (!isSorting && Manager != null)
            {
                isSorting = true;

                RectTransform rowRectTransform = ItemPrefab.GetComponent<RectTransform>();
                RectTransform containerRectTransform = Target.GetComponent<RectTransform>();
                Vector2 modifierVector = Vector2.zero;

                //clear existing items
                for (int ii = Target.transform.childCount - 1; ii >= 0; ii--)
                {
                    Transform child = Target.transform.GetChild(ii);
                    child.SetParent(null);
                    Destroy(child.gameObject);
                }

                yield return null;

                //Debug.Log("LOADED: " + Manager.Players.Count);

                System.Collections.Generic.List<RadioPlayer> items;

                //            if (orderByName) {
                //               items = Manager.RadioStationsByName();
                //            } else if (orderByNameDesc) {

                if (orderByNameDesc)
                {
                    items = Manager.PlayersByName(true, filter);
                }
                else if (orderByUrl)
                {
                    items = Manager.PlayersByURL(false, filter);
                }
                else if (orderByUrlDesc)
                {
                    items = Manager.PlayersByURL(true, filter);
                }
                else if (orderByFormat)
                {
                    items = Manager.PlayersByFormat(false, filter);
                }
                else if (orderByFormatDesc)
                {
                    items = Manager.PlayersByFormat(true, filter);
                }
                else if (orderByStation)
                {
                    items = Manager.PlayersByStation(false, filter);
                }
                else if (orderByStationDesc)
                {
                    items = Manager.PlayersByStation(true, filter);
                }
                else if (orderByBitrate)
                {
                    items = Manager.PlayersByBitrate(false, filter);
                }
                else if (orderByBitrateDesc)
                {
                    items = Manager.PlayersByBitrate(true, filter);
                }
                else if (orderByGenre)
                {
                    items = Manager.PlayersByGenres(false, filter);
                }
                else if (orderByGenreDesc)
                {
                    items = Manager.PlayersByGenres(true, filter);
                }
                else if (orderByRating)
                {
                    items = Manager.PlayersByRating(false, filter);
                }
                else if (orderByRatingDesc)
                {
                    items = Manager.PlayersByRating(true, filter);
                }
                else
                {
                    items = Manager.PlayersByName(false, filter);
                }

                //items = Manager.Players;

                StationCounter.text = "Stations: " + items.Count;

                //calculate the width and height of each child item.
                float width = containerRectTransform.rect.width / ColumnCount - (SpaceWidth.x + SpaceWidth.y) * ColumnCount;
                float height = rowRectTransform.rect.height - (SpaceHeight.x + SpaceHeight.y);

                int rowCount = items.Count / ColumnCount;

                if (rowCount > 0 && items.Count % rowCount > 0)
                {
                    rowCount++;
                }

                //adjust the height of the container so that it will just barely fit all its children
                float scrollHeight = height * rowCount;
                modifierVector.Set(containerRectTransform.offsetMin.x, -scrollHeight / 2f);
                containerRectTransform.offsetMin = modifierVector;
                modifierVector.Set(containerRectTransform.offsetMax.x, scrollHeight / 2f);
                containerRectTransform.offsetMax = modifierVector;

                int jj = 0;
                GameObject newItem;
                RectTransform rectTransform;

                float middleHeight = -containerRectTransform.rect.width / 2f;
                float middleWidth = containerRectTransform.rect.height / 2f;

                for (int ii = 0; ii < items.Count; ii++)
                {
                    //this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
                    if (ii % ColumnCount == 0)
                    {
                        jj++;
                    }

                    //create a new item, name it, and set the parent
                    newItem = Instantiate(ItemPrefab) as GameObject;
                    newItem.name = Target.name + " item at (" + ii + "," + jj + ")";
                    newItem.transform.SetParent(Target.transform);
                    newItem.transform.localScale = Vector3.one;

                    if (ii % 2 == 0)
                    {
                        newItem.GetComponent<Image>().color = OddColor;
                    }

                    newItem.GetComponent<GUIRadioStatic>().Player = items[ii];
                    rectTransform = newItem.GetComponent<RectTransform>();

                    //move and size the new item
                    modifierVector.Set(middleHeight + (width + SpaceWidth.x) * (ii % ColumnCount) + SpaceWidth.x * ColumnCount, middleWidth - height * jj);
                    rectTransform.offsetMin = modifierVector;
                    modifierVector.Set(rectTransform.offsetMin.x + width, rectTransform.offsetMin.y + height);
                    rectTransform.offsetMax = modifierVector;

                    if (ii % 13 == 0)
                    { //return to the main thread after 13 processed items (reduces jttering)
                        Scroll.value = 1f;
                        yield return null;
                    }
                }

                isSorting = false;
            }
            Scroll.value = 1f;
        }

        private void resetOrderBy()
        {
            //orderByName = false;
            orderByStation = false;
            orderByUrl = false;
            orderByFormat = false;
            orderByBitrate = false;
            orderByGenre = false;
            orderByRating = false;

            orderByNameDesc = false;
            orderByStationDesc = false;
            orderByUrlDesc = false;
            orderByFormatDesc = false;
            orderByBitrateDesc = false;
            orderByGenreDesc = false;
            orderByRatingDesc = false;
        }

        #endregion
    }
}
// © 2015-2017 crosstales LLC (https://www.crosstales.com)