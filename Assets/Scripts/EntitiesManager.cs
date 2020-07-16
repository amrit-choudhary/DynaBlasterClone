using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynaBlasterClone
{
    // Class for populating the board with entities (Player and Enemies) and control entities interaction
    public class EntitiesManager : MonoBehaviour
    {
        public GameObject playerPrefab;
        public List<GameObject> enemyPrefabs;
        private Map _map;                                                   // Cached map reference
        private GameObject _playerObj;
        private Player _player;
        private Transform _playerTrans;
        private Movement _movement;                                         // Cached player movement class
        private Movement _enemyMovement;                                    // Cached player movement class
        private ExplosionManager _explosionManager;
        private int _enemyCounter = 0;
        private List<GameObject> _enemies;
        private GameObject _enemyObj;
        private Transform _enemyTrans;
        private List<Transform> _enemyTransforms = new List<Transform>();   // List of all the current enemies
        private Enemy _enemy;
        private List<Transform> _blastedEnemies = new List<Transform>();
        private Action<GameState> _playerCallback;                          // Callback for notifying player death
        private Action<int> _enemyCallback;                                 // Callback for notifying enemy death

        // Initialize and cache references, spawn player and enemies
        public void Init(Map map_, Movement movement_, Movement enemeyMovement_, ExplosionManager explosionManager_,
            Action<GameState> playerCallback_, Action<int> enemyCallback_)
        {
            _map = map_;
            _movement = movement_;
            _enemyMovement = enemeyMovement_;
            _explosionManager = explosionManager_;
            _playerCallback = playerCallback_;
            _enemyCallback = enemyCallback_;
            SetEntities();
            SpawnEntities();
        }

        private void Update()
        {
            // If player collided with an enemy
            if (CheckAllPlayerEnemyCollision())
            {
                _playerCallback?.Invoke(GameState.PlayerCollided);
            }
        }

        // Set player and enemy positions on the map
        private void SetEntities()
        {
            // Find a suitable position for player
            Vector2Int playerPos = GetPlayerPosition();
            _map.entities[playerPos.x][playerPos.y] = EntityType.Player;

            List<Vector3Int> possibles = new List<Vector3Int>();

            // Find all the possible enemy positions
            for (int i = 1; i < _map.sizeX - 1; i++)
            {
                for (int j = 1; j < _map.sizeY - 1; j++)
                {
                    if (i + j < 8) continue;

                    Vector3Int checkedSlot = CheckEnemySpawn(i, j);

                    if (checkedSlot.z != 0)
                    {
                        possibles.Add(checkedSlot);
                    }
                }
            }

            List<Vector3Int> selected = new List<Vector3Int>();

            // Selecting 5 positions out of all the possible positions
            for (int i = 0; i < enemyPrefabs.Count; i++)
            {
                int rand = UnityEngine.Random.Range(0, possibles.Count);
                selected.Add(possibles[rand]);
                possibles.RemoveAt(rand);
            }

            // Setting initial movement direction of spawned enemies
            for (int i = 0; i < selected.Count; i++)
            {
                if (selected[i].z == 1)
                {
                    _map.entities[selected[i].x][selected[i].y] = EntityType.EnemyUp;
                }

                if (selected[i].z == 2)
                {
                    _map.entities[selected[i].x][selected[i].y] = EntityType.EnemyRight;
                }

                if (selected[i].z == 3)
                {
                    _map.entities[selected[i].x][selected[i].y] = EntityType.EnemyDown;
                }

                if (selected[i].z == 4)
                {
                    _map.entities[selected[i].x][selected[i].y] = EntityType.EnemyLeft;
                }
            }
        }

        // Check if an enemy can be spawned here
        // The tile has to be empty and one of the adjacent tile has to be empty too
        // Direction of empty adjacent tile, determines the movement direction
        // If right tile is empty, initial movement direction will be to the right
        private Vector3Int CheckEnemySpawn(int i_, int j_)
        {
            Vector3Int result = Vector3Int.zero;

            if (_map.tiles[i_][j_] != TileType.Grass) return result;

            bool anySideEmpty = false;

            if (_map.tiles[i_ + 0][j_ + 1] == TileType.Grass)
            {
                result = new Vector3Int(i_, j_, 1);
                anySideEmpty = true;
            }

            if (_map.tiles[i_ + 1][j_ + 0] == TileType.Grass)
            {
                result = new Vector3Int(i_, j_, 2);
                anySideEmpty = true;
            }

            if (_map.tiles[i_ + 0][j_ - 1] == TileType.Grass)
            {
                result = new Vector3Int(i_, j_, 3);
                anySideEmpty = true;
            }

            if (_map.tiles[i_ - 1][j_ + 0] == TileType.Grass)
            {
                result = new Vector3Int(i_, j_, 4);
                anySideEmpty = true;
            }

            if (anySideEmpty)
            {
                return result;
            }
            else
            {
                return Vector3Int.zero;
            }
        }

        // Spawn and initialize all the entities and set their initial direction
        private void SpawnEntities()
        {
            for (int i = 0; i < _map.sizeX; i++)
            {
                for (int j = 0; j < _map.sizeY; j++)
                {
                    if (_map.entities[i][j] == EntityType.Player)
                    {
                        _playerObj = Instantiate(playerPrefab);
                        _player = _playerObj.GetComponent<Player>();
                        _player.Init(_movement, _explosionManager);
                        _playerTrans = _playerObj.GetComponent<Transform>();
                        _playerTrans.position = new Vector2(GameManager.tileSizeX * i, GameManager.tileSizeY * j);
                    }

                    if (_map.entities[i][j] == EntityType.EnemyUp ||
                        _map.entities[i][j] == EntityType.EnemyRight ||
                        _map.entities[i][j] == EntityType.EnemyDown ||
                        _map.entities[i][j] == EntityType.EnemyLeft)
                    {
                        _enemyObj = Instantiate(enemyPrefabs[_enemyCounter]);
                        _enemy = _enemyObj.GetComponent<Enemy>();
                        _enemyTrans = _enemyObj.GetComponent<Transform>();
                        _enemyTrans.position = new Vector2(GameManager.tileSizeX * i, GameManager.tileSizeY * j);
                        _enemyTransforms.Add(_enemyTrans);
                        _enemyCounter++;

                        switch (_map.entities[i][j])
                        {
                            case EntityType.EnemyUp: _enemy.Init(_enemyMovement, Vector2Int.up); break;
                            case EntityType.EnemyRight: _enemy.Init(_enemyMovement, Vector2Int.right); break;
                            case EntityType.EnemyDown: _enemy.Init(_enemyMovement, Vector2Int.down); break;
                            case EntityType.EnemyLeft: _enemy.Init(_enemyMovement, Vector2Int.left); break;
                        }
                    }
                }
            }
        }

        public void Blast(int blastX_, int blastY_)
        {
            // If player was affected by the blast
            if (CheckForBlast(_playerTrans.position, blastX_, blastY_))
            {
                _playerCallback?.Invoke(GameState.PlayerBlasted);
            }

            _blastedEnemies.Clear();

            // Find all the enemies affected by the blast
            for (int i = 0; i < _enemyTransforms.Count; i++)
            {
                if (CheckForBlast(_enemyTransforms[i].position, blastX_, blastY_))
                {
                    _blastedEnemies.Add(_enemyTransforms[i]);
                }
            }

            // Remove blasted enemies from the enemy list and Destory them
            for (int i = 0; i < _blastedEnemies.Count; i++)
            {
                _enemyTransforms.Remove(_blastedEnemies[i]);
                Destroy(_blastedEnemies[i].gameObject);
            }

            // If any enemy was destoryed
            if (_blastedEnemies.Count > 0)
            {
                _enemyCallback?.Invoke(_enemyTransforms.Count);
            }
        }

        // Check if and enemy or player was in 3x3 tile vicinity of the blast
        private bool CheckForBlast(Vector2 pos_, int blastX_, int blastY_)
        {
            int x = Mathf.RoundToInt(pos_.x);
            int y = Mathf.RoundToInt(pos_.y);

            if (x <= blastX_ + 1 && x >= blastX_ - 1 &&
                y <= blastY_ + 1 && y >= blastY_ - 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Check player-enemy collision for all the enemies
        private bool CheckAllPlayerEnemyCollision()
        {
            for (int i = 0; i < _enemyTransforms.Count; i++)
            {
                if (CheckPlayerEnemyCollision(_enemyTransforms[i])) return true;
            }

            return false;
        }

        // Check if player and an enemy are on the same tile
        private bool CheckPlayerEnemyCollision(Transform enemyTrans_)
        {
            Vector3 pos_ = _playerTrans.position;
            int x = Mathf.RoundToInt(pos_.x);
            int y = Mathf.RoundToInt(pos_.y);

            Vector3 enemyPos_ = enemyTrans_.position;
            int enemyX = Mathf.RoundToInt(enemyPos_.x);
            int enemyY = Mathf.RoundToInt(enemyPos_.y);

            if (x == enemyX && y == enemyY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Find a suitable tile for the player, starting from the bottom right corner, find an empty tile
        public Vector2Int GetPlayerPosition()
        {
            for (int i = 0; i < _map.sizeX; i++)
            {
                for (int j = 0; j < _map.sizeY; j++)
                {
                    if (_map.tiles[i][j] == TileType.Grass)
                    {
                        return new Vector2Int(i, j);
                    }
                }
            }

            return Vector2Int.zero;
        }
    }
}
