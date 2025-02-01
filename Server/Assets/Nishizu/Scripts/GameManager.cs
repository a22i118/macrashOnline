using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Linq;
using System.Net.Sockets;
using System;
using PlayerCS;
public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _hutons;
    [SerializeField] private GameObject _door;
    [SerializeField] private GameObject _teacherObj;
    [SerializeField] private GameObject _makuraPrefub;
    [SerializeField] private GameObject _happeningBall;
    [SerializeField] private GameObject _playerInputManager;
    [SerializeField] private GameObject _scoreManager;
    [SerializeField] private GameObject _clock;
    [SerializeField] private Camera _resultCamera;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameObject _result;
    [SerializeField] private GameObject _guide;
    [SerializeField] private GameObject _teacherGuide;
    [SerializeField] private GameObject _ready;
    [SerializeField] private GameObject _go;
    [SerializeField] private GameObject _finish;
    // private bool _isGameStart = false;
    // private bool _isPlayerSet = true;
    // private bool _isGameStartCheck = false;
    // private bool _isGameEnd = false;
    // private bool _isGuideKind = true;
    // private bool _isCoroutineSet = false;
    // private float _gameTime = 180.0f;
    // private float _teacherEventTime = 60.0f;
    // private string _startGuide = "コントローラーの接続を待っています... ";
    // private ResultManager _resultManager;
    // private PlayerInputManager _playerInputM;
    // private DoorController _doorController;
    // private Teacher _teacher;
    // private Event _event;
    // private TextMeshProUGUI _teacherComent;
    // private List<GameObject> _players;
    // private List<GameObject> _makuras = new List<GameObject>();
    // private List<MakuraController> _makuraControllers = new List<MakuraController>();
    // private List<PlayerController> _playerControllers = new List<PlayerController>();
    // private List<HappeningBall> _happeningBalls = new List<HappeningBall>();


    [SerializeField] private string _ipAddress;
    [SerializeField] private string _macAddress;
    [SerializeField] private string _priorityIntafaceName;    // 優先するインターフェースを文字列で指定（なければ空白）
    [SerializeField] private TMP_Dropdown _selectNetworkInterfaceDropdown;
    private List<NetworkInterfaceData> _networkInterfaces;    // 利用可能な機器のリスト
    // private List<PlayerBase> _players = new List<PlayerBase>();
    private PlayerInput _playerInput;
    // private Player _offlinePlayer;
    private UdpClient _udpClient;    // UDP通信のためのクラス
    [SerializeField] private string host = "localhost";    // ServerのIpAddressを指定する（localhostは自分自身）
    private int _portServer = 53724;    // 使用するPort(Server)
    private int _portClient = 53725;    // 使用するPort(自分:Client)
    UdpBuffer _udpTransmitter = new UdpBuffer();    // 送信バッファ
    UdpBuffer _udpReceiver = new UdpBuffer();    // 受信バッファ

    // Start is called before the first frame update
    private void Awake()
    {
        // _mainCamera.enabled = true;
        // _resultCamera.enabled = false;
        // _resultManager = _result.GetComponent<ResultManager>();
        // _playerInputM = _playerInputManager.GetComponent<PlayerInputManager>();
        // _event = GetComponent<Event>();

        // if (_hutons != null)
        // {
        //     foreach (var huton in _hutons)
        //     {
        //         Vector3 hutonPosition = huton.GetComponent<HutonController>().GetCenterPosition();
        //         Quaternion hutonRotation = huton.GetComponent<HutonController>().GetRotation();
        //         _makuras.Add(Instantiate(_makuraPrefub, new Vector3(hutonPosition.x, hutonPosition.y + 0.1f, hutonPosition.z + 0.6f), hutonRotation));
        //     }
        //     _event.Makuras = _makuras;
        // }
        // if (_makuras != null)
        // {
        //     foreach (var makura in _makuras)
        //     {
        //         var makuraController = makura.GetComponent<MakuraController>();
        //         _makuraControllers.Add(makuraController);
        //     }
        // }

        // if (_door != null)
        // {
        //     _doorController = _door.GetComponent<DoorController>();
        // }
        // if (_teacherObj != null)
        // {
        //     _teacher = _teacherObj.GetComponent<Teacher>();
        // }
    }
    private void Start()
    {
        // _doorController.OpenDoors();
        // _teacherComent = _teacherGuide.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        // _teacherComent.text = "就寝時間だぞ";
        // _ready.SetActive(false);
        // _go.SetActive(false);
        // _finish.SetActive(false);

        // 利用可能な機器のリストを取得
        _networkInterfaces = NetworkInterfaceData.GetIpAddress();
        {
            // ドロップダウンリストに機器名を反映
            foreach (var ni in _networkInterfaces)
                _selectNetworkInterfaceDropdown.options.Add(new TMP_Dropdown.OptionData(ni.InterfaceName));

            // 優先するインターフェース名を検索
            int index = _selectNetworkInterfaceDropdown.options.FindIndex((options) => { return options.text == _priorityIntafaceName; });
            // 見つかった場合はそれを選択
            if (index > 0)
            {
                _selectNetworkInterfaceDropdown.value = index;
                _ipAddress = _networkInterfaces[index].IpAddress.ToString();
                _macAddress = _networkInterfaces[index].MacAddress.ToString();
            }
            // 見つからなかった場合は０番を選択
            else if (_selectNetworkInterfaceDropdown.options.Count > 0)
            {
                _selectNetworkInterfaceDropdown.value = 0;
                _ipAddress = _networkInterfaces[0].IpAddress.ToString();
                _macAddress = _networkInterfaces[0].MacAddress.ToString();
            }
            _selectNetworkInterfaceDropdown.RefreshShownValue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if (!_isGameEnd)
        // {
        //     if (_isGameStart)
        //     {
        //         HappeningBallEvnt();
        //         if (_isPlayerSet)
        //         {
        //             StartCoroutine(StartDerey());
        //             _isPlayerSet = false;
        //         }

        //     }
        //     else
        //     {
        //         _players = _playerInputM.Players;
        //         _event.Players = _players;
        //         TextMeshProUGUI text = _guide.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        //         text.text = _startGuide + "( " + _players.Count + " / 4 )";
        //         if (!_isCoroutineSet && _players != null)
        //         {
        //             _isCoroutineSet = true;
        //             StartCoroutine(StartGuideCoroutine());
        //         }

        //         foreach (var player in _players)
        //         {
        //             if (SleepCheck(_players) && _players.Count > 1 && player.GetComponent<PlayerController>().IsGameStartCheck)
        //             // if (SleepCheck(_players) && player.GetComponent<PlayerController>().IsGameStartCheck)//デバッグ用
        //             {
        //                 _isGameStartCheck = true;
        //             }
        //         }

        //         if (SleepCheck(_players) && (_players.Count == 4 || _isGameStartCheck))
        //         {
        //             Init();
        //         }
        //     }
        // }
    }
    /// <summary>
    ///ゲームマネージャー初期化
    /// </summary>
    private void Init()
    {
        // _isGameStart = true;
        // _event.IsGameStart = true;
        // _scoreManager.GetComponent<ScoreManager>().IsGameStart = true;
        // _clock.GetComponent<ClockController>().IsGameStart = true;
        // foreach (var player in _players)
        // {
        //     player.GetComponent<PlayerStatus>().IsGameStart = true;
        //     player.GetComponent<PlayerController>().IsGameStart = true;
        // }
        // foreach (var makura in _makuraControllers)
        // {
        //     makura.IsGameStart = true;
        // }
        // _playerInputManager.SetActive(false);
        // StartCoroutine(GameEnd());
    }
    /// <summary>
    /// 接続されているプレイヤーが全員寝ているかどうかの判定
    /// </summary>
    /// <param name="players">は接続されているプレイヤー</param>
    /// <returns>接続されているプレイヤーが全員寝ていればtrueを返す</returns>
    // private bool SleepCheck(List<GameObject> players)
    // {
    //     foreach (var player in players)
    //     {
    //         if (!player.GetComponent<PlayerController>().IsSleep)
    //         {
    //             return false;
    //         }
    //     }
    //     return true;
    // }
    /// <summary>
    /// ハプニングボールの生成位置をランダムで取得
    /// </summary>
    /// <returns>ハプニングボールの生成位置をランダムで返す</returns>
    // private Vector3 RandomPosition()
    // {
    // float xMin = -8.0f;
    // float xMax = 8.0f;
    // float zMin = -3.0f;
    // float zMax = 7.0f;
    // float y = 6.0f;

    // float randomX = Random.Range(xMin, xMax);
    // float randomZ = Random.Range(zMin, zMax);

    // return new Vector3(randomX, y, randomZ);
    // }
    /// <summary>
    /// ハプニングボールが割れたときにランダムでイベントを発生させる
    /// </summary>
    // private void HappeningBallEvnt()
    // {
    //     if (_happeningBalls != null)
    //     {
    //         foreach (var happeningBall in _happeningBalls)
    //         {
    //             if (happeningBall.Outbreak)
    //             {
    //                 _event.RandomEvent(happeningBall.Starter);
    //                 happeningBall.Outbreak = false;
    //             }
    //         }
    //         if (_happeningBalls.Count > 10)
    //         {
    //             HappeningBall happeningBall = _happeningBalls[0];
    //             _happeningBalls.RemoveAt(0);
    //             if (happeningBall != null)
    //             {
    //                 Destroy(happeningBall.gameObject);
    //             }
    //         }
    //     }
    // }
    /// <summary>
    /// ハプニングボールを10秒に一回ランダムで生成
    /// </summary>
    /// <returns></returns>
    // private IEnumerator HappeningBallGeneration()
    // {
    //     GameObject happeningBall = Instantiate(_happeningBall, RandomPosition(), Quaternion.identity);
    //     _happeningBalls.Add(happeningBall.GetComponent<HappeningBall>());
    //     yield return new WaitForSeconds(10.0f);
    //     if (_isGameStart)
    //     {
    //         StartCoroutine(HappeningBallGeneration());
    //     }
    // }
    /// <summary>
    /// 先生イベントを60秒に一回発生
    /// </summary>
    /// <returns></returns>
    // private IEnumerator TeacherEvent()
    // {
    //     yield return new WaitForSeconds(_teacherEventTime);
    //     if (_isGameStart)
    //     {
    //         _event.TeacherEvent.Init(_playerControllers);
    //         StartCoroutine(TeacherEvent());
    //     }
    // }
    /// <summary>
    /// ゲーム終了時の処理
    /// </summary>
    /// <returns></returns>
    // private IEnumerator GameEnd()
    // {
    //     yield return new WaitForSeconds(_gameTime);
    //     _isGameStart = false;
    //     _isGameStartCheck = false;
    //     _event.IsGameStart = false;
    //     _scoreManager.GetComponent<ScoreManager>().IsGameStart = false;
    //     _clock.GetComponent<ClockController>().IsGameStart = false;
    //     foreach (var player in _players)
    //     {
    //         player.GetComponent<PlayerStatus>().IsGameStart = false;
    //     }
    //     for (int i = 0; i < _playerControllers.Count; i++)
    //     {
    //         var playerController = _playerControllers[i];
    //         playerController.IsGameStart = false;
    //         playerController.IsGameEnd = true;
    //         playerController.CurrentMakuraDisplays[0].SetActive(false);
    //         playerController.CurrentMakuraDisplays[1].SetActive(false);
    //         playerController.SpGageInstance.SetActive(false);
    //         playerController.PlayerTagUIInstance.SetActive(false);
    //         playerController.ResultSleep(_resultManager.ResultHutonControllers[i]);
    //     }
    //     for (int i = _happeningBalls.Count - 1; i >= 0; i--)
    //     {
    //         var happeningBall = _happeningBalls[i];

    //         if (happeningBall != null)
    //         {
    //             Destroy(happeningBall.gameObject);
    //             yield return null;
    //             _happeningBalls.RemoveAt(i);
    //         }
    //     }
    //     yield return new WaitForSeconds(0.5f);
    //     _finish.SetActive(true);
    //     yield return new WaitForSeconds(5.0f);
    //     _mainCamera.enabled = false;
    //     _resultCamera.enabled = true;
    //     _finish.SetActive(false);

    //     int scoretmp = -1;
    //     int rank = -1;
    //     int rankSkip = 0;
    //     foreach (var score in SortScores(_scoreManager.GetComponent<ScoreManager>().ScoreNum))
    //     {
    //         if (scoretmp != score.Value)
    //         {
    //             scoretmp = score.Value;
    //             rank += 1 + rankSkip;
    //             rankSkip = 0;
    //         }
    //         else
    //         {
    //             rankSkip++;
    //         }
    //         _resultManager.ResultHutonControllers[score.Key].Rank = rank;
    //     }
    //     _resultManager.ScoreDic = _scoreManager.GetComponent<ScoreManager>().ScoreNum;
    //     _resultManager.IsGameEnd = true;
    //     _resultManager.PlayerControllers = _playerControllers;
    // }
    /// <summary>
    /// ゲーム開始時の処理
    /// </summary>
    /// <returns></returns>
    // private IEnumerator StartDerey()
    // {
    //     _guide.SetActive(false);
    //     _teacherComent.text = "よし。";
    //     yield return new WaitForSeconds(2.0f);
    //     _teacherGuide.SetActive(false);
    //     _teacher.IsGameStart = true;
    //     yield return new WaitForSeconds(1.0f);
    //     _ready.SetActive(true);
    //     yield return new WaitForSeconds(1.0f);
    //     _doorController.IsGameStart = true;
    //     yield return new WaitForSeconds(2.0f);
    //     _ready.SetActive(false);

    //     _go.SetActive(true);

    //     for (int i = 0; i < _players.Count; i++)
    //     {
    //         var playerController = _players[i].GetComponent<PlayerController>();
    //         playerController.WakeUp();
    //         _playerControllers.Add(playerController);
    //     }
    //     StartCoroutine(TeacherEvent());
    //     if (_happeningBall != null)
    //     {
    //         StartCoroutine(HappeningBallGeneration());
    //     }
    //     yield return new WaitForSeconds(1.0f);

    //     _go.SetActive(false);
    // }
    /// <summary>
    /// ロビーでのガイド
    /// </summary>
    /// <returns></returns>
    // private IEnumerator StartGuideCoroutine()
    // {
    //     if (_players.Count == 4)
    //     {
    //         _isGuideKind = false;
    //     }
    //     if (_isGuideKind)
    //     {
    //         _isGuideKind = false;
    //         _startGuide = "コントローラーの接続を待っています... ";
    //     }
    //     else
    //     {
    //         _isGuideKind = true;
    //         _startGuide = "始めるには、全員が寝た状態で ZR + ZL 同時押し... ";
    //     }
    //     yield return new WaitForSeconds(8.0f);
    //     StartCoroutine(StartGuideCoroutine());
    // }
    /// <summary>
    /// スコアを高い順に並び変える
    /// </summary>
    /// <param name="scoreDic"></param>
    /// <returns></returns>
    // public static List<KeyValuePair<int, int>> SortScores(Dictionary<int, int> scoreDic)
    // {
    //     return scoreDic.OrderByDescending(entry => entry.Value).ToList();
    // }
}
