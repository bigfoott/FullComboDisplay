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
        public string Version => "2.1.0";

        public static string[] effects = { "Fade", "FlyOut", "Flicker", "Shrink" };

        private void OnSceneChange(Scene arg0, Scene arg1)
        {
            if (arg1.name != "GameCore" || !ModPrefs.GetBool("FCDisplay", "Enabled", true, true)) return;
            new GameObject("FCDisplay").AddComponent<FCDisplay>();
        }
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name != "MenuCore") return;

            var menu = SettingsUI.CreateSubMenu("FullComboDisplay");
            
            float[] effectVals = new float[effects.Length];
            for (int i = 0; i < effects.Length; i++) effectVals[i] = i;

            var enabled = menu.AddBool("Enabled");
            enabled.GetValue += delegate { return ModPrefs.GetBool("FCDisplay", "Enabled", true , true); };
            enabled.SetValue += delegate (bool value) { ModPrefs.SetBool("FCDisplay", "Enabled", value); };
            enabled.EnabledText = "Enabled";
            enabled.DisabledText = "Disabled";

            var effect = menu.AddList("Miss Effect", effectVals);
            effect.GetValue += delegate { return ModPrefs.GetInt("FCDisplay", "MissEffect", 1, true); };
            effect.SetValue += delegate (float value) { ModPrefs.SetInt("FCDisplay", "MissEffect", (int)value); };
            effect.FormatValue += delegate (float value) { return effects[(int)value]; };

            var vanilla = menu.AddBool("Vanilla Display (Combo Border)");
            vanilla.GetValue += delegate { return ModPrefs.GetBool("FCDisplay", "VanillaEnabled", false, true); };
            vanilla.SetValue += delegate (bool value) { ModPrefs.SetBool("FCDisplay", "VanillaEnabled", value); };
            vanilla.EnabledText = "Visible";
            vanilla.DisabledText = "Hidden";
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