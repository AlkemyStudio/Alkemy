using UnityEngine;

namespace Menu
{
    public class UISettingsManager : MonoBehaviour
    {
        [SerializeField] private GameObject previousMenuCanvas;
        [SerializeField] private GameObject settingsMenuCanvas;
        [SerializeField] private SettingEntry[] settingEntries;
        
        public void SaveSettings()
        {
            foreach (SettingEntry settingEntry in settingEntries)
            {
                settingEntry.Save();
            }
            
            ReturnToPreviousMenu();
        }
        
        public void OpenSettings()
        {
            previousMenuCanvas.SetActive(false);
            settingsMenuCanvas.SetActive(true);
        }
        
        public void ReturnToPreviousMenu()
        {
            previousMenuCanvas.SetActive(true);
            settingsMenuCanvas.SetActive(false);
        }
    }
}
