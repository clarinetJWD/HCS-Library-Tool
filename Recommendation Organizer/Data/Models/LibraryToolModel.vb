Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Net
Imports FluentFTP

Public Class LibraryToolModel : Implements INotifyPropertyChanged

    Public Event ProgressChanged(sender As Object, e As ProgressBarEventArgs)
    Public Event ErrorOccurred(sender As Object, e As ErrorCodeEventArgs)
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Property _LibraryData As LibraryData
        Get
            Return __LibraryData
        End Get
        Set(value As LibraryData)
            __LibraryData = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Library)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Composers)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Eras)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Tags)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LibraryHasChanges)))
        End Set
    End Property
    Private Sub OnLibraryDataPropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles __LibraryData.PropertyChanged
        If e.PropertyName = NameOf(LibraryData.HasChanges) Then
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LibraryHasChanges)))
        End If
    End Sub
    Private WithEvents __LibraryData As LibraryData = New LibraryData

#Region "Library"

    Friend ReadOnly Property LibraryHasChanges As Boolean
        Get
            Return If(_LibraryData?.HasChanges, False)
        End Get
    End Property

    Friend Property Library As RecommendationList
        Get
            Return _LibraryData?.RecommendationList
        End Get
        Private Set(value As RecommendationList)
            _LibraryData.RecommendationList = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Library)))
        End Set
    End Property

    Friend Async Function LoadLibrary() As Task(Of Boolean)

        Try
            Dim success As Boolean = True
            Dim needsSave As Boolean = False

            Dim newLibraryData As LibraryData = Await Task.Run(
                Function()
                    Dim client As FtpClient = Nothing
                    Try
                        RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs With {
                                                               .IsMarquee = True,
                                                               .Caption = "Loading Library..."})

                        client = FtpAndSecurity.GetFtpConnection(My.Settings.Passcode)
                        client.AutoConnect()
                        If client.FileExists(FtpPath_Library) Then
                            Dim dlStatus = client.DownloadFile(
                                LocalPath_Library, FtpPath_Library, FtpLocalExists.Overwrite, FtpVerify.Retry,
                                Sub(prg As FtpProgress)
                                    RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs With {
                                                               .Minimum = 0,
                                                               .Maximum = 100,
                                                               .Value = prg.Progress,
                                                               .Caption = "Loading Library..."})
                                End Sub)
                            success = (dlStatus = FtpStatus.Success)
                        End If
                    Catch ex As Exception
                        ' TODO Log
                        success = False
                    Finally
                        client?.Disconnect()
                        client?.Dispose()
                    End Try

                    Dim newLibraryDataInternal As LibraryData

                    If success Then
                        If IO.File.Exists(LocalPath_Library) Then
                            Dim ser As New Xml.Serialization.XmlSerializer(GetType(LibraryData))
                            Using fs As New IO.FileStream(LocalPath_Library, IO.FileMode.Open)
                                newLibraryDataInternal = ser.Deserialize(fs)
                                newLibraryDataInternal.IsDataLoaded = True
                            End Using
                        Else
                            If newLibraryDataInternal Is Nothing Then
                                newLibraryDataInternal = New LibraryData
                            End If

                            If newLibraryDataInternal.RecommendationList Is Nothing Then
                                newLibraryDataInternal.RecommendationList = New RecommendationList
                            End If
                            If newLibraryDataInternal.Composers Is Nothing Then
                                newLibraryDataInternal.Composers = New ComposerAliases
                            End If

                            needsSave = True

                        End If
                    End If

                    Return newLibraryDataInternal
                End Function)

            If needsSave AndAlso newLibraryData IsNot Nothing Then
                Await SaveLibrary(newLibraryData)
                Return Await LoadLibrary()
            End If

            If success Then

                ApplyComposerAliases(newLibraryData.RecommendationList, newLibraryData.Composers)

                newLibraryData.HasChanges = False

                Me._LibraryData = newLibraryData

                Return True
            Else
                Me._LibraryData = New LibraryData

                RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {.ErrorCode = ErrorCodeEventArgs.ErrorCodes.LibraryFailedToLoadFromServer, .Critical = True})

                Return False
            End If
        Finally
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs With {
                                                       .Visible = False})
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Library)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Composers)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Eras)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Tags)))
        End Try

    End Function

    Friend Sub AddSeasonPlannerItem(seasonItem As Recommendation)
        Me.SeasonPlannerItems.Add(New SeasonItem(seasonItem))
        EnsureNoSeasonPlanningDuplicates()
    End Sub

    Friend Sub RemoveSeasonPlannerItem(recommendation As Recommendation)
        If SeasonPlannerItems.FindSeasonItem(recommendation) IsNot Nothing Then
            SeasonPlannerItems.Remove(SeasonPlannerItems.FindSeasonItem(recommendation))
        Else
            For Each concertInfo In WorkingSeasonInformation.WorkingConcertInformations
                Dim foundItem = concertInfo.Compositions.FindSeasonItem(recommendation)
                If foundItem IsNot Nothing Then
                    concertInfo.Compositions.Remove(foundItem)
                    Exit Sub
                End If
            Next
        End If
        If HiddenSeasonPlannerItems.FindSeasonItem(recommendation) IsNot Nothing Then
            HiddenSeasonPlannerItems.Remove(HiddenSeasonPlannerItems.FindSeasonItem(recommendation))
        End If
    End Sub

    Friend Function GetSeasonPlannerContainsItem(recommendation As Recommendation) As Boolean
        If SeasonPlannerItems.FindSeasonItem(recommendation) IsNot Nothing Then
            Return True
        Else
            For Each concertInfo In WorkingSeasonInformation.WorkingConcertInformations
                Dim foundItem = concertInfo.Compositions.FindSeasonItem(recommendation)
                If foundItem IsNot Nothing Then
                    Return True
                End If
            Next
        End If
        Return False
    End Function

    Friend Async Function SaveLibrary() As Task(Of Boolean)
        Return Await SaveLibrary(Me._LibraryData)
    End Function

    Friend Async Function SaveLibrary(libraryData As LibraryData) As Task(Of Boolean)

        Dim client As FtpClient
        Try
            Dim prevLibDate As Date = libraryData.LastChanged
            libraryData.LastChanged = Now

            Dim uploadFailed As Boolean = True
            Dim uploadFailedCode As ErrorCodeEventArgs.ErrorCodes = ErrorCodeEventArgs.ErrorCodes.None
            Dim uploadFailedShowMode As ErrorCodeEventArgs.ShowModes = ErrorCodeEventArgs.ShowModes.Modal

            Await Task.Run(
                Sub()
                    RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs With {
                                               .IsMarquee = True,
                                               .Caption = "Saving Library..."})

                    If IO.File.Exists(LocalPath_Library) Then
                        IO.File.Move(LocalPath_Library, LocalPath_Library_Temp(Now.ToString("yyyyMMddHHmmssfff")))
                    End If

                    Dim ser As New Xml.Serialization.XmlSerializer(GetType(LibraryData))
                    Using fs As New IO.FileStream(LocalPath_Library, IO.FileMode.Create)
                        ser.Serialize(fs, libraryData)
                    End Using

                    ' Check if server file is newer than your file.
                    client = FtpAndSecurity.GetFtpConnection(My.Settings.Passcode)
                    client.AutoConnect()

                    Dim dlStatus = client.DownloadFile(LocalPath_Library_Temp("temp"), FtpPath_Library, FtpLocalExists.Overwrite, FtpVerify.Retry,
                                                       Sub(prg As FtpProgress)
                                                           RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs With {
                                                                .Minimum = 0,
                                                                .Maximum = 100,
                                                                .Value = prg.Progress / 2.0,
                                                                .Caption = "Saving Library..."})
                                                       End Sub)

                    If dlStatus = FtpStatus.Success Then
                        Using fs As New IO.FileStream(LocalPath_Library_Temp("temp"), IO.FileMode.Open)
                            Dim tmpLib As LibraryData = ser.Deserialize(fs)
                            If tmpLib.LastChanged <= prevLibDate Then
                                Dim moveStat = client.MoveFile(FtpPath_Library, FtpPath_Library_Temp(Now.ToString("yyyyMMddHHmmssfff")))
                                If Not moveStat Then
                                    uploadFailedCode = ErrorCodeEventArgs.ErrorCodes.SaveLibraryFailedBecauseRemoteFileCouldNotBeReplaced
                                    uploadFailedShowMode = ErrorCodeEventArgs.ShowModes.StatusBar
                                Else
                                    Dim upStat = client.UploadFile(LocalPath_Library, FtpPath_Library,,, FtpVerify.Retry,
                                                                   Sub(prg As FtpProgress)
                                                                       RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs With {
                                                                           .Minimum = 0,
                                                                           .Maximum = 100,
                                                                           .Value = 50.0 + (prg.Progress / 2.0),
                                                                           .Caption = "Uploading Library..."})
                                                                   End Sub)

                                    If upStat <> FtpStatus.Success Then
                                        uploadFailedCode = ErrorCodeEventArgs.ErrorCodes.SaveLibraryFailedBecauseFileCouldNotBeUploaded
                                        uploadFailedShowMode = ErrorCodeEventArgs.ShowModes.StatusBar
                                    Else
                                        uploadFailed = False
                                    End If
                                End If
                            Else
                                uploadFailedCode = ErrorCodeEventArgs.ErrorCodes.SaveLibraryFailedBecauseTheRemoteFileIsNewer
                                uploadFailedShowMode = ErrorCodeEventArgs.ShowModes.Modal
                            End If
                        End Using
                    End If
                End Sub)

            If uploadFailed Then
                RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {.ErrorCode = uploadFailedCode, .ShowMode = uploadFailedShowMode})
                Return False
            Else
                libraryData.HasChanges = False
            End If

            Return True
        Catch ex As Exception
            RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {.ErrorCode = ErrorCodeEventArgs.ErrorCodes.SaveLibraryFailedForAnUnknownReason, .ShowMode = ErrorCodeEventArgs.ShowModes.StatusBar})
            Return False
        Finally
            If client IsNot Nothing Then client.Disconnect()
            If client IsNot Nothing Then client.Dispose()
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs With {
                                                       .Visible = False})
        End Try

    End Function

    Friend Function ReplaceLibraryEntries(itemsToReplace As IEnumerable(Of Recommendation), newItem As Recommendation) As Boolean
        For Each selection In itemsToReplace
            Library.Remove(selection)

            Dim seasonPlannerItem = Me.SeasonPlannerItems.FindSeasonItem(selection)
            If seasonPlannerItem IsNot Nothing Then seasonPlannerItem.SetRecommendation(newItem, True)

            Dim hiddenSeasonPlannerItem = Me.HiddenSeasonPlannerItems.FindSeasonItem(selection)
            If hiddenSeasonPlannerItem IsNot Nothing Then hiddenSeasonPlannerItem.SetRecommendation(newItem, True)

            For Each concert In Me.WorkingSeasonInformation.WorkingConcertInformations
                Dim concertItem = concert.Compositions.FindSeasonItem(selection)
                If concertItem IsNot Nothing Then concertItem.SetRecommendation(newItem, True)
            Next
        Next
        Library.Add(newItem)
        Return True
    End Function

    Friend Function MergeRecommendationsIntoLibrary(recommendations As IEnumerable(Of MusicianRecommendation)) As Boolean
        For Each musicianRecommendation As MusicianRecommendation In recommendations
            If musicianRecommendation Is Nothing Then Continue For

            Dim foundRecommendation As Recommendation
            If Not Library.FindOrCreateRecommendation(musicianRecommendation, foundRecommendation) Then
                Composers.SyncWith(foundRecommendation)
            End If

            Dim foundRecommendedBy = foundRecommendation.RecommendedBy.ToList.Find(Function(x) x.Key = musicianRecommendation.Key)
            If foundRecommendedBy IsNot Nothing Then
                foundRecommendedBy.UpdateFrom(musicianRecommendation)
            Else
                foundRecommendation.RecommendedBy.Add(musicianRecommendation)
            End If
        Next

        Return True
    End Function

    Friend Function MergeHoldingsIntoLibrary(holdings As IEnumerable(Of HoldingEntry)) As Boolean
        For Each holding As HoldingEntry In holdings
            If holding Is Nothing Then Continue For

            Dim foundRecommendation As Recommendation = Nothing
            If Not Library.FindOrCreateRecommendation(holding, foundRecommendation) Then
                Composers.SyncWith(foundRecommendation)
            End If

            If foundRecommendation IsNot Nothing Then
                If holding.Arranger <> Nothing Then
                    foundRecommendation.Arranger = holding.Arranger
                End If
                foundRecommendation.Owned = True
            End If
        Next

        Return True
    End Function

    Friend Function MergeRepertoireIntoLibrary(repertoires As IEnumerable(Of RepertoireEntry)) As Boolean
        For Each repertoire As RepertoireEntry In repertoires
            If repertoire Is Nothing Then Continue For

            Dim foundRecommendation As Recommendation
            If Not Library.FindOrCreateRecommendation(repertoire, foundRecommendation) Then
                Composers.SyncWith(foundRecommendation)
            End If

            If foundRecommendation IsNot Nothing Then
                Dim allYears As New List(Of Integer)
                allYears.AddRange(foundRecommendation.PerformedYears)
                allYears.AddRange(repertoire.Years)
                allYears = allYears.Distinct.ToList
                allYears.Sort(Function(x, y) y.CompareTo(x))

                If Not ListsAreSame(allYears, foundRecommendation.PerformedYears) Then
                    foundRecommendation.PerformedYears.Clear()
                    For Each yearVal In allYears
                        foundRecommendation.PerformedYears.Add(yearVal)
                    Next
                End If
            End If
        Next

        Return True
    End Function


