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
    // 優先するインターフェースを文字列で指定（なければ空白）
    [SerializeField] private string _priorityIntafaceName;
    // ドロップダウン
    [SerializeField] private TMP_Dropdown _selectNetworkInterfaceDropdown;
    // 利用可能な機器のリスト
    private List<NetworkInterfaceData> _networkInterfaces;

    public GameObject _playerPrefab;
    public GameObject _makuraPrefab;

    private List<PlayerBase> _players = new List<PlayerBase>();
    private List<Makura> _makuras = new List<Makura>();

    private PlayerInput _playerInput;
    private Player _offlinePlayer;
    // UDP通信のためのクラス
    private UdpClient _udpClient;
    // ServerのIpAddressを指定する（localhostは自分自身）
    [SerializeField] private string host = "localhost";
    // 使用するPort(Server)
    private int _portServer = 53724;
    // 使用するPort(自分:Client)
    private int _portClient = 53725;
    // 送信バッファ
    UdpBuffer _udpTransmitter = new UdpBuffer();
    // 受信バッファ
    UdpBuffer _udpReceiver = new UdpBuffer();
    // 受信したユーザーID
    private byte _userIdWork = Byte.MaxValue;
    // ユーザーID
    private byte _userId = Byte.MaxValue;
    // タイマー（サーバー側でパケットロスの判定などに使用する）
    private byte _globalTimer = 0;
    // 送信パケット
    private Packet _paket = new Packet();

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
        _playerInput = GetComponent<PlayerInput>();
        _offlinePlayer = new Player(_playerPrefab, this.transform);

        // UDP送受信のためのクラスを作成する
        _udpClient = new UdpClient(_portClient);
        {
            // 1byteのダミーデータを送信してサーバーにIDを振ってもらう
            byte[] message = { 0 };
            _udpClient.Send(message, message.Length, host, _portServer);
        }

        // 待ち受け開始（受信があったときOnReceivedが呼ばれる）
        _udpClient.BeginReceive(OnReceived, _udpClient);
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

        // 受信したユーザーIDを採用する
        if (_userId != _userIdWork)
        {
            _userId = _userIdWork;
        }

        lock (_udpReceiver.LockObject)
        {
            // 受信バッファにデータがある
            if (_udpReceiver.Buffer != null)
            {
                // プレイヤー数
                int playerNum = _udpReceiver.Buffer[0];
                // マクラ数
                int makuraNum = _udpReceiver.Buffer[1];

                int offset = 2;

                // プレイヤーが足りない場合は補充する
                for (int i = _players.Count; i < playerNum; i++)
                    _players.Add(new NetPlayer(_playerPrefab, this.transform));

                // プレイヤー情報を読み込む
                for (int i = 0; i < playerNum; i++)
                    offset = _players[i].ReadByte(_udpReceiver.Buffer, offset);

                // プレイヤーリストが多いときは削除
                if (playerNum < _players.Count)
                {
                    for (int i = playerNum; i < _players.Count; i++)
                    {
                        GameObject.Destroy(_players[i].Obj);
                        _players[i].Obj = null;
                    }
                    _players.RemoveRange(playerNum, _players.Count - playerNum);
                }

                // マクラが足りない場合は補充する
                for (int i = _makuras.Count; i < makuraNum; i++)
                    _makuras.Add(new Makura(_makuraPrefab, true));

                // マクラ情報を読み込む
                for (int i = 0; i < makuraNum; i++)
                    offset = _makuras[i].ReadByte(_udpReceiver.Buffer, offset);

                // マクラリストが多いときは削除
                if (makuraNum < _makuras.Count)
                {
                    for (int i = makuraNum; i < _makuras.Count; i++)
                    {
                        GameObject.Destroy(_makuras[i].Obj);
                        _makuras[i].Obj = null;
                    }
                    _makuras.RemoveRange(makuraNum, _makuras.Count - makuraNum);
                }

                // 受信バッファを解放
                _udpReceiver.Buffer = null;
            }
        }

        bool isNetwork = _userId != byte.MaxValue;

        // ネットワーク接続中はリストから自分のIDを探し、それをプレイヤーとして使う
        PlayerBase player = !isNetwork ? null : _players.Find(x => x.Id == _userId);
        // IDが見つからなかった場合やオフラインのときは_offlinePlayerをプレイヤーとして使う
        if (player == null)
        {
            player = _offlinePlayer;
            isNetwork = false;
        }
        // ネットワーク接続の有無で_offlinePlayerを無効（有効）にする
        _offlinePlayer.SetActive(!isNetwork);

        // Playerの更新
        player.Update(_playerInput);

        // タイマー、入力、フォースをパケットに詰む
        _paket.Push(_globalTimer, player.InputMask, player.Movement);
        // if ((player.InputMask & PacketData.eInputMask.PickUp) != 0)
        // {
        //     Debug.Log("拾い");
        // }

        // IDが振られていたらサーバーにデータを送信
        if (_userId != Byte.MaxValue)
        {
            List<byte> buffer = new List<byte>();

            // ID
            buffer.Add(_userId);
            // パケット
            buffer.AddRange(_paket.GetBytes());

            // 送信
            _udpClient.Send(buffer.ToArray(), buffer.Count, host, _portServer);
        }

        // {
        //     // カメラの注視点にプレイヤー位置をコピー
        //     _cameraTarget.transform.position = player.Obj.transform.position;
        //     // プレイヤー切り替えやリセットが発生していたら
        //     if (player.IsChangeCharacter | player.IsReset)
        //     {
        //         Quaternion q = player.Obj.transform.rotation;
        //         Vector3 startPos = player.Obj.transform.position + q * new Vector3(0, 3, -5);
        //         // カメラを即時プレイヤー後方に切り替え
        //         _cinemachineVirtualCamera1.ForceCameraPosition(startPos, q);
        //         // 補間を無効にする
        //         _cinemachineVirtualCamera1.PreviousStateIsValid = false;
        //     }
        //     else
        //     {
        //         // 補間を有効にする
        //         _cinemachineVirtualCamera1.PreviousStateIsValid = true;
        //     }
        // }

        _globalTimer++;
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

    private void OnReceived(System.IAsyncResult result)
    {
        UdpClient getUdp = (UdpClient)result.AsyncState;
        IPEndPoint ipEnd = null;

        if (getUdp.Client == null)
        {
            return;
        }

        // ロックしてから受信したByte配列を読み取る
        lock (_udpReceiver.LockObject)
        {
            try
            {
                _udpReceiver.Buffer = getUdp.EndReceive(result, ref ipEnd);

                // 1byteのときはIDを取り出して破棄
                if (_udpReceiver.Buffer.Length <= 1)
                {
                    _userIdWork = _udpReceiver.Buffer[0];
                    _udpReceiver.Buffer = null;
                }
            }
            catch
            {
                _udpReceiver.Buffer = null;
            }
        }

        getUdp.BeginReceive(OnReceived, getUdp);
    }
    private void OnApplicationQuit()
    {
        Dispose();
    }
    private void Dispose()
    {
        if (_udpClient != null)
        {
            _udpClient.Close();
            _udpClient.Dispose();
            _udpClient = null;
        }
    }
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
