using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GhostEnemy : MonoBehaviour
{
    Material mat;
    [SerializeField]private float _MaxEmission;
    [SerializeField]private float _MaxAlpha;
    [SerializeField]private Color _CustomColor;

    private float _currentEmission;
    private float _currentAlpha;

    private void Awake()
    {
        mat = GetComponentInChildren<Renderer>().material;

    }

    void Start()
    {
        mat.SetFloat("_Alpha", _MaxAlpha);
        mat.SetColor("_EmissionColor", _CustomColor);
        mat.SetFloat("_EmissionIntensity", _MaxEmission);
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.TryGetComponent(out ResetPlayer _resetPlayer);
            _resetPlayer.PlayerReset();
        }
    }

    void Update()
    {
        float t = Mathf.PingPong(Time.time / 2f, 1f);
        t = t * t * t;
        mat.SetFloat("_Alpha", t * _MaxAlpha);
        mat.SetFloat("_EmissionIntensity", t * _MaxEmission);
    }

    /*private IEnumerator GhostRoutine()
    {
        while (true)
        {
            mat.SetFloat("_Alpha", Mathf.Lerp(_MaxAlpha, 0, 0.25f * Time.deltaTime));
            mat.SetFloat("_EmissionIntensity", Mathf.Lerp(_MaxEmission, 0, 0.25f * Time.deltaTime));
     
            yield return new WaitForSeconds(2f);

            mat.SetFloat("_Alpha", Mathf.Lerp(0, _MaxAlpha, 0.25f * Time.deltaTime));
            mat.SetFloat("_EmissionIntensity", Mathf.Lerp(0, _MaxEmission, 0.25f * Time.deltaTime));

            yield return new WaitForSeconds(2f);
            yield return null;
        }
   } */
}
