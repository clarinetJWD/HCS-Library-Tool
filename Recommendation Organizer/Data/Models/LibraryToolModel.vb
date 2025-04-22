Imports System.ComponentModel
Imports System.Net
Imports DevExpress.Drawing.Internal.Images
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

            success = Await Task.Run(Function() FtpDownloadFile(LocalPath_Library, FtpPath_Library, True, "Loading Library...", 5))

            Dim newLibraryData As LibraryData = Await Task.Run(
                Function()
                    Dim newLibraryDataInternal As LibraryData = Nothing

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

    Friend Sub AddSeasonPlannerItem(seasonItem As Recommendation, Optional insertAtIndex As Integer = -1)
        If Me.WorkingSeasonInformation?.WorkingSeasonInformation?.SeasonPlannerItems Is Nothing Then Exit Sub
        Me.WorkingSeasonInformation.WorkingSeasonInformation.AddSeasonPlannerItem(seasonItem, insertAtIndex)
    End Sub

    Friend Sub RemoveSeasonPlannerItem(recommendation As Recommendation)
        If Me.WorkingSeasonInformation?.WorkingSeasonInformation?.SeasonPlannerItems Is Nothing Then Exit Sub
        Me.WorkingSeasonInformation.WorkingSeasonInformation.RemoveSeasonPlannerItem(recommendation)
    End Sub

    Friend Function GetSeasonPlannerContainsItem(recommendation As Recommendation) As Boolean
        If Me.WorkingSeasonInformation?.WorkingSeasonInformation?.SeasonPlannerItems Is Nothing Then Return False
        Return Me.WorkingSeasonInformation.WorkingSeasonInformation.GetSeasonPlannerContainsItem(recommendation)
    End Function

    Friend Async Function SaveLibrary() As Task(Of Boolean)
        Return Await SaveLibrary(Me._LibraryData)
    End Function

    Friend Async Function SaveLibrary(libraryData As LibraryData) As Task(Of Boolean)

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
                    Dim successTask = Task.Run(Function() FtpDownloadFile(LocalPath_Library_Temp("temp"), FtpPath_Library, True, "Saving Library...", 5))
                    Dim success As Boolean = successTask.Result

                    If success Then
                        Using fs As New IO.FileStream(LocalPath_Library_Temp("temp"), IO.FileMode.Open)
                            Dim tmpLib As LibraryData = ser.Deserialize(fs)
                            If tmpLib.LastChanged <= prevLibDate Then

                                Dim libBackupSuccess = True
                                If Ftp.FileExists(FtpPath_Library) Then
                                    libBackupSuccess = Ftp.MoveFile(FtpPath_Library, FtpPath_Library_Temp(Now.ToString("yyyyMMddHHmmssfff")))
                                End If

                                If Not libBackupSuccess Then
                                    uploadFailedCode = ErrorCodeEventArgs.ErrorCodes.SaveLibraryFailedBecauseRemoteFileCouldNotBeReplaced
                                    uploadFailedShowMode = ErrorCodeEventArgs.ShowModes.StatusBar
                                Else
                                    Dim upStat = FtpUploadFile(LocalPath_Library, FtpPath_Library, True, "Uploading Library...", 5)

                                    If Not upStat Then
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
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs With {
                                                       .Visible = False})
        End Try

    End Function

    Friend Function ReplaceLibraryEntries(itemsToReplace As IEnumerable(Of Recommendation), newItem As Recommendation) As Boolean
        For Each selection In itemsToReplace
            Library.Remove(selection)
        Next
        Library.Add(newItem)

        If Me.WorkingSeasonInformation?.WorkingSeasonInformation?.SeasonPlannerItems Is Nothing Then Return False
        Return Me.WorkingSeasonInformation.WorkingSeasonInformation.ReplaceLibraryEntries(itemsToReplace, newItem)

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

    Friend Property WorkingSeasonInformation As LocalSeasonInformation
        Get
            Return _WorkingSeasonInformation
        End Get
        Private Set(value As LocalSeasonInformation)
            For Each item In value.WorkingSeasonInformation.ConcertInformations
                For Each comp In item.Compositions
                    Dim foundRec = Library.FindRecommendation(comp)
                    If foundRec IsNot Nothing Then
                        comp.SetRecommendation(foundRec, False)
                    End If
                Next
            Next

            _WorkingSeasonInformation = value

            value.WorkingSeasonInformation.EnsureNoSeasonPlanningDuplicates()

            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(WorkingSeasonInformation)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SeasonPlannerItems)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HiddenSeasonPlannerItems)))
        End Set
    End Property
    Private _WorkingSeasonInformation As LocalSeasonInformation

    Friend ReadOnly Property SeasonPlannerItems As SeasonPlanningList
        Get
            Return Me.WorkingSeasonInformation?.WorkingSeasonInformation?.SeasonPlannerItems
        End Get
    End Property

    Friend ReadOnly Property HiddenSeasonPlannerItems As SeasonPlanningList
        Get
            Return Me.WorkingSeasonInformation?.WorkingSeasonInformation?.HiddenSeasonPlannerItems
        End Get
    End Property


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
    Private Function LoadSeasonPlanningIndexes() As Boolean
        Dim shouldStartTimer As Boolean = _timerSeasonPlannerIndexes.Enabled
        _timerSeasonPlannerIndexes.Stop()

        Dim needsUpdate As Boolean = False
        Dim success As Boolean = True

        Try
            Try
                If CheckNeedsPublishedSeasonsUpdate() Then
                    Dim successTask = HttpDownloadFileAsync(LocalPath_PublishedSeasons, HttpPath_PublishedSeasons, True, "Updating Published Seasons...")
                    success = successTask.Result

                    If success Then
                        _PublishedSeasonsIndexUpdatedAt = Ftp.GetModifiedTime(FtpPath_PublishedSeasons)
                    End If
                    needsUpdate = True
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

    Private Function CheckNeedsPublishedSeasonsUpdate() As Boolean
        If _PublishedSeasonsIndexUpdatedAt = Date.MinValue Then Return True

        If Ftp.FileExists(FtpPath_PublishedSeasons) Then
            Return Ftp.GetModifiedTime(FtpPath_PublishedSeasons) > _PublishedSeasonsIndexUpdatedAt
        Else
            Return False
        End If
    End Function

    Friend Function ReplaceWorkingSeasonFromIndex(publishedSeasonIndex As PublishedSeasonIndex) As Boolean
        Try
            Dim fileName = IO.Path.GetFileName(publishedSeasonIndex.ftpPath)

            If Ftp.FileExists(publishedSeasonIndex.ftpPath) Then
                Dim success = FtpDownloadFile(fileName, publishedSeasonIndex.ftpPath, True, "Loading Season Data...", 5)

                If Not success Then
                    RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                            .ErrorCode = ErrorCodeEventArgs.ErrorCodes.LoadSeasonProposalFromIndexFailedBecauseTheFileFailedToDownload})
                    Return False
                End If

                Using fs As New IO.FileStream(fileName, IO.FileMode.Open)
                    Dim ser As New Xml.Serialization.XmlSerializer(GetType(SeasonInformation))
                    Dim newSeasonData As SeasonInformation = ser.Deserialize(fs)

                    For Each concert In newSeasonData.ConcertInformations
                        For Each composition In concert.Compositions
                            Dim foundItem = Library.FindRecommendation(composition)
                            If foundItem IsNot Nothing Then
                                composition.SetRecommendation(foundItem, False)
                            End If
                        Next
                    Next

                    For Each itemList In {newSeasonData.SeasonPlannerItems, newSeasonData.HiddenSeasonPlannerItems}
                        For Each item In itemList
                            Dim foundRec = Me.Library.FindRecommendation(item)
                            If foundRec IsNot Nothing Then
                                item.SetRecommendation(foundRec, False)
                            End If
                        Next
                    Next

                    Dim newSeasonInfo = New LocalSeasonInformation() With {.WorkingSeasonInformation = newSeasonData, .WorkingSeasonIndex = publishedSeasonIndex}
                    Me.WorkingSeasonInformation = newSeasonInfo

                    Return True
                End Using
            Else
                RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                        .ErrorCode = ErrorCodeEventArgs.ErrorCodes.LoadSeasonProposalFromIndexFailedBecauseTheFileDoesNotExist})
                Return False
            End If

        Catch ex As Exception
            If ReplaceWorkingSeasonFromIndex_Legacy(publishedSeasonIndex) Then
                Return True
            Else
                RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                    .ErrorCode = ErrorCodeEventArgs.ErrorCodes.LoadSeasonProposalFromIndexFailedUnknownReason})
                Return False
            End If
        Finally
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Visible = False})
        End Try
    End Function

    Private Function ReplaceWorkingSeasonFromIndex_Legacy(publishedSeasonIndex As PublishedSeasonIndex) As Boolean
        Try
            Dim fileName = IO.Path.GetFileName(publishedSeasonIndex.ftpPath)

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

                Dim newSeasonInfo = New LocalSeasonInformation() With {.WorkingSeasonIndex = publishedSeasonIndex}
                newSeasonInfo.WorkingSeasonInformation = New SeasonInformation() With {.ConcertInformations = newSeasonData}
                Me.WorkingSeasonInformation = newSeasonInfo

                Return True
            End Using

        Catch ex As Exception
            Return False
        End Try
    End Function

    Friend Function SaveSeason(publishedSeasonIndex As PublishedSeasonIndex) As Boolean
        Return SaveSeason(publishedSeasonIndex, Me.WorkingSeasonInformation.WorkingSeasonInformation)
    End Function

    Friend Function SaveSeason(publishedSeasonIndex As PublishedSeasonIndex, infos As SeasonInformation) As Boolean
        Dim shouldStartTimer As Boolean = _timerSeasonPlannerIndexes.Enabled
        _timerSeasonPlannerIndexes.Stop()

        Try
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Saving Season Data...", .IsMarquee = True})

            If CheckNeedsPublishedSeasonsUpdate() Then
                LoadSeasonPlanningIndexes()
            End If

            Dim foundIndex = PublishedSeasonIndexes.ToList.Find(Function(x) x.Name.ToUpper.Trim = publishedSeasonIndex.Name.ToUpper.Trim)
            If foundIndex IsNot Nothing Then
                PublishedSeasonIndexes.Remove(foundIndex)
            End If
            PublishedSeasonIndexes.Add(publishedSeasonIndex)

            SaveWorkingSeasonInformation(publishedSeasonIndex, infos)

            Dim fileName = IO.Path.GetFileName(publishedSeasonIndex.ftpPath)
            Using fs As New IO.FileStream(fileName, IO.FileMode.Create)
                Dim ser As New Xml.Serialization.XmlSerializer(GetType(SeasonInformation))
                ser.Serialize(fs, infos)
            End Using

            Dim dlSuccess = FtpUploadFile(fileName, publishedSeasonIndex.ftpPath, True, "Uploading Season Data...", 5)

            If Not dlSuccess Then
                RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                         .ErrorCode = ErrorCodeEventArgs.ErrorCodes.PublishSeasonProposalFailedBecauseTheFileFailedToUpload})
                Return False
            End If

            publishedSeasonIndex.LastModified = Now

            If Not SaveSeasonIndexes(PublishedSeasonIndexes) Then
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
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Visible = False})
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PublishedSeasonIndexes)))

            If shouldStartTimer Then _timerSeasonPlannerIndexes.Start()
        End Try

    End Function

    Friend Function DeleteSeason(publishedSeasonIndex As PublishedSeasonIndex) As Boolean
        Dim shouldStartTimer As Boolean = _timerSeasonPlannerIndexes.Enabled
        _timerSeasonPlannerIndexes.Stop()

        Try
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Deleting Season...", .IsMarquee = True})

            If CheckNeedsPublishedSeasonsUpdate() Then
                LoadSeasonPlanningIndexes()
            End If

            Dim foundIndex = PublishedSeasonIndexes.ToList.Find(Function(x) x.Name.ToUpper.Trim = publishedSeasonIndex.Name.ToUpper.Trim)
            If foundIndex IsNot Nothing Then
                PublishedSeasonIndexes.Remove(foundIndex)
            End If

            publishedSeasonIndex.IsDeleted = True

            If Not SaveSeasonIndexes(PublishedSeasonIndexes) Then
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
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Visible = False})
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PublishedSeasonIndexes)))

            If shouldStartTimer Then _timerSeasonPlannerIndexes.Start()
        End Try

    End Function


    Friend Async Function LoadSeasonPlannerItems() As Task(Of Boolean)
        If Not (WorkingSeasonInformation.WorkingSeasonInformation.GetTotalSeasonPlannerItemCount = 0 AndAlso IO.File.Exists(LocalPath_SeasonPlanningItems)) Then
            Return True
        End If
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

            For Each seasonItem In seasonPlannerItemsTemp
                Dim rec = Library.FindRecommendation(seasonItem)
                If rec IsNot Nothing Then
                    Me.WorkingSeasonInformation.WorkingSeasonInformation.AddSeasonPlannerItem(New SeasonItem(rec))
                Else
                    Me.WorkingSeasonInformation.WorkingSeasonInformation.AddSeasonPlannerItem(seasonItem)
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

    Friend Async Function LoadWorkingSeasonInformation() As Task(Of Boolean)
        Try
            Dim exInternal As Exception = Nothing
            Try
                RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Loading Local Season Data...", .IsMarquee = True})

                Dim localInfos As LocalSeasonInformation = Await Task.Run(
                    Function()
                        Dim localInfosInternal As LocalSeasonInformation

                        If IO.File.Exists(LocalPath_WorkingSeasonInfo) Then
                            Using fs As New IO.FileStream(LocalPath_WorkingSeasonInfo, IO.FileMode.Open)
                                Dim ser As New Xml.Serialization.XmlSerializer(GetType(LocalSeasonInformation))
                                localInfosInternal = ser.Deserialize(fs)
                            End Using

                            For Each concertInfo In localInfosInternal.WorkingSeasonInformation.ConcertInformations
                                For Each composition In concertInfo.Compositions
                                    Dim foundRec = Me.Library.FindRecommendation(composition)
                                    If foundRec IsNot Nothing Then
                                        composition.SetRecommendation(foundRec, False)
                                    End If
                                Next
                            Next

                            For Each itemList In {localInfosInternal.WorkingSeasonInformation.SeasonPlannerItems, localInfosInternal.WorkingSeasonInformation.HiddenSeasonPlannerItems}
                                For Each item In itemList
                                    Dim foundRec = Me.Library.FindRecommendation(item)
                                    If foundRec IsNot Nothing Then
                                        item.SetRecommendation(foundRec, False)
                                    End If
                                Next
                            Next
                        Else
                            localInfosInternal = New LocalSeasonInformation With {.WorkingSeasonInformation = New SeasonInformation() With {.ConcertInformations = New ConcertInformations}}
                        End If

                        Return localInfosInternal
                    End Function)

                Me.WorkingSeasonInformation = localInfos

                Return True
            Catch ex As Exception
                exInternal = ex
            End Try

            If exInternal IsNot Nothing Then
                If Await LoadWorkingSeasonInformation_Legacy() Then
                    Return True
                Else
                    Throw exInternal
                End If
            End If

            Return True
        Catch ex As Exception
            RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                         .ErrorCode = ErrorCodeEventArgs.ErrorCodes.CouldNotLoadUserWorkingSeasonData})
            Return False
        Finally
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Visible = False})
        End Try

    End Function

    Friend Async Function LoadWorkingSeasonInformation_Legacy() As Task(Of Boolean)
        Try
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

            Dim newInfo As New LocalSeasonInformation()
            newInfo.WorkingSeasonIndex = localInfos.WorkingSeasonIndex
            newInfo.WorkingSeasonInformation = New SeasonInformation
            newInfo.WorkingSeasonInformation.ConcertInformations = localInfos.WorkingConcertInformations

            Me.WorkingSeasonInformation = newInfo

            Return True
        Catch ex As Exception
            Me.WorkingSeasonInformation = New LocalSeasonInformation With {.WorkingSeasonInformation = New SeasonInformation() With {.ConcertInformations = New ConcertInformations}}
            Return False
        End Try
    End Function

    Friend Function SaveWorkingSeasonInformation(publishedSeasonIndex As PublishedSeasonIndex, seasonInfo As SeasonInformation) As Boolean
        Try
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Saving Season Data...", .IsMarquee = True})

            Dim localInfos As New LocalSeasonInformation With {.WorkingSeasonInformation = seasonInfo, .WorkingSeasonIndex = publishedSeasonIndex}

            Using fs As New IO.FileStream(LocalPath_WorkingSeasonInfo, IO.FileMode.Create)
                Dim ser As New Xml.Serialization.XmlSerializer(localInfos.GetType)
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

    Friend Function NewWorkingSeasonInformation() As Boolean
        Try
            Dim localInfos As New LocalSeasonInformation() With {.WorkingSeasonInformation = New SeasonInformation}
            localInfos.WorkingSeasonInformation.ConcertInformations = New ConcertInformations
            Me.WorkingSeasonInformation = localInfos

            Return True
        Catch ex As Exception

            RaiseEvent ErrorOccurred(Me, New ErrorCodeEventArgs() With {
                                     .ErrorCode = ErrorCodeEventArgs.ErrorCodes.CouldNotCreateNewUserWorkingSeasonData,
                                     .ShowMode = ErrorCodeEventArgs.ShowModes.StatusBar})
            Return False
        End Try
    End Function

    Private Function SaveSeasonIndexes(publishedSeasonIndexes As PublishedSeasonIndexes) As Boolean
        Dim shouldStartTimer As Boolean = _timerSeasonPlannerIndexes.Enabled
        _timerSeasonPlannerIndexes.Stop()

        Try

            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Caption = "Saving Season Table of Contents...", .IsMarquee = True})

            Using fs As New IO.FileStream(LocalPath_PublishedSeasons, IO.FileMode.Create)
                Dim ser As New Xml.Serialization.XmlSerializer(GetType(PublishedSeasonIndexes))
                ser.Serialize(fs, publishedSeasonIndexes)
            End Using

            Dim success = FtpUploadFile(LocalPath_PublishedSeasons, FtpPath_PublishedSeasons, True, "Uploading Season Table of Contents...", 5)
            Return success

        Catch ex As Exception
            Return False
        Finally
            RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs() With {.Visible = False})
            If shouldStartTimer Then _timerSeasonPlannerIndexes.Start()
        End Try

    End Function

