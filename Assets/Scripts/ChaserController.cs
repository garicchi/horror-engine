using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class ChaserController : MonoBehaviour
{
    [SerializeField] public Transform[] Points;

    [SerializeField] public GameObject Player;
    [SerializeField] public GameObject Eye;
    private NavMeshAgent _agent;
    private float _normalSpeed = 3.5f; 
    private float _chaseSpeed = 8.5f;

    private int _nextPointIndex;

    private ChaseEyeController _chaseEyeController;

    private bool _isChaseMode = false;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.autoBraking = false;
        _agent.speed = _normalSpeed;
        _chaseEyeController = Eye.GetComponent<ChaseEyeController>();
        _chaseEyeController.OnDetect += (collider) =>
        {
            if (collider.gameObject == Player)
            {
                Debug.Log("Detect");
                _isChaseMode = true;
                _agent.speed = _chaseSpeed;
            }
        };
        _chaseEyeController.OnLost += (collider) =>
        {
            if (collider.gameObject == Player)
            {
                Debug.Log("Lost");
                _isChaseMode = false;
                _agent.speed = _normalSpeed;
            }
        };
        _nextPointIndex = -1;
        UpdateNextPoint();
        
    }

    void UpdateNextPoint()
    {
        if (_nextPointIndex >= (Points.Length - 1))
        {
            _nextPointIndex = 0;
        }
        else
        {
            _nextPointIndex++;
        }
        _agent.destination = Points[_nextPointIndex].position;
        Debug.Log(_nextPointIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isChaseMode)
        {
            _agent.destination = Player.transform.position;
        }
        else
        {
            if (Vector3.Distance(transform.position, _agent.destination) < 1.5f)
            {
                UpdateNextPoint();
            }
        }
    }
}
