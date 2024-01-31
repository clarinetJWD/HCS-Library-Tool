Imports System.ComponentModel

Public Class SeasonInformation : Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Property ConcertInformations As ConcertInformations
        Get
            Return _ConcertInformations
        End Get
        Set(value As ConcertInformations)
            _ConcertInformations = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ConcertInformations)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Eras)))
        End Set
    End Property
    Private WithEvents _ConcertInformations As ConcertInformations = Nothing

    Property SeasonPlannerItems As SeasonPlanningList
        Get
            Return _SeasonPlannerItems
        End Get
        Set(value As SeasonPlanningList)
            _SeasonPlannerItems = value

            EnsureNoSeasonPlanningDuplicates()

            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SeasonPlannerItems)))
        End Set
    End Property
    Private _SeasonPlannerItems As New SeasonPlanningList

    Property HiddenSeasonPlannerItems As SeasonPlanningList
        Get
            Return _HiddenSeasonPlannerItems
        End Get
        Set(value As SeasonPlanningList)
            _HiddenSeasonPlannerItems = value

            EnsureNoSeasonPlanningDuplicates()

            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HiddenSeasonPlannerItems)))
        End Set
    End Property
    Private _HiddenSeasonPlannerItems As New SeasonPlanningList

    ReadOnly Property Eras As Eras
        Get
            Return ConcertInformations.Eras
        End Get
    End Property

    Private Sub OnConcertInformationsPropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _ConcertInformations.PropertyChanged
        If e.PropertyName = NameOf(ConcertInformations.Eras) Then
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Eras)))
        End If
    End Sub

    Friend Sub ClearSeasonPlanningList()
        Me.HiddenSeasonPlannerItems.Clear()
        Me.SeasonPlannerItems.Clear()
    End Sub

    Friend Sub AddSeasonPlannerItem(seasonItem As Recommendation, Optional insertAtIndex As Integer = -1)
        If insertAtIndex >= 0 AndAlso insertAtIndex < Me.SeasonPlannerItems.Count Then
            Me.SeasonPlannerItems.Insert(insertAtIndex, New SeasonItem(seasonItem))
        Else
            Me.SeasonPlannerItems.Add(New SeasonItem(seasonItem))
        End If

        EnsureNoSeasonPlanningDuplicates()
    End Sub

    Friend Sub RemoveSeasonPlannerItem(recommendation As Recommendation)
        If SeasonPlannerItems.FindSeasonItem(recommendation) IsNot Nothing Then
            SeasonPlannerItems.Remove(SeasonPlannerItems.FindSeasonItem(recommendation))
        Else
            For Each concertInfo In ConcertInformations
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
            For Each concertInfo In ConcertInformations
                Dim foundItem = concertInfo.Compositions.FindSeasonItem(recommendation)
                If foundItem IsNot Nothing Then
                    Return True
                End If
            Next
        End If
        Return False
    End Function

    Friend Function ReplaceLibraryEntries(itemsToReplace As IEnumerable(Of Recommendation), newItem As Recommendation) As Boolean
        For Each selection In itemsToReplace

            Dim seasonPlannerItem = Me.SeasonPlannerItems.FindSeasonItem(selection)
            If seasonPlannerItem IsNot Nothing Then seasonPlannerItem.SetRecommendation(newItem, True)

            Dim hiddenSeasonPlannerItem = Me.HiddenSeasonPlannerItems.FindSeasonItem(selection)
            If hiddenSeasonPlannerItem IsNot Nothing Then hiddenSeasonPlannerItem.SetRecommendation(newItem, True)

            For Each concert In ConcertInformations
                Dim concertItem = concert.Compositions.FindSeasonItem(selection)
                If concertItem IsNot Nothing Then concertItem.SetRecommendation(newItem, True)
            Next
        Next

        Return True
    End Function

    Friend Function GetTotalSeasonPlannerItemCount() As Integer
        Return If(SeasonPlannerItems?.Count, 0) + If(HiddenSeasonPlannerItems?.Count, 0)
    End Function

    Friend Function GetHiddenSeasonPlannerItemCount() As Integer
        Return If(HiddenSeasonPlannerItems?.Count, 0)
    End Function

    Friend Function GetVisibleSeasonPlannerItemCount() As Integer
        Return If(SeasonPlannerItems?.Count, 0)
    End Function

    Friend Sub EnsureNoSeasonPlanningDuplicates()

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
            Dim spiToDelete As New List(Of SeasonItem)
            For Each seasonItem In SeasonPlannerItems
                If usedItems.FindSeasonItem(seasonItem) IsNot Nothing Then
                    spiToDelete.Add(seasonItem)
                Else
                    usedItems.Add(seasonItem)
                End If
            Next
            For Each seasonItem In spiToDelete
                SeasonPlannerItems.Remove(seasonItem)
            Next
        End If

        ' Next, make sure that the concerts don't have dupes.
        Dim usedCompositions As New SeasonPlanningList
        If Me IsNot Nothing Then
            ' Make sure no duplicates on concerts
            For Each concertInfo In ConcertInformations

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
End Class
