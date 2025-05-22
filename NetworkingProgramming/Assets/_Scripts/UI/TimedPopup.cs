using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimedPopup : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private UnityEvent onElapsed;
    [Space]
    [SerializeField] private TextMeshProUGUI header;
    [SerializeField] private TextMeshProUGUI message;

    public void Show(string header, string message)
    {
        this.header.text = header;
        this.message.text = message;
        StartCoroutine(Timer(duration));
    }

    IEnumerator Timer(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration) 
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        onElapsed?.Invoke();
        gameObject.SetActive(false);
    }
}
