using System.Collections;
using UnityEngine;

public abstract class BaseUIController : MonoBehaviour
{
    private Canvas rootCanvas;
    private CanvasGroup rootGroup;

    private Coroutine fade;

    private void Awake()
    {
        rootCanvas = GetComponent<Canvas>();
        if(rootCanvas == null) { Debug.LogError($"No root canvas found on {gameObject.name}"); }
        rootGroup = GetComponent<CanvasGroup>();
        if(rootCanvas == null) { Debug.LogError($"No root canvas group found on {gameObject.name}"); }
    }

    public void HideCanvas() { rootCanvas.enabled = false; }
    public void HideCanvas(float duration = .1f)
    {
        rootGroup.interactable = false;
        rootGroup.blocksRaycasts = false;
        fade = Fade(rootGroup, 1f, 0f, duration);
    }

    public void ShowCanvas() { rootCanvas.enabled = true; }
    public void ShowCanvas(float duration = .1f)
    {
        rootGroup.interactable = true;
        rootGroup.blocksRaycasts = true;
        fade = Fade(rootGroup, 0f, 1f, duration);
    }

    private Coroutine Fade(CanvasGroup group, float start, float end, float duration, bool killExisting = true)
    {
        if(fade != null) 
        {
            if (!killExisting) { return null; }
            StopCoroutine(fade);
            fade = null;
        }
        return StartCoroutine(FadeCanvas(group, start, end, duration));
    }
    IEnumerator FadeCanvas(CanvasGroup group, float start, float end, float duration)
    {
        float elapsed = 0f;

        group.alpha = start;

        while(elapsed < duration)
        {
            group.alpha = Mathf.Lerp(start, end, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        group.alpha = end;
    }
}
