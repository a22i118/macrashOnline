using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCS;

public class Event : MonoBehaviour
{
    [SerializeField] private List<GameObject> _hutons;
    [SerializeField] private GameObject _door;
    [SerializeField] private GameObject _teacher;
    [SerializeField] private GameObject _meteor;
    [SerializeField] private GameObject _tatami;
    [SerializeField] private GameObject _makuraPrefub;
    private bool _isPlayerSet = true;
    private bool _isGameStart = false;
    private bool _one = false;
    private int _lastEvent = -1;
    private int _lastEventColorChangeMakura = -1;
    private TeacherShadowController _teacherEvent;
    private MeteorEvent _meteorEvent;
    private TatamiEvent _tatamiEvent;
    private List<GameObject> _players;
    private List<GameObject> _makuras = new List<GameObject>();
    private List<MakuraController> _makuraControllers = new List<MakuraController>();
    private List<PlayerController> _playerControllers = new List<PlayerController>();
    public bool IsGameStart { get => _isGameStart; set => _isGameStart = value; }
    public List<GameObject> Makuras { get => _makuras; set => _makuras = value; }
    public List<GameObject> Players { get => _players; set => _players = value; }
    public TeacherShadowController TeacherEvent { get => _teacherEvent; set => _teacherEvent = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (_makuras != null)
        {
            foreach (var makura in _makuras)
            {
                var makuraController = makura.GetComponent<MakuraController>();
                _makuraControllers.Add(makuraController);
            }
        }

        if (_teacher != null)
        {
            _teacherEvent = _teacher.GetComponent<TeacherShadowController>();
        }
        if (_meteor != null)
        {
            _meteorEvent = _meteor.GetComponent<MeteorEvent>();
        }
        if (_tatami != null)
        {
            _tatamiEvent = _tatami.GetComponent<TatamiEvent>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isGameStart)
        {
            if (_isPlayerSet)
            {
                for (int i = 0; i < _players.Count; i++)
                {
                    var playerController = _players[i].GetComponent<PlayerController>();
                    _playerControllers.Add(playerController);
                    _isPlayerSet = false;
                }
            }
        }
        //デバッグ用
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (var player in _playerControllers)
            {
                player.IsCanSleep = true;
            }
            _teacherEvent.Init(_playerControllers);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            _tatamiEvent.Init();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            _meteorEvent.Init();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (var makura in _makuraControllers)
            {
                makura.CurrentColorType = ColorChanger.ColorType.Red;
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            foreach (var makura in _makuraControllers)
            {
                makura.CurrentColorType = ColorChanger.ColorType.Green;
            }
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            foreach (var makura in _makuraControllers)
            {
                makura.CurrentColorType = ColorChanger.ColorType.Blue;
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (var makura in _makuraControllers)
            {
                makura.CurrentColorType = ColorChanger.ColorType.Black;
            }
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            foreach (var makura in _makuraControllers)
            {
                makura.CurrentColorType = ColorChanger.ColorType.Nomal;
            }
        }
    }
    /// <summary>
    /// ランダムでイベントを発生
    /// </summary>
    /// <param name="starter">が発動した人</param>
    public void RandomEvent(GameObject starter)
    {
        if (!_one)
        {
            _one = true;

            int randomNumber = Random.Range(0, 5);
            while (randomNumber == _lastEvent)
            {
                randomNumber = Random.Range(0, 5);
            }
            switch (randomNumber)
            {
                case 0:
                    starter.GetComponent<PlayerController>().StartCoroutine(starter.GetComponent<PlayerController>().SpeedUpCoroutine());
                    break;
                case 1:
                    _tatamiEvent.Init();
                    break;
                case 2:
                    _meteorEvent.Init();
                    break;
                case 3:
                    RandomColorChangeMakura();
                    break;
                case 4:
                    MakuraController makuraController = Instantiate(_makuraPrefub, RandomPosition(), Quaternion.identity).GetComponent<MakuraController>();
                    makuraController.IsGameStart = true;
                    break;
            }
            _one = false;
            _lastEvent = randomNumber;
        }
    }
    private void RandomColorChangeMakura()
    {
        int randomNumberMakura = Random.Range(0, 5);
        while (randomNumberMakura == _lastEventColorChangeMakura)
        {
            randomNumberMakura = Random.Range(0, 5);
        }
        switch (randomNumberMakura)
        {
            case 0:
                foreach (var makura in _makuraControllers)
                {
                    if (!makura.IsThrow)
                    {
                        makura.CurrentColorType = ColorChanger.ColorType.Red;
                    }
                }
                break;
            case 1:
                foreach (var makura in _makuraControllers)
                {
                    if (!makura.IsThrow)
                    {
                        makura.CurrentColorType = ColorChanger.ColorType.Green;
                    }
                }
                break;
            case 2:
                foreach (var makura in _makuraControllers)
                {
                    if (!makura.IsThrow)
                    {
                        makura.CurrentColorType = ColorChanger.ColorType.Blue;
                    }
                }
                break;
            case 3:
                foreach (var makura in _makuraControllers)
                {
                    if (!makura.IsThrow)
                    {
                        makura.CurrentColorType = ColorChanger.ColorType.Black;
                    }
                }
                break;
            case 4:
                foreach (var makura in _makuraControllers)
                {
                    if (!makura.IsThrow)
                    {
                        makura.CurrentColorType = ColorChanger.ColorType.Nomal;
                    }
                }
                break;
        }
    }
    private Vector3 RandomPosition()
    {
        float xMin = -8.0f;
        float xMax = 8.0f;
        float zMin = -3.0f;
        float zMax = 7.0f;
        float y = 6.0f;

        float randomX = Random.Range(xMin, xMax);
        float randomZ = Random.Range(zMin, zMax);

        return new Vector3(randomX, y, randomZ);
    }
}
