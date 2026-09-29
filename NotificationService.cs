using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartBuss
{
    public class NotificationService
    {
        private readonly List<NotificationItem> notifications;

        public NotificationService()
        {
            notifications = new List<NotificationItem>();
        }

        public IReadOnlyList<NotificationItem> GetAll()
        {
            return notifications
                .OrderByDescending(item => item.Time)
                .ToList()
                .AsReadOnly();
        }

        public void Add(
            string title,
            string message,
            NotificationPriority priority)
        {
            notifications.Add(new NotificationItem
            {
                Title = title,
                Message = message,
                Priority = priority,
                Time = DateTime.Now
            });
        }

        public void AddObjectNotification(
            DetectedObject detectedObject)
        {
            Add(
                "Εντοπίστηκε αντικείμενο: " +
                detectedObject.ObjectName,
                detectedObject.Details +
                " Σημείο: " +
                detectedObject.Area +
                " | Ενημερώθηκαν οδηγός, εταιρεία και επιβάτες.",
                NotificationPriority.High);
        }

        public void Clear()
        {
            notifications.Clear();
        }

        public int Count
        {
            get { return notifications.Count; }
        }
    }
}