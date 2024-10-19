using System.Collections.Generic;
using Controllers;
using UnityEngine;

namespace Managers
{
    public class GroupManager : MonoSingleton<GroupManager>
    {
        [Header("References")] [SerializeField]
        private List<MobController> mobs;

        [Header("Debug")] [SerializeField] private List<MobController> linedMobs;

        private void Update()
        {
            if (mobs.Count == 0) return;
            if (!Input.GetKeyDown(KeyCode.Space)) return; 

            int randomIndex = Random.Range(0, mobs.Count - 1);

            MobController moverMob = mobs[randomIndex];
            moverMob.Move();
            mobs.Remove(moverMob);
            linedMobs.Add(moverMob);
        }

        public MobController GetLastMovedMob()
        {
            return linedMobs[^1];
        }
    }
}