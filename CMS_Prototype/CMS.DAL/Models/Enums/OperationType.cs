namespace CMS.DAL.Models
{
    public enum OperationType : int
    {
        EqualTo = 0,
        NotEqualTo = 1,
        Like = 2,
        NotLike = 3,
        In = 4,
        NotIn = 5,
        GreaterOrEqualTo = 6,
        LessOrEqualTo = 7,
        GreaterThan = 8,
        LessThan = 9,
        BetweenInclusive = 10,
        StartsWith = 11
    }
}
