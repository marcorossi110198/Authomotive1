using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Locator<TLocatedType>
{
    private Dictionary<Type, TLocatedType> _myLocatedObjects;

    public Locator()
    {
        _myLocatedObjects = new Dictionary<Type, TLocatedType>();
    }

    public void Add<T>(TLocatedType instance) where T : TLocatedType
    {
        Type type = typeof(T);
        _myLocatedObjects[type] = instance;
    }

    public T Get<T>() where T : TLocatedType
    {
        Type type = typeof(T);
        
        return (T)_myLocatedObjects[type];
    }

    public List<TLocatedType> GetAll()
    {
        return _myLocatedObjects.Values.ToList();
    }
}
