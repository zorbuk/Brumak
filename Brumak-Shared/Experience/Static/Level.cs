namespace Brumak_Shared.Experience.Static
{
    public struct LevelInfo
    {
        public int Level;
        public long CurrentXp;
        public long MinXp;
        public long MaxXp;
        public long XpToNext;
    }

    public static class Level
    {
        private const int MAX_LEVEL = 40;
        private const double BASE_XP = 100.0;
        private const double FACTOR = 1.5;

        public static LevelInfo GetLevelInfo(long experience)
        {
            double xp = BASE_XP;
            long accumulated = 0;

            for (int level = 1; level <= MAX_LEVEL; level++)
            {
                long needed = (long)xp;
                long next = accumulated + needed;

                if (experience < next)
                {
                    return new LevelInfo
                    {
                        Level = level,
                        CurrentXp = experience,
                        MinXp = accumulated,
                        MaxXp = next,
                        XpToNext = next - experience
                    };
                }

                accumulated = next;
                xp *= FACTOR;
            }

            return new LevelInfo
            {
                Level = MAX_LEVEL,
                CurrentXp = experience,
                MinXp = accumulated,
                MaxXp = accumulated,
                XpToNext = 0
            };
        }

        public static int GetLevelFromExperience(long experience)
        {
            double xp = BASE_XP;
            long accumulated = 0;

            for (int level = 1; level <= MAX_LEVEL; level++)
            {
                accumulated += (long)xp;

                if (experience < accumulated)
                    return level;

                xp *= FACTOR;
            }

            return MAX_LEVEL;
        }

        public static double GetProgress(long experience)
        {
            var info = GetLevelInfo(experience);
            long range = info.MaxXp - info.MinXp;
            return range <= 0 ? 1.0 : (double)(experience - info.MinXp) / range;
        }
    }
}