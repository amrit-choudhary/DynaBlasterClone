using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace DynaBlasterClone
{
    // Updates HUD and shows result screen
    public class UIManager : MonoBehaviour
    {
        public TextMeshProUGUI scoreText, highScoreText, timerText, lifeText;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public string winText, lossText;
        public string playerBlastedText, playerCollidedText, timeRanOutText, enemiesDeadText;
        public GameObject resultPanel;
        private string tag1 = "<mspace=0.6em>", tag2 = "<mspace=0.3em>", tag3 = "</mspace>";

        public void UpdateScore(int score_)
        {
            scoreText.text = score_.ToString();
        }

        public void UpdateHighscore(int highScore_)
        {
            highScoreText.text = highScore_.ToString();
        }

        public void UpdateTimer(int time_)
        {
            int seconds = (int)(time_ % 60);
            int minutes = (int)((time_ / 60) % 60);

            timerText.text = minutes.ToString() + tag3
                + tag2 + ":" + tag3
                + tag1 + seconds.ToString("00") + tag3;
        }

        // Show result scree and sets result text based on game state
        public void ShowResultScreen(GameState gameState_)
        {
            switch (gameState_)
            {
                case GameState.PlayerBlasted:
                    {
                        titleText.text = lossText;
                        descriptionText.text = playerBlastedText;
                        break;
                    }
                case GameState.PlayerCollided:
                    {
                        titleText.text = lossText;
                        descriptionText.text = playerCollidedText;
                        break;
                    }
                case GameState.TimeRanOut:
                    {
                        titleText.text = lossText;
                        descriptionText.text = timeRanOutText;
                        break;
                    }
                case GameState.EnemiesDead:
                    {
                        titleText.text = winText;
                        descriptionText.text = enemiesDeadText;
                        break;
                    }
            }

            lifeText.text = 0.ToString();
            resultPanel.SetActive(true);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene("Game");
        }

        public void ExitGame()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
