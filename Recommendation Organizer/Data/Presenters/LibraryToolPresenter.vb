Imports System.ComponentModel
Imports System.Timers

Public Class LibraryToolPresenter : Implements INotifyPropertyChanged

#Region "Events"

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event ErrorOccurred(sender As Object, e As ErrorCodeEventArgs)
    Public Event ProgressChanged(sender As Object, e As ProgressBarEventArgs)
    Public Event SeasonPlannerListChanged(sender As Object, e As ListChangedEventArgs)

#End Region

#Region "Fields and Properties"

    Private WithEvents _Model As New LibraryToolModel

#Region "Fields and Properties - Closing"

    Friend Property IsAllowedClose As Boolean
        Get
            Return _IsAllowedClose
        End Get
        Private Set(value As Boolean)
            If _IsAllowedClose <> value Then
                _IsAllowedToCloseTimer.Stop()

                If Not value Then
                    _IsAllowedClose = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsAllowedClose)))
                Else
                    _IsAllowedToCloseTimer.Start()
                End If
            End If
        End Set
    End Property
    Private WithEvents _IsAllowedToCloseTimer As New Timers.Timer(1000)
    Private Sub OnIsAllowedToCloseTimerElapsed(sender As Object, e As ElapsedEventArgs) Handles _IsAllowedToCloseTimer.Elapsed
        _IsAllowedToCloseTimer.Stop()
        _IsAllowedClose = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsAllowedClose)))
    End Sub
    Private _IsAllowedClose As Boolean = True

#End Region

#Region "Fields and Properties - Library Data"

    Friend ReadOnly Property LibraryHasChanges As Boolean
        Get
            Return _Model.LibraryHasChanges
        End Get
    End Property

    Friend ReadOnly Property Library As RecommendationList
        Get
            Return _Model.Library
        End Get
    End Property

    Friend ReadOnly Property Composers As ComposerAliases
        Get
            Return _Model.Composers
        End Get
    End Property

    Friend ReadOnly Property Eras As Eras
        Get
            Return _Model.Eras
        End Get
    End Property

    Friend ReadOnly Property Tags As Tags
        Get
            Return _Model.Tags
        End Get
    End Property

#End Region

#Region "Fields and Properties - Seasons"

    Friend ReadOnly Property PublishedSeasonIndexes As PublishedSeasonIndexes
        Get
            Return _Model.PublishedSeasonIndexes
        End Get
    End Property

    Friend ReadOnly Property WorkingSeasonInformation As LocalConcertInformations
        Get
            Return _Model.WorkingSeasonInformation
        End Get
    End Property

    Friend ReadOnly Property SeasonPlannerItems As SeasonPlanningList
        Get
            Return _Model.SeasonPlannerItems
        End Get
    End Property

    Friend ReadOnly Property HiddenSeasonPlannerItems As SeasonPlanningList
        Get
            Return _Model.HiddenSeasonPlannerItems
        End Get
    End Property

#End Region

