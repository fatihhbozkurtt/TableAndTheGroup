using System.Collections.Generic;
using Controllers;
using UnityEngine;
using System;

namespace Managers
{
    public class TableManager : MonoSingleton<TableManager>
    {
        public event Action OneMobLeftEvent;

        [Header("References")] [SerializeField]
        private List<PointController> points;

        public PointController GetAvailablePoint()
        {
            PointController availablePoint = null;
            foreach (var point in points)
            {
                if (point.isOccupied) continue;
                availablePoint = point;
                break;
            }

            return availablePoint;
        }

        public int GetIndexByPoint(PointController p)
        {
            return points.IndexOf(p);
        }

        public PointController GetPointByIndex(int index)
        { 
            return points[index];
        }

        public void TriggerMobLeftEvent()
        {
            OneMobLeftEvent?.Invoke();
        }
    }
}