using UnityEngine;
using System.Linq;

namespace OranUnityUtils
{
    public static class RectTransformEx {
        
        public static Rect RectTransformToScreenSpace(this RectTransform transform) {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            Rect rect = new Rect(transform.position.x, transform.position.y, size.x, size.y);
            rect.x -= (transform.pivot.x * size.x);
            rect.y -= (transform.pivot.y * size.y);
            return rect;
        }
		
        public static Vector2 CenterToScreenSpace(this RectTransform rectTransf) {
            return rectTransf.RectTransformToScreenSpace().center;
        }

		public static int GetPixelHeight(this RectTransform rectTransform) {
			Vector3[] worldCorners = new Vector3[4];
			rectTransform.GetWorldCorners(worldCorners);
			var minY = worldCorners.Min(c => c.y);
			var maxY = worldCorners.Max(c => c.y);
			return (int)(maxY - minY);
		}

		public static int GetPixelWidth(this RectTransform rectTransform) {
			Vector3[] worldCorners = new Vector3[4];
			rectTransform.GetWorldCorners(worldCorners);
			var minX = worldCorners.Min(c => c.x);
			var maxX = worldCorners.Max(c => c.x);
			return (int)(maxX - minX);
		}
	}
}
