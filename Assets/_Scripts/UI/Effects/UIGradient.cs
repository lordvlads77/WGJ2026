using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Graphic))]
public sealed class UIGradient : BaseMeshEffect, IMaterialModifier
{
    public enum GradientType { Linear = 0, Radial = 1, Angular = 2 }
    public enum GradientWrap { Clamp = 0, Repeat = 1, PingPong = 2 }

    private static readonly int GradientTexId = Shader.PropertyToID("_GradientTex");
    private static readonly int TypeId = Shader.PropertyToID("_GradientType");
    private static readonly int AngleId = Shader.PropertyToID("_GradientAngle");
    private static readonly int CenterId = Shader.PropertyToID("_GradientCenter");
    private static readonly int ScaleId = Shader.PropertyToID("_GradientScale");
    private static readonly int OffsetId = Shader.PropertyToID("_GradientOffset");
    private static readonly int WrapId = Shader.PropertyToID("_GradientWrap");
    private static readonly int KeepAspectId = Shader.PropertyToID("_KeepAspect");
    private static readonly int SpriteBlendId = Shader.PropertyToID("_SpriteColorBlend");

    [SerializeField]
    private Gradient _gradient = new Gradient
    {
        colorKeys = new[]
        {
            new GradientColorKey(new Color(0.1f, 0.8f, 1f), 0f),
            new GradientColorKey(new Color(1f, 0.2f, 0.7f), 1f)
        },
        alphaKeys = new[]
        {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(1f, 1f)
        }
    };

    [SerializeField] private GradientType _type = GradientType.Linear;
    [SerializeField, Range(0f, 360f)] private float _angle;
    [SerializeField] private Vector2 _center = new Vector2(0.5f, 0.5f);
    [SerializeField, Min(0.01f)] private float _scale = 1f;
    [SerializeField] private float _offset;
    [SerializeField] private GradientWrap _wrap = GradientWrap.Clamp;
    [Tooltip("Mantiene círculos redondos y ángulos correctos en rects no cuadrados.")]
    [SerializeField] private bool _keepAspect = true;
    [Tooltip("0 = solo usa el alpha del sprite. 1 = multiplica también su color.")]
    [SerializeField, Range(0f, 1f)] private float _spriteColorBlend;
    [SerializeField, Range(16, 1024)] private int _resolution = 256;

    private Texture2D _ramp;
    private Color32[] _pixels;
    private Material _material;

    public Gradient Gradient
    {
        get => _gradient;
        set { _gradient = value; Refresh(); }
    }

    public float Angle
    {
        get => _angle;
        set { _angle = value; SetMaterialDirty(); }
    }

    public float Offset
    {
        get => _offset;
        set { _offset = value; SetMaterialDirty(); }
    }

    public float Scale
    {
        get => _scale;
        set { _scale = Mathf.Max(0.01f, value); SetMaterialDirty(); }
    }

    public Vector2 Center
    {
        get => _center;
        set { _center = value; SetMaterialDirty(); }
    }

    public void Refresh()
    {
        BakeRamp();
        SetMaterialDirty();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EnsureCanvasChannel();
        SetMaterialDirty();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SetMaterialDirty();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SafeDestroy(_material);
        SafeDestroy(_ramp);
    }

    protected override void OnCanvasHierarchyChanged()
    {
        base.OnCanvasHierarchyChanged();
        EnsureCanvasChannel();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        Refresh();
    }
#endif

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || vh.currentVertCount == 0) return;

        Rect rect = graphic.rectTransform.rect;
        if (rect.width <= 0f || rect.height <= 0f) return;

        float aspect = rect.width / rect.height;
        UIVertex vertex = default;

        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            vertex.uv1 = new Vector4(
                (vertex.position.x - rect.xMin) / rect.width,
                (vertex.position.y - rect.yMin) / rect.height,
                aspect,
                0f);
            vh.SetUIVertex(vertex, i);
        }
    }

    public Material GetModifiedMaterial(Material baseMaterial)
    {
        if (!IsActive() || baseMaterial == null) return baseMaterial;

        if (_ramp == null) BakeRamp();

        if (_material == null)
        {
            _material = new Material(baseMaterial) { hideFlags = HideFlags.HideAndDontSave };
        }
        else
        {
            if (_material.shader != baseMaterial.shader) _material.shader = baseMaterial.shader;
            _material.CopyPropertiesFromMaterial(baseMaterial); // conserva stencil de Mask
        }

        _material.SetTexture(GradientTexId, _ramp);
        _material.SetFloat(TypeId, (float)_type);
        _material.SetFloat(AngleId, _angle);
        _material.SetVector(CenterId, _center);
        _material.SetFloat(ScaleId, _scale);
        _material.SetFloat(OffsetId, _offset);
        _material.SetFloat(WrapId, (float)_wrap);
        _material.SetFloat(KeepAspectId, _keepAspect ? 1f : 0f);
        _material.SetFloat(SpriteBlendId, _spriteColorBlend);

        return _material;
    }

    private void BakeRamp()
    {
        if (_ramp == null || _ramp.width != _resolution)
        {
            SafeDestroy(_ramp);
            _ramp = new Texture2D(_resolution, 1, TextureFormat.RGBA32, false)
            {
                name = "UIGradientRamp",
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        _ramp.filterMode = _gradient.mode == GradientMode.Fixed ? FilterMode.Point : FilterMode.Bilinear;

        if (_pixels == null || _pixels.Length != _resolution) _pixels = new Color32[_resolution];

        for (int i = 0; i < _resolution; i++)
        {
            _pixels[i] = _gradient.Evaluate(i / (float)(_resolution - 1));
        }

        _ramp.SetPixels32(_pixels);
        _ramp.Apply(false);
    }

    private void EnsureCanvasChannel()
    {
        var canvas = graphic != null ? graphic.canvas : null;
        
        if (canvas == null) return;

        canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
        if (canvas.rootCanvas != canvas)
        {
            canvas.rootCanvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
        }
    }

    private void SetMaterialDirty()
    {
        if (graphic != null) graphic.SetMaterialDirty();
    }

    private static void SafeDestroy(Object obj)
    {
        if (obj == null) return;
        if (Application.isPlaying) Destroy(obj);
        else DestroyImmediate(obj);
    }
}