#Region "Fields and Properties - Loading Status"

    Friend Property LibraryIsLoaded As Boolean
        Get
            Return _LibraryIsLoaded
        End Get
        Set(value As Boolean)
            _LibraryIsLoaded = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LibraryIsLoaded)))
        End Set
    End Property
    Private _LibraryIsLoaded As Boolean = False

    Friend Property LibraryLoadError As Boolean
        Get
            Return _LibraryLoadError
        End Get
        Set(value As Boolean)
            _LibraryLoadError = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LibraryLoadError)))
        End Set
    End Property
    Private _LibraryLoadError As Boolean = False

    Friend Property ComposersAreLoaded As Boolean
        Get
            Return _ComposersAreLoaded
        End Get
        Set(value As Boolean)
            _ComposersAreLoaded = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ComposersAreLoaded)))
        End Set
    End Property
    Private _ComposersAreLoaded As Boolean = False

    Friend Property ComposersLoadError As Boolean
        Get
            Return _ComposersLoadError
        End Get
        Set(value As Boolean)
            _ComposersLoadError = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ComposersLoadError)))
        End Set
    End Property
    Private _ComposersLoadError As Boolean = False

    Friend Property ErasAreLoaded As Boolean
        Get
            Return _ErasAreLoaded
        End Get
        Set(value As Boolean)
            _ErasAreLoaded = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ErasAreLoaded)))
        End Set
    End Property
    Private _ErasAreLoaded As Boolean = False

    Friend Property ErasLoadError As Boolean
        Get
            Return _ErasLoadError
        End Get
        Set(value As Boolean)
            _ErasLoadError = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ErasLoadError)))
        End Set
    End Property
    Private _ErasLoadError As Boolean = False

    Friend Property TagsAreLoaded As Boolean
        Get
            Return _TagsAreLoaded
        End Get
        Set(value As Boolean)
            _TagsAreLoaded = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TagsAreLoaded)))
        End Set
    End Property
    Private _TagsAreLoaded As Boolean = False

    Friend Property TagsLoadError As Boolean
        Get
            Return _TagsLoadError
        End Get
        Set(value As Boolean)
            _TagsLoadError = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TagsLoadError)))
        End Set
    End Property
    Private _TagsLoadError As Boolean = False

    Friend Property SeasonPlannerListIsLoaded As Boolean
        Get
            Return _SeasonPlannerListIsLoaded
        End Get
        Set(value As Boolean)
            _SeasonPlannerListIsLoaded = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SeasonPlannerListIsLoaded)))
        End Set
    End Property
    Private _SeasonPlannerListIsLoaded As Boolean = False

    Friend Property SeasonPlannerListLoadError As Boolean
        Get
            Return _SeasonPlannerListError
        End Get
        Set(value As Boolean)
            _SeasonPlannerListError = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SeasonPlannerListLoadError)))
        End Set
    End Property
    Private _SeasonPlannerListError As Boolean = False

    Friend Property WorkingSeasonIsLoaded As Boolean
        Get
            Return _WorkingSeasonIsLoaded
        End Get
        Set(value As Boolean)
            _WorkingSeasonIsLoaded = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(WorkingSeasonIsLoaded)))
        End Set
    End Property
    Private _WorkingSeasonIsLoaded As Boolean = False

    Friend Property WorkingSeasonLoadError As Boolean
        Get
            Return _WorkingSeasonError
        End Get
        Set(value As Boolean)
            _WorkingSeasonError = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(WorkingSeasonLoadError)))
        End Set
    End Property
    Private _WorkingSeasonError As Boolean = False

    Friend Property SeasonIndexesAreLoaded As Boolean
        Get
            Return _SeasonIndexesAreLoaded
        End Get
        Set(value As Boolean)
            _SeasonIndexesAreLoaded = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SeasonIndexesAreLoaded)))
        End Set
    End Property
    Private _SeasonIndexesAreLoaded As Boolean = False

    Friend Property SeasonIndexesLoadError As Boolean
        Get
            Return _SeasonIndexesError
        End Get
        Set(value As Boolean)
            _SeasonIndexesError = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SeasonIndexesLoadError)))
        End Set
    End Property
    Private _SeasonIndexesError As Boolean = False

#End Region

#End Region

#Region "Initialize"

    Friend Async Function Initialize() As Task(Of Boolean)
        If Not Await _Model.LoadLibrary() Then
            LibraryLoadError = True
            ComposersLoadError = True
            ErasLoadError = True
            TagsLoadError = True

            Return False
        Else
            LibraryIsLoaded = True
            LibraryLoadError = False
            ComposersAreLoaded = True
            ComposersLoadError = False
            ErasAreLoaded = True
            ErasLoadError = False
            TagsAreLoaded = True
            TagsLoadError = False
        End If

        If Not Await _Model.LoadSeasonPlannerItems() Then
            SeasonPlannerListLoadError = True
        Else
            SeasonPlannerListIsLoaded = True
            SeasonPlannerListLoadError = False
        End If

        If Not Await _Model.LoadWorkingSeasonInformation() Then
            WorkingSeasonLoadError = True
        Else
            WorkingSeasonIsLoaded = True
            WorkingSeasonLoadError = False
        End If

        If Not Await Task.Run(Function() _Model.InitializeSeasonPlanningIndexes()) Then
            SeasonIndexesLoadError = True
        Else
            SeasonIndexesAreLoaded = True
            SeasonIndexesLoadError = False
        End If

        Return True
    End Function

