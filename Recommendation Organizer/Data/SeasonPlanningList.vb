Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

Public Class SeasonPlanningList : Inherits BindingList(Of SeasonItem)

    Friend Function FindSeasonItem(mRec As IComposerTitleKey) As SeasonItem
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

Public Class SeasonItem : Implements INotifyPropertyChanged, IComposerArrangerTitleKey, IComposerAlternates, ISupportHasChanges, IMetadata

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property Composer As String Implements IComposer.Composer
        Get
            Return If(Recommendation?.Composer, _Composer)
        End Get
        Set(value As String)
            If Recommendation IsNot Nothing Then Recommendation.Composer = value
            _Composer = value
        End Set
    End Property
    Private _Composer As String

    Public Property Arranger As String Implements IArranger.Arranger
        Get
            Return If(Recommendation?.Arranger, _Arranger)
        End Get
        Set(value As String)
            If Recommendation IsNot Nothing Then Recommendation.Arranger = value
            _Arranger = value
        End Set
    End Property
    Private _Arranger As String

    Public Property Title As String Implements ITitle.Title
        Get
            Return If(Recommendation?.Title, _Title)
        End Get
        Set(value As String)
            If Recommendation IsNot Nothing Then Recommendation.Title = value
            _Title = value
        End Set
    End Property
    Private _Title As String

    <Xml.Serialization.XmlIgnore>
    Public Property Length As TimeSpan Implements IMetadata.Length
        Get
            Return Recommendation?.Length
        End Get
        Set(value As TimeSpan)
            If Recommendation IsNot Nothing Then Recommendation.Length = value
        End Set
    End Property

    <Xml.Serialization.XmlIgnore>
    Public Property Difficulty As Difficulties Implements IMetadata.Difficulty
        Get
            Return Recommendation?.Difficulty
        End Get
        Set(value As Difficulties)
            If Recommendation IsNot Nothing Then Recommendation.Difficulty = value
        End Set
    End Property

    <Xml.Serialization.XmlIgnore>
    Public Property Era As Era Implements IMetadata.Era
        Get
            Return Recommendation?.Era
        End Get
        Set(value As Era)
            If Recommendation IsNot Nothing Then Recommendation.Era = value
        End Set
    End Property

    <Xml.Serialization.XmlIgnore>
    Public Property Tags As Tags Implements IMetadata.Tags
        Get
            Return Recommendation?.Tags
        End Get
        Set(value As Tags)
            If Recommendation IsNot Nothing Then Recommendation.Tags = value
        End Set
    End Property

    <Xml.Serialization.XmlIgnore>
    Public Property AlternateComposerSpellings As BindingList(Of String) Implements IComposerAlternates.AlternateComposerSpellings
        Get
            Return Recommendation?.AlternateComposerSpellings
        End Get
        Set(value As BindingList(Of String))
            If Recommendation IsNot Nothing Then Recommendation.AlternateComposerSpellings = value
        End Set
    End Property

    <Xml.Serialization.XmlIgnore>
    Public ReadOnly Property Recommendation As Recommendation
        Get
            Return _Recommendation
        End Get
    End Property
    Friend Sub SetRecommendation(recommendation As Recommendation, raiseEvents As Boolean)
        If _Recommendation Is Nothing OrElse recommendation IsNot _Recommendation Then
            _Recommendation = recommendation
            If raiseEvents Then _Recommendation.RaiseAllPropertyChangedEvents()
        End If
    End Sub

    <Display(AutoGenerateField:=False)>
    <Editable(False)>
    Public Property RecommendationKey As String Implements IKey.Key
        Get
            Return If(_Recommendation?.Key, _RecommendationKey)
        End Get
        Set(value As String)
            _RecommendationKey = value
        End Set
    End Property
    Private _RecommendationKey As String

    <Editable(False)>
    <Display(AutoGenerateField:=False)>
    Public Property HasChanges As Boolean Implements ISupportHasChanges.HasChanges
        Get
            Return _HasChanges
        End Get
        Set(value As Boolean)
            _HasChanges = False
            If Not value Then
                If _Recommendation IsNot Nothing Then
                    _Recommendation.HasChanges = False
                End If
            End If
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
        End Set
    End Property
    Private _HasChanges As Boolean = True

    Private WithEvents _Recommendation As Recommendation = Nothing

    Sub New()

    End Sub

    Sub New(recommendation As Recommendation)
        SetRecommendation(recommendation, False)
    End Sub

    Private Sub OnBasePropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _Recommendation.PropertyChanged
        HasChanges = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(e.PropertyName))
    End Sub

    Friend Function IsMatch(mRec As IComposerTitleKey) As Boolean
        Return Recommendation.IsMatch(mRec)
    End Function


    Public Shared Widening Operator CType(recommendation As Recommendation) As SeasonItem
        Return New SeasonItem(recommendation)
    End Operator


    Public Shared Widening Operator CType(seasonItem As SeasonItem) As Recommendation
        Return seasonItem.Recommendation
    End Operator

End Class