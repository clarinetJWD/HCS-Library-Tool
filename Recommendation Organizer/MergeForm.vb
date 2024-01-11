Imports System.ComponentModel

Public Class MergeForm

    ReadOnly Property MergedItem As Recommendation = Nothing
    ReadOnly Property MergedAlias As ComposerAlias = Nothing

    Private _recommendations As IEnumerable(Of Recommendation)
    ReadOnly Property OriginalComposerAliases As ComposerAliases

    Sub Initialize(recommendations As IEnumerable(Of Recommendation), composerAliases As ComposerAliases)

        _recommendations = recommendations
        _OriginalComposerAliases = New ComposerAliases
        Dim titles As New List(Of String)
        'Dim names As New List(Of String)
        Dim aliases As New ComposerAliases

        For Each rec In recommendations
            titles.Add(rec.Title.Trim)
            For Each altTitle In rec.AlternateTitleSpellings
                titles.Add(altTitle)
            Next
            'names.Add(rec.Composer.Trim)
            'For Each altComp In rec.AlternateComposerSpellings
            'names.Add(altComp)
            'Next
            Dim compAlias = composerAliases.FindComposer(rec)
            If compAlias Is Nothing Then
                compAlias = New ComposerAlias(rec.Composer.Trim, rec.AlternateComposerSpellings.Select(Function(x) x.Trim))
            Else
                Dim alreadyAlias = OriginalComposerAliases.FindComposer(compAlias)
                If alreadyAlias Is Nothing Then
                    OriginalComposerAliases.Add(compAlias)
                End If
            End If
            aliases.Add(compAlias)
        Next

        titles = titles.Distinct.ToList
        Dim names As New List(Of String)
        For Each compAlias In aliases
            names.Add(compAlias.PrimaryName)
            names.AddRange(compAlias.Aliases)
        Next
        names = names.Distinct.ToList

        Me.ComboBoxEditTitle.Properties.Items.AddRange(titles)
        Me.ComboBoxEditTitle.SelectedIndex = 0
        Me.ComboBoxEditComposer.Properties.Items.AddRange(names)
        Me.ComboBoxEditComposer.SelectedIndex = 0
    End Sub

    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        Dim primaryTitle As String = ComboBoxEditTitle.EditValue
        Dim primaryComposer As String = ComboBoxEditComposer.EditValue
        Dim primaryArranger As String = Nothing
        Dim isOwned As Boolean = False

        'Dim alternateComposers As New BindingList(Of String)
        Dim alternateTitles As New BindingList(Of String)
        Dim allYears As New List(Of Integer)
        Dim recommendedBys As New BindingList(Of MusicianRecommendation)
        Dim mergedAlias As New ComposerAlias(primaryComposer, New List(Of String))

        For Each title As String In Me.ComboBoxEditTitle.Properties.Items
            If title <> primaryTitle Then
                alternateTitles.Add(title)
            End If
        Next

        For Each name As String In Me.ComboBoxEditComposer.Properties.Items
            If name <> primaryComposer Then
                mergedAlias.Aliases.Add(name)
                'alternateComposers.Add(name)
            End If
        Next

        Me._MergedAlias = mergedAlias

        For Each rec In _recommendations
            allYears.AddRange(rec.PerformedYears)
            For Each recBy In rec.RecommendedBy
                recommendedBys.Add(recBy)
            Next
            If rec.Arranger IsNot Nothing AndAlso rec.Arranger.Trim <> Nothing Then
                primaryArranger = rec.Arranger.Trim
            End If
            If rec.Owned Then isOwned = True
        Next
        allYears = allYears.Distinct.ToList
        allYears.Sort(Function(x, y) y.CompareTo(x))
        Dim allYearsBinding As New BindingList(Of Integer)
        For Each yearVal In allYears
            allYearsBinding.Add(yearVal)
        Next

        _MergedItem = New Recommendation() With {
            .Composer = ComboBoxEditComposer.EditValue,
            .Title = ComboBoxEditTitle.EditValue,
            .AlternateComposerSpellings = mergedAlias.Aliases,
            .AlternateTitleSpellings = alternateTitles,
            .PerformedYears = allYearsBinding,
            .RecommendedBy = recommendedBys,
            .Arranger = primaryArranger,
            .Owned = isOwned}

        Me.Close()
    End Sub
End Class