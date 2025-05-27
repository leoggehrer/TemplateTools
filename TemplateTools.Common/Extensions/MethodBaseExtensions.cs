//@BaseCode
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TemplateTools.Common.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="MethodBase"/> class.
    /// </summary>
    public static class MethodBaseExtensions
    {
        /// <summary>
        /// Returns the original method from an async method if it exists.
        /// </summary>
        /// <param name="method">The method to retrieve the original from.</param>
        /// <returns>The original method if found; otherwise, the input method.</returns>
        public static MethodBase GetAsyncOriginal(this MethodBase method)
        {
            var result = method;
            
            if (method != null
            && method.DeclaringType != null
            && method.DeclaringType.GetInterfaces().Any(i => i == typeof(IAsyncStateMachine)))
            {
                var generatedType = method.DeclaringType;
                var originalType = generatedType.DeclaringType;
                
                if (originalType != null)
                {
                    result = originalType.GetMethods(BindingFlags.Instance
                    | BindingFlags.Static
                    | BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.DeclaredOnly)
                                  .First(m => m.GetCustomAttribute<AsyncStateMachineAttribute>()?.StateMachineType == generatedType);
                }
            }
            return result;
        }
    }
}
