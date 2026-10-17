using UnityEngine;

[CreateAssetMenu(fileName = "PlayerVariables", menuName = "Scriptable Objects/PlayerVariables")]
public class PlayerVariables : ScriptableObject
{
    [SerializeField]private float _baseMoveSpeed;
    [SerializeField] private float _sprintMultiplier;
    
    public float BaseMoveSpeed => _baseMoveSpeed;
    public float SprintMultiplier => _sprintMultiplier;
    
}