#End Region

#Region "Composers"

    Friend Property Composers As ComposerAliases
        Get
            Return _LibraryData?.Composers
        End Get
        Private Set(value As ComposerAliases)
            _LibraryData.Composers = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Composers)))
        End Set
    End Property

    Private Sub ApplyComposerAliases(library As RecommendationList, composers As ComposerAliases)
        For Each rec In library
            Dim foundComposer As ComposerAlias = composers.FindComposer(rec)
            If foundComposer IsNot Nothing Then
                rec.Composer = foundComposer.PrimaryName
                rec.AlternateComposerSpellings = foundComposer.Aliases
            End If
        Next
    End Sub

    Friend Function ReplaceAliases(itemsToReplace As IEnumerable(Of ComposerAlias), newItem As ComposerAlias) As Boolean
        For Each selection In itemsToReplace
            Composers.Remove(selection)
        Next
        Composers.Add(newItem)
        ApplyComposerAliases(Library, Composers)
        Return True
    End Function

#End Region

#Region "Eras"

    Friend Property Eras As Eras
        Get
            Return _LibraryData?.Eras
        End Get
        Private Set(value As Eras)
            _LibraryData.Eras = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Eras)))
        End Set
    End Property

#End Region

#Region "Tags"

    Friend Property Tags As Tags
        Get
            Return _LibraryData?.Tags
        End Get
        Private Set(value As Tags)
            _LibraryData.Tags = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Tags)))
        End Set
    End Property

