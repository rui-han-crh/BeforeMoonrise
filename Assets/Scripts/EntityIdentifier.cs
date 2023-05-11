using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityIdentifier : MonoBehaviour
{
    public enum EntityType
    {
        Player,
        Enemy
    }

    [SerializeField]
    private EntityType entityType;

    public EntityType Type => entityType;
}
