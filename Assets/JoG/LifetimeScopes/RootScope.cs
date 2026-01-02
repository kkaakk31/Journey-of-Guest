using GuestUnion.UI;
using JoG.Modding;
using JoG.Networking;
using JoG.Player;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace JoG.LifetimeScopes {

    public class RootScope : LifetimeScope {

        protected override void Configure(IContainerBuilder builder) {
            var unityServices = UnityServices.Instance;
            builder.RegisterInstance(unityServices);
            builder.RegisterInstance(unityServices.GetAuthenticationService());
            builder.RegisterInstance(unityServices.GetLobbyService());
            builder.RegisterInstance(unityServices.GetMatchmakerService());
            builder.RegisterInstance(unityServices.GetMultiplayerService());
            builder.RegisterInstance(unityServices.GetPlayerAccountService());
            builder.RegisterInstance(unityServices.GetQosService());
            builder.RegisterInstance(unityServices.GetRelayService());
            foreach (var map in InputSystem.actions.actionMaps) {
                builder.RegisterInstance(map).Keyed(map.name);
                foreach (var action in map.actions) {
                    builder.RegisterInstance(action).Keyed(action.name);
                }
            }
            builder.RegisterComponent(NetworkManager.Singleton);
            builder.RegisterComponent(FindFirstObjectByType<PopupManager>());
            builder.Register<ModManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<DefaultPackageManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<DefaultLanguageManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<UnityProfileService>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PlayerRegistry>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.RegisterEntryPoint<SessionService>();
            builder.RegisterBuildCallback(static container => {
                var manager = container.Resolve<NetworkManager>();
                var playerPrefab = manager.NetworkConfig.PlayerPrefab.GetComponent<NetworkObject>();
                var handler = new VContainerPrefabInstanceHandler(manager,
                    playerPrefab,
                    container);
                manager.PrefabHandler.AddHandler(playerPrefab, handler);
            });
        }
    }
}