#End Region

#Region "Model Event Handling"

    Private WithEvents _SeasonPlannerItemsForEvents As SeasonPlanningList
    Private WithEvents _HiddenSeasonPlannerItemsForEvents As SeasonPlanningList

    Private Sub OnModelPropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _Model.PropertyChanged
        Select Case e.PropertyName
            Case NameOf(LibraryToolModel.LibraryHasChanges)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LibraryHasChanges)))
            Case NameOf(LibraryToolModel.Library)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Library)))
            Case NameOf(LibraryToolModel.Composers)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Composers)))
            Case NameOf(LibraryToolModel.Eras)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Eras)))
            Case NameOf(LibraryToolModel.Tags)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Tags)))
            Case NameOf(LibraryToolModel.PublishedSeasonIndexes)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PublishedSeasonIndexes)))
            Case NameOf(LibraryToolModel.WorkingSeasonInformation)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(WorkingSeasonInformation)))
            Case NameOf(LibraryToolModel.SeasonPlannerItems)
                _SeasonPlannerItemsForEvents = _Model.SeasonPlannerItems
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SeasonPlannerItems)))
            Case NameOf(LibraryToolModel.HiddenSeasonPlannerItems)
                _HiddenSeasonPlannerItemsForEvents = _Model.HiddenSeasonPlannerItems
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HiddenSeasonPlannerItems)))
        End Select
    End Sub

    Private Sub OnModelProgressChanged(sender As Object, e As ProgressBarEventArgs) Handles _Model.ProgressChanged
        RaiseEvent ProgressChanged(Me, e)
    End Sub

    Private Sub OnSeasonPlannerItemsListChanged(sender As Object, e As ListChangedEventArgs) Handles _SeasonPlannerItemsForEvents.ListChanged, _HiddenSeasonPlannerItemsForEvents.ListChanged
        RaiseEvent SeasonPlannerListChanged(sender, e)
    End Sub

    Private Sub OnModelErrorMessage(sender As Object, e As ErrorCodeEventArgs) Handles _Model.ErrorOccurred
        RaiseEvent ErrorOccurred(Me, e)
    End Sub

#End Region

#Region "Library Data Load, Save, and Change"

    Friend Async Function SaveLibrary() As Task(Of Boolean)
        Try
            IsAllowedClose = False
            Return Await _Model.SaveLibrary()
        Finally
            IsAllowedClose = True
        End Try
    End Function

    Friend Function ReplaceLibraryEntries(itemsToReplace As IEnumerable(Of Recommendation), newItem As Recommendation) As Boolean
        Try
            IsAllowedClose = False
            Return _Model.ReplaceLibraryEntries(itemsToReplace, newItem)
        Finally
            IsAllowedClose = True
        End Try
    End Function

    Friend Function MergeRecommendationsIntoLibrary(recommendations As IEnumerable(Of MusicianRecommendation)) As Boolean
        Try
            IsAllowedClose = False
            Return _Model.MergeRecommendationsIntoLibrary(recommendations)
        Finally
            IsAllowedClose = True
        End Try
    End Function
    Friend Function MergeHoldingsIntoLibrary(holdings As IEnumerable(Of HoldingEntry)) As Boolean
        Try
            IsAllowedClose = False
            Return _Model.MergeHoldingsIntoLibrary(holdings)
        Finally
            IsAllowedClose = True
        End Try
    End Function

    Friend Function MergeRepertoireIntoLibrary(repertoires As IEnumerable(Of RepertoireEntry)) As Boolean
        Try
            IsAllowedClose = False
            Return _Model.MergeRepertoireIntoLibrary(repertoires)
        Finally
            IsAllowedClose = True
        End Try
    End Function

    Friend Function ReplaceAliases(itemsToReplace As IEnumerable(Of ComposerAlias), newItem As ComposerAlias) As Boolean
        Try
            IsAllowedClose = False
            Return _Model.ReplaceAliases(itemsToReplace, newItem)
        Finally
            IsAllowedClose = True
        End Try
    End Function

