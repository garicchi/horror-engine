using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

namespace KowaiKit
{
    /// <summary>
    /// 追跡者のスクリプト
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]  // 追跡者はNavMeshAgentを使って追跡する
    [RequireComponent(typeof(CapsuleCollider))]  // 追跡者がプレイヤーに追いついた時の判定コライダー
    public class Chaser : MonoBehaviour
    {
        /// <summary>
        /// inspectorで設定可能なフィールド
        /// </summary>
        [SerializeField] public SearchArea SearchArea;
        [SerializeField] public AudioSource AudioSourceWalk;
        [SerializeField] public AudioClip AudioWalk;  // 追跡者が歩いている時の足音

        [SerializeField] public float NormalSpeed = 3.5f;  // 通常時の歩行スピード
        [SerializeField] public float ChaseSpeed = 9.5f;  // 追跡時のスピード
        
        /// <summary>
        /// イベント
        /// </summary>
        public event Action OnKilled;  // サバイバーを殺したとき
        
        /// <summary>
        /// privateフィールド一覧
        /// </summary>
        private Survivor _survivor;  // 追跡対象のプレイヤーのオブジェクト
        private PatrolPoint[] _patrolPoints;  // 巡回ポイント
        private NavMeshAgent _agent; // 巡回と追跡を制御するコンポーネント
        private int _nextPointIndex; // 次の巡回ポイントのインデックス
        private bool _isChaseMode = false;  // 追跡モードかどうか
        private bool _isKilled = false; // サバイバーを殺したかどうか
        

        /// <summary>
        /// 初期化
        /// </summary>
        void Start()
        {
            _survivor = FindObjectsOfType<Survivor>()[0]; // 追跡対象ははSurvivorクラスがAttachされているものとする
            _patrolPoints = FindObjectsOfType<PatrolPoint>(); // 巡回ポイントはPatrolPointクラスがAttachされているものとする
            // 巡回ポイントはランダムにシャッフルする
            _patrolPoints = _patrolPoints.OrderBy(q => Guid.NewGuid()).ToArray();

            _nextPointIndex = 0;
            AudioSourceWalk.clip = AudioWalk;
            AudioSourceWalk.Play();
            
            _agent = GetComponent<NavMeshAgent>();
            _agent.autoBraking = false;
            _agent.speed = NormalSpeed;
            
            // 追跡者を発見した時のイベント
            SearchArea.OnDetect += (collider) =>
            {
                _isChaseMode = true;
                _agent.speed = ChaseSpeed;
                // 目的地をプレイヤーにする
                _agent.destination = _survivor.transform.position;
            };
            // 追跡者を見失ったときのイベント
            SearchArea.OnLost += (collider) =>
            {
                _isChaseMode = false;
                _agent.speed = NormalSpeed;
                // 目的地を次の巡回ポイントにする
                UpdateNextPoint();
            };
            
            UpdateNextPoint();
        }
        
        // フレームごとの処理
        void Update()
        {
            // 追跡モードでない場合、巡回ポイントに接近したら次の巡回ポイントに目的地を変更する
            if (!_isChaseMode)
            {
                if (Vector3.Distance(transform.position, _agent.destination) < 1.5f)
                {
                    UpdateNextPoint();
                }
            }
        }

        /// <summary>
        /// 次の巡回ポイントへと目的地を変更する
        /// </summary>
        void UpdateNextPoint()
        {
            _nextPointIndex++;
            if (_nextPointIndex > (_patrolPoints.Length - 1))
            {
                _nextPointIndex = 0;
            }
            _agent.destination = _patrolPoints[_nextPointIndex].transform.position;
        }

        /// <summary>
        /// コライダーに接触があったら
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            // 対象がSurvivorかどうかを判定してSurvivorなら殺す
            Survivor s = null;
            if (other.TryGetComponent(out s))
            {
                if (!_isKilled)
                {
                    _agent.isStopped = true;
                    OnKilled();
                    _isKilled = true;
                }
            }
        }

        /// <summary>
        /// 追跡を停止するかどうかを変更する
        /// </summary>
        /// <param name="isStop"></param>
        public void ChangeChaseState(bool isStop)
        {
            _agent.isStopped = isStop;
        }
    }
}
