namespace ChobiLibRealm;

using System.Numerics;
using MongoDB.Bson;
using Realms;

public class RealmBigInteger : RealmObject
{
    public static RealmBigInteger CreateNew(BigInteger? bigInteger = null) => new()
    {
        Id = ObjectId.GenerateNewId(),
        BigInteger = bigInteger ?? BigInteger.Zero
    };


    [PrimaryKey]
    public ObjectId Id { get; set; }

    public byte[] BigIntBytes { get; set; } = [];


    private BigInteger? _bigInt;

    [Ignored]
    public BigInteger BigInteger
    {
        get
        {
            _bigInt ??= new(BigIntBytes);
            return _bigInt.Value;
        }
        set
        {
            BigIntBytes = value.ToByteArray();
            _bigInt = null;
        }
    }

    public static implicit operator BigInteger(RealmBigInteger rbi) => rbi.BigInteger;
    public static explicit operator RealmBigInteger(BigInteger bi) => CreateNew(bi);
}