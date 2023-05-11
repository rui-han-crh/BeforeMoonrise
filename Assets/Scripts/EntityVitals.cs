using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EntityVitals : MonoBehaviour
{
    private float maxHealth;
    private float currentHealth;

    private float maxStamina;
    private float currentStamina;

    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float CurrentHealth { 
        get => currentHealth; 
        set {
            if (value < currentHealth) {
                OnTakeDamage?.Invoke();
            }
            
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            if (currentHealth == 0)
            {
                OnDeath?.Invoke();
            }
        }
    }

    public float HealthPercentage => currentHealth / maxHealth;

    public float MaxStamina { get => maxStamina; set => maxStamina = value; }
    public float CurrentStamina { get => currentStamina; set => currentStamina = value; }

    public float StaminaPercentage => currentStamina / maxStamina;
    public event Action OnDeath;

    public delegate void TakeDamageAction();
    public event TakeDamageAction OnTakeDamage;
}

#if UNITY_EDITOR
[CustomEditor(typeof(EntityVitals))]
public class EntityVitalsEditor : Editor
{
    EntityVitals entityVitals;

    public void OnEnable()
    {
        entityVitals = (EntityVitals)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField($"Health: {entityVitals.CurrentHealth}/{entityVitals.MaxHealth}");
        EditorGUILayout.LabelField($"Stamina: {entityVitals.CurrentStamina}/{entityVitals.MaxStamina}");
    }
}
#endif