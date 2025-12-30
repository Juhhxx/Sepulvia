using System;
using UnityEngine;

public interface IMovementType
{
    public Vector3 Direction { get; }
    public float Speed { get; }

    public void Move();
    public void ResetMovement();
}