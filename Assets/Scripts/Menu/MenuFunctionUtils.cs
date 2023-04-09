using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    /// <summary>
    /// Menu Function Utils is used to store functions that are used in the menu.
    /// </summary>
    public class MenuFunctionUtils : MonoBehaviour
    {
        /// <summary>
        /// Load the scene with the given name.
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        
        /// <summary>
        /// Load the scene with the given build index.
        /// </summary>
        /// <param name="sceneBuildIndex"></param>
        public void LoadScene(int sceneBuildIndex)
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }

        /// <summary>
        /// Exit the game.
        /// </summary>
        public void ExitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}