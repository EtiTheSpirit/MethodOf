using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace MethodOf {
	public static class MethodOfProvider {

		private static readonly ConditionalWeakTable<Delegate, MethodBase> _methods = new ConditionalWeakTable<Delegate, MethodBase>();
		private static readonly Type[] _getMethodFromHandleParams = new Type[] { typeof(RuntimeMethodHandle) };
		private static MethodInfo? _getMethodFromHandle = null;

		/// <summary>
		/// Mimics an IL intrinsic that has been coined <see langword="methodof"/> (after <see langword="typeof"/>).
		/// It operates exactly like <see langword="typeof"/>, with the exception that it operates on methods instead
		/// of types. Input a method, and its corresponding <see cref="MethodBase"/> (often <see cref="MethodInfo"/>) will be returned.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="del"></param>
		/// <returns></returns>
		public static MethodBase methodof<T>(T del) where T : Delegate {
			if (_methods.TryGetValue(del, out MethodBase? method)) {
				return method;
			}
			DynamicMethod dynamicMethod = new DynamicMethod("GetSelf", typeof(MethodBase), Array.Empty<Type>());
			ILGenerator generator = dynamicMethod.GetILGenerator();
			// TARGET IL:
			/*
			* ldtoken SomeNamespace.SomeType.del							// Returns RuntimeMethodHandle
			* call MethodBase::GetMethodFromHandle(RuntimeMethodHandle)		// RuntimeMethodHandle => MethodBase
			* ret
			*/
			_getMethodFromHandle ??= typeof(MethodBase).GetMethod(nameof(MethodBase.GetMethodFromHandle), BindingFlags.Static | BindingFlags.Public, null, _getMethodFromHandleParams, null);
			generator.Emit(OpCodes.Ldtoken, del.GetMethodInfo());
			generator.Emit(OpCodes.Call, _getMethodFromHandle!);
			generator.Emit(OpCodes.Ret);
			Func<MethodBase> getter = (Func<MethodBase>)dynamicMethod.CreateDelegate(typeof(Func<MethodBase>));
			MethodBase result = getter.Invoke();
			_methods.Add(del, result);
			return result;
		}
	}
}
