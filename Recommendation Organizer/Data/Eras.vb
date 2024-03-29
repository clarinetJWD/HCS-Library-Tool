﻿Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

Public Class Eras : Inherits BindingList(Of Era) : Implements ISupportHasChanges, INotifyPropertyChanged

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

    Friend Function GetErasTokenString() As String
        Return String.Join(", ", Me.ToList.Select(Function(x) If(x?.Name, String.Empty)).ToList.FindAll(Function(x) x <> Nothing))
    End Function

    Friend Function Find(item As Era) As Era
        If item Is Nothing Then Return Nothing

        For Each listItem In Me
            If listItem IsNot Nothing AndAlso listItem.Id = item.Id Then
                Return listItem
            End If
        Next
        For Each listItem In Me
            If listItem IsNot Nothing AndAlso listItem.Name = item.Name Then
                Return listItem
            End If
        Next
        Return Nothing
    End Function

    Friend Function Find(item As String) As Era
        If item = Nothing Then Return Nothing

        For Each listItem In Me
            If If(listItem.Name <> Nothing, listItem.Name.ToUpper, String.Empty) = item.ToUpper.Trim Then
                Return listItem
            End If
        Next
        Return Nothing
    End Function

End Class

Public Class Era : Inherits NameIdBase : Implements IEquatable(Of Era)

    <Xml.Serialization.XmlIgnore>
    Property Color As Color
        Get
            Return _Color
        End Get
        Set(value As Color)
            If value <> _Color Then
                HasChanges = True
                _Color = value
                RaisePropertyChangedEvent(NameOf(Color))
            End If
        End Set
    End Property
    Private _Color As Color = Color.Transparent

    <Display(AutoGenerateField:=False)>
    <Xml.Serialization.XmlElement(ElementName:="Color")>
    Property ColorString As String
        Get
            Return $"{Color.R},{Color.G},{Color.B}"
        End Get
        Set(value As String)
            Dim colorTokens = value.Split(",")
            If colorTokens.Count = 3 Then
                Dim r As Integer
                Dim g As Integer
                Dim b As Integer
                If Integer.TryParse(colorTokens(0), r) Then
                    If Integer.TryParse(colorTokens(1), g) Then
                        If Integer.TryParse(colorTokens(2), b) Then
                            Me.Color = Color.FromArgb(r, g, b)
                        End If
                    End If
                End If
            End If
        End Set
    End Property

    Shared Widening Operator CType(value As String) As Era
        Return New Era(value)
    End Operator

    Shared Widening Operator CType(value As Era) As String
        Return value?.Name
    End Operator

    Sub New(name As String)
        MyBase.New(name)
    End Sub

    Sub New()
        MyBase.New
    End Sub

    Public Shadows Function Equals(other As Era) As Boolean Implements IEquatable(Of Era).Equals
        Return MyBase.Equals(other)
    End Function

End Class

Public Class ErasTypeConverter : Inherits TypeConverter

    Shared Property StandardEras As Eras

    Public Overrides Function GetStandardValues(context As ITypeDescriptorContext) As StandardValuesCollection
        Return New StandardValuesCollection(StandardEras.Select(Function(x) x.Name))
    End Function
End Class