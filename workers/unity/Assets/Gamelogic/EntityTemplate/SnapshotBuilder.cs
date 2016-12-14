using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Math;
using Improbable.Worker;
using System.Collections.Generic;
using Improbable.Global;
using UnityEngine;
using Random = UnityEngine.Random;
using System.IO;

namespace Assets.Gamelogic.EntityTemplate
{
    public class SnapshotBuilder
    {
        private static readonly string PerlinNoiseTexture = SimulationSettings.PerlinNoiseTexture;
        private string SnapshotPath;

        private int currentEntityId;
        private System.Random rand = new System.Random();
        private IDictionary<EntityId, SnapshotEntity> snapshotEntities = new Dictionary<EntityId, SnapshotEntity>();
        private IList<Team> teamList = new List<Team>(SimulationSettings.TeamCount);

        public void CreateSnapshot()
        {
            SnapshotPath = Application.dataPath + "/../../../snapshots/" + SimulationSettings.SnapshotFileName;

            SetupTeamContainers();
            AddHQs();
            AddNPCsAroundHQs();
            AddTrees();
            AddSimulationManagerEntity();
            SaveSnapshot();
        }

        private void SaveSnapshot()
        {
            File.Delete(SnapshotPath);
            var result = Snapshot.Save(SnapshotPath, snapshotEntities);

            if (result.HasValue)
            {
                Debug.LogErrorFormat("Failed to generate initial world snapshot: {0}", result.Value);
            }
            else
            {
                Debug.LogFormat("Successfully generated initial world snapshot at {0} with {1} entities", SnapshotPath, currentEntityId);
            }
        }

        private void AddSimulationManagerEntity()
        {
            snapshotEntities.Add(GenerateId(), EntityTemplateFactory.CreateSimulationManagerEntitySnapshotTemplate(teamList));
        }

        private void AddTrees()
        {
            var treeCountSqrt = (int)Mathf.Ceil(Mathf.Sqrt(SimulationSettings.AttemptedTreeCount));
            var spawnGridIntervals = SimulationSettings.SpawningWorldEdgeLength / treeCountSqrt;
            var perlin = Resources.Load(PerlinNoiseTexture) as Texture2D;
            var placedTreeCount = 0;

            for (var x = 0; x < treeCountSqrt; x++)
            {
                var xProportion = (float)x / (float)treeCountSqrt;
                for (var z = 0; z < treeCountSqrt; z++)
                {
                    var zProportion = (float)z / (float)treeCountSqrt;
                    var perlinSample = perlin.GetPixel((int)(xProportion * perlin.width), (int)(zProportion * perlin.height)).maxColorComponent;

                    if (perlinSample > 0.35f && Random.value < perlinSample)
                    {
                        Vector3d positionJitter = new Vector3d(Random.Range(-SimulationSettings.TreeJitter, SimulationSettings.TreeJitter), 0d, Random.Range(-SimulationSettings.TreeJitter, SimulationSettings.TreeJitter));
                        Vector3d offsetFromWorldRoot = new Vector3d(x * spawnGridIntervals, 0d, z * spawnGridIntervals);
                        Coordinates spawnPosition = SimulationSettings.WorldRootPosition + offsetFromWorldRoot + positionJitter;
                        AddTree(spawnPosition);
                        placedTreeCount++;
                    }
                }
            }
        }

        private void AddTree(Coordinates position)
        {
            var treeEntityId = GenerateId();
            var spawnRotation = (uint)Mathf.CeilToInt((float)rand.NextDouble() * 360);
            snapshotEntities.Add(treeEntityId, EntityTemplateFactory.CreateTreeTemplate(position, spawnRotation));
        }

        private void SetupTeamContainers()
        {
            for (var teamCount = 0; teamCount < SimulationSettings.TeamCount; teamCount++)
            {
                teamList.Add(new Team(new Improbable.Collections.List<EntityId>()));
            }
        }

        private void AddHQs()
        {
            for (int teamId = 0; teamId < SimulationSettings.TeamHQLocations.Length; teamId++)
            {
                var hqentityId = GenerateId();
                snapshotEntities.Add(hqentityId, EntityTemplateFactory.CreateHQEntitySnapShotTemplate(SimulationSettings.TeamHQLocations[teamId], 0, (uint)teamId));
            }
        }

        private void AddNPCsAroundHQs()
        {
            for (int teamId = 0; teamId < SimulationSettings.TeamHQLocations.Length; teamId++)
            {
                SpawnNpcsAroundPosition(SimulationSettings.TeamHQLocations[teamId], (uint)teamId);
            }
        }

        private void SpawnNpcsAroundPosition(Coordinates position, uint team)
        {
            float totalNpcs = SimulationSettings.HQStartingWizardsCount + SimulationSettings.HQStartingLumberjacksCount;
            float radiusFromHQ = SimulationSettings.HQSpawnRadius;

            for (int i = 0; i < totalNpcs; i++)
            {
                float x = radiusFromHQ*Mathf.Cos((i/totalNpcs)*2*Mathf.PI);
                float z = radiusFromHQ*Mathf.Sin((i/totalNpcs)*2*Mathf.PI);

                Coordinates coordinates = (position.ToVector3() + new Vector3(x, 0, z)).ToCoordinates();

                if (i < SimulationSettings.HQStartingLumberjacksCount)
                {
                    snapshotEntities.Add(GenerateId(), EntityTemplateFactory.CreateNPCLumberjackTemplate(coordinates, team));
                }
                else
                {
                    snapshotEntities.Add(GenerateId(), EntityTemplateFactory.CreateNPCWizardSnapshotTemplate(coordinates, team));
                }
            }
        }

        private EntityId GenerateId()
        {
            return new EntityId(currentEntityId++);
        }
    }
}
