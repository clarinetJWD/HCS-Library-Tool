Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Threading
Imports Csv
Imports FluentFTP

Public Class Form1
    Private ReadOnly Property _Library As LibraryData

    Private _Context As SynchronizationContext
    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _Context = Threading.SynchronizationContext.Current
        AddHandler Application.Idle, AddressOf LoadOnApplicationIdle
    End Sub

    Private Async Sub LoadOnApplicationIdle(sender As Object, e As EventArgs)
        RemoveHandler Application.Idle, AddressOf LoadOnApplicationIdle

        GridControl1.DataSource = Nothing
        GridControlComposers.DataSource = Nothing

        If Not LoadOrPromptPassword() Then
            Exit Sub
        End If

        Me.RepositoryItemProgressBar1.Minimum = 0
        Me.RepositoryItemProgressBar1.Maximum = 100
        Me.BarEditItem1.EditValue = 0
        BarManager1.StatusBar.Visible = True

        Await Task.Run(Sub() LoadLibraryData())

        BarManager1.StatusBar.Visible = False
        Me.BarEditItem1.EditValue = 0

        GridControl1.DataSource = _Library.RecommendationList
        GridView1.Columns("Composer").SortIndex = 0
        GridView1.Columns("Title").SortIndex = 1

        GridControlComposers.DataSource = _Library.Composers
        GridViewComposers.Columns("PrimaryName").SortIndex = 0
    End Sub

    Private Function LoadOrPromptPassword() As Boolean
        If Not FtpAndSecurity.TestPasscode(My.Settings.Passcode) Then
            Dim fPass As New PasscodeForm()
            If Not fPass.ShowDialog Then
                MsgBox("Passcode could not be validated. Application will exit...")
                Me.Close()
                Return False
            End If
        End If
        Return True
    End Function

    Private Sub LoadRecommendations()
        Dim ofd As New OpenFileDialog
        If ofd.ShowDialog = DialogResult.OK Then
            LoadCsv(ofd.FileName)
        End If
    End Sub

    Private Sub ApplyComposerAliases()
        For Each rec In _Library.RecommendationList
            Dim foundComposer As ComposerAlias = _Library.Composers.FindComposer(rec)
            If foundComposer IsNot Nothing Then
                rec.Composer = foundComposer.PrimaryName
                rec.AlternateComposerSpellings = foundComposer.Aliases
            End If
        Next
    End Sub

    Private Sub LoadHoldings()
        Dim ofd As New OpenFileDialog
        If ofd.ShowDialog = DialogResult.OK Then
            LoadHoldingsCsv(ofd.FileName)
        End If
    End Sub

    Private Sub LoadHoldingsCsv(fileName As String)
        Dim csvText = IO.File.ReadAllText(fileName)

        For Each line In Csv.CsvReader.ReadFromText(csvText, New CsvOptions() With {.AllowNewLineInEnclosedFieldValues = True})
            Dim mRec As HoldingEntry = HoldingEntry.GetEntryFromCsvLine(line)
            If mRec Is Nothing Then Continue For

            Dim foundRecommendation As Recommendation
            If Not _Library.RecommendationList.FindRecommendation(mRec, foundRecommendation) Then
                _Library.Composers.SyncWith(foundRecommendation)
            End If

            If foundRecommendation IsNot Nothing Then
                If mRec.Arranger <> Nothing Then
                    foundRecommendation.Arranger = mRec.Arranger
                End If
                foundRecommendation.Owned = True
            End If
        Next
    End Sub

    Private Sub LoadRepertoireFromWebsite()
        Dim ofd As New OpenFileDialog
        If ofd.ShowDialog = DialogResult.OK Then
            LoadRepertoireCsv(ofd.FileName)
        End If
    End Sub

    Private Sub LoadRepertoireCsv(fileName As String)
        Dim csvText = IO.File.ReadAllText(fileName)

        For Each line In Csv.CsvReader.ReadFromText(csvText, New CsvOptions() With {.AllowNewLineInEnclosedFieldValues = True})
            Dim mRec As RepertoireEntry = RepertoireEntry.GetEntryFromCsvLine(line)
            If mRec Is Nothing Then Continue For


            Dim foundRecommendation As Recommendation
            If Not _Library.RecommendationList.FindRecommendation(mRec, foundRecommendation) Then
                _Library.Composers.SyncWith(foundRecommendation)
            End If

            If foundRecommendation IsNot Nothing Then
                Dim allYears As New List(Of Integer)
                allYears.AddRange(foundRecommendation.PerformedYears)
                allYears.AddRange(mRec.Years)
                allYears = allYears.Distinct.ToList
                allYears.Sort(Function(x, y) y.CompareTo(x))
                foundRecommendation.PerformedYears.Clear()
                For Each yearVal In allYears
                    foundRecommendation.PerformedYears.Add(yearVal)
                Next
            End If

        Next
    End Sub


    Private Sub LoadLibraryData()

        Dim client As FtpClient = FtpAndSecurity.GetFtpConnection(My.Settings.Passcode)
        client.AutoConnect()
        Dim dlStatus = client.DownloadFile("library.dat", "library.dat", FtpLocalExists.Overwrite, FtpVerify.OnlyChecksum,
                                           Sub(prg As FtpProgress)
                                               _Context.Send(Sub(progress As FtpProgress)
                                                                 Me.BarEditItem1.EditValue = progress.Progress
                                                             End Sub, prg)
                                           End Sub)
        client.Disconnect()
        client.Dispose()

        If dlStatus = FtpStatus.Success Then
            If IO.File.Exists("library.dat") Then
                Dim ser As New Xml.Serialization.XmlSerializer(GetType(LibraryData))
                Using fs As New IO.FileStream("library.dat", IO.FileMode.Open)
                    __Library = ser.Deserialize(fs)
                End Using
            Else
                If _Library Is Nothing Then
                    __Library = New LibraryData
                End If

                If IO.File.Exists("savedRecommendations.dat") Then
                    Dim ser As New Xml.Serialization.XmlSerializer(GetType(RecommendationList))
                    Using fs As New IO.FileStream("savedRecommendations.dat", IO.FileMode.Open)
                        _Library.RecommendationList = ser.Deserialize(fs)
                    End Using
                End If
                If IO.File.Exists("composerAliases.dat") Then
                    Dim aliasList As ComposerAliases

                    Dim ser As New Xml.Serialization.XmlSerializer(GetType(ComposerAliases))
                    Using fs As New IO.FileStream("composerAliases.dat", IO.FileMode.Open)
                        aliasList = ser.Deserialize(fs)
                    End Using

                    _Library.Composers = aliasList
                End If

                If _Library.RecommendationList Is Nothing Then
                    _Library.RecommendationList = New RecommendationList
                End If
                If _Library.Composers Is Nothing Then
                    _Library.Composers = New ComposerAliases
                End If
                SaveLibraryData()
                LoadLibraryData()
            End If

            ApplyComposerAliases()

            For Each item In _Library.RecommendationList
                item.HasChanges = False
            Next
            For Each item In _Library.Composers
                item.HasChanges = False
            Next
        End If

    End Sub



    Private Sub LoadCsv(fileName As String)
        Dim csvText = IO.File.ReadAllText(fileName)

        For Each line In Csv.CsvReader.ReadFromText(csvText, New CsvOptions() With {.AllowNewLineInEnclosedFieldValues = True})
            Dim mRecs As IEnumerable(Of MusicianRecommendation) = MusicianRecommendation.GetRecommendationsFromCsvLine(line)

            For Each mRec In mRecs
                Dim foundRecommendation As Recommendation
                If Not _Library.RecommendationList.FindRecommendation(mRec, foundRecommendation) Then
                    _Library.Composers.SyncWith(foundRecommendation)
                End If

                Dim foundRecommendedBy = foundRecommendation.RecommendedBy.ToList.Find(Function(x) x.Key = mRec.Key)
                If foundRecommendedBy IsNot Nothing Then
                    foundRecommendedBy.UpdateFrom(mRec)
                Else
                    foundRecommendation.RecommendedBy.Add(mRec)
                End If
            Next
        Next
    End Sub

    Private Sub GridView1_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridView1.FocusedRowChanged
        'PropertyGridControl1.SelectedObject = GridView1.GetFocusedRow
    End Sub

    Private Sub BarButtonItem1_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem1.ItemClick

        If TabPane1.SelectedPage Is TabNavigationPage1 Then
            MergeSelectionsFromMusicList()
        ElseIf TabPane1.SelectedPage Is TabNavigationPage2 Then
            MergeSelectionsFromComposers()
        End If


    End Sub

    Private Sub MergeSelectionsFromComposers()
        Dim selectedComposers As New List(Of ComposerAlias)
        If GridViewComposers.GetSelectedRows.Count > 0 Then
            For Each selectedRowHandle In GridViewComposers.GetSelectedRows
                selectedComposers.Add(GridViewComposers.GetRow(selectedRowHandle))
            Next

            Dim fMerge As New ComposerMergeForm
            fMerge.Initialize(selectedComposers)
            fMerge.ShowDialog()

            If fMerge.MergedItem IsNot Nothing Then
                For Each selection In selectedComposers
                    _Library.Composers.Remove(selection)
                Next
                _Library.Composers.Add(fMerge.MergedItem)
                ApplyComposerAliases()

                Dim handle = GridViewComposers.GetRowHandle(_Library.Composers.IndexOf(fMerge.MergedItem))
                GridViewComposers.FocusedRowHandle = handle
            End If

        End If
    End Sub

    Private Sub MergeSelectionsFromMusicList()
        Dim selections As New List(Of Recommendation)
        If GridView1.GetSelectedRows.Count > 0 Then
            For Each selectedRowHandle In GridView1.GetSelectedRows
                selections.Add(GridView1.GetRow(selectedRowHandle))
            Next

            Dim fMerge As New MergeForm
            fMerge.Initialize(selections, _Library.Composers)
            fMerge.ShowDialog()

            If fMerge.MergedItem IsNot Nothing Then
                For Each selection In selections
                    _Library.RecommendationList.Remove(selection)
                Next
                _Library.RecommendationList.Add(fMerge.MergedItem)

                Dim handle = GridView1.GetRowHandle(_Library.RecommendationList.IndexOf(fMerge.MergedItem))
                GridView1.FocusedRowHandle = handle
            End If
            If fMerge.MergedAlias IsNot Nothing Then
                For Each oldAlias In fMerge.OriginalComposerAliases
                    _Library.Composers.Remove(oldAlias)
                Next
                _Library.Composers.Add(fMerge.MergedAlias)
                ApplyComposerAliases()
            End If

        End If
    End Sub

    Private Async Sub BarButtonItem4_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem4.ItemClick

        BarButtonItem4.Enabled = False
        Me.RepositoryItemProgressBar1.Minimum = 0
        Me.RepositoryItemProgressBar1.Maximum = 100
        Me.BarEditItem1.EditValue = 0
        Me.BarStaticItem1.Caption = "Saving library data..."
        BarManager1.StatusBar.Visible = True

        Dim errorMessage = Await Task.Run(Function() SaveLibraryData())

        BarManager1.StatusBar.Visible = False
        Me.BarEditItem1.EditValue = 0

        If errorMessage <> Nothing Then
            If MsgBox(errorMessage & Environment.NewLine & Environment.NewLine & "Would you like to save a copy locally?", MsgBoxStyle.YesNo, "Upload Failed") = MsgBoxResult.Yes Then
                Dim sfd As New SaveFileDialog
                sfd.FileName = $"library_{Now:yyyyMMddHHmmssfff}_export.dat"
                If sfd.ShowDialog = DialogResult.OK Then
                    IO.File.Copy("library.dat", sfd.FileName)
                End If
            End If
        Else
            For Each item In _Library.RecommendationList
                item.HasChanges = False
            Next
            For Each item In _Library.Composers
                item.HasChanges = False
            Next
        End If

        BarButtonItem4.Enabled = True
    End Sub

    Private Function SaveLibraryData() As String

        If IO.File.Exists("library.dat") Then
            IO.File.Move("library.dat", $"library_{Now:yyyyMMddHHmmssfff}.dat")
        End If

        Dim prevLibDate As Date = _Library.LastChanged
        _Library.LastChanged = Now

        Dim ser As New Xml.Serialization.XmlSerializer(GetType(LibraryData))
        Using fs As New IO.FileStream("library.dat", IO.FileMode.Create)
            ser.Serialize(fs, _Library)
        End Using

        ' Check if server file is newer than your file.
        Dim client As FtpClient = FtpAndSecurity.GetFtpConnection(My.Settings.Passcode)
        client.AutoConnect()
        Dim dlStatus = client.DownloadFile("tmplibrary.dat", "library.dat", FtpLocalExists.Overwrite, FtpVerify.OnlyChecksum,
                                           Sub(prg As FtpProgress)
                                               _Context.Send(Sub(progress As FtpProgress)
                                                                 Me.BarEditItem1.EditValue = progress.Progress / 2.0
                                                             End Sub, prg)
                                           End Sub)

        Dim uploadFailed As Boolean = True
        Dim uploadFailedMessage = "Upload failed for an unknown reason."
        If dlStatus = FtpStatus.Success Then
            Using fs As New IO.FileStream("tmplibrary.dat", IO.FileMode.Open)
                Dim tmpLib As LibraryData = ser.Deserialize(fs)
                If tmpLib.LastChanged <= prevLibDate Then
                    Dim moveStat = client.MoveFile("library.dat", $"library_{Now:yyyyMMddHHmmssfff}.dat")
                    If Not moveStat Then
                        uploadFailedMessage = "Upload failed because remote file could not be renamed."
                    Else
                        Dim upStat = client.UploadFile("library.dat", "library.dat",,,,
                                                       Sub(prg As FtpProgress)
                                                           _Context.Send(Sub(progress As FtpProgress)
                                                                             Me.BarEditItem1.EditValue = 50.0 + (progress.Progress / 2.0)
                                                                         End Sub, prg)
                                                       End Sub)
                        If upStat <> FtpStatus.Success Then
                            uploadFailedMessage = "File failed to upload to server."
                        Else
                            uploadFailed = False
                        End If
                    End If
                Else
                    uploadFailedMessage = "Upload failed because server version is newer."
                End If
            End Using
        End If

        client.Disconnect()
        client.Dispose()

        If uploadFailed Then
            Return uploadFailedMessage
        End If

        Return Nothing
    End Function

    Private Sub BarButtonItem3_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItem3.ItemClick
        Dim ofd As New OpenFileDialog
        ofd.Filter = "*.csv|*.csv"

        If ofd.ShowDialog = DialogResult.OK Then
            Select Case DetermineCsvFormat(ofd.FileName)
                Case CsvFormats.Holdings
                    LoadHoldingsCsv(ofd.FileName)
                Case CsvFormats.Recommendation
                    LoadCsv(ofd.FileName)
                Case CsvFormats.Repertoire
                    LoadRepertoireCsv(ofd.FileName)
            End Select
        End If
    End Sub

    Enum CsvFormats
        None
        Holdings
        Repertoire
        Recommendation
    End Enum
    Private Function DetermineCsvFormat(fileName As String) As CsvFormats
        Dim csvText = IO.File.ReadAllText(fileName)

        For Each line In Csv.CsvReader.ReadFromText(csvText, New CsvOptions() With {.AllowNewLineInEnclosedFieldValues = True})
            If line.Headers.Count = 20 AndAlso line.Headers(0) = "Mark" Then
                Return CsvFormats.Holdings
            ElseIf line.Headers.Count = 19 AndAlso line.Headers(0) = "Timestamp" Then
                Return CsvFormats.Recommendation
            ElseIf line.Headers.Count = 3 AndAlso line.Headers(0) = "Piece" Then
                Return CsvFormats.Repertoire
            End If

        Next
        Return CsvFormats.None
    End Function

    Private Sub GridView1_CustomDrawRowIndicator(sender As Object, e As DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs) Handles GridView1.CustomDrawRowIndicator
        Dim row As Recommendation = GridView1.GetRow(e.RowHandle)
        If row IsNot Nothing AndAlso row.HasChanges AndAlso e.Info.ImageIndex <> 0 Then
            e.Info.ImageIndex = 1
        End If
    End Sub

    Private Sub GridViewComposers_CustomDrawRowIndicator(sender As Object, e As DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs) Handles GridViewComposers.CustomDrawRowIndicator
        Dim row As ComposerAlias = GridViewComposers.GetRow(e.RowHandle)
        If row IsNot Nothing AndAlso row.HasChanges Then
            e.Info.ImageIndex = 1
        End If
    End Sub
