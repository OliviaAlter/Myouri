using System;

namespace DiscordBot.Model
{
    public class JokeModel : IEquatable<JokeModel>
    {
        public string Category { get; set; }
        public string Joke { get; set; }
        public string Lang { get; set; }
        public Flags Flags { get; set; }
        public bool Equals(JokeModel other)
        {
            throw new NotImplementedException();
        }
    }

    public class Flags
    {
        public bool NSFW { get; set; }
        public bool Religious { get; set; }
        public bool Political { get; set; }
        public bool Racist { get; set; }
        public bool Sexist { get; set; }
    }
}