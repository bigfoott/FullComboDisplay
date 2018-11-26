using UnityEngine;
using UnityEngine.SceneManagement;
using IllusionPlugin;
using FullComboDisplay;
using CustomUI.Settings;

namespace FullComboDisplay
{
    public class Plugin : IPlugin
    {

        public string Name => "Full Combo Display";
        public string Version => "2.0.0";

        public static string[] effects = { "Fade", "FlyOut", "Flicker", "Shrink" };

        private void OnSceneChange(Scene arg0, Scene arg1)
        {
            if (arg1.name != "GameCore") return;
            new GameObject("FCDisplay").AddComponent<FCDisplay>();
        }
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name != "Menu") return;

            var menu = SettingsUI.CreateSubMenu("FullComboDisplay");
            
            float[] effectVals = new float[effects.Length];
            for (int i = 0; i < effects.Length; i++) effectVals[i] = i;

            var effect = menu.AddList("Miss Effect", effectVals);
            effect.GetValue += delegate { return ModPrefs.GetInt("FCDisplay", "MissEffect", 1, true); };
            effect.SetValue += delegate (float value) { ModPrefs.SetInt("FCDisplay", "MissEffect", (int)value); };
            effect.FormatValue += delegate (float value) { return effects[(int)value]; };
        }

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += OnSceneChange;
            SceneManager.sceneLoaded += OnSceneLoaded;

            // set defaults if they dont exist
            ModPrefs.GetInt("FCDisplay", "ColorRed", 255, true);
            ModPrefs.GetInt("FCDisplay", "ColorGreen", 255, true);
            ModPrefs.GetInt("FCDisplay", "ColorBlue", 255, true);
            ModPrefs.GetInt("FCDisplay", "MissEffect", 0, true);
        }
        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnSceneChange;
        }
        public void OnLevelWasLoaded(int level) { }
        public void OnLevelWasInitialized(int level) { }
        public void OnUpdate() { }
        public void OnFixedUpdate() { }
    }
}