End Class

Public Class ComposerAliases : Inherits BindingList(Of ComposerAlias)

    Friend Sub SyncWith(foundRecommendation As Recommendation)
        If foundRecommendation Is Nothing OrElse foundRecommendation.Composer = Nothing Then Exit Sub

        Dim foundAlias = Me.FindComposer(foundRecommendation)
        If foundAlias Is Nothing Then
            ' Doesn't exist yet
            Dim newAlias As New ComposerAlias(foundRecommendation.Composer, foundRecommendation.AlternateComposerSpellings)
            Me.Add(newAlias)
        Else
            ' Does exist
            Dim allComposerNames As New List(Of String)
            allComposerNames.Add(foundAlias.PrimaryName)
            allComposerNames.Add(foundRecommendation.Composer)
            allComposerNames.AddRange(foundAlias.Aliases)
            allComposerNames.AddRange(foundRecommendation.AlternateComposerSpellings)
            allComposerNames = allComposerNames.Distinct

            allComposerNames.Remove(foundAlias.PrimaryName)
            allComposerNames.Sort()

            foundRecommendation.Composer = foundAlias.PrimaryName

            foundRecommendation.AlternateComposerSpellings.Clear()
            foundAlias.Aliases.Clear()

            For Each name In allComposerNames
                foundRecommendation.AlternateComposerSpellings.Add(name)
                foundAlias.Aliases.Add(name)
            Next
        End If
    End Sub

    Friend Function FindComposer(rec As IComposer) As ComposerAlias
        Return Me.ToList.Find(Function(x)
                                  If x.PrimaryName.ToUpper.Trim = rec.Composer.ToUpper.Trim Then Return True
                                  If x.Aliases.Select(Function(y) y.ToUpper.Trim).Contains(rec.Composer.ToUpper.Trim) Then Return True
                                  Return False
                              End Function)
    End Function
