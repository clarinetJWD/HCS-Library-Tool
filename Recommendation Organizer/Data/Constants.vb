Imports System.ComponentModel
Imports System.Environment

Public Module Constants

    Sub New()
        CleanUpDataFiles()
    End Sub

    Public Enum CsvFormats
        None
        Holdings
        Repertoire
        Recommendation
    End Enum

    Public Enum Difficulties
        <Description("")>
        NotSet
        <Description("Easy")>
        Easy
        <Description("Intermediate")>
        Intermediate
        <Description("Difficult")>
        Difficult
        <Description("Very Difficult")>
        VeryDifficult
        <Description("Too Hard")>
        TooHard
    End Enum

#Region "Files and Paths"

    ReadOnly Property FtpPath_Library As String
        Get
            Return "library.dat"
        End Get
    End Property

    ReadOnly Property FtpPath_Library_Temp(uniqueString As String) As String
        Get
            Return $"library_{uniqueString}.dat"
        End Get
    End Property

    ReadOnly Property FtpPath_PublishedSeasons As String
        Get
            Return "published_seasons.dat"
        End Get
    End Property

    ReadOnly Property LocalPath_Library As String
        Get
            Return IO.Path.Combine(LocalDirectory_AppData_Library, "library.dat")
        End Get
    End Property

    ReadOnly Property LocalPath_Library_Temp(uniqueString As String) As String
        Get
            Return IO.Path.Combine(LocalDirectory_AppData_Library, $"library_{uniqueString}.dat")
        End Get
    End Property

    ReadOnly Property LocalPath_PublishedSeasons As String
        Get
            Return IO.Path.Combine(LocalDirectory_AppData_Library, "published_seasons.dat")
        End Get
    End Property

    ReadOnly Property LocalPath_SeasonPlanningItems As String
        Get
            Return IO.Path.Combine(LocalDirectory_AppData, "SeasonPlanningItems.xml")
        End Get
    End Property

    ReadOnly Property LocalPath_PublishedSeasons_Temp(uniqueString As String) As String
        Get
            Return IO.Path.Combine(LocalDirectory_AppData_Library, $"published_seasons_{uniqueString}.dat")
        End Get
    End Property

    ReadOnly Property LocalPath_WorkingSeasonInfo As String
        Get
            Return IO.Path.Combine(LocalDirectory_AppData_Library, "working_season_info.dat")
        End Get
    End Property

    ReadOnly Property LocalPath_WorkingSeasonInfo_Temp(uniqueString As String) As String
        Get
            Return IO.Path.Combine(LocalDirectory_AppData_Library, $"working_season_info_{uniqueString}.dat")
        End Get
    End Property

    ReadOnly Property LocalPath_RecommendationGridSettings As String
        Get
            Return IO.Path.Combine(LocalDirectory_AppData, $"RecommendationGridSettings.xml")
        End Get
    End Property

    ReadOnly Property LocalPath_ComposerGridSettings As String
        Get
            Return IO.Path.Combine(LocalDirectory_AppData, $"ComposerGridSettings.xml")
        End Get
    End Property

    ReadOnly Property LocalPath_ErasGridSettings As String
        Get
            Return IO.Path.Combine(LocalDirectory_AppData, $"ErasGridSettings.xml")
        End Get
    End Property

    ReadOnly Property LocalPath_TagsGridSettings As String
        Get
            Return IO.Path.Combine(LocalDirectory_AppData, $"TagsGridSettings.xml")
        End Get
    End Property

    ReadOnly Property LocalPath_SeasonPlanningGridSettings As String
        Get
            Return IO.Path.Combine(LocalDirectory_AppData, $"SeasonPlanningGridSettings.xml")
        End Get
    End Property

    ReadOnly Property LocalPath_ConcertGridSettings(name As String) As String
        Get
            Return IO.Path.Combine(LocalDirectory_AppData, $"ConcertGridSettings_{name}.xml")
        End Get
    End Property


    ReadOnly Property LocalDirectory_AppData As String
        Get
            If Not IO.Directory.Exists(IO.Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), "HcsLibraryTool")) Then
                IO.Directory.CreateDirectory(IO.Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), "HcsLibraryTool"))
            End If
            Return IO.Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), "HcsLibraryTool")
        End Get
    End Property

    ReadOnly Property LocalDirectory_AppData_Library As String
        Get
            If Not IO.Directory.Exists(IO.Path.Combine(LocalDirectory_AppData, "Library")) Then
                IO.Directory.CreateDirectory(IO.Path.Combine(LocalDirectory_AppData, "Library"))
            End If
            Return IO.Path.Combine(LocalDirectory_AppData, "Library")
        End Get
    End Property

    Private Sub CleanUpDataFiles()
        If IO.File.Exists("library.dat") Then
            Dim shouldMoveLibrary As Boolean = True

            If IO.File.Exists(LocalPath_Library) Then
                If IO.File.GetLastWriteTime("library.dat") < IO.File.GetLastWriteTime(LocalPath_Library) Then
                    shouldMoveLibrary = False
                End If
                If shouldMoveLibrary Then
                    Dim timeString = Now.ToString("yyyyMMddHHmmssfff")
                    IO.File.Move(LocalPath_Library, LocalPath_Library_Temp(timeString))
                End If
            End If

            If shouldMoveLibrary Then
                IO.File.Move("library.dat", LocalPath_Library)
            End If
        End If

        If IO.File.Exists("published_seasons.dat") Then
            Dim shouldMoveLibrary As Boolean = True

            If IO.File.Exists(LocalPath_PublishedSeasons) Then
                If IO.File.GetLastWriteTime("published_seasons.dat") < IO.File.GetLastWriteTime(LocalPath_PublishedSeasons) Then
                    shouldMoveLibrary = False
                End If
                If shouldMoveLibrary Then
                    Dim timeString = Now.ToString("yyyyMMddHHmmssfff")
                    IO.File.Move(LocalPath_PublishedSeasons, LocalPath_PublishedSeasons_Temp(timeString))
                End If
            End If

            If shouldMoveLibrary Then
                IO.File.Move("published_seasons.dat", LocalPath_PublishedSeasons)
            End If
        End If

        If IO.File.Exists("working_season_info.dat") Then
            Dim shouldMoveLibrary As Boolean = True

            If IO.File.Exists(LocalPath_WorkingSeasonInfo) Then
                If IO.File.GetLastWriteTime("working_season_info.dat") < IO.File.GetLastWriteTime(LocalPath_WorkingSeasonInfo) Then
                    shouldMoveLibrary = False
                End If
                If shouldMoveLibrary Then
                    Dim timeString = Now.ToString("yyyyMMddHHmmssfff")
                    IO.File.Move(LocalPath_WorkingSeasonInfo, LocalPath_WorkingSeasonInfo_Temp(timeString))
                End If
            End If

            If shouldMoveLibrary Then
                IO.File.Move("working_season_info.dat", LocalPath_WorkingSeasonInfo)
            End If
        End If
    End Sub

#End Region



End Module
