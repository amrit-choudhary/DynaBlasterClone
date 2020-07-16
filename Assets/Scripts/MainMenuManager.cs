using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DynaBlasterClone
{
    public class MainMenuManager : MonoBehaviour
    {
        public void Play()
        {
            SceneManager.LoadScene("Game");
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
