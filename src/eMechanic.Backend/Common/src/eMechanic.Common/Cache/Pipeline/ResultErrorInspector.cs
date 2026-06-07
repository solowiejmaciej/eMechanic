namespace eMechanic.Common.Cache.Pipeline;

using System.Reflection;
using eMechanic.Common.Result;

internal static class ResultErrorInspector<TResponse>
{
	private static readonly MethodInfo? HasErrorMethod =
		typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<,>)
			? typeof(TResponse).GetMethod("HasError", BindingFlags.Public | BindingFlags.Instance)
			  ?? typeof(TResponse).GetProperty("HasError")?.GetGetMethod()
			: null;

	internal static bool IsSuccessResponseType => HasErrorMethod is not null;

	internal static bool HasError(TResponse response)
	{
		if (HasErrorMethod is null)
		{
			return false;
		}

		return (bool)HasErrorMethod.Invoke(response, null)!;
	}
}

