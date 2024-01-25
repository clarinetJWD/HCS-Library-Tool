Imports System.ComponentModel

Public Class PublishedSeasonIndexes : Inherits BindingList(Of PublishedSeasonIndex)

End Class

Public Class PublishedSeasonIndex : Implements IName, ISupportHasChanges, INotifyPropertyChanged

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

    Property ftpPath As String
        Get
            Return _ftpPath
        End Get
        Set(value As String)
            If _ftpPath <> value Then
                HasChanges = True
                _ftpPath = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ftpPath)))
            End If
        End Set
    End Property
    Private _ftpPath As String

    Property LastModified As Date
        Get
            Return _LastModified
        End Get
        Set(value As Date)
            If _LastModified <> value Then
                HasChanges = True
                _LastModified = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LastModified)))
            End If
        End Set
    End Property
    Private _LastModified As Date

    Property IsDeleted As Boolean
        Get
            Return _IsDeleted
        End Get
        Set(value As Boolean)
            If _IsDeleted <> value Then
                HasChanges = True
                _IsDeleted = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsDeleted)))
            End If
        End Set
    End Property
    Private _IsDeleted As Boolean = False

    Public Property HasChanges As Boolean Implements ISupportHasChanges.HasChanges
        Get
            Return _HasChanges
        End Get
        Set(value As Boolean)
            _HasChanges = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
        End Set
    End Property
    Private _HasChanges As Boolean = True

    Public Overrides Function ToString() As String
        If IsDeleted Then
            Return $"{Name} [Unsaved]"
        Else
            Return $"{Name} [{LastModified}]"
        End If
    End Function

End Class