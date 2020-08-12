using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using KowaiKit;
using UnityEngine;
using UnityEngine.UI;

namespace KowaiKit
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] public ProgressBar ProgressBarBattery;

        [SerializeField] public LabelText TextMissionItemStatus;

        [SerializeField] public Image ImageGameClear;

        [SerializeField] public Image ImageGameOver;

        // Start is called before the first frame update
        void Start()
        {
            ImageGameClear.gameObject.SetActive(false);
            ImageGameOver.gameObject.SetActive(false);
        }

        public void UpdateItemStatus(int itemCount, int missionItemCount)
        {
            TextMissionItemStatus.Value.text = $"{itemCount}/{missionItemCount} GET";
        }

        public void UpdateFlashLightBattery(int battery)
        {
            ProgressBarBattery.Progress = battery / 100f;
        }

        public void ShowGameFinishView(bool isClear)
        {
            if (isClear)
            {
                ImageGameClear.gameObject.SetActive(true);
            }
            else
            {
                ImageGameOver.gameObject.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
