using Improbable.Fire;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Fire
{
    [EngineType(EnginePlatform.FSim)]
    public class FlammableDataVisualizer : MonoBehaviour
    {
        [Require] public Flammable.Reader flammable;
    }
}
