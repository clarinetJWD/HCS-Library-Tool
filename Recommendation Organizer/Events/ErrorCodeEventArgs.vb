Public Class ErrorCodeEventArgs : Inherits EventArgs

    Public Property ErrorCode As ErrorCodes = ErrorCodes.None
    Public Property Critical As Boolean = False
    Public Property ShowMode As ShowModes = ShowModes.Modal

    Enum ErrorCodes
        None
        PublishedSeasonIndexFailedToDownload
        SaveLibraryFailedBecauseRemoteFileCouldNotBeReplaced
        SaveLibraryFailedBecauseFileCouldNotBeUploaded
        SaveLibraryFailedBecauseTheRemoteFileIsNewer
        SaveLibraryFailedForAnUnknownReason
        LoadSeasonProposalFromIndexFailedBecauseTheFileFailedToDownload
        LoadSeasonProposalFromIndexFailedBecauseTheFileDoesNotExist
        LoadSeasonProposalFromIndexFailedUnknownReason
        PublishSeasonProposalFailedBecauseTheFileFailedToUpload
        PublishSeasonProposalFailedBecauseTheTableOfContentsFailedToUpload
        PublishSeasonProposalFailedUnknownReason
        DeleteSeasonProposalFailedBecauseTheTableOfContentsFailedToUpload
        DeleteSeasonProposalFailedUnknownReason
        CouldNotLoadUserSeasonPlanningList
        CouldNotSaveUserSeasonPlanningList
        CouldNotLoadUserWorkingSeasonData
        CouldNotSaveUserWorkingSeasonData
        LibraryFailedToLoadFromServer
    End Enum

    Enum ShowModes
        Modal
        StatusBar
    End Enum

End Class