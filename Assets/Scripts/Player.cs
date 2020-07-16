using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynaBlasterClone
{
    // Player class for input, movement and changing the look based on movement direction
    public class Player : MonoBehaviour
    {
        public float speed = 1.0f;
        public float lerpFactor = 0.1f;
        public SpriteRenderer playerRenderer;
        public Sprite upSprite, rightSprite, downSprite, leftSprite;
        private Vector2 _velocity = new Vector2();
        private Movement _movement;
        private ExplosionManager _explosionManger;
        private Transform _trans;
        private Vector3 _targetPosition;
        private float _inputX, _inputY;
        private Vector2Int _directionVector = Vector2Int.zero;

        public void Init(Movement movement_, ExplosionManager explosionManger_)
        {
            _movement = movement_;
            _explosionManger = explosionManger_;
            _trans = gameObject.GetComponent<Transform>();
        }

        private void Update()
        {
            _inputX = Input.GetAxis("Horizontal");

            if (_inputX > 0) _inputX = Mathf.Clamp(_inputX, 0.2f, 1.0f);
            if (_inputX < 0) _inputX = Mathf.Clamp(_inputX, -1.0f, -0.2f);

            _inputY = Input.GetAxis("Vertical");

            if (_inputY > 0) _inputY = Mathf.Clamp(_inputY, 0.2f, 1.0f);
            if (_inputY < 0) _inputY = Mathf.Clamp(_inputY, -1.0f, -0.2f);

            _velocity.x = _inputX * speed;
            _velocity.y = _inputY * speed;

            // Change sprite based on the final movement direction
            if (_velocity.sqrMagnitude > 0.4f)
            {
                _trans.position = _movement.TryMove(_trans.position, _velocity, out _directionVector);

                if (_directionVector == Vector2Int.up) playerRenderer.sprite = upSprite;
                if (_directionVector == Vector2Int.right) playerRenderer.sprite = rightSprite;
                if (_directionVector == Vector2Int.down) playerRenderer.sprite = downSprite;
                if (_directionVector == Vector2Int.left) playerRenderer.sprite = leftSprite;
            }
            else
            {
                _trans.position = _movement.TrySnap(_trans.position);
                playerRenderer.sprite = downSprite;
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                _explosionManger.PlaceBomb(_trans.position);
            }
        }
    }
}
