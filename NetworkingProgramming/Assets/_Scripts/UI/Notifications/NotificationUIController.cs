using System.Collections.Generic;
using UnityEngine;

public class NotificationUIController : BaseUIController
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform container;
    [Space]
    [SerializeField] private int maxQueueSize = 10;
    private Queue<INotification> notificationQueue = new Queue<INotification>();

    private float timestamp = Mathf.Infinity;
    [SerializeField] private float showTime = 5f;

    private void Start()
    {
        UIManager.Instance.AddReference<BaseUIController>(GetType().ToString(), this);
        HideCanvas(.7f);
    }

    private void Update()
    {
        if(isVisible && Time.time > timestamp + showTime)
        {
            HideCanvas();
        }
    }

    public void SendNotification(string source, string message)
    {
        // This could probably be pooled

        // check if queue is full, if so remove oldest message
        if(notificationQueue.Count >= maxQueueSize)
        {
            INotification oldestNotification = notificationQueue.Dequeue();
            oldestNotification.Destroy();
        }

        // Create new notification
        INotification newNotification = Instantiate(prefab, container).GetComponent<INotification>();
        notificationQueue.Enqueue(newNotification);
        newNotification.Init(source, message);

        ShowCanvas(.5f);
        timestamp = Time.time;
    }
}

public interface INotification
{
    public void Init(string source, string message);
    public void Destroy();
}
