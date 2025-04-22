Imports System.ComponentModel

Public Class MatchSelectForm

    ReadOnly Property MatchResult As String = Nothing
    ReadOnly Property Result As MatchResults = MatchResults.Canceled

    Enum MatchResults
        SelectionMade
        NoMatch
        Canceled
    End Enum
    Sub Initialize(stringToMatch As String, columnCaption As String, possibleStrings As IEnumerable(Of String))
        Me.IconOptions.SvgImage = Global.HcsLibraryTool.My.Resources.Resources.hcs_icon

        LabelControl2.Text = stringToMatch

        Dim dtMatches As New DataTable()
        dtMatches.Columns.Add("StringMatch")
        For Each possibleMatch In possibleStrings
            dtMatches.Rows.Add(possibleMatch)
        Next
        Me.GridControl1.DataSource = dtMatches
        Me.GridColumn1.Caption = columnCaption

    End Sub

    Private Sub SimpleButtonSave_Click(sender As Object, e As EventArgs) Handles SimpleButtonSave.Click
        If GridView1.FocusedRowHandle >= 0 Then
            Dim focusedRow = GridView1.GetFocusedRow.Row.ItemArray(0)
            Me._MatchResult = focusedRow
            Me._Result = MatchResults.SelectionMade
            Me.Close()
        Else
            ' Do nothing
        End If
    End Sub

    Private Sub SimpleButtonNoMatch_Click(sender As Object, e As EventArgs) Handles SimpleButtonNoMatch.Click
        Me._MatchResult = Nothing
        Me._Result = MatchResults.NoMatch
        Me.Close()
    End Sub
End Class