using System;
using System.Collections.Generic;

namespace SmartBuss
{
    public class RobotService
    {
        private readonly Random random;
        private bool objectAlreadyDetected;

        public RobotState State { get; private set; }

        public event EventHandler<DetectedObjectEventArgs>
            ObjectDetected;

        public event EventHandler CleaningCompleted;

        public RobotService(int batteryLevel)
        {
            random = new Random();
            State = new RobotState
            {
                BatteryLevel = batteryLevel
            };
        }

        public void StartCleaning(
            string area,
            string method,
            string cleaningZones,
            int durationSeconds)
        {
            if (string.IsNullOrWhiteSpace(area))
            {
                area = "Όλο το λεωφορείο";
            }

            if (string.IsNullOrWhiteSpace(method))
            {
                method = "Κανονικός καθαρισμός";
            }

            if (string.IsNullOrWhiteSpace(cleaningZones))
            {
                cleaningZones = "Όλο το επιλεγμένο τμήμα";
            }

            objectAlreadyDetected = false;

            State.Status = RobotStatus.DeployingLegs;
            State.Area = area;
            State.Method = method;
            State.CleaningZones = cleaningZones;
            State.LegsExtended = true;
            State.Progress = 0;
            State.RemainingSeconds = durationSeconds;
        }

        public void PauseCleaning()
        {
            if (State.Status == RobotStatus.Idle ||
                State.Status == RobotStatus.Completed)
            {
                return;
            }

            State.Status = RobotStatus.Paused;
        }

        public void AdvanceOneSecond()
        {
            if (State.Status == RobotStatus.Idle ||
                State.Status == RobotStatus.Paused ||
                State.Status == RobotStatus.Completed)
            {
                return;
            }

            State.Progress = Math.Min(
                100,
                State.Progress + 5);

            State.RemainingSeconds = Math.Max(
                0,
                State.RemainingSeconds - 1);

            if (State.Progress <= 10)
            {
                State.Status = RobotStatus.DeployingLegs;
            }
            else if (State.Progress <= 30)
            {
                State.Status = RobotStatus.Moving;
            }
            else if (State.Progress <= 65)
            {
                State.Status = RobotStatus.Cleaning;
            }
            else if (State.Progress < 90)
            {
                State.Status = RobotStatus.DetectingObjects;

                DetectOneObjectIfNeeded();
            }
            else
            {
                State.Status = RobotStatus.Returning;
            }

            if (State.Progress >= 100)
            {
                State.Status = RobotStatus.Completed;
                State.LegsExtended = false;
                State.RemainingSeconds = 0;

                if (CleaningCompleted != null)
                {
                    CleaningCompleted(
                        this,
                        EventArgs.Empty);
                }
            }
        }

        private void DetectOneObjectIfNeeded()
        {
            if (objectAlreadyDetected)
            {
                return;
            }

            objectAlreadyDetected = true;

            DetectedObject detectedObject = new DetectedObject
            {
                ObjectName = "Διαβατήριο",
                Details = "Εντοπίστηκε κάτω από κάθισμα.",
                Area = State.Area,
                Time = DateTime.Now
            };

            if (ObjectDetected != null)
            {
                ObjectDetected(
                    this,
                    new DetectedObjectEventArgs(detectedObject));
            }
        }

        public string GetLocationDescription()
        {
            if (State.Status == RobotStatus.DeployingLegs)
            {
                return "Είσοδος λεωφορείου - ανάπτυξη ποδιών";
            }

            if (State.Status == RobotStatus.Moving)
            {
                if (State.Area == "Πάνω όροφος")
                {
                    return "Ανάβαση μέσω της σκάλας προς τον πάνω όροφο";
                }

                if (State.Area == "Κάτω όροφος")
                {
                    return "Κίνηση στον κάτω όροφο";
                }

                if (State.Area == "Σκάλα μεταξύ ορόφων")
                {
                    return "Σκάλα μεταξύ κάτω και πάνω ορόφου";
                }

                if (State.Area == "Περιοχή οδηγού")
                {
                    return "Περιοχή οδηγού";
                }

                return "Έλεγχος κάτω και πάνω ορόφου";
            }

            if (State.Status == RobotStatus.Cleaning)
            {
                return "Καθαρισμός: " + State.CleaningZones;
            }

            if (State.Status == RobotStatus.DetectingObjects)
            {
                return "Έλεγχος για αντικείμενα αξίας";
            }

            if (State.Status == RobotStatus.Returning ||
                State.Status == RobotStatus.Completed)
            {
                return "Σταθμός φόρτισης";
            }

            return "Σταθμός φόρτισης";
        }
    }

    public class DetectedObjectEventArgs : EventArgs
    {
        public DetectedObject DetectedObject { get; private set; }

        public DetectedObjectEventArgs(
            DetectedObject detectedObject)
        {
            DetectedObject = detectedObject;
        }
    }
}