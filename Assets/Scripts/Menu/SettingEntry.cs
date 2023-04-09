using UnityEngine;

namespace Menu
{
    /// <summary>
    /// SettingEntry is used to easily save the settings.
    /// </summary>
    public abstract class SettingEntry : MonoBehaviour
    {
        public abstract void Save();
    }
}