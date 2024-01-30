Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

Public Class ConcertInformations : Inherits BindingList(Of ConcertInformation) : Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    ReadOnly Property Eras As Eras
        Get
            Dim returnEras As New Eras
            For Each concert In Me
                For Each era In concert.Eras
                    If returnEras.Find(era) Is Nothing Then
                        returnEras.Add(era)
                    End If
                Next
            Next
            Return returnEras
        End Get
    End Property

    Private Sub OnListChangedHandler(sender As Object, e As ListChangedEventArgs) Handles Me.ListChanged
        If e.ListChangedType = ListChangedType.ItemAdded OrElse e.ListChangedType = ListChangedType.ItemDeleted OrElse (e.ListChangedType = ListChangedType.ItemChanged AndAlso e.PropertyDescriptor.Name = NameOf(ConcertInformation.Eras)) Then
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Eras)))
        End If
    End Sub

End Class

Public Class ConcertInformation : Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    <Display(AutoGenerateField:=False)>
    Property Compositions As SeasonPlanningList
        Get
            Return _Compositions
        End Get
        Set(value As SeasonPlanningList)
            _Compositions = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Compositions)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Length)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Eras)))
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Tags)))
        End Set
    End Property
    Private WithEvents _Compositions As New SeasonPlanningList

    Property [Date] As Date
        Get
            Return _Date
        End Get
        Set(value As Date)
            _Date = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf([Date])))
        End Set
    End Property
    Private _Date As Date

    Property Location As String
        Get
            Return _Location
        End Get
        Set(value As String)
            _Location = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Location)))
        End Set
    End Property
    Private _Location As String

    ReadOnly Property Length As TimeSpan
        Get
            Dim calculatedLength As TimeSpan = TimeSpan.Zero
            For Each composition In Compositions
                calculatedLength += composition.Recommendation.Length
            Next
            Return calculatedLength
        End Get
    End Property

    <Display(AutoGenerateField:=False)>
    ReadOnly Property DifficultyScore As Integer
        Get
            Dim difficulty As Double = 0.0
            Dim length As UInt32 = 0

            For Each composition In Compositions
                If composition.Difficulty = Difficulties.NotSet Then Continue For

                Dim compDiff = CDbl(composition.Difficulty) / CDbl(Difficulties.TooHard) * 100.0
                Dim compLen = If(composition.Length > TimeSpan.Zero, composition.Length.TotalMinutes, 20)
                difficulty += (compDiff * compLen)
                length += compLen
            Next

            If length = 0 Then Return 1

            Return Math.Max(1, Math.Round(difficulty / CDbl(length)))
        End Get
    End Property


    <Display(AutoGenerateField:=False)>
    <Xml.Serialization.XmlIgnore>
    ReadOnly Property Eras As Eras
        Get
            Dim _eras As New Eras
            For Each composition In Compositions
                If composition.Recommendation Is Nothing Then Continue For
                If _eras.ToList.Find(Function(x) x = composition.Recommendation.Era) Is Nothing Then
                    _eras.Add(composition.Recommendation.Era)
                End If
            Next

            Return _eras
        End Get
    End Property

    <Display(AutoGenerateField:=False)>
    <Xml.Serialization.XmlIgnore>
    ReadOnly Property Tags As Tags
        Get
            Dim _tags As New Tags
            For Each composition In Compositions
                If composition.Recommendation Is Nothing Then Continue For
                For Each tag In composition.Recommendation.Tags
                    If _tags.ToList.Find(Function(x) x.Name = tag.Name) Is Nothing Then
                        _tags.Add(tag)
                    End If
                Next
            Next

            Return _tags
        End Get
    End Property

    Private Sub OnCompositionsListChanged(sender As Object, e As ListChangedEventArgs) Handles _Compositions.ListChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Length)))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(DifficultyScore)))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Eras)))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Tags)))
    End Sub

End Class