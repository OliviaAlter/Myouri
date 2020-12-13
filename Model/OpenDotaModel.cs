namespace DiscordBot.Model
{
    public class OpenDotaModel
    {
        public class MyProfile
        {
            public long account_id;
            public string personaname;
        }

        public class RecentMatches
        {
            public long match_id;
            public long player_slot;
            public bool radiant_win;
        }

        public class ParesMatches
        {
            public long match_id;
        }
    }
}