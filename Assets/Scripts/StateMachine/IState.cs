using Cysharp.Threading.Tasks;

namespace Scripts.StateMachine
{
    public interface IState 
    {
        UniTask Enter(IState fromState);
        UniTask Exit(IState toState);
    }
}