#End Region

#Region "Season Planner"

    Friend Property PublishedSeasonIndexes As PublishedSeasonIndexes
        Get
            Return _PublishedSeasonIndexes
        End Get
        Private Set(value As PublishedSeasonIndexes)
            _PublishedSeasonIndexes = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PublishedSeasonIndexes)))
        End Set
    End Property
    Private _PublishedSeasonIndexes As PublishedSeasonIndexes

    Friend Property WorkingSeasonInformation As LocalConcertInformations
        Get
            Return _WorkingSeasonInformation
        End Get
        Private Set(value As LocalConcertInformations)
            For Each item In value.WorkingConcertInformations
                For Each comp In item.Compositions
                    Dim foundRec = Library.FindRecommendation(comp)
                    If foundRec IsNot Nothing Then
                        comp.SetRecommendation(foundRec, False)
                    End If
                Next
            Next

            _WorkingSeasonInformation = value

            EnsureNoSeasonPlanningDuplicates()

            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(WorkingSeasonInformation)))
        End Set
    End Property
    Private _WorkingSeasonInformation As LocalConcertInformations

    Friend Property SeasonPlannerItems As SeasonPlanningList
        Get
            Return _SeasonPlannerItems
        End Get
        Set(value As SeasonPlanningList)
            For Each item In value
                Dim foundRec = Library.FindRecommendation(item)
                If foundRec IsNot Nothing Then
                    item.SetRecommendation(foundRec, False)
                End If
            Next

            _SeasonPlannerItems = value

            EnsureNoSeasonPlanningDuplicates()

            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SeasonPlannerItems)))
        End Set
    End Property
    Private _SeasonPlannerItems As SeasonPlanningList

    Friend Property HiddenSeasonPlannerItems As SeasonPlanningList
        Get
            Return _HiddenSeasonPlannerItems
        End Get
        Set(value As SeasonPlanningList)
            For Each item In value
                Dim foundRec = Library.FindRecommendation(item)
                If foundRec IsNot Nothing Then
                    item.SetRecommendation(foundRec, False)
                End If
            Next

            _HiddenSeasonPlannerItems = value

            EnsureNoSeasonPlanningDuplicates()

            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HiddenSeasonPlannerItems)))
        End Set
    End Property
    Private _HiddenSeasonPlannerItems As SeasonPlanningList

    Private _PublishedSeasonsIndexUpdatedAt As Date = Date.MinValue
    Private WithEvents _timerSeasonPlannerIndexes As New Timers.Timer(30000)

    Friend Function InitializeSeasonPlanningIndexes() As Boolean
        Try
            Return LoadSeasonPlanningIndexes()
        Finally
            _timerSeasonPlannerIndexes.Start()
        End Try

    End Function

    Private Sub OnLoadSeasonPlanningIndexesTimerElapsed() Handles _timerSeasonPlannerIndexes.Elapsed
        LoadSeasonPlanningIndexes()
    End Sub
    Private Function LoadSeasonPlanningIndexes(Optional ftpClient As FtpClient = Nothing) As Boolean
        Dim shouldStartTimer As Boolean = _timerSeasonPlannerIndexes.Enabled
        _timerSeasonPlannerIndexes.Stop()

        Dim needsUpdate As Boolean = False
        Dim success As Boolean = True

        Try
            Dim client As FtpClient = Nothing
            Try
                If ftpClient IsNot Nothing Then
                    client = ftpClient
                Else
                    client = FtpAndSecurity.GetFtpConnection(My.Settings.Passcode)
                    client.AutoConnect()
                End If

                If CheckNeedsPublishedSeasonsUpdate(client) Then
                    If client.FileExists(FtpPath_PublishedSeasons) Then
                        RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {
                                                   .Caption = "Updating Published Seasons...",
                                                   .Minimum = 0, .Maximum = 100, .Value = 0})

                        Dim dlStatus = client.DownloadFile(LocalPath_PublishedSeasons, FtpPath_PublishedSeasons, FtpLocalExists.Overwrite, FtpVerify.Retry,
                                                                   Sub(prg As FtpProgress)
                                                                       RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Value = prg.Progress})
                                                                   End Sub)
                        success = (dlStatus = FtpStatus.Success)
                        If success Then
                            _PublishedSeasonsIndexUpdatedAt = client.GetModifiedTime(FtpPath_PublishedSeasons)
                        End If
                        needsUpdate = True
                    End If
                Else
                    If _PublishedSeasonIndexes Is Nothing Then
                        _PublishedSeasonIndexes = New PublishedSeasonIndexes
                        needsUpdate = True
                    End If

                    Return True
                End If

            Catch ex As Exception
                ' TODO Log
                success = False
            Finally
                If ftpClient Is Nothing Then
                    client?.Disconnect()
                    client?.Dispose()
                End If
            End Try

            If success Then
                If IO.File.Exists(LocalPath_PublishedSeasons) Then
                    Dim ser As New Xml.Serialization.XmlSerializer(GetType(PublishedSeasonIndexes))
                    Using fs As New IO.FileStream(LocalPath_PublishedSeasons, IO.FileMode.Open)
                        Dim newSeasons As PublishedSeasonIndexes = ser.Deserialize(fs)
                        _PublishedSeasonIndexes = newSeasons
                        needsUpdate = True
                    End Using
                Else
                    If _PublishedSeasonIndexes Is Nothing Then
                        _PublishedSeasonIndexes = New PublishedSeasonIndexes
                        needsUpdate = True
                    End If
                End If

                Return True
            Else
                RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                        .ErrorCode = ErrorCodeEventArgs.ErrorCodes.PublishedSeasonIndexFailedToDownload,
                                        .ShowMode = ErrorCodeEventArgs.ShowModes.StatusBar})
                Return False
            End If

        Finally
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Visible = False})

            If success AndAlso needsUpdate Then
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PublishedSeasonIndexes)))
            End If

            If shouldStartTimer Then _timerSeasonPlannerIndexes.Start()
        End Try
    End Function

    Private Function CheckNeedsPublishedSeasonsUpdate(client As FtpClient) As Boolean
        If _PublishedSeasonsIndexUpdatedAt = Date.MinValue Then Return True

        If client.FileExists(FtpPath_PublishedSeasons) Then
            Return client.GetModifiedTime(FtpPath_PublishedSeasons) > _PublishedSeasonsIndexUpdatedAt
        Else
            Return False
        End If
    End Function

    Friend Function ReplaceWorkingSeasonFromIndex(publishedSeasonIndex As PublishedSeasonIndex) As Boolean
        Dim client As FtpClient = Nothing
        Try
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Loading Season Data...", .IsMarquee = True})

            client = FtpAndSecurity.GetFtpConnection(My.Settings.Passcode)
            client.AutoConnect()

            Dim fileName = IO.Path.GetFileName(publishedSeasonIndex.ftpPath)

            If client.FileExists(publishedSeasonIndex.ftpPath) Then
                Dim dlStatus = client.DownloadFile(fileName, publishedSeasonIndex.ftpPath,, FtpVerify.Retry,
                                              Sub(prg)
                                                  RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {
                                                                             .Minimum = 0,
                                                                             .Maximum = 100,
                                                                             .Value = prg.Progress})
                                              End Sub)

                If Not dlStatus = FtpStatus.Success Then
                    RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                            .ErrorCode = ErrorCodeEventArgs.ErrorCodes.LoadSeasonProposalFromIndexFailedBecauseTheFileFailedToDownload})
                    Return False
                End If

                Using fs As New IO.FileStream(fileName, IO.FileMode.Open)
                    Dim ser As New Xml.Serialization.XmlSerializer(GetType(ConcertInformations))
                    Dim newSeasonData As ConcertInformations = ser.Deserialize(fs)

                    For Each concert In newSeasonData
                        For Each composition In concert.Compositions
                            Dim foundItem = Library.FindRecommendation(composition)
                            If foundItem IsNot Nothing Then
                                composition.SetRecommendation(foundItem, False)
                            End If
                        Next
                    Next

                    Me.WorkingSeasonInformation = New LocalConcertInformations() With {.WorkingConcertInformations = newSeasonData, .WorkingSeasonIndex = publishedSeasonIndex}

                    Return True
                End Using
            Else
                RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                        .ErrorCode = ErrorCodeEventArgs.ErrorCodes.LoadSeasonProposalFromIndexFailedBecauseTheFileDoesNotExist})
                Return False
            End If

        Catch ex As Exception
            RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                    .ErrorCode = ErrorCodeEventArgs.ErrorCodes.LoadSeasonProposalFromIndexFailedUnknownReason})
            Return False
        Finally
            client?.Disconnect()
            client?.Dispose()

            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Visible = False})
        End Try
    End Function

    Friend Function SaveSeason(publishedSeasonIndex As PublishedSeasonIndex) As Boolean
        Return SaveSeason(publishedSeasonIndex, Me.WorkingSeasonInformation.WorkingConcertInformations)
    End Function

    Friend Function SaveSeason(publishedSeasonIndex As PublishedSeasonIndex, infos As ConcertInformations) As Boolean
        Dim shouldStartTimer As Boolean = _timerSeasonPlannerIndexes.Enabled
        _timerSeasonPlannerIndexes.Stop()

        Dim client As FtpClient = Nothing
        Try
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Saving Season Data...", .IsMarquee = True})

            client = FtpAndSecurity.GetFtpConnection(My.Settings.Passcode)
            client.AutoConnect()

            If CheckNeedsPublishedSeasonsUpdate(client) Then
                LoadSeasonPlanningIndexes(client)
            End If

            Dim foundIndex = PublishedSeasonIndexes.ToList.Find(Function(x) x.Name.ToUpper.Trim = publishedSeasonIndex.Name.ToUpper.Trim)
            If foundIndex IsNot Nothing Then
                PublishedSeasonIndexes.Remove(foundIndex)
            End If
            PublishedSeasonIndexes.Add(publishedSeasonIndex)

            SaveWorkingSeasonInformation(publishedSeasonIndex, infos)

            Dim fileName = IO.Path.GetFileName(publishedSeasonIndex.ftpPath)
            Using fs As New IO.FileStream(fileName, IO.FileMode.Create)
                Dim ser As New Xml.Serialization.XmlSerializer(GetType(ConcertInformations))
                ser.Serialize(fs, infos)
            End Using

            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Uploading Season Data...", .IsMarquee = True})

            Dim dlStatus = client.UploadFile(fileName, publishedSeasonIndex.ftpPath,, True, FtpVerify.Retry,
                                              Sub(prg)
                                                  RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {
                                                                             .Minimum = 0,
                                                                             .Maximum = 100,
                                                                             .Value = prg.Progress})
                                              End Sub)

            If Not dlStatus = FtpStatus.Success Then
                RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                         .ErrorCode = ErrorCodeEventArgs.ErrorCodes.PublishSeasonProposalFailedBecauseTheFileFailedToUpload})
                Return False
            End If

            publishedSeasonIndex.LastModified = Now

            If Not SaveSeasonIndexes(client, PublishedSeasonIndexes) Then
                RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                         .ErrorCode = ErrorCodeEventArgs.ErrorCodes.PublishSeasonProposalFailedBecauseTheTableOfContentsFailedToUpload})
                Return False
            End If

            Return True
        Catch ex As Exception
            RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                     .ErrorCode = ErrorCodeEventArgs.ErrorCodes.PublishSeasonProposalFailedUnknownReason})
            Return False
        Finally
            client?.Disconnect()
            client?.Dispose()

            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Visible = False})
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PublishedSeasonIndexes)))

            If shouldStartTimer Then _timerSeasonPlannerIndexes.Start()
        End Try

    End Function

    Friend Function DeleteSeason(publishedSeasonIndex As PublishedSeasonIndex) As Boolean
        Dim shouldStartTimer As Boolean = _timerSeasonPlannerIndexes.Enabled
        _timerSeasonPlannerIndexes.Stop()

        Dim client As FtpClient = Nothing
        Try
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Deleting Season...", .IsMarquee = True})

            client = FtpAndSecurity.GetFtpConnection(My.Settings.Passcode)
            client.AutoConnect()

            If CheckNeedsPublishedSeasonsUpdate(client) Then
                LoadSeasonPlanningIndexes(client)
            End If

            Dim foundIndex = PublishedSeasonIndexes.ToList.Find(Function(x) x.Name.ToUpper.Trim = publishedSeasonIndex.Name.ToUpper.Trim)
            If foundIndex IsNot Nothing Then
                PublishedSeasonIndexes.Remove(foundIndex)
            End If

            publishedSeasonIndex.IsDeleted = True

            If Not SaveSeasonIndexes(client, PublishedSeasonIndexes) Then
                RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                         .ErrorCode = ErrorCodeEventArgs.ErrorCodes.DeleteSeasonProposalFailedBecauseTheTableOfContentsFailedToUpload})
                Return False
            End If

            Return True
        Catch ex As Exception
            RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                     .ErrorCode = ErrorCodeEventArgs.ErrorCodes.DeleteSeasonProposalFailedUnknownReason})
            Return False
        Finally
            client?.Disconnect()
            client?.Dispose()

            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Visible = False})
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PublishedSeasonIndexes)))

            If shouldStartTimer Then _timerSeasonPlannerIndexes.Start()
        End Try

    End Function


    Friend Async Function LoadSeasonPlannerItems() As Task(Of Boolean)
        Try
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Loading Season Planner...", .IsMarquee = True})

            Dim seasonPlannerItemsTemp As SeasonPlanningList = Await Task.Run(
                Function()
                    Dim seasonPlannerItemsInternal As SeasonPlanningList

                    If IO.File.Exists(LocalPath_SeasonPlanningItems) Then

                        Try
                            Using fs As New IO.FileStream(LocalPath_SeasonPlanningItems, IO.FileMode.Open)
                                Dim ser As New Xml.Serialization.XmlSerializer(GetType(SeasonPlanningList))
                                seasonPlannerItemsInternal = ser.Deserialize(fs)
                                For Each item In seasonPlannerItemsInternal
                                    Dim foundRec = Me.Library.FindRecommendation(item)
                                    If foundRec IsNot Nothing Then
                                        item.SetRecommendation(foundRec, False)
                                    End If
                                Next
                            End Using
                        Catch ex As Exception
                            Try
                                Using fs As New IO.FileStream(LocalPath_SeasonPlanningItems, IO.FileMode.Open)
                                    Dim ser As New Xml.Serialization.XmlSerializer(GetType(RecommendationList))
                                    Dim recList As RecommendationList = ser.Deserialize(fs)
                                    seasonPlannerItemsInternal = New SeasonPlanningList
                                    For Each recItem In recList
                                        seasonPlannerItemsInternal.Add(recItem)
                                    Next
                                End Using
                            Catch ex2 As Exception

                            End Try
                        End Try

                    End If
                    If seasonPlannerItemsInternal Is Nothing Then seasonPlannerItemsInternal = New SeasonPlanningList

                    Return seasonPlannerItemsInternal
                End Function)

            Me.HiddenSeasonPlannerItems = New SeasonPlanningList
            Me.SeasonPlannerItems = New SeasonPlanningList
            For Each seasonItem In seasonPlannerItemsTemp
                Dim rec = Library.FindRecommendation(seasonItem)
                If rec IsNot Nothing Then
                    Me.SeasonPlannerItems.Add(New SeasonItem(rec))
                Else
                    Me.SeasonPlannerItems.Add(seasonItem)
                End If
            Next

            Return True
        Catch ex As Exception
            RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                     .ErrorCode = ErrorCodeEventArgs.ErrorCodes.CouldNotLoadUserSeasonPlanningList,
                                     .ShowMode = ErrorCodeEventArgs.ShowModes.StatusBar})
            Return False
        Finally
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Visible = False})
        End Try
    End Function

    Friend Function SaveSeasonPlannerItems() As Boolean
        Try
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Saving Season Planner...", .IsMarquee = True})

            Using fs As New IO.FileStream(LocalPath_SeasonPlanningItems, IO.FileMode.Create)
                Dim ser As New Xml.Serialization.XmlSerializer(GetType(SeasonPlanningList))

                Dim combinedSpl As New List(Of SeasonItem)
                For Each item In SeasonPlannerItems
                    combinedSpl.Add(item)
                Next
                For Each item In _HiddenSeasonPlannerItems
                    combinedSpl.Add(item)
                Next
                combinedSpl.Sort(Function(x, y)
                                     Dim compScore = x.Composer.CompareTo(y.Composer)
                                     If compScore <> 0 Then Return compScore
                                     Return x.Title.CompareTo(y.Title)
                                 End Function)

                Dim splToSave As New SeasonPlanningList
                For Each item In combinedSpl
                    If splToSave.FindSeasonItem(item) Is Nothing Then
                        splToSave.Add(item)
                    End If
                Next

                ser.Serialize(fs, splToSave)
            End Using

            Return True
        Catch ex As Exception
            RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                     .ErrorCode = ErrorCodeEventArgs.ErrorCodes.CouldNotSaveUserSeasonPlanningList})
            Return False
        Finally
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Visible = False})
        End Try
    End Function

    Friend Async Function LoadWorkingSeasonInformation() As Task(Of Boolean)
        Try
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Loading Local Season Data...", .IsMarquee = True})

            Dim localInfos As LocalConcertInformations = Await Task.Run(
                Function()
                    Dim localInfosInternal As LocalConcertInformations

                    If IO.File.Exists(LocalPath_WorkingSeasonInfo) Then
                        Using fs As New IO.FileStream(LocalPath_WorkingSeasonInfo, IO.FileMode.Open)
                            Dim ser As New Xml.Serialization.XmlSerializer(GetType(LocalConcertInformations))
                            localInfosInternal = ser.Deserialize(fs)
                        End Using

                        For Each concertInfo In localInfosInternal.WorkingConcertInformations
                            For Each composition In concertInfo.Compositions
                                Dim foundRec = Me.Library.FindRecommendation(composition)
                                If foundRec IsNot Nothing Then
                                    composition.SetRecommendation(foundRec, False)
                                End If
                            Next
                        Next
                    Else
                        localInfosInternal = New LocalConcertInformations() With {.WorkingConcertInformations = New ConcertInformations}
                    End If

                    Return localInfosInternal
                End Function)

            Me.WorkingSeasonInformation = localInfos

            Return True
        Catch ex As Exception
            RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                     .ErrorCode = ErrorCodeEventArgs.ErrorCodes.CouldNotLoadUserWorkingSeasonData})
            Return False
        Finally
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Visible = False})
        End Try
    End Function

    Friend Function SaveWorkingSeasonInformation(publishedSeasonIndex As PublishedSeasonIndex, infos As ConcertInformations) As Boolean
        Try
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Saving Season Data...", .IsMarquee = True})


            Dim localInfos As New LocalConcertInformations With {.WorkingConcertInformations = infos, .WorkingSeasonIndex = publishedSeasonIndex}

            Using fs As New IO.FileStream(LocalPath_WorkingSeasonInfo, IO.FileMode.Create)
                Dim ser As New Xml.Serialization.XmlSerializer(GetType(LocalConcertInformations))
                ser.Serialize(fs, localInfos)
            End Using

            Return True
        Catch ex As Exception
            RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                     .ErrorCode = ErrorCodeEventArgs.ErrorCodes.CouldNotSaveUserWorkingSeasonData})
            Return False
        Finally
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Visible = False})
        End Try
    End Function

    Private Function SaveSeasonIndexes(client As FtpClient, publishedSeasonIndexes As PublishedSeasonIndexes) As Boolean
        Dim shouldStartTimer As Boolean = _timerSeasonPlannerIndexes.Enabled
        _timerSeasonPlannerIndexes.Stop()

        Try

            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Saving Season Table of Contents...", .IsMarquee = True})

            Using fs As New IO.FileStream(LocalPath_PublishedSeasons, IO.FileMode.Create)
                Dim ser As New Xml.Serialization.XmlSerializer(GetType(PublishedSeasonIndexes))
                ser.Serialize(fs, publishedSeasonIndexes)
            End Using

            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Uploading Season Table of Contents...", .Minimum = 0, .Maximum = 100, .Value = 0})

            Dim dlStatus = client.UploadFile(LocalPath_PublishedSeasons, FtpPath_PublishedSeasons,,, FtpVerify.Retry,
                                              Sub(prg)
                                                  RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Value = prg.Progress})
                                              End Sub)

            If Not dlStatus = FtpStatus.Success Then
                Return False
            End If

            Return True
        Catch ex As Exception
            Return False
        Finally
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Visible = False})
            If shouldStartTimer Then _timerSeasonPlannerIndexes.Start()
        End Try

    End Function

    Friend Sub CopyCurrentSeasonItemsToPlanningList()
        For Each concertInfo In WorkingSeasonInformation.WorkingConcertInformations
            For Each compositionInfo In concertInfo.Compositions
                If Me.SeasonPlannerItems.FindSeasonItem(compositionInfo) Is Nothing AndAlso
                    Me.HiddenSeasonPlannerItems.FindSeasonItem(compositionInfo) Is Nothing Then

                    Me.HiddenSeasonPlannerItems.Add(compositionInfo)
                End If
            Next
        Next
    End Sub

    Private Sub EnsureNoSeasonPlanningDuplicates()

        ' First, move all hidden items to the season list (except dupes).
        If SeasonPlannerItems IsNot Nothing Then
            While _HiddenSeasonPlannerItems.Count > 0
                Dim hiddenItem = _HiddenSeasonPlannerItems.First
                _HiddenSeasonPlannerItems.RemoveAt(0)
                If SeasonPlannerItems.FindSeasonItem(hiddenItem) Is Nothing Then
                    SeasonPlannerItems.Add(hiddenItem)
                End If
            End While

            Dim usedItems As New SeasonPlanningList
            Dim splToDelete As New List(Of SeasonItem)
            For Each item In SeasonPlannerItems
                If usedItems.FindSeasonItem(item) IsNot Nothing Then
                    splToDelete.Add(item)
                Else
                    usedItems.Add(item)
                End If
            Next
            For Each item In splToDelete
                SeasonPlannerItems.Remove(item)
            Next
        End If

        ' Next, make sure that the concerts don't have dupes.
        Dim usedCompositions As New SeasonPlanningList
        If WorkingSeasonInformation IsNot Nothing AndAlso WorkingSeasonInformation.WorkingConcertInformations IsNot Nothing Then
            ' Make sure no duplicates on concerts
            For Each concertInfo In WorkingSeasonInformation.WorkingConcertInformations

                Dim compositionsToDelete As New List(Of SeasonItem)

                For Each composition In concertInfo.Compositions
                    If usedCompositions.FindSeasonItem(composition) IsNot Nothing Then
                        compositionsToDelete.Add(composition)
                    Else
                        usedCompositions.Add(composition)
                    End If
                Next

                For Each compositionToDelete In compositionsToDelete
                    concertInfo.Compositions.Remove(compositionToDelete)
                Next
            Next
        End If

        ' Finally, move any items from season toolbox that are in concerts to hidden list.
        If SeasonPlannerItems IsNot Nothing Then
            ' Make sure that season toolbox doesn't contain anything from a concert.
            For Each usedComposition In usedCompositions
                Dim foundSeasonItem = SeasonPlannerItems.FindSeasonItem(usedComposition)
                If foundSeasonItem IsNot Nothing Then
                    SeasonPlannerItems.Remove(foundSeasonItem)
                    _HiddenSeasonPlannerItems.Add(foundSeasonItem)
                End If
            Next
        End If

    End Sub

#End Region

End Class
