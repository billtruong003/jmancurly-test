namespace Fusion.Addons.AnimationController
{
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using UnityEngine;

	public static partial class GameObjectAnimationsExtensions
	{
		// PUBLIC METHODS

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T GetComponentNoAlloc<T>(this GameObject gameObject) where T : class
		{
#if UNITY_EDITOR
			return GameObjectAnimationsExtensions<T>.GetComponentNoAlloc(gameObject);
#else
			return gameObject.GetComponent<T>();
#endif
		}
	}

	public static partial class GameObjectAnimationsExtensions<T> where T : class
	{
		// PRIVATE MEMBERS

		private static List<T> _components = new List<T>();

		// PUBLIC METHODS

		public static T GetComponentNoAlloc(GameObject gameObject)
		{
			_components.Clear();

			gameObject.GetComponents(_components);

			if (_components.Count > 0)
			{
				T component = _components[0];

				_components.Clear();

				return component;
			}

			return null;
		}
	}
}
