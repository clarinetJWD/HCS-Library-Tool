Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports DevExpress.Utils
Imports HcsLibraryTool.Extensions.ObjectExtensions

Namespace Extensions.StringExtensions
    <HideModuleName>
    Public Module M_StringExtensions

        Private _StringExtensions As New StringExtensions

#Region "Alpha-numeric"

        ''' <summary>
        ''' Removes all characters from the string, except for letters.
        ''' </summary>
        ''' <param name="inputString"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function MakeAlpha(ByRef inputString As String) As String
            Return _StringExtensions.MakeAlpha(inputString)
        End Function

        ''' <summary>
        ''' Removes all characters from the string, except for letters and numerals.
        ''' </summary>
        ''' <param name="inputString"></param>
        ''' <param name="allowPeriods">Will leave periods/decimal points in the string when true.</param>
        ''' <param name="allowUnderscore">Will leave underscores '_' in the string when true.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function MakeAlphaNumeric(ByRef inputString As String, Optional allowPeriods As Boolean = False, Optional allowUnderscore As Boolean = False) As String
            Return _StringExtensions.MakeAlphaNumeric(inputString, allowPeriods, allowUnderscore)
        End Function

        ''' <summary>
        ''' Removes all characters from the string, except for letters, numerals, and specified characters.
        ''' </summary>
        ''' <param name="inputString"></param>
        ''' <param name="additionalAllowedCharacters">A collection of other characters to allow.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function MakeAlphaNumeric(ByRef inputString As String, additionalAllowedCharacters As IEnumerable(Of Char)) As String
            Return _StringExtensions.MakeAlphaNumeric(inputString, additionalAllowedCharacters)
        End Function

        ''' <summary>
        ''' Removes all non numeric (0, 1, 2, 3, 4, 5, 6, 7, 8, 9) from the string and returns the result.
        ''' </summary>
        ''' <param name="inputString"></param>
        ''' <param name="allowDecimalPoint">Should the return include the decimal point '.'?</param>
        ''' <returns>The string of numerals</returns>
        ''' <remarks></remarks>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function MakeNumeric(ByRef inputString As String, Optional allowDecimalPoint As Boolean = False) As String
            Return _StringExtensions.MakeNumeric(inputString, allowDecimalPoint)
        End Function

#End Region ' Alpha-numeric

