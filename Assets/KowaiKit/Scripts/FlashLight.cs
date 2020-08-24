using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace KowaiKit
{
    /// <summary>
    /// ハンドライトを制御するスクリプト
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class FlashLight : MonoBehaviour
    {
        /// <summary>
        /// inspectorで設定したいフィールド
        /// </summary>
        [SerializeField] public bool IsOn = true;  // ライトががオンかオフか
        [SerializeField] public int MaxBattery = 100;  // バッテリー最大値
        [SerializeField] public int RemainBattery = 100;  // バッテリー残量
        [SerializeField] public float BatteryDecreaseSec = 1;  // バッテリーが減る秒数
        [SerializeField] public int BatteryDecreaseAmount = 2;  // バッテリーの減少量

        /// <summary>
        /// イベント
        /// </summary>
        public event Action<int> OnUpdateBattery;  // バッテリーが増減したとき

        public event Action OnLostBattery;
        
        /// <summary>
        /// privateフィールド
        /// </summary>
        private Light _light;
        private float _defaultIntensity;
        private TickTimer _timerBatteryDecrease;

        /// <summary>
        /// 初期化
        /// </summary>
        void Start()
        {
            _light = GetComponent<Light>();
            // バッテリーの減少タイマー
            _timerBatteryDecrease = new TickTimer(BatteryDecreaseSec, () =>
            {
                RemainBattery -= BatteryDecreaseAmount;
                RemainBattery = RemainBattery < 0 ? 0 : RemainBattery;
                if (RemainBattery == 0)
                {
                    _light.intensity = 0;
                    OnLostBattery();
                }
                OnUpdateBattery(RemainBattery);
            });
            _timerBatteryDecrease.Start();
            _defaultIntensity = _light.intensity;
        }

        /// <summary>
        /// スイッチのトグル
        /// </summary>
        public void ToggleSwitch()
        {
            IsOn = !IsOn;
            if (IsOn && RemainBattery > 0)
            {
                _light.intensity = _defaultIntensity;
            }
            else
            {
                _light.intensity = 0;
            }
        }

        /// <summary>
        /// バッテリーチャージ
        /// </summary>
        /// <param name="amount"></param>
        public void Charge(int amount)
        {
            RemainBattery += amount;
            RemainBattery = RemainBattery > MaxBattery ? MaxBattery : RemainBattery;
            OnUpdateBattery(RemainBattery);
        }

        /// <summary>
        /// フレームごとの処理
        /// </summary>
        void Update()
        {
            if (IsOn)
            {
                _timerBatteryDecrease.UpdateTime();
            }
        }

    }

}