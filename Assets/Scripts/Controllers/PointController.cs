using UnityEngine;

namespace Controllers
{
    public class PointController : MonoBehaviour
    {
        [Header("Debug")] public bool isOccupied;
        [SerializeField] private MobController currentMob;

        public void SetOccupied(MobController newMob)
        {
            isOccupied = true;
            currentMob = newMob;
        }

        public void SetFree()
        {
            isOccupied = false;
        }
    }
}