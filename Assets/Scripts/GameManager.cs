using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynaBlasterClone
{
    public enum GameState
    {
        None,
        Playing,
        PlayerBlasted,
        PlayerCollided,
        TimeRanOut,
        EnemiesDead,
    }

    // Class for initializing the game system and receive callback from the system.
    // Will update UI when callbacks are received or game state changes
    public class GameManager : MonoBehaviour
    {
        public UIManager uiManager;
        public static float tileSizeX = 1.0f;
        public static float tileSizeY = 1.0f;
        public MapDrawer mapDrawer;
        public TileSet tileSet;
        private Map _map;
        public EntitiesManager entitiesManager;
        public ExplosionManager explosionManager;
        public Timer timer;
        public int enemiesCount = 5;
        private MapGenerator _mapGenerator;
        private Player _player;
        private Movement _movement;
        private Movement _enemyMovement;
        private int _enemiesLeftCount = 0;
        private GameState gameState = GameState.None;
        public int scorePerEnemy = 100;
        public int levelTime = 200;
        private int _score = 0;
        private int _highScore = 0;
        private const string _highScoreKey = "HighScore";   // Key for saving highscore in PlayerPrefs

        private void Start()
        {
            NewGame();
        }

        // Initialize game system and pass callbacks
        private void NewGame()
        {
            _mapGenerator = new MapGenerator();
            _map = _mapGenerator.GenerateMap();
            mapDrawer.Init(tileSet, _map);
            _movement = new Movement(_map);
            _enemyMovement = new Movement(_map);
            entitiesManager.Init(_map, _movement, _enemyMovement, explosionManager, PlayerCallback, EnemyCallback);
            explosionManager.Init(_map, mapDrawer, entitiesManager);
            timer.Init(levelTime, UpdateTimer);
            _enemiesLeftCount = 5;
            uiManager.UpdateHighscore(PlayerPrefs.GetInt(_highScoreKey, 0));
            gameState = GameState.Playing;
        }

        // Callback for player events, player getting blasted for collision with enemy
        // Comes from EntitiesManager after collision check
        public void PlayerCallback(GameState gameState_)
        {
            if (gameState_ == GameState.PlayerCollided) PlayerEnemyCollided();
            if (gameState_ == GameState.PlayerBlasted) PlayerBlasted();
        }

        // Callback for enemy events
        // When enemies are destoryed by blast
        public void EnemyCallback(int count_)
        {
            EnemyBlasted(count_);
        }

        // If player killed by enemy
        private void PlayerEnemyCollided()
        {
            if (gameState != GameState.Playing) return;

            gameState = GameState.PlayerCollided;

            // Show fail screen
            uiManager.ShowResultScreen(gameState);
        }

        // If player killed by blast
        private void PlayerBlasted()
        {
            if (gameState != GameState.Playing) return;

            gameState = GameState.PlayerBlasted;

            // Show fail screen
            uiManager.ShowResultScreen(gameState);
        }

        // If enemy killed
        private void EnemyBlasted(int count_)
        {
            if (gameState != GameState.Playing) return;

            _enemiesLeftCount = count_;
            ManageScore();

            // If all enemies killed
            // Show win screen
            if (count_ == 0)
            {
                gameState = GameState.EnemiesDead;
                uiManager.ShowResultScreen(gameState);
            }
        }

        // Updated every second from Timer script
        private void UpdateTimer(int time_)
        {
            if (gameState != GameState.Playing) return;

            uiManager.UpdateTimer(time_);

            if (time_ == 0)
            {
                gameState = GameState.TimeRanOut;
                uiManager.ShowResultScreen(gameState);
            }
        }

        // Calucate and update score in UI and manage highscore saving and UI update
        private void ManageScore()
        {
            _score = (enemiesCount - _enemiesLeftCount) * scorePerEnemy;
            uiManager.UpdateScore(_score);
            _highScore = PlayerPrefs.GetInt(_highScoreKey, 0);

            // If highscore was beaten
            if (_score > _highScore)
            {
                _highScore = _score;
                PlayerPrefs.SetInt(_highScoreKey, _highScore);
                uiManager.UpdateHighscore(_highScore);
            }
        }
    }
}
