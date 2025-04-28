using System;
using Unity.VisualScripting;
using UnityEngine;

public class Lider : MoveNodeBase
{
    [SerializeField] public Team myTeam;
    [SerializeField] private bool _onDrawGizmos;

    internal MouseButton _buttonDown;

    public StateMachine stateMachine;
    public Vector3 safeZone;

    private void Start()
    {
        _buttonDown = myTeam == Team.Red ? MouseButton.Left : MouseButton.Right;

        stateMachine = gameObject.AddComponent<StateMachine>();

        stateMachine.AddState(LeaderState.Await, new Await_Leader(this));
        stateMachine.AddState(LeaderState.Walk, new Walk_Leader(this));
        stateMachine.AddState(LeaderState.Attack, new Attack_Leader(this));

        stateMachine.ChangeState(LeaderState.Await);
    }

    public void SetTargetNode()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Node nearestNode = FindNearestNode(hit.point);

            if (nearestNode != null)
            {
                TargetNode = nearestNode;
                Path = ThetaManager.FindPath(GetCurrentNode(), TargetNode);

                stateMachine.ChangeState(LeaderState.Walk);
            }
        }
    }

    public void MoveAlongPath()
    {
        if (Path.Count == 0) return;

        transform.position =
            Vector3.MoveTowards(transform.position, Path[0].transform.position, MoveSpeed * Time.deltaTime);
        transform.LookAt(Path[0].transform.position);

        if (Vector3.Distance(transform.position, Path[0].transform.position) < 0.1f)
        {
            Path.RemoveAt(0);
            stateMachine.ChangeState(LeaderState.Await);
        }
    }

    public enum Team
    {
        Red,
        Blue
    }

    public enum LeaderState
    {
        Await,
        Attack,
        Walk
    }
}