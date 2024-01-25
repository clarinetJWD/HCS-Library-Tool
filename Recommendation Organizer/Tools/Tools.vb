Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Module Tools
    Function ListsAreSame(list1 As IList, list2 As IList) As Boolean
        If list1.Count <> list2.Count Then Return False
        For i As Integer = 0 To list1.Count - 1
            If list1(i) <> list2(i) Then Return False
        Next
        Return True
    End Function

    ''' <summary>
    ''' Removes all characters from the string, except for letters.
    ''' </summary>
    ''' <param name="inputString"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <System.Diagnostics.DebuggerStepThrough()>
    <Extension>
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
    <Extension>
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
    <Extension>
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
    <Extension>
    Public Function MakeNumeric(ByRef inputString As String, Optional allowDecimalPoint As Boolean = False) As String
        If allowDecimalPoint Then
            Return Regex.Replace(inputString, "[^0-9.]+", "", RegexOptions.Compiled)
        Else
            Return Regex.Replace(inputString, "[^0-9]+", "", RegexOptions.Compiled)
        End If
    End Function
End Module
