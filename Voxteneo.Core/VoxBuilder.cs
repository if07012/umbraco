using Castle.Core.Internal;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Voxteneo.Core
{
    public class VoxProxyBuilder : IProxyBuilder
    {
        public VoxProxyBuilder()
        {
            ModuleScope = new ModuleScope();
        }

        public ILogger Logger { get; set; }


        public ModuleScope ModuleScope { get; }

        public Type CreateClassProxyType(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options)
        {
            AssertValidType(classToProxy);
            AssertValidTypes(additionalInterfacesToProxy);

            var generator = new VoxClassProxyWithTargetGenerator(ModuleScope, classToProxy) { Logger = Logger };
            return generator.GenerateCode(additionalInterfacesToProxy, options);
        }

        public Type CreateClassProxyTypeWithTarget(Type classToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options)
        {
            AssertValidType(classToProxy);
            AssertValidTypes(additionalInterfacesToProxy);
            var generator = new VoxClassProxyWithTargetGenerator(ModuleScope, classToProxy, additionalInterfacesToProxy, options)
            { Logger = Logger };
            return generator.GetGeneratedType();
        }

        private void AssertValidType(Type target)
        {
            if (target.IsGenericTypeDefinition)
            {
                throw new GeneratorException("Type " + target.FullName + " is a generic type definition. " +
                                             "Can not create proxy for open generic types.");
            }
            if (IsPublic(target) == false && IsAccessible(target) == false)
            {
                throw new GeneratorException("Type " + target.FullName + " is not visible to DynamicProxy. " +
                                             "Can not create proxy for types that are not accessible. " +
                                             "Make the type public, or internal and mark your assembly with " +
                                             "[assembly: InternalsVisibleTo(InternalsVisible.ToDynamicProxyGenAssembly2)] attribute.");
            }
        }

        private bool IsAccessible(Type target)
        {
            return IsInternal(target) && IsInternalToDynamicProxy(target.Assembly);
        }

        private static readonly IDictionary<Assembly, bool> InternalsToDynProxy = new Dictionary<Assembly, bool>();

        private static readonly Lock InternalsToDynProxyLock = Lock.Create();

        public static bool IsInternalToDynamicProxy(Assembly asm)
        {
            using (var locker = InternalsToDynProxyLock.ForReadingUpgradeable())
            {
                if (InternalsToDynProxy.ContainsKey(asm))
                {
                    return InternalsToDynProxy[asm];
                }

                locker.Upgrade();

                if (InternalsToDynProxy.ContainsKey(asm))
                {
                    return InternalsToDynProxy[asm];
                }

                var internalsVisibleTo = asm.GetAttributes<InternalsVisibleToAttribute>();
                var found = internalsVisibleTo.Any(VisibleToDynamicProxy);

                InternalsToDynProxy.Add(asm, found);
                return found;
            }
        }

        private static bool VisibleToDynamicProxy(InternalsVisibleToAttribute attribute)
        {
            return attribute.AssemblyName.Contains(ModuleScope.DEFAULT_ASSEMBLY_NAME);
        }

        private static bool IsInternal(Type target)
        {
            var isTargetNested = target.IsNested;
            var isNestedAndInternal = isTargetNested && (target.IsNestedAssembly || target.IsNestedFamORAssem);
            var isInternalNotNested = target.IsVisible == false && isTargetNested == false;

            return isInternalNotNested || isNestedAndInternal;
        }

        private bool IsPublic(Type target)
        {
            return target.IsPublic || target.IsNestedPublic;
        }

        private void AssertValidTypes(IEnumerable<Type> targetTypes)
        {
            if (targetTypes == null)
                return;

            foreach (var t in targetTypes)
            {
                AssertValidType(t);
            }
        }

        public Type CreateInterfaceProxyTypeWithoutTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options)
        {
            AssertValidType(interfaceToProxy);
            AssertValidTypes(additionalInterfacesToProxy);

            var generator = new InterfaceProxyWithoutTargetGenerator(ModuleScope, interfaceToProxy) { Logger = Logger };
            return generator.GenerateCode(typeof(object), additionalInterfacesToProxy, options);
        }

        public Type CreateInterfaceProxyTypeWithTarget(Type interfaceToProxy, Type[] additionalInterfacesToProxy, Type targetType, ProxyGenerationOptions options)
        {
            AssertValidType(interfaceToProxy);
            AssertValidTypes(additionalInterfacesToProxy);

            var generator = new InterfaceProxyWithTargetGenerator(ModuleScope, interfaceToProxy) { Logger = Logger };
            return generator.GenerateCode(targetType, additionalInterfacesToProxy, options);
        }

        public Type CreateInterfaceProxyTypeWithTargetInterface(Type interfaceToProxy, Type[] additionalInterfacesToProxy, ProxyGenerationOptions options)
        {
            AssertValidType(interfaceToProxy);
            AssertValidTypes(additionalInterfacesToProxy);

            var generator = new InterfaceProxyWithTargetInterfaceGenerator(ModuleScope, interfaceToProxy) { Logger = Logger };
            return generator.GenerateCode(interfaceToProxy, additionalInterfacesToProxy, options);
        }
    }
}
