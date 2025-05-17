using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace MiniDi
{
    public abstract class BaseContainer : MonoBehaviour
    {
        protected BaseContainer Parent;
        private readonly Dictionary<Type, DIBindingInfo> _scopeBase = new();
        public List<InjectionInstallerBase> injectionInstallers = new List<InjectionInstallerBase>();
        public List<InjectionInstallerBase> injectionInstallersPrefabs = new List<InjectionInstallerBase>();
        protected virtual void Awake()
        {
        }
        public virtual void InstallBindings()
        {
            foreach (var installer in injectionInstallers)
            {
                installer.SetContainer(this);
                installer.InstallBindings();
            }
            foreach (var installerPrefab in injectionInstallersPrefabs)
            {
                var newInstallerPrefab = Instantiate(installerPrefab);
                newInstallerPrefab.transform.SetParent(transform);
                newInstallerPrefab.transform.position = Vector3.zero;
                newInstallerPrefab.SetContainer(this);
                newInstallerPrefab.InstallBindings();
            }
        }
        public virtual void SetParent(BaseContainer parent)
        {
            Parent = parent;
        }

        public void Bind<TInterface, TConcrete>(
        Lifetime lifetime = Lifetime.Singleton) where TConcrete : TInterface, new()
        {
            Bind<TInterface>(() => new TConcrete(), lifetime);
        }
        public void Bind<TInterface>(
        Func<object> factory,
        Lifetime lifetime = Lifetime.Singleton)
        {
            var binding = new DIBindingInfo
            {
                Factory = factory,
                Lifetime = lifetime
            };

            RegisterBinding(typeof(TInterface), binding);
        }


        public void Bind<TInterface>(
        TInterface instance,
        Lifetime lifetime = Lifetime.Singleton)
        {
            var binding = new DIBindingInfo
            {
                Factory = () => instance,
                CachedInstance = instance,
                Lifetime = lifetime
            };

            RegisterBinding(typeof(TInterface), binding);
        }

        private void RegisterBinding(Type type, DIBindingInfo binding)
        {
            _scopeBase[type] = binding;
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            if (TryGetBinding(type, out var binding))
            {
                var instance = GetInstance(binding);
                return instance;
            }

            throw new Exception($"Type {type.Name} not registered in any scope.");
        }

        internal virtual bool TryGetBinding(Type type, out DIBindingInfo binding)
        {
            if (_scopeBase.TryGetValue(type, out binding))
                return true;

            if (Parent != null)
                return Parent.TryGetBinding(type, out binding);

            binding = null;
            return false;
        }

        private object GetInstance(DIBindingInfo binding)
        {
            switch (binding.Lifetime)
            {
                case Lifetime.Singleton:
                case Lifetime.Lazy:
                    if (binding.CachedInstance == null)
                        binding.CachedInstance = binding.Factory();
                    return binding.CachedInstance;

                case Lifetime.Transient:
                    return binding.Factory();

                case Lifetime.Weak:
                    if (binding.WeakRef == null || !binding.WeakRef.IsAlive)
                    {
                        var instance = binding.Factory();
                        binding.WeakRef = new WeakReference(instance);
                        return instance;
                    }
                    return binding.WeakRef.Target;

                case Lifetime.Pooled:
                    if (binding.Pool == null)
                        binding.Pool = new Queue<object>();

                    if (binding.Pool.Count > 0)
                        return binding.Pool.Dequeue();

                    return binding.Factory();

                default:
                    throw new Exception($"Unsupported lifetime: {binding.Lifetime}");
            }
        }

        protected void InjectDependencies(object target)
        {
            if (target == null) return;

            var targetType = target.GetType();
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var field in targetType.GetFields(flags))
            {
                if (Attribute.IsDefined(field, typeof(InjectAttribute)))
                {
                    var value = Resolve(field.FieldType);
                    field.SetValue(target, value);
                }
            }

            foreach (var property in targetType.GetProperties(flags))
            {
                if (Attribute.IsDefined(property, typeof(InjectAttribute)) && property.CanWrite)
                {
                    var value = Resolve(property.PropertyType);
                    property.SetValue(target, value);
                }
            }
        }

        public void ReturnToPool<T>(T instance)
        {
            var type = typeof(T);
            if (_scopeBase.TryGetValue(type, out var binding) && binding.Lifetime == Lifetime.Pooled)
            {
                binding.Pool ??= new Queue<object>();
                binding.Pool.Enqueue(instance);
            }
        }
        public void ClearSceneScope() => _scopeBase.Clear();
    }
}