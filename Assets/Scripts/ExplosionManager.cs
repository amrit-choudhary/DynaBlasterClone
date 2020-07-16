using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynaBlasterClone
{
    // Spawns explosions and affects player and enemies
    public class ExplosionManager : MonoBehaviour
    {
        public GameObject explosionPrefab;
        public GameObject firePrefab;
        public float explosionDelay;
        private Map _map;
        private MapDrawer _mapDrawer;
        private EntitiesManager _entitiesManager;
        private GameObject _bomb;
        private Transform _bombTrans;
        private GameObject _fire;
        private Transform _fireTrans;
        private bool _bombPlaced = false;
        private int _bombX, _bombY;

        // Initialize and cache references and pre-spawn bomb prefab for pooling
        public void Init(Map map_, MapDrawer mapDrawer_, EntitiesManager entitiesManager_)
        {
            _map = map_;
            _mapDrawer = mapDrawer_;
            _entitiesManager = entitiesManager_;
            _bomb = Instantiate(explosionPrefab);
            _bombTrans = _bomb.GetComponent<Transform>();
            _bombTrans.SetParent(gameObject.GetComponent<Transform>());
            _bomb.SetActive(false);
            _fire = Instantiate(firePrefab);
            _fireTrans = _fire.GetComponent<Transform>();
            _fireTrans.SetParent(gameObject.GetComponent<Transform>());
            _fire.SetActive(false);
        }

        // Place a bomb in the tile where player is standing
        public void PlaceBomb(Vector2 playerPos_)
        {
            if (_bombPlaced) return;

            _bombPlaced = true;

            _bombX = Mathf.RoundToInt(playerPos_.x);
            _bombY = Mathf.RoundToInt(playerPos_.y);

            _bombTrans.position = new Vector2(_bombX, _bombY);
            _bomb.SetActive(true);

            Invoke("Explode", explosionDelay);
        }

        // Remove all the breakable tiles from the map when explosion happens
        private void Explode()
        {
            _bomb.SetActive(false);

            for (int i = _bombX - 1; i < _bombX + 2; i++)
            {
                for (int j = _bombY - 1; j < _bombY + 2; j++)
                {
                    if (_map.tiles[i][j] == TileType.Breakable)
                    {
                        _map.tiles[i][j] = TileType.Grass;
                    }
                }
            }

            _mapDrawer.Redraw();

            _entitiesManager.Blast(_bombX, _bombY);

            _bombPlaced = false;

            // Spawn the fire vfx after explosion
            _fireTrans.position = new Vector2(_bombX, _bombY);
            _fire.SetActive(true);
            Invoke("TurnOffFire", 0.6f);
        }

        // Turn off fire vfx
        private void TurnOffFire()
        {
            _fire.SetActive(false);
        }
    }
}
