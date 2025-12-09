using UnityEditor;
using UnityEngine;

public static class RectTransformTools
{
	[MenuItem("Tools/UI/Anchors → Match RectTransform %&a")]
	private static void SetAnchorsToRect()
	{
		if (Selection.activeTransform is RectTransform rect)
		{
			var parent = rect.parent as RectTransform;
			if (parent == null) return;

			var rectPosMin = rect.offsetMin;
			var rectPosMax = rect.offsetMax;

			rect.anchorMin = new Vector2(
			                             rect.anchorMin.x + rectPosMin.x / parent.rect.width,
			                             rect.anchorMin.y + rectPosMin.y / parent.rect.height);

			rect.anchorMax = new Vector2(
			                             rect.anchorMax.x + rectPosMax.x / parent.rect.width,
			                             rect.anchorMax.y + rectPosMax.y / parent.rect.height);

			rect.offsetMin = rect.offsetMax = Vector2.zero;
		}
	}
}