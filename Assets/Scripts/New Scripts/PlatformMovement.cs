using UnityEngine;
using Unity.AI.Navigation;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField] private Vector3 _localOffsetA = new Vector3(-3, 0, 0);
    [SerializeField] private Vector3 _localOffsetB = new Vector3(3, 0, 0);
    [SerializeField] private float _speed = 2f;

    private NavMeshSurface _surface;
    private bool _goingToPointB = true;

    private Vector3 _startPos;
    private Vector3 _targetA;
    private Vector3 _targetB;

    void Start()
    {
        _startPos = transform.position;
        _targetA = _startPos + _localOffsetA;
        _targetB = _startPos + _localOffsetB;

        _surface = FindObjectOfType<NavMeshSurface>();
    }

    void Update()
    {
        Vector3 target = _goingToPointB ? _targetB : _targetA;
        transform.position = Vector3.MoveTowards(transform.position, target, _speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
            _goingToPointB = !_goingToPointB;

        _surface.BuildNavMesh();
    }
}
