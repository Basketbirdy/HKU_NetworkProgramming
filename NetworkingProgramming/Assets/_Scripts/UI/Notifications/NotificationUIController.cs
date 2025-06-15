using System.Collections.Generic;
using UnityEngine;

public class NotificationUIController : BaseUIController
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform container;
    [Space]
    [SerializeField] private int maxQueueSize = 10;
    private Queue<INotification> notificationQueue = new Queue<INotification>();

    private void Start()
    {
        UIManager.Instance.AddReference<BaseUIController>(GetType().ToString(), this);
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
    }
}

public interface INotification
{
    public void Init(string source, string message);
    public void Destroy();
}
