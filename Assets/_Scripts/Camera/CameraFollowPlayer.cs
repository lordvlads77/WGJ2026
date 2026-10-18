using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Vector3 _followOffset;

    private Vector3 _rightOffset;
    private Vector3 _leftOffset;
    private Transform _cameraTransform;
    private Vector2 _lookDirection;

    void Start()
    {
        _rightOffset = _followOffset;
        _leftOffset = new Vector3(-_followOffset.x, _followOffset.y, _followOffset.z);
        PlayerEvents.OnMove += OnPlayermove;
        _cameraTransform = GetComponent<Transform>();
    }

    void OnDestroy()
    {
        PlayerEvents.OnMove -= OnPlayermove;
    }

    void LateUpdate()
    {
        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position,_playerTransform.position + _followOffset,0.75f * Time.deltaTime);
    }

    public void SnapToTarget()
    {
        transform.position = _playerTransform.position + _followOffset;
    }

    void OnPlayermove(bool isMoving,Vector2 direction)
    {
        _lookDirection = direction;
        if(_lookDirection.x < 0)
        {
            _followOffset = _leftOffset;
        }
        else if(_lookDirection.x > 0)
        {
            _followOffset = _rightOffset;
        }
    }
}