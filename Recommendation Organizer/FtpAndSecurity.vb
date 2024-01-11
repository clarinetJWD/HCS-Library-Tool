Imports FluentFTP
Imports Utils.Encrypt

Public Class FtpAndSecurity

    Private Const _CREDS_ENC = "04nnwiUdtO/uVeDDbdeQUTm7LpP6u1+uOVEj3MSkjrKjR+2S19UgHLQO8aaeaTCB9f5rNlifPQezlmyoyER7fQ=="

    Friend Shared Function TestPasscode(passcode As String) As Boolean
        Dim connect As FtpClient = Nothing
        Try
            connect = GetFtpConnection(passcode)
            If connect Is Nothing Then Return False

            connect.AutoConnect()
            Return connect.IsAuthenticated
        Catch ex As Exception
            Return False
        Finally
            connect?.Disconnect()
            connect?.Dispose()
        End Try
    End Function

    Private Shared Function PadPasscode(passcode As String) As String
        If passcode.Length < 16 Then Return passcode.PadRight(24, " ")
        If passcode.Length < 24 Then Return passcode.PadRight(24, " ")
        Return passcode.PadRight(32, " ")
    End Function


    Friend Shared Function GetFtpConnection(passcode) As FtpClient
        Try
            Dim creds = AES.Decrypt(_CREDS_ENC, PadPasscode(passcode))
            Return New FtpClient(creds.Split(Chr(31))(0), creds.Split(Chr(31))(1), creds.Split(Chr(31))(2))
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Class
