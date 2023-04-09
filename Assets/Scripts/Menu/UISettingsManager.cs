using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class UISettingsManager : MonoBehaviour
    {
        [SerializeField] private GameObject previousMenuCanvas;
        [SerializeField] private GameObject settingsMenuCanvas;
        [SerializeField] private SettingEntry[] settingEntries;
        [SerializeField] private Selectable firstSelectableInSettingsMenu;
        [SerializeField] private Selectable settingsButtonInPreviousMenu;
        
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
            firstSelectableInSettingsMenu.Select();
        }
        
        public void ReturnToPreviousMenu()
        {
            previousMenuCanvas.SetActive(true);
            settingsMenuCanvas.SetActive(false);
            settingsButtonInPreviousMenu.Select();
        }
    }
}
