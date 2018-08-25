using IllusionPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace FullComboDisplay
{
    class FCDisplay : MonoBehaviour
    {
        Image img;
        GameObject g;

        private void Awake()
        {
            StartCoroutine(WaitForLoad());
        }

        IEnumerator WaitForLoad()
        {
            bool loaded = false;
            while (!loaded)
            {
                var mutliplier = Resources.FindObjectsOfTypeAll<ScoreMultiplierUIController>().FirstOrDefault();
                var score = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();

                if (mutliplier == null || score == null)
                    yield return new WaitForSeconds(0.01f);
                else
                    loaded = true;
            }
            Init();
        }

        void Init()
        {
            var score = Resources.FindObjectsOfTypeAll<ScoreController>().First();
            score.noteWasCutEvent += OnNoteCut;
            score.noteWasMissedEvent += OnNoteMiss;

            g = new GameObject();
            Canvas canvas = g.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            CanvasScaler cs = g.AddComponent<CanvasScaler>();
            cs.scaleFactor = 10.0f;
            cs.dynamicPixelsPerUnit = 10f;
            GraphicRaycaster gr = g.AddComponent<GraphicRaycaster>();
            g.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1f);
            g.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);
            
            GameObject g2 = new GameObject();
            img = g2.AddComponent<Image>();
            g2.transform.parent = g.transform;
            g2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0.5f);
            g2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.5f);
            g2.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

            var circleimage = ReflectionUtil.GetPrivateField<Image>(Resources.FindObjectsOfTypeAll<ScoreMultiplierUIController>().First(), "_multiplierProgressImage");
            
            img.sprite = circleimage.sprite;
            img.color = new Color(
                ModPrefs.GetInt("FCDisplay", "ColorRed", 255, true),
                ModPrefs.GetInt("FCDisplay", "ColorGreen", 255, true),
                ModPrefs.GetInt("FCDisplay", "ColorBlue", 0, true));
            img.material.shader = Shader.Find("Custom/SpriteNoGlow");
            img.CrossFadeAlpha(0.05f, 0, false);

            
            g.transform.position = new Vector3(-3.25f, 1.2f, 7f);
        }

        private void OnNoteMiss(NoteData data, int c)
        {
            if (data.noteType != NoteType.Bomb)
                StartCoroutine(FlyAway());
        }
        private void OnNoteCut(NoteData data, NoteCutInfo nci, int c)
        {
            if (data.noteType == NoteType.Bomb || !nci.allIsOK)
                StartCoroutine(FlyAway());
        }

        bool startedFly = false;
        IEnumerator FlyAway()
        {
            if (!startedFly)
            {
                startedFly = true;
                img.CrossFadeAlpha(0, 0.4f, false);
                for (int i = 0; i < 20; i++)
                {
                    g.transform.Translate(new Vector3(0, 0, 0.5f));
                    yield return new WaitForSeconds(0.02f);
                }
                Destroy(g);
                Destroy(this);
            }
        }
    }
}
