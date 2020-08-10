using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] public GameObject ProgressBarBattery;
    private ProgressBarController _progreesBarController;

    [SerializeField] public GameObject FlashLight;

    private FlashLightController _flashLightController;

    [SerializeField] public Text TextItemStatus;

    [SerializeField] public GameObject Player;

    private PlayerController _playerController;
    // Start is called before the first frame update
    void Start()
    {
        _flashLightController = FlashLight.GetComponent<FlashLightController>();
        _progreesBarController = ProgressBarBattery.GetComponent<ProgressBarController>();
        _playerController = Player.GetComponent<PlayerController>();
        _playerController.GetItemEvent += (itemCount) =>
        {
            TextItemStatus.text = $"{itemCount}/10 取得";
        };
    }

    // Update is called once per frame
    void Update()
    {    
        _progreesBarController.Progress = _flashLightController.RemainBattery / 100f;
    }
}
