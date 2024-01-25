Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports Csv

Public Class RepertoireEntry : Implements IComposerTitle

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Property Title As String Implements ITitle.Title
        Get
            Return _Title
        End Get
        Set(value As String)
            If _Title <> value Then
                HasChanges = True
                _Title = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Title)))
            End If

        End Set
    End Property
    Private _Title As String

    Property Composer As String Implements IComposer.Composer
        Get
            Return _Composer
        End Get
        Set(value As String)
            If _Composer <> value Then
                HasChanges = True
                _Composer = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Composer)))
            End If
        End Set
    End Property
    Private _Composer As String

    <Display(AutoGenerateField:=False)>
    Property Years As BindingList(Of Integer)
        Get
            Return _Years
        End Get
        Set(value As BindingList(Of Integer))
            If _Years IsNot value Then
                HasChanges = True
                _Years = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Years)))
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LastPerformed)))
            End If
        End Set
    End Property
    Private WithEvents _Years As New BindingList(Of Integer)
    Private Sub OnYearsListChanged(sender As Object, e As ListChangedEventArgs) Handles _Years.ListChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LastPerformed)))
        HasChanges = True
    End Sub

    ReadOnly Property LastPerformed As Integer
        Get
            Return _Years.Max
        End Get
    End Property

    <Editable(False)>
    <Display(AutoGenerateField:=False)>
    Property HasChanges As Boolean
        Get
            Return _HasChanges
        End Get
        Set(value As Boolean)
            _HasChanges = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
        End Set
    End Property
    Private _HasChanges As Boolean

    Friend Shared Function GetEntryFromCsvLine(line As ICsvLine) As RepertoireEntry
        Dim newEntry As New RepertoireEntry

        If line(0) = Nothing Then
            Return Nothing
        End If

        newEntry.Title = line(0)
        newEntry.Composer = If(line.ColumnCount > 1, line(1), Nothing)
        Dim years = If(line.ColumnCount > 2, line(2), String.Empty)
        Dim yearsList As New List(Of Integer)

        For Each match As System.Text.RegularExpressions.Match In System.Text.RegularExpressions.Regex.Matches(years, "[0-9\?][0-9\?][0-9\?][0-9\?]")
            If match.Success Then
                yearsList.Add(CInt(match.Value.Replace("?", "0")))
            End If
        Next

        yearsList = yearsList.Distinct.ToList
        yearsList.Sort(Function(x, y) y.CompareTo(x))

        For Each yearInt In yearsList
            newEntry.Years.Add(yearInt)
        Next

        Return newEntry
    End Function

    Public Overrides Function ToString() As String
        Return $"{Composer}: {Title}"
    End Function

End Class
