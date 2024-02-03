Imports System.Net
Imports FluentFTP

Public Class Ftp

    Private Shared _CredentialsEncoded As String = Nothing
    Private Shared ReadOnly Property _Clients As New Dictionary(Of String, FtpClient)

    Shared Sub New()
        AddHandler Application.ApplicationExit, AddressOf OnApplicationExit

        Dim webClient As New WebClient()
        Try
            Dim encodedCreds As String = webClient.DownloadString(HttpPath_FtpCredentials)
            _CredentialsEncoded = encodedCreds
        Catch ex As Exception
            _CredentialsEncoded = Nothing
        End Try
    End Sub

    Private Shared Sub OnApplicationExit(sender As Object, e As EventArgs)
        DisposeClients()
    End Sub

    Private Shared Sub DisposeClients()
        For Each key In _Clients.Keys
            DisposeClient(key)
        Next
    End Sub
    Private Shared Sub DisposeClient(passcode As String)
        If _Clients.ContainsKey(passcode) AndAlso _Clients(passcode) IsNot Nothing Then
            If _Clients(passcode).IsConnected Then
                _Clients(passcode).Disconnect()
            End If
            _Clients(passcode).Dispose()
        End If
    End Sub

    Friend Shared Function ValidatePasscode(credentialsKey As String) As Boolean
        If credentialsKey = Nothing Then credentialsKey = String.Empty

        If _Clients.ContainsKey(credentialsKey) Then
            If _Clients(credentialsKey).IsAuthenticated Then
                Return True
            End If
        End If

        Dim connection As FtpClient = Nothing
        Try
            connection = GetFtpConnection(credentialsKey, False)
            If connection IsNot Nothing AndAlso connection.IsAuthenticated Then Return True
        Catch ex As Exception
            Return False
        Finally
            If connection IsNot Nothing Then
                connection.Disconnect()
                connection.Dispose()
            End If
        End Try
    End Function

    Private Shared Function GetFtpConnection(passcode As String, Optional useCache As Boolean = True) As FtpClient
        Try
            If useCache Then
                If _Clients.ContainsKey(passcode) Then
                    Dim thisClient = _Clients(passcode)
                    If thisClient IsNot Nothing AndAlso thisClient.IsConnected Then
                        Return thisClient
                    End If
                End If
            End If

            Dim creds = EncryptionHelper.DecryptString(_CredentialsEncoded, passcode)
            Dim tempClient = New FtpClient(creds.Split(Chr(31))(0), creds.Split(Chr(31))(1), creds.Split(Chr(31))(2))
            tempClient.AutoConnect()
            If tempClient.IsConnected Then
                If useCache Then
                    If _Clients.ContainsKey(passcode) Then
                        DisposeClient(passcode)
                    End If

                    _Clients(passcode) = tempClient
                End If
            End If

            If useCache Then
                Return _Clients(passcode)
            Else
                Return tempClient
            End If

        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Friend Shared Function DownloadFile(localPath As String, remotePath As String, numRetries As Integer, Optional progressAction As Action(Of FtpProgress) = Nothing) As Boolean
        Return DownloadFile(localPath, remotePath, numRetries, My.Settings.Passcode, progressAction)
    End Function
    Friend Shared Function DownloadFile(localPath As String, remotePath As String, numRetries As Integer, passcode As String, Optional progressAction As Action(Of FtpProgress) = Nothing) As Boolean

        Do
            Try
                Dim client As FtpClient = Ftp.GetFtpConnection(passcode)
                If client Is Nothing Then Return False

                If client.FileExists(remotePath) Then
                    Dim dlStatus = client.DownloadFile(
                            localPath, remotePath, FtpLocalExists.Overwrite, FtpVerify.Retry, progressAction)
                    Return (dlStatus = FtpStatus.Success)
                End If
            Catch ex As Exception
                ' TODO Log
            End Try

            numRetries -= 1
        Loop While numRetries > 0

        Return False

    End Function

    Friend Shared Function UploadFile(localPath As String, remotePath As String, numRetries As Integer, Optional progressAction As Action(Of FtpProgress) = Nothing) As Boolean
        Return UploadFile(localPath, remotePath, numRetries, My.Settings.Passcode, progressAction)
    End Function
    Friend Shared Function UploadFile(localPath As String, remotePath As String, numRetries As Integer, passcode As String, Optional progressAction As Action(Of FtpProgress) = Nothing) As Boolean

        Do
            Try
                Dim client As FtpClient = Ftp.GetFtpConnection(passcode)
                If client Is Nothing Then Return False

                If IO.File.Exists(localPath) Then
                    Dim upStatus = client.UploadFile(localPath, remotePath,, True, FtpVerify.Retry, progressAction)

                    Return (upStatus = FtpStatus.Success)
                End If
            Catch ex As Exception
                ' TODO Log
            End Try

            numRetries -= 1
        Loop While numRetries > 0

        Return False

    End Function

    Friend Shared Function MoveFile(oldPath As String, newPath As String) As Boolean
        Return FtpMoveFile(oldPath, newPath, My.Settings.Passcode)
    End Function
    Friend Shared Function FtpMoveFile(oldPath As String, newPath As String, passcode As String) As Boolean

        Try
            Dim client As FtpClient = Ftp.GetFtpConnection(passcode)
            If client.FileExists(oldPath) Then
                Return client.MoveFile(oldPath, newPath)
            End If
        Catch ex As Exception
            ' TODO Log
        End Try

        Return False
    End Function

    Friend Shared Function FileExists(path As String) As Boolean
        Return FileExists(path, My.Settings.Passcode)
    End Function
    Friend Shared Function FileExists(path As String, passcode As String) As Boolean
        Try
            Dim client As FtpClient = Ftp.GetFtpConnection(passcode)
            If client Is Nothing Then Return False

            Return client.FileExists(path)
        Catch ex As Exception
            ' TODO Log
        End Try

        Return False
    End Function

    Friend Shared Function GetModifiedTime(path As String) As Date
        Return GetModifiedTime(path, My.Settings.Passcode)
    End Function
    Friend Shared Function GetModifiedTime(path As String, passcode As String) As Date
        Try
            Dim client As FtpClient = Ftp.GetFtpConnection(passcode)
            If client Is Nothing Then Return Date.MinValue

            Return client.GetModifiedTime(path)
        Catch ex As Exception
            ' TODO Log
        End Try

        Return Date.MinValue
    End Function

End Class
