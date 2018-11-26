using IllusionPlugin;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using FullComboDisplay.Util;


namespace FullComboDisplay
{
    class FCDisplay : MonoBehaviour
    {
        private Image img;
        private GameObject g;
        private GameObject g2;

        private ScoreMultiplierUIController multi;
        private ScoreController score;
        private PlayerHeadAndObstacleInteraction obstacles;

        private void Awake()
        {
            StartCoroutine(WaitForLoad());
        }

        private IEnumerator WaitForLoad()
        {
            bool loaded = false;
            while (!loaded)
            {
                multi = Resources.FindObjectsOfTypeAll<ScoreMultiplierUIController>().FirstOrDefault();
                score = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
                obstacles = Resources.FindObjectsOfTypeAll<PlayerHeadAndObstacleInteraction>().FirstOrDefault();

                if (multi == null || score == null || obstacles == null)
                    yield return new WaitForSeconds(0.01f);
                else
                    loaded = true;
            }
            Init();
        }

        private void Init()
        {
            score.noteWasCutEvent += OnNoteCut;
            score.noteWasMissedEvent += OnNoteMiss;

            g = new GameObject("FCRing");
            Canvas canvas = g.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            CanvasScaler cs = g.AddComponent<CanvasScaler>();
            cs.scaleFactor = 10.0f;
            cs.dynamicPixelsPerUnit = 10f;
            GraphicRaycaster gr = g.AddComponent<GraphicRaycaster>();
            g.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1f);
            g.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);
            
            g2 = new GameObject();
            img = g2.AddComponent<Image>();
            g2.transform.parent = g.transform;
            g2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0.5f);
            g2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.5f);
            g2.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

            var circleimage = ReflectionUtil.GetPrivateField<Image>(multi, "_multiplierProgressImage");

            img.sprite = circleimage.sprite;
            img.color = new Color(
                ModPrefs.GetInt("FCDisplay", "ColorRed", 255, true),
                ModPrefs.GetInt("FCDisplay", "ColorGreen", 255, true),
                ModPrefs.GetInt("FCDisplay", "ColorBlue", 255, true));
            
            g.transform.position = new Vector3(3.025f, 1.2f, 7.08f);
        }

        private void Update()
        {
            if (obstacles != null && obstacles.intersectingObstacles.Count > 0 && !startedDelete)
                StartCoroutine(Missed());
        }
        private void OnNoteMiss(NoteData data, int c)
        {
            if (data.noteType != NoteType.Bomb)
                StartCoroutine(Missed());
        }
        private void OnNoteCut(NoteData data, NoteCutInfo nci, int c)
        {
            if (data.noteType == NoteType.Bomb || !nci.allIsOK)
                StartCoroutine(Missed());
        }
        
        private bool startedDelete = false;
        private IEnumerator Missed()
        {
            if (!startedDelete)
            {
                startedDelete = true;

                string mode = Plugin.effects[ModPrefs.GetInt("FCDisplay", "MissEffect", 1)];

                if (mode == "FlyOut")
                {
                    img.CrossFadeAlpha(0, 0.5f, false);
                    for (int i = 0; i < 20; i++)
                    {
                        g.transform.Translate(new Vector3(0, 0, 0.5f));
                        yield return new WaitForSeconds(0.025f);
                    }
                }
                else if (mode == "Fade")
                {
                    img.CrossFadeAlpha(0, 0.5f, false);
                    yield return new WaitForSeconds(0.5f);
                }
                else if (mode == "Shrink")
                {
                    img.CrossFadeAlpha(0, 0.4f, false);
                    for (int i = 0; i < 20; i++)
                    {
                        g2.transform.localScale = new Vector3(
                            g2.transform.localScale.x - 0.125f, 
                            g2.transform.localScale.y - 0.125f, 
                            g2.transform.localScale.z - 0.125f);
                        yield return new WaitForSeconds(0.02f);
                    }
                }
                else if (mode == "Flicker")
                {
                    img.CrossFadeAlpha(0, 0.01f, false);
                    yield return new WaitForSeconds(0.1f);
                    img.CrossFadeAlpha(1, 0.01f, false);
                    yield return new WaitForSeconds(0.08f);
                    img.CrossFadeAlpha(0, 0.01f, false);
                    yield return new WaitForSeconds(0.05f);
                    img.CrossFadeAlpha(1, 0.01f, false);
                    yield return new WaitForSeconds(0.05f);
                    img.CrossFadeAlpha(0, 0.01f, false);
                    yield return new WaitForSeconds(0.05f);
                    img.CrossFadeAlpha(1, 0.01f, false);
                    yield return new WaitForSeconds(0.03f);
                    img.CrossFadeAlpha(0, 0.01f, false);
                    yield return new WaitForSeconds(0.02f);
                    img.CrossFadeAlpha(1, 0.01f, false);
                    yield return new WaitForSeconds(0.02f);
                    img.CrossFadeAlpha(0, 0.01f, false);
                }

                score.noteWasCutEvent -= OnNoteCut;
                score.noteWasMissedEvent -= OnNoteMiss;
                
                Destroy(g);
                Destroy(this);
            }
        }
    }
}
