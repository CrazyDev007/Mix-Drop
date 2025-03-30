using System.Collections.Generic;
using PolyAndCode.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public struct LevelInfo
    {
        public string Name;
        public string Gender;
        public string ID;
    }

    public interface IOnItemClickListener
    {
        void OnItemClick(LevelCell cell);
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

            string[] genders = { "Male", "Female" };
            for (var i = 0; i < dataLength; i++)
            {
                var obj = new LevelInfo
                {
                    Name = i + "_Name",
                    Gender = genders[Random.Range(0, 2)],
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