using UnityEngine;
using UnityEngine.AI;

public class MovePlayer : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;

    [SerializeField] private Camera _cam;

    private bool _isJumping = false;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _navMeshAgent.updateRotation = false;
    }

    void Update()
    {
        if (_isJumping) return;

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _navMeshAgent.SetDestination(hit.point);
            }
        }

        if (_navMeshAgent.isOnOffMeshLink && !_isJumping)
        {
            StartCoroutine(JumpAcrossLink());
            return;
        }

        Vector3 velocity = _navMeshAgent.velocity;

        if (velocity.sqrMagnitude > 0.01f)
        {
            transform.forward = velocity.normalized;
        }

        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        float speed = velocity.magnitude;
        float x = localVelocity.x;
        float y = localVelocity.z;

        _animator.SetFloat("X", x);
        _animator.SetFloat("Y", y);
        _animator.SetFloat("Speed", speed);
    }

    System.Collections.IEnumerator JumpAcrossLink()
    {
        _isJumping = true;

        OffMeshLinkData data = _navMeshAgent.currentOffMeshLinkData;
        Vector3 startPos = transform.position;
        Vector3 endPos = data.endPos + Vector3.up * _navMeshAgent.baseOffset;

        float jumpDuration = 0.5f;
        float time = 0;

        _navMeshAgent.isStopped = true;
        _animator.SetTrigger("Jump");

        while (time < jumpDuration)
        {
            float t = time / jumpDuration;
            float height = Mathf.Sin(Mathf.PI * t) * 1.5f;
            transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        _navMeshAgent.CompleteOffMeshLink();
        _navMeshAgent.isStopped = false;
        _isJumping = false;
    }
}
