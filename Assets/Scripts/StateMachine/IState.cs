using Cysharp.Threading.Tasks;

namespace Scripts.States
{
    public interface IState 
    {
        UniTask Enter(IState fromState);
        UniTask Exit(IState toState);
    }
}
