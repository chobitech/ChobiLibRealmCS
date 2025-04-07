namespace ChobiLibRealm;

using System.Diagnostics;
using Realms;
using Realms.Schema;

public class ChobiRealm(string realmFilePath, ulong schemaVersion, IList<Type> schemeTypes) : IDisposable
{
    internal static readonly Type roType = typeof(RealmObject);


    public string RealmFilePath { get; private set; } = realmFilePath;
    public ulong SchemaVersion { get; private set; } = schemaVersion;

    public List<Type> Schemes { get; private set; } = [..schemeTypes];


    private RealmConfiguration? _configuration;

    public RealmConfiguration Configuration
    {
        get
        {
            _configuration ??= CreateConfiguration();
            return _configuration;
        }
    }


    private Realm? _realm;
    public Realm Realm
    {
        get
        {
            _realm ??= Realm.GetInstance(Configuration);
            return _realm;
        }
    }


    public virtual void DeleteAll()
    {
        _realm?.Dispose();
        _realm = null;
        
        Realm.DeleteRealm(Configuration);

        _configuration = null;
    }



    protected virtual RealmConfiguration CreateConfiguration()
    {
        var config = new RealmConfiguration(RealmFilePath)
        {
            SchemaVersion = SchemaVersion,
        };

        var sBuilder = new RealmSchema.Builder();

        foreach (var t in Schemes)
        {
            if (roType.IsAssignableFrom(t))
            {
                sBuilder.Add(t);
            }
        }

        config.Schema = sBuilder.Build();

        return config;
    }


    public T With<T>(Func<Realm, T> func) => func(Realm);
    public async Task<T> WithAsync<T>(Func<Realm, T> func) => await Task.Run(() => func(Realm));
    public void With(Action<Realm> action) => action(Realm);
    public async Task WithAsync(Action<Realm> action) => await Task.Run(() => action(Realm));


    public T WithTransaction<T>(Func<Realm, T> func)
    {
        if (Realm.IsInTransaction)
        {
            return func(Realm);
        }
        else
        {
            var tr = Realm.BeginWrite();
            try
            {
                var result = func(Realm);
                tr.Commit();
                return result;
            }
            catch (Exception ex)
            {
                tr.Rollback();
                Debug.WriteLine(ex);
                throw;
            }
        }
    }

    public void WithTransaction(Action<Realm> action) => WithTransaction<object?>(r =>
    {
        action(r);
        return null;
    });

    public async Task<T> WithTransactionAsync<T>(Func<Realm, T> func) => await Task.Run(() => WithTransaction(func));

    public async Task WithTransactionAsync(Action<Realm> action) => await WithTransactionAsync<object?>(r =>
    {
        action(r);
        return null;
    });



    public virtual void Dispose()
    {
        _realm?.Dispose();
        _realm = null;
        _configuration = null;
    }
}
