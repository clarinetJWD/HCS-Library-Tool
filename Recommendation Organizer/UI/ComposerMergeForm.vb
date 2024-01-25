Imports System.ComponentModel

Public Class ComposerMergeForm

    ReadOnly Property MergedItem As ComposerAlias = Nothing

    Private _composers As IEnumerable(Of ComposerAlias)

    Sub Initialize(composers As IEnumerable(Of ComposerAlias))

        _composers = composers

        Dim names As New List(Of String)

        For Each rec In composers
            names.Add(rec.PrimaryName.Trim)
            For Each altComp In rec.Aliases
                names.Add(altComp)
            Next
        Next

        names = names.Distinct.ToList

        Me.ComboBoxEditComposer.Properties.Items.AddRange(names)
        Me.ComboBoxEditComposer.SelectedIndex = 0
    End Sub

    Private Sub SimpleButtonSave_Click(sender As Object, e As EventArgs) Handles SimpleButtonSave.Click
        Dim primaryComposer As String = ComboBoxEditComposer.EditValue

        Dim alternateComposers As New BindingList(Of String)

        For Each name As String In Me.ComboBoxEditComposer.Properties.Items
            If name <> primaryComposer Then
                alternateComposers.Add(name)
            End If
        Next

        _MergedItem = New ComposerAlias() With {
            .PrimaryName = primaryComposer,
            .Aliases = alternateComposers}

        Me.Close()
    End Sub

End Class