using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConfigUIButton : MonoBehaviour
{
    [SerializeField] private List<InputActionReference> _actionRefs;
    private string _scheme = "Gamepad";
    [SerializeField] private List<TextMeshProUGUI> _pathtexts;
    [SerializeField] private GameObject _mask;

    private List<InputAction> _actions;
    private InputActionRebindingExtensions.RebindingOperation _rebindOperation;

    private int _slot;
    private SaveData _saveData;
    private ConfigSaveManager _manager;

    [SerializeField] private GameObject _loadmenu;
    [SerializeField] private GameObject _keymenu;
    [SerializeField] private GameObject _configmask;
    [SerializeField] private GameObject _returnbutton;

    private void Awake()
    {
        _actions = new List<InputAction>(_actionRefs.Count);
        _manager = FindObjectOfType<ConfigSaveManager>();
        // 初期化時にリストを埋める
        for (int i = 0; i < _actionRefs.Count; i++)
        {
            _actions.Add(null); // 空の要素を追加
        }

        for (int i = 0; i < _actionRefs.Count; i++)
        {
            if (_actionRefs[i] != null)
            {
                _actions[i] = _actionRefs[i].action; // 正しい代入
                RefreshDisplay(i);
            }
        }
    }

    public void LoadConfig(int i)
    {
        if (_saveData != null)
        {
            _slot = i;
            _saveData = _manager.LoadSaveData(i);

            _actions[0].RemoveAllBindingOverrides();
            _actions[0].ApplyBindingOverride(_saveData.Throw);

            _actions[1].RemoveAllBindingOverrides();
            _actions[1].ApplyBindingOverride(_saveData.Catch);

            _actions[2].RemoveAllBindingOverrides();
            _actions[2].ApplyBindingOverride(_saveData.SpecialAttack);

            _actions[3].RemoveAllBindingOverrides();
            _actions[3].ApplyBindingOverride(_saveData.Jump);

            _actions[4].RemoveAllBindingOverrides();
            _actions[4].ApplyBindingOverride(_saveData.Sleep);

            for (int j = 0; j < _actionRefs.Count; j++)
            {
                RefreshDisplay(j);
            }
        }

        _loadmenu.SetActive(false);
        _keymenu.SetActive(true);
        _configmask.SetActive(true);
        _returnbutton.SetActive(false);
    }

    private void OnDestroy()
    {
        CleanUpOperation();
    }

    public void StartRebinding(int i)
    {
        if (_actions[i] == null)
        {
            return;
        }

        _rebindOperation?.Cancel();
        _actions[i].Disable();

        var bindingIndex = _actions[i].GetBindingIndex(InputBinding.MaskByGroup(_scheme));

        if (bindingIndex == -1)
        {
            Debug.LogWarning($"No binding found for scheme {_scheme} in action {_actions[i].name}");
            _actions[i].Enable();
            return;
        }

        if (_mask != null)
        {
            _mask.SetActive(true);
        }

        void OnFinished()
        {
            CleanUpOperation();
            _actions[i].Enable();

            if (_mask != null)
            {
                _mask.SetActive(false);
            }
        }

        _rebindOperation = _actions[i].PerformInteractiveRebinding(bindingIndex)
            .OnComplete(_ =>
            {
                RefreshDisplay(i);
                OnFinished();
            })
            .OnCancel(_ =>
            {
                OnFinished();
            })
            .Start();
    }

    public void ResetOverrides(int i)
    {
        _actions[i]?.RemoveAllBindingOverrides();
        RefreshDisplay(i);
    }

    public void RefreshDisplay(int i)
    {
        if (_actions[i] == null || i >= _pathtexts.Count)
        {
            return;
        }

        _pathtexts[i].text = _actions[i].GetBindingDisplayString(InputBinding.MaskByGroup(_scheme), InputBinding.DisplayStringOptions.DontIncludeInteractions);
    }

    private void CleanUpOperation()
    {
        _rebindOperation?.Dispose();
        _rebindOperation = null;
    }

    public void InitSlot(int i)
    {
        _slot = i;
    }

    public void KeyConfigSave()
    {
        if (_actions == null || _actions.Count < 5 || _actions.Any(action => action == null))
        {
            return;
        }
        _saveData = new SaveData();
        _saveData.Controllertype = _scheme;
        _saveData.Name = "player";
        _saveData.Throw = _actions[0].bindings[0].effectivePath; // 0番目のバインディングのパスを取得
        _saveData.Catch = _actions[1].bindings[0].effectivePath;
        _saveData.SpecialAttack = _actions[2].bindings[0].effectivePath;
        _saveData.Jump = _actions[3].bindings[0].effectivePath;
        _saveData.Sleep = _actions[4].bindings[0].effectivePath;
        _manager.KeyConfigSave(_saveData, _slot);

        _loadmenu.SetActive(true);
        _keymenu.SetActive(false);
        _configmask.SetActive(false);
        _returnbutton.SetActive(true);
    }
}
