using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynaBlasterClone
{
    // Types of level tiles
    public enum TileType
    {
        None,
        Grass,
        Breakable,
        Unbreakable,
        Edge,
    }

    // Types of entities in the level
    // EnemyUp is an enemy with initial movement direction in up direction
    public enum EntityType
    {
        None,
        Player,
        Enemy,
        EnemyUp,
        EnemyRight,
        EnemyDown,
        EnemyLeft,
        Exit,
    }

    // Map class from the level and entities layout
    // Stores the tiles and entities in a 2D list
    // tiles[i][j], i is in x direction, increasing toward the right
    // j is y direction, increasing towards the top
    public class Map
    {
        private int _sizeX = 0;
        private int _sizeY = 0;
        private List<List<TileType>> _tiles = new List<List<TileType>>();
        private List<List<EntityType>> _entities = new List<List<EntityType>>();
        public int sizeX { get { return _sizeX; } }
        public int sizeY { get { return _sizeY; } }
        public List<List<TileType>> tiles { get { return _tiles; } }
        public List<List<EntityType>> entities { get { return _entities; } }


        public Map()
        {
            _sizeX = 0;
            _sizeY = 0;
        }

        public Map(List<List<TileType>> tiles_, List<List<EntityType>> entities_)
        {
            _tiles = tiles_;
            _sizeX = _tiles.Count;

            if (_sizeX != 0)
            {
                _sizeY = _tiles[0].Count;
            }
            else
            {
                _sizeY = 0;
            }

            _entities = entities_;
        }
    }
}
