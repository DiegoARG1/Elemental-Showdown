using UnityEngine;

public struct AbilityContext
{
    public GameObject Caster;       // Quien lanza la habilidad
    public Vector2 TargetDirection; // La direccion del cast
}

public interface IAbility
{
    void OnInputStarted(AbilityContext context);//Se llama en cuanto tocas la pantalla

    void OnInputStay(AbilityContext context);//Se llama mientras mantienes presionada la pantalla 

    void OnInputEnded(AbilityContext context);//Se llama cuando el dedo se levanta
}