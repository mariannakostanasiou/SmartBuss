using System;
using System.Collections.Generic;

namespace SmartBuss
{
    public class CartItem
    {
        public string Name { get; set; }
        public string Store { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal Subtotal
        {
            get
            {
                return Price * Quantity;
            }
        }
    }

    public class Order
    {
        public int Number { get; set; }
        public string Store { get; set; }
        public List<string> Items { get; set; }
        public string DeliveryStop { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Total { get; set; }

        public Order()
        {
            Items = new List<string>();
        }
    }

    public class DetectedObject
    {
        public string ObjectName { get; set; }
        public string Details { get; set; }
        public string Area { get; set; }
        public DateTime Time { get; set; }
    }

    public class NotificationItem
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationPriority Priority { get; set; }
        public DateTime Time { get; set; }
    }

    public enum NotificationPriority
    {
        Normal,
        High
    }

    public enum RobotStatus
    {
        Idle,
        DeployingLegs,
        Moving,
        Cleaning,
        DetectingObjects,
        Returning,
        Paused,
        Completed
    }

    public class RobotState
    {
        public RobotStatus Status { get; set; }
        public string Area { get; set; }
        public string Method { get; set; }
        public string CleaningZones { get; set; }
        public bool LegsExtended { get; set; }
        public int Progress { get; set; }
        public int RemainingSeconds { get; set; }
        public int BatteryLevel { get; set; }

        public RobotState()
        {
            Status = RobotStatus.Idle;
            Area = "Σταθμός φόρτισης";
            Method = "Κανονικός καθαρισμός";
            CleaningZones = "Δεν έχουν επιλεγεί σημεία";
            LegsExtended = false;
            Progress = 0;
            RemainingSeconds = 0;
            BatteryLevel = 68;
        }
    }
}