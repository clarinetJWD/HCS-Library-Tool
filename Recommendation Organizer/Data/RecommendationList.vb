Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations

Public Class RecommendationList : Inherits BindingList(Of Recommendation) : Implements ISupportHasChanges, INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property HasChanges As Boolean Implements ISupportHasChanges.HasChanges
        Get
            For Each rec In Me
                If rec.HasChanges Then Return True
            Next
            Return False
        End Get
        Set(value As Boolean)
            For Each rec In Me
                rec.HasChanges = value
            Next
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
        End Set
    End Property

    Friend Function FindRecommendation(mRec As IComposerTitle) As Recommendation
        For Each recommendation In Me
            If recommendation.IsMatch(mRec) Then
                Return recommendation
            End If
        Next
        Return Nothing
    End Function

    Friend Function FindOrCreateRecommendation(mRec As HoldingEntry, ByRef foundRecommendation As Recommendation) As Boolean
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

    Friend Function FindOrCreateRecommendation(mRec As RepertoireEntry, ByRef foundRecommendation As Recommendation) As Boolean
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

    Friend Function FindOrCreateRecommendation(mRec As MusicianRecommendation, ByRef foundRecommendation As Recommendation) As Boolean
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

Public Class Recommendation : Implements INotifyPropertyChanged, IComposerArrangerTitle, IComposerAlternates, IMetadata, ISupportHasChanges

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    <Editable(False)>
    Property Composer As String Implements IComposer.Composer
        Get
            Return _Composer
        End Get
        Set(value As String)
            If _Composer <> value Then
                HasChanges = True
                _Composer = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Composer)))
            End If
        End Set
    End Property
    Private _Composer As String

    <Editable(False)>
    Property Arranger As String Implements IArranger.Arranger
        Get
            Return _Arranger
        End Get
        Set(value As String)
            If _Arranger <> value Then
                HasChanges = True
                _Arranger = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Arranger)))
            End If
        End Set
    End Property
    Private _Arranger As String

    <Editable(False)>
    Property Title As String Implements ITitle.Title
        Get
            Return _Title
        End Get
        Set(value As String)
            If _Title <> value Then
                HasChanges = True
                _Title = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Title)))
            End If
        End Set
    End Property
    Private _Title As String

    <Xml.Serialization.XmlIgnore>
    Property Length As TimeSpan Implements IMetadata.Length
        Get
            Return _Length
        End Get
        Set(value As TimeSpan)
            If _Length <> value Then
                HasChanges = True
                _Length = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Length)))
            End If
        End Set
    End Property
    Private _Length As TimeSpan = TimeSpan.Zero

    Property Difficulty As Difficulties Implements IMetadata.Difficulty
        Get
            Return _Difficulty
        End Get
        Set(value As Difficulties)
            If _Difficulty <> value Then
                HasChanges = True
                _Difficulty = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Difficulty)))
            End If
        End Set
    End Property
    Private _Difficulty As Difficulties = Difficulties.NotSet

    <TypeConverter(GetType(ErasTypeConverter))>
    Property Era As Era Implements IMetadata.Era
        Get
            If _Era IsNot Nothing AndAlso _Era.Name = Nothing Then Return Nothing
            Return _Era
        End Get
        Set(value As Era)
            If _Era <> value Then
                HasChanges = True
                If value Is Nothing OrElse value.Name = Nothing Then
                    _Era = Nothing
                Else
                    _Era = value
                End If
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Era)))
            End If
        End Set
    End Property
    Private Sub OnEraPropertyChangedHandler(sender As Object, e As PropertyChangedEventArgs) Handles _Era.PropertyChanged
        If e.PropertyName = NameOf(Era.Name) Then
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(e.PropertyName))
        End If
    End Sub
    Private WithEvents _Era As Era = Nothing

    <TypeConverter(GetType(TagsTypeConverter))>
    <Xml.Serialization.XmlIgnore>
    <Display(AutoGenerateField:=False)>
    Property Tags As Tags Implements IMetadata.Tags
        Get
            Return _Tags
        End Get
        Set(value As Tags)
            If _Tags IsNot value Then
                HasChanges = True
                _Tags = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Tags)))
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TagsString)))
            End If
        End Set
    End Property
    Private WithEvents _Tags As Tags = New Tags


    <TypeConverter(GetType(TagsTypeConverter))>
    <Xml.Serialization.XmlElement(ElementName:="Tags")>
    <Display(Name:="Tags")>
    Property TagsString As String
        Get
            Return String.Join(", ", Tags)
        End Get
        Set(value As String)
            Dim newTags As New Tags
            For Each tagToken In value.Split(",")
                newTags.Add(New Tag(tagToken.Trim))
            Next

            Tags = newTags
        End Set
    End Property

    Private Sub OnTagsListChanged(sender As Object, e As ListChangedEventArgs) Handles _Tags.ListChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Tags)))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TagsString)))

        HasChanges = True
    End Sub

    <Editable(False)>
    <Browsable(False)>
    <EditorBrowsable(EditorBrowsableState.Never)>
    <Xml.Serialization.XmlElement(DataType:="duration", ElementName:="Length")>
    <Display(AutoGenerateField:=False)>
    Property LengthString As String
        Get
            Return _Length.ToString
        End Get
        Set(value As String)
            Dim newLength As TimeSpan
            If TimeSpan.TryParse(value, newLength) Then
                Length = newLength
            End If
        End Set
    End Property

    <Editable(False)>
    Property Owned As Boolean
        Get
            Return _Owned
        End Get
        Set(value As Boolean)
            If _Owned <> value Then
                HasChanges = True
                _Owned = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Owned)))
            End If
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


    <Editable(False)>
    Property PerformedYears As BindingList(Of Integer)
        Get
            Return _PerformedYears
        End Get
        Set(value As BindingList(Of Integer))
            If _PerformedYears IsNot value Then
                HasChanges = True
                _PerformedYears = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(PerformedYears)))
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LastPerformed)))
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimesPerformed)))
            End If
        End Set
    End Property
    Private WithEvents _PerformedYears As New BindingList(Of Integer)
    Private Sub OnYearsListChanged(sender As Object, e As ListChangedEventArgs) Handles _PerformedYears.ListChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LastPerformed)))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimesPerformed)))
        HasChanges = True
    End Sub

    <Editable(False)>
    Property AlternateComposerSpellings As BindingList(Of String) Implements IComposerAlternates.AlternateComposerSpellings
        Get
            Return _AlternateComposerSpellings
        End Get
        Set(value As BindingList(Of String))
            If _AlternateComposerSpellings IsNot value Then
                HasChanges = True
                _AlternateComposerSpellings = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(AlternateComposerSpellings)))
            End If
        End Set
    End Property
    Private WithEvents _AlternateComposerSpellings As New BindingList(Of String)
    Private Sub OnAlternateComposerSpellingsListChanged(sender As Object, e As ListChangedEventArgs) Handles _AlternateComposerSpellings.ListChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(AlternateComposerSpellings)))
        HasChanges = True
    End Sub


    <Editable(False)>
    Property AlternateTitleSpellings As BindingList(Of String)
        Get
            Return _AlternateTitleSpellings
        End Get
        Set(value As BindingList(Of String))
            If _AlternateTitleSpellings IsNot value Then
                HasChanges = True
                _AlternateTitleSpellings = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(AlternateTitleSpellings)))
            End If
        End Set
    End Property
    Private WithEvents _AlternateTitleSpellings As New BindingList(Of String)
    Private Sub OnAlternateTitleSpellingsListChanged(sender As Object, e As ListChangedEventArgs) Handles _AlternateTitleSpellings.ListChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(AlternateTitleSpellings)))
        HasChanges = True
    End Sub

    <Editable(False)>
    Property RecommendedBy As BindingList(Of MusicianRecommendation)
        Get
            Return _RecommendedBy
        End Get
        Set(value As BindingList(Of MusicianRecommendation))
            If _RecommendedBy IsNot value Then
                HasChanges = True
                _RecommendedBy = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RecommendedBy)))
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimesRecommended)))
            End If
        End Set
    End Property
    Private WithEvents _RecommendedBy As New BindingList(Of MusicianRecommendation)
    Private Sub OnRecommendedByListChanged(sender As Object, e As ListChangedEventArgs) Handles _RecommendedBy.ListChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TimesRecommended)))
        HasChanges = True
    End Sub

    <Editable(False)>
    <Display(AutoGenerateField:=False)>
    Public Property HasChanges As Boolean Implements ISupportHasChanges.HasChanges
        Get
            Return _HasChanges
        End Get
        Set(value As Boolean)
            _HasChanges = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
        End Set
    End Property
    Private _HasChanges As Boolean = True

    Friend Function IsMatch(mRec As IComposerTitle) As Boolean
        Dim composerNames As New List(Of String) From {If(Me.Composer = Nothing, String.Empty, Me.Composer.ToUpper.Trim)}
        composerNames.AddRange(Me.AlternateComposerSpellings.Select(Function(x) x.ToUpper.Trim))
        If Me.Arranger <> Nothing Then
            composerNames.Add(Me.Arranger.ToUpper.Trim)
        End If

        Dim titles As New List(Of String) From {If(Me.Title = Nothing, String.Empty, Me.Title.ToUpper.Trim)}
        titles.AddRange(Me.AlternateTitleSpellings.Select(Function(x) x.ToUpper.Trim))

        Dim recComposerName = If(mRec.Composer = Nothing, String.Empty, mRec.Composer.ToUpper.Trim)
        Dim recArrName = String.Empty
        If TypeOf mRec Is IComposerArrangerTitle Then
            recArrName = If(DirectCast(mRec, IComposerArrangerTitle).Arranger = Nothing, String.Empty, DirectCast(mRec, IComposerArrangerTitle).Arranger.ToUpper.Trim)
        End If
        Dim recTitle = If(mRec.Title = Nothing, String.Empty, mRec.Title.ToUpper.Trim)

        If composerNames.Contains(recComposerName) AndAlso titles.Contains(recTitle) AndAlso (recArrName = Nothing OrElse composerNames.Contains(recArrName)) Then
            Return True
        End If

        Return False
    End Function

    Public Overrides Function ToString() As String
        Return $"{Composer}: {Title}"
    End Function

    Friend Sub RaiseAllPropertyChangedEvents()
        For Each propInfo In GetType(Recommendation).GetProperties
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(propInfo.Name)))
        Next
    End Sub
End Class
