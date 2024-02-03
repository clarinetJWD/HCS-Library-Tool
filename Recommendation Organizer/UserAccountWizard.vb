Imports System.ComponentModel
Imports System.Net
Imports Npgsql

Public Class UserAccountWizard

    Private Property _UserNameAndPasswords As New UserNameAndPasswords
    Private Property _UserInformation As UserInformation

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.DialogResult = DialogResult.None

        _UserNameAndPasswords.Username = My.Settings.Username

        Me.TextEditLoginUsername.DataBindings.Add(New Binding(NameOf(TextEditLoginUsername.Text), _UserNameAndPasswords, NameOf(UserNameAndPasswords.Username), False, DataSourceUpdateMode.OnPropertyChanged))
        Me.TextEditCreateUsername.DataBindings.Add(New Binding(NameOf(TextEditCreateUsername.Text), _UserNameAndPasswords, NameOf(UserNameAndPasswords.Username), False, DataSourceUpdateMode.OnPropertyChanged))

        Me.TextEditLoginPassword.DataBindings.Add(New Binding(NameOf(TextEditLoginPassword.Text), _UserNameAndPasswords, NameOf(UserNameAndPasswords.Password), False, DataSourceUpdateMode.OnPropertyChanged))
        Me.TextEditCreatePassword.DataBindings.Add(New Binding(NameOf(TextEditCreatePassword.Text), _UserNameAndPasswords, NameOf(UserNameAndPasswords.Password), False, DataSourceUpdateMode.OnPropertyChanged))
        Me.TextEditCreatePasswordConfirm.DataBindings.Add(New Binding(NameOf(TextEditCreatePasswordConfirm.Text), _UserNameAndPasswords, NameOf(UserNameAndPasswords.ConfirmPassword), False, DataSourceUpdateMode.OnPropertyChanged))
    End Sub

    Private Sub UserAccountWizard_ResizeEnd(sender As Object, e As EventArgs) Handles MyBase.ResizeEnd
        LayoutControl1.BestFit()
        LayoutControl2.BestFit()
    End Sub

    Private Sub NavigationFrame1_SelectedPageIndexChanged(sender As Object, e As EventArgs) Handles NavigationFrame1.SelectedPageIndexChanged
        If NavigationFrame1.SelectedPage Is NavigationPageLogin Then
            Me.AcceptButton = Me.SimpleButtonLogin
        ElseIf NavigationFrame1.SelectedPage Is NavigationPageCreate Then
            Me.AcceptButton = Me.SimpleButtonCreate
        Else
            Me.AcceptButton = Nothing
        End If
    End Sub

#Region "User Page Navigation"

    Private Sub HyperlinkLabelControlNewAccount_Click(sender As Object, e As EventArgs) Handles HyperlinkLabelControlNewAccount.Click
        NavigationFrame1.SelectedPage = NavigationPageToken
    End Sub

    Private Sub HyperlinkLabelControlCreateAlreadyHave_Click(sender As Object, e As EventArgs) Handles HyperlinkLabelControlCreateAlreadyHave.Click
        NavigationFrame1.SelectedPage = NavigationPageLogin
    End Sub

    Private Sub HyperlinkLabelControlCreateAl_Click(sender As Object, e As EventArgs) Handles HyperlinkLabelControlCreateAl.Click
        NavigationFrame1.SelectedPage = NavigationPageLogin
    End Sub

#End Region

#Region "Login Page"

    Private Sub SimpleButtonLogin_Click(sender As Object, e As EventArgs) Handles SimpleButtonLogin.Click
        If CredentialsHelper.TestUsernameAndPasswordCredentials(_UserNameAndPasswords.Username.ToLower.Trim, _UserNameAndPasswords.Password) Then
            If CredentialsHelper.SaveUserCredentials(_UserNameAndPasswords.Username.ToLower.Trim, _UserNameAndPasswords.Password, If(_UserInformation?.CredentialsKey, My.Settings.Passcode)) Then
                Me.DialogResult = DialogResult.OK
                Me.Close()
            End If
        Else
            LayoutControlItemLoginInvalid.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always
        End If
    End Sub

