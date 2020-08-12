using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace KowaiKit
{
    [RequireComponent(typeof(Light))]
    public class FlashLight : MonoBehaviour
    {
        [SerializeField] public bool IsOn = true;
        [SerializeField] public int MaxBattery = 100;
        [SerializeField] public int RemainBattery = 100;
        [SerializeField] public float BatteryDecreaseSec = 1;
        [SerializeField] public int BatteryDecreaseAmount = 2;

        public event Action<int> OnUpdateBattery;
        private Light _light;
        private float _defaultIntensity;

        private TickTimer _timerBatteryDecrease;

        void Start()
        {
            _light = GetComponent<Light>();
            _timerBatteryDecrease = new TickTimer(BatteryDecreaseSec, () =>
            {
                RemainBattery -= BatteryDecreaseAmount;
                RemainBattery = RemainBattery < 0 ? 0 : RemainBattery;
                if (RemainBattery == 0)
                {
                    _light.range = 0;
                }
                OnUpdateBattery(RemainBattery);
            });
            _timerBatteryDecrease.Start();
            _defaultIntensity = _light.intensity;
        }

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

        public void Charge(int amount)
        {
            RemainBattery += amount;
            RemainBattery = RemainBattery > MaxBattery ? MaxBattery : RemainBattery;
            OnUpdateBattery(RemainBattery);
        }

        // Update is called once per frame
        void Update()
        {
            if (IsOn)
            {
                _timerBatteryDecrease.UpdateTime();
            }
        }

    }

}