End Class

Public Class ComposerAlias : Implements INotifyPropertyChanged, IComposer

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Property Aliases As BindingList(Of String)
        Get
            Return _Aliases
        End Get
        Set(value As BindingList(Of String))
            _Aliases = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Aliases)))
        End Set
    End Property
    Private _Aliases As New BindingList(Of String)

    Property PrimaryName As String Implements IComposer.Composer
        Get
            Return _PrimaryName
        End Get
        Set(value As String)
            _PrimaryName = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PrimaryName)))
        End Set
    End Property
    Private _PrimaryName As String

    <Display(AutoGenerateField:=False)>
    Public Property HasChanges As Boolean
        Get
            Return _HasChanges
        End Get
        Set(value As Boolean)
            _HasChanges = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
        End Set
    End Property
    Private _HasChanges As Boolean = True

    Sub New()

    End Sub

    Sub New(primaryName As String, aliases As IEnumerable(Of String))
        Me.PrimaryName = primaryName

        Dim aliasList = aliases.ToList
        aliasList.Sort()
        For Each aliasName In aliasList
            Me.Aliases.Add(aliasName)
        Next
    End Sub

    Friend Sub MergeFrom(rec As IComposerAlternates)
        Dim aliasList As New List(Of String)
        aliasList.Add(Me.PrimaryName)
        aliasList.AddRange(Me.Aliases)
        aliasList.Add(rec.Composer)
        aliasList.AddRange(rec.AlternateComposerSpellings)
        aliasList = aliasList.Distinct.ToList
        aliasList.Remove(Me.PrimaryName)
        aliasList.Sort()

        Me.Aliases.Clear()
        For Each aliasName In aliasList
            Me.Aliases.Add(aliasName)
        Next
    End Sub

    Public Overrides Function ToString() As String
        Return PrimaryName
    End Function

