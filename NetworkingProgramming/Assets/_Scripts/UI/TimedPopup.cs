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
    private Coroutine popup;

    public void Show(string header, string message)
    {
        if(popup != null) { StopCoroutine(popup); popup = null; }

        this.header.text = header;
        this.message.text = message;

        gameObject.SetActive(true);
        popup = StartCoroutine(Timer(duration));
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    IEnumerator Timer(float duration)
    {
        yield return new WaitForSeconds(duration);

        onElapsed?.Invoke();
        Close();
    }
}
