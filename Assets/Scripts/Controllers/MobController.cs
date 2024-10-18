using System.Collections;
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
        private bool _canStart;
        private TableManager _tm;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = Random.Range(speedRange.x, speedRange.y);
        }

        private void Start()
        {
            _tm = TableManager.instance;
            _tm.OneMobLeftEvent += OnOneMobLeft;
        }

        private void Update()
        {
            if (!_canStart) return;
            // Check if the agent has reached its destination
            if (!agent.pathPending && agent.remainingDistance <= 0.1f && !agent.hasPath)
            {
                if (_hasReachedDestination) return; // Check if the destination is reached for the first time
                _hasReachedDestination = true;
                Debug.LogWarning("Routine invoked");
                StartCoroutine(WaitingRoutine());
            }
            else
            {
                _hasReachedDestination = false; // Reset the flag when moving to a new point
            }
        }

        public void Move()
        {
            SetNewDestinationAndPoint(_tm.GetAvailablePoint());
            //  _canStart = true;
            StartCoroutine(WaitingRoutine());
        }

        private void SetNewDestinationAndPoint(PointController newPoint)
        {
            currentPoint = newPoint;
            currentPoint.SetOccupied(this);
            var pointPos = currentPoint.transform.position;
            agent.SetDestination(new Vector3(pointPos.x, 1.55f, pointPos.z));
        }

        private IEnumerator WaitingRoutine()
        {
            if (!IsFirstMobInLine()) yield break;

            yield return new WaitForSeconds(5);

            _tm.OneMobLeftEvent -= OnOneMobLeft;
            agent.SetDestination(new Vector3(-5, 1.55f, transform.position.z));
            _tm.TriggerMobLeftEvent();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void OnOneMobLeft()
        {
            if (currentPoint == null) return;
            int currIndex = _tm.GetIndexByPoint(currentPoint);
            if (IsLastMobInLine())
            {
                currentPoint.SetFree();
            }

            SetNewDestinationAndPoint(_tm.GetPointByIndex(Mathf.Max(currIndex - 1, 0)));
            //  _canStart = true;
            StartCoroutine(WaitingRoutine());
        }

        bool IsFirstMobInLine()
        {
            return _tm.GetIndexByPoint(currentPoint) == 0;
        }

        bool IsLastMobInLine()
        {
            return GameManager.instance.GetLastMovedMob() == this;
        }
    }
}