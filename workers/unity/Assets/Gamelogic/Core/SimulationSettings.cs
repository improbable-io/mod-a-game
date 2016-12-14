using Improbable;
using Improbable.Math;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    public static class SimulationSettings
    {
        // Entity Prefab Names
        public static string PlayerPrefabName = "Player";
        public static string NPCPrefabName = "NPCLumberjack";
        public static string NPCWizardPrefabName = "NPCWizard";
        public static string HQPrefabName = "HQ";
        public static string TreePrefabName = "Tree";
        public static string StockpilePrefabName = "Stockpile";
        public static string BarracksPrefabName = "Barracks";
        public static string SimulationManagerEntityName = "SimulationManagerEntity";
        public static string LightningPrefabName = "Lightning";
        public static string TeamPrefabName = "Team";

        // Resource Prefab Paths
        public static string SpellAOEIndicatorPrefabPath = "UI/SpellAOEIndicator";
        public static string LightningEffectPrefabPath = "UI/LightningEffect";
        public static string RainEffectPrefabPath = "UI/RainEffect";
        public static string FireEffectPrefabPath = "UI/Fire";
        public static string SmallFireEffectPrefabPath = "UI/SmallFire";
        public static string EntityInfoCanvasPrefabPath = "UI/EntityInfoCanvas";
        public static string DeathEffectPrefabPath = "UI/DeathEffect";

        // Unity Layers
        public static string TerrainLayerName = "Terrain";

        // UI
        public static Color TransparentWhite = new Color(1f, 1f, 1f, 0.3f);

        // Unity tags
        public static string HealthyTreeTag = "Tree";
        public static string TreeStumpTag = "Stump";
        public static string BurningTreeTag = "BurningTree";
        public static string BurntTreeTag = "BurntTree";
        public static string StockpileTag = "Stockpile";
        public static string BarracksTag = "Barracks";
        public static string TerrainTag = "Terrain";

        // World
        public static double SpawningWorldEdgeLength = 1000;
        public static Coordinates WorldRootPosition = new Coordinates(-SpawningWorldEdgeLength / 2d, 0d, -SpawningWorldEdgeLength / 2d);
        public static float SimulationTickInterval = 1f;

        // Tree
        public static int InitialResourceTotal = 3;
        public static int HarvestReturnQuantity = 1;
        public static int TreeBurningTimeSecs = 10;
        public static int TreeStumpRegrowthTimeSecs = 300;
        public static int BurntTreeRegrowthTimeSecs = 100;
        public static float TreeIgnitionTimeBuffer = 0.4f;
        public static float TreeExtinguishTimeBuffer = 1f;
        public static float TreeCutDownTimeBuffer = 1f;

        // Entity counts
        public static int AttemptedTreeCount = 30000;
        public static float TreeJitter = 2.0f;
        public static int NPCCount = 40;
		public static int TeamCount = 2;

        // Component Updates
        public static int TransformUpdatesToSkipBetweenSends = 5;
        public static float AngleQuantisationFactor = 2f;
        
        // Worker Connection
        public static int TargetFramerate = 120;
        public static int FixedFramerate = 10;
        public static float PlayerHeartbeatInterval = 10f;
        public static float PlayerDeletionInterval = 10f;
        public static float PlayerHeartbeatInactivityMaxDuration = 30f;

        // Player Life Cycle
        public static int PlayerMaxHealth = 10;
        public static float PlayerRespawnDelay = 4f;

        // Fire
        public static float FireSpreadInterval = 1f;
        public static float FireSpreadRadius = 6f;
        public static float FireSpreadProbability = 0.5f;
        public static int FireDamagePerTick = 1;
        public static float OnFireMovementSpeedIncreaseFactor = 3f;

        // Player Controls
        public static float PlayerMovementSpeed = 5f;
        public static float PlayerPositionUpdateMinSqrDistance = 0.001f;
        public static float PlayerPositionUpdateMaxSqrDistance = PlayerMovementSpeed * PlayerMovementSpeed * OnFireMovementSpeedIncreaseFactor * OnFireMovementSpeedIncreaseFactor * 4f;
        public static float MaxRaycastDistance = 40f;
        public static float PlayerMovementTargetSlowingThreshold = 0.005f;

        // Player Spells
        public static float PlayerSpellCastRange = 10f;
        public static float PlayerSpellAOEDiameter = 4f;
        public static int MaxSpellTargets = 64;
        public static float SpellEffectDuration = 2f;
        public static float SpellCooldown = 10f;
        public static float RainCloudSpawnHeight = 7f;
        public static float PlayerCastAnimationTime = 0.7f;
        public static float PlayerCastAnimationBuffer = 0.5f;

        // Player Input Buttons
        public static int CastSpellMouseButton = 0;
        public static int RotateCameraMouseButton = 1;
        public static KeyCode CastLightningKey = KeyCode.E;
        public static KeyCode CastRainKey = KeyCode.R;
        public static KeyCode SubmitChatKey = KeyCode.Return;
        public static KeyCode AbortKey = KeyCode.Escape;

        // Camera Controls
        public static float CameraSensitivity = 2f;
        public static float CameraDefaultDistance = 20f;
        public static float CameraMinDistance = 14f;
        public static float CameraMaxDistance = 24f;
        public static float CameraDistanceSensitivity = 2f;
        public static float CameraMaxPitch = 65f;
        public static float CameraInitialPitch = 15f;
        public static float CameraMinPitch = 20f;
        public static float CameraDefaultPitch = 30f;

        // NPC
        public static bool NPCDeathActive = true;
        public static int LumberjackMaxHealth = 5;
        public static int WizardMaxHealth = 5;
        public static int NPCSpawningWorldEdgeLength = Mathf.CeilToInt(4 * (float)SpawningWorldEdgeLength / 5);
        public static float NPCTargetSettingDistance = 40f;
        public static float NPCProgressUpdateIntervalSeconds = 0.4f;
        public static float NPCMovementSpeed = 2f;
        public static float NPCDefaultInteractionRange = 5f;
        public static float NPCChoppingRange = 3f;
        public static float NPCStockpilingRange = 3f;
        public static float NPCCollisionDodgeDistance = 4f;
        public static float NPCResourceGatheringPreferredDistance = 40f;
        public static int NPCCandidateTrees = 5;
        public static float NPCInteractionProximityBuffer = 0.8f;
        public static float NPCInteractionDelayTime = 0.2f;
        public static float NPCChoppingAnimationBuffer = 0.2f;
        public static float NPCChoppingAnimationFinishTime = 2f;
        public static float NPCStockpilingAnimationBuffer = 0.2f;
        public static float NPCStockpilingAnimationFinishTime = 2f;
        public static float NPCWizardInteractionRange = 20f;
        public static float OnFireWaypointDistance = 10f;
        public static float NPCWizardSearchToAttackRadius = 25f;
        public static float NPCWizardSearchForTreeRadius = 50f;
        public static float NPCWizardRandomWalkRadius = 10f;
        public static float NPCWizardDetectEnemiesInterval = 0.1f;
        public static float HQSpawnRadius = 30f;

        // Buildings
        public static int HQMaxHealth = 20;
        public static int BarracksMaxHealth = 10;
        public static float LumberjackSpawningCooldown = 20f;
        public static float WaizardSpawningCooldown = 5;
        public static float SpawnOffsetFactor = 2f;
        public static float PlayerSpawnOffsetFactor = 16.0f;
        public static int HQStartingLumberjacksCount = 20;
        public static int HQStartingWizardsCount = 5;
        public static float DefaultHQBarracksSpawnRadius = 20f;
        public static float MaxHQBarracksSpawnRadius = 200f;
        public static float HQBarracksSpawnRadiusIncrease = 10f;
        public static int HQBarracksSpawnPositionPickingRetries = 4;
        public static float HQBarracksSpawnPositionSamplingHeight = 4f;
        public static float HQBarracksSpawnPositionSamplingRadius = 6f;
        public static float NPCWizardDefensivePriority = 0.8f;
        public static float HQBarracksSpawningSeparation = 15f;

        // Heartbeats
        public const uint HeartbeatMax = 3;
        public const float HeartbeatInterval = 3.0f;

        // Teams
        public const int RedTeamId = 0;
        public const int BlueTeamId = 1;

        public static Coordinates[] TeamHQLocations = 
        {
            new Coordinates(-150.0, 0.0, -100.0),
            new Coordinates(230.0, 0.0, 150.0)
        };

        public static EntityId[] HQEntityIds =
        {
            new EntityId(0), new EntityId(1)
        };

        // Audio Volume
        public static float NPCChopVolume = 0.6f;
        public static float RainVolume = 0.8f;
        public static float LightningStrikeVolume = 0.4f;
        public static float IgnitionVolume = 1f;
        public static float FireVolume = 1f;
        public static float ExtinguishVolume = 0.4f;
        public static float SpellChannelVolume = 0.8f;
        public static float FootstepVolume = 0.8f;

        // Animation
        public static float DeathEffectSpawnHeight = 1f;

        // Snapshot
        public static string SnapshotFileName = "initial_world.snapshot";
        public static string PerlinNoiseTexture = "perlin";
    }
}
