using Improbable.Fire;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Fire
{
    [EngineType(EnginePlatform.FSim)]
    class FlammableBehaviourOptimize : MonoBehaviour
    {
        [Require] private FlammableWriter flammable;

        public void IgniteUpdate()
        {
            if (!flammable.IsOnFire)
            {
                flammable.Update.IsOnFire(true).CanBeIgnited(false).FinishAndSend();
            }
        }

        public void ExtinguishUpdate(bool canBeIgnited)
        {
            if (flammable.IsOnFire)
            {
                flammable.Update.IsOnFire(false).CanBeIgnited(canBeIgnited).FinishAndSend();
            }
        }
    }
}
