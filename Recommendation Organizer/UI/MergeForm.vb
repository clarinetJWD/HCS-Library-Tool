Imports System.ComponentModel

Public Class MergeForm

    ReadOnly Property MergedItem As Recommendation = Nothing
    ReadOnly Property MergedAlias As ComposerAlias = Nothing

    Private _recommendations As IEnumerable(Of Recommendation)
    ReadOnly Property OriginalComposerAliases As ComposerAliases

    Sub Initialize(recommendations As IEnumerable(Of Recommendation), composerAliases As ComposerAliases, eras As Eras, tags As Tags)

        _recommendations = recommendations
        _OriginalComposerAliases = New ComposerAliases
        Dim titles As New List(Of String)
        Dim aliases As New ComposerAliases
        Dim eraList As New List(Of Era)
        Dim selectedEra As String = Nothing
        Dim tagList As New List(Of Tag)
        Dim selectedTags As New List(Of Tag)
        Dim selectedDifficulty As Difficulties = Difficulties.NotSet

        For Each rec In recommendations
            titles.Add(rec.Title.Trim)
            For Each altTitle In rec.AlternateTitleSpellings
                titles.Add(altTitle)
            Next

            Dim compAlias = composerAliases.FindComposer(rec)
            If rec.Composer <> Nothing Then
                If compAlias Is Nothing Then
                    compAlias = New ComposerAlias(rec.Composer.Trim, rec.AlternateComposerSpellings.Select(Function(x) x.Trim))
                Else
                    Dim alreadyAlias = OriginalComposerAliases.FindComposer(compAlias)
                    If alreadyAlias Is Nothing Then
                        OriginalComposerAliases.Add(compAlias)
                    End If
                End If
                aliases.Add(compAlias)
            End If

            If rec.Difficulty > selectedDifficulty Then
                selectedDifficulty = rec.Difficulty
            End If

            If rec.Era <> Nothing Then
                eraList.Add(rec.Era)
                selectedEra = rec.Era
            End If

            If rec.Tags IsNot Nothing Then
                For Each tagItem In rec.Tags
                    tagList.Add(tagItem)
                    selectedTags.Add(tagItem)
                Next
            End If

            If rec.Composer <> Nothing Then
                If compAlias Is Nothing Then
                    compAlias = New ComposerAlias(rec.Composer.Trim, rec.AlternateComposerSpellings.Select(Function(x) x.Trim))
                Else
                    Dim alreadyAlias = OriginalComposerAliases.FindComposer(compAlias)
                    If alreadyAlias Is Nothing Then
                        OriginalComposerAliases.Add(compAlias)
                    End If
                End If
                aliases.Add(compAlias)
            End If

            If rec.Length > TimeSpan.Zero Then
                TimeSpanEditLength.EditValue = rec.Length
            End If
        Next

        For Each era In eras
            eraList.Add(era)
        Next
        eraList = eraList.Distinct.ToList
        eraList.Sort()

        For Each tagItem In tags
            tagList.Add(tagItem)
        Next
        tagList = tagList.Distinct.ToList
        tagList.Sort()

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

        Me.ComboBoxEditDifficulty.Properties.AddEnum(GetType(Difficulties))
        Me.ComboBoxEditDifficulty.EditValue = selectedDifficulty

        Me.ComboBoxEditEras.Properties.Items.AddRange(eraList)
        If selectedEra <> Nothing Then
            Me.ComboBoxEditEras.SelectedIndex = eraList.IndexOf(selectedEra)
        Else
            Me.ComboBoxEditEras.SelectedIndex = 0
        End If

        For Each tagItem In tagList
            Me.CheckedComboBoxEditTags.Properties.Items.Add(tagItem)
        Next
        If selectedTags IsNot Nothing AndAlso selectedTags.Count > 0 Then
            For Each item As DevExpress.XtraEditors.Controls.CheckedListBoxItem In Me.CheckedComboBoxEditTags.Properties.Items
                If selectedTags.Contains(item.Value) Then
                    item.CheckState = CheckState.Checked
                End If
            Next
        End If

    End Sub

    Private Sub SimpleButtonSave_Click(sender As Object, e As EventArgs) Handles SimpleButtonSave.Click
        Dim primaryTitle As String = ComboBoxEditTitle.EditValue
        Dim primaryComposer As String = ComboBoxEditComposer.EditValue
        Dim difficulty As Difficulties = ComboBoxEditDifficulty.EditValue
        Dim era As String = ComboBoxEditEras.EditValue
        Dim tags As New Tags
        If CheckedComboBoxEditTags.EditValue IsNot Nothing Then
            For Each tagItem In CheckedComboBoxEditTags.EditValue
                tags.Add(tagItem)
            Next
        End If

        Dim primaryArranger As String = Nothing
        Dim isOwned As Boolean = False
        Dim length As TimeSpan = If(TimeSpanEditLength.EditValue, TimeSpan.Zero)

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
            .Owned = isOwned,
            .Length = length,
            .Era = era,
            .Tags = tags,
            .Difficulty = difficulty}

        Me.Close()
    End Sub
End Class