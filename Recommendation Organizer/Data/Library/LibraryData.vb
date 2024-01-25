Imports System.ComponentModel

Public Class LibraryData : Implements ISupportHasChanges, INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property RecommendationList As RecommendationList
        Get
            Return _RecommendationList
        End Get
        Set(value As RecommendationList)
            If _RecommendationList IsNot value Then
                _RecommendationList = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RecommendationList)))
                HasChanges = True
            End If
        End Set
    End Property
    Private Sub OnRecommendationListChanged(sender As Object, e As ListChangedEventArgs) Handles _RecommendationList.ListChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RecommendationList)))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
    End Sub

    Private WithEvents _RecommendationList As New RecommendationList

    Public Property Composers As ComposerAliases
        Get
            Return _Composers
        End Get
        Set(value As ComposerAliases)
            If _Composers IsNot value Then
                _Composers = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Composers)))
                HasChanges = True
            End If
        End Set
    End Property
    Private Sub OnComposersListChanged(sender As Object, e As ListChangedEventArgs) Handles _Composers.ListChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Composers)))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
    End Sub
    Private WithEvents _Composers As New ComposerAliases

    Public Property Eras As Eras
        Get
            Return _Eras
        End Get
        Set(value As Eras)
            Dim newEraList = value.ToList
            newEraList.Sort(Function(x, y) x.Name.CompareTo(y.Name))
            Dim newEras As New Eras
            For Each era In newEraList
                newEras.Add(era)
            Next
            _Eras = newEras

            If Not ListsAreSame(newEraList, _Eras) Then
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Eras)))
                HasChanges = True
            End If
        End Set
    End Property
    Private Sub OnErasListChanged(sender As Object, e As ListChangedEventArgs) Handles _Eras.ListChanged
        If IsDataLoaded Then
            Select Case e.ListChangedType
                Case ListChangedType.ItemDeleted
                    For Each rec In RecommendationList
                        If rec.Era Is Nothing Then Continue For

                        Dim foundEra = _Eras.Find(rec.Era)
                        If foundEra Is Nothing Then
                            rec.Era = Nothing
                        End If
                    Next
                Case ListChangedType.ItemChanged
                    Dim modItem As Era = _Eras(e.NewIndex)

                    For Each rec In RecommendationList
                        If rec.Era Is Nothing Then Continue For

                        If rec.Era.Id = modItem.Id Then
                            rec.Era.Name = modItem.Name
                        End If
                    Next
            End Select
        End If

        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Eras)))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
    End Sub
    Private WithEvents _Eras As Eras = New Eras

    Public Property Tags As Tags
        Get
            Return _Tags
        End Get
        Set(value As Tags)
            Dim newTagsList = value.ToList
            newTagsList.Sort(Function(x, y) x.Name.CompareTo(y.Name))
            Dim newTags As New Tags
            For Each tag In newTagsList
                newTags.Add(tag)
            Next
            _Tags = newTags

            If Not ListsAreSame(newTags, _Tags) Then
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Tags)))
                HasChanges = True
            End If
        End Set
    End Property
    Private Sub OnTagsListChanged(sender As Object, e As ListChangedEventArgs) Handles _Tags.ListChanged
        If IsDataLoaded Then
            Select Case e.ListChangedType
                Case ListChangedType.ItemDeleted
                    For Each rec In RecommendationList

                        Dim tagsToDelete As New List(Of Tag)
                        For Each tag In rec.Tags
                            Dim foundTag = _Tags.Find(tag)
                            If foundTag Is Nothing Then
                                tagsToDelete.Add(tag)
                            End If
                        Next

                        For Each tagToDelete In tagsToDelete
                            rec.Tags.Remove(tagToDelete)
                        Next

                    Next
                Case ListChangedType.ItemChanged
                    Dim modItem As Tag = _Tags(e.NewIndex)

                    For Each rec In RecommendationList
                        For i As Integer = 0 To rec.Tags.Count - 1
                            If rec.Tags(i).Id = modItem.Id Then
                                rec.Tags(i).Name = modItem.Name
                            End If
                        Next
                    Next
            End Select
        End If

        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Tags)))
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
    End Sub
    Private WithEvents _Tags As Tags = New Tags

    Public Property LastChanged As Date
        Get
            Return _LastChanged
        End Get
        Set(value As Date)
            If value <> _LastChanged Then
                _LastChanged = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LastChanged)))
                HasChanges = True
            End If
        End Set
    End Property
    Private _LastChanged As Date = Date.MinValue

    <Xml.Serialization.XmlIgnore>
    Public Property IsDataLoaded As Boolean
        Get
            Return _IsDataLoaded
        End Get
        Set(value As Boolean)
            If value <> _IsDataLoaded Then
                If value Then
                    SyncTagsEras()
                End If

                _IsDataLoaded = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsDataLoaded)))
            End If
        End Set
    End Property
    Private _IsDataLoaded As Boolean = False


    Public Property HasChanges As Boolean Implements ISupportHasChanges.HasChanges
        Get
            Return _HasChanges OrElse
                RecommendationList.HasChanges OrElse
                Composers.HasChanges OrElse
                Eras.HasChanges OrElse
                Tags.HasChanges

        End Get
        Set(value As Boolean)
            _HasChanges = value

            If Not value Then
                RecommendationList.HasChanges = False
                Composers.HasChanges = False
                Eras.HasChanges = False
                Tags.HasChanges = False
            End If

            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HasChanges)))
        End Set
    End Property
    Private _HasChanges As Boolean = False

    Private Sub SyncTagsEras()
        For Each rec In RecommendationList
            Dim foundEra = Eras.Find(rec.Era)
            If foundEra IsNot Nothing Then
                rec.Era = foundEra
            End If

            For i As Integer = 0 To rec.Tags.Count - 1
                Dim foundTag = Tags.Find(rec.Tags(i))
                If foundTag IsNot Nothing Then
                    rec.Tags(i) = foundTag
                End If
            Next
        Next
    End Sub

End Class