End Class

Public Class LibraryData
    Public Property RecommendationList As RecommendationList
    Public Property Composers As ComposerAliases
    Public Property LastChanged As Date

End Class

Public Class RecommendationList : Inherits BindingList(Of Recommendation)

    Friend Function FindRecommendation(mRec As HoldingEntry, ByRef foundRecommendation As Recommendation) As Boolean
        For Each recommendation In Me
            If recommendation.IsMatch(mRec) Then
                foundRecommendation = recommendation
                Return True
            End If
        Next
        Dim newRec = New Recommendation() With {.Composer = mRec.Composer, .Title = mRec.Title, .Arranger = mRec.Arranger}
        Me.Add(newRec)
        foundRecommendation = newRec
        Return False
    End Function
    Friend Function FindRecommendation(mRec As RepertoireEntry, ByRef foundRecommendation As Recommendation) As Boolean
        For Each recommendation In Me
            If recommendation.IsMatch(mRec) Then
                foundRecommendation = recommendation
                Return True
            End If
        Next
        Dim newRec = New Recommendation() With {
            .Composer = mRec.Composer,
            .Title = mRec.Title,
            .PerformedYears = mRec.Years}
        Me.Add(newRec)
        foundRecommendation = newRec
        Return False
    End Function
    Friend Function FindRecommendation(mRec As MusicianRecommendation, ByRef foundRecommendation As Recommendation) As Boolean
        For Each recommendation In Me
            If recommendation.IsMatch(mRec) Then
                foundRecommendation = recommendation
                Return True
            End If
        Next
        Dim newRec = New Recommendation() With {.Composer = mRec.Composer, .Title = mRec.Title}
        Me.Add(newRec)
        foundRecommendation = newRec
        Return False
    End Function
