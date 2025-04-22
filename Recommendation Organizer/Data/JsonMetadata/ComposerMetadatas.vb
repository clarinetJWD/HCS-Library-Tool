Imports System.ComponentModel

Class ComposerMetadatas : Inherits BindingList(Of ComposerMetadata)

    Function GetMetadata(composer As ComposerAlias) As ComposerMetadata
        Dim foundMetadata = Find(composer)
        If foundMetadata IsNot Nothing Then Return foundMetadata

        foundMetadata = New ComposerMetadata(composer)
        If foundMetadata.Load() Then
            WorkMetadata.ClearCacheFor(foundMetadata)

            Return foundMetadata
        End If

        Return Nothing
    End Function

    Function Find(composer As IComposer) As ComposerMetadata
        Return Me.ToList.Find(Function(x) x.Composer.Equals(composer))
    End Function

End Class

Class ComposerMetadata
    Public Sub New(composer As ComposerAlias)
        _Composer = composer
    End Sub

    ReadOnly Property Composer As ComposerAlias
    ReadOnly Property Metadata As ComposerSearchJsonResultComposer

    Function Load() As Boolean
        'https://api.openopus.org/composer/list/search/richard%20strauss.json

        Dim possibleMatches As New List(Of ComposerSearchJsonResultComposer)

        For Each searchName As String In GetSearchNames()
            Dim webClient As New Net.WebClient()
            Dim json = webClient.DownloadString(New Uri($"https://api.openopus.org/composer/list/search/{searchName}.json"))

            Dim deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject(Of ComposerSearchJsonResult)(json)

            If deserialized.status.success Then

                For Each jsonComposer In deserialized.composers
                    If Composer.Equals(jsonComposer.complete_name) Then
                        _Metadata = jsonComposer
                        Return True
                    End If
                Next

                For Each jsonComposer In deserialized.composers
                    If Not possibleMatches.Any(Function(x) x.id = jsonComposer.id) Then
                        possibleMatches.Add(jsonComposer)
                    End If
                Next
            End If

        Next

        Dim scores As New List(Of (ComposerMetadata As ComposerSearchJsonResultComposer, Score As Double))
        For Each possibleMatch In possibleMatches
            Dim score = Extensions.StringExtensions.GetSimilarityScore(Composer.PrimaryName, possibleMatch.complete_name, True)
            scores.Add((possibleMatch, score.OverallScore))
        Next

        scores.Sort(Function(x, y) y.Score.CompareTo(x.Score))
        If scores.Count > 5 Then
            scores = scores.Where(Function(x) x.Score > 0).ToList
        End If
        If scores.Count = 0 Then Return False

        Dim matchForm = New MatchSelectForm()
        matchForm.Initialize(Composer.PrimaryName, "Name", scores.Select(Function(x) x.ComposerMetadata.complete_name).ToList)
        matchForm.ShowDialog()
        Select Case matchForm.Result
            Case MatchSelectForm.MatchResults.SelectionMade
                Dim foundScore = scores.Find(Function(x) x.ComposerMetadata.complete_name = matchForm.MatchResult)
                If foundScore.ComposerMetadata IsNot Nothing Then
                    _Metadata = foundScore.ComposerMetadata
                    Return True
                End If
            Case MatchSelectForm.MatchResults.NoMatch
                    ' Do nothing
            Case MatchSelectForm.MatchResults.Canceled
                ' Cancel
                Throw New Exception("Cancellation")
        End Select

        Return False

    End Function

    Private Function GetSearchNames() As IEnumerable(Of String)
        Dim sourceNames As New List(Of String)
        If Composer.PrimaryName <> Nothing Then
            sourceNames.Add(FormatNameWithNoCommas(Composer.PrimaryName).ToLower)
        End If
        For Each composerAlias In Composer.Aliases
            sourceNames.Add(FormatNameWithNoCommas(composerAlias).ToLower)
        Next

        Dim searchNames As New List(Of String)
        For Each sourceName In sourceNames
            Dim thisName = sourceName
            While thisName <> Nothing
                searchNames.Add(thisName.Replace(".", " ").MakeAlphaNumeric({"-"c, " "c}))
                Dim thisNameTokens = thisName.Split(" ").ToList
                thisNameTokens.RemoveAt(0)
                thisName = String.Join(" ", thisNameTokens)
            End While
        Next

        searchNames = searchNames.Distinct().ToList

        Return searchNames
    End Function

End Class