#Region "SQL"

        ''' <summary>
        ''' Formats a string for SQL statements (replaces ')
        ''' </summary>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function MakeSQLFriendly(value As String) As String
            Return _StringExtensions.MakeSQLFriendly(value)
        End Function

#End Region ' SQL

#Region "String Encoding and Compression"

        ''' <summary>
        ''' Decodes old GS encrypted values.
        ''' </summary>
        <Extension>
        Public Function GSDecode(ByRef value As String) As String
            Return _StringExtensions.GSDecode(value)
        End Function

        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function Compress(ByRef uncompressedValue As String) As String
            Return _StringExtensions.Compress(uncompressedValue)
        End Function

        <System.Diagnostics.DebuggerStepThrough()>
        Public Function CompressString(uncompressedValue As String) As String
            Return _StringExtensions.CompressString(uncompressedValue)
        End Function

        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function TryDecompress(ByRef base64String As String) As String
            Return _StringExtensions.TryDecompress(base64String)
        End Function

        <System.Diagnostics.DebuggerStepThrough()>
        Public Function TryDecompressString(base64String As String) As String
            Return _StringExtensions.TryDecompressString(base64String)
        End Function

#End Region ' String Encoding and Compression

#Region "Split"

        ''' <summary>
        ''' Splits the string based on a string delimiter
        ''' </summary>
        ''' <param name="input">The string to split</param>
        ''' <param name="delimiter">The delimiter string</param>
        ''' <returns>An array of string objects</returns>
        ''' <remarks></remarks>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function Split(ByVal input As String, ByVal ParamArray delimiter As String()) As String()
            Return _StringExtensions.Split(input, delimiter)
        End Function

        ''' <summary>
        ''' Attempts to split a string into chunks of <paramref name="lengthOfEachChunk"/> characters into an array.
        ''' </summary>
        ''' <param name="inputString">The string to split.</param>
        ''' <param name="lengthOfEachChunk">The length of each resulting chunk.</param>
        ''' <param name="result">The resulting collection of chunks of length <paramref name="lengthOfEachChunk"/> from <paramref name="inputString"/>.
        ''' The final chunk may be a shorter length.</param>
        ''' <returns>True if successful, false otherwise.</returns>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function TryPositionalSplit(ByRef inputString As String, lengthOfEachChunk As Integer, ByRef result() As String) As Boolean
            Return _StringExtensions.TryPositionalSplit(inputString, lengthOfEachChunk, result)
        End Function

        ''' <summary>
        ''' Attempts to split a string into chunks of <paramref name="lengthOfEachChunk"/> characters and returns an array.
        ''' </summary>
        ''' <param name="inputString">The string to split.</param>
        ''' <param name="lengthOfEachChunk">The length of each resulting chunk.</param>
        ''' <returns>The resulting collection of chunks of length <paramref name="lengthOfEachChunk"/> from <paramref name="inputString"/>.
        ''' The final chunk may be a shorter length.</returns>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function PositionalSplit(ByRef inputString As String, lengthOfEachChunk As Integer) As String()
            Return _StringExtensions.PositionalSplit(inputString, lengthOfEachChunk)
        End Function

#End Region ' Split

#Region "Trim and Truncate"

        Enum EllipsisModes
            ''' <summary>
            ''' The string will be trimmed with no ellipsis
            ''' </summary>
            None
            ''' <summary>
            ''' The string will be trimmed to one character shorter than the maximum and appended with an ellipsis (…)
            ''' </summary>
            EllipsisEnd
            ''' <summary>
            ''' The string will be smartly split in the middle with an ellipsis (…) between the two parts. 
            ''' The first part will be prioritized for length.
            ''' </summary>
            EllipsisInteriorPrioritizeStart
            ''' <summary>
            ''' The string will be smartly split in the middle with an ellipsis (…) between the two parts. 
            ''' The last part will be prioritized for length.
            ''' </summary>
            EllipsisInteriorPrioritizeEnd
        End Enum

        <Flags>
        Enum SmartSplitModes
            ''' <summary>
            ''' The string will be smartly split on whitespace.
            ''' </summary>
            Whitespace = 1
            ''' <summary>
            ''' The string will be smartly split on hyphens and underscores.
            ''' </summary>
            HyphensAndUnderscores = 1 << 1
            ''' <summary>
            ''' The string will be smartly split on symbols. Note that this does NOT include single or double quotes so 
            ''' that contractions and quotations are not split weirdly.
            ''' </summary>
            Symbols = 1 << 2
            ''' <summary>
            ''' The string will be split on each capital letter LikeThisBecomes ["Like", "This", "Becomes"].
            ''' </summary>
            CapitalLetters = 1 << 3
        End Enum

        ''' <summary>
        ''' Returns a string with a maximum length of maxLength. If original length is less than maxLength, does not pad.
        ''' </summary>
        ''' <param name="inputString">The string to truncate.</param>
        ''' <param name="maxLength">The maximum length the string should be before it is truncated.</param>
        ''' <returns>The original string if it is less than the <paramref name="maxLength"/>, or the truncated 
        ''' string otherwise.</returns>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function Truncate(ByRef inputString As String, maxLength As Integer) As String
            Return _StringExtensions.Truncate(inputString, maxLength)
        End Function

        ''' <summary>
        ''' Returns a string with a maximum length of maxLength. If original length is less than maxLength, does not pad.
        ''' </summary>
        ''' <param name="inputString">The string to truncate.</param>
        ''' <param name="maxLength">The maximum length the string should be before it is truncated.</param>
        ''' <param name="ellipsisMode">Selects how an ellipsis should be inserted, if at all. 
        ''' See <see cref="EllipsisModes"/>.</param>
        ''' <returns>The original string if it is less than the <paramref name="maxLength"/>, or the truncated 
        ''' string otherwise.</returns>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function Truncate(ByRef inputString As String,
                                 maxLength As Integer,
                                 ellipsisMode As EllipsisModes) As String
            Return _StringExtensions.Truncate(inputString, maxLength, ellipsisMode)
        End Function

        ''' <summary>
        ''' Returns a string with a maximum length of maxLength. If original length is less than maxLength, does not pad.
        ''' </summary>
        ''' <param name="inputString">The string to truncate.</param>
        ''' <param name="maxLength">The maximum length the string should be before it is truncated.</param>
        ''' <param name="trimBeforeTruncate">Selects whether or not the input string has its whitespace trimmed 
        ''' prior to truncation.</param>
        ''' <returns>The original string if it is less than the <paramref name="maxLength"/>, or the truncated 
        ''' string otherwise.</returns>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function Truncate(ByRef inputString As String,
                                 maxLength As Integer,
                                 trimBeforeTruncate As Boolean) As String
            Return _StringExtensions.Truncate(inputString, maxLength, trimBeforeTruncate)
        End Function

        ''' <summary>
        ''' Returns a string with a maximum length of maxLength. If original length is less than maxLength, does not pad.
        ''' </summary>
        ''' <param name="inputString">The string to truncate.</param>
        ''' <param name="maxLength">The maximum length the string should be before it is truncated.</param>
        ''' <param name="ellipsisMode">Selects how an ellipsis should be inserted, if at all. 
        ''' See <see cref="EllipsisModes"/>.</param>
        ''' <param name="trimBeforeTruncate">Selects whether or not the input string has its whitespace trimmed 
        ''' prior to truncation.</param>
        ''' <returns>The original string if it is less than the <paramref name="maxLength"/>, or the truncated 
        ''' string otherwise.</returns>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function Truncate(ByRef inputString As String,
                                 maxLength As Integer,
                                 ellipsisMode As EllipsisModes,
                                 trimBeforeTruncate As Boolean) As String
            Return _StringExtensions.Truncate(inputString, maxLength, ellipsisMode, trimBeforeTruncate)
        End Function

        ''' <summary>
        ''' Splits a string smartly based on a set of rules specified in <paramref name="splitRules"/>.
        ''' </summary>
        ''' <param name="inputString">The string to split.</param>
        ''' <param name="splitRules">The types of splits that should occur. 
        ''' See <see cref="SmartSplitModes"/>.</param>
        ''' <param name="splitAfter">When false, splits before the rule character. When true, splits after.</param>
        ''' <returns>The string split by the rules.</returns>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function SmartSplit(inputString As String,
                                   splitRules As SmartSplitModes,
                                   splitAfter As Boolean) As IEnumerable(Of String)

            Return _StringExtensions.SmartSplit(inputString, splitRules, splitAfter)
        End Function

        ''' <summary>
        ''' Returns a string with the specified StringToRemove removed from the end of the string.
        ''' 
        ''' Example:
        ''' Dim str as String = "I work for Global Shop!"
        ''' str = str.TrimEndString(" Shop!")
        ''' ' str = "I work for Global"
        ''' </summary>
        ''' <param name="inputString">The string to be trimmed</param>
        ''' <param name="stringToRemove">The string to be removed from the end of strIn</param>
        ''' <returns>The trimmed string</returns>
        ''' <remarks>This does NOT automatically set the supplied string to the trimmed version.</remarks>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function TrimEndString(inputString As String, stringToRemove As String) As String
            Return _StringExtensions.TrimEndString(inputString, stringToRemove)
        End Function

        ''' <summary>
        ''' Returns a string with the specified StringToRemove removed from the beginning of the string.
        ''' 
        ''' Example:
        ''' Dim str as String = "I work for Global Shop!"
        ''' str = str.TrimStartString("I work")
        ''' ' str = " for Global Shop!"
        ''' </summary>
        ''' <param name="inputString">The string to be trimmed</param>
        ''' <param name="stringToRemove">The string to be removed from the beginning of strIn</param>
        ''' <returns>The trimmed string</returns>
        ''' <remarks>This does NOT automatically set the supplied string to the trimmed version.</remarks>
        <System.Diagnostics.DebuggerStepThrough()>
        <Extension>
        Public Function TrimStartString(inputString As String, stringToRemove As String) As String
            Return _StringExtensions.TrimStartString(inputString, stringToRemove)
        End Function

        ''' <summary>
        ''' Smartly reduces the length of the actual text within an HTML formatted string and optionally appends a
        ''' string to the shortened string. Commonly Chr(133) (ellipsis).
        ''' 
        ''' Will shorten the visible text no less than <paramref name="numberOfCharactersToReduceLengthBy"/> characters.
        ''' If <paramref name="appendStringToEnd"/> is used, it will continue to trim non-alpha-numeric characters 
        ''' before appending the string so you don't end up with "My String-...", etc.
        ''' </summary>
        ''' <param name="htmlString">The HTML formatted string to modify.</param>
        ''' <param name="numberOfCharactersToReduceLengthBy">The minimum number of characters that are trimmed from the 
        ''' HTML string. If <paramref name="appendStringToEnd"/> is used, it will keep trimming past this amount to 
        ''' ensure the appended string immediately follows an alpha-numeric character.</param>
        ''' <param name="appendStringToEnd">A string that's appended to the end of the HTML string. Commonly Chr(133), 
        ''' the Ellipsis.</param>
        ''' <returns>An HTML formatted string whose actual visible text is trimmed.</returns>
        ''' <remarks>
        ''' 1. Empty tags are removed as the string is trimmed.
        ''' 2. <nbsp> is treated as a single character.
        ''' 3. If "Append..." is used, the string WILL end with that.
        ''' </remarks>
        ''' <example>
        ''' Input: <code><p>My String!</p><p><size=14>Second Paragraph.</size></p></code>
        ''' 
        ''' 1. Trim by 5, append ellipsis
        '''    <p>My String!</p><p><size=14>Second Parag…</size></p>
        ''' 2. Trim by 10, append ellipsis (note that it trimmed 11 because of the space)
        '''    <p>My String!</p><p><size=14>Second…</size></p>
        ''' 3. Trim by 17, append ellipsis
        '''    <p>My String…</p>
        ''' </example>
        <Extension>
        Public Function ReduceHtmlStringLengthBy(ByRef htmlString As String, Optional numberOfCharactersToReduceLengthBy As UInteger = 1, Optional appendStringToEnd As String = Nothing) As String
            Return _StringExtensions.ReduceHtmlStringLengthBy(htmlString, numberOfCharactersToReduceLengthBy, appendStringToEnd)
        End Function

#End Region ' Trim and Truncate

#Region "Formatting"

        ''' <summary>
        ''' A class that contains options about how <see cref="AddSpacesToSentence(ByRef String, SentenceFormatOptions)"/>
        ''' should format the string.
        ''' </summary>
        Class SentenceFormatOptions
            ''' <summary>
            ''' When <see langword="True"/>, acronyms are detected and maintained. When false, each capital letter
            ''' is treated like a new word. For example, the string "theBOMIsExploding" becomes "the BOM Is Exploding"
            ''' when True and "the B O M Is Exploding" when false.
            ''' </summary>
            Property PreserveAcronyms As Boolean = True
            ''' <summary>
            ''' Determines how the words in the sentence are capitalized.
            ''' </summary>
            Property CapitalizationStrategy As CapitalizationStrategies = CapitalizationStrategies.TitleCase

            Enum CapitalizationStrategies
                ''' <summary>
                ''' The capitalization in the string is unchanged: 
                ''' "theBOMIsExplodingInTheProgram" becomes "the BOM Is Exploding In The Program"
                ''' </summary>
                DoNotModify
                ''' <summary>
                ''' All words are capitalized: 
                ''' "theBOMIsExplodingInTheProgram" becomes "The BOM Is Exploding In The Program"
                ''' </summary>
                AllWords
                ''' <summary>
                ''' All words are capitalized except for words that are not usually capitalized in titles: 
                ''' "theBOMIsExplodingInTheProgram" becomes "The BOM Is Exploding in the Program"
                ''' </summary>
                TitleCase
                ''' <summary>
                ''' The first word is capitalized: 
                ''' "theBOMIsExplodingInTheProgram" becomes "The BOM is exploding in the program"
                ''' Note that acronyms are still capitalized when <see cref="PreserveAcronyms"/> is <see langword="True"/>.
                ''' </summary>
                FirstWord
                ''' <summary>
                ''' No words are capitalized: 
                ''' "theBOMIsExplodingInTheProgram" becomes "the BOM is exploding in the program"
                ''' Note that acronyms are still capitalized when <see cref="PreserveAcronyms"/> is <see langword="True"/>.
                ''' </summary>
                None
            End Enum
        End Class

        ''' <summary>
        ''' Takes a string and adds spaces where it finds capital letters or underscores.
        ''' </summary>
        ''' <param name="text">The input text to add spaces to.</param>
        ''' <param name="sentenceOptions">The options used for formatting the string, including if acronyms are 
        ''' preserved, and which letters are capitalized. By default, acronyms ARE preserved and 
        ''' <see cref="SentenceFormatOptions.CapitalizationStrategies.TitleCase"/> is used.</param>
        <Extension>
        Public Function AddSpacesToSentence(ByRef text As String,
                                            Optional sentenceOptions As SentenceFormatOptions = Nothing) As String
            Return _StringExtensions.AddSpacesToSentence(text, sentenceOptions)
        End Function

#End Region ' Formatting

#Region "Match Score"

        ''' <summary>
        ''' An information type that contains the overall and highest single score for the similarity between two strings.
        ''' </summary>
        Public Class StringMatchScoreInfo

            ''' <summary>
            ''' The overall score for the match. If using "Match Any", this will be a weighted score based on the 
            ''' average of the full string score and the highest single token score. 
            ''' This is useful for ranking searches. Many strings may match one token within the search string, but you 
            ''' want to rank by the closest overall string as well.
            ''' </summary>
            ReadOnly Property OverallScore As Double
            ''' <summary>
            ''' When using "Match Any" rules, this is the highest score for a single token match. Both the input string 
            ''' and the match string will be split on spaces, and each individual token is compared. This is the highest 
            ''' score of any of those input/match token pairs.
            ''' 
            ''' When NOT using "Match Any", this will be the same as the <see cref="OverallScore"/>.
            ''' </summary>
            ''' <returns></returns>
            ReadOnly Property HighestSingleMatchScore As Double

            Public Sub New(overallScore As Double, highestSingleMatchScore As Double)
                Me.OverallScore = overallScore
                Me.HighestSingleMatchScore = highestSingleMatchScore
            End Sub

            Public Shared Widening Operator CType(value As StringMatchScoreInfo) As Double
                Return value.OverallScore
            End Operator

        End Class

        ''' <summary>
        ''' Returns a <see cref="StringMatchScoreInfo"/> (Range 0.0 to 100.0) based on how closely the 
        ''' <paramref name="inputText"/> matches the <paramref name="matchText"/>.
        ''' </summary>
        ''' <param name="inputText">The initial string to match</param>
        ''' <param name="matchText">The string to compare the <paramref name="inputText"/> with. When 
        ''' <paramref name="matchAny"/> is active, this is tokenized.</param>
        ''' <param name="matchAny">When <see langword="False"/>, the entire <paramref name="inputText"/> and 
        ''' <paramref name="matchText"/> are compared and scored. When <see langword="True"/>, both strings are split 
        ''' on whitespace, and each individual token is compared.</param>
        <Extension>
        Public Function GetSimilarityScore(ByRef inputText As String, matchText As String, matchAny As Boolean) As StringMatchScoreInfo
            Return _StringExtensions.GetSimilarityScore(inputText, matchText, matchAny)
        End Function

        ''' <summary>
        ''' Returns a <see cref="StringMatchScoreInfo"/> (Range 0.0 to 100.0) based on how closely the 
        ''' <paramref name="inputText"/> matches the regex <paramref name="pattern"/>.
        ''' </summary>
        ''' <param name="inputText">The initial string to match</param>
        ''' <param name="pattern">The regular expressions pattern to use. The overall score is based on how much of the 
        ''' <paramref name="inputText"/> is matched by the pattern. 
        ''' This uses default Regex Options <see cref="RegexOptions.Singleline"/> and 
        ''' <see cref="RegexOptions.IgnoreCase"/>.</param>
        <Extension>
        Public Function GetRegexMatchScore(ByRef inputText As String, pattern As String) As StringMatchScoreInfo
            Return _StringExtensions.GetRegexMatchScore(inputText, pattern)
        End Function

        ''' <summary>
        ''' Returns a <see cref="StringMatchScoreInfo"/> (Range 0.0 to 100.0) based on how closely the 
        ''' <paramref name="inputText"/> matches the regex <paramref name="pattern"/>.
        ''' </summary>
        ''' <param name="inputText">The initial string to match</param>
        ''' <param name="pattern">The regular expressions pattern to use. The overall score is based on how much of the 
        ''' <paramref name="inputText"/> is matched by the pattern.</param>
        ''' <param name="regexOptions">The regex options to use when matching <paramref name="pattern"/>.</param>
        <Extension>
        Public Function GetRegexMatchScore(ByRef inputText As String, pattern As String, regexOptions As RegexOptions) As StringMatchScoreInfo
            Return _StringExtensions.GetRegexMatchScore(inputText, pattern, regexOptions)
        End Function

        ''' <summary>
        ''' Returns a <see cref="StringMatchScoreInfo"/> (Range 0.0 to 100.0) based on how closely the 
        ''' <paramref name="inputText"/> matches the <paramref name="regex"/>.
        ''' </summary>
        ''' <param name="inputText">The initial string to match</param>
        ''' <param name="regex">The regular expression to use for matching. The overall score is based on how much of the 
        ''' <paramref name="inputText"/> is matched by this regex definition.</param>
        <Extension>
        Public Function GetRegexMatchScore(ByRef inputText As String, regex As Regex) As StringMatchScoreInfo
            Return _StringExtensions.GetRegexMatchScore(inputText, regex)
        End Function

#End Region ' Match Score

#Region "File Names"

        ''' <summary>
        ''' Accepts a File Name or fully qualified File Name and Path and ensures that it is a valid file name.
        ''' If the path contains an error, an exception is thrown. Otherwise, returns a valid file name or 
        ''' "<see langword="Nothing"/>" if the name could not be corrected.
        ''' </summary>
        ''' <param name="fileNameOrFileNameAndPath">The File Name or fully qualified File Name and Path to correct.</param>
        ''' <param name="incrementIfFileExists">When True, will append an increment value to the end of a file name if 
        ''' the file already exists. For example, "MyFile (1).txt"</param>
        ''' <param name="replaceBadCharsWith">Overrides the default "_" character for replacing invalid characters.</param>
        ''' <param name="disallowAdditionalCharacters">Allows characters other than the file system invalid list to be 
        ''' considered invalid. Useful for removing spaces, dashes, or other characters.</param>
        ''' <returns>A corrected file name (and path if applicable).
        ''' A "<see langword="Nothing"/>" value indicates a correction error.</returns>
        <Extension>
        Function MakeFileNameValid(ByRef fileNameOrFileNameAndPath As String,
                                   Optional incrementIfFileExists As Boolean = False,
                                   Optional replaceBadCharsWith As String = "_",
                                   Optional disallowAdditionalCharacters As IEnumerable(Of Char) = Nothing) As String

            Return _StringExtensions.MakeFileNameAndPathValidInternal(
                fileNameOrFileNameAndPath,
                incrementIfFileExists,
                replaceBadCharsWith,
                disallowAdditionalCharacters,
                Nothing)
        End Function

        ''' <summary>
        ''' Accepts a collection of File Names or fully qualified File Names and Paths and ensures that they are valid 
        ''' file names. This does NOT throw an exception on error. Instead, any file name that cannot be corrected will 
        ''' be "<see langword="Nothing"/>" in that position of the return collection.
        ''' </summary>
        ''' <param name="fileNamesOrFileNameAndPaths">The collection of File Names or fully qualified 
        ''' File Names and Paths to correct.</param>
        ''' <param name="incrementIfFileExists">When True, will append an increment value to the end of a file names if 
        ''' the file already exists. For example, "MyFile (1).txt"</param>
        ''' <param name="replaceBadCharsWith">Overrides the default "_" character for replacing invalid characters.</param>
        ''' <param name="disallowAdditionalCharacters">Allows characters other than the file system invalid list to be 
        ''' considered invalid. Useful for removing spaces, dashes, or other characters.</param>
        ''' <returns>A collection of corrected file names (and paths if applicable).
        ''' A "<see langword="Nothing"/>" value indicates a correction error.</returns>
        <Extension>
        Function MakeFileNamesValid(ByRef fileNamesOrFileNameAndPaths As IEnumerable(Of String),
                                    Optional incrementIfFileExists As Boolean = False,
                                    Optional replaceBadCharsWith As String = "_",
                                    Optional disallowAdditionalCharacters As IEnumerable(Of Char) = Nothing) As String()

            If fileNamesOrFileNameAndPaths Is Nothing OrElse fileNamesOrFileNameAndPaths.Count = 0 Then
                Return fileNamesOrFileNameAndPaths?.ToArray
            End If

            Dim correctedFqns As New List(Of String)
            For Each fileNameOrFqn In fileNamesOrFileNameAndPaths
                Dim fileNameOrFqnInternal As String = fileNameOrFqn
                Try
                    Dim correctedFqn = _StringExtensions.MakeFileNameAndPathValidInternal(
                        fileNameOrFqnInternal,
                        incrementIfFileExists,
                        replaceBadCharsWith,
                        disallowAdditionalCharacters,
                        correctedFqns.ToArray)

                    correctedFqns.Add(correctedFqn)
                Catch ex As Exception
                    correctedFqns.Add(Nothing)
                End Try
            Next

            Return correctedFqns.ToArray
        End Function

        ''' <summary>
        ''' Accepts a File Name or fully qualified File Name and Path and ensures that it is a valid file name.
        ''' If the path contains an error, an exception is thrown. Otherwise, the variable passed in 
        ''' <paramref name="fileNameOrFileNameAndPath"/> will be changed to the corrected value. 
        ''' If the file name and path cannot be corrected, it will not be changed and the method will 
        ''' return "<see langword="False"/>".
        ''' </summary>
        ''' <param name="fileNameOrFileNameAndPath">The File Name or fully qualified File Name and Path 
        ''' to correct (by reference).</param>
        ''' <param name="incrementIfFileExists">When True, will append an increment value to the end of a file name if 
        ''' the file already exists. For example, "MyFile (1).txt"</param>
        ''' <param name="replaceBadCharsWith">Overrides the default "_" character for replacing invalid characters.</param>
        ''' <param name="disallowAdditionalCharacters">Allows characters other than the file system invalid list to be 
        ''' considered invalid. Useful for removing spaces, dashes, or other characters.</param>
        ''' <returns>"<see langword="True"/>" if the correction was made (or if no correction is necessary), 
        ''' "<see langword="False"/>" otherwise. Does NOT throw an exception on error.</returns>
        <Extension>
        Function TryMakeFileNameValid(ByRef fileNameOrFileNameAndPath As String,
                           Optional incrementIfFileExists As Boolean = False,
                           Optional replaceBadCharsWith As String = "_",
                           Optional disallowAdditionalCharacters() As Char = Nothing) As Boolean

            If fileNameOrFileNameAndPath = Nothing Then Return False

            Dim fileNameOrFqnInternal As String = fileNameOrFileNameAndPath
            Try
                Dim correctedName = _StringExtensions.MakeFileNameAndPathValidInternal(
                    fileNameOrFqnInternal,
                    incrementIfFileExists,
                    replaceBadCharsWith,
                    disallowAdditionalCharacters,
                    Nothing)

                If correctedName = Nothing Then
                    Return False
                Else
                    fileNameOrFileNameAndPath = correctedName
                    Return True
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function

#End Region

    End Module

    Public Class StringExtensions

#Region "Alpha-numeric"

        ''' <summary>
        ''' Removes all characters from the string, except for letters.
        ''' </summary>
        ''' <param name="inputString"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function MakeAlpha(ByRef inputString As String) As String
            Return Regex.Replace(inputString, "[^a-zA-Z]+", "", RegexOptions.Compiled)
        End Function

        ''' <summary>
        ''' Removes all characters from the string, except for letters and numerals.
        ''' </summary>
        ''' <param name="inputString"></param>
        ''' <param name="allowPeriods">Will leave periods/decimal points in the string when true.</param>
        ''' <param name="allowUnderscore">Will leave underscores '_' in the string when true.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function MakeAlphaNumeric(ByRef inputString As String, Optional allowPeriods As Boolean = False, Optional allowUnderscore As Boolean = False) As String
            If allowPeriods AndAlso allowUnderscore Then
                Return Regex.Replace(inputString, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled)
            ElseIf allowPeriods Then
                Return Regex.Replace(inputString, "[^a-zA-Z0-9.]+", "", RegexOptions.Compiled)
            ElseIf allowUnderscore Then
                Return Regex.Replace(inputString, "[^a-zA-Z0-9_]+", "", RegexOptions.Compiled)
            Else
                Return Regex.Replace(inputString, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled)
            End If
        End Function

        ''' <summary>
        ''' Removes all characters from the string, except for letters, numerals, and specified characters.
        ''' </summary>
        ''' <param name="inputString"></param>
        ''' <param name="additionalAllowedCharacters">A collection of other characters to allow.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function MakeAlphaNumeric(ByRef inputString As String, additionalAllowedCharacters As IEnumerable(Of Char)) As String

            Dim sOther As String = String.Join("", additionalAllowedCharacters)
            ' Some jank. Minus should come first so that it's not confused as a range. Think [A-Z], so it needs to be
            ' [-A-Z] to be counted right every time. Secondly, for some reason, Regex.Escape escapes [ properly, but
            ' doesn't do ], so we do that manually.
            Dim hasMinus As Boolean = False
            If sOther.Contains("-") Then
                hasMinus = True
                sOther = sOther.Replace("-", "")
            End If
            sOther = Regex.Escape(sOther)
            sOther = sOther.Replace("]", "\]")
            Return Regex.Replace(inputString, $"[^{If(hasMinus, "-", "")}a-zA-Z0-9{sOther}]+", "", RegexOptions.Compiled)
        End Function

        ''' <summary>
        ''' Removes all non numeric (0, 1, 2, 3, 4, 5, 6, 7, 8, 9) from the string and returns the result.
        ''' </summary>
        ''' <param name="inputString"></param>
        ''' <param name="allowDecimalPoint">Should the return include the decimal point '.'?</param>
        ''' <returns>The string of numerals</returns>
        ''' <remarks></remarks>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function MakeNumeric(ByRef inputString As String, Optional allowDecimalPoint As Boolean = False) As String
            If allowDecimalPoint Then
                Return Regex.Replace(inputString, "[^0-9.]+", "", RegexOptions.Compiled)
            Else
                Return Regex.Replace(inputString, "[^0-9]+", "", RegexOptions.Compiled)
            End If
        End Function

#End Region ' Alpha-numeric

#Region "SQL"

        ''' <summary>
        ''' Formats a string for SQL statements (replaces ')
        ''' </summary>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function MakeSQLFriendly(value As String) As String
            Dim result As String = Replace(value, "'", "''")
            If result <> Nothing Then
                If result.EndsWith("\") AndAlso Not result.EndsWith("\\") Then result += "\"
            End If
            Return result
        End Function

#End Region ' SQL

#Region "String Encoding and Compression"

        ''' <summary>
        ''' Decodes old GS encrypted values.
        ''' </summary>
        Public Function GSDecode(ByRef value As String) As String
            Dim result As String = ""

            Try
                ' Get List of bytes by converting each set of 4 Hex characters to ASCII.
                If value.Length Mod 2 = 1 Then
                    result = value
                    Return result
                End If

                Dim bytes As New List(Of Byte)
                If String.IsNullOrWhiteSpace(value) = False Then
                    For i As Integer = 0 To value.Length - 1 Step 4
                        Dim hexString As String = value.Substring(i, 2)
                        Dim byteValue As Byte = Convert.ToByte(0)

                        If System.Text.RegularExpressions.Regex.Match(hexString.Trim.ToUpper.PadLeft(2, "0"), "^[A-F0-9]{2}$").Success = True Then
                            byteValue = Convert.ToByte(Convert.ToInt32(hexString.ToUpper, 16))
                        End If

                        bytes.Add(byteValue)
                    Next
                End If

                ' Get String from ASCII Byte array, then reverse the result.
                If bytes.Count > 0 Then
                    Dim encoding As New System.Text.ASCIIEncoding
                    result = encoding.GetString(bytes.ToArray)
                    result = StrReverse(result)
                End If
            Catch ex As Exception
                result = value
            End Try

            Return result
        End Function

        <System.Diagnostics.DebuggerStepThrough()>
        Public Function Compress(ByRef uncompressedValue As String) As String
            Return CompressString(uncompressedValue)
        End Function

        <System.Diagnostics.DebuggerStepThrough()>
        Public Function CompressString(uncompressedValue As String) As String
            Dim lessBytes As Byte() = CompressBytes(System.Text.Encoding.UTF8.GetBytes(uncompressedValue))
            Dim base64String As String = Convert.ToBase64String(lessBytes, 0, lessBytes.Length)
            Return base64String
        End Function


        ''' <summary>
        ''' Compresses a Byte() array using the DEFLATE algorithm
        ''' </summary>
        Private Function CompressBytes(ByVal toCompress As Byte()) As Byte()
            ' Get the stream of the source file.
            Using inputStream As IO.MemoryStream = New IO.MemoryStream(toCompress)

                ' Create the compressed stream.
                Using outputStream As IO.MemoryStream = New IO.MemoryStream()
                    Using compressionStream As IO.Compression.DeflateStream =
                New IO.Compression.DeflateStream(outputStream, IO.Compression.CompressionMode.Compress)

                        ' Copy the source file into the compression stream.
                        inputStream.CopyTo(compressionStream)

                    End Using

                    CompressBytes = outputStream.ToArray()

                End Using

            End Using
        End Function

        <System.Diagnostics.DebuggerStepThrough()>
        Public Function TryDecompress(ByRef base64String As String) As String
            Return TryDecompressString(base64String)
        End Function

        <System.Diagnostics.DebuggerStepThrough()>
        Public Function TryDecompressString(base64String As String) As String
            If String.IsNullOrEmpty(base64String) OrElse
                base64String.Length Mod 4 <> 0 OrElse
                base64String.Contains(" ") OrElse
                base64String.Contains("\t") OrElse
                base64String.Contains("\r") OrElse
                base64String.Contains("\n") Then

                Return base64String
            End If

            Try
                Dim convertedBytes As Byte() = Convert.FromBase64String(base64String)
                Dim decompressedBytes As Byte() = DecompressBytes(convertedBytes)
                Return System.Text.Encoding.UTF8.GetString(decompressedBytes)

            Catch e As Exception
                Return base64String
            End Try

        End Function

        ''' <summary>
        ''' Decompresses a Byte() array using the DEFLATE algorithm.
        ''' </summary>
        Private Function DecompressBytes(ByVal toDecompress As Byte()) As Byte()
            ' Get the stream of the source file.
            Using inputStream As IO.MemoryStream = New IO.MemoryStream(toDecompress)

                ' Create the decompressed stream.
                Using outputStream As IO.MemoryStream = New IO.MemoryStream()
                    Using decompressionStream As IO.Compression.DeflateStream =
                New IO.Compression.DeflateStream(inputStream, IO.Compression.CompressionMode.Decompress)

                        ' Copy the decompression stream
                        ' into the output file.
                        decompressionStream.CopyTo(outputStream)

                    End Using

                    DecompressBytes = outputStream.ToArray

                End Using
            End Using
        End Function

#End Region ' String Encoding and Compression

#Region "Split"

        ''' <summary>
        ''' Splits the string based on a string delimiter
        ''' </summary>
        ''' <param name="input">The string to split</param>
        ''' <param name="delimiter">The delimiter string</param>
        ''' <returns>An array of string objects</returns>
        ''' <remarks></remarks>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function Split(ByVal input As String, ByVal ParamArray delimiter As String()) As String()
            Return input.Split(delimiter, StringSplitOptions.None)
        End Function

        ''' <summary>
        ''' Attempts to split a string into chunks of <paramref name="lengthOfEachChunk"/> characters into an array.
        ''' </summary>
        ''' <param name="inputString">The string to split.</param>
        ''' <param name="lengthOfEachChunk">The length of each resulting chunk.</param>
        ''' <param name="result">The resulting collection of chunks of length <paramref name="lengthOfEachChunk"/> from <paramref name="inputString"/>.
        ''' The final chunk may be a shorter length.</param>
        ''' <returns>True if successful, false otherwise.</returns>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function TryPositionalSplit(ByRef inputString As String, lengthOfEachChunk As Integer, ByRef result() As String) As Boolean
            Try
                result = PositionalSplit(inputString, lengthOfEachChunk)

                Return True
            Catch ex As Exception
                result = Nothing
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Attempts to split a string into chunks of <paramref name="lengthOfEachChunk"/> characters and returns an array.
        ''' </summary>
        ''' <param name="inputString">The string to split.</param>
        ''' <param name="lengthOfEachChunk">The length of each resulting chunk.</param>
        ''' <returns>The resulting collection of chunks of length <paramref name="lengthOfEachChunk"/> from <paramref name="inputString"/>.
        ''' The final chunk may be a shorter length.</returns>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function PositionalSplit(ByRef inputString As String, lengthOfEachChunk As Integer) As String()

            Dim resultList As New List(Of String)
            Dim currentString = If(inputString = Nothing, String.Empty, inputString)

            While currentString.Length > 0
                Dim currentChunk As String = currentString.Substring(0, Math.Min(currentString.Length, lengthOfEachChunk))
                resultList.Add(currentChunk)
                currentString = currentString.Substring(currentChunk.Length)
            End While

            Return resultList.ToArray

        End Function

#End Region ' Split

#Region "Trim and Truncate"

        ''' <summary>
        ''' Returns a string with a maximum length of maxLength. If original length is less than maxLength, does not pad.
        ''' </summary>
        ''' <param name="inputString">The string to truncate.</param>
        ''' <param name="maxLength">The maximum length the string should be before it is truncated.</param>
        ''' <returns>The original string if it is less than the <paramref name="maxLength"/>, or the truncated 
        ''' string otherwise.</returns>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function Truncate(inputString As String, maxLength As Integer) As String
            Return Truncate(inputString, maxLength, EllipsisModes.None)
        End Function

        ''' <summary>
        ''' Returns a string with a maximum length of maxLength. If original length is less than maxLength, does not pad.
        ''' </summary>
        ''' <param name="inputString">The string to truncate.</param>
        ''' <param name="maxLength">The maximum length the string should be before it is truncated.</param>
        ''' <param name="ellipsisMode">Selects how an ellipsis should be inserted, if at all. 
        ''' See <see cref="EllipsisModes"/>.</param>
        ''' <returns>The original string if it is less than the <paramref name="maxLength"/>, or the truncated 
        ''' string otherwise.</returns>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function Truncate(inputString As String,
                                 maxLength As Integer,
                                 ellipsisMode As EllipsisModes) As String
            Return Truncate(inputString, maxLength, ellipsisMode, False)
        End Function

        ''' <summary>
        ''' Returns a string with a maximum length of maxLength. If original length is less than maxLength, does not pad.
        ''' </summary>
        ''' <param name="inputString">The string to truncate.</param>
        ''' <param name="maxLength">The maximum length the string should be before it is truncated.</param>
        ''' <param name="trimBeforeTruncate">Selects whether or not the input string has its whitespace trimmed 
        ''' prior to truncation.</param>
        ''' <returns>The original string if it is less than the <paramref name="maxLength"/>, or the truncated 
        ''' string otherwise.</returns>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function Truncate(inputString As String,
                                 maxLength As Integer,
                                 trimBeforeTruncate As Boolean) As String
            Return Truncate(inputString, maxLength, EllipsisModes.None, trimBeforeTruncate)
        End Function

        ''' <summary>
        ''' Returns a string with a maximum length of maxLength. If original length is less than maxLength, does not pad.
        ''' </summary>
        ''' <param name="inputString">The string to truncate.</param>
        ''' <param name="maxLength">The maximum length the string should be before it is truncated.</param>
        ''' <param name="ellipsisMode">Selects how an ellipsis should be inserted, if at all. 
        ''' See <see cref="EllipsisModes"/>.</param>
        ''' <param name="trimBeforeTruncate">Selects whether or not the input string has its whitespace trimmed 
        ''' prior to truncation.</param>
        ''' <returns>The original string if it is less than the <paramref name="maxLength"/>, or the truncated 
        ''' string otherwise.</returns>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function Truncate(inputString As String,
                                 maxLength As Integer,
                                 ellipsisMode As EllipsisModes,
                                 trimBeforeTruncate As Boolean) As String
            If trimBeforeTruncate Then inputString = inputString.Trim

            If inputString.IsDbNullNothingEmpty Then Return inputString
            If inputString.Length <= maxLength Then Return inputString
            If maxLength = 0 Then Return String.Empty

            Select Case ellipsisMode
                Case EllipsisModes.EllipsisEnd
                    If inputString.Length >= 2 AndAlso maxLength >= 2 Then
                        Return EnsureEndsAlphaNumeric(inputString.Substring(0, maxLength - 1)) & Chr(133)
                    Else
                        Return Truncate(inputString, maxLength, EllipsisModes.None, trimBeforeTruncate)
                    End If

                Case EllipsisModes.EllipsisInteriorPrioritizeStart
                    If inputString.Length >= 3 AndAlso maxLength >= 3 Then
                        Return EllipsisInterior(inputString, maxLength, True)
                    Else
                        Return Truncate(inputString, maxLength, EllipsisModes.EllipsisEnd, trimBeforeTruncate)
                    End If

                Case EllipsisModes.EllipsisInteriorPrioritizeEnd
                    If inputString.Length >= 3 AndAlso maxLength >= 3 Then
                        Return EllipsisInterior(inputString, maxLength, False)
                    Else
                        Return Truncate(inputString, maxLength, EllipsisModes.EllipsisEnd, trimBeforeTruncate)
                    End If

                Case Else
                    Return inputString.Substring(0, maxLength)

            End Select

        End Function

        Private Function EnsureEndsAlphaNumeric(ByVal nonPriorityString As String) As String
            If nonPriorityString = Nothing Then Return String.Empty

            Dim returnString = nonPriorityString
            Dim lastChar As String = returnString.Substring(returnString.Length - 1)

            If Regex.Match(lastChar, "[^A-Za-z0-9]").Success Then
                Return EnsureEndsAlphaNumeric(returnString.Truncate(returnString.Length - 1))
            Else
                Return returnString
            End If

        End Function

        Private Function EnsureStartsAlphaNumeric(ByVal nonPriorityString As String) As String
            If nonPriorityString = Nothing Then Return String.Empty

            Dim returnString = nonPriorityString
            Dim firstChar As String = returnString.Substring(0, 1)

            If Regex.Match(firstChar, "[^A-Za-z0-9]").Success Then
                Return EnsureStartsAlphaNumeric(returnString.Substring(1))
            Else
                Return returnString
            End If

        End Function

        Private Function EllipsisInterior(ByRef inputString As String, maxLength As Integer, prioritizeStart As Boolean) As String
            Dim enumFlag As SmartSplitModes = 0
            Dim nonPriorityString As String = String.Empty
            Dim priorityString As String = String.Empty

            For Each enumMode In [Enum].GetValues(GetType(SmartSplitModes))
                enumFlag = enumFlag Or enumMode
                Dim tokens As List(Of String) = SmartSplit(inputString, enumFlag, False)

                If tokens.Count < 2 Then Continue For
                If prioritizeStart Then
                    If tokens.Last.Length > maxLength / 2.0 Then Continue For
                Else
                    If tokens.First.Length > maxLength / 2.0 Then Continue For
                End If

                While nonPriorityString.Length < maxLength * 0.25 AndAlso nonPriorityString.Length < 24
                    If (If(prioritizeStart, tokens.Last, tokens.First) & nonPriorityString).Length < maxLength * 0.5 Then
                        If prioritizeStart Then
                            nonPriorityString = tokens.Last & nonPriorityString
                            tokens.RemoveAt(tokens.Count - 1)
                        Else
                            nonPriorityString = nonPriorityString & tokens.First
                            tokens.RemoveAt(0)
                        End If
                    Else
                        Exit While
                    End If
                End While
                If nonPriorityString = Nothing Then Continue For

                If prioritizeStart Then
                    nonPriorityString = Chr(133) & EnsureStartsAlphaNumeric(nonPriorityString)
                Else
                    nonPriorityString = EnsureEndsAlphaNumeric(nonPriorityString) & Chr(133)
                End If

                priorityString = String.Join(String.Empty, tokens)
                If prioritizeStart Then
                    priorityString = EnsureEndsAlphaNumeric(Truncate(priorityString, maxLength - nonPriorityString.Length))
                Else
                    priorityString = EnsureStartsAlphaNumeric(priorityString.Substring(priorityString.Length - (maxLength - nonPriorityString.Length)))
                End If

                If priorityString = Nothing Then Continue For

                If prioritizeStart Then
                    Return priorityString & nonPriorityString
                Else
                    Return nonPriorityString & priorityString
                End If
            Next

            Dim nonPriorityLength = Math.Min(inputString.Length - 2, Math.Max(1, Math.Ceiling(maxLength * 0.25)))
            If prioritizeStart Then
                nonPriorityString = EnsureStartsAlphaNumeric(inputString.Substring(inputString.Length - nonPriorityLength))
                inputString = inputString.Substring(0, inputString.Length - nonPriorityLength)
                nonPriorityString = Chr(133) & nonPriorityString
                priorityString = EnsureEndsAlphaNumeric(Truncate(inputString, maxLength - nonPriorityString.Length))
                Return priorityString & nonPriorityString
            Else
                nonPriorityString = EnsureEndsAlphaNumeric(Truncate(inputString, nonPriorityLength))
                inputString = inputString.Substring(nonPriorityString.Length)
                nonPriorityString = nonPriorityString & Chr(133)
                priorityString = EnsureStartsAlphaNumeric(inputString.Substring(inputString.Length - (maxLength - nonPriorityString.Length)))
                Return nonPriorityString & priorityString
            End If
        End Function

        ''' <summary>
        ''' Splits a string smartly based on a set of rules specified in <paramref name="splitRules"/>.
        ''' </summary>
        ''' <param name="inputString">The string to split.</param>
        ''' <param name="splitRules">The types of splits that should occur. 
        ''' See <see cref="SmartSplitModes"/>.</param>
        ''' <param name="splitAfter">When false, splits before the rule character. When true, splits after.</param>
        ''' <returns>The string split by the rules.</returns>
        Public Function SmartSplit(inputString As String,
                                   splitRules As SmartSplitModes,
                                   splitAfter As Boolean) As IEnumerable(Of String)

            Dim splitPatterns As New Dictionary(Of SmartSplitModes, Dictionary(Of Boolean, String)) From {
                {SmartSplitModes.Whitespace,
                New Dictionary(Of Boolean, String)() From {
                    {False, "([\s*\r*\n*]*[^\s*^\r*^\n*]*)"},
                    {True, "([^\s*^\r*^\n*]*[\s*\r*\n*]*)"}}},
                {SmartSplitModes.HyphensAndUnderscores,
                New Dictionary(Of Boolean, String)() From {
                    {False, "([-*_*]*[^-*^_*]*)"},
                    {True, "([^-*^_*]*[-*_*]*)"}}},
                {SmartSplitModes.Symbols,
                New Dictionary(Of Boolean, String)() From {
                    {False, "([^a-zA-Z0-9\'\""]*[a-zA-Z0-9\'\""]*)"},
                    {True, "([a-zA-Z0-9\'\""]*[^a-zA-Z0-9\'\""]*)"}}},
                {SmartSplitModes.CapitalLetters,
                New Dictionary(Of Boolean, String)() From {
                    {False, "([A-Z]*[^A-Z]*)"},
                    {True, "([^A-Z]*[A-Z]*)"}}}
            }

            Dim mainSplitList As New List(Of String)
            Dim splitListThisIteration As New List(Of String)

            mainSplitList.Add(inputString)

            For Each flag In EnumExtensions.GetFlags(Of SmartSplitModes)(splitRules)
                splitListThisIteration.Clear()

                For Each splitString In mainSplitList

                    For Each match As Match In Regex.Matches(splitString, splitPatterns(flag)(splitAfter))
                        splitListThisIteration.Add(match.Value)
                    Next
                Next

                mainSplitList.Clear()
                mainSplitList.AddRange(splitListThisIteration)
            Next

            Return mainSplitList.FindAll(Function(x) x <> Nothing)
        End Function

        ''' <summary>
        ''' Returns a string with the specified StringToRemove removed from the end of the string.
        ''' 
        ''' Example:
        ''' Dim str as String = "I work for Global Shop!"
        ''' str = str.TrimEndString(" Shop!")
        ''' ' str = "I work for Global"
        ''' </summary>
        ''' <param name="inputString">The string to be trimmed</param>
        ''' <param name="stringToRemove">The string to be removed from the end of strIn</param>
        ''' <returns>The trimmed string</returns>
        ''' <remarks>This does NOT automatically set the supplied string to the trimmed version.</remarks>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function TrimEndString(inputString As String, stringToRemove As String) As String
            TrimEndString = inputString
            If Not inputString Is Nothing Then
                If inputString.EndsWith(stringToRemove) Then
                    TrimEndString = inputString.Substring(0, inputString.Length - stringToRemove.Length)
                End If
            End If
        End Function

        ''' <summary>
        ''' Returns a string with the specified StringToRemove removed from the beginning of the string.
        ''' 
        ''' Example:
        ''' Dim str as String = "I work for Global Shop!"
        ''' str = str.TrimStartString("I work")
        ''' ' str = " for Global Shop!"
        ''' </summary>
        ''' <param name="inputString">The string to be trimmed</param>
        ''' <param name="stringToRemove">The string to be removed from the beginning of strIn</param>
        ''' <returns>The trimmed string</returns>
        ''' <remarks>This does NOT automatically set the supplied string to the trimmed version.</remarks>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function TrimStartString(inputString As String, stringToRemove As String) As String
            TrimStartString = inputString
            If Not inputString Is Nothing Then
                If inputString.StartsWith(stringToRemove) Then
                    TrimStartString = inputString.Substring(stringToRemove.Length)
                End If
            End If
        End Function

        ''' <summary>
        ''' Smartly reduces the length of the actual text within an HTML formatted string and optionally appends a
        ''' string to the shortened string. Commonly Chr(133) (ellipsis).
        ''' 
        ''' Will shorten the visible text no less than <paramref name="numberOfCharactersToReduceLengthBy"/> characters.
        ''' If <paramref name="appendStringToEnd"/> is used, it will continue to trim non-alpha-numeric characters 
        ''' before appending the string so you don't end up with "My String-...", etc.
        ''' </summary>
        ''' <param name="htmlString">The HTML formatted string to modify.</param>
        ''' <param name="numberOfCharactersToReduceLengthBy">The minimum number of characters that are trimmed from the 
        ''' HTML string. If <paramref name="appendStringToEnd"/> is used, it will keep trimming past this amount to 
        ''' ensure the appended string immediately follows an alpha-numeric character.</param>
        ''' <param name="appendStringToEnd">A string that's appended to the end of the HTML string. Commonly Chr(133), 
        ''' the Ellipsis.</param>
        ''' <returns>An HTML formatted string whose actual visible text is trimmed.</returns>
        ''' <remarks>
        ''' 1. Empty tags are removed as the string is trimmed.
        ''' 2. <nbsp> is treated as a single character.
        ''' 3. If "Append..." is used, the string WILL end with that.
        ''' </remarks>
        ''' <example>
        ''' Input: <code><p>My String!</p><p><size=14>Second Paragraph.</size></p></code>
        ''' 
        ''' 1. Trim by 5, append ellipsis
        '''    <p>My String!</p><p><size=14>Second Parag…</size></p>
        ''' 2. Trim by 10, append ellipsis (note that it trimmed 11 because of the space)
        '''    <p>My String!</p><p><size=14>Second…</size></p>
        ''' 3. Trim by 17, append ellipsis
        '''    <p>My String…</p>
        ''' </example>
        Public Function ReduceHtmlStringLengthBy(ByRef htmlString As String, Optional numberOfCharactersToReduceLengthBy As UInteger = 1, Optional appendStringToEnd As String = Nothing) As String

            Dim numberLeftToReduce As Integer = If(numberOfCharactersToReduceLengthBy < 0, 0, numberOfCharactersToReduceLengthBy)

            ' This splits the text based on the HTML tags. For example "Hello<p>World</p>" would become
            ' {"Hello", "<p>", "World", "</p>"}. This way, we can ONLY trim actual text and not tags.
            Dim splits = System.Text.RegularExpressions.Regex.Split(htmlString, "(</?[a-zA-Z0-9]*[^<>]*>)")

            ' Start at the end and work toward the beginning.
            ' Previous version used < and > to check if it was a tag, but that failed when "append" had a bracket.
            ' Instead, we should track when we added the append string so that we can remove it before tag detection.
            ' The index also means that the empty tag detection can exit when the actual affected string is found, 
            ' regardless of what the append string is.
            Dim appendedLastTime As Boolean = False
            Dim indexHasAppend As Integer = -1
            For i As Integer = splits.Count - 1 To 0 Step -1
                Dim splitWithoutAppendString As String = splits(i)
                If appendedLastTime Then
                    splitWithoutAppendString = splits(i).Substring(0, splits(i).Length - appendStringToEnd.Length)
                End If
                appendedLastTime = False
                ' Don't trim tags.
                If Not splitWithoutAppendString.StartsWith("<") AndAlso Not splitWithoutAppendString.EndsWith(">") AndAlso splitWithoutAppendString.Trim.Length > 0 Then
                    ' Trim off the trailing ellipsis append string if it exists.
                    If splits(i).StartsWith(splitWithoutAppendString) Then
                        splits(i) = splitWithoutAppendString
                    End If

                    ' Continue to trim the string until the last character is alpha-numeric, or the string
                    ' is empty. We don't want to have something like "Hello-...".
                    While splits(i).Length > 0 AndAlso (numberLeftToReduce > 0 OrElse (Not Regex.IsMatch(splits(i)(splits(i).Length - 1), "[A-Za-z0-9]") AndAlso appendStringToEnd <> Nothing))
                        splits(i) = splits(i).Substring(0, splits(i).Length - 1)
                        numberLeftToReduce -= 1
                    End While

                    ' If the string is not empty, add the append string to the end and test again.
                    If splits(i).Length > 0 Then
                        appendedLastTime = True
                        indexHasAppend = i
                        splits(i) = splits(i) & appendStringToEnd
                    End If

                    If numberLeftToReduce > 0 OrElse (appendStringToEnd <> Nothing AndAlso splits(i).Length = 0) Then
                        ' If we have an append string, we need to keep going because we cleared this string fully.
                        If numberLeftToReduce < 0 Then numberLeftToReduce = 0
                    Else
                        Exit For
                    End If

                ElseIf Not splits(i).Trim.ToLower <> "<nbsp>" Then
                    splits(i) = String.Empty
                    numberLeftToReduce -= 1
                    If appendStringToEnd = Nothing AndAlso numberLeftToReduce <= 0 Then
                        Exit For
                    Else
                        If numberLeftToReduce < 0 Then numberLeftToReduce = 0
                    End If
                End If
            Next

            ' New string can have empty tags at the end, so we need to trim those.
            ' This stack holds the regex that matches the opening tag plus the index of the closing tag.
            Dim closingTagStack As New Stack(Of Tuple(Of String, Integer))
            ' This list is the collection of indices that represent empty tags.
            Dim itemsToRemove As New List(Of Integer)
            For i As Integer = splits.Count - 1 To 0 Step -1
                If i = indexHasAppend OrElse (Not splits(i).StartsWith("<") AndAlso Not splits(i).EndsWith(">") AndAlso splits(i).Trim.Length > 0) Then
                    ' We found a non-trivial plain text string, so we stop trimming.
                    Exit For
                End If

                ' This is an ending tag
                Dim closingTagMatch = System.Text.RegularExpressions.Regex.Match(splits(i), "<\/\s*(?<tagName>[^>\s]*)\s*>")
                If closingTagMatch IsNot Nothing AndAlso closingTagMatch.Success Then
                    Dim regexPattern = $"<\s*{closingTagMatch.Groups("tagName")}[^>]*>"
                    closingTagStack.Push(New Tuple(Of String, Integer)(regexPattern, i))
                ElseIf closingTagStack.Count > 0 Then
                    Dim openingTagMatch = System.Text.RegularExpressions.Regex.Match(splits(i), closingTagStack.Peek.Item1)
                    If openingTagMatch IsNot Nothing AndAlso openingTagMatch.Success Then
                        itemsToRemove.Add(i)
                        itemsToRemove.Add(closingTagStack.Pop.Item2)
                    End If
                End If
            Next
            itemsToRemove.Sort()
            itemsToRemove.Reverse()
            Dim splitsList As New List(Of String)
            splitsList.AddRange(splits)
            For Each index As Integer In itemsToRemove
                splitsList.RemoveAt(index)
            Next

            ' Join the split collection to get your new string. Then recalculate the size.
            Dim newText = String.Join("", splitsList)
            Return newText

        End Function

#End Region ' Trim and Truncate

#Region "Formatting"

        ''' <summary>
        ''' Takes a string and adds spaces where it finds capital letters or underscores.
        ''' </summary>
        ''' <param name="text">The input text to add spaces to.</param>
        ''' <param name="sentenceOptions">The options used for formatting the string, including if acronyms are 
        ''' preserved, and which letters are capitalized. By default, acronyms ARE preserved and 
        ''' <see cref="SentenceFormatOptions.CapitalizationStrategies.TitleCase"/> is used.</param>
        Public Function AddSpacesToSentence(ByRef text As String,
                                            Optional sentenceOptions As SentenceFormatOptions = Nothing) As String

            If String.IsNullOrWhiteSpace(text) Then Return String.Empty

            If sentenceOptions Is Nothing Then sentenceOptions = New SentenceFormatOptions

            ' First, replace underscores with spaces. Trim white space before and after and eliminate double spaces.
            Dim textSource = text.Replace("_", " ").Trim
            While textSource.Contains("  ")
                textSource = textSource.Replace("  ", " ")
            End While

            ' Start our first word. It will always start with the first letter.
            Dim words As New List(Of String) From {textSource(0).ToString}

            For i As Integer = 1 To textSource.Length - 1

                If textSource(i) = " " Then
                    ' If this is a space, add a new empty word, but don't write any character.
                    words.Add(String.Empty)
                Else
                    ' Check if this is a new word.
                    If words.Last.Length > 0 Then ' If the previous word is empty, just add the next letter.
                        If Char.IsUpper(textSource(i)) Then
                            If sentenceOptions.PreserveAcronyms Then
                                If Char.IsUpper(words.Last.Last) AndAlso (i = textSource.Length - 1 OrElse Char.IsUpper(textSource(i + 1)) OrElse textSource(i + 1) = " ") Then
                                    ' If the previous letter was uppercase, and this is the end of the string OR the next
                                    ' is also uppercase, OR the next char is a space it's part of the previous acronym.
                                Else
                                    ' Otherwise, it's a new word
                                    words.Add(String.Empty)
                                End If
                            Else
                                ' If we don't preserve acronyms, we call it a new word right here.
                                words.Add(String.Empty)
                            End If
                        Else
                            ' Not a new word, it's lower case
                        End If
                    End If

                    words(words.Count - 1) = words(words.Count - 1) & textSource(i)
                End If
            Next

            ' Lastly, set the caps based on the options.
            For i As Integer = 0 To words.Count - 1
                words(i) = SetCapitalizationOfWordBasedOnStrategy(i, words(i), sentenceOptions)
            Next

            Return String.Join(" ", words)
        End Function

        ''' <summary>
        ''' Accepts a word, the index of the word within the sentence, and sentence options and decides which letters
        ''' in the word (if any) should be capitalized.
        ''' </summary>
        ''' <param name="index">The index of the word within the full sentence.</param>
        ''' <param name="word">The word itself.</param>
        ''' <param name="sentenceOptions">The options for setting the letter casing.</param>
        Private Function SetCapitalizationOfWordBasedOnStrategy(ByVal index As Integer, ByVal word As String, ByVal sentenceOptions As SentenceFormatOptions) As String

            Dim isWordAnAcronym As Boolean = (word.Length > 1 AndAlso word = word.ToUpper)
            If sentenceOptions.PreserveAcronyms AndAlso isWordAnAcronym Then
                ' If this word is an acronym and we're preserving acronyms, we should simply return it as is (caps).
                Return word
            End If

            ' Normalize the word to lower case to start.
            Dim formattedWord = word.ToLower.Trim

            Select Case sentenceOptions.CapitalizationStrategy
                Case SentenceFormatOptions.CapitalizationStrategies.AllWords
                    ' Return the word with the first letter capitalized.
                    Return formattedWord.Substring(0, 1).ToUpper & If(formattedWord.Length > 1, formattedWord.Substring(1), "")

                Case SentenceFormatOptions.CapitalizationStrategies.TitleCase
                    ' Return the word with the first letter capitalized if it's the first word, or it's not one of the 
                    ' words that should be lower case in titles.
                    If index = 0 Then Return formattedWord.Substring(0, 1).ToUpper & If(formattedWord.Length > 1, formattedWord.Substring(1), "")
                    If GetNonCapitalizedWords.Contains(formattedWord) Then Return formattedWord
                    Return formattedWord.Substring(0, 1).ToUpper & If(formattedWord.Length > 1, formattedWord.Substring(1), "")

                Case SentenceFormatOptions.CapitalizationStrategies.FirstWord
                    ' Return the word with the first letter capitalized if it's the first word in the sentence.
                    If index = 0 Then Return formattedWord.Substring(0, 1).ToUpper & If(formattedWord.Length > 1, formattedWord.Substring(1), "")
                    Return formattedWord

                Case SentenceFormatOptions.CapitalizationStrategies.None
                    ' Return the word as lowercase.
                    Return formattedWord

                Case Else
                    ' Return the word as it originally appeared.
                    Return word.Trim
            End Select

        End Function

        Private Function GetNonCapitalizedWords() As List(Of String)
            ' TODO - Need a tokenized translation that contains the list of lower case words for various languages.
            ' But since we're using this system to convert variable and class names to strings, and all of our code
            ' is in English, this is good for now.
            Return {"a", "an", "and", "but", "for", "in", "nor", "of", "on", "or", "the"}.ToList
        End Function

#End Region ' Formatting

#Region "Match Score"

        ''' <summary>
        ''' Returns a <see cref="StringMatchScoreInfo"/> (Range 0.0 to 100.0) based on how closely the 
        ''' <paramref name="inputText"/> matches the <paramref name="matchText"/>.
        ''' </summary>
        ''' <param name="inputText">The initial string to match</param>
        ''' <param name="matchText">The string to compare the <paramref name="inputText"/> with. When 
        ''' <paramref name="matchAny"/> is active, this is tokenized.</param>
        ''' <param name="matchAny">When <see langword="False"/>, the entire <paramref name="inputText"/> and 
        ''' <paramref name="matchText"/> are compared and scored. When <see langword="True"/>, both strings are split 
        ''' on whitespace, and each individual token is compared.</param>
        Public Function GetSimilarityScore(inputText As String, matchText As String, matchAny As Boolean) As StringMatchScoreInfo
            Return CalculateLevenshteinDistanceScore(matchText, inputText, matchAny)
        End Function

        ''' <summary>
        ''' Returns a <see cref="StringMatchScoreInfo"/> (Range 0.0 to 100.0) based on how closely the 
        ''' <paramref name="inputText"/> matches the regex <paramref name="pattern"/>.
        ''' </summary>
        ''' <param name="inputText">The initial string to match</param>
        ''' <param name="pattern">The regular expressions pattern to use. The overall score is based on how much of the 
        ''' <paramref name="inputText"/> is matched by the pattern. 
        ''' This uses default Regex Options <see cref="RegexOptions.Singleline"/> and 
        ''' <see cref="RegexOptions.IgnoreCase"/>.</param>
        Public Function GetRegexMatchScore(inputText As String, pattern As String) As StringMatchScoreInfo
            Return GetRegexMatchScore(inputText, pattern, RegexOptions.IgnoreCase Or RegexOptions.Singleline)
        End Function

        ''' <summary>
        ''' Returns a <see cref="StringMatchScoreInfo"/> (Range 0.0 to 100.0) based on how closely the 
        ''' <paramref name="inputText"/> matches the regex <paramref name="pattern"/>.
        ''' </summary>
        ''' <param name="inputText">The initial string to match</param>
        ''' <param name="pattern">The regular expressions pattern to use. The overall score is based on how much of the 
        ''' <paramref name="inputText"/> is matched by the pattern.</param>
        ''' <param name="regexOptions">The regex options to use when matching <paramref name="pattern"/>.</param>
        Public Function GetRegexMatchScore(inputText As String, pattern As String, regexOptions As RegexOptions) As StringMatchScoreInfo
            Dim regex As New Regex(pattern, regexOptions)
            Return GetRegexMatchScore(inputText, regex)
        End Function

        ''' <summary>
        ''' Returns a <see cref="StringMatchScoreInfo"/> (Range 0.0 to 100.0) based on how closely the 
        ''' <paramref name="inputText"/> matches the <paramref name="regex"/>.
        ''' </summary>
        ''' <param name="inputText">The initial string to match</param>
        ''' <param name="regex">The regular expression to use for matching. The overall score is based on how much of the 
        ''' <paramref name="inputText"/> is matched by this regex definition.</param>
        Public Function GetRegexMatchScore(inputText As String, regex As Regex) As StringMatchScoreInfo
            If regex Is Nothing Then Return New StringMatchScoreInfo(0.0, 0.0)
            If inputText = Nothing Then Return New StringMatchScoreInfo(0.0, 0.0)

            Dim success As Boolean = False
            Dim matchedChars As Integer = 0
            Dim percentMatch As Double = 0.0
            For Each match As System.Text.RegularExpressions.Match In regex.Matches(inputText)
                If match.Success Then success = True
                matchedChars += match.Length
            Next
            ' Score is calculated as the percentage of the character within the string that are matched.
            Dim matchScore = (1.0 - (CDbl(inputText.Length - matchedChars) / inputText.Length)) * 100.0
            Return New StringMatchScoreInfo(matchScore, matchScore)

        End Function

        Private Function CalculateLevenshteinDistanceScore(ByVal value As String, ByVal inputString As String, matchAny As Boolean) As StringMatchScoreInfo
            If inputString = Nothing Then Return New StringMatchScoreInfo(0.0, 0.0)

            If matchAny Then
                Dim maxScore As Double = 0.0
                Dim overallScore As Double = CalculateLevenshteinDistanceScore(value, inputString, False).OverallScore

                For Each valueToken In value.Split(" "c)
                    For Each inputToken In inputString.Split(" "c)
                        Dim score = CalculateLevenshteinDistanceScore(valueToken, inputToken, False)
                        If score.OverallScore > maxScore Then maxScore = score.OverallScore
                    Next
                Next
                Return New StringMatchScoreInfo((maxScore + overallScore) / 2.0, maxScore)
            Else
                Dim lowerDistance = GetLevenshteinDistance(If(value, String.Empty).ToLower, If(inputString, String.Empty).ToLower)
                Dim lowerScore = CDbl(lowerDistance) / CDbl(inputString.Length)

                Dim distance = GetLevenshteinDistance(value, inputString)
                Dim distanceScore = CDbl(lowerDistance) / CDbl(inputString.Length)

                Dim retScore = Math.Max(0.0, (1.0 - (((lowerScore * 4) + distanceScore) / 5.0)) * 100.0)
                Return New StringMatchScoreInfo(retScore, retScore)

            End If
        End Function

        Private Function GetLevenshteinDistance(ByVal value1 As String, ByVal value2 As String) As Integer
            If value1.Length = 0 Then
                Return 0
            End If

            Dim costs As Integer() = New Integer(value1.Length - 1) {}
            Dim i As Integer = 0

            While i < costs.Length
                costs(i) = System.Threading.Interlocked.Increment(i)
            End While

            Dim minSize As Integer = If(value1.Length < value2.Length, value1.Length, value2.Length)

            For i = 0 To minSize - 1
                Dim cost As Integer = i
                Dim previousCost As Integer = i
                Dim value2Char As Char = value2(i)

                For j As Integer = 0 To value1.Length - 1
                    Dim currentCost As Integer = cost
                    cost = costs(j)

                    If value2Char <> value1(j) Then

                        If previousCost < currentCost Then
                            currentCost = previousCost
                        End If

                        If cost < currentCost Then
                            currentCost = cost
                        End If

                        currentCost += 1
                    End If

                    costs(j) = currentCost
                    previousCost = currentCost
                Next
            Next

            Return costs(costs.Length - 1)
        End Function

#End Region

#Region "File Names"

        ''' <summary>
        ''' Takes a File Name or File Name and Path that may be invalid, and applies rules to make the path valid.
        ''' 1. Paths that are not valid will simply throw an exception and return False, as paths aren't 
        '''    malleable like file names.
        ''' 2. File Names will have illegal characters stripped, and will be truncated, and appended with an 
        '''    increment to satisfy valid file name rules and the parameters.
        ''' </summary>
        ''' <param name="fileNameOrFileNameAndPath">The file name without path or fully qualified file name and path. 
        ''' Will be modified if invalid.</param>
        ''' <param name="incrementIfFileExists">When True, if the file name already exists, a counter value is added 
        ''' to the end of it to ensure uniqueness. For example "MyFile (1).txt"</param>
        ''' <param name="replaceBadCharsWith">The string that is used for replacing bad characters. 
        ''' For example, "_" on file name "MyBad:FileName.txt" would result in "MyBad_FileName.txt"</param>
        ''' <param name="disallowAdditionalCharacters">Allows you to specify other character values that are not 
        ''' allowed within the file names beyond the file system limitations.</param>
        ''' <param name="disallowFileNameAndPaths">A collection of paths that are not allowed. Useful for creating a 
        ''' batch of file names that should be incremented, but don't yet exist.</param>
        ''' <returns>True if the validation is successful, False otherwise.</returns>
        Protected Friend Function MakeFileNameAndPathValidInternal(ByRef fileNameOrFileNameAndPath As String,
                                                    incrementIfFileExists As Boolean,
                                                    replaceBadCharsWith As String,
                                                    disallowAdditionalCharacters As IEnumerable(Of Char),
                                                    disallowFileNameAndPaths As IEnumerable(Of String)) As String

            Dim fileName As String = Nothing
            Dim maxFileLength = 255

            ' Because the user can specify what "bad" characters are replaced with, they can specify another "bad"
            ' character. Check for this and make sure this replacement string contains only valid file name characters.
            replaceBadCharsWith = ValidateBadCharReplacementString(replaceBadCharsWith, disallowAdditionalCharacters)

            If fileNameOrFileNameAndPath.Contains(IO.Path.DirectorySeparatorChar) OrElse
                fileNameOrFileNameAndPath.Contains(IO.Path.AltDirectorySeparatorChar) Then

                ' We know this is a FQN, not just a file.
                Dim directoryName As String = Nothing
                Dim maxComponentLength As Integer = 255 ' The default max component length
                Dim maxPathLength = 260

                ' Get the directory name (even if it has long components or invalid chars)
                ' and check it for invalid chars. Throw an exception if one is found.
                directoryName = GetDirectoryNameUnsafe(fileNameOrFileNameAndPath)
                If directoryName.EndsWith(":") Then directoryName = directoryName & IO.Path.DirectorySeparatorChar

                If DirectoryNameContainsInvalidCharacters(directoryName) Then
                    Throw New System.IO.IOException($"Directory name contains illegal characters: {directoryName}")
                    Return Nothing
                End If
                ' Detect the maximum component length for the current file system (max length of the file or
                ' folder name per level)
                maxComponentLength = GetMaxPathComponentLength(directoryName)
                ' Check if the directory name violates the file system length requirements,
                ' throw an exception if it does.
                If DirectoryNameViolatesLengthRules(directoryName, maxComponentLength) Then
                    Throw New System.IO.PathTooLongException($"Directory name violates file system length requirements: {directoryName}")
                    Return Nothing
                End If

                ' Get the file name (even if it has long components or invalid chars) and make it valid by
                ' replacing disallowed characters.
                fileName = GetFileNameUnsafe(fileNameOrFileNameAndPath)
                fileName = MakeFileNameValid(replaceBadCharsWith, disallowAdditionalCharacters, fileName)

                ' The maximum length the file name can be is the smaller of: (1) the max component length and
                ' (2) the available remaining characters in the max path length after the directory length is removed.
                maxFileLength = Math.Min(maxFileLength, maxPathLength - directoryName.Length - 1)
                If maxFileLength < 12 AndAlso maxFileLength < fileName.Length Then ' 8.3 format as a minimum
                    ' If the current file name needs to be truncated AND the resulting length of the file name is less
                    ' than 12 characters (an arbitrary length based on an 8 char file name and 3 char extension),
                    ' then we don't allow.
                    Throw New System.IO.PathTooLongException($"The file name and path is too long: {fileNameOrFileNameAndPath}")
                    Return Nothing
                End If

                ' Apply the max file length rule by truncating the file name (without extension) and reconstructing.
                Dim extension As String = IO.Path.GetExtension(fileName)
                Dim fileNameWithoutExtension As String = IO.Path.GetFileNameWithoutExtension(fileName)
                Dim proposedFileName = fileNameWithoutExtension.Truncate(maxFileLength - extension.Length) & extension

                ' If we are not worried about incrementing for files that exist, we simply return the file name and path.
                If Not incrementIfFileExists Then

                    Return IO.Path.Combine(directoryName, proposedFileName)
                End If

                ' The user has chosen to increment the path until it is unique.
                Dim incrementCount As Integer = 1
                ' This is a collection of FQNs that are not allowed. Generally, this is because we are correcting
                ' several names at once which may collide with each other.
                Dim disallowFqnList As New List(Of String)
                If disallowFileNameAndPaths IsNot Nothing Then
                    For Each disallowFqn In disallowFileNameAndPaths
                        If Not disallowFqnList.Contains(disallowFqn.ToLower.Trim) Then disallowFqnList.Add(disallowFqn.ToLower.Trim)
                    Next
                End If

                ' While the file exists or the full path is in the disallowed list, keep incrementing.
                ' Check the list first to avoid file IO if it's disallowed.
                While disallowFqnList.Find(Function(x)
                                               Return x = IO.Path.Combine(directoryName, proposedFileName).ToLower.Trim
                                           End Function) IsNot Nothing OrElse
                                           IO.File.Exists(IO.Path.Combine(directoryName, proposedFileName))

                    ' This method returns the "increment append" string based on the counter and disallowed characters.
                    Dim incrementString As String = GetIncrementString(incrementCount, disallowAdditionalCharacters)
                    ' Determine the maximum length of the file name without extension and increment append string.
                    Dim truncateLength = maxFileLength - extension.Length - incrementString.Length
                    If truncateLength < 1 Then
                        ' If the file name is reduced to 0 characters to make room for a very large append number,
                        ' we can't do anything so we throw an exception.
                        Throw New Exception("Could not increment file name to be unique and satisfy maximum length requirements.")
                        Return Nothing
                    End If
                    ' The new proposed name is the newly truncated file name without extension plus the increment
                    ' string and extension.
                    proposedFileName = fileNameWithoutExtension.Truncate(truncateLength) & incrementString & extension
                    incrementCount += 1 ' Prepare for next loop.
                End While

                ' We now know the proposed file name is unique, return the FQN.
                Return IO.Path.Combine(directoryName, proposedFileName)

            Else
                ' This is just a file name with no path component.
                fileName = fileNameOrFileNameAndPath
                fileName = MakeFileNameValid(replaceBadCharsWith, disallowAdditionalCharacters, fileName)

                Dim extension As String = IO.Path.GetExtension(fileName)
                Dim fileNameWithoutExtension As String = IO.Path.GetFileNameWithoutExtension(fileName)

                Dim proposedFileName = fileNameWithoutExtension.Truncate(maxFileLength - extension.Length) & extension

                ' If we aren't incrementing disallowed, we simply return.
                If Not incrementIfFileExists Then
                    Return proposedFileName
                End If

                ' For files, we throw away the path component for the disallow list.
                Dim disallowFqnList As New List(Of String)
                If disallowFileNameAndPaths IsNot Nothing Then
                    For Each disallowFqn In disallowFileNameAndPaths
                        disallowFqn = IO.Path.GetFileName(disallowFqn)
                        If Not disallowFqnList.Contains(disallowFqn.ToLower.Trim) Then
                            disallowFqnList.Add(disallowFqn.ToLower.Trim)
                        End If
                    Next
                End If
                ' While the proposed file name is disallowed, we increment.
                Dim incrementCount As Integer = 1
                While disallowFqnList.Find(Function(x)
                                               Return x = proposedFileName.ToLower.Trim
                                           End Function) IsNot Nothing

                    ' This method returns the "increment append" string based on the counter and disallowed characters.
                    Dim incrementString As String = GetIncrementString(incrementCount, disallowAdditionalCharacters)
                    ' Determine the maximum length of the file name without extension and increment append string.
                    Dim truncateLength = maxFileLength - extension.Length - incrementString.Length
                    If truncateLength < 1 Then
                        ' If the file name is reduced to 0 characters to make room for a very large append number,
                        ' we can't do anything so we throw an exception.
                        Throw New Exception("Could not increment file name to be unique and satisfy maximum length requirements.")
                        Return Nothing
                    End If
                    ' The new proposed name is the newly truncated file name without extension plus the increment
                    ' string and extension.
                    proposedFileName = fileNameWithoutExtension.Truncate(truncateLength) & incrementString & extension
                    incrementCount += 1 ' Prepare for next loop.
                End While

                Return proposedFileName

            End If
        End Function

        Private Function GetDirectoryNameUnsafe(ByVal fileNameAndPath As String) As String
            If fileNameAndPath.Contains(IO.Path.DirectorySeparatorChar) Then
                Dim directoryTokens = fileNameAndPath.Split(IO.Path.DirectorySeparatorChar).ToList
                Return String.Join(IO.Path.DirectorySeparatorChar, directoryTokens.Take(directoryTokens.Count - 1))
            ElseIf fileNameAndPath.Contains(IO.Path.AltDirectorySeparatorChar) Then
                Dim directoryTokens = fileNameAndPath.Split(IO.Path.AltDirectorySeparatorChar)
                Return String.Join(IO.Path.AltDirectorySeparatorChar, directoryTokens.Take(directoryTokens.Count - 1))
            Else
                Return Nothing
            End If
        End Function

        Private Function GetFileNameUnsafe(ByVal fileNameAndPath As String) As String
            Return fileNameAndPath.Split(IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar).ToList.LastOrDefault
        End Function

        Private Shared Function DirectoryNameContainsInvalidCharacters(directoryName As String) As Boolean
            For Each illegalChar In IO.Path.GetInvalidPathChars
                If directoryName.Contains(illegalChar) Then Return True
            Next

            Return False
        End Function

        Private Shared Function DirectoryNameViolatesLengthRules(directoryName As String,
                                                                 maxComponentLength As UInteger) As Boolean

            Dim tokens = directoryName.Split({IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar})
            For Each token In tokens
                If token.Length > maxComponentLength Then Return True
            Next
            Return False
        End Function

        Private Function GetIncrementString(ByVal incrementCount As Integer, ByVal disallowAdditionalCharacters As Char()) As String
            If disallowAdditionalCharacters IsNot Nothing AndAlso disallowAdditionalCharacters.Count > 0 Then
                If disallowAdditionalCharacters.Contains(" ") OrElse disallowAdditionalCharacters.Contains("(") OrElse disallowAdditionalCharacters.Contains(")") Then
                    If disallowAdditionalCharacters.Contains("_") Then
                        Return $"{incrementCount}"
                    Else
                        Return $"_{incrementCount}"
                    End If
                Else
                    Return $" ({incrementCount})"
                End If
            Else
                Return $" ({incrementCount})"
            End If
        End Function

        Private Shared Function MakeFileNameValid(replaceBadCharsWith As String, disallowAdditionalCharacters() As Char, fileName As String) As String
            For Each illegalChar In IO.Path.GetInvalidFileNameChars
                fileName = fileName.Replace(illegalChar, replaceBadCharsWith)
            Next
            If disallowAdditionalCharacters IsNot Nothing Then
                For Each illegalChar In disallowAdditionalCharacters
                    fileName = fileName.Replace(illegalChar, replaceBadCharsWith)
                Next
            End If

            Return fileName
        End Function

        Private Shared Function ValidateBadCharReplacementString(replaceBadCharsWith As String, disallowAdditionalCharacters() As Char) As String
            If replaceBadCharsWith <> Nothing Then
                For Each illegalChar In IO.Path.GetInvalidFileNameChars
                    replaceBadCharsWith = replaceBadCharsWith.Replace(illegalChar, "")
                Next
                If disallowAdditionalCharacters IsNot Nothing Then
                    For Each illegalChar In disallowAdditionalCharacters
                        replaceBadCharsWith = replaceBadCharsWith.Replace(illegalChar, "")
                    Next
                End If
                If replaceBadCharsWith = Nothing Then
                    If disallowAdditionalCharacters.Contains("_") Then
                        replaceBadCharsWith = ""
                    Else
                        replaceBadCharsWith = "_"
                    End If
                End If
            End If

            Return replaceBadCharsWith
        End Function

        ''' <summary>
        ''' Returns volume name and serial number, maximum path component length, and filesystem name and flags
        ''' for the specified directory. Works with UNC. Can throw an exception.
        ''' </summary>
        ''' <param name="folderPath">Path to drive or shared folder to find about.</param>
        ''' <remarks></remarks>
        Private Function GetMaxPathComponentLength(ByVal folderPath As String) As Integer

            Dim rootFolder As String = IO.Path.GetPathRoot(folderPath)
            If Not rootFolder.EndsWith(IO.Path.DirectorySeparatorChar) Then rootFolder &= IO.Path.DirectorySeparatorChar

            Const MAX_PATH As Integer = &H104

            Dim volumeName As New System.Text.StringBuilder(MAX_PATH + 1)
            Dim fileSystemName As New System.Text.StringBuilder(MAX_PATH + 1)
            Dim volumeSerialNumber As UInt32
            Dim maxComponentLength As UInt32
            Dim fileSystemFlags As UInt32

            Dim status As UInteger = GetVolumeInformation(rootFolder, volumeName, volumeName.Capacity,
                          volumeSerialNumber, maxComponentLength, fileSystemFlags,
                          fileSystemName, fileSystemName.Capacity)
            If status = 0 Then Throw New System.ComponentModel.Win32Exception(Err.LastDllError)
            Return maxComponentLength
        End Function

        Private Declare Auto Function GetVolumeInformation Lib "kernel32.dll" (
             ByVal RootPathName As String,
             ByVal VolumeNameBuffer As System.Text.StringBuilder,
             ByVal VolumeNameSize As UInt32,
             ByRef VolumeSerialNumber As UInt32,
             ByRef MaximumComponentLength As UInt32,
             ByRef FileSystemFlags As UInt32,
             ByVal FileSystemNameBuffer As System.Text.StringBuilder,
             ByVal FileSystemNameSize As UInt32) As UInt32

#End Region

    End Class
End Namespace