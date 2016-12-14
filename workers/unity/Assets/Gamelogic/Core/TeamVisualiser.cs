using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [EngineType(EnginePlatform.Client)]
    public class TeamVisualiser : MonoBehaviour
    {
        [Require] private TeamAssignment.Reader teamAssignment;
        public uint TeamId { get { return teamAssignment.Data.teamId; } }

        public Renderer[] ModelRenderers;
        public Texture RedTexture;
        public Texture BlueTexture;

        private void OnEnable()
        {
            teamAssignment.ComponentUpdated += TeamAssigned;
            SetTeamColour(teamAssignment.Data.teamId);
        }

        private void OnDisable()
        {
            teamAssignment.ComponentUpdated -= TeamAssigned;
        }

        private void TeamAssigned(TeamAssignment.Update teamAssigned)
        {
            SetTeamColour(teamAssigned.teamId.Value);
        }

        private void SetTeamColour(uint teamIdValue)
        {
            for (int rendererNum = 0; rendererNum < ModelRenderers.Length; rendererNum++)
            {
                Material[] modelMaterials = ModelRenderers[rendererNum].materials;
                SetRendererColour(teamIdValue, modelMaterials);
            }
        }

        private void SetRendererColour(uint teamIdValue, Material[] modelMaterials)
        {
            switch (teamIdValue)
            {
                case SimulationSettings.RedTeamId:
                    SetMaterialTextures(modelMaterials, RedTexture);
                    break;
                case SimulationSettings.BlueTeamId:
                    SetMaterialTextures(modelMaterials, BlueTexture);
                    break;
            }
        }

        private void SetMaterialTextures(Material[] materials, Texture newTexture)
        {
            for (int materialNum = 0; materialNum < materials.Length; materialNum++)
            {
                materials[materialNum].mainTexture = newTexture;
            }
        }
    }
}
