using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    /// <summary>
    /// UISettingsManager is used to manage the settings menu.
    /// </summary>
    public class UISettingsManager : MonoBehaviour
    {
        [SerializeField] private GameObject previousMenuCanvas;
        [SerializeField] private GameObject settingsMenuCanvas;
        [SerializeField] private SettingEntry[] settingEntries;
        [SerializeField] private Selectable firstSelectableInSettingsMenu;
        [SerializeField] private Selectable settingsButtonInPreviousMenu;
        
        // Save the settings and return to the previous menu.
        public void SaveSettings()
        {
            foreach (SettingEntry settingEntry in settingEntries)
            {
                settingEntry.Save();
            }
            
            ReturnToPreviousMenu();
        }
        
        // Open the settings menu.
        public void OpenSettings()
        {
            previousMenuCanvas.SetActive(false);
            settingsMenuCanvas.SetActive(true);
            firstSelectableInSettingsMenu.Select();
        }
        
        // Return to the previous menu.
        public void ReturnToPreviousMenu()
        {
            previousMenuCanvas.SetActive(true);
            settingsMenuCanvas.SetActive(false);
            settingsButtonInPreviousMenu.Select();
        }
    }
}
