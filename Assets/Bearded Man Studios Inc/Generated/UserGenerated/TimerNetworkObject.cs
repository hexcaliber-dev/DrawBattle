using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0]")]
	public partial class TimerNetworkObject : NetworkObject
	{
		public const int IDENTITY = 15;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private int _timeRemaining;
		public event FieldEvent<int> timeRemainingChanged;
		public Interpolated<int> timeRemainingInterpolation = new Interpolated<int>() { LerpT = 0f, Enabled = false };
		public int timeRemaining
		{
			get { return _timeRemaining; }
			set
			{
				// Don't do anything if the value is the same
				if (_timeRemaining == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_timeRemaining = value;
				hasDirtyFields = true;
			}
		}

		public void SettimeRemainingDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_timeRemaining(ulong timestep)
		{
			if (timeRemainingChanged != null) timeRemainingChanged(_timeRemaining, timestep);
			if (fieldAltered != null) fieldAltered("timeRemaining", _timeRemaining, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			timeRemainingInterpolation.current = timeRemainingInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _timeRemaining);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_timeRemaining = UnityObjectMapper.Instance.Map<int>(payload);
			timeRemainingInterpolation.current = _timeRemaining;
			timeRemainingInterpolation.target = _timeRemaining;
			RunChange_timeRemaining(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _timeRemaining);

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
				if (timeRemainingInterpolation.Enabled)
				{
					timeRemainingInterpolation.target = UnityObjectMapper.Instance.Map<int>(data);
					timeRemainingInterpolation.Timestep = timestep;
				}
				else
				{
					_timeRemaining = UnityObjectMapper.Instance.Map<int>(data);
					RunChange_timeRemaining(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (timeRemainingInterpolation.Enabled && !timeRemainingInterpolation.current.UnityNear(timeRemainingInterpolation.target, 0.0015f))
			{
				_timeRemaining = (int)timeRemainingInterpolation.Interpolate();
				//RunChange_timeRemaining(timeRemainingInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public TimerNetworkObject() : base() { Initialize(); }
		public TimerNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public TimerNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
