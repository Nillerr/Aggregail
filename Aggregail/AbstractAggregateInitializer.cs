using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Aggregail
{
    internal static class AbstractAggregateInitializer
    {
        private static int _isInitialized;
        
        public static void Initialize()
        {
            if (Interlocked.Exchange(ref _isInitialized, 1) == 1)
            {
                return;
            }
            
            var loadedAssemblies = new HashSet<string>();
            
            var assemblyNames = new Stack<AssemblyName>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                assemblyNames.Push(assembly.GetName());
            }
            
            while (assemblyNames.TryPop(out var assemblyName))
            {
                var assemblyNameString = assemblyName.FullName;
                if (!loadedAssemblies.Add(assemblyNameString))
                {
                    continue;
                }

                try
                {
                    var assembly = Assembly.Load(assemblyName);
                    
                    InitializeAbstractAggregateDescedants(assembly);
                    
                    foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
                    {
                        assemblyNames.Push(referencedAssemblyName);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private static void InitializeAbstractAggregateDescedants(Assembly assembly)
        {
            foreach (var exportedType in assembly.GetExportedTypes())
            {
                if (exportedType.IsDescendantOfAbstractAggregate())
                {
                    RunClassConstructors(exportedType);
                }
            }
        }

        private static bool IsDescendantOfAbstractAggregate(this Type type)
        {
            return type.IsClass && type.IsConstructedGenericTypeOf(typeof(AbstractAggregate<,>));
        }

        private static bool IsConstructedGenericTypeOf(this Type type, Type genericTypeDefinition)
        {
            var currentType = type;
            while (currentType.BaseType != null)
            {
                var baseType = currentType.BaseType;
                
                if (baseType.IsConstructedGenericType && baseType.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return true;
                }

                currentType = baseType;
            }

            return false;
        }

        private static void RunClassConstructors(Type type)
        {
            var currentType = type;
            while (currentType != null)
            {
                RuntimeHelpers.RunClassConstructor(currentType.TypeHandle);
                currentType = currentType.BaseType;
            }
        }
    }
}