#End Region

#Region "Token Page"

    Private Sub LayoutControlDragToken_DragEnter(sender As Object, e As DragEventArgs) Handles LayoutControlDragToken.DragEnter, LabelControlTokenTitle.DragEnter, LabelControlTokenPrompt.DragEnter, HyperlinkLabelControlTokenBrowse.DragEnter, LabelControlTokenError.DragEnter, LabelControlTokenInfo.DragEnter
        ' FileDrop is array of names, same for FileNameW, FileName
        Try
            For Each fmat In e.Data.GetFormats
                If {"FileDrop", "FileNameW", "FileName"}.Contains(fmat) Then
                    Dim data = e.Data.GetData(fmat)
                    If TypeOf data Is IList Then
                        If DirectCast(data, IList).Count > 0 Then
                            Dim ext = IO.Path.GetExtension(DirectCast(data, IList)(0))
                            If ext.ToLower = ".hcsuser" Then
                                e.Effect = DragDropEffects.Copy
                            End If
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            ' No
        End Try
    End Sub

    Private Sub LayoutControlDragToken_DragDrop(sender As Object, e As DragEventArgs) Handles LayoutControlDragToken.DragDrop, LabelControlTokenTitle.DragDrop, LabelControlTokenPrompt.DragDrop, HyperlinkLabelControlTokenBrowse.DragDrop, LabelControlTokenError.DragDrop, LabelControlTokenInfo.DragDrop
        Try
            For Each fmat In e.Data.GetFormats
                If {"FileDrop", "", ""}.Contains(fmat) Then
                    Dim data = e.Data.GetData(fmat)
                    If TypeOf data Is IList Then
                        If DirectCast(data, IList).Count > 0 Then
                            If ValidateInvitation(CStr(DirectCast(data, IList)(0))) Then
                                Exit Sub
                            End If
                        End If
                    End If
                End If
            Next
        Catch ex As Exception

        End Try

        SetFileDropError()
    End Sub

    Private Sub HyperlinkLabelControlTokenBrowse_Click(sender As Object, e As EventArgs) Handles HyperlinkLabelControlTokenBrowse.Click
        Dim ofd As New OpenFileDialog()
        ofd.Title = "Open Invitation Token"
        ofd.Filter = "*.hcsuser|*.hcsuser"
        If ofd.ShowDialog = DialogResult.OK Then
            If ValidateInvitation(ofd.FileName) Then
                Exit Sub
            End If
            SetFileDropError()
        End If
    End Sub

    Private Function ValidateInvitation(filePath As String) As Boolean
        If IO.File.Exists(filePath) Then
            Dim encryptedUserInfo = IO.File.ReadAllText(filePath)

            Dim user = DatabaseConnection.DB.GetUserInformationFromInvitationKey(encryptedUserInfo)
            If user IsNot Nothing Then
                If Ftp.ValidatePasscode(user.CredentialsKey) Then
                    SetUserTokenAndChangeToCreatePage(user)
                    Return True
                End If
            End If
        End If
        Return False
    End Function

    Private Sub SetFileDropError()
        LayoutControlItemTokenError.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always
    End Sub

    Private Sub SetUserTokenAndChangeToCreatePage(user As UserInformation)
        LayoutControlItemTokenError.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never
        LabelControlCreateTitle.Text = $"Welcome, {user.Name}!"
        NavigationFrame1.SelectedPage = NavigationPageCreate

        _UserInformation = user
    End Sub

#End Region

