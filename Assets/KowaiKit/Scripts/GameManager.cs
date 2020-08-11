using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KowaiKit;

namespace KowaiKit
{
    public class GameManager : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] public Survivor Survivor;
        [SerializeField] public Chaser Chaser;
        [SerializeField] public PlayerUI PlayerUi;
        [SerializeField] public int MissionItemCount = 5;
        [SerializeField] public AudioClip AudioChase;

        public event Action<bool> OnGameFinish;
        private TickTimer _finishTimer;
        private bool _isFinished = false;
        private AudioSource _audioSourceChase;

        private void Awake()
        {
            _audioSourceChase = transform.Find("AudioSourceChase").GetComponent<AudioSource>();
        }

        void Start()
        {
            Survivor.OnGetItemEvent += (count) =>
            {
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
            };
            Chaser = Chaser.GetComponent<Chaser>();
            Chaser.OnKilled += () =>
            {
                if (!_isFinished)
                {
                    Survivor.Scream();
                    _finishTimer.Start();
                    PlayerUi.ShowGameFinishView(false);
                    _isFinished = true;
                }
            };
            Survivor.HandLight.OnUpdateBattery += (remain) =>
            {
                PlayerUi.UpdateFlashLightBattery(remain);
            };
            Survivor.OnGetItemEvent += (itemCount) =>
            {
                PlayerUi.UpdateItemStatus(itemCount, MissionItemCount);
            };
            _finishTimer = new TickTimer(3f, () =>
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _finishTimer.Stop();
                SceneManager.LoadScene("TitleScene");
            });
            _finishTimer.Stop();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PlayerUi.UpdateItemStatus(Survivor.CurrentItemCount, MissionItemCount);
            PlayerUi.UpdateFlashLightBattery(Survivor.HandLight.RemainBattery);
            _audioSourceChase.clip = AudioChase;
            Chaser.SearchArea.OnDetect += (collider) =>
            {
                if (!_audioSourceChase.isPlaying)
                {
                    _audioSourceChase.Play();
                }
            };
            Chaser.SearchArea.OnLost += (collider) =>
            {
                if (_audioSourceChase.isPlaying)
                {
                    _audioSourceChase.Stop();
                }
            };
        }

        // Update is called once per frame
        void Update()
        {
            _finishTimer.UpdateTime();
        }
    }

}