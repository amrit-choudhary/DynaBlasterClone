using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynaBlasterClone
{
    // Class for generating the map
    public class MapGenerator
    {
        private int _maxTilesX = 0;
        private int _maxTilesY = 0;
        private float _noiseAmp = 0.0f;     // Amplitude for Perlin Noise
        private float _noiseCutoff = 0.0f;  // Cutoff value for the Perlin Noise Function

        public MapGenerator()
        {
            _maxTilesX = 16;
            _maxTilesY = 12;
            _noiseAmp = 0.21f;
            _noiseCutoff = 0.24f;
        }

        public MapGenerator(int maxTilesX_, int maxTilesY_, float noiseAmp_, float noiseCutoff_)
        {
            _maxTilesX = maxTilesX_;
            _maxTilesY = maxTilesY_;
            _noiseAmp = noiseAmp_;
            _noiseCutoff = noiseCutoff_;
        }

        // Map generation algo:
        // 1. Fill everything as a grass tile
        // 2. Fill the edge tiles
        // 3. Fill the unbreakable tiles as they have a fixed pattern
        // 4. Using a random offset, use a perlin noise over the grid
        // using the x, y coordinates of the grid as perlin noise input
        // 5. Assign the tiles as breakable if the noise output is less than the cutoff
        public Map GenerateMap()
        {
            List<List<TileType>> _tiles = new List<List<TileType>>();
            List<List<EntityType>> _entities = new List<List<EntityType>>();

            // Initialize the tiles and enitites list of lists
            for (int i = 0; i < _maxTilesX + 3; i++)
            {
                _tiles.Add(new List<TileType>());
                _entities.Add(new List<EntityType>());

                for (int j = 0; j < _maxTilesY + 3; j++)
                {
                    _tiles[i].Add(TileType.None);
                    _entities[i].Add(EntityType.None);
                }
            }

            // Fill tiles
            for (int i = 0; i < _maxTilesX + 3; i++)
            {
                for (int j = 0; j < _maxTilesY + 3; j++)
                {
                    int origin = Random.Range(0, 1000);
                    float f = Mathf.PerlinNoise(origin + i * _noiseAmp, origin + j * _noiseAmp);

                    if (f < _noiseCutoff || f > 1 - _noiseCutoff)
                    {
                        _tiles[i][j] = TileType.Breakable;
                    }
                    else
                    {
                        _tiles[i][j] = TileType.Grass;
                    }

                    if (i == 0 || j == 0 || i == _maxTilesX + 2 || j == _maxTilesY + 2)
                    {
                        _tiles[i][j] = TileType.Edge;
                    }
                    else if (i % 2 == 0 && j % 2 == 0)
                    {
                        _tiles[i][j] = TileType.Unbreakable;
                    }
                }
            }

            return new Map(_tiles, _entities);
        }
    }
}
