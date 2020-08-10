using System;
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
    [SerializeField] public GameObject AudioChase;
    public event Action OnKilledEvent;
    private AudioSource _audioChase;
    private NavMeshAgent _agent;
    private float _normalSpeed = 3.5f; 
    private float _chaseSpeed = 8.5f;

    private int _nextPointIndex;

    private ChaseEyeController _chaseEyeController;

    private bool _isChaseMode = false;

    private bool _isKilled = false;
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
        UpdateNextPoint();
        _audioChase = AudioChase.GetComponent<AudioSource>();
    }

    void UpdateNextPoint()
    {
        bool is_detect = false;
        for (int i = _nextPointIndex + 1; i < Points.Length; i++)
        {
            if (Points[i] != null)
            {
                _nextPointIndex = i;
                is_detect = true;
                break;
            }
        }

        if (!is_detect)
        {
            _nextPointIndex = 0;
        }
        _agent.destination = Points[_nextPointIndex].position;
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
