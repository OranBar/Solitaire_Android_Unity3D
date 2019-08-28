using System;
using System.Collections.Generic;

public static class IEnumerableEx {

	public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (var item in source)
			action(item);
	}
}
