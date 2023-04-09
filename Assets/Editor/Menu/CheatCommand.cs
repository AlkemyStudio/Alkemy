using Player;
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
        
        [MenuItem("Cheats/Players/Effects/Enable Push Bomb Effect")]
        private static void EnablePushBombEffect()
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

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            
            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerEffects>().SetPushBombEffect(true);
            }
            
            Debug.Log("Push bomb effect has been enabled for all players");
        }

        [MenuItem("Tests/Push Bomb Effect")]
        private static void SetupPushBombEffectTestZone()
        {
            DestroyAllWalls();
            EnablePushBombEffect();
            AddAllBonusOnPlayer();
        }

        [MenuItem("Cheats/Players/Add All Bonus")]
        private static void AddAllBonusOnPlayer()
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

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            
            foreach (GameObject player in players)
            {
                PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
                playerMovement.AddSpeedBonus();
                playerMovement.AddSpeedBonus();
                
                PlayerBombController playerBombController = player.GetComponent<PlayerBombController>();
                playerBombController.AddBombAmount();
                playerBombController.AddBombAmount();
                playerBombController.AddBombAmount();
                playerBombController.AddBombAmount();
                playerBombController.AddBombAmount();
                playerBombController.AddBombAmount();
                
                playerBombController.AddBombPower();
                playerBombController.AddBombPower();
                playerBombController.AddBombPower();
                playerBombController.AddBombPower();
                playerBombController.AddBombPower();
                playerBombController.AddBombPower();
                playerBombController.AddBombPower();
                playerBombController.AddBombPower();
                playerBombController.AddBombPower();
                playerBombController.AddBombPower();
                playerBombController.AddBombPower();
                playerBombController.AddBombPower();
            }
            
            Debug.Log("All bonus have been added to all players");
        } 
    }
}