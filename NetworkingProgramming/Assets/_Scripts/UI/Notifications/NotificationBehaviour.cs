using System;
using TMPro;
using UnityEngine;

public class NotificationBehaviour : MonoBehaviour, INotification
{
    private string source;
    private string message;

    [SerializeField] private TextMeshProUGUI notificationText;

    public void Init(string source, string message)
    {
        this.source = source;
        this.message = message;
        SetText();
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    private void SetText()
    {
        notificationText.text = $"[{DateTime.Now.ToString("HH:mm:ss")}] [{source}] {message}";
    }
}
