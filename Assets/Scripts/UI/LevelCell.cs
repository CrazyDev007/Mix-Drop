using PolyAndCode.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Cell class for demo. A cell in Recyclable Scroll Rect must have a cell class inheriting from ICell.
//The class is required to configure the cell(updating UI elements etc.) according to the data during recycling of cells.
//The configuration of a cell is done through the DataSource SetCellData method.
//Check RecyclableScrollerDemo class
namespace UI
{
    public class LevelCell : MonoBehaviour, ICell
    {
        //UI
        public TextMeshProUGUI levelNumberText;
        [SerializeField] private GameObject lockImage;

        private IOnItemClickListener mListener;

        //Model
        private LevelInfo mContactInfo;

        public int CellIndex { get; private set; }

        private void Start()
        {
            //Can also be done in the inspector
            GetComponent<Button>().onClick.AddListener(ButtonListener);
        }

        //This is called from the SetCell method in DataSource
        public void ConfigureCell(LevelInfo contactInfo, int cellIndex, IOnItemClickListener listener)
        {
            mListener = listener;
            CellIndex = cellIndex;
            mContactInfo = contactInfo;
            switch (mContactInfo.LevelStatus)
            {
                case LevelStatus.Locked:
                    levelNumberText.text = "";
                    lockImage.SetActive(true);
                    break;
                case LevelStatus.Unlocked:
                    levelNumberText.text = mContactInfo.LevelNumber.ToString();
                    lockImage.SetActive(false);
                    break;
                case LevelStatus.None:
                default:
                    break;
            }
        }


        private void ButtonListener()
        {
            mListener.OnItemClick(mContactInfo);
        }
    }
}