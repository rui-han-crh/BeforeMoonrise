using System;

public interface IMovementController
{
    public event Action OnMove;

    public event Action OnStop;

    public event Action OnBeginDodge;

    public event Action OnEndDodge;
}