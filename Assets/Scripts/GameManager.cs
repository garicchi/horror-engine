using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public GameObject Player;
    [SerializeField] public GameObject Chaser;
    [SerializeField] public GameObject ImageGameOver;
    [SerializeField] public GameObject ImageGameClear;
    private PlayerController _playerController;
    private ChaserController _chaserController;
    private TickTimer _finishTimer;
    void Start()
    {
        _playerController = Player.GetComponent<PlayerController>();
        _playerController.CompleteMissionEvent += () =>
        {
            ImageGameClear.SetActive(true);
            _chaserController.ChangeChaseState(true);
            _finishTimer.Start();
        };
        _chaserController = Chaser.GetComponent<ChaserController>();
        _chaserController.OnKilledEvent += () =>
        {
            ImageGameOver.SetActive(true);
            _playerController.Scream();
            _finishTimer.Start();
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
    }

    // Update is called once per frame
    void Update()
    {
        _finishTimer.UpdateTime();
    }
}
