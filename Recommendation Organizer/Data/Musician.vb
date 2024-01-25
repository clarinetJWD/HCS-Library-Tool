Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

Public Class Musician : Implements INotifyPropertyChanged, ISupportHasChanges

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Property FirstName As String
        Get
            Return _FirstName
        End Get
        Set(value As String)
            If _FirstName <> value Then
                HasChanges = True
                _FirstName = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(FirstName)))
            End If
        End Set
    End Property
    Private _FirstName As String

    Property LastName As String
        Get
            Return _LastName
        End Get
        Set(value As String)
            If _LastName <> value Then
                HasChanges = True
                _LastName = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LastName)))
            End If
        End Set
    End Property
    Private _LastName As String

    <Editable(False)>
    <Display(AutoGenerateField:=False)>
    Property HasChanges As Boolean Implements ISupportHasChanges.HasChanges
        Get
            Return _HasChanges
        End Get
        Set(value As Boolean)
            _HasChanges = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
        End Set
    End Property
    Private _HasChanges As Boolean

    Public Overrides Function ToString() As String
        Return $"{LastName}, {FirstName}"
    End Function

End Class
