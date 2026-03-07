using HeadshotDarkness.Enums;
using HeadshotDarkness.Helpers;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace HeadshotDarkness.Components
{
    public class DeathTextManager : MonoBehaviour
    {
        public static DeathTextManager Instance { get; private set; }

        public Canvas Canvas;
        public CanvasScaler CanvasScaler;
        public TextMeshProUGUI DeathText;

        public Color TextColor;
        public Color TextColor2;

        public static DeathTextManager Create()
        {
            GameObject gameObject = new GameObject("DeathTextManager");
            DeathTextManager deathTextManager = gameObject.AddComponent<DeathTextManager>();
            return deathTextManager;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Canvas = gameObject.AddComponent<Canvas>();
            Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            Canvas.sortingOrder = 999;

            CanvasScaler = gameObject.AddComponent<CanvasScaler>();

            DeathText = gameObject.AddComponent<TextMeshProUGUI>();
            DeathText.color = new Color(1, 1, 1, 0);
            DeathText.font = FontHelper.FindFont(EDeathStringFont.Arial);
            DeathText.fontSize = Plugin.DeathTextFontSize.Value;
            DeathText.alignment = TextAlignmentOptions.Center;
            
            UpdateFromConfig();
        }

        public static void UpdateFromConfig()
        {
            if (Instance == null) return;

            DeathTextManager manager = Instance;
            
            Color color = Plugin.DeathTextFontColor.Value;

            manager.TextColor = color;
            manager.TextColor2 = new Color(color.r, color.g, color.b, 0);

            manager.DeathText.color = manager.TextColor2;
            manager.DeathText.font = FontHelper.FindFont(Plugin.DeathTextFont.Value);
            manager.DeathText.text = Plugin.DeathTextString.Value;
            manager.DeathText.fontSize = Plugin.DeathTextFontSize.Value;
        }

        private IEnumerator FadeText(float targetAlpha, float duration)
        {
            Color curColor = DeathText.color;
            float startAlpha = curColor.a;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float newAlpha = startAlpha + (targetAlpha - startAlpha) * (elapsedTime / duration);
                DeathText.color = new Color(curColor.r, curColor.g, curColor.b, newAlpha);
                yield return null;
            }

            DeathText.color = new Color(curColor.r, curColor.g, curColor.b, targetAlpha);
        }

        private IEnumerator TextSequence(string text, int size, float time, float fadeInTime, float fadeOutTime, float fadeDelay)
        {
            DeathText.text = text;
            DeathText.fontSize = size;

            PluginDebug.LogInfo("text fade delay");
            yield return new WaitForSeconds(fadeDelay);

            PluginDebug.LogInfo("text fade in");
            yield return StartCoroutine(FadeText(1f, fadeInTime));

            PluginDebug.LogInfo("text fade hold");
            yield return new WaitForSeconds(time);

            PluginDebug.LogInfo("text fade out");
            yield return StartCoroutine(FadeText(0f, fadeOutTime));
        }

        public void DoDeathText(string text, int size, float time, float fadeInTime, float fadeOutTime, float fadeDelay)
        {
            StartCoroutine(TextSequence(text, size, time, fadeInTime, fadeOutTime, fadeDelay));
        }
    }
}
