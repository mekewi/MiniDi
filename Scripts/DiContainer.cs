using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace MiniDi
{
    public class DiContainer
    {
        private readonly Dictionary<Type, DIBindingInfo> _projectScope = new();
        private readonly Dictionary<Type, DIBindingInfo> _sceneScope = new();
        private readonly Dictionary<GameObject, Dictionary<Type, DIBindingInfo>> _gameObjectScopes = new();

        public void Bind<TInterface, TConcrete>(
        Lifetime lifetime = Lifetime.Singleton,
        Scope scope = Scope.Project,
        GameObject contextGO = null) where TConcrete : TInterface, new()
        {
            Bind<TInterface>(() => new TConcrete(), lifetime, scope, contextGO);
        }


        public void Bind<TInterface>(
        Func<object> factory,
        Lifetime lifetime = Lifetime.Singleton,
        Scope scope = Scope.Project,
        GameObject contextGO = null)
        {
            var binding = new DIBindingInfo
            {
                Factory = factory,
                Lifetime = lifetime
            };

            RegisterBinding(typeof(TInterface), binding, scope, contextGO);
        }


        public void Bind<TInterface>(
        TInterface instance,
        Lifetime lifetime = Lifetime.Singleton,
        Scope scope = Scope.Project,
        GameObject contextGO = null)
        {
            var binding = new DIBindingInfo
            {
                Factory = () => instance,
                CachedInstance = instance,
                Lifetime = lifetime
            };

            RegisterBinding(typeof(TInterface), binding, scope, contextGO);
        }

        private void RegisterBinding(Type type, DIBindingInfo binding, Scope scope, GameObject contextGO)
        {
            switch (scope)
            {
                case Scope.Project:
                    _projectScope[type] = binding;
                    break;
                case Scope.Scene:
                    _sceneScope[type] = binding;
                    break;
                case Scope.GameObject:
                    if (contextGO == null) throw new ArgumentNullException(nameof(contextGO));
                    if (!_gameObjectScopes.ContainsKey(contextGO))
                        _gameObjectScopes[contextGO] = new();
                    _gameObjectScopes[contextGO][type] = binding;
                    break;
            }
        }

        public T Resolve<T>(GameObject contextGO = null)
        {
            return (T)Resolve(typeof(T), contextGO);
        }

        public object Resolve(Type type, GameObject contextGO = null)
        {
            if (TryGetBinding(type, contextGO, out var binding))
            {
                var instance = GetInstance(binding);
                //InjectDependencies(instance);
                return instance;
            }

            throw new Exception($"Type {type.Name} not registered in any scope.");
        }

        private bool TryGetBinding(Type type, GameObject contextGO, out DIBindingInfo binding)
        {
            binding = null;

            if (contextGO != null &&
                _gameObjectScopes.TryGetValue(contextGO, out var goScope) &&
                goScope.TryGetValue(type, out binding))
            {
                return true;
            }

            if (_sceneScope.TryGetValue(type, out binding))
                return true;

            if (_projectScope.TryGetValue(type, out binding))
                return true;

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

/*        private void InjectDependencies(object instance)
        {
            var fields = instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<InjectAttribute>() != null)
                {
                    var dep = Resolve(field.FieldType);
                    field.SetValue(instance, dep);
                }
            }
        }*/

        public void ReturnToPool<T>(T instance)
        {
            var type = typeof(T);
            if (_projectScope.TryGetValue(type, out var binding) && binding.Lifetime == Lifetime.Pooled)
            {
                binding.Pool ??= new Queue<object>();
                binding.Pool.Enqueue(instance);
            }
        }

        public void ClearSceneScope() => _sceneScope.Clear();
        public void ClearGameObjectScope(GameObject go) => _gameObjectScopes.Remove(go);
    }
}


[RequireComponent(typeof(Rigidbody))]
public class ShipMovement : MonoBehaviour 
{
    [SerializeField] private Vector3 targetPoint;
    [SerializeField] private bool useRandom;
    [SerializeField] private float speed;
    [SerializeField] private float distanceToStop = 1;
    private Vector3 targetDirection;
    private Rigidbody rb;
    private bool allowMove;
    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }
        if (useRandom)
        {
            targetPoint.x = UnityEngine.Random.Range(0, 200f);
            targetPoint.z = UnityEngine.Random.Range(0, 200f);
            targetPoint.y = UnityEngine.Random.Range(0, 200f);
        }
        allowMove = true;
    }

    private void Update()
    {
        if (!allowMove) {
            return;
        }
        targetDirection = targetPoint - transform.position;
        transform.position += targetDirection.normalized * speed * Time.deltaTime;
        if (CheckIfReachedTarget())
        {
            OnReachedTareget();
        }
    }
    private bool CheckIfReachedTarget() 
    {
        return targetDirection.magnitude <= distanceToStop;
    }
    private void OnReachedTareget() 
    {
        allowMove = false;
        rb.linearVelocity = Vector3.zero;
    }
}