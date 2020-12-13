using System;

namespace DiscordBot.Services
{
    internal class LevenshteinDistanceComputingService
    {
        public static int DistanceComputing(string s, string t)
        {
            var n = s.Length;
            var m = t.Length;
            var d = new int[n + 1, m + 1];
            //verify argument
            if (n == 0) return m;

            if (m == 0) return n;

            //Initialize arrays.
            for (var i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (var j = 0; j <= m; d[0, j] = j++)
            {
            }

            //begin looping
            for (var i = 1; i <= n; i++)
            for (var j = 1; j <= m; j++)
            {
                //computing cost start here
                var cost = t[j - 1] == s[i - 1] ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(
                        d[i - 1, j] + 1,
                        d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }

            //return cost
            return d[n, m];
        }

        public static int Evaluation(string a, string b)
        {
            /*
            List<string[]> list = new List<string[]>
            {
                new[] {a, b}
            };
            */
            var cost = DistanceComputing(a, b);
            return cost;
        }
    }
}