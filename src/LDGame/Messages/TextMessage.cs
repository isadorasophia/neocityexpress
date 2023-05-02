using Bang.Components;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace LDGame.Messages
{
    public struct ParsedMessage
    {
        public string Message;
        public string Parsed;

        public int MatchedIndex;
    }

    internal readonly struct TextMessage : IMessage
    {
        public readonly bool Clear { get; init; } = false;

        public readonly bool IsOurs { get; init; } = false;

        public readonly string Content { get; init; } = string.Empty;

        public string Sender { get; init; } = string.Empty;

        public readonly ParsedMessage[]? Choices { get; init; } = null;

        public TextMessage() { }

        public static TextMessage CreateClear() => new() { Clear = true };

        public static TextMessage Create(string content, bool isOurs, string sender) => 
            new() { Content = content, IsOurs = isOurs, Sender = sender };

        private readonly static Regex _escapePonctuation = new(@"[^a-zA-Z0-9]");

        public static string EscapePunctuation(string text) => _escapePonctuation.Replace(text, string.Empty);

        public static TextMessage Create(string content, ImmutableArray<string> choices, string sender)
        {
            ParsedMessage[] parsedChoices = new ParsedMessage[choices.Length];
            for (int i = 0; i < choices.Length; i++)
            {
                string choice = choices[i];

                parsedChoices[i].Message = choice;
                parsedChoices[i].MatchedIndex = 0;
                parsedChoices[i].Parsed = EscapePunctuation(choice);
            }

            return new() { Content = content, Choices = parsedChoices, Sender = sender };
        }

        /// <summary>
        /// Convert the current text message to a single choice.
        /// </summary>
        public TextMessage AsChoice()
        {
            return new() { Choices = new ParsedMessage[] { 
                new() { Message = Content, MatchedIndex = 0, Parsed = EscapePunctuation(Content) } } };
        }
    }
}
