using UnityEngine;

[CreateAssetMenu(fileName = "PlayerVariables", menuName = "Scriptable Objects/PlayerVariables")]
public class PlayerVariables : ScriptableObject
{
    [SerializeField]private float _baseMoveSpeed;
    [SerializeField] private float _sprintMultiplier;
    [SerializeField] private float _jumpForce;
    
    public float BaseMoveSpeed => _baseMoveSpeed;
    public float SprintMultiplier => _sprintMultiplier;
    public float JumpForce => _jumpForce;
    
}
