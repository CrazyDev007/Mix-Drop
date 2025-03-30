using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public enum LevelStatus
    {
        None,
        Locked,
        Unlocked
    }

    public struct LevelInfo
    {
        public int LevelNumber;
        public LevelStatus LevelStatus;
        public string ID;
    }

    public interface IOnItemClickListener
    {
        void OnItemClick(LevelInfo info);
    }

    public class LevelRecyclableScroller : MonoBehaviour, IRecyclableScrollRectDataSource
    {
        [FormerlySerializedAs("_recyclableScrollRect")] [SerializeField]
        private RecyclableScrollRect recyclableScrollRect;

        [FormerlySerializedAs("_dataLength")] [SerializeField]
        private int dataLength;

        public IOnItemClickListener OnItemClickListener;

        private readonly List<LevelInfo> contactList = new List<LevelInfo>();

        private void Awake()
        {
            InitData();
            recyclableScrollRect.DataSource = this;
        }

        private void InitData()
        {
            contactList?.Clear();

            for (var i = 0; i < dataLength; i++)
            {
                var obj = new LevelInfo
                {
                    LevelNumber = i + 1,
                    LevelStatus = PlayerPrefs.GetInt("CompletedLevels", 0) + 1 >= i + 1
                        ? LevelStatus.Unlocked
                        : LevelStatus.Locked,
                    ID = "item : " + i
                };
                contactList?.Add(obj);
            }
        }

        #region DATA-SOURCE

        /// <summary>
        /// Data source method. return the list length.
        /// </summary>
        public int GetItemCount()
        {
            return contactList.Count;
        }

        /// <summary>
        /// Data source method. Called for a cell every time it is recycled.
        /// Implement this method to do the necessary cell configuration.
        /// </summary>
        public void SetCell(ICell cell, int index)
        {
            //Casting to the implemented Cell
            var item = cell as LevelCell;
            if (item != null) item.ConfigureCell(contactList[index], index, OnItemClickListener);
        }

        #endregion
    }
}