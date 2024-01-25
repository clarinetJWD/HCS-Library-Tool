Public Class LocalConcertInformations
    Property WorkingConcertInformations As ConcertInformations
    Property WorkingSeasonIndex As PublishedSeasonIndex

    Friend Function ContainsComposition(seasonItem As SeasonItem) As Boolean
        If WorkingConcertInformations Is Nothing Then Return False
        For Each concertInfo In WorkingConcertInformations
            If concertInfo.Compositions.FindSeasonItem(seasonItem) IsNot Nothing Then
                Return True
            End If
        Next
        Return False
    End Function
End Class