using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GhostEnemy : MonoBehaviour
{
    private static readonly int AlphaId = Shader.PropertyToID("_Alpha");
    private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");
    private static readonly int EmissionIntensityId = Shader.PropertyToID("_EmissionIntensity");

    [SerializeField] private float _MaxEmission;
    [SerializeField] private float _MaxAlpha;
    [SerializeField] private Color _CustomColor;
    [SerializeField, Range(0f, 1f)] private float _lethalThreshold = 0.9f;

    private Material _mat;
    private float _currentEmission;
    private float _currentAlpha;
    private ResetPlayer _playerInContact;

    private bool IsLethal => _MaxEmission > 0f && _currentEmission >= _MaxEmission * _lethalThreshold;

    private void Awake()
    {
        _mat = GetComponentInChildren<Renderer>().material;
    }

    private void Start()
    {
        _mat.SetFloat(AlphaId, _MaxAlpha);
        _mat.SetColor(EmissionColorId, _CustomColor);
        _mat.SetFloat(EmissionIntensityId, _MaxEmission);
    }

    private void OnDestroy()
    {
        if (_mat != null)
        {
            Destroy(_mat);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.TryGetComponent(out ResetPlayer resetPlayer))
        {
            _playerInContact = resetPlayer;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && other.TryGetComponent(out ResetPlayer resetPlayer))
        {
            _playerInContact = resetPlayer;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (_playerInContact != null && collision.gameObject == _playerInContact.gameObject)
        {
            _playerInContact = null;
        }
    }

    private void Update()
    {
        float t = Mathf.PingPong(Time.time / 2f, 1f);
        t = t * t * t;

        _currentAlpha = t * _MaxAlpha;
        _currentEmission = t * _MaxEmission;

        _mat.SetFloat(AlphaId, _currentAlpha);
        _mat.SetFloat(EmissionIntensityId, _currentEmission);

        if (_playerInContact != null && IsLethal)
        {
            _playerInContact.PlayerReset();
            _playerInContact = null;
        }
    }
}
