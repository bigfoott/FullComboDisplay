using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using IllusionPlugin;
using FullComboDisplay;

namespace FulLComboDisplay
{
    public class Plugin : IPlugin
    {

        public string Name => "Full Combo Display";
        public string Version => "1.1.0";

        private readonly string[] env = { "DefaultEnvironment", "BigMirrorEnvironment", "TriangleEnvironment", "NiceEnvironment" };

        private void OnSceneChange(Scene arg0, Scene arg1)
        {
            if (!env.Contains(arg1.name)) return;
            new GameObject("FCDisplay").AddComponent<FCDisplay>();
        }
        
        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += OnSceneChange;

            // set defaults if they dont exist
            ModPrefs.GetInt("FCDisplay", "ColorRed", 255, true);
            ModPrefs.GetInt("FCDisplay", "ColorGreen", 200, true);
            ModPrefs.GetInt("FCDisplay", "ColorBlue", 0, true);
            ModPrefs.GetString("FCDisplay", "MissEffect", "Fade", true);
        }
        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnSceneChange;
        }
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) { }
        public void OnLevelWasLoaded(int level) { }
        public void OnLevelWasInitialized(int level) { }
        public void OnUpdate() { }
        public void OnFixedUpdate() { }
    }
}