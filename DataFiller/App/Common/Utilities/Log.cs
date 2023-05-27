using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Common.Utilities
{
    public static class Log
    {
        private static string GetMethodName(MethodBase asyncMethod)
        {
            var generatedType = asyncMethod.DeclaringType;
            var originalType = generatedType.DeclaringType;

            if (originalType == null)
            {
                return asyncMethod.Name;
            }

            var matchingMethods =
                from methodInfo in originalType.GetMethods()
                let attr = methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>()
                where attr != null && attr.StateMachineType == generatedType
                select methodInfo;

            var foundMethod = matchingMethods.Single();
            return foundMethod.Name;
        }

        public static void Write(ILogger logger, MethodBase method, params object[] parameters)
        {
            try
            {
                var methodName = GetMethodName(method);

                var infoMessage = $"{methodName} Executed";
                logger.LogInformation(infoMessage);

                if (parameters == null)
                {
                    return;
                }

                var jsonParameters = JsonConvert.SerializeObject(parameters);
                var traceMessage = $"{methodName} Parameters : {jsonParameters}";
                logger.LogTrace(traceMessage);
            }
            catch (Exception e)
            {
            }
        }
        public static void Write(ILogger logger,  string parameters)
        {
            try
            {

                var infoMessage = string.Empty;

                if (parameters == null)
                {
                    return;
                }

                logger.LogTrace(parameters);
            }
            catch (Exception e)
            {
            }
        }
    }
}