using BeardedManStudios.Forge.Networking.Frame;
using System;
using MainThreadManager = BeardedManStudios.Forge.Networking.Unity.MainThreadManager;

namespace BeardedManStudios.Forge.Networking.Generated
{
	public partial class NetworkObjectFactory : NetworkObjectFactoryBase
	{
		public override void NetworkCreateObject(NetWorker networker, int identity, uint id, FrameStream frame, Action<NetworkObject> callback)
		{
			if (networker.IsServer)
			{
				if (frame.Sender != null && frame.Sender != networker.Me)
				{
					if (!ValidateCreateRequest(networker, identity, id, frame))
						return;
				}
			}
			
			bool availableCallback = false;
			NetworkObject obj = null;
			MainThreadManager.Run(() =>
			{
				switch (identity)
				{
					case BarrierBlockNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new BarrierBlockNetworkObject(networker, id, frame);
						break;
					case BattleNetworkNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new BattleNetworkNetworkObject(networker, id, frame);
						break;
					case ChatManagerNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new ChatManagerNetworkObject(networker, id, frame);
						break;
					case CubeForgeGameNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new CubeForgeGameNetworkObject(networker, id, frame);
						break;
					case DrawableTextureNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new DrawableTextureNetworkObject(networker, id, frame);
						break;
					case ExampleProximityPlayerNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new ExampleProximityPlayerNetworkObject(networker, id, frame);
						break;
					case HomeBaseNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new HomeBaseNetworkObject(networker, id, frame);
						break;
					case LobbyDotsNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new LobbyDotsNetworkObject(networker, id, frame);
						break;
					case LobbyPlayerNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new LobbyPlayerNetworkObject(networker, id, frame);
						break;
					case NetworkCameraNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new NetworkCameraNetworkObject(networker, id, frame);
						break;
					case PlayerControllerNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new PlayerControllerNetworkObject(networker, id, frame);
						break;
					case PlayerDrawNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new PlayerDrawNetworkObject(networker, id, frame);
						break;
					case ProjectileNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new ProjectileNetworkObject(networker, id, frame);
						break;
					case ServerInfoNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new ServerInfoNetworkObject(networker, id, frame);
						break;
					case TestNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new TestNetworkObject(networker, id, frame);
						break;
					case TimerNetworkObject.IDENTITY:
						availableCallback = true;
						obj = new TimerNetworkObject(networker, id, frame);
						break;
				}

				if (!availableCallback)
					base.NetworkCreateObject(networker, identity, id, frame, callback);
				else if (callback != null)
					callback(obj);
			});
		}

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}