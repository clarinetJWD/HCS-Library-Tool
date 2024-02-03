Imports Utils.Encrypt

Public Class EncryptionHelper
    Friend Shared Function EncryptString(inputString As String, key As String) As String
        Return AES.Encrypt(inputString, PadPasscode(key))
    End Function

    Friend Shared Function DecryptString(encryptedString As String, key As String) As String
        Return AES.Decrypt(encryptedString, PadPasscode(key))
    End Function

    Private Shared Function PadPasscode(passcode As String) As String
        If passcode.Length < 16 Then Return passcode.PadRight(24, " ")
        If passcode.Length < 24 Then Return passcode.PadRight(24, " ")
        Return passcode.PadRight(32, " ")
    End Function
End Class
