using GuestUnion.Extensions;
using JoG.Chat;
using JoG.Networking;
using MessagePipe;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace JoG.LifetimeScopes {

    public class GameplaySceneScope : LifetimeScope {

        protected override void Configure(IContainerBuilder builder) {
            builder.RegisterInstance(Camera.main).Keyed(Constants.Camera.MainCamera);
            var uiCamera = Camera.allCameras.Find(c => c.CompareTag(Constants.Camera.UICamera));
            builder.RegisterInstance(uiCamera).Keyed(Constants.Camera.UICamera);
            builder.Register<DamageService>(Lifetime.Singleton).AsImplementedInterfaces();
            var options = builder.RegisterMessagePipe();
            options.InstanceLifetime = InstanceLifetime.Singleton;
            builder.RegisterMessageBroker<UIStateChangedMessage>(options);
            builder.RegisterEntryPoint<UIStateChangedHandler>();
            builder.RegisterEntryPoint<UnnamedMessageBroker>().AsSelf();
            builder.RegisterEntryPoint<ChatService>();
            builder.RegisterComponentInHierarchy<ChatBoxController>();
            builder.RegisterBuildCallback(static container => {
                var networkManager = container.Resolve<NetworkManager>();
                var damageService = container.Resolve<IDamageService>();
                foreach (var prefab in networkManager.NetworkConfig.Prefabs.Prefabs) {
                    var prefabObject = prefab.Prefab.GetComponent<NetworkObject>();
                    INetworkPrefabInstanceHandler handler;
                    if (prefab.Prefab.TryGetComponent<LifetimeScope>(out _)) {
                        handler = new GenericPrefabInstanceHandler(networkManager, prefabObject);
                    } else {
                        handler = new VContainerPrefabInstanceHandler(networkManager, prefabObject, container);
                    }
                    networkManager.PrefabHandler.AddHandler(prefabObject, handler);
                }
            });
            builder.RegisterDisposeCallback(static container => {
                var networkManager = container.Resolve<NetworkManager>();
                foreach (var prefab in networkManager.NetworkConfig.Prefabs.Prefabs) {
                    var prefabObject = prefab.Prefab.GetComponent<NetworkObject>();
                    networkManager.PrefabHandler.RemoveHandler(prefabObject);
                }
            });
        }
    }
}