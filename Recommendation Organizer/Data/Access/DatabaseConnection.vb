Imports System.Net
Imports Npgsql

Friend Class DatabaseConnection

    Shared Property DB As DatabaseConnection

    ReadOnly Property UseDebugDatabase As Boolean

    Private ReadOnly Property _CredentialsEncoded As String
        Get
            Static __CredEnc As String = Nothing
            If __CredEnc Is Nothing Then
                Dim webClient As New WebClient()
                Try
                    Dim encodedCreds As String = webClient.DownloadString(HttpPath_DatabaseCredentials(UseDebugDatabase))
                    __CredEnc = encodedCreds
                Catch ex As Exception
                    __CredEnc = Nothing
                End Try
            End If
            Return __CredEnc
        End Get
    End Property

    Sub New(debug As Boolean)
        Me.UseDebugDatabase = debug
    End Sub

    Friend Function GetUserInvitation(name As String, userPermissions As UserPermissions, userExpiry As Date, credentialsKey As String) As String

        Dim userString = name & Chr(31) & CStr(CInt(userPermissions)) & Chr(31) & userExpiry.ToString("yyyy-MM-dd") & Chr(31) & credentialsKey
        Return EncryptionHelper.EncryptString(userString, "hcs-user-info")

    End Function

    Friend Function GetUserInformationFromInvitationKey(invitationKey As String) As UserInformation
        Try
            Dim nameAndCredentialsKey = EncryptionHelper.DecryptString(invitationKey, "hcs-user-info")
            Dim userTokens = nameAndCredentialsKey.Split(Chr(31))

            Return New UserInformation() With {
                .Name = userTokens(0),
                .Permissions = userTokens(1),
                .UserExpiry = Date.ParseExact(userTokens(2), "yyyy-MM-dd", Nothing),
                .CredentialsKey = userTokens(3)}
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Friend Function GetDatabaseConnection() As Npgsql.NpgsqlConnection

        ' 1. Get the user's credentials
        Dim httpPath = Constants.HttpPath_UserCredentials(My.Settings.Username)
        Dim encCreds As String

        Dim webClient As New WebClient()
        Try
            encCreds = webClient.DownloadString(httpPath)
        Catch ex As Exception
            Return Nothing
        End Try

        Dim userNamePassKey = String.Join("", System.Net.Dns.GetHostName.Reverse)
        Dim unencryptedUserNamePass = EncryptionHelper.DecryptString(My.Settings.PasswordHash, userNamePassKey)
        Dim unencryptedCredKey = EncryptionHelper.DecryptString(encCreds, unencryptedUserNamePass)

        Dim connString = EncryptionHelper.DecryptString(_CredentialsEncoded, unencryptedCredKey)
        Dim dataSourceBuilder = New NpgsqlDataSourceBuilder(connString)
        Dim dataSource = dataSourceBuilder.Build()

        Dim conn = dataSource.OpenConnection()

        Return conn

    End Function

    Friend Function TestCredentials(unencryptedCredKey As String) As Boolean

        Dim conn As Npgsql.NpgsqlConnection = Nothing
        Try
            Dim connString = EncryptionHelper.DecryptString(_CredentialsEncoded, unencryptedCredKey)
            Dim dataSourceBuilder = New NpgsqlDataSourceBuilder(connString)
            Dim dataSource = dataSourceBuilder.Build()

            conn = dataSource.OpenConnection()
            If conn.State = ConnectionState.Open Then
                Return True
            End If
            Return False
        Catch ex As Exception
            Return False
        Finally
            If conn IsNot Nothing Then
                conn.Close()
                conn.Dispose()
            End If
        End Try

    End Function

End Class
