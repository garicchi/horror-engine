using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace KowaiKit
{
    /// <summary>
    /// プレイヤー(Survivor)のスクリプト
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class Survivor : MonoBehaviour
    {
        /// <summary>
        /// inspectorで設定可能なフィールド
        /// </summary>
        [SerializeField] public float SpeedMove = 8.0f;  // 移動スピード
        [SerializeField] public float SpeedRotate = 40.0f;  // 視点回転スピード
        [SerializeField] public AudioClip AudioWalk;  // 歩いている時のAudio
        [SerializeField] public AudioClip AudioScream;  // 殺されたときの叫びAudio
        
        /// <summary>
        /// inspectorで設定したくないがpublicにしたいフィールド
        /// </summary>
        [NonSerialized] public FlashLight HandLight;  // ハンドライト
        [NonSerialized] public int CurrentItemCount = 0;  // すでに持っているアイテム数
        
        /// <summary>
        /// イベント
        /// </summary>
        public event Action<int> OnGetItemEvent;  // アイテムを取得したときのイベント
        
        /// <summary>
        /// privateフィールド
        /// </summary>
        private float _cameraVerticalAngle = 0f;
        private CharacterController _characterController;
        private Camera _faceCamera;
        private AudioSource _audioSourceWalk;
        private AudioSource _audioSourceScream;

        /// <summary>
        /// FindはStartよりも先にする
        /// </summary>
        private void Awake()
        {
            HandLight = transform.Find("HandLight").GetComponent<FlashLight>();
            _faceCamera = transform.Find("FaceCamera").GetComponent<Camera>();
            _audioSourceWalk = transform.Find("SoundWalk").GetComponent<AudioSource>();
            _audioSourceScream = transform.Find("SoundScream").GetComponent<AudioSource>();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        void Start()
        {
            _characterController = GetComponent<CharacterController>();
            
            _audioSourceWalk.clip = AudioWalk;
            
            _audioSourceScream.clip = AudioScream;
        }

        /// <summary>
        /// フレームごとの処理
        /// </summary>
        void Update()
        {
            // WASDから移動ベクトルを取得する
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");
            var forward = transform.forward * v * SpeedMove * Time.deltaTime;
            var right = transform.right * h * SpeedMove * Time.deltaTime;
            var moveVec = forward + right;
            // 移動ベクトルが0以上なら歩く音声を流す
            if (moveVec.magnitude > 0)
            {
                if (!_audioSourceWalk.isPlaying)
                {
                    _audioSourceWalk.Play();
                }
            }
            else
            {
                if (_audioSourceWalk.isPlaying)
                {
                    _audioSourceWalk.Stop();
                }
            }
            // 移動ベクトルを使って移動する
            _characterController.Move(moveVec);

            // マウスから視点の回転を得る
            var rotateHorizontal = Input.GetAxis("Mouse X") * SpeedRotate * Time.deltaTime;
            var rotateVertical = -1f * Input.GetAxis("Mouse Y") * SpeedRotate * Time.deltaTime;
            // 横向きの視点を回転する
            transform.Rotate(Vector3.up, rotateHorizontal);
            // 縦向きの視点はカメラとFlashLightのみ回転する
            _cameraVerticalAngle += rotateVertical;
            _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -50f, 50f);  // 最大回転角度は50度とする
            _faceCamera.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);
            HandLight.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);

            // 右クリックでFlashLightをオンオフする
            if (Input.GetMouseButtonDown(1))
            {
                HandLight.ToggleSwitch();
            }

            // 左クリックでアイテムの取得をする
            if (Input.GetMouseButtonDown(0))
            {
                // facecameraからrayを放ってヒットしたらアイテムの取得処理を行う
                var ray = _faceCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                var hit = new RaycastHit();
                if (Physics.Raycast(ray ,out hit))
                {
                    var hitObj = hit.collider.gameObject;
                    MissionItem item; 
                    if (hitObj.TryGetComponent(out item))
                    {
                        hitObj.SetActive(false);
                        CurrentItemCount++;
                        OnGetItemEvent(CurrentItemCount);
                    }

                    Battery battery;
                    if (hitObj.TryGetComponent(out battery))
                    {
                        hitObj.SetActive(false);
                        HandLight.Charge(30);
                    }
                }
            }
        }

        /// <summary>
        /// 叫ぶ
        /// </summary>
        public void Scream()
        {
            _audioSourceScream.loop = false;
            _audioSourceScream.Play();
        }
    }
}