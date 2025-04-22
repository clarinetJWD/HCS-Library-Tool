Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports Csv

Public Class MusicianRecommendation : Implements INotifyPropertyChanged, IComposerTitleKey, ISupportHasChanges

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged


    <Display(AutoGenerateField:=False)>
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


    <Display(AutoGenerateField:=False)>
    Property Key As String Implements IKey.Key
        Get
            Return _Key
        End Get
        Set(value As String)
            If _Key <> value Then
                HasChanges = True
                _Key = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Key)))
            End If
        End Set
    End Property
    Private _Key As String

    Property TimeStamp As Date
        Get
            Return _TimeStamp
        End Get
        Set(value As Date)
            If _TimeStamp <> value Then
                HasChanges = True
                _TimeStamp = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimeStamp)))
            End If
        End Set
    End Property
    Private _TimeStamp As Date

    Property Musician As Musician
        Get
            Return _Musician
        End Get
        Set(value As Musician)
            If _Musician IsNot value Then
                HasChanges = True
                _Musician = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Musician)))
            End If
        End Set
    End Property
    Private WithEvents _Musician As Musician
    Private Sub OnMusicianPropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _Musician.PropertyChanged
        HasChanges = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Musician)))
    End Sub

    Property Comments As String
        Get
            Return _Comments
        End Get
        Set(value As String)
            If _Comments <> value Then
                HasChanges = True
                _Comments = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Comments)))
            End If
        End Set
    End Property
    Private _Comments As String

    Property AdditionalThoughts As String
        Get
            Return _AdditionalThoughts
        End Get
        Set(value As String)
            If _AdditionalThoughts <> value Then
                HasChanges = True
                _AdditionalThoughts = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(AdditionalThoughts)))
            End If
        End Set
    End Property
    Private _AdditionalThoughts As String

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

    Friend Shared Function GetRecommendationsFromCsvLine(line As ICsvLine) As IEnumerable(Of MusicianRecommendation)
        Dim ret As New List(Of MusicianRecommendation)

        Try
            Dim timeStamp As DateTime = Date.Parse(line(0))
            Dim firstName As String = line(1)
            Dim lastName As String = line(2)
            Dim addlThoughts As String = If(line.ColumnCount > 18, line(18), Nothing)

            Dim musician = New Musician() With {.FirstName = firstName.Trim, .LastName = lastName.Trim}

            For i As Integer = 0 To 4
                Dim composerName = If(line.ColumnCount > 3 + (i * 3), line(3 + (i * 3)).Trim, Nothing)
                Dim title = If(line.ColumnCount > 4 + (i * 3), line(4 + (i * 3)).Trim, Nothing)
                Dim comment = If(line.ColumnCount > 5 + (i * 3), line(5 + (i * 3)).Trim, Nothing)

                If title <> Nothing Then
                    Dim key = firstName.Trim.ToUpper & lastName.Trim.ToUpper & timeStamp.ToString("yyyyMMddHHmmss") & composerName.Trim.ToUpper & title.Trim.ToUpper
                    Dim newRec As New MusicianRecommendation() With {.Composer = composerName, .Title = title, .Comments = comment, .Musician = musician, .AdditionalThoughts = addlThoughts, .TimeStamp = timeStamp, .Key = key}
                    ret.Add(newRec)
                End If
            Next

            Return ret
        Catch ex As Exception
            DevExpress.XtraEditors.XtraMessageBox.Show($"Could not import line: {Environment.NewLine}""{String.Join(""",""", line.Values)}""{Environment.NewLine}{Environment.NewLine}With error message '{ex.Message}'", "Import Error")
            Return ret
        End Try

    End Function

    Friend Sub UpdateFrom(mRec As MusicianRecommendation)
        Me.Composer = mRec.Composer
        Me.Title = mRec.Title

        Dim allComments As New List(Of String)
        If Me.Comments <> Nothing Then
            allComments.AddRange(Me.Comments.Split({vbLf, vbCrLf, Environment.NewLine}, StringSplitOptions.None))
        End If
        allComments.Add(mRec.Comments)

        allComments = allComments.Distinct.ToList

        Me.Comments = String.Join(Environment.NewLine, allComments)
    End Sub

    Public Overrides Function ToString() As String
        Return $"{Composer}: {Title} (by {Musician})"
    End Function

End Class