End Class

Public Class Recommendation : Implements INotifyPropertyChanged, IComposerTitle, IComposerAlternates

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Property Composer As String Implements IComposerTitle.Composer
        Get
            Return _Composer
        End Get
        Set(value As String)
            _Composer = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Composer)))
        End Set
    End Property
    Private _Composer As String

    Property Arranger As String
        Get
            Return _Arranger
        End Get
        Set(value As String)
            _Arranger = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Arranger)))
        End Set
    End Property
    Private _Arranger As String

    Property Title As String Implements IComposerTitle.Title
        Get
            Return _Title
        End Get
        Set(value As String)
            _Title = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Title)))
        End Set
    End Property
    Private _Title As String

    Property Owned As Boolean
        Get
            Return _Owned
        End Get
        Set(value As Boolean)
            _Owned = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Owned)))
        End Set
    End Property
    Private _Owned As Boolean

    ReadOnly Property LastPerformed As Integer
        Get
            Return If(PerformedYears IsNot Nothing AndAlso PerformedYears.Count > 0, PerformedYears.Max, 0)
        End Get
    End Property

    ReadOnly Property TimesPerformed As Integer
        Get
            Return If(PerformedYears?.Count, 0)
        End Get
    End Property

    ReadOnly Property TimesRecommended As Integer
        Get
            Return If(RecommendedBy?.Select(Function(x) x.Musician.ToString).Distinct.Count, 0)
        End Get
    End Property


    Property PerformedYears As BindingList(Of Integer)
        Get
            Return _PerformedYears
        End Get
        Set(value As BindingList(Of Integer))
            _PerformedYears = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PerformedYears)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LastPerformed)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimesPerformed)))
        End Set
    End Property
    Private WithEvents _PerformedYears As New BindingList(Of Integer)
    Private Sub OnYearsListChanged(sender As Object, e As ListChangedEventArgs) Handles _PerformedYears.ListChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LastPerformed)))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimesPerformed)))
    End Sub

    Property AlternateComposerSpellings As BindingList(Of String) Implements IComposerAlternates.AlternateComposerSpellings
        Get
            Return _AlternateComposerSpellings
        End Get
        Set(value As BindingList(Of String))
            _AlternateComposerSpellings = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(AlternateComposerSpellings)))
        End Set
    End Property
    Private _AlternateComposerSpellings As New BindingList(Of String)

    Property AlternateTitleSpellings As BindingList(Of String)
        Get
            Return _AlternateTitleSpellings
        End Get
        Set(value As BindingList(Of String))
            _AlternateTitleSpellings = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(AlternateTitleSpellings)))
        End Set
    End Property
    Private _AlternateTitleSpellings As New BindingList(Of String)

    Property RecommendedBy As BindingList(Of MusicianRecommendation)
        Get
            Return _RecommendedBy
        End Get
        Set(value As BindingList(Of MusicianRecommendation))
            _RecommendedBy = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RecommendedBy)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimesRecommended)))
        End Set
    End Property
    Private WithEvents _RecommendedBy As New BindingList(Of MusicianRecommendation)

    <Display(AutoGenerateField:=False)>
    Public Property HasChanges As Boolean
        Get
            Return _HasChanges
        End Get
        Set(value As Boolean)
            _HasChanges = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
        End Set
    End Property
    Private _HasChanges As Boolean = True

    Private Sub OnRecommendedByListChanged(sender As Object, e As ListChangedEventArgs) Handles _RecommendedBy.ListChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimesRecommended)))
    End Sub

    Friend Function IsMatch(mRec As IComposerTitle) As Boolean
        Dim composerNames As New List(Of String) From {Me.Composer.ToUpper.Trim}
        composerNames.AddRange(Me.AlternateComposerSpellings.Select(Function(x) x.ToUpper.Trim))

        Dim titles As New List(Of String) From {Me.Title.ToUpper.Trim}
        titles.AddRange(Me.AlternateTitleSpellings.Select(Function(x) x.ToUpper.Trim))

        If composerNames.Contains(mRec.Composer.ToUpper.Trim) AndAlso titles.Contains(mRec.Title.ToUpper.Trim) Then
            Return True
        End If

        Return False
    End Function

    Public Overrides Function ToString() As String
        Return $"{Composer}: {Title}"
    End Function

