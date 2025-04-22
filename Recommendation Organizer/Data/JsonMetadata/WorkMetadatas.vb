Imports System.ComponentModel
Imports System.Net

Class WorkMetadatas : Inherits BindingList(Of WorkMetadata)

    Function GetMetadata(recommendation As Recommendation, composer As IComposerMetadata) As WorkMetadata
        Dim foundMetadata = Find(recommendation)
        If foundMetadata IsNot Nothing Then Return foundMetadata

        foundMetadata = New WorkMetadata(recommendation, composer)
        If foundMetadata.Load() Then
            Return foundMetadata
        End If

        Return Nothing
    End Function

    Function Find(recommendation As IComposerTitle) As WorkMetadata
        Return Me.ToList.Find(Function(x) x.Recommendation.Equals(recommendation))
    End Function

End Class

Class WorkMetadata
    Public Sub New(recommendation As Recommendation, composer As ComposerAlias)
        _Recommendation = recommendation
        _Composer = composer
    End Sub

    ReadOnly Property Recommendation As Recommendation
    ReadOnly Property Composer As ComposerAlias
    ReadOnly Property Metadata As WorkSearchJsonResultWork

    Shared ReadOnly Property ComposerWorks As New Dictionary(Of Integer, WorkSearchJsonResult)



    Function Load() As Boolean
        'https://api.openopus.org/work/list/composer/{ID}/genre/all/search/the%20title.json
        '/work/list/composer/129/genre/all.json

        If Not ComposerWorks.ContainsKey(Composer.MetadataId) Then
            Dim webClient As New Net.WebClient()
            Dim json = webClient.DownloadString(New Uri($"https://api.openopus.org/work/list/composer/{Composer.MetadataId}/genre/all.json"))
            Dim deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject(Of WorkSearchJsonResult)(json)
            If deserialized.status.success Then
                ComposerWorks(Composer.MetadataId) = deserialized
            End If
        End If

        If ComposerWorks.ContainsKey(Composer.MetadataId) Then
            For Each jsonWork In ComposerWorks(Composer.MetadataId).works
                If Recommendation.HasTitle(jsonWork.title) Then
                    _Metadata = jsonWork
                    Return True
                End If
            Next

            Dim scores As New List(Of (WorkMetadata As WorkSearchJsonResultWork, Score As Double))

            For Each jsonWork In ComposerWorks(Composer.MetadataId).works
                Dim score = Extensions.StringExtensions.GetSimilarityScore(Recommendation.Title, jsonWork.title, True)
                scores.Add((jsonWork, score.OverallScore))
            Next

            scores.Sort(Function(x, y) y.Score.CompareTo(x.Score))
            If scores.Count > 5 Then
                scores = scores.Where(Function(x) x.Score > 0).ToList
            End If
            If scores.Count = 0 Then Return False

            Dim matchForm = New MatchSelectForm()
            matchForm.Initialize(Recommendation.Title, "Title", scores.Select(Function(x) x.WorkMetadata.title).ToList)
            matchForm.ShowDialog()
            Select Case matchForm.Result
                Case MatchSelectForm.MatchResults.SelectionMade
                    Dim foundScore = scores.Find(Function(x) x.WorkMetadata.title = matchForm.MatchResult)
                    If foundScore.WorkMetadata IsNot Nothing Then
                        _Metadata = foundScore.WorkMetadata
                        Return True
                    End If
                Case MatchSelectForm.MatchResults.NoMatch
                    ' Do nothing
                Case MatchSelectForm.MatchResults.Canceled
                    ' Cancel
                    Throw New Exception("Cancellation")
            End Select

        End If

        Return False

    End Function

    Friend Shared Sub ClearCacheFor(foundMetadata As ComposerMetadata)
        If ComposerWorks.ContainsKey(foundMetadata.Metadata.id) Then
            ComposerWorks.Remove(foundMetadata.Metadata.id)
        End If
    End Sub

End Class
