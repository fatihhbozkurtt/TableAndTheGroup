using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [Header("References")] [SerializeField]
        private List<MobController> mobs;

        [Header("Debug")] [SerializeField] private List<MobController> linedMobs;

        private void Update()
        {
            if (mobs.Count == 0) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // activate random mob to move

                int randomIndex = Random.Range(0, mobs.Count - 1);

                MobController moverMob = mobs[randomIndex];
                moverMob.Move();
                mobs.Remove(moverMob);
                linedMobs.Add(moverMob);
            }
        }

        public MobController GetLastMovedMob()
        {
            return linedMobs[^1];
        }
    }
}