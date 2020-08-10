using System;
using UnityEngine;

public class TickTimer
{
    private float _currentTimeSec;
    private float _tickSec;
    private Action _onTick;
    public TickTimer(float tickSec, Action onTick)
    {
        _tickSec = tickSec;
        _onTick = onTick;
    }

    public void UpdateTime()
    {
        _currentTimeSec += Time.deltaTime;
        if (_currentTimeSec >= _tickSec)
        {
            _onTick();
            _currentTimeSec = 0f;
        }
    }
}