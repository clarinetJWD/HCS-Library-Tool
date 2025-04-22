Imports System.Runtime.CompilerServices

Namespace Extensions.EnumExtensions
    <HideModuleName>
    Public Module M_EnumExtensions

        Private _EnumExtensions As New EnumExtensions

        <Extension>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function GetFlags(Of Tenum As {IComparable, IFormattable, IConvertible})(ByRef value As Tenum) As IEnumerable(Of Tenum)
            Return _EnumExtensions.GetFlags(value)
        End Function

        ''' <summary>
        ''' Adds given flag if not present.
        ''' </summary>
        <Extension>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Sub AddEnumFlag(Of Tenum As {IComparable, IFormattable, IConvertible})(ByRef value As Tenum, flag As Tenum)
            _EnumExtensions.AddEnumFlag(Of Tenum)(value, flag)
        End Sub

        ''' <summary>
        ''' Removes given flag if present. 
        ''' </summary>
        <Extension>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Sub RemoveEnumFlag(Of Tenum As {IComparable, IFormattable, IConvertible})(ByRef value As Tenum, flag As Tenum)
            _EnumExtensions.RemoveEnumFlag(Of Tenum)(value, flag)
        End Sub


        ''' <summary>
        ''' Performs [Enum].HasFlag on an array of flags.  
        ''' Returns TRUE if value contains at least one flag in the array.
        ''' </summary>
        ''' <param name="allRequired">
        ''' If TRUE, the value must contain all flags
        ''' </param>
        <Extension>
        <System.Diagnostics.DebuggerStepThrough()>
        Public Function HasFlagMulti(Of Tenum As {IComparable, IFormattable, IConvertible})(ByRef value As Tenum, flags As IEnumerable(Of Tenum), Optional allRequired As Boolean = False) As Boolean
            Return _EnumExtensions.HasFlagMulti(Of Tenum)(value, flags, allRequired)
        End Function

    End Module

    Public Class EnumExtensions

        Overridable Function GetFlags(Of Tenum As {IComparable, IFormattable, IConvertible})(ByRef value As Tenum) As IEnumerable(Of Tenum)
            CheckIsEnumType(GetType(Tenum))

            Dim hasFlags As New List(Of Tenum)
            For Each flagValue As Tenum In [Enum].GetValues(value.GetType())
                If CObj(value).HasFlag(CObj(flagValue)) Then
                    hasFlags.Add(flagValue)
                End If
            Next

            Return hasFlags
        End Function

        ''' <summary>
        ''' Adds given flag if not present.
        ''' </summary>
        Overridable Sub AddEnumFlag(Of Tenum As {IComparable, IFormattable, IConvertible})(ByRef value As Tenum, flag As Tenum)

            CheckIsEnumType(GetType(Tenum))

            If value Is Nothing Then value = flag
            If flag Is Nothing Then Exit Sub

            Dim result As Tenum = value
            If DirectCast(result, Object).HasFlag(flag) = False Then
                result = CObj(result) Or CObj(flag)
            End If
            If DirectCast(result, Object) IsNot DirectCast(value, Object) Then
                value = result
            End If
        End Sub

        ''' <summary>
        ''' Removes given flag if present. 
        ''' </summary>
        Overridable Sub RemoveEnumFlag(Of Tenum As {IComparable, IFormattable, IConvertible})(ByRef value As Tenum, flag As Tenum)

            CheckIsEnumType(GetType(Tenum))

            If value Is Nothing OrElse flag Is Nothing Then Exit Sub

            Dim result As Tenum = value
            If DirectCast(result, Object).HasFlag(flag) = True Then
                result = CObj(result) Xor CObj(flag)
            End If
            If DirectCast(result, Object) IsNot DirectCast(value, Object) Then
                value = result
            End If
        End Sub


        ''' <summary>
        ''' Performs [Enum].HasFlag on an array of flags.  
        ''' Returns TRUE if value contains at least one flag in the array.
        ''' </summary>
        ''' <param name="allRequired">
        ''' If TRUE, the value must contain all flags
        ''' </param>
        Overridable Function HasFlagMulti(Of Tenum As {IComparable, IFormattable, IConvertible})(ByRef value As Tenum, flags As IEnumerable(Of Tenum), Optional allRequired As Boolean = False) As Boolean

            CheckIsEnumType(GetType(Tenum))

            Dim hasAny As Boolean = False
            Dim hasAll As Boolean = True

            For Each flag As Tenum In flags
                If DirectCast(value, Object).HasFlag(flag) = True Then
                    hasAny = True
                    If Not allRequired Then Return True
                Else
                    hasAll = False
                    If allRequired Then Return False
                End If
            Next
            Return If(allRequired, hasAll, hasAny)
        End Function

        Private Sub CheckIsEnumType(type As Type)
            If Not (GetType([Enum]).IsAssignableFrom(type) OrElse (type.IsEnum)) Then
                Throw New FormatException($"Type {type} must be an enum type.")
            End If
        End Sub

    End Class
End Namespace