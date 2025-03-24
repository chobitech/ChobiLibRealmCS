namespace ChobiLibRealm;

using System.Collections;
using System.Reflection;
using Realms;

public static class ChobiLibRealmSub
{
    internal static readonly Type _cascadeDeleteAttrType = typeof(CascadeDeleteAttribute);

    internal static readonly Type _iListType = typeof(IList);
    internal static readonly Type _iListGenericType = typeof(IList<>);

    public static void DeleteCascade(this Realm realm, object? obj)
    {
        if (obj is RealmObject rObj)
        {
            var type = rObj.GetType();
            var props = type.GetProperties().Where(p => p.GetCustomAttribute<CascadeDeleteAttribute>() != null).ToList();

            foreach (var p in props)
            {
                var value = p.GetValue(rObj);
                var pType = p.PropertyType;

                if (value is RealmObject pObj)
                {
                    realm.Remove(pObj);
                }
                else if (value is IEnumerable elms)
                {
                    foreach (var elm in elms)
                    {
                        realm.DeleteCascade(elm);
                    }
                }
            }

            realm.Remove(rObj);
        }        
    }

    public static void DeleteCascade(this Realm realm, params RealmObject[] targets)
    {
        foreach (var obj in targets)
        {
            realm.DeleteCascade(obj: obj);
        }
    }
}