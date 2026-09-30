using UnityEngine;

public struct AbilityContext
{
    public GameObject Caster;       // Quien lanza la habilidad
    public Vector2 TargetDirection; // La direccion del cast
}

public interface IAbility
{
    void Execute(AbilityContext context);
}