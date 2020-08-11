using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]

public class ChaserController : MonoBehaviour
{
    [SerializeField] public Transform[] Points;

    [SerializeField] public GameObject Player;
    [SerializeField] public GameObject Eye;
    [SerializeField] public GameObject AudioChase;
    [SerializeField] public GameObject AudioWalk;
    public event Action OnKilledEvent;
    private AudioSource _audioChase;
    private AudioSource _audioWalk;
    private NavMeshAgent _agent;
    private float _normalSpeed = 3.5f; 
    private float _chaseSpeed = 8.5f;

    private int _nextPointIndex;

    private ChaseEyeController _chaseEyeController;

    private bool _isChaseMode = false;

    private bool _isKilled = false;

    private List<Transform> _patrolPointList;
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
                if (!_audioChase.isPlaying)
                {
                    _audioChase.Play();
                }
            }
        };
        _chaseEyeController.OnLost += (collider) =>
        {
            if (collider.gameObject == Player)
            {
                Debug.Log("Lost");
                _isChaseMode = false;
                _agent.speed = _normalSpeed;
                if (_audioChase.isPlaying)
                {
                    _audioChase.Stop();
                }
            }
        };
        _nextPointIndex = 0;
        _audioChase = AudioChase.GetComponent<AudioSource>();
        _audioWalk = AudioWalk.GetComponent<AudioSource>();
        _audioWalk.Play();
        _patrolPointList = new List<Transform>();
        foreach (var p in Points)
        {
            if (p != null)
            {
                _patrolPointList.Add(p);
            }
        }

        _patrolPointList = _patrolPointList.OrderBy(q => Guid.NewGuid()).ToList();
        UpdateNextPoint();
    }

    void UpdateNextPoint()
    {
        bool is_detect = false;
        _nextPointIndex++;
        if (_nextPointIndex > (_patrolPointList.Count - 1))
        {
            _nextPointIndex = 0;
        } 
        _agent.destination = _patrolPointList[_nextPointIndex].position;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("killed");
            if (!_isKilled)
            {
                _agent.isStopped = true;
                OnKilledEvent();
                _isKilled = true;
            }
        }
    }

    public void ChangeChaseState(bool isStop)
    {
        _agent.isStopped = isStop;
    }
}
