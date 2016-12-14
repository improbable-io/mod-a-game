using UnityEngine;
using Improbable.Core;
using Improbable.Unity.Visualizer;

public class TeamAssignmentVisualizer : MonoBehaviour {

    [Require]
    private TeamAssignment.Reader teamAssignmentReader;

    [SerializeField]
    private uint teamId;

    public uint TeamId
    {
        get { return teamId; }
        private set { teamId = value; }  
    }

    public void OnEnable ()
	{
        TeamId = teamAssignmentReader.Data.teamId;
	    teamAssignmentReader.ComponentUpdated += OnTeamAssignmentComponentUpdate;
	}
	
	public void OnDisable ()
    {
        teamAssignmentReader.ComponentUpdated -= OnTeamAssignmentComponentUpdate;
    }

    private void OnTeamAssignmentComponentUpdate(TeamAssignment.Update update)
    {
        if (update.teamId.HasValue)
        {
            TeamId = update.teamId.Value;
        }
    }
}
