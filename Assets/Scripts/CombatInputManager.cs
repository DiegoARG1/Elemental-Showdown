using UnityEngine;
using System;
using UnityEngine.InputSystem.EnhancedTouch;
// Usamos un alias para evitar conflictos con el Touch viejo de Unity
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CombatInputManager : MonoBehaviour
{
    public static event Action<Vector2> OnBasicAttackStarted;
    public static event Action<Vector2> OnBasicAttackStay;
    public static event Action OnBasicAttackEnded;

    private Vector2 startTouchPosition;
    private Finger trackingFinger; // Guarda el ID del dedo atacante

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
        Touch.onFingerDown += HandleFingerDown;//Presiona
        Touch.onFingerMove += HandleFingerMove;//Mueve
        Touch.onFingerUp += HandleFingerUp;//Suelta
    }

    private void OnDisable()
    {
        Touch.onFingerDown -= HandleFingerDown;
        Touch.onFingerMove -= HandleFingerMove;
        Touch.onFingerUp -= HandleFingerUp;
        TouchSimulation.Disable();
        EnhancedTouchSupport.Disable();
    }

    private void HandleFingerDown(Finger finger)
    {
        if (trackingFinger == null && finger.screenPosition.x > Screen.width / 2f)
        {
            trackingFinger = finger; // Bloqueamos nuestra mira en este dedo
            startTouchPosition = finger.screenPosition;
            OnBasicAttackStarted?.Invoke(Vector2.right);//Ataque por defecto a la derecha
        }
    }

    private void HandleFingerMove(Finger finger)
    {
        if (trackingFinger == finger)
        {
            Vector2 dragDirection = (finger.screenPosition - startTouchPosition).normalized;

            if (dragDirection != Vector2.zero)
            {
                OnBasicAttackStay?.Invoke(dragDirection);//Manda hacia donde apunta
            }
        }
    }

    private void HandleFingerUp(Finger finger)
    {
        // Solo respondemos si el dedo que se acaba de levantar es el mismo que tenemos rastreando
        if (trackingFinger == finger)
        {
            OnBasicAttackEnded?.Invoke();//Sueltas el dedo
            trackingFinger = null;
        }
    }
}