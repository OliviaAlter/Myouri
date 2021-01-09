// Root myDeserializedClass = JsonSerializer.Deserialize<RootDictionary>(myJsonResponse);

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DiscordBot.Model
{
    public class Phonetic
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("audio")]
        public string Audio { get; set; }
    }

    public class WordDefinition 
    {
        [JsonPropertyName("definition")]
        public string Definition { get; set; }

        [JsonPropertyName("example")]
        public string Example { get; set; }

        [JsonPropertyName("synonyms")]
        public List<string> Synonyms { get; set; }

        [JsonPropertyName("antonyms")]
        public List<string> Antonyms { get; set; }

    }

    public class Meaning
    {
        [JsonPropertyName("partOfSpeech")]
        public string PartOfSpeech { get; set; }

        [JsonPropertyName("definitions")]
        public List<WordDefinition> Definitions { get; set; }
    }

    public class InvalidWord
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("resolution")]
        public string Resolution { get; set; }
    }

    public class RootDictionary
    {
        [JsonPropertyName("word")]
        public string Word { get; set; }

        [JsonPropertyName("phonetics")]
        public List<Phonetic> Phonetics { get; set; }

        [JsonPropertyName("meanings")]
        public List<Meaning> Meanings { get; set; }
    }
}