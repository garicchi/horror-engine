using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class FlashLightController : MonoBehaviour
{
    private int _remainBattery;
    private bool _isOn;
    private const int _batteryMax = 100;
    private Light _light;
    private const float _range = 30;

    public int RemainBattery
    {
        get => _remainBattery;
        set => _remainBattery = value;
    }

    private TickTimer _timerBatteryDecrease;
    void Start()
    {
        _remainBattery = _batteryMax;
        _isOn = false;
        _light = gameObject.GetComponent<Light>();
        _timerBatteryDecrease = new TickTimer(2f, () =>
        {
            _remainBattery -= 2;
            _remainBattery = _remainBattery < 0 ? 0 : _remainBattery;
            if (_remainBattery == 0)
            {
                _light.range = 0;
            }
        });
        _timerBatteryDecrease.Start();
        _light.range = 0;
    }

    public void ToggleSwitch()
    {
        _isOn = !_isOn;
        if (_isOn && _remainBattery > 0)
        {
            _light.range = _range;
        }
        else
        {
            _light.range = 0;
        }
    }

    public void Charge(int amount)
    {
        RemainBattery += amount;
        RemainBattery = RemainBattery > _batteryMax ? _batteryMax : RemainBattery;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isOn)
        {
            _timerBatteryDecrease.UpdateTime();
        }
    }

}
