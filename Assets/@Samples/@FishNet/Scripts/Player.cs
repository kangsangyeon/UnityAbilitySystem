using FishNet.Object;

namespace Samples.FishNet
{
    public class Player : NetworkBehaviour
    {
        public event System.Action<Player> onStartNetwork;
        public event System.Action<Player> onStartOwnerClient_OnServer;

        [ServerRpc]
        private void ServerRpc_OnStartOwnerClient()
        {
            onStartOwnerClient_OnServer?.Invoke(this);
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            gameObject.name = $"Player_{base.OwnerId}";
            GameDependencies.playerManager.AddPlayer(base.Owner, this);

            onStartNetwork?.Invoke(this);
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
            GameDependencies.playerManager.RemovePlayer(base.Owner);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (base.IsOwner)
            {
                ServerRpc_OnStartOwnerClient();
            }
        }
    }
}