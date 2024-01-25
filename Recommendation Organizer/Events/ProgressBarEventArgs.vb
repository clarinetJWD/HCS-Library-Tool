Public Class ProgressBarEventArgs : Inherits EventArgs
    Property Minimum As Integer? = Nothing
    Property Maximum As Integer? = Nothing
    Property Value As Integer? = Nothing
    Property Caption As String = Nothing
    Property Visible As Boolean = True
    Property IsMarquee As Boolean = False
End Class