using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class ChaserController : MonoBehaviour
{
    [SerializeField] public Transform[] Points;

    [SerializeField] public GameObject Player;
    private NavMeshAgent _agent;

    private int _nextPointIndex;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.autoBraking = false;
        _nextPointIndex = -1;
        UpdateNextPoint();
    }

    void UpdateNextPoint()
    {
        if (_nextPointIndex > (Points.Length - 1))
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
        if (Vector3.Distance(transform.position, _agent.destination) < 1.5f)
        {
            UpdateNextPoint();
        }
    }
}
