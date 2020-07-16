using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynaBlasterClone
{
    // Drawing tiles on the screen based on the map created by the MapGenerator
    public class MapDrawer : MonoBehaviour
    {
        public Transform mapParent;
        public GameObject tilePrefab;
        private TileSet _tileSet;
        private Map _map;
        private List<List<GameObject>> _tiles = new List<List<GameObject>>();

        // Instantite the tiles and assign sprites based on TileType.
        // TileSet return sprites based on TileType
        public void Init(TileSet tileSet_, Map map_)
        {
            _tileSet = tileSet_;
            _map = map_;

            for (int i = 0; i < _map.sizeX; i++)
            {
                _tiles.Add(new List<GameObject>());

                for (int j = 0; j < _map.sizeY; j++)
                {
                    GameObject tile = GameObject.Instantiate(tilePrefab, Vector2.zero, Quaternion.identity);
                    _tiles[i].Add(tile);
                    Transform tileTrans = tile.GetComponent<Transform>();
                    tileTrans.SetParent(mapParent);
                    tileTrans.position = new Vector2(GameManager.tileSizeX * i, GameManager.tileSizeY * j);
                    SpriteRenderer rend = tile.GetComponent<SpriteRenderer>();
                    rend.sprite = _tileSet.GetSprite(_map.tiles[i][j]);
                    rend.sortingOrder = _tileSet.GetSortingOrder(_map.tiles[i][j]);
                }
            }

            Redraw();
        }

        // Redraw the tiles if the map changes, like after explosion
        public void Redraw()
        {
            for (int i = 0; i < _map.sizeX; i++)
            {
                for (int j = 0; j < _map.sizeY; j++)
                {
                    GameObject tile = _tiles[i][j];
                    SpriteRenderer rend = tile.GetComponent<SpriteRenderer>();
                    rend.sprite = _tileSet.GetSprite(_map.tiles[i][j]);
                    rend.sortingOrder = _tileSet.GetSortingOrder(_map.tiles[i][j]);
                }
            }
        }
    }
}
