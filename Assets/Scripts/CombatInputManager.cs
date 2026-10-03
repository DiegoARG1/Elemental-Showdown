using UnityEngine;
using System;
using UnityEngine.InputSystem.EnhancedTouch;
// Usamos un alias para evitar conflictos con el Touch viejo de Unity
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class CombatInputManager : MonoBehaviour
{
    [Header("Configuracion Tactil")]
    [Tooltip("Porcentaje de la pantalla que representa el estiramiento maximo (0.1 a 0.3 recomendado)")]
    [Range(0.05f, 0.5f)]
    public float maxDragScreenPercentage = 0.15f;

    [Header("Interfaz")]
    public RectTransform skill2ButtonRect;


    public static event Action<Vector2> OnBasicAttackStarted;
    public static event Action<Vector2> OnBasicAttackStay;
    public static event Action OnBasicAttackEnded;

    public static event Action OnSkill2Started;
    public static event Action<Vector2> OnSkill2Stay;
    public static event Action OnSkill2Ended;

    private Vector2 startTouchPosition;
    private Finger trackingFinger; // Guarda el ID del dedo atacante
    private Finger skill2Finger;
    private Vector2 startSkill2TouchPosition;


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
        if (skill2ButtonRect != null && RectTransformUtility.RectangleContainsScreenPoint(skill2ButtonRect, finger.screenPosition))
        {
            if (skill2Finger == null)
            {
                skill2Finger = finger;
                startSkill2TouchPosition = finger.screenPosition;
                OnSkill2Started?.Invoke();
                return;
            }
        }

        if (trackingFinger == null && finger.screenPosition.x > Screen.width / 2f)
        {
            trackingFinger = finger;
            startTouchPosition = finger.screenPosition;
            OnBasicAttackStarted?.Invoke(Vector2.right);
        }
    }

    private void HandleFingerMove(Finger finger)
    {
        float maxDragDistance = Screen.width * maxDragScreenPercentage;

        if (trackingFinger == finger)
        {
            Vector2 rawDrag = finger.screenPosition - startTouchPosition;
            float dragPower = Mathf.Clamp01(rawDrag.magnitude / maxDragDistance);
            Vector2 dragDirection = rawDrag.normalized * dragPower;

            if (dragDirection != Vector2.zero)
            {
                OnBasicAttackStay?.Invoke(dragDirection);
            }
        }
        else if (skill2Finger == finger)
        {
            Vector2 rawDrag = finger.screenPosition - startSkill2TouchPosition;
            float dragPower = Mathf.Clamp01(rawDrag.magnitude / maxDragDistance);
            Vector2 dragDirection = rawDrag.normalized * dragPower;

            if (dragDirection != Vector2.zero)
            {
                OnSkill2Stay?.Invoke(dragDirection);
            }
        }
    }

    private void HandleFingerUp(Finger finger)
    {
        if (trackingFinger == finger)
        {
            OnBasicAttackEnded?.Invoke();
            trackingFinger = null;
        }
        else if (skill2Finger == finger)
        {
            OnSkill2Ended?.Invoke();
            skill2Finger = null;
        }
    }
}