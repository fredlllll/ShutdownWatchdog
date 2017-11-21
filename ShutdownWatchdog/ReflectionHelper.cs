using System;
using System.Reflection;

namespace ShutdownWatchdog
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// searches using name, namespace+name and assembly qualified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Type GetType(string name)
        {
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = assembly.GetTypes();
                foreach(Type t in types)
                {
                    if(t.Name == name || t.FullName == name || t.AssemblyQualifiedName == name)
                    {
                        return t;
                    }
                }
            }

            return Type.GetType(name);
        }
    }
}
