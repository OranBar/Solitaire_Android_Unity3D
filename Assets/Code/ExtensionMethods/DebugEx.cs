using System;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public static class DebugEx
{

	public static Dictionary<Type, string> typeToTag = new Dictionary<Type, string>();

	public static void PrintMethodName()
	{
		StackTrace st = new StackTrace();
		StackFrame sf = st.GetFrame(1);
		MethodBase currentMethodName = sf.GetMethod();

		UnityEngine.Debug.Log("I'm in " + currentMethodName);
	}

	/// <summary>
	/// First argument is the gameObject associated with that code module.
	/// 
	/// </summary>
	/// <param name="context"></param>
	/// <param name="parameters"></param>
	public static void PrintMethodName(GameObject context, params object[] parameters)
	{
		StackTrace st = new StackTrace();
		StackFrame sf = st.GetFrame(1);
		MethodBase currentMethodName = sf.GetMethod();
		//ParameterInfo[] parameters = sf.GetMethod().GetParameters();
		UnityEngine.Debug.Log("I'm in " + currentMethodName, context);

		ParameterInfo[] parameterTypes = st.GetFrame(1).GetMethod().GetParameters();

		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < parameters.Length; i++)
		{
			var parameterType = parameterTypes[i];
			sb.Append(parameterType.ParameterType + ": " + parameterType.Name + " = " + parameters[i].ToString() + "\n");
			i++;
		}

		UnityEngine.Debug.Log(sb.ToString());

	}

	#region Logging Helpers
	#region Tag Logic
	public static void RegisterTag(this MonoBehaviour mb, string tag)
	{
		typeToTag[mb.GetType()] = "[" + tag + "] ";
	}

	private static string ApplyTagIfTagWasAssigned<T>(string message)
	{
		string tag = "";
		if (typeToTag.TryGetValue(typeof(T), out tag))
		{
			return message = tag + message;
		}
		else
		{
			return message;
		}
	}

	private static object ApplyTagIfTagWasAssigned<T>(object message)
	{
		string tag = "";
		if (typeToTag.TryGetValue(typeof(T), out tag))
		{
			return message = tag + message;
		}
		else
		{
			return message;
		}
	}
	#endregion

	#region Log
	public static void Log<T>(this T mb, object message) where T : MonoBehaviour
	{
		message = ApplyTagIfTagWasAssigned<T>(message);

		Debug.Log(message, mb.gameObject);
	}

	public static void LogFormat<T>(this T mb, string format, params string[] args) where T : MonoBehaviour
	{
		format = ApplyTagIfTagWasAssigned<T>(format);

		Debug.LogFormat(mb.gameObject, format, args);
	}

	public static void LogWarning<T>(this T mb, object message) where T : MonoBehaviour
	{
		message = ApplyTagIfTagWasAssigned<T>(message);

		Debug.LogWarning(message, mb.gameObject);
	}

	public static void LogWarningFormat<T>(this T mb, string format, params string[] args) where T : MonoBehaviour
	{
		format = ApplyTagIfTagWasAssigned<T>(format);

		Debug.LogWarningFormat(mb.gameObject, format, args);
	}

	public static void LogError<T>(this T mb, object message) where T : MonoBehaviour
	{
		message = ApplyTagIfTagWasAssigned<T>(message);
		//For now we'll keep it like that. We'd need a way of knowing if we

		Debug.LogError(message, mb.gameObject);
	}

	public static void LogErrorFormat<T>(this T mb, string format, params string[] args) where T : MonoBehaviour
	{
		format = ApplyTagIfTagWasAssigned<T>(format);

		Debug.LogErrorFormat(mb.gameObject, format, args);
	}
	#endregion

	#region Assertions
	public static void Assert<T>(this T mb, bool condition) where T : MonoBehaviour
	{
		Debug.Assert(condition, mb.gameObject);
	}

	public static void Assert<T>(this T mb, string message, bool condition) where T : MonoBehaviour
	{
		Debug.Assert(condition, message, mb.gameObject);
	}
	#endregion

	#endregion

}


