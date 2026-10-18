using TMPro;
using UnityEngine;

public class ColorblindDropdown : MonoBehaviour
{
    private TMP_Dropdown _dropdown;

    private void Awake()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        LoadCurrentValue();
        _dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnDestroy()
    {
        _dropdown.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void LoadCurrentValue()
    {
        _dropdown.SetValueWithoutNotify((int)ColorFilterManager.Source.CurrentColorblindMode);
    }

    private void OnValueChanged(int index)
    {
        var mode = (ColorFilter)index;
        ColorFilterManager.Source.SetColorblindMode(mode);
    }
}
