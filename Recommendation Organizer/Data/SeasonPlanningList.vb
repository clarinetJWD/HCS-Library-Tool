Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

Public Class SeasonPlanningList : Inherits BindingList(Of SeasonItem)

    Friend Function FindSeasonItem(mRec As IComposerTitle) As SeasonItem
        For Each recommendation In Me
            If recommendation.IsMatch(mRec) Then
                Return recommendation
            End If
        Next
        Return Nothing
    End Function

    Public Shared Widening Operator CType(recList As RecommendationList) As SeasonPlanningList
        Dim spl As New SeasonPlanningList
        For Each recItem In recList
            spl.Add(recItem)
        Next
        Return spl
    End Operator
End Class

Public Class SeasonItem : Implements INotifyPropertyChanged, IComposerArrangerTitle, IComposerAlternates, ISupportHasChanges, IMetadata

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property Composer As String Implements IComposer.Composer
        Get
            Return Recommendation?.Composer
        End Get
        Set(value As String)
            If Recommendation IsNot Nothing Then Recommendation.Composer = value
        End Set
    End Property

    Public Property Arranger As String Implements IArranger.Arranger
        Get
            Return Recommendation?.Arranger
        End Get
        Set(value As String)
            If Recommendation IsNot Nothing Then Recommendation.Arranger = value
        End Set
    End Property

    Public Property Title As String Implements ITitle.Title
        Get
            Return Recommendation?.Title
        End Get
        Set(value As String)
            If Recommendation IsNot Nothing Then Recommendation.Title = value
        End Set
    End Property

    Public Property Length As TimeSpan Implements IMetadata.Length
        Get
            Return Recommendation?.Length
        End Get
        Set(value As TimeSpan)
            If Recommendation IsNot Nothing Then Recommendation.Length = value
        End Set
    End Property

    Public Property Difficulty As Difficulties Implements IMetadata.Difficulty
        Get
            Return Recommendation?.Difficulty
        End Get
        Set(value As Difficulties)
            If Recommendation IsNot Nothing Then Recommendation.Difficulty = value
        End Set
    End Property

    Public Property Era As Era Implements IMetadata.Era
        Get
            Return Recommendation?.Era
        End Get
        Set(value As Era)
            If Recommendation IsNot Nothing Then Recommendation.Era = value
        End Set
    End Property

    Public Property Tags As Tags Implements IMetadata.Tags
        Get
            Return Recommendation?.Tags
        End Get
        Set(value As Tags)
            If Recommendation IsNot Nothing Then Recommendation.Tags = value
        End Set
    End Property

    Public Property AlternateComposerSpellings As BindingList(Of String) Implements IComposerAlternates.AlternateComposerSpellings
        Get
            Return Recommendation?.AlternateComposerSpellings
        End Get
        Set(value As BindingList(Of String))
            If Recommendation IsNot Nothing Then Recommendation.AlternateComposerSpellings = value
        End Set
    End Property

    Public Property Recommendation As Recommendation
        Get
            Return _Recommendation
        End Get
        Set(value As Recommendation)
            _Recommendation = value
            _Recommendation.RaiseAllPropertyChangedEvents()
        End Set
    End Property

    <Editable(False)>
    <Display(AutoGenerateField:=False)>
    Public Property HasChanges As Boolean Implements ISupportHasChanges.HasChanges
        Get
            Return _HasChanges
        End Get
        Set(value As Boolean)
            _HasChanges = False
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
        End Set
    End Property
    Private _HasChanges As Boolean = True

    Private WithEvents _Recommendation As Recommendation = Nothing

    Sub New()
        Me.Recommendation = New Recommendation
    End Sub

    Sub New(recommendation As Recommendation)
        Me.Recommendation = recommendation
    End Sub

    Private Sub OnBasePropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _Recommendation.PropertyChanged
        HasChanges = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(e.PropertyName))
    End Sub

    Friend Function IsMatch(mRec As IComposerTitle) As Boolean
        Return Recommendation.IsMatch(mRec)
    End Function


    Public Shared Widening Operator CType(recommendation As Recommendation) As SeasonItem
        Return New SeasonItem(recommendation)
    End Operator


    Public Shared Widening Operator CType(seasonItem As SeasonItem) As Recommendation
        Return seasonItem.Recommendation
    End Operator

End Class