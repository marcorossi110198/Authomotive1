using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClusterAudi
{
    public class Broadcaster : IBroadcaster
    {
        private Dictionary<Type, List<object>> _eventsDictionary;

        public Broadcaster()
        {
            _eventsDictionary = new Dictionary<Type, List<object>>();
        }

        public void Add<T>(Action<T> arg)
        {
            Type type = typeof(T);

            if (!_eventsDictionary.ContainsKey(type))
            {
                _eventsDictionary[type] = new List<object>();
            }

            _eventsDictionary[type].Add(arg);
        }

        public void Remove<T>(Action<T> arg)
        {
            Type type = typeof(T);
            if (_eventsDictionary.ContainsKey(type))
            {
                _eventsDictionary[type].Remove(arg);
            }
        }

        public void Broadcast<T>(T arg)
        {
            Type type = typeof(T);
            if (_eventsDictionary.ContainsKey(type))
            {
                for (int i = 0; i < _eventsDictionary[type].Count; i++)
                {
                    try
                    {
                        (_eventsDictionary[type][i] as Action<T>).Invoke(arg);
                    }
                    catch (Exception e)
                    {
                        // Prevent exception from blocking the whole thing
                        Debug.LogError(e.Message);
                    }
                }
            }
        }
    }
}