#Region "Create Page"

    Private Sub TextEditCreateUsername_Validating(sender As Object, e As CancelEventArgs) Handles TextEditCreateUsername.Validating
        ValidateUsername()
    End Sub

    Private Sub TextEditCreatePassword_Validating(sender As Object, e As CancelEventArgs) Handles TextEditCreatePassword.Validating
        ValidatePasswordMeetsRequirements()
    End Sub

    Private Sub TextEditCreatePasswordConfirm_Validating(sender As Object, e As CancelEventArgs) Handles TextEditCreatePasswordConfirm.Validating
        ValidatePasswordsMatch()
    End Sub

    Private Function ValidateUsername() As Boolean
        Dim username = _UserNameAndPasswords.Username.Trim.ToLower
        If username.Length < 8 Then
            TextEditCreateUsername.ErrorText = "Username must be at least 8 characters."
            Return False
        End If

        If UserAlreadyExists() Then
            TextEditCreateUsername.ErrorText = "Username is already registered."
            Return False
        End If

        Return True
    End Function

    Private Function UserAlreadyExists() As Boolean
        Dim username = _UserNameAndPasswords.Username.Trim.ToLower
        Dim remotePath = Constants.HttpPath_UserCredentials(username)
        Dim webClient As New WebClient()
        Try
            Dim userCreds = webClient.DownloadString(remotePath)
            If userCreds <> Nothing Then Return True
        Catch ex As Exception
            Return False
        End Try

        Return False
    End Function

    Private Function ValidatePasswordMeetsRequirements() As Boolean

        Dim password = _UserNameAndPasswords.Password.Trim
        If password.Length < 8 Then
            TextEditCreatePassword.ErrorText = "Password must be at least 8 characters."
            Return False
        End If

        ' ASCII non-visible characters
        Dim asciiNonVisibleChars As New List(Of Char)
        For i = 0 To 31
            asciiNonVisibleChars.Add(Chr(i))
        Next
        For Each bannedAscii In asciiNonVisibleChars
            If password.Contains(bannedAscii) Then
                TextEditCreatePassword.ErrorText = "Nice try, no SQL injection please."
                Return False
            End If
        Next

        ' Whitespace
        If System.Text.RegularExpressions.Regex.Match(password, "[\s]").Success Then
            TextEditCreatePassword.ErrorText = "Password cannot contain whitespace."
            Return False
        End If

        ' Banned Characters
        Dim bannedChars = {";"c, "'"c, """"c, "{"c, "}"c, "\"c, "/"c}
        For Each bannedChar In bannedChars
            If password.Contains(bannedChar) Then
                TextEditCreatePassword.ErrorText = "Password cannot contain: ; ' "" { } \ /"
                Return False
            End If
        Next

        Return True
    End Function

    Private Function ValidatePasswordsMatch() As Boolean
        If _UserNameAndPasswords.Password.Trim <> _UserNameAndPasswords.ConfirmPassword.Trim Then
            TextEditCreatePasswordConfirm.ErrorText = "Passwords do not match."
            Return False
        End If
        Return True
    End Function

    Private Sub SimpleButtonCreate_Click(sender As Object, e As EventArgs) Handles SimpleButtonCreate.Click
        If ValidateForm() Then
            CredentialsHelper.CreateNewUser(_UserNameAndPasswords.Username.ToLower.Trim, _UserNameAndPasswords.Password.Trim, _UserInformation)

            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
    End Sub

    Private Function ValidateForm() As Boolean
        If Not ValidateUsername() Then Return False
        If Not ValidatePasswordsMatch() Then Return False
        If Not ValidatePasswordMeetsRequirements() Then Return False
        Return True
    End Function

#End Region

End Class

Class UserNameAndPasswords : Implements INotifyPropertyChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Property Username As String
        Get
            Return _Username
        End Get
        Set(value As String)
            _Username = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Username)))
        End Set
    End Property
    Private _Username As String = String.Empty

    Property Password As String
        Get
            Return _Password
        End Get
        Set(value As String)
            _Password = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Password)))
        End Set
    End Property
    Private _Password As String = String.Empty

    Property ConfirmPassword As String
        Get
            Return _ConfirmPassword
        End Get
        Set(value As String)
            _ConfirmPassword = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ConfirmPassword)))
        End Set
    End Property
    Private _ConfirmPassword As String = String.Empty
End Class

Class CredentialsHelper

    Friend Shared Function SaveUserCredentials(username As String, password As String, credentialsKey As String) As Boolean
        Dim userNamePass = GetUserPassKey(username.ToLower, password)
        Dim userNamePassKey = String.Join("", System.Net.Dns.GetHostName.Reverse)
        Dim encryptedUserNamePass = EncryptionHelper.EncryptString(userNamePass, userNamePassKey)

        My.Settings.Username = username.ToLower
        My.Settings.PasswordHash = encryptedUserNamePass
        My.Settings.Passcode = credentialsKey

        My.Settings.Save()

        Return True
    End Function

    Friend Shared Function CreateNewUser(username As String, userPassword As String, userInfo As UserInformation) As Boolean
        ' Download encoded DB connection string
        ' Decrpyt it using the credentials key

        ' User password is encrypted into a local key.
        Dim userNamePass = GetUserPassKey(username.ToLower, userPassword)

        SaveUserCredentials(username.ToLower, userPassword, userInfo.CredentialsKey)

        ' Secret key is encrypted with the user name pass and uploaded to an access folder
        Dim userEncryptedKey = EncryptionHelper.EncryptString(userInfo.CredentialsKey, userNamePass)

        Dim fName = Guid.NewGuid.ToString & ".enc"
        IO.File.WriteAllText(fName, userEncryptedKey)

        Try
            Return Ftp.UploadFile(fName, Constants.FtpPath_UserCredentials(username.ToLower), 5)
        Catch ex As Exception
            Return False
        Finally
            IO.File.Delete(fName)
        End Try

    End Function

    Private Shared Function GetUserPassKey(userName As String, userPassword As String) As String

        Dim passKey As String = ""
        For i As Integer = 0 To Math.Max(userName.Length, userPassword.Length) - 1
            If userName.Length > i Then
                passKey &= userName.ToLower.Chars(i)
            End If
            If userPassword.Length > i Then
                passKey &= userPassword.Chars(i)
            End If
        Next
        Return passKey
    End Function

    Friend Shared Function TestSavedCredentials() As Boolean
        Dim userNamePassKey = String.Join("", System.Net.Dns.GetHostName.Reverse)
        Try
            Return TestUsernameAndKeyCredentials(
                My.Settings.Username,
                EncryptionHelper.DecryptString(My.Settings.PasswordHash, userNamePassKey))
        Catch ex As Exception
            Return False
        End Try
    End Function

    Friend Shared Function TestUsernameAndPasswordCredentials(username As String, userPassword As String) As Boolean

        Dim userNamePass = GetUserPassKey(username.ToLower, userPassword)
        Return TestUsernameAndKeyCredentials(username.ToLower, userNamePass)

    End Function

    Friend Shared Function TestUsernameAndKeyCredentials(username As String, userNamePass As String) As Boolean

        Dim httpPath = Constants.HttpPath_UserCredentials(username.ToLower)

        Dim encCreds As String
        Dim webClient As New WebClient()
        Try
            encCreds = webClient.DownloadString(httpPath)
        Catch ex As Exception
            Return False
        End Try

        If encCreds = Nothing Then Return False

        Try
            Dim unencryptedCredKey = EncryptionHelper.DecryptString(encCreds, userNamePass)
            Return DatabaseConnection.DB.TestCredentials(unencryptedCredKey)
        Catch ex As Exception
            Return False
        End Try

    End Function


End Class

Friend Class UserInformation
    Property Name As String
    Property CredentialsKey As String
    Property Permissions As UserPermissions
    Property UserExpiry As Date

End Class


<Flags>
Friend Enum UserPermissions
    ViewOnly = 0
    CreatePersonalSeasons = 1
    CreatePublicSeasons = 1 << 1
    EditData = 1 << 2

    Admin = 1 << 30
End Enum