End Class

Public Interface IComposer : Inherits INotifyPropertyChanged
    Property Composer As String

End Interface

Public Interface ITitle : Inherits INotifyPropertyChanged
    Property Title As String

End Interface

Public Interface IComposerTitle : Inherits IComposer, ITitle

End Interface

Public Interface IComposerAlternates : Inherits IComposer
    Property AlternateComposerSpellings As BindingList(Of String)
End Interface

Public Class MusicianRecommendation : Implements INotifyPropertyChanged, IComposerTitle

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    <Display(AutoGenerateField:=False)>
    Property Composer As String Implements IComposerTitle.Composer
        Get
            Return _Composer
        End Get
        Set(value As String)
            _Composer = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Composer)))
        End Set
    End Property
    Private _Composer As String

    <Display(AutoGenerateField:=False)>
    Property Title As String Implements IComposerTitle.Title
        Get
            Return _Title
        End Get
        Set(value As String)
            _Title = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Title)))
        End Set
    End Property
    Private _Title As String

    <Display(AutoGenerateField:=False)>
    Property Key As String
        Get
            Return _Key
        End Get
        Set(value As String)
            _Key = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Key)))
        End Set
    End Property
    Private _Key As String

    Property TimeStamp As Date
        Get
            Return _TimeStamp
        End Get
        Set(value As Date)
            _TimeStamp = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimeStamp)))
        End Set
    End Property
    Private _TimeStamp As Date

    Property Musician As Musician
        Get
            Return _Musician
        End Get
        Set(value As Musician)
            _Musician = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Musician)))
        End Set
    End Property
    Private WithEvents _Musician As Musician
    Private Sub OnMusicianPropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _Musician.PropertyChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Musician)))
    End Sub

    Property Comments As String
        Get
            Return _Comments
        End Get
        Set(value As String)
            _Comments = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Comments)))
        End Set
    End Property
    Private _Comments As String

    Property AdditionalThoughts As String
        Get
            Return _AdditionalThoughts
        End Get
        Set(value As String)
            _AdditionalThoughts = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(AdditionalThoughts)))
        End Set
    End Property
    Private _AdditionalThoughts As String

    Friend Shared Function GetRecommendationsFromCsvLine(line As ICsvLine) As IEnumerable(Of MusicianRecommendation)
        Dim timeStamp As DateTime = Date.Parse(line(0))
        Dim firstName As String = line(1)
        Dim lastName As String = line(2)
        Dim addlThoughts As String = If(line.ColumnCount > 18, line(18), Nothing)

        Dim musician = New Musician() With {.FirstName = firstName.Trim, .LastName = lastName.Trim}

        Dim ret As New List(Of MusicianRecommendation)

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
    End Function

    Friend Sub UpdateFrom(mRec As MusicianRecommendation)
        Me.Composer = mRec.Composer
        Me.Title = mRec.Title
        Me.Comments = String.Join(Environment.NewLine, {Me.Comments, mRec.Comments})
    End Sub

    Public Overrides Function ToString() As String
        Return $"{Composer}: {Title} (by {Musician})"
    End Function

