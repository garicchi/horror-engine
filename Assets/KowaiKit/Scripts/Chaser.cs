using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

namespace KowaiKit
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CapsuleCollider))]

    public class Chaser : MonoBehaviour
    {
        [SerializeField] public AudioClip AudioWalk;
        [NonSerialized] public SearchArea SearchArea;
        private AudioSource _audioWalk;
        private Survivor[] _survivors;
        private PatrolPoint[] _patrolPoints;
        private NavMeshAgent _agent;
        private float _normalSpeed = 3.5f;
        private float _chaseSpeed = 8.5f;

        private int _nextPointIndex;

        private bool _isChaseMode = false;

        private bool _isKilled = false;
        
        public event Action OnKilled;

        private void Awake()
        {
            SearchArea = transform.Find("SearchArea").GetComponent<SearchArea>();
            _audioWalk = transform.Find("AudioWalk").GetComponent<AudioSource>();
        }

        // Start is called before the first frame update
        void Start()
        {
            _survivors = FindObjectsOfType<Survivor>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.autoBraking = false;
            _agent.speed = _normalSpeed;
            SearchArea.OnDetect += (collider) =>
            {
                _isChaseMode = true;
                _agent.speed = _chaseSpeed;
            };
            SearchArea.OnLost += (collider) =>
            {
                _isChaseMode = false;
                _agent.speed = _normalSpeed;
            };
            _nextPointIndex = 0;
            _audioWalk.clip = AudioWalk;
            _audioWalk.Play();
            _patrolPoints = FindObjectsOfType<PatrolPoint>();

            _patrolPoints = _patrolPoints.OrderBy(q => Guid.NewGuid()).ToArray();
            UpdateNextPoint();
        }

        void UpdateNextPoint()
        {
            bool is_detect = false;
            _nextPointIndex++;
            if (_nextPointIndex > (_patrolPoints.Length - 1))
            {
                _nextPointIndex = 0;
            }

            _agent.destination = _patrolPoints[_nextPointIndex].transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (_isChaseMode)
            {
                _agent.destination = _survivors[0].transform.position;
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
            Survivor s = null;
            if (other.TryGetComponent(out s))
            {
                Debug.Log("killed");
                if (!_isKilled)
                {
                    _agent.isStopped = true;
                    OnKilled();
                    _isKilled = true;
                }
            }
        }

        public void ChangeChaseState(bool isStop)
        {
            _agent.isStopped = isStop;
        }
    }
}
