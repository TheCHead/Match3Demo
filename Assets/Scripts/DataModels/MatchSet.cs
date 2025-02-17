using System.Collections.Generic;
using UnityEngine;
using static MatchPatterns;

namespace Scripts.DataModels
{
    public struct MatchSet
    {
        public List<MatchBatch> batches;
    }

    public struct MatchBatch
    {
        public HashSet<Vector2Int> matches;
        public GemTypeSO gemType;
        public MatchType type;

        public MatchBatch(MatchType type, GemTypeSO gemType)
        {
            this.type = type;
            this.gemType = gemType;
            matches = new();
        }
    }
}

