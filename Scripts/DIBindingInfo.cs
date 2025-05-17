using System;
using System.Collections.Generic;

namespace MiniDi
{
    public class DIBindingInfo
    {
        public Func<object> Factory;
        public Lifetime Lifetime;
        public object CachedInstance;
        public Queue<object> Pool;
        public WeakReference WeakRef;
    }
}

