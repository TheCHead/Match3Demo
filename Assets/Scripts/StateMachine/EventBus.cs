using System;
using Cysharp.Threading.Tasks;
using Scripts.States;

public static class EventBus
{
    public static event Func<EGameState, UniTask> OnStateChangeRequested;

    public static async UniTask RequestStateChange(EGameState newState)
    {
        if (OnStateChangeRequested != null)
        {
            await OnStateChangeRequested.Invoke(newState);
        }
    }
}
