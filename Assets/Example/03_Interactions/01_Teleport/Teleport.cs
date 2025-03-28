namespace Example.Teleport
{
	using UnityEngine;
	using Fusion;
	using Fusion.Addons.KCC;

	/// <summary>
	/// Interface to notify other processors about teleport event.
	/// </summary>
	public interface ITeleportListener
	{
		void OnTeleport(KCC kcc, KCCData data);
	}

	/// <summary>
	/// Example processor - teleporting to specific position and optionally setting look rotation and resetting velocities.
	/// </summary>
	public sealed class Teleport : KCCProcessor
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private Transform[] _targets;
		[SerializeField]
		private bool        _setLookRotation;
		[SerializeField]
		private bool        _resetDynamicVelocity;
		[SerializeField]
		private bool        _resetKinematicVelocity;

		// KCCProcessor INTERFACE

		public override void OnEnter(KCC kcc, KCCData data)
		{
			// Teleport only in fixed update to not introduce glitches caused by incorrect render prediction.
			if (kcc.IsInFixedUpdate == false)
				return;

			if (_targets.Length == 0)
			{
				Debug.LogError($"Missing target on {nameof(Teleport)} {name}", gameObject);
				return;
			}

			// If there are multiple targets the logic is executed on server only.
			bool executeOnServerOnly = _targets.Length > 1 && kcc.Runner.GameMode != GameMode.Shared;
			if (executeOnServerOnly == true && kcc.Runner.IsServer == false)
				return;

			Transform target = _targets[Random.Range(0, _targets.Length)];

			kcc.SetPosition(target.position);

			if (_setLookRotation == true)
			{
				kcc.SetLookRotation(target.rotation, true);
			}

			if (_resetDynamicVelocity == true)
			{
				kcc.SetDynamicVelocity(Vector3.zero);
			}

			if (_resetKinematicVelocity == true)
			{
				kcc.SetKinematicVelocity(Vector3.zero);
			}

			// Notify all listeners.
			foreach (ITeleportListener listener in kcc.GetProcessors<ITeleportListener>(true))
			{
				try
				{
					listener.OnTeleport(kcc, data);
				}
				catch (System.Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}
	}
}
