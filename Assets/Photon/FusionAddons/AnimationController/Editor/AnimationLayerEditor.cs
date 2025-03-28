namespace Fusion.Addons.AnimationController.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(AnimationLayer), true)]
	public class AnimationLayerEditor : Editor
	{
		// Editor INTERFACE

		public override bool RequiresConstantRepaint()
		{
			return true;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (Application.isPlaying == false)
				return;

			AnimationLayer layer = target as AnimationLayer;

			DrawLine(Color.gray);

			EditorGUILayout.Toggle("Is Active", layer.IsActive());
			EditorGUILayout.LabelField("Weight", layer.Weight.ToString("0.00"));
			EditorGUILayout.LabelField("Fading Speed", layer.FadingSpeed.ToString("0.00"));
			EditorGUILayout.LabelField("Playable Weight", layer.GetPlayableInputWeight().ToString("0.00"));

			DrawLine(Color.gray);

			layer.OnInspectorGUI();
		}

		// PRIVATE METHODS

		public static void DrawLine(Color color, float thickness = 1.0f, float padding = 10.0f)
		{
			Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));

			controlRect.height = thickness;
			controlRect.y += padding * 0.5f;

			EditorGUI.DrawRect(controlRect, color);
		}
	}
}
