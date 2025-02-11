using System.Collections.Generic;



public static class MatchPatterns
{
    public enum MatchType { Horizontal, Vertical, Square, Clover, Tower, Holy };
    public static readonly Dictionary<MatchType, int[,]> Patterns = new Dictionary<MatchType, int[,]>()
    {
        {MatchType.Square, new int[2,2]
        {
            { 1, 1 },
            { 1, 1 }
        }}
        ,
        {MatchType.Tower, new int[3,3]
        {
            { 0, 1, 0},
            { 0, 1, 0},
            { 1, 1, 1}
        }}
        ,
        {MatchType.Clover, new int[3,3]
        {
            { 0, 1, 0},
            { 1, 1, 1},
            { 0, 1, 0}
        }}
        ,
        {MatchType.Holy, new int[4,3]
        {
            { 0, 1, 0},
            { 1, 1, 1},
            { 0, 1, 0},
            { 0, 1, 0}
        }}
    };
}