#End Region

#Region "FTP"

    Private Async Function HttpDownloadFileAsync(localPath As String, remotePath As String, useProgress As Boolean, progressCaption As String) As Task(Of Boolean)
        Try
            If useProgress Then
                RaiseEvent ProgressChanged(
                    Me,
                    New ProgressBarEventArgs With {
                        .IsMarquee = True,
                        .Caption = progressCaption})
            End If

            Dim completeArgs As AsyncCompletedEventArgs = Nothing


            Dim webClient As New WebClient()
            AddHandler webClient.DownloadProgressChanged,
                Sub(sender As Object, e As DownloadProgressChangedEventArgs)
                    If useProgress Then
                        RaiseEvent ProgressChanged(
                                            Me,
                                            New ProgressBarEventArgs With {
                                                .Minimum = 0,
                                                .Maximum = 100,
                                                .Value = e.ProgressPercentage,
                                                .Caption = progressCaption})
                    End If
                End Sub

            AddHandler webClient.DownloadFileCompleted, Sub(sender As Object, e As AsyncCompletedEventArgs)
                                                            completeArgs = e
                                                        End Sub

            webClient.DownloadFileAsync(New Uri(remotePath), localPath)

            Await Task.Run(Sub()
                               While completeArgs Is Nothing
                                   Threading.Thread.Sleep(100)
                               End While
                           End Sub)


            Return Not (completeArgs.Cancelled OrElse completeArgs.Error IsNot Nothing)

        Finally
            If useProgress Then
                RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs With {
                                                       .Visible = False})
            End If
        End Try

    End Function



    Private Function FtpDownloadFile(localPath As String, remotePath As String, useProgress As Boolean, progressCaption As String, numRetries As Integer) As Boolean
        Try
            If useProgress Then
                RaiseEvent ProgressChanged(
                    Me,
                    New ProgressBarEventArgs With {
                        .IsMarquee = True,
                        .Caption = progressCaption})
            End If

            Return Ftp.DownloadFile(localPath, remotePath, numRetries,
                                        Sub(prg As FtpProgress)
                                            If useProgress Then
                                                RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs With {
                                                       .Minimum = 0,
                                                       .Maximum = 100,
                                                       .Value = prg.Progress,
                                                       .Caption = progressCaption})
                                            End If

                                        End Sub)

        Finally
            If useProgress Then
                RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs With {
                                                       .Visible = False})
            End If
        End Try

    End Function

    Private Function FtpUploadFile(localPath As String, remotePath As String, useProgress As Boolean, progressCaption As String, numRetries As Integer) As Boolean
        Try
            If useProgress Then
                RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs With {
                                                                   .IsMarquee = True,
                                                                   .Caption = progressCaption})
            End If

            Return Ftp.UploadFile(localPath, remotePath, numRetries,
                                             Sub(prg As FtpProgress)
                                                 If useProgress Then
                                                     RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs With {
                                                                               .Minimum = 0,
                                                                               .Maximum = 100,
                                                                               .Value = prg.Progress,
                                                                               .Caption = progressCaption})
                                                 End If
                                             End Sub)

        Finally
            If useProgress Then
                RaiseEvent ProgressChanged(Me, New ProgressBarEventArgs With {
                                                       .Visible = False})
            End If
        End Try
    End Function

#End Region

End Class