End Class

Public Class Musician : Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Property FirstName As String
        Get
            Return _FirstName
        End Get
        Set(value As String)
            _FirstName = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(FirstName)))
        End Set
    End Property
    Private _FirstName As String

    Property LastName As String
        Get
            Return _LastName
        End Get
        Set(value As String)
            _LastName = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LastName)))
        End Set
    End Property
    Private _LastName As String

    Public Overrides Function ToString() As String
        Return $"{LastName}, {FirstName}"
    End Function
End Class

Public Class RepertoireEntry : Implements IComposerTitle

    Property Title As String Implements IComposerTitle.Title
        Get
            Return _Title
        End Get
        Set(value As String)
            _Title = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Title)))
        End Set
    End Property
    Private _Title As String

    Property Composer As String Implements IComposerTitle.Composer
        Get
            Return _Composer
        End Get
        Set(value As String)
            _Composer = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Composer)))
        End Set
    End Property
    Private _Composer As String

    <Display(AutoGenerateField:=False)>
    Property Years As BindingList(Of Integer)
        Get
            Return _Years
        End Get
        Set(value As BindingList(Of Integer))
            _Years = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Years)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LastPerformed)))
        End Set
    End Property
    Private WithEvents _Years As New BindingList(Of Integer)
    Private Sub OnYearsListChanged(sender As Object, e As ListChangedEventArgs) Handles _Years.ListChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LastPerformed)))
    End Sub

    ReadOnly Property LastPerformed As Integer
        Get
            Return _Years.Max
        End Get
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

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

        yearsList = yearsList.Distinct
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

Public Class HoldingEntry : Implements IComposerTitle

    Property Title As String Implements IComposerTitle.Title
        Get
            Return _Title
        End Get
        Set(value As String)
            _Title = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Title)))
        End Set
    End Property
    Private _Title As String

    Property Composer As String Implements IComposerTitle.Composer
        Get
            Return _Composer
        End Get
        Set(value As String)
            _Composer = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Composer)))
        End Set
    End Property
    Private _Composer As String

    Property Arranger As String
        Get
            Return _Arranger
        End Get
        Set(value As String)
            _Arranger = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Arranger)))
        End Set
    End Property
    Private _Arranger As String

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

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