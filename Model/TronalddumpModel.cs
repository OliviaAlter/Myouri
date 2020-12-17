// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DiscordBot.Model
{
    public class Self : IEquatable<Self>
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }

        public bool Equals(Self other)
        {
            throw new NotImplementedException();
        }
    }

    public class Links : IEquatable<Links>
    {
        [JsonPropertyName("self")]
        public Self Self { get; set; }

        public bool Equals(Links other)
        {
            throw new NotImplementedException();
        }
    }

    public class Author : IEquatable<Author>
    {
        [JsonPropertyName("author_id")]
        public string AuthorId { get; set; }

        [JsonPropertyName("bio")]
        public object Bio { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("_links")]
        public Links Links { get; set; }

        public bool Equals(Author other)
        {
            throw new NotImplementedException();
        }
    }

    public class Self2
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }
    }

    public class Links2
    {
        [JsonPropertyName("self")]
        public Self2 Self { get; set; }
    }

    public class Source : IEquatable<Source>
    {
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("filename")]
        public object Filename { get; set; }

        [JsonPropertyName("quote_source_id")]
        public string QuoteSourceId { get; set; }

        [JsonPropertyName("remarks")]
        public object Remarks { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("_links")]
        public Links2 Links { get; set; }

        public bool Equals(Source other)
        {
            throw new NotImplementedException();
        }
    }

    public class Embedded
    {
        [JsonPropertyName("author")]
        public List<Author> Author { get; set; }

        [JsonPropertyName("source")]
        public List<Source> Source { get; set; }
    }

    public class Self3
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }
    }

    public class Links3
    {
        [JsonPropertyName("self")]
        public Self3 Self { get; set; }
    }

    public class Root : IEquatable<Root>
    {
        [JsonPropertyName("appeared_at")]
        public DateTime AppearedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("quote_id")]
        public string QuoteId { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("_embedded")]
        public Embedded Embedded { get; set; }

        [JsonPropertyName("_links")]
        public Links3 Links { get; set; }

        public bool Equals(Root other)
        {
            throw new NotImplementedException();
        }
    }
}