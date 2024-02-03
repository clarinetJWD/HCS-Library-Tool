Imports System.ComponentModel

Public Class PasscodeForm

    Private _IsValidated As Boolean? = Nothing

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.TextEditPasscode.Text = My.Settings.Passcode
    End Sub

    Private Sub SimpleButtonSave_Click(sender As Object, e As EventArgs) Handles SimpleButtonSave.Click
        Dim passcode As String = TextEditPasscode.EditValue
        My.Settings.Passcode = passcode
        My.Settings.Save()

        If Ftp.ValidatePasscode(passcode) Then
            _IsValidated = True
            Me.Close()
        Else
            TextEditPasscode.ErrorText = "The passcode was incorrect. Please try again."
            _IsValidated = False
        End If
    End Sub

    Private Sub OnFormClosingHandler(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If _IsValidated IsNot Nothing AndAlso _IsValidated.Value = False Then
            e.Cancel = True
            _IsValidated = Nothing
        End If
    End Sub

    Shadows Function ShowDialog() As Boolean
        MyBase.ShowDialog()
        If _IsValidated Is Nothing Then
            Return Ftp.ValidatePasscode(My.Settings.Passcode)
        End If
        Return _IsValidated.Value
    End Function

End Class