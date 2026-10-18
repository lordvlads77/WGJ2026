using System;
using UnityEngine;
using UnityEngine.Rendering;

public class ColorFilterManager : Singleton<ISettingsSource>, ISettingsSource
{
    private const ColorFilter DEFAULT_COLORBLIND_MODE = ColorFilter.None;
    private const string COLORBLIND_MODE_KEY = "ColorblindMode";

    [SerializeField] private Material _colorblindMaterial;

    public ColorFilter CurrentColorblindMode { get; private set; }

    public event Action<ColorFilter> OnModeChanged;

    private void Start()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        var savedMode = PlayerPrefs.GetInt(COLORBLIND_MODE_KEY, (int)DEFAULT_COLORBLIND_MODE);
        CurrentColorblindMode = (ColorFilter)savedMode;

        ApplyColorblindMode(CurrentColorblindMode);
    }

    public void SetColorblindMode(ColorFilter mode)
    {
        CurrentColorblindMode = mode;

        ApplyColorblindMode(mode);

        PlayerPrefs.SetInt(COLORBLIND_MODE_KEY, (int)mode);
        PlayerPrefs.Save();

        OnModeChanged?.Invoke(mode);
    }

    private void ApplyColorblindMode(ColorFilter mode)
    {
        var shader = _colorblindMaterial.shader;
        var none = new LocalKeyword(shader, "_COLORBLIND_MODE_NONE");
        var tritanopia = new LocalKeyword(shader, "_COLORBLIND_MODE_TRITANOPIA");
        var protonopia = new LocalKeyword(shader, "_COLORBLIND_MODE_PROTONOPIA");
        var deuteranopia = new LocalKeyword(shader, "_COLORBLIND_MODE_DEUTERANOPIA");

        _colorblindMaterial.SetKeyword(none, false);
        _colorblindMaterial.SetKeyword(tritanopia, false);
        _colorblindMaterial.SetKeyword(protonopia, false);
        _colorblindMaterial.SetKeyword(deuteranopia, false);

        switch (mode)
        {
            case ColorFilter.None:
                _colorblindMaterial.SetKeyword(none, true);
                break;

            case ColorFilter.Tritanopia:
                _colorblindMaterial.SetKeyword(tritanopia, true);
                break;

            case ColorFilter.Protonopia:
                _colorblindMaterial.SetKeyword(protonopia, true);
                break;

            case ColorFilter.Deuteranopia:
                _colorblindMaterial.SetKeyword(deuteranopia, true);
                break;
        }
    }
}

public interface ISettingsSource
{
    event Action<ColorFilter> OnModeChanged;

    ColorFilter CurrentColorblindMode { get; }
    void SetColorblindMode(ColorFilter mode);
}
