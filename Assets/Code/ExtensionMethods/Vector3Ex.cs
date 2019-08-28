using UnityEngine;				
		  
namespace OranUnityUtils {

	public static class Vector3Ex {

		/// <summary>
		/// Returns a copy of the given vector with clamped values
		/// </summary>
		/// <returns>A copy of the vector with the valuex clamped</returns>
		public static Vector3 ClampXYZ(this Vector3 vector, float min, float max){
			return new Vector3 (Mathf.Clamp(vector.x, min, max), Mathf.Clamp(vector.y, min, max), Mathf.Clamp(vector.z, min, max));
		}

		/// <summary>
		/// Returns a copy of the given vector with clamped values
		/// </summary>
		/// <returns>A copy of the vector with the valuex clamped</returns>
		public static Vector3 ClampX(this Vector3 vector, float min, float max) {
			return new Vector3(Mathf.Clamp(vector.x, min, max), vector.y, vector.z);
		}

		/// <summary>
		/// Returns a copy of the given vector with clamped values
		/// </summary>
		/// <returns>A copy of the vector with the valuex clamped</returns>
		public static Vector3 ClampY(this Vector3 vector, float min, float max) {
			return new Vector3(vector.x, Mathf.Clamp(vector.y, min, max), vector.z);
		}

		/// <summary>
		/// Returns a copy of the given vector with clamped values
		/// </summary>
		/// <returns>A copy of the vector with the valuex clamped</returns>
		public static Vector3 ClampZ(this Vector3 vector, float min, float max) {
			return new Vector3(vector.x, vector.y, Mathf.Clamp(vector.z, min, max));
		}

		/// <summary>
		/// Returns a copy of the given vector with clamped values
		/// </summary>
		/// <returns>A copy of the vector with the valuex clamped</returns>
		public static Vector3 ClampXY(this Vector3 vector, float min, float max) {
			return vector.ClampX(min, max).ClampY(min, max);
		}

		/// <summary>
		/// Returns a copy of the given vector with clamped values
		/// </summary>
		/// <returns>A copy of the vector with the valuex clamped</returns>
		public static Vector3 ClampXZ(this Vector3 vector, float min, float max) {
			return vector.ClampX(min, max).ClampZ(min, max);
		}

		/// <summary>
		/// Returns a copy of the given vector with clamped values
		/// </summary>
		/// <returns>A copy of the vector with the valuex clamped</returns>
		public static Vector3 ClampYZ(this Vector3 vector, float min, float max) {
			return vector.ClampY(min, max).ClampZ(min, max);
		}
	}
}