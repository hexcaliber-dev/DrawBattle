using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0,0]")]
	public partial class PlayerStatsNetworkObject : NetworkObject
	{
		public const int IDENTITY = 12;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private int _baseHealth;
		public event FieldEvent<int> baseHealthChanged;
		public Interpolated<int> baseHealthInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int baseHealth
		{
			get { return _baseHealth; }
			set
			{
				// Don't do anything if the value is the same
				if (_baseHealth == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_baseHealth = value;
				hasDirtyFields = true;
			}
		}

		public void SetbaseHealthDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_baseHealth(ulong timestep)
		{
			if (baseHealthChanged != null) baseHealthChanged(_baseHealth, timestep);
			if (fieldAltered != null) fieldAltered("baseHealth", _baseHealth, timestep);
		}
		[ForgeGeneratedField]
		private int _tankHealth;
		public event FieldEvent<int> tankHealthChanged;
		public Interpolated<int> tankHealthInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int tankHealth
		{
			get { return _tankHealth; }
			set
			{
				// Don't do anything if the value is the same
				if (_tankHealth == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_tankHealth = value;
				hasDirtyFields = true;
			}
		}

		public void SettankHealthDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_tankHealth(ulong timestep)
		{
			if (tankHealthChanged != null) tankHealthChanged(_tankHealth, timestep);
			if (fieldAltered != null) fieldAltered("tankHealth", _tankHealth, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			baseHealthInterpolation.current = baseHealthInterpolation.target;
			tankHealthInterpolation.current = tankHealthInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _baseHealth);
			UnityObjectMapper.Instance.MapBytes(data, _tankHealth);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_baseHealth = UnityObjectMapper.Instance.Map<int>(payload);
			baseHealthInterpolation.current = _baseHealth;
			baseHealthInterpolation.target = _baseHealth;
			RunChange_baseHealth(timestep);
			_tankHealth = UnityObjectMapper.Instance.Map<int>(payload);
			tankHealthInterpolation.current = _tankHealth;
			tankHealthInterpolation.target = _tankHealth;
			RunChange_tankHealth(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _baseHealth);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _tankHealth);

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
				if (baseHealthInterpolation.Enabled)
				{
					baseHealthInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					baseHealthInterpolation.Timestep = timestep;
				}
				else
				{
					_baseHealth = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_baseHealth(timestep);
				}
			}
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (tankHealthInterpolation.Enabled)
				{
					tankHealthInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					tankHealthInterpolation.Timestep = timestep;
				}
				else
				{
					_tankHealth = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_tankHealth(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (baseHealthInterpolation.Enabled && !baseHealthInterpolation.current.UnityNear(baseHealthInterpolation.target, 0.0015f))
			{
				_baseHealth = (int)baseHealthInterpolation.Interpolate();
				//RunChange_baseHealth(baseHealthInterpolation.Timestep);
			}
			if (tankHealthInterpolation.Enabled && !tankHealthInterpolation.current.UnityNear(tankHealthInterpolation.target, 0.0015f))
			{
				_tankHealth = (int)tankHealthInterpolation.Interpolate();
				//RunChange_tankHealth(tankHealthInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public PlayerStatsNetworkObject() : base() { Initialize(); }
		public PlayerStatsNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public PlayerStatsNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
