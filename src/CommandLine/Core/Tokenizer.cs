// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;
using CSharpx;
using RailwaySharp.ErrorHandling;
using System.Text.RegularExpressions;

namespace CommandLine.Core
{
    static class Tokenizer
    {
        public static Result<IEnumerable<Token>, Error> Tokenize(
            IEnumerable<string> arguments,
            Func<string, NameLookupResult> nameLookup)
        {
            return Tokenizer.Tokenize(arguments, nameLookup, ignoreUnknownArguments:false, allowDashDash:true);
        }

        public static Result<IEnumerable<Token>, Error> Tokenize(
            IEnumerable<string> arguments,
            Func<string, NameLookupResult> nameLookup,
            bool ignoreUnknownArguments,
            bool allowDashDash)
        {
            var errors = new List<Error>();
            Action<Error> onError = errors.Add;

            int consumeNext = 0;
            var tokens = new List<Token>();
            Action<string> addValue = (s => tokens.Add(new Value(s)));
            Action<string> addName = (s => tokens.Add(new Name(s)));

            var enumerator = arguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string arg = enumerator.Current;
                // TODO: Turn this into a switch statement with pattern matching
                if (arg == null)
                {
                    continue;
                }

                if (consumeNext > 0)
                {
                    addValue(arg);
                    consumeNext = consumeNext - 1;
                    continue;
                }

                if (arg == "--")
                {
                    if (allowDashDash)
                    {
                        consumeNext = System.Int32.MaxValue;
                        continue;
                    }
                    else
                    {
                        addValue(arg);
                        continue;
                    }
                }

                if (arg.StartsWith("--"))
                {
                    if (arg.Contains("="))
                    {
                        string[] parts = arg.Substring(2).Split(new char[] { '=' }, 2);
                        if (String.IsNullOrWhiteSpace(parts[0]) || parts[0].Contains(" "))
                        {
                            onError(new BadFormatTokenError(arg));
                            continue;
                        }
                        else
                        {
                            var name = parts[0];
                            var tokenType = nameLookup(name);
                            if (tokenType == NameLookupResult.NoOptionFound)
                            {
                                if (ignoreUnknownArguments)
                                {
                                    continue;
                                }
                                else
                                {
                                    onError(new UnknownOptionError(name));
                                    continue;
                                }
                            }
                            addName(parts[0]);
                            addValue(parts[1]);
                            continue;
                        }
                    }
                    else
                    {
                        var name = arg.Substring(2);
                        var tokenType = nameLookup(name);
                        if (tokenType == NameLookupResult.OtherOptionFound)
                        {
                            addName(name);
                            consumeNext = 1;
                            continue;
                        }
                        else if (tokenType == NameLookupResult.NoOptionFound)
                        {
                            if (ignoreUnknownArguments)
                            {
                                // When ignoreUnknownArguments is true and AutoHelp is true, calling code is responsible for
                                // setting up nameLookup so that it will return a known name for --help, so that we don't skip it here
                                continue;
                            }
                            else
                            {
                                onError(new UnknownOptionError(name));
                                continue;
                            }
                        }
                        else
                        {
                            addName(name);
                            continue;
                        }
                    }
                }

                if (arg == "-")
                {
                    // A single hyphen is always a value (it usually means "read from stdin" or "write to stdout")
                    addValue(arg);
                    continue;
                }

                if (arg.StartsWith("-"))
                {
                    // First option char that requires a value means we swallow the rest of the string as the value
                    // But if there is no rest of the string, then instead we swallow the next argument
                    string chars = arg.Substring(1);
                    int len = chars.Length;
                    if (len > 0 && Char.IsDigit(chars[0]))
                    {
                        // Assume it's a negative number
                        addValue(arg);
                        continue;
                    }
                    for (int i = 0; i < len; i++)
                    {
                        var s = new String(chars[i], 1);
                        var tokenType = nameLookup(s);
                        if (tokenType == NameLookupResult.OtherOptionFound)
                        {
                            addName(s);
                            if (i+1 < len)
                            {
                                addValue(chars.Substring(i+1));
                                break;
                            }
                            else
                            {
                                consumeNext = 1;
                            }
                        }
                        else if (tokenType == NameLookupResult.NoOptionFound)
                        {
                            if (ignoreUnknownArguments)
                            {
                                continue;
                            }
                            else
                            {
                                onError(new UnknownOptionError(s));
                            }
                        }
                        else
                        {
                            addName(s);
                        }
                    }
                    continue;
                }

                // If we get this far, it's a plain value
                addValue(arg);
            }

            return Result.Succeed<IEnumerable<Token>, Error>(tokens.AsEnumerable(), errors.AsEnumerable());
        }

        public static Result<IEnumerable<Token>, Error> ExplodeOptionList(
            Result<IEnumerable<Token>, Error> tokenizerResult,
            Func<string, Maybe<char>> optionSequenceWithSeparatorLookup)
        {
            // TODO: I don't like how this works. I don't want "-s foo;bar baz" to put three values into -s. Let's instead have a third token type, List, besides Name and Value.
            var tokens = tokenizerResult.SucceededWith().Memoize();

            var replaces = tokens.Select((t, i) =>
                optionSequenceWithSeparatorLookup(t.Text)
                    .MapValueOrDefault(sep => Tuple.Create(i + 1, sep),
                        Tuple.Create(-1, '\0'))).SkipWhile(x => x.Item1 < 0).Memoize();

            var exploded = tokens.Select((t, i) =>
                        replaces.FirstOrDefault(x => x.Item1 == i).ToMaybe()
                            .MapValueOrDefault(r => t.Text.Split(r.Item2).Select(Token.Value),
                                Enumerable.Empty<Token>().Concat(new[] { t })));

            var flattened = exploded.SelectMany(x => x);

            return Result.Succeed(flattened, tokenizerResult.SuccessMessages());
        }

        public static Func<
                    IEnumerable<string>,
                    IEnumerable<OptionSpecification>,
                    Result<IEnumerable<Token>, Error>>
            ConfigureTokenizer(
                    StringComparer nameComparer,
                    bool ignoreUnknownArguments,
                    bool enableDashDash)
        {
            return (arguments, optionSpecs) =>
                {
                    var tokens = Tokenizer.Tokenize(arguments, name => NameLookup.Contains(name, optionSpecs, nameComparer), ignoreUnknownArguments, enableDashDash);
                    var explodedTokens = Tokenizer.ExplodeOptionList(tokens, name => NameLookup.HavingSeparator(name, optionSpecs, nameComparer));
                    return explodedTokens;
                };
        }
    }
}
