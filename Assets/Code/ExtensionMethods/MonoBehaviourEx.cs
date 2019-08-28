using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

namespace OranUnityUtils
{
	public static class MonoBehaviourEx
	{
		private static IEnumerator WaitFrames(int frames)
        {
            while (frames-- > 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }

		/// <summary>
		/// This method will immediately throw an exception if the component is not found. Use this when the T component is necessary for the correct execution of the program, 
		/// as opposed to a normal GetComponent with a null check
		/// </summary>														
		public static T GetRequiredComponent<T>(this MonoBehaviour thisMonoBehaviour) where T : class {
			var retrievedComponent = thisMonoBehaviour.GetComponent<T>();
			Assert.IsNotNull(retrievedComponent,
				string.Format("Script {1} on GameObject \"{0}\" does not have the required component of type {2}", thisMonoBehaviour.name, thisMonoBehaviour.GetType(), typeof(T)));

			return retrievedComponent;
		}
		/// <summary>
		/// This method will immediately throw an exception if no component is found. Use this when the T component is necessary for the correct execution of the program, 
		/// as opposed to a normal GetComponent with a null check
		/// </summary>														
		public static T GetRequiredComponentInChildren<T>(this MonoBehaviour thisMonoBehaviour) where T : class {
			var retrievedComponent = thisMonoBehaviour.GetComponentInChildren<T>();
			Assert.IsNotNull(retrievedComponent,
				string.Format("Script {1} on GameObject \"{0}\" does not have a child with component of type {2}", thisMonoBehaviour.name, thisMonoBehaviour.GetType(), typeof(T)));

			return retrievedComponent;
		}
	}
}