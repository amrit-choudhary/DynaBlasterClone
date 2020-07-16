using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynaBlasterClone
{
    // Maintains and updates game time
    public class Timer : MonoBehaviour
    {
        private bool _initialized = false;
        private int _levelTime = 0;
        private float _accumulator = 0.0f;
        private Action<int> _timerCallback;     // Callback for updating game manager when a second passess

        public void Init(int levelTime_, Action<int> timerCallback_)
        {
            _levelTime = levelTime_;
            _accumulator = 0;
            _timerCallback = timerCallback_;

            _timerCallback?.Invoke(_levelTime);
            _initialized = true;
        }

        private void Update()
        {
            if (!_initialized) return;

            _accumulator += Time.deltaTime;

            if (_accumulator > 1.0f)
            {
                _accumulator = 0.0f;
                _levelTime--;
                if (_levelTime < 0) _levelTime = 0;
                _timerCallback?.Invoke(_levelTime);
            }
        }
    }
}