#End Region

#Region "Season Data Load, Save, and Change"

    Friend Function SaveSeason(publishedSeasonIndex As PublishedSeasonIndex) As Boolean
        Try
            IsAllowedClose = False
            Return _Model.SaveSeason(publishedSeasonIndex)
        Finally
            IsAllowedClose = True
        End Try
    End Function

    Friend Function DeleteSeason(publishedSeasonIndex As PublishedSeasonIndex) As Boolean
        Try
            IsAllowedClose = False
            Return _Model.DeleteSeason(publishedSeasonIndex)
        Finally
            IsAllowedClose = True
        End Try
    End Function

    Friend Function SaveSeasonPlannerItems() As Boolean
        Try
            IsAllowedClose = False
            Return _Model.SaveSeasonPlannerItems()
        Finally
            IsAllowedClose = True
        End Try
    End Function

    Friend Function SaveWorkingSeasonInformation(publishedSeasonIndex As PublishedSeasonIndex, infos As ConcertInformations) As Boolean
        Try
            IsAllowedClose = False
            Return _Model.SaveWorkingSeasonInformation(publishedSeasonIndex, infos)
        Finally
            IsAllowedClose = True
        End Try
    End Function

    Friend Function NewWorkingSeasonInformation() As Boolean
        Try
            IsAllowedClose = False
            Return _Model.NewWorkingSeasonInformation()
        Finally
            IsAllowedClose = True
        End Try
    End Function

    Friend Function ReplaceWorkingSeasonFromIndex(index As PublishedSeasonIndex) As Boolean
        Try
            IsAllowedClose = False
            Return _Model.ReplaceWorkingSeasonFromIndex(index)
        Finally
            IsAllowedClose = True
        End Try
    End Function

    Friend Sub AddSeasonPlannerItem(reccomendation As Recommendation, Optional insertAtIndex As Integer = -1)
        Try
            IsAllowedClose = False
            _Model.AddSeasonPlannerItem(reccomendation, insertAtIndex)
        Finally
            IsAllowedClose = True
        End Try
    End Sub

    Friend Sub RemoveSeasonPlannerItem(reccomendation As Recommendation)
        Try
            IsAllowedClose = False
            _Model.RemoveSeasonPlannerItem(reccomendation)
        Finally
            IsAllowedClose = True
        End Try
    End Sub

    Friend Function GetSeasonPlannerContainsItem(recommendation As Recommendation) As Boolean
        Try
            IsAllowedClose = False
            Return _Model.GetSeasonPlannerContainsItem(recommendation)
        Finally
            IsAllowedClose = True
        End Try
    End Function

    Friend Sub CopyCurrentSeasonItemsToPlanningList()
        Try
            IsAllowedClose = False
            _Model.CopyCurrentSeasonItemsToPlanningList()
        Finally
            IsAllowedClose = True
        End Try
    End Sub

    Friend Sub ClearSeasonPlanningList()
        Try
            IsAllowedClose = False
            _Model.ClearSeasonPlanningList()
        Finally
            IsAllowedClose = True
        End Try
    End Sub

    Friend Function GetTotalSeasonPlannerItemCount() As Integer
        Return If(SeasonPlannerItems?.Count, 0) + If(HiddenSeasonPlannerItems?.Count, 0)
    End Function

    Friend Function GetHiddenSeasonPlannerItemCount() As Integer
        Return If(HiddenSeasonPlannerItems?.Count, 0)
    End Function

    Friend Function GetVisibleSeasonPlannerItemCount() As Integer
        Return If(SeasonPlannerItems?.Count, 0)
    End Function

#End Region

End Class
