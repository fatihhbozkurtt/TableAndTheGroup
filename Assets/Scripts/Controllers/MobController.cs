using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class MobController : MonoBehaviour
    {
        [Header("Config")] [SerializeField] private Vector2 speedRange;

        [Header("Debug")] [SerializeField] private PointController currentPoint;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private bool isActive;
        private bool _hasReachedDestination;
        private bool _canStartCounter;
        private TableManager _tm;
        private const float Radius = 10f;
        private const float MinDistance = 1.5f; //  Minimum distance between mobs
        private readonly List<Vector3> _previousPoints = new();
        private CounterPanel _counterPanel;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = Random.Range(speedRange.x, speedRange.y);
        }

        private void Start()
        {
            _counterPanel = GetComponentInChildren<CounterPanel>();
            _tm = TableManager.instance;
            _tm.OneMobLeftEvent += OnOneMobLeft;
        }

        private void Update()
        {
            if (!_canStartCounter) return;
            if (_hasReachedDestination) return; 
            if (agent.pathPending || !(agent.remainingDistance <= 0.1f) || agent.hasPath) return;
            
            _hasReachedDestination = true;
            _counterPanel.StartCounter();
        }

        private void OnOneMobLeft()
        {
            if (currentPoint == null) return;
            int currIndex = _tm.GetIndexByPoint(currentPoint);
            if (IsLastMobInLine())
            {
                currentPoint.SetFree();
            }

            Move(_tm.GetPointByIndex(Mathf.Max(currIndex - 1, 0)));
        }

        public void Move(PointController targetPoint = null)
        {
            SetNewDestinationAndPoint(targetPoint == null
                ? _tm.GetAvailablePoint()
                : targetPoint);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void SetNewDestinationAndPoint(PointController newPoint)
        {
            currentPoint = newPoint;
            currentPoint.SetOccupied(this);
            var pointPos = currentPoint.transform.position;
            agent.SetDestination(new Vector3(pointPos.x, 1.55f, pointPos.z));

            if (IsFirstMobInLine())
                _canStartCounter = true;
        }

        public void OnCounterFinished()
        {
            _tm.OneMobLeftEvent -= OnOneMobLeft;
            _tm.TriggerMobLeftEvent();
            MoveToRandomPoint();
        }

        #region Booleans

        bool IsFirstMobInLine()
        {
            return _tm.GetIndexByPoint(currentPoint) == 0;
        }

        bool IsLastMobInLine()
        {
            return GroupManager.instance.GetLastMovedMob() == this;
        }

        #endregion

        private void MoveToRandomPoint()
        {
            Vector3 myPos = transform.position;
 
            Vector3 centerPosition = myPos + new Vector3(0f, 0f, 15f);

            bool validPointFound = false;
            int maxAttempts = 10;  
            int attempt = 0;

            while (!validPointFound && attempt < maxAttempts)
            {
                attempt++;
                Vector2 randomPoint2D = Random.insideUnitCircle * Radius;
                var randomPoint = new Vector3(randomPoint2D.x, centerPosition.y, randomPoint2D.y) + centerPosition;

                bool isFarEnough = true;
                foreach (var point in _previousPoints)
                {
                    if (Vector3.Distance(randomPoint, point) < MinDistance)
                    {
                        isFarEnough = false;
                        break;
                    }
                }

                // Mesafe uygunsa ve NavMesh üzerinde geçerli bir pozisyon bulursa
                if (!isFarEnough) continue;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, Radius, NavMesh.AllAreas))
                {
                    validPointFound = true;
                    agent.SetDestination(hit.position);
                }
            }
        }
    }
}