using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KowaiKit;

namespace KowaiKit
{
    /// <summary>
    /// ゲームの管理クラス
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// inspectorで設定するフィールド
        /// </summary>
        [SerializeField] public Survivor Survivor;
        [SerializeField] public Chaser Chaser;
        [SerializeField] public PlayerUI PlayerUi;
        [SerializeField] public int MissionItemCount = 5;  // ゲームをクリアするのに必要なアイテムの数
        [SerializeField] public AudioClip AudioChase;  // 追跡BGM

        /// <summary>
        /// イベント
        /// </summary>
        public event Action<bool> OnGameFinish;
        
        /// <summary>
        /// privateフィールド
        /// </summary>
        private TickTimer _finishTimer;
        private bool _isFinished = false;
        private AudioSource _audioSourceChase;

        /// <summary>
        /// FindはStartより先に初期化する
        /// </summary>
        private void Awake()
        {
            _audioSourceChase = transform.Find("AudioSourceChase").GetComponent<AudioSource>();
        }

        /// <summary>
        /// 初期化
        ///   各ゲームオブジェクトのイベントから、別のゲームオブジェクトに作用することでゲームを動かす
        /// </summary>
        void Start()
        {
            // Survivorがアイテムを取得したとき
            Survivor.OnGetItemEvent += (count) =>
            {
                // ミッションアイテム数に到達したらゲームを終了する
                if (count >= MissionItemCount)
                {
                    if (!_isFinished)
                    {
                        Chaser.ChangeChaseState(true);
                        PlayerUi.ShowGameFinishView(true);
                        _finishTimer.Start();
                        _isFinished = true;
                    }
                }
                // UIのアイテム数を更新する
                PlayerUi.UpdateItemStatus(count, MissionItemCount);
            };
            Chaser = Chaser.GetComponent<Chaser>();
            // 追跡者がSurvivorを殺した時
            Chaser.OnKilled += () =>
            {
                // ゲームが終了していないなら、終了する
                if (!_isFinished)
                {
                    Survivor.Scream();
                    _finishTimer.Start();
                    PlayerUi.ShowGameFinishView(false);
                    _isFinished = true;
                }
            };
            // ハンドライトのバッテリーが減少したら
            Survivor.HandLight.OnUpdateBattery += (remain) =>
            {
                // UIのバッテリーを減らす
                PlayerUi.UpdateFlashLightBattery(remain);
            };
            // Survivorが殺されてからゲームが終了するまでのタイマー(3秒)
            _finishTimer = new TickTimer(3f, () =>
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _finishTimer.Stop();
                SceneManager.LoadScene("TitleScene");
            });
            _finishTimer.Stop();
            // マウスカーソルをロックし、非表示にする
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerUi.UpdateItemStatus(Survivor.CurrentItemCount, MissionItemCount);
            PlayerUi.UpdateFlashLightBattery(Survivor.HandLight.RemainBattery);
            _audioSourceChase.clip = AudioChase;
            // 追跡者がサバイバーを発見したら追跡BGMを鳴らす
            Chaser.SearchArea.OnDetect += (collider) =>
            {
                if (!_audioSourceChase.isPlaying)
                {
                    _audioSourceChase.Play();
                }
            };
            // 追跡者がサバイバーを見失ったら追跡BGMを消す
            Chaser.SearchArea.OnLost += (collider) =>
            {
                if (_audioSourceChase.isPlaying)
                {
                    _audioSourceChase.Stop();
                }
            };
        }

        /// <summary>
        /// フレームごとの処理
        /// </summary>
        void Update()
        {
            _finishTimer.UpdateTime();
        }
    }

}