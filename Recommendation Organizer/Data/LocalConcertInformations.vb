Imports System.ComponentModel

Public Class LocalConcertInformations : Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Property WorkingConcertInformations As ConcertInformations
        Get
            Return _WorkingConcertInformations
        End Get
        Set(value As ConcertInformations)
            _WorkingConcertInformations = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(WorkingConcertInformations)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Eras)))
        End Set
    End Property
    Private WithEvents _WorkingConcertInformations As ConcertInformations = Nothing

    Property WorkingSeasonIndex As PublishedSeasonIndex
        Get
            Return _WorkingSeasonIndex
        End Get
        Set(value As PublishedSeasonIndex)
            _WorkingSeasonIndex = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(WorkingSeasonIndex)))
        End Set
    End Property
    Private _WorkingSeasonIndex As PublishedSeasonIndex = Nothing

    ReadOnly Property Eras As Eras
        Get
            Return _WorkingConcertInformations.Eras
        End Get
    End Property

    Private Sub OnConertInformationsPropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _WorkingConcertInformations.PropertyChanged
        If e.PropertyName = NameOf(ConcertInformations.Eras) Then
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Eras)))
        End If
    End Sub

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
