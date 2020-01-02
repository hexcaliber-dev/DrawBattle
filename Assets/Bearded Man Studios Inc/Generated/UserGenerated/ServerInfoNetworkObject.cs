using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0]")]
	public partial class ServerInfoNetworkObject : NetworkObject
	{
		public const int IDENTITY = 6;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private int _numPlayers;
		public event FieldEvent<int> numPlayersChanged;
		public Interpolated<int> numPlayersInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int numPlayers
		{
			get { return _numPlayers; }
			set
			{
				// Don't do anything if the value is the same
				if (_numPlayers == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_numPlayers = value;
				hasDirtyFields = true;
			}
		}

		public void SetnumPlayersDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_numPlayers(ulong timestep)
		{
			if (numPlayersChanged != null) numPlayersChanged(_numPlayers, timestep);
			if (fieldAltered != null) fieldAltered("numPlayers", _numPlayers, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			numPlayersInterpolation.current = numPlayersInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _numPlayers);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_numPlayers = UnityObjectMapper.Instance.Map<int>(payload);
			numPlayersInterpolation.current = _numPlayers;
			numPlayersInterpolation.target = _numPlayers;
			RunChange_numPlayers(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _numPlayers);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (numPlayersInterpolation.Enabled)
				{
					numPlayersInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					numPlayersInterpolation.Timestep = timestep;
				}
				else
				{
					_numPlayers = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_numPlayers(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (numPlayersInterpolation.Enabled && !numPlayersInterpolation.current.UnityNear(numPlayersInterpolation.target, 0.0015f))
			{
				_numPlayers = (int)numPlayersInterpolation.Interpolate();
				//RunChange_numPlayers(numPlayersInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public ServerInfoNetworkObject() : base() { Initialize(); }
		public ServerInfoNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public ServerInfoNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
