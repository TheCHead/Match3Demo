using Scripts.States;
using Scripts.UI;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<StateMachine>().AsSingle().NonLazy();
        Container.Bind<StateFactory>().AsSingle().NonLazy();
        Container.Bind<PresenterFactory>().AsSingle();
        Container.Bind<ViewFactory>().AsSingle();

        Container.Bind<UIService>().AsSingle().NonLazy();
    }
}
