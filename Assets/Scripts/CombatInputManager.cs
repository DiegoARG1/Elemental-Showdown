using UnityEngine;
using System;
using UnityEngine.InputSystem.EnhancedTouch;
// Usamos un alias para evitar conflictos con el Touch viejo de Unity
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CombatInputManager : MonoBehaviour
{
    public static event Action<Vector2> OnBasicAttackReleased;

    private Vector2 startTouchPosition;
    private Finger trackingFinger; // Guarda el ID del dedo atacante

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
        Touch.onFingerDown += HandleFingerDown;//Presiona
        Touch.onFingerUp += HandleFingerUp;//Suelta
    }

    private void OnDisable()
    {
        Touch.onFingerDown -= HandleFingerDown;
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
        }
    }

    private void HandleFingerUp(Finger finger)
    {
        // Solo respondemos si el dedo que se acaba de levantar es el mismo que tenemos rastreando
        if (trackingFinger == finger)
        {
            Vector2 endTouchPosition = finger.screenPosition;

            // Calculamos hacia donde arrastro el dedo y normalizamos el valor (para que su longitud sea 1)
            Vector2 dragDirection = (endTouchPosition - startTouchPosition).normalized;

            // Si el jugador solo hizo "Tap" (tocó y solto sin arrastrar)
            if (dragDirection == Vector2.zero)
            {
                dragDirection = Vector2.right; // Por defecto disparamos a la derecha
            }

            OnBasicAttackReleased?.Invoke(dragDirection);//pasamos la direccion

            trackingFinger = null;
        }
    }
}