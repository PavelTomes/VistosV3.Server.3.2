using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Extensions
{
    internal static class StringBuilderExtensions
    {
        public static void TrimLast(this StringBuilder builder, int howManyCharactersToRemove)
        {
            if (builder.Length >= howManyCharactersToRemove)
            {
                builder.Remove(builder.Length - howManyCharactersToRemove, howManyCharactersToRemove);
            }
        }
    }
}