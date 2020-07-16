using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynaBlasterClone
{
    // TileType and sprites collection
    // Enable easy swapping of the level look
    [CreateAssetMenu(fileName = "TileSet")]
    public class TileSet : ScriptableObject
    {
        public Sprite none;
        public Sprite grass;
        public Sprite breakable;
        public Sprite unbreakable;
        public Sprite edge;

        public Sprite GetSprite(TileType tileType_)
        {
            switch (tileType_)
            {
                case TileType.None:
                    return none;
                case TileType.Grass:
                    return grass;
                case TileType.Breakable:
                    return breakable;
                case TileType.Unbreakable:
                    return unbreakable;
                case TileType.Edge:
                    return edge;
            }

            return none;
        }

        public int GetSortingOrder(TileType tileType_)
        {
            switch (tileType_)
            {
                case TileType.None:
                    return 0;
                case TileType.Grass:
                    return 10;
                case TileType.Breakable:
                    return 20;
                case TileType.Unbreakable:
                    return 30;
                case TileType.Edge:
                    return 40;
            }

            return 0;
        }
    }
}
