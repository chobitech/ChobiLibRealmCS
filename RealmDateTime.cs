namespace ChobiLibRealm;

using MongoDB.Bson;
using Realms;

public class RealmDateTime : RealmObject
{
    public static RealmDateTime CreateNew(DateTime? dateTime = null) => new()
    {
        Id = ObjectId.GenerateNewId(),
        DateTime = dateTime ?? DateTime.Now
    };


    [PrimaryKey]
    public ObjectId Id { get; set; }

    [Indexed]
    public long DateTimeTicksUtc { get; set; }


    private DateTime? _dateTime;

    [Ignored]
    public DateTime DateTime
    {
        get
        {
            _dateTime ??= new DateTime(DateTimeTicksUtc).ToLocalTime();
            return _dateTime.Value;
        }
        set
        {
            DateTimeTicksUtc = value.ToUniversalTime().Ticks;
            _dateTime = null;
        }
    }


    public static implicit operator DateTime(RealmDateTime rdt) => rdt.DateTime;

    public static explicit operator RealmDateTime(DateTime dt) => CreateNew(dt);
}
