namespace ZLogger.Internal
{
    internal static class CallerArgumentExpressionParser
    {
        public static ReadOnlySpan<char> GetParameterizedName(ReadOnlySpan<char> expressionString)
        {
            var lastDotPos = -1;
            var lastOpenParenthesisPos = -1;
            for (var i = 0; i < expressionString.Length; i++)
            {
                var ch = expressionString[i];
                switch (ch)
                {
                    case '"':
                        // escaped 
                        var quoteCharCount = 1;
                        while (i + quoteCharCount < expressionString.Length)
                        {
                            if (expressionString[i + quoteCharCount] != '"')
                            {
                                break;
                            }
                            quoteCharCount++;
                        }

                        i += quoteCharCount; // Skip continuous quotes

                        switch (quoteCharCount)
                        {
                            case 1:
                                SkipStringLiteral(ref i, expressionString);
                                break;
                            case 2:
                                // Empty string
                                break;
                            case >= 3:
                                SkipRawStringLiteral(ref i, quoteCharCount, expressionString);
                                break;
                        }
                        break;
                    case '@':
                        if (i + 1 < expressionString.Length && expressionString[i + 1] == '"')
                        {
                            i += 2; // Skip @"
                            SkipVerbatimStringLiteral(ref i, expressionString);
                        }

                        break;
                    case '.':
                        lastDotPos = i;
                        break;
                    case '(':
                        lastOpenParenthesisPos = i;
                        break;
                }
            }
            
            var start = lastDotPos >= 0 ? lastDotPos + 1 : 0;
            var last = lastOpenParenthesisPos >= 0 ? lastOpenParenthesisPos - 1 : expressionString.Length - 1;
            var count = last - start + 1;
            return expressionString.Slice(start, count);
        }
        
        static void SkipStringLiteral(ref int offset, ReadOnlySpan<char> expressionString)
        {
            while (offset < expressionString.Length)
            {
                var ch = expressionString[offset];
                switch (ch)
                {
                    case '\\':
                        if (offset + 1 < expressionString.Length && expressionString[offset + 1] == '"')
                        {
                            offset += 2; // skip escaped double quote
                        }
                        break;
                    case '"':
                        offset++;
                        return;
                }
                offset++;
            }
        }
        
        static void SkipVerbatimStringLiteral(ref int offset, ReadOnlySpan<char> expressionString)
        {
            while (offset < expressionString.Length)
            {
                var ch = expressionString[offset];
                if (ch == '"')
                {
                    if (offset + 1 < expressionString.Length && expressionString[offset + 1] == '"')
                    {
                        offset += 2; // skip escaped double quote
                    }
                    else
                    {
                        offset++;
                        return;
                    }
                }
                offset++;
            }
        }

        static void SkipRawStringLiteral(ref int offset, int quoteCount, ReadOnlySpan<char> expressionString)
        {
            var closeQuoteCount = 0;
            while (offset < expressionString.Length)
            {
                if (expressionString[offset] == '"')
                {
                    closeQuoteCount++;
                    if (closeQuoteCount >= quoteCount)
                    {
                        offset++;
                        return;
                    }
                }
                else
                {
                    closeQuoteCount = 0;
                }
                offset++;
            }
        }
    }    
}
