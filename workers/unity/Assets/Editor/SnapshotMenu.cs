using Assets.Gamelogic.EntityTemplate;
using System;
using UnityEditor;

namespace Assets.Editor
{
    [InitializeOnLoad]
    public static class SnapshotMenu
    {
        private static Action improbableBuild;

        static SnapshotMenu()
        {
            improbableBuild = Improbable.Unity.EditorTools.Build.SimpleBuildSystem.BuildAction;
            Improbable.Unity.EditorTools.Build.SimpleBuildSystem.BuildAction = InjectBuild;
        }

        [MenuItem("Improbable/Snapshots/Generate Snapshot Programmatically %#&w")]
        private static void GenerateSnapshotProgrammatically()
        {
            new SnapshotBuilder().CreateSnapshot();
        }

        private static void InjectBuild()
        {
            improbableBuild();
            GenerateSnapshotProgrammatically();
        }
    }
}
