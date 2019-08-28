using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

namespace OranUnityUtils
{
	public static class UnityObjectEx
	{
		public static IEnumerator ToIEnum(this object unityObj, Action method)
		{
			return ToIEnumImpl(method);
		}

		private static IEnumerator ToIEnumImpl(Action method)
		{
			method();
			yield return null;
		}

		public static IEnumerator WaitForSeconds_Coro(this object unityObj, float time)
		{
			return WaitForSecondsImpl(time);
		}

		private static IEnumerator WaitForSecondsImpl(float time)
		{
			yield return new WaitForSeconds(time);
		}

		public static IEnumerator WaitForNextFrame_Coro(this object unityObj)
		{
			return WaitForNextFrameImpl();
		}

		private static IEnumerator WaitForNextFrameImpl()
		{
			yield return new WaitForEndOfFrame();
		}

		public static IEnumerator WaitForAnim(this object unityObj, Animator anim, int layer = 0)
		{
			AnimatorStateInfo animStateInfo = anim.GetCurrentAnimatorStateInfo(0);
			float currentAnimTime = animStateInfo.normalizedTime * animStateInfo.length;
			yield return new WaitForSeconds(animStateInfo.length - currentAnimTime);
		}

		/// <summary>
		/// For instances of classes that are components, but don't have access to .gameObject (i.e. interfaces)
		/// </summary>
		/// <returns>The GameObject.</returns>
		public static GameObject GameObject(this object o)
		{
			Assert.IsTrue(o is Component, "The argument is not a component. Its type is " + o.GetType());
			//Component tmp = o as Component;
			//return (tmp != null) ? tmp.gameObject : null;
			if ((Component)o == null)
			{
				return null;
			}
			else
			{
				return ((Component)o).gameObject;
			}
		}
	}
}

