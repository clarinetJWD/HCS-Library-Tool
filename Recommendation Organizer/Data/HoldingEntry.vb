Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports Csv

Public Class HoldingEntry : Implements IComposerArrangerTitle, ISupportHasChanges

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Property Title As String Implements ITitle.Title
        Get
            Return _Title
        End Get
        Set(value As String)
            If _Title <> value Then
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

    Property Arranger As String Implements IArranger.Arranger
        Get
            Return _Arranger
        End Get
        Set(value As String)
            If _Arranger <> value Then
                HasChanges = True
                _Arranger = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Arranger)))
            End If

        End Set
    End Property
    Private _Arranger As String

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

    Friend Shared Function GetEntryFromCsvLine(line As ICsvLine) As HoldingEntry
        Dim newEntry As New HoldingEntry

        If line(5) = Nothing Then
            Return Nothing
        End If

        Dim arrangerLName As String = If(line.ColumnCount > 6, line(6), Nothing)
        Dim arrangerFName As String = If(line.ColumnCount > 7, line(7), Nothing)
        Dim composerLName As String = If(line.ColumnCount > 3, line(3), Nothing)
        Dim composerFName As String = If(line.ColumnCount > 4, line(4), Nothing)

        Dim arrangerName As String = String.Empty
        Dim composerName As String = String.Empty

        If arrangerLName <> Nothing Then
            arrangerName = arrangerLName & If(arrangerFName = Nothing, String.Empty, ", " & arrangerFName)
        End If

        If composerLName <> Nothing Then
            composerName = composerLName & If(composerFName = Nothing, String.Empty, ", " & composerFName)
        End If

        newEntry.Title = If(line.ColumnCount > 5, line(5), Nothing)
        newEntry.Composer = composerName
        newEntry.Arranger = arrangerName

        Return newEntry
    End Function

    Public Overrides Function ToString() As String
        Return $"{Composer}: {Title}"
    End Function

End Class
