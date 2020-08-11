using System;
using UnityEngine;

namespace KowaiKit
{
    public class TickTimer
    {
        private float _currentTimeSec;
        private float _tickSec;
        private Action _onTick;
        private bool _isEnabled;

        public TickTimer(float tickSec, Action onTick)
        {
            _tickSec = tickSec;
            _onTick = onTick;
            _isEnabled = false;
        }

        public void Start()
        {
            _isEnabled = true;
        }

        public void Stop()
        {
            _isEnabled = false;
        }

        public void UpdateTime()
        {
            if (!_isEnabled)
                return;

            _currentTimeSec += Time.deltaTime;
            if (_currentTimeSec >= _tickSec)
            {
                _onTick();
                _currentTimeSec = 0f;
            }
        }
    }
}