using Character;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wall;

namespace Editor.Menu
{
    public class CheatCommand : MonoBehaviour
    {
        [MenuItem("Cheats/Destructible Walls/Destroy All Walls")]
        private static void DestroyAllWalls()
        {
            if (!Application.isPlaying)
            {
                Debug.Log("You must be in play mode to use this command");
                return;
            }
            
            if (SceneManager.GetActiveScene().buildIndex != 2)
            {
                Debug.Log("You must be in the Main Scene to use this command");
                return;
            }

            GameObject[] walls = GameObject.FindGameObjectsWithTag("Destructible");
            
            foreach (GameObject wall in walls)
            {
                wall.GetComponent<DestructibleWall>().DestroyWallWithoutEffect();
            }
            
            Debug.Log("All walls have been destroyed");
        }
    }
}