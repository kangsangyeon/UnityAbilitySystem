using FishNet.Object;

namespace Samples.FishNet
{
    public class Player : NetworkBehaviour
    {
        [Server]
        private void Server_AddPlayer()
        {
            GameDependencies.playerManager.AddPlayer(base.Owner, this);
        }

        [Server]
        private void Server_RemovePlayer()
        {
            GameDependencies.playerManager.RemovePlayer(base.Owner);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Server_AddPlayer();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            Server_RemovePlayer();
        }
    }
}