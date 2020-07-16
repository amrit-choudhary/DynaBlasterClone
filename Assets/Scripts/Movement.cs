using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynaBlasterClone
{
    // Movement class for grid based movement for player and enemies
    public class Movement
    {
        private Map _map;
        private float _gridSnapCutoff = 0.2f;                       // Snap when on grid lines
        private float _onAxisCutoff = 0.3f;                         // Determine is player is on the grid
        private float _inputCutoff = 0.2f;
        private float _collisionCheckDistance = 1.05f;              // Minimum distance for collision with map tiles
        private int _gridX, _gridY;
        private float _deltaX, _deltaY;
        private bool _onAxisX, _onAxisY;
        private Vector2 _result = Vector2.zero;
        private Vector2Int _directionVector = Vector2Int.zero;
        private Vector2Int _collisionCheckVector = Vector2Int.zero;
        private float _seraparation;

        public Movement(Map map_)
        {
            _map = map_;
        }

        // Try to move an enemy or player on the grid, based on it's position and velocity
        // Will check for collision and if movement is possible on the grid
        // will return the new position
        public Vector2 TryMove(Vector2 pos_, Vector2 velocity_, out Vector2Int direction_)
        {
            if (velocity_.sqrMagnitude < _inputCutoff)
            {
                direction_ = Vector2Int.zero;
                return pos_;
            }

            // Finding out values relative to the grid
            _gridX = Mathf.RoundToInt(pos_.x);
            _gridY = Mathf.RoundToInt(pos_.y);
            _deltaX = Mathf.Abs(pos_.x - _gridX);
            _deltaY = Mathf.Abs(pos_.y - _gridY);
            _onAxisX = (_deltaX < _onAxisCutoff);
            _onAxisY = (_deltaY < _onAxisCutoff);

            // Finding if grid movement is possible
            if (Mathf.Abs(velocity_.x) > _inputCutoff)
            {
                if (_onAxisY)
                {
                    _result.x = pos_.x + velocity_.x * Time.deltaTime;
                    _result.y = _gridY;
                    _directionVector = (velocity_.x > 0) ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                }
            }
            else
            {
                if (_onAxisX)
                {
                    _result.x = _gridX;
                    _result.y = pos_.y + velocity_.y * Time.deltaTime;
                    _directionVector = (velocity_.y > 0) ? new Vector2Int(0, 1) : new Vector2Int(0, -1);
                }
            }

            float separation = CheckCollision(pos_, _directionVector);

            // Checking if movement is not blocked by a collision with other tiles
            if (separation < _collisionCheckDistance)
            {
                direction_ = Vector2Int.zero;
                return pos_;
            }
            else
            {
                direction_ = _directionVector;
                return _result;
            }
        }

        // Check collision by checking if next tile in the movement direction is empty
        private float CheckCollision(Vector2 pos_, Vector2Int directionVector_)
        {
            int i = Mathf.Clamp(Mathf.FloorToInt(pos_.x) + directionVector_.x, 0, _map.sizeX - 1);
            int j = Mathf.Clamp(Mathf.FloorToInt(pos_.y) + directionVector_.y, 0, _map.sizeX - 1);

            _collisionCheckVector = new Vector2Int(i, j);

            if (_map.tiles[_collisionCheckVector.x][_collisionCheckVector.y] != TileType.Grass)
            {
                return (_collisionCheckVector.x - pos_.x) * directionVector_.x + (_collisionCheckVector.y - pos_.y) * directionVector_.y;
            }
            else
            {
                return float.PositiveInfinity;
            }
        }

        // Snap the movement to the grid lines when it's sufficiently close enough
        public Vector2 TrySnap(Vector2 result_)
        {
            _gridX = Mathf.RoundToInt(result_.x);
            _gridY = Mathf.RoundToInt(result_.y);
            _deltaX = Mathf.Abs(result_.x - _gridX);
            _deltaY = Mathf.Abs(result_.y - _gridY);

            if (_deltaX < _gridSnapCutoff) result_.x = _gridX;
            if (_deltaY < _gridSnapCutoff) result_.y = _gridY;

            return result_;
        }
    }
}
