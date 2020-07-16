using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynaBlasterClone
{
    // Enemy movement class
    public class Enemy : MonoBehaviour
    {
        public float speed;
        private Vector2Int _cachedDirection = Vector2Int.zero;  // Caching direction for reversing after collision
        private Vector2Int _directionVector = Vector2Int.zero;  // Current movement direction
        private Transform _trans;
        private Movement _movement;
        private Vector2 _velocity = Vector2.zero;               // Current velocity

        // Initialize direction and cache components
        public void Init(Movement movement_, Vector2Int directionVector_)
        {
            _movement = movement_;
            _directionVector = directionVector_;
            _cachedDirection = directionVector_;
            _trans = GetComponent<Transform>();
        }

        // Movement
        private void Update()
        {
            _velocity = new Vector2(_directionVector.x * speed, _directionVector.y * speed);
            _trans.position = _movement.TryMove(_trans.position, _velocity, out _directionVector);

            // Reverse direction after collsion
            if (_directionVector == Vector2Int.zero)
            {
                _directionVector = new Vector2Int(_cachedDirection.x * -1, _cachedDirection.y * -1);
                _cachedDirection = _directionVector;
            }
        }
    }
}
