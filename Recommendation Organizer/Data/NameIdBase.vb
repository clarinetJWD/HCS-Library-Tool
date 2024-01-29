Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

Public Class NameIdBase : Implements INotifyPropertyChanged, IConvertible, ISupportHasChanges, INameId, IEquatable(Of NameIdBase)

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Property Name As String Implements IName.Name
        Get
            Return _Name
        End Get
        Set(value As String)
            If _Name <> value Then
                HasChanges = True
                _Name = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Name)))
            End If
        End Set
    End Property
    Private _Name As String

    <Display(AutoGenerateField:=False)>
    Property Id As String Implements IId.Id
        Get
            If _Id = Nothing Then
                _Id = Guid.NewGuid.ToString
            End If
            Return _Id
        End Get
        Set(value As String)
            If _Id <> value Then
                HasChanges = True
                _Id = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Id)))
            End If
        End Set
    End Property
    Private _Id As String

    <Editable(False)>
    <Display(AutoGenerateField:=False)>
    Property HasChanges As Boolean Implements ISupportHasChanges.HasChanges
        Get
            Return _HasChanges
        End Get
        Set(value As Boolean)
            If _HasChanges <> value Then
                _HasChanges = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
            End If
        End Set
    End Property
    Private _HasChanges As Boolean = True

    Sub New(name As String)
        Me.Id = Guid.NewGuid.ToString
        Me.Name = name
    End Sub

    Sub New()

    End Sub

    Sub RaisePropertyChangedEvent(propertyName)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Public Overrides Function ToString() As String
        Return Name
    End Function

    Public Function GetTypeCode() As TypeCode Implements IConvertible.GetTypeCode
        Return TypeCode.String
    End Function

    Public Function ToBoolean(provider As IFormatProvider) As Boolean Implements IConvertible.ToBoolean
        Throw New NotImplementedException()
    End Function

    Public Function ToChar(provider As IFormatProvider) As Char Implements IConvertible.ToChar
        Throw New NotImplementedException()
    End Function

    Public Function ToSByte(provider As IFormatProvider) As SByte Implements IConvertible.ToSByte
        Throw New NotImplementedException()
    End Function

    Public Function ToByte(provider As IFormatProvider) As Byte Implements IConvertible.ToByte
        Throw New NotImplementedException()
    End Function

    Public Function ToInt16(provider As IFormatProvider) As Short Implements IConvertible.ToInt16
        Throw New NotImplementedException()
    End Function

    Public Function ToUInt16(provider As IFormatProvider) As UShort Implements IConvertible.ToUInt16
        Throw New NotImplementedException()
    End Function

    Public Function ToInt32(provider As IFormatProvider) As Integer Implements IConvertible.ToInt32
        Throw New NotImplementedException()
    End Function

    Public Function ToUInt32(provider As IFormatProvider) As UInteger Implements IConvertible.ToUInt32
        Throw New NotImplementedException()
    End Function

    Public Function ToInt64(provider As IFormatProvider) As Long Implements IConvertible.ToInt64
        Throw New NotImplementedException()
    End Function

    Public Function ToUInt64(provider As IFormatProvider) As ULong Implements IConvertible.ToUInt64
        Throw New NotImplementedException()
    End Function

    Public Function ToSingle(provider As IFormatProvider) As Single Implements IConvertible.ToSingle
        Throw New NotImplementedException()
    End Function

    Public Function ToDouble(provider As IFormatProvider) As Double Implements IConvertible.ToDouble
        Throw New NotImplementedException()
    End Function

    Public Function ToDecimal(provider As IFormatProvider) As Decimal Implements IConvertible.ToDecimal
        Throw New NotImplementedException()
    End Function

    Public Function ToDateTime(provider As IFormatProvider) As Date Implements IConvertible.ToDateTime
        Throw New NotImplementedException()
    End Function

    Public Function CToString(provider As IFormatProvider) As String Implements IConvertible.ToString
        Return Name
    End Function

    Public Function ToType(conversionType As Type, provider As IFormatProvider) As Object Implements IConvertible.ToType
        If conversionType Is GetType(String) Then
            Return Name
        End If
        Return Nothing
    End Function

    Public Shadows Function Equals(other As NameIdBase) As Boolean Implements IEquatable(Of NameIdBase).Equals
        If Me.Id = other.Id AndAlso Me.Name = other.Name Then Return True
        Return False
    End Function

    Shared Operator =(value1 As NameIdBase, value2 As NameIdBase) As Boolean
        If value1 Is Nothing AndAlso value2 Is Nothing Then Return True
        If value1 Is Nothing Then Return False
        If value2 Is Nothing Then Return False

        Return value1.Equals(value2)
    End Operator

    Shared Operator <>(value1 As NameIdBase, value2 As NameIdBase) As Boolean
        Return Not value1 = value2
    End Operator
End Class