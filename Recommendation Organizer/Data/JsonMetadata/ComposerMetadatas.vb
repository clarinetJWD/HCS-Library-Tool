Imports System.ComponentModel

Class ComposerMetadatas : Inherits BindingList(Of ComposerMetadata)

    Function GetMetadata(composer As ComposerAlias) As ComposerMetadata
        Dim foundMetadata = Find(composer)
        If foundMetadata IsNot Nothing Then Return foundMetadata

        foundMetadata = New ComposerMetadata(composer)
        foundMetadata.Load()

        Return foundMetadata
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
                    If DevExpress.XtraEditors.XtraMessageBox.Show($"Is {jsonComposer.complete_name} a match for {Composer.PrimaryName}?", "Metadata Match?", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                        _Metadata = jsonComposer
                        Return True
                    End If
                Next
            End If

        Next

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
