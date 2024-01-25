Imports System.ComponentModel

Public Class Tags : Inherits BindingList(Of Tag) : Implements ISupportHasChanges, INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property HasChanges As Boolean Implements ISupportHasChanges.HasChanges
        Get
            If _HasChanges Then Return True
            For Each era In Me
                If era.HasChanges Then Return True
            Next
            Return False
        End Get
        Set(value As Boolean)
            _HasChanges = value
            If Not value Then
                For Each era In Me
                    era.HasChanges = value
                Next
            End If
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
        End Set
    End Property
    Private _HasChanges As Boolean = True

    Private Sub OnListChangedHandler(sender As Object, e As ListChangedEventArgs) Handles Me.ListChanged
        If e.ListChangedType = ListChangedType.ItemAdded OrElse e.ListChangedType = ListChangedType.ItemDeleted Then
            _HasChanges = True
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
        End If
    End Sub

    Public Overrides Function ToString() As String
        Dim eraList = Me.ToList.Select(Function(x) x.Name).ToList
        Return String.Join(", ", eraList.FindAll(Function(x) x <> Nothing))
    End Function

    Friend Function Find(item As Tag) As Tag
        If item Is Nothing Then Return Nothing

        For Each listItem In Me
            If listItem.Id = item.Id Then
                Return listItem
            End If
        Next
        For Each listItem In Me
            If listItem.Name = item.Name Then
                Return listItem
            End If
        Next
        Return Nothing
    End Function

End Class

Public Class Tag : Inherits NameIdBase
    Shared Widening Operator CType(value As String) As Tag
        Return New Tag(value)
    End Operator

    Shared Widening Operator CType(value As Tag) As String
        Return value.Name
    End Operator

    Sub New(name As String)
        MyBase.New(name)
    End Sub

    Sub New()
        MyBase.New
    End Sub
End Class

Public Class TagsTypeConverter : Inherits TypeConverter

    Shared Property StandardTags As Tags

    Public Overrides Function GetStandardValues(context As ITypeDescriptorContext) As StandardValuesCollection
        Return New StandardValuesCollection(StandardTags.Select(Function(x) x.Name))
    End Function
End Class