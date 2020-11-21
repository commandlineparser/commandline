using System;
using System.Collections.Generic;
using Xunit;
using CommandLine.Core;
using CommandLine.Text;

namespace CommandLine.Csharp8.Tests
{
    public class ReflectionExtensionsTests
    {
        [Fact]
        public void GetUsageDataDoesNotExplodeWhenUsedWithNullableReferenceTypesUsingTheUsageAttribute()
        {
            typeof(NullableReferenceTypeOptions).GetUsageData();
        }
    }

    public class NullableReferenceTypeOptions
    {
        [Option(HelpText = "Define a string value here.")]
        public string? StringValue { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples 
        {
            get 
            {
                yield return new Example("Run on current directory", new NullableReferenceTypeOptions());
            }
        }
    }
}
