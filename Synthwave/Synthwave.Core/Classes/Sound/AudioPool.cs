using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthwave.Core.Classes.Sound;

public class AudioPool
{
    private readonly Dictionary<string, Queue<object>> _pool = [];

    public object Get(string key)
    {
        if (_pool.TryGetValue(key, out var queue) && queue.Count > 0)
            return queue.Dequeue();

        return null; // backend will create new instance
    }

    public void Return(string key, object instance)
    {
        if (!_pool.ContainsKey(key))
            _pool[key] = new Queue<object>();

        _pool[key].Enqueue(instance);
    }
}
