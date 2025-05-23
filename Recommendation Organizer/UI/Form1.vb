﻿Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Deployment.Application
Imports System.Environment
Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports System.Security
Imports System.Security.Policy
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Csv
Imports DevExpress.Utils.DragDrop
Imports DevExpress.XtraBars
Imports DevExpress.XtraBars.Navigation
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Grid
Imports FluentFTP
Imports Npgsql

Public Class Form1 : Implements INotifyPropertyChanged

#Region "Events"

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

#End Region

#Region "Class Fields and Properties"

    Private WithEvents _Presenter As New LibraryToolPresenter

    Private _Context As SynchronizationContext

    Private _ErrorIcon As DevExpress.Utils.Svg.SvgImage

    ReadOnly Property LibraryTabIsVisible As Boolean
        Get
            Return True
        End Get
    End Property

    ReadOnly Property MetadataTabIsVisible As Boolean
        Get
            Return (_Presenter.ComposersAreLoaded AndAlso _Presenter.ErasAreLoaded AndAlso _Presenter.TagsAreLoaded) AndAlso
                (Not _Presenter.ComposersLoadError AndAlso Not _Presenter.ErasLoadError AndAlso Not _Presenter.TagsLoadError)
        End Get
    End Property

    ReadOnly Property SeasonPlanningTabIsVisible As Boolean
        Get
            Return _Presenter.SeasonPlannerListIsLoaded AndAlso Not _Presenter.SeasonPlannerListLoadError AndAlso
                _Presenter.SeasonIndexesAreLoaded AndAlso Not _Presenter.SeasonIndexesLoadError
        End Get
    End Property

#End Region

#Region "Initialization"

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        'DatabaseConnection.DB = New DatabaseConnection(True)

        'If Not CredentialsHelper.TestSavedCredentials() Then
        '    Dim uaw As New UserAccountWizard
        '    uaw.ShowDialog()
        'End If

        'Dim dbCon = DatabaseConnection.DB.GetDatabaseConnection()
        'dbCon.Close()

        _Context = Threading.SynchronizationContext.Current

#If DEBUG Then
        If Not Debugger.IsAttached Then MsgBox("Attach Debugger")
#Else
        'BarButtonItemRefreshMetadataSelected.Visibility = BarItemVisibility.Never
        BarButtonItemRefreshMetadataAll.Visibility = BarItemVisibility.Never
#End If
        _ErrorIcon = BarStaticItemMessage.ImageOptions.SvgImage

        If Deployment.Application.ApplicationDeployment.IsNetworkDeployed Then
            Try
                Me.Text &= $" v{Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion:4}"
                Me.BarStaticItemVersion.Caption = $"HCS Library Tool v{Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion:4}"
            Catch ex As Exception
            End Try
        End If
        InitializeTabs()
        InitializeConcertGrids()
        InitializeData()
    End Sub
    Private Sub InitializeTabs()
        Me.TabNavigationPageLibrary.PageVisible = LibraryTabIsVisible
        Me.TabNavigationPageMetadata.PageVisible = MetadataTabIsVisible
        Me.TabNavigationPageSeasonPlanner.PageVisible = SeasonPlanningTabIsVisible
    End Sub

    Private Async Sub InitializeData()
        If Not LoadOrPromptPassword() Then
            Exit Sub
        End If

        If Not Await _Presenter.Initialize() Then
            Exit Sub
        Else
            CheckForUpdateAndApply(True)
        End If

    End Sub

#End Region

#Region "Closing"

    Private _CloseInProgress As Boolean = False

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If Not _Presenter.IsAllowedClose Then
            _CloseInProgress = True
            e.Cancel = True
            Exit Sub
        Else
            _CloseInProgress = False
        End If

        If _Presenter.LibraryIsLoaded AndAlso _Presenter.LibraryHasChanges Then
            Select Case DevExpress.XtraEditors.XtraMessageBox.Show("Do you want to save Library changes?", "Unsaved Changes", MessageBoxButtons.YesNoCancel)
                Case DialogResult.Yes
                    SaveLibraryAndExit()
                    e.Cancel = True
                    Exit Sub
                Case DialogResult.No
                    ' Do nothing, lose changes
                Case DialogResult.Cancel
                    e.Cancel = True
                    Exit Sub
            End Select
        End If

        SaveUserSettings()
    End Sub

    Private Async Sub SaveLibraryAndExit()
        If Await _Presenter.SaveLibrary Then
            Me.Close()
        End If
    End Sub

    Private Sub CloseFormIfAbleAndNeeded()
        If _Presenter.IsAllowedClose AndAlso _CloseInProgress Then
            _Context.Post(Sub(state)
                              Me.Close()
                          End Sub, Nothing)
        End If
    End Sub

#End Region

#Region "Menu Bar"

    Private Async Sub BarButtonItemSave_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItemSaveAll.ItemClick
        DisableSaveButtons()
        Await _Presenter.SaveLibrary()
        EnableSaveButtons()
    End Sub

    Private Sub BarButtonItemMergeSelected_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItemMergeSelected.ItemClick
        If TabPane1.SelectedPage Is TabNavigationPageLibrary Then
            MergeSelectionsFromMusicList()
        ElseIf TabPane1.SelectedPage Is TabNavigationPageMetadata Then
            MergeSelectionsFromComposers()
        End If
    End Sub

    Private Sub BarButtonItemBarButtonItemImportFromCsv_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BarButtonItemImportFromCsv.ItemClick
        Dim ofd As New OpenFileDialog
        ofd.Filter = "*.csv|*.csv"

        If ofd.ShowDialog = DialogResult.OK Then
            LoadCsv(ofd.FileName)
        End If
    End Sub

    Private Sub BarButtonItemClearSeasonPlanner_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BarButtonItemClearSeasonPlanner.ItemClick
        If DevExpress.XtraEditors.XtraMessageBox.Show("Are you sure you wish to clear all items from the Season Planning List?", "Clear Season Planning List", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            _Presenter.ClearSeasonPlanningList()
        End If
    End Sub

    Private Sub OnShortcutItemClick(sender As Object, e As ShortcutItemClickEventArgs) Handles BarManager1.ShortcutItemClick

        Dim anyVisible As Boolean = False
        For Each linkMenu As BarSubItem In e.Item.Links.Select(Function(x) x.OwnerItem)
            If linkMenu IsNot Nothing Then
                If linkMenu.Visibility = BarItemVisibility.Always Then
                    anyVisible = True
                End If
            End If
        Next

        If Not anyVisible Then
            e.Cancel = True
        End If

    End Sub

    Private Sub TabPane1_SelectedPageChanged(sender As Object, e As DevExpress.XtraBars.Navigation.SelectedPageChangedEventArgs) Handles TabPane1.SelectedPageChanged
        BarManager1.BeginUpdate()

        If e.Page Is TabNavigationPageLibrary Then
            Me.BarSubItemFileLibrary.Visibility = DevExpress.XtraBars.BarItemVisibility.Always
            Me.BarSubItemFileMetadata.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
            Me.BarSubItemFileSeason.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

            Me.BarSubItemEditLibrary.Visibility = DevExpress.XtraBars.BarItemVisibility.Always
            Me.BarSubItemEditMetadata.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
            Me.BarSubItemEditSeason.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        ElseIf e.Page Is TabNavigationPageMetadata Then
            Me.BarSubItemFileLibrary.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
            Me.BarSubItemFileMetadata.Visibility = DevExpress.XtraBars.BarItemVisibility.Always
            Me.BarSubItemFileSeason.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

            Me.BarSubItemEditLibrary.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
            Me.BarSubItemEditMetadata.Visibility = DevExpress.XtraBars.BarItemVisibility.Always
            Me.BarSubItemEditSeason.Visibility = DevExpress.XtraBars.BarItemVisibility.Never

        ElseIf e.Page Is TabNavigationPageSeasonPlanner Then
            Me.BarSubItemFileLibrary.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
            Me.BarSubItemFileMetadata.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
            Me.BarSubItemFileSeason.Visibility = DevExpress.XtraBars.BarItemVisibility.Always

            Me.BarSubItemEditLibrary.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
            Me.BarSubItemEditMetadata.Visibility = DevExpress.XtraBars.BarItemVisibility.Never
            Me.BarSubItemEditSeason.Visibility = DevExpress.XtraBars.BarItemVisibility.Always

        End If

        BarManager1.EndUpdate()
    End Sub

    Private Sub EnableSaveButtons()
        BarButtonItemSaveAll.Enabled = True
    End Sub

    Private Sub DisableSaveButtons()
        BarButtonItemSaveAll.Enabled = False
    End Sub

#End Region

#Region "Season Planner Tab"

    Public ReadOnly Property ProposedSeasonTitle As String
        Get
            If _CurrentlyAppliedSeason Is Nothing Then Return "Proposed Season [Unsaved]"
            Return $"Proposed Season: {_CurrentlyAppliedSeason}"
        End Get
    End Property

    Private Property _CurrentlyAppliedSeason As PublishedSeasonIndex
        Get
            Return __CurrentlyAppliedSeason
        End Get
        Set(value As PublishedSeasonIndex)
            __CurrentlyAppliedSeason = value
            LayoutControlGroupProposedSeason.Text = ProposedSeasonTitle
        End Set
    End Property
    Private Sub OnCurrentSeasonPropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles __CurrentlyAppliedSeason.PropertyChanged
        LayoutControlGroupProposedSeason.Text = ProposedSeasonTitle
    End Sub
    Private WithEvents __CurrentlyAppliedSeason As PublishedSeasonIndex

    Private Sub InitializeConcertGrids()
        For Each concertGrid In GetConcertGrids()
            concertGrid.Initialize(_Presenter, BehaviorManager1)
        Next

        BehaviorManager1.Attach(Of DragDropBehavior)(GridViewSeasonPlanner,
                                                     Sub(behavior)
                                                         behavior.Properties.AllowDrop = True
                                                         behavior.Properties.AllowDrag = True
                                                         behavior.Properties.InsertIndicatorVisible = True
                                                         behavior.Properties.PreviewVisible = True
                                                         AddHandler behavior.DragDrop, AddressOf OnDragDropToPlannerGrid
                                                     End Sub)
    End Sub

    Private Sub OnDragDropToPlannerGrid(sender As Object, e As DragDropEventArgs)

        Dim sourceView = DirectCast(e.Source, GridView)
        Dim rowsToMove As New List(Of SeasonItem)
        For Each dataHandle As Integer In e.Data
            Dim row As SeasonItem = sourceView.GetRow(dataHandle)
            rowsToMove.Add(row)
        Next
        Dim targetView = DirectCast(e.Target, GridView)

        Select Case e.Action
            Case DragDropActions.Copy
                For Each row In rowsToMove
                    _Presenter.AddSeasonPlannerItem(row.Recommendation)
                Next
                e.Handled = True
            Case DragDropActions.Move
                Dim hi = targetView.CalcHitInfo(targetView.GridControl.PointToClient(e.Location))
                Dim insertLocation = targetView.DataRowCount
                If hi.InDataRow AndAlso e.InsertType <> InsertType.AsChild Then
                    Dim targetRow As SeasonItem = targetView.GetRow(hi.RowHandle)
                    insertLocation = targetView.DataSource.IndexOf(targetRow)
                    If e.InsertType <> InsertType.Before Then
                        insertLocation += 1
                    End If
                End If

                For i As Integer = rowsToMove.Count - 1 To 0 Step -1
                    Dim row = rowsToMove(i)
                    sourceView.DataSource.remove(row)
                    _Presenter.AddSeasonPlannerItem(row.Recommendation, insertLocation)

                    Dim foundItem = _Presenter.SeasonPlannerItems.FindSeasonItem(row)

                    Dim ind = _Presenter.SeasonPlannerItems.IndexOf(foundItem)
                    Dim handle = targetView.GetRowHandle(ind)

                    targetView.FocusedRowHandle = handle
                Next

                e.Handled = True
        End Select
        _Presenter.EnsureNoSeasonPlanningDuplicates()
    End Sub

    Private Function GetConcertGrids() As List(Of ConcertGrid)
        Return {ConcertGrid1, ConcertGrid2, ConcertGrid3, ConcertGrid4, ConcertGrid5, ConcertGrid6}.ToList
    End Function

    Private Sub OnPublishedSeasonsListChanged(sender As Object, e As ListChangedEventArgs)
        UpdatePublishedSeasonsMenu()
    End Sub

    Private Sub UpdatePublishedSeasonsMenu()
        _Context.Post(Sub(state)
                          BarSubItemLoadSeason.ClearLinks()
                          BarSubItemPublishSeason.ClearLinks()
                          BarSubItemDeleteSeason.ClearLinks()

                          For Each publishedSeasonIndex In _Presenter.PublishedSeasonIndexes
                              Dim seasonButton As New BarButtonItem(Me.BarManager1, publishedSeasonIndex.Name & $" [{publishedSeasonIndex.LastModified:g}]")
                              seasonButton.Tag = publishedSeasonIndex
                              AddHandler seasonButton.ItemClick, AddressOf OnLoadPublishedSeasonItemClick
                              BarSubItemLoadSeason.AddItem(seasonButton)


                              Dim publishButton As New BarButtonItem(Me.BarManager1, publishedSeasonIndex.Name & $" [{publishedSeasonIndex.LastModified:g}]")
                              publishButton.Tag = publishedSeasonIndex
                              AddHandler publishButton.ItemClick, AddressOf OnUpdatePublishedSeasonItemClick
                              BarSubItemPublishSeason.AddItem(publishButton)

                              Dim deleteButton As New BarButtonItem(Me.BarManager1, publishedSeasonIndex.Name & $" [{publishedSeasonIndex.LastModified:g}]")
                              deleteButton.Tag = publishedSeasonIndex
                              AddHandler deleteButton.ItemClick, AddressOf OnDeletePublishedSeasonItemClick
                              BarSubItemDeleteSeason.AddItem(deleteButton)
                          Next

                          Dim saveAsNewButton As New BarButtonItem(Me.BarManager1, "Save as new and upload...")
                          AddHandler saveAsNewButton.ItemClick, AddressOf OnSaveAsNewSeason
                          Dim newLink = BarSubItemPublishSeason.AddItem(saveAsNewButton)
                          newLink.BeginGroup = True
                      End Sub, Nothing)
    End Sub

    Private Sub BarButtonItemNewSeason_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BarButtonItemNewSeason.ItemClick
        Select Case DevExpress.XtraEditors.XtraMessageBox.Show("Unpublished changes will be lost. Do you wish to continue?", "New Season Proposal", MessageBoxButtons.YesNo)
            Case DialogResult.Yes
                _Presenter.NewWorkingSeasonInformation()

            Case DialogResult.No
                ' Do nothing

        End Select
    End Sub

    Private Sub OnLoadPublishedSeasonItemClick(sender As Object, e As ItemClickEventArgs)
        Dim index As PublishedSeasonIndex = e.Item.Tag
        If Not _Presenter.ReplaceWorkingSeasonFromIndex(index) Then
            ' TODO error
        End If
    End Sub

    Private Sub OnUpdatePublishedSeasonItemClick(sender As Object, e As ItemClickEventArgs)
        If DevExpress.XtraEditors.XtraMessageBox.Show("This season proposal already exists. Do you wish to overwrite?", "Overwrite Season Proposal", MessageBoxButtons.YesNoCancel) = DialogResult.Yes Then
            Dim index As PublishedSeasonIndex = e.Item.Tag
            SaveCurrentSeasonToServerAs(index)

            _CurrentlyAppliedSeason = _CurrentlyAppliedSeason
        End If
    End Sub

    Private Sub OnDeletePublishedSeasonItemClick(sender As Object, e As ItemClickEventArgs)
        Dim index As PublishedSeasonIndex = e.Item.Tag
        If DevExpress.XtraEditors.XtraMessageBox.Show($"Are you sure you want to delete ""{index.Name}""? This cannot be undone.", "Delete Season Proposal", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            DeleteSeasonFromServer(index)

            _CurrentlyAppliedSeason = _CurrentlyAppliedSeason
        End If
    End Sub

    Private Sub OnSaveAsNewSeason(sender As Object, e As ItemClickEventArgs)
        Dim fName = DevExpress.XtraEditors.XtraInputBox.Show("File Name", "Publish New Season", "Untitled")
        If fName = Nothing OrElse fName.Trim = Nothing Then
            Exit Sub
        End If
        Dim fNameActual = GetUniqueSeasonName(fName)

        Dim publishedSeasonIndex As PublishedSeasonIndex = Nothing

        If fName <> fNameActual Then
            Select Case DevExpress.XtraEditors.XtraMessageBox.Show($"Season '{fName}' already exists. Would you like to overwrite?", "Overwrite Existing Season", MessageBoxButtons.YesNoCancel)
                Case DialogResult.Yes
                    fNameActual = fName
                    publishedSeasonIndex = _Presenter.PublishedSeasonIndexes.ToList.Find(Function(x) x.Name.ToUpper.Trim = fNameActual.ToUpper.Trim)
                Case DialogResult.No
                    ' use fNameActual
                    publishedSeasonIndex = New PublishedSeasonIndex() With {.Name = fNameActual, .ftpPath = "seasons\" & Guid.NewGuid.ToString & ".dat"}
                Case Else
                    ' Do nothing
            End Select
        Else
            publishedSeasonIndex = New PublishedSeasonIndex() With {.Name = fNameActual, .ftpPath = "seasons\" & Guid.NewGuid.ToString & ".dat"}
        End If

        If publishedSeasonIndex IsNot Nothing Then
            SaveCurrentSeasonToServerAs(publishedSeasonIndex)
        End If

    End Sub

    Private Sub SaveCurrentSeasonToServerAs(publishedSeasonIndex As PublishedSeasonIndex)
        _Presenter.SaveSeason(publishedSeasonIndex)
        _CurrentlyAppliedSeason = publishedSeasonIndex
    End Sub

    Private Sub DeleteSeasonFromServer(publishedSeasonIndex As PublishedSeasonIndex)
        _Presenter.DeleteSeason(publishedSeasonIndex)
    End Sub

    Private Function GetUniqueSeasonName(fName As String) As String
        Dim testFName = fName
        Dim iterator = 1

        While _Presenter.PublishedSeasonIndexes.ToList.Find(Function(x) x.Name.ToUpper.Trim = testFName.ToUpper.Trim) IsNot Nothing
            testFName = fName & $" ({iterator})"
            iterator += 1
        End While

        Return testFName
    End Function

#End Region

#Region "Update Data From Presenter"

    Private Sub OnPresenterPropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _Presenter.PropertyChanged
        Select Case e.PropertyName
            Case NameOf(LibraryToolPresenter.IsAllowedClose)
                CloseFormIfAbleAndNeeded()
            Case NameOf(LibraryToolPresenter.LibraryHasChanges)
                UpdateFormTitle()
            Case NameOf(LibraryToolPresenter.Library)
                UpdateLibraryGridDataSource()
            Case NameOf(LibraryToolPresenter.Composers)
                UpdateComposersGridDataSource()
            Case NameOf(LibraryToolPresenter.Eras)
                UpdateErasGridDataSource()
            Case NameOf(LibraryToolPresenter.Tags)
                UpdateTagsGridDataSource()
            Case NameOf(LibraryToolPresenter.PublishedSeasonIndexes)
                UpdatePublishedSeasonsDataSource()
            Case NameOf(LibraryToolPresenter.WorkingSeasonInformation)
                UpdateWorkingSeasonsInfoDataSources()
            Case NameOf(LibraryToolPresenter.SeasonPlannerItems)
                UpdateSeasonPlannerItemsDataSources()

            Case NameOf(LibraryToolPresenter.LibraryIsLoaded), NameOf(LibraryToolPresenter.LibraryLoadError)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(LibraryTabIsVisible)))
                Me.TabNavigationPageLibrary.PageVisible = LibraryTabIsVisible

            Case NameOf(LibraryToolPresenter.ComposersAreLoaded),
                 NameOf(LibraryToolPresenter.ComposersLoadError),
                 NameOf(LibraryToolPresenter.ErasAreLoaded),
                 NameOf(LibraryToolPresenter.ErasLoadError),
                 NameOf(LibraryToolPresenter.TagsAreLoaded),
                 NameOf(LibraryToolPresenter.TagsLoadError)

                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(MetadataTabIsVisible)))
                Me.TabNavigationPageMetadata.PageVisible = MetadataTabIsVisible

            Case NameOf(LibraryToolPresenter.SeasonPlannerListIsLoaded),
                 NameOf(LibraryToolPresenter.SeasonPlannerListLoadError),
                 NameOf(LibraryToolPresenter.WorkingSeasonIsLoaded),
                 NameOf(LibraryToolPresenter.WorkingSeasonLoadError),
                 NameOf(LibraryToolPresenter.SeasonIndexesAreLoaded),
                 NameOf(LibraryToolPresenter.SeasonIndexesLoadError)

                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SeasonPlanningTabIsVisible)))
                Me.TabNavigationPageSeasonPlanner.PageVisible = SeasonPlanningTabIsVisible

        End Select
    End Sub

    Private Sub UpdateFormTitle()
        Static _OriginalText As String = Nothing
        If _OriginalText = Nothing Then
            _OriginalText = Me.Text
        End If

        If _Presenter.LibraryHasChanges Then
            Me.Text = _OriginalText & " *"
        Else
            Me.Text = _OriginalText
        End If
    End Sub

    Private Sub UpdateLibraryGridDataSource()
        _Context.Post(Sub(state)
                          GridControlLibrary.BeginUpdate()
                          GridControlLibrary.DataSource = _Presenter.Library
                          If _Presenter?.Library IsNot Nothing Then
                              GridViewLibrary.Columns("Composer").SortIndex = 0
                              GridViewLibrary.Columns("Title").SortIndex = 1
                          End If
                          GridControlLibrary.EndUpdate()

                          GridViewLibrary.BestFitColumns()

                          ApplyUserGridSettings_Library()
                      End Sub, Nothing)
    End Sub

    Private Sub UpdateComposersGridDataSource()
        _Context.Post(Sub(state)
                          GridControlComposers.BeginUpdate()
                          GridControlComposers.DataSource = _Presenter.Composers
                          If _Presenter?.Composers IsNot Nothing Then
                              GridViewComposers.Columns("PrimaryName").SortIndex = 0
                              GridViewComposers.Columns("Era").Visible = False
                              GridViewComposers.Columns("MetadataId").Visible = False
                          End If
                          GridControlComposers.EndUpdate()

                          GridViewComposers.BestFitColumns()

                          ApplyUserGridSettings_Composers()
                      End Sub, Nothing)
    End Sub

    Private Sub UpdateErasGridDataSource()
        _Context.Post(Sub(state)
                          ErasTypeConverter.StandardEras = _Presenter.Eras

                          GridControlEras.BeginUpdate()
                          GridControlEras.DataSource = _Presenter.Eras
                          If _Presenter?.Eras IsNot Nothing Then
                              GridViewEras.Columns("Name").SortIndex = 0
                          End If
                          GridControlEras.EndUpdate()

                          GridViewEras.BestFitColumns()

                          ApplyUserGridSettings_Eras()
                      End Sub, Nothing)
    End Sub

    Private Sub UpdateTagsGridDataSource()
        _Context.Post(Sub(state)
                          TagsTypeConverter.StandardTags = _Presenter.Tags

                          GridControlTags.BeginUpdate()
                          GridControlTags.DataSource = _Presenter.Tags
                          If _Presenter?.Tags IsNot Nothing Then
                              GridViewTags.Columns("Name").SortIndex = 0
                          End If
                          GridControlTags.EndUpdate()

                          GridViewTags.BestFitColumns()

                          ApplyUserGridSettings_Tags()
                      End Sub, Nothing)
    End Sub

    Private Sub UpdatePublishedSeasonsDataSource()
        AddHandler _Presenter.PublishedSeasonIndexes.ListChanged, AddressOf OnPublishedSeasonsListChanged
        UpdatePublishedSeasonsMenu()
    End Sub

    Private Sub UpdateWorkingSeasonsInfoDataSources()
        _Context.Post(Sub(state)
                          Dim concertGrids As New List(Of ConcertGrid) From {ConcertGrid1, ConcertGrid2, ConcertGrid3, ConcertGrid4, ConcertGrid5, ConcertGrid6}

                          For i As Integer = 0 To concertGrids.Count - 1
                              If _Presenter.WorkingSeasonInformation.WorkingSeasonInformation.ConcertInformations.Count <= i Then
                                  _Presenter.WorkingSeasonInformation.WorkingSeasonInformation.ConcertInformations.Add(New ConcertInformation)
                              End If

                              Dim concertInfo = _Presenter.WorkingSeasonInformation.WorkingSeasonInformation.ConcertInformations(i)
                              concertGrids(i).InitializeInfo(concertInfo, _Presenter.Eras, _Presenter.Tags)
                          Next

                          _CurrentlyAppliedSeason = _Presenter.WorkingSeasonInformation.WorkingSeasonIndex

                          Me.TokenEditEras.Properties.DataSource = _Presenter.Eras
                          Me.TokenEditEras.Properties.DisplayMember = "Name"
                          Me.TokenEditEras.Properties.ValueMember = "Name"
                          Me.TokenEditEras.Properties.Separators.Add(";")

                          AddHandler _Presenter.WorkingSeasonInformation.PropertyChanged, AddressOf OnWorkingSeasonInformationPropertyChanged
                          SetEras()

                          ApplyUserGridSettings_Concerts()
                      End Sub, Nothing)
    End Sub

    Private Sub OnWorkingSeasonInformationPropertyChanged(sender As Object, e As PropertyChangedEventArgs)
        SetEras()
    End Sub

    Private Sub SetEras()
        Dim eraString = _Presenter.WorkingSeasonInformation.Eras.GetErasTokenString()
        If eraString Is Nothing OrElse eraString.Trim = Nothing Then
            LayoutControlItemSeasonEras.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never
        Else
            LayoutControlItemSeasonEras.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always
        End If

        Me.TokenEditEras.EditValue = eraString
    End Sub

    Private Sub TokenEditEras_CustomDrawTokenBackground(sender As Object, e As DevExpress.XtraEditors.TokenEditCustomDrawTokenBackgroundEventArgs) Handles TokenEditEras.CustomDrawTokenBackground
        e.Cache.FillRoundedRectangle(Color.FromArgb(40, GetColorForEraDescription(e.Description)), e.Bounds, New DevExpress.Utils.Drawing.CornerRadius(4))
    End Sub

    Private Function GetColorForEraDescription(era As String) As Color
        Dim foundEra = _Presenter.Eras.Find(era)
        If foundEra IsNot Nothing AndAlso foundEra.Color <> Color.Transparent Then
            Return foundEra.Color
        End If

        Return Color.Gray
    End Function

    Private Sub UpdateSeasonPlannerItemsDataSources()
        _Context.Post(Sub(state)
                          GridControlSeasonPlanner.BeginUpdate()
                          GridControlSeasonPlanner.DataSource = _Presenter.SeasonPlannerItems
                          If _Presenter?.SeasonPlannerItems IsNot Nothing Then
                              GridViewSeasonPlanner.Columns("Recommendation").Visible = False
                              GridViewSeasonPlanner.Columns("Recommendation").OptionsColumn.ShowInCustomizationForm = False
                          End If
                          GridControlSeasonPlanner.EndUpdate()

                          GridViewSeasonPlanner.BestFitColumns()

                          UpdateSeasonPlannerHeading()

                          ApplyUserGridSettings_Seasons()
                      End Sub, Nothing)
    End Sub

    Private Sub OnSeasonPlannerItemsListChanged(sender As Object, e As ListChangedEventArgs) Handles _Presenter.SeasonPlannerListChanged
        _Context.Post(Sub(state)
                          UpdateSeasonPlannerHeading()
                      End Sub, Nothing)
    End Sub

    Private Sub UpdateSeasonPlannerHeading()

        Dim newText = "Season Planner List"
        Dim totalCount As Integer = _Presenter.GetTotalSeasonPlannerItemCount
        Dim hiddenCount As Integer = _Presenter.GetHiddenSeasonPlannerItemCount

        If totalCount > 0 Then
            Dim totalText = If(totalCount > 0, $"{totalCount} items", "")
            Dim hiddenText = If(hiddenCount > 0, $"{hiddenCount} in season", "")
            Dim texts = {totalText, hiddenText}.ToList.FindAll(Function(x) x <> Nothing)
            newText &= $" ({String.Join(", ", texts)})"
        End If

        LayoutControlGroupSeasonPlanningList.Text = newText
    End Sub

#End Region

#Region "Progress Bar"

    Private WithEvents _timerHideProgress As New Windows.Forms.Timer() With {.Interval = 3000}

    Private Sub OnPresenterProgressChanged(sender As Object, e As ProgressBarEventArgs) Handles _Presenter.ProgressChanged
        If e.Visible Then
            If e.IsMarquee Then
                ShowProgresss(e.Minimum, e.Maximum, e.Value, e.Caption)
            Else
                ShowMarqueeProgress(e.Caption)
            End If

        Else
            HideProgress(3000)
        End If
    End Sub

    Private Sub ShowProgresss(minimum As Integer?, maximum As Integer?, value As Integer?, caption As String)

        _Context.Post(Sub(state)
                          _timerHideProgress.Stop()

                          If Not Me.Visible Then Exit Sub

                          BarManager1.BeginUpdate()

                          If minimum IsNot Nothing AndAlso Not Me.RepositoryItemProgressBar1.Minimum = minimum Then
                              Me.RepositoryItemProgressBar1.Minimum = minimum
                          End If
                          If maximum IsNot Nothing AndAlso Not Me.RepositoryItemProgressBar1.Maximum = maximum Then
                              Me.RepositoryItemProgressBar1.Maximum = maximum
                          End If
                          If value IsNot Nothing AndAlso Not Me.BarEditItemProgressBar.EditValue = value Then
                              Me.BarEditItemProgressBar.EditValue = value
                          End If
                          If caption IsNot Nothing AndAlso Not Me.BarStaticItemLoadingCaption.Caption = caption Then
                              Me.BarStaticItemLoadingCaption.Caption = caption
                          End If
                          If Not Me.BarEditItemProgressBar.Visibility = BarItemVisibility.Always Then
                              Me.BarEditItemProgressBar.Visibility = BarItemVisibility.Always
                          End If
                          If Not Me.BarEditItemMarqueeProgressBar.Visibility = BarItemVisibility.Never Then
                              Me.BarEditItemMarqueeProgressBar.Visibility = BarItemVisibility.Never
                          End If
                          If Not Me.BarStaticItemLoadingCaption.Visibility = BarItemVisibility.Always Then
                              Me.BarStaticItemLoadingCaption.Visibility = BarItemVisibility.Always
                          End If

                          BarManager1.EndUpdate()
                      End Sub, Nothing)
    End Sub

    Private Sub ShowMarqueeProgress(caption As String)

        _Context.Post(Sub(state)
                          _timerHideProgress.Stop()

                          If Not Me.Visible Then Exit Sub

                          BarManager1.BeginUpdate()

                          If caption IsNot Nothing AndAlso Not Me.BarStaticItemLoadingCaption.Caption = caption Then
                              Me.BarStaticItemLoadingCaption.Caption = caption
                          End If
                          If Not Me.BarEditItemProgressBar.Visibility = BarItemVisibility.Never Then
                              Me.BarEditItemProgressBar.Visibility = BarItemVisibility.Never
                          End If
                          If Not Me.BarEditItemMarqueeProgressBar.Visibility = BarItemVisibility.Always Then
                              Me.BarEditItemMarqueeProgressBar.Visibility = BarItemVisibility.Always
                          End If
                          If Not Me.BarStaticItemLoadingCaption.Visibility = BarItemVisibility.Always Then
                              Me.BarStaticItemLoadingCaption.Visibility = BarItemVisibility.Always
                          End If

                          BarManager1.EndUpdate()
                      End Sub, Nothing)
    End Sub

    Private Sub HideProgress(delayInMs As Integer)
        _Context.Post(Sub(state)
                          _timerHideProgress.Stop()

                          If Not Me.Visible Then Exit Sub

                          If delayInMs > 0 Then
                              _timerHideProgress.Interval = delayInMs
                              _timerHideProgress.Start()
                          Else
                              HideProgress()
                          End If
                      End Sub, Nothing)
    End Sub

    Private Sub HideProgress() Handles _timerHideProgress.Tick
        _Context.Post(Sub(state)
                          _timerHideProgress.Stop()

                          If Not Me.Visible Then Exit Sub

                          BarManager1.BeginUpdate()

                          If Not Me.BarEditItemProgressBar.Visibility = BarItemVisibility.Never Then
                              Me.BarEditItemProgressBar.Visibility = BarItemVisibility.Never
                          End If
                          If Not Me.BarEditItemMarqueeProgressBar.Visibility = BarItemVisibility.Never Then
                              Me.BarEditItemMarqueeProgressBar.Visibility = BarItemVisibility.Never
                          End If
                          If Not Me.BarStaticItemLoadingCaption.Visibility = BarItemVisibility.Never Then
                              Me.BarStaticItemLoadingCaption.Visibility = BarItemVisibility.Never
                          End If

                          BarManager1.EndUpdate()
                      End Sub, Nothing)
    End Sub

#End Region

#Region "Security"

    Private Function LoadOrPromptPassword() As Boolean
        If Not Ftp.ValidatePasscode(My.Settings.Passcode) Then
            Dim fPass As New PasscodeForm()
            If Not fPass.ShowDialog Then
                DevExpress.XtraEditors.XtraMessageBox.Show("Passcode could not be validated. Application will exit...")
                Me.Close()
                Return False
            End If
        End If
        Return True
    End Function

#End Region

#Region "Merge and Edit Entries"

    Private Sub MergeSelectionsFromComposers()
        Dim selectedComposers As New List(Of ComposerAlias)
        If GridViewComposers.GetSelectedRows.Count > 0 Then
            For Each selectedRowHandle In GridViewComposers.GetSelectedRows
                selectedComposers.Add(GridViewComposers.GetRow(selectedRowHandle))
            Next

            Dim fMerge As New ComposerMergeForm
            fMerge.Initialize(selectedComposers)
            fMerge.ShowDialog()

            If fMerge.MergedItem IsNot Nothing Then
                _Presenter.ReplaceAliases(selectedComposers, fMerge.MergedItem)

                Dim handle = GridViewComposers.GetRowHandle(_Presenter.Composers.IndexOf(fMerge.MergedItem))
                GridViewComposers.FocusedRowHandle = handle
            End If

        End If
    End Sub

    Private Sub MergeSelectionsFromMusicList()
        Dim selections As New List(Of Recommendation)
        If GridViewLibrary.GetSelectedRows.Count > 0 Then
            For Each selectedRowHandle In GridViewLibrary.GetSelectedRows
                selections.Add(GridViewLibrary.GetRow(selectedRowHandle))
            Next

            Dim fMerge As New MergeForm
            fMerge.Initialize(selections, _Presenter.Composers, _Presenter.Eras, _Presenter.Tags)
            fMerge.ShowDialog()

            If fMerge.MergedItem IsNot Nothing Then
                _Presenter.ReplaceLibraryEntries(selections, fMerge.MergedItem)
                Dim handle = GridViewLibrary.GetRowHandle(_Presenter.Library.IndexOf(fMerge.MergedItem))
                GridViewLibrary.FocusedRowHandle = handle
            End If
            If fMerge.MergedAlias IsNot Nothing Then
                _Presenter.ReplaceAliases(fMerge.OriginalComposerAliases, fMerge.MergedAlias)
            End If

        End If
    End Sub

#End Region

#Region "Import Data from CSV"

    Private Sub LoadCsv(fileName As String)
        Select Case DetermineCsvFormat(fileName)
            Case CsvFormats.Holdings
                LoadHoldingsCsv(fileName)
            Case CsvFormats.Recommendation
                LoadRecommendationCsv(fileName)
            Case CsvFormats.Repertoire
                LoadRepertoireCsv(fileName)
        End Select
    End Sub

    Private Function DetermineCsvFormat(fileName As String) As CsvFormats
        Try
            Dim csvText = IO.File.ReadAllText(fileName)

            For Each line In Csv.CsvReader.ReadFromText(csvText, New CsvOptions() With {.AllowNewLineInEnclosedFieldValues = True})
                If line.Headers.Count = 20 AndAlso line.Headers(0) = "Mark" Then
                    Return CsvFormats.Holdings
                ElseIf line.Headers.Count = 19 AndAlso line.Headers(0) = "Timestamp" Then
                    Return CsvFormats.Recommendation
                ElseIf line.Headers.Count = 3 AndAlso line.Headers(0) = "Piece" Then
                    Return CsvFormats.Repertoire
                End If

            Next

        Catch ex As IO.IOException
            DevExpress.XtraEditors.XtraMessageBox.Show("The selected file could not be opened, if it is opened in Excel, please close it before importing.", "Import Error")
        End Try
        Return CsvFormats.None

    End Function

    Private Sub LoadHoldingsCsv(fileName As String)
        Dim csvText = IO.File.ReadAllText(fileName)
        Dim holdings As New List(Of HoldingEntry)

        For Each line In Csv.CsvReader.ReadFromText(csvText, New CsvOptions() With {.AllowNewLineInEnclosedFieldValues = True})
            Dim holding As HoldingEntry = HoldingEntry.GetEntryFromCsvLine(line)
            holdings.Add(holding)
        Next

        _Presenter.MergeHoldingsIntoLibrary(holdings)
    End Sub

    Private Sub LoadRepertoireCsv(fileName As String)
        Dim csvText = IO.File.ReadAllText(fileName)
        Dim repertoires As New List(Of RepertoireEntry)

        For Each line In Csv.CsvReader.ReadFromText(csvText, New CsvOptions() With {.AllowNewLineInEnclosedFieldValues = True})
            Dim mRec As RepertoireEntry = RepertoireEntry.GetEntryFromCsvLine(line)
            repertoires.Add(mRec)
        Next

        _Presenter.MergeRepertoireIntoLibrary(repertoires)
    End Sub

    Private Sub LoadRecommendationCsv(fileName As String)
        Dim csvText = IO.File.ReadAllText(fileName)
        Dim recommendations As New List(Of MusicianRecommendation)

        For Each line In Csv.CsvReader.ReadFromText(csvText, New CsvOptions() With {.AllowNewLineInEnclosedFieldValues = True})
            Dim mRecs As IEnumerable(Of MusicianRecommendation) = MusicianRecommendation.GetRecommendationsFromCsvLine(line)
            recommendations.AddRange(mRecs)
        Next

        _Presenter.MergeRecommendationsIntoLibrary(recommendations)
    End Sub

#End Region

#Region "Error Messages"

    Private WithEvents _timerHideStatusBarMessage As New Windows.Forms.Timer() With {.Interval = 10000}
    Private _StatusBarMessageQueue As New Queue(Of (Message As String, Interval As Integer, IsError As Boolean))

    Private Sub OnPresenterError(sender As Object, args As ErrorCodeEventArgs) Handles _Presenter.ErrorOccurred
        _Context.Post(Sub(e As ErrorCodeEventArgs)
                          ShowErrorThreadUnsafe(e)
                      End Sub, args)
    End Sub

    Private Sub ShowErrorThreadUnsafe(e As ErrorCodeEventArgs)
        Dim messageCaption As String = ""
        Dim messageText As String = ""
        Dim buttons As MessageBoxButtons = MessageBoxButtons.OK
        Dim actions As New Dictionary(Of DialogResult, Action)

        Select Case e.ErrorCode
            Case ErrorCodeEventArgs.ErrorCodes.LibraryFailedToLoadFromServer
                messageCaption = "Library Load Failure"
                messageText = "The library file could not be downloaded. Try again?"
                buttons = MessageBoxButtons.RetryCancel


                actions.Add(DialogResult.Retry, Sub()
                                                    e.Critical = False
                                                    InitializeData()
                                                End Sub)

                actions.Add(DialogResult.Cancel, Sub()
                                                     End
                                                 End Sub)


            Case ErrorCodeEventArgs.ErrorCodes.PublishedSeasonIndexFailedToDownload
                messageCaption = "Season Proposal Load Error"
                messageText = "The list of published season proposals could not be downloaded."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.LoadSeasonProposalFromIndexFailedBecauseTheFileFailedToDownload
                messageCaption = "Season Proposal Load Error"
                messageText = "Could not load season because the file does not exist."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.LoadSeasonProposalFromIndexFailedBecauseTheFileDoesNotExist
                messageCaption = "Season Proposal Load Error"
                messageText = "Could not load season because the file failed to download."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.LoadSeasonProposalFromIndexFailedUnknownReason
                messageCaption = "Season Proposal Load Error"
                messageText = "Could not load season for an unknown reason."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.PublishSeasonProposalFailedBecauseTheFileFailedToUpload
                messageCaption = "Season Proposal Save Error"
                messageText = "Could not publish season proposal because the file failed to upload."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.PublishSeasonProposalFailedBecauseTheTableOfContentsFailedToUpload
                messageCaption = "Season Proposal Save Error"
                messageText = "Could not publish season proposal because the Table of Contents file failed to upload."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.PublishSeasonProposalFailedUnknownReason
                messageCaption = "Season Proposal Save Error"
                messageText = "Could not publish season proposal for an unknown reason."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.DeleteSeasonProposalFailedBecauseTheTableOfContentsFailedToUpload
                messageCaption = "Season Proposal Delete Error"
                messageText = "Could not delete season proposal because the Table of Contents file failed to upload."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.DeleteSeasonProposalFailedUnknownReason
                messageCaption = "Season Proposal Delete Error"
                messageText = "Could not delete season proposal for an unknown reason."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.SaveLibraryFailedBecauseRemoteFileCouldNotBeReplaced
                messageCaption = "Library Save Error"
                messageText = "Library upload failed because remote file could not be renamed."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.SaveLibraryFailedBecauseFileCouldNotBeUploaded
                messageCaption = "Library Save Error"
                messageText = "Library update failed because file could not be uploaded."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.SaveLibraryFailedForAnUnknownReason
                messageCaption = "Library Save Error"
                messageText = "Library update failed for an unknown reason."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.SaveLibraryFailedBecauseTheRemoteFileIsNewer
                messageCaption = "Library Save Error"
                messageText = "Library update failed because the remote library is newer." & Environment.NewLine & Environment.NewLine & "Would you like to save a copy locally?"
                buttons = MessageBoxButtons.YesNo

                actions.Add(DialogResult.Yes, Sub()
                                                  Dim sfd As New SaveFileDialog
                                                  sfd.FileName = $"library_{Now:yyyyMMddHHmmssfff}_export.dat"
                                                  If sfd.ShowDialog = DialogResult.OK Then
                                                      IO.File.Copy(LocalPath_Library, sfd.FileName)
                                                  End If
                                              End Sub)

            Case ErrorCodeEventArgs.ErrorCodes.CouldNotLoadUserSeasonPlanningList
                messageCaption = "Season Planning Error"
                messageText = "Could not load user season planning list."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.CouldNotSaveUserSeasonPlanningList
                messageCaption = "Season Planning Error"
                messageText = "Could not save user season planning list."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.CouldNotCreateNewUserWorkingSeasonData
                messageCaption = "Season Planning Error"
                messageText = "Could not create new user season concert data."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.CouldNotLoadUserWorkingSeasonData
                messageCaption = "Season Planning Error"
                messageText = "Could not load user season concert data."
                buttons = MessageBoxButtons.OK
            Case ErrorCodeEventArgs.ErrorCodes.CouldNotSaveUserWorkingSeasonData
                messageCaption = "Season Planning Error"
                messageText = "Could not save user season concert data."
                buttons = MessageBoxButtons.OK
        End Select

        Select Case e.ShowMode
            Case ErrorCodeEventArgs.ShowModes.Modal
                Dim result = DevExpress.XtraEditors.XtraMessageBox.Show(messageText, messageCaption, buttons)
                If actions.ContainsKey(result) Then
                    actions(result).Invoke()
                End If
            Case ErrorCodeEventArgs.ShowModes.StatusBar
                QueueStatusBarMessage(messageText, True)
        End Select

        If e.Critical Then
            End
        End If
    End Sub

    Private Sub QueueStatusBarMessage(messageText As String, isError As Boolean, Optional intervalMs As Integer = 10000)
        _StatusBarMessageQueue.Enqueue((messageText, Math.Max(1, intervalMs), isError))
        ProcessStatusBarMessageQueue()
    End Sub

    Private Sub ProcessStatusBarMessageQueue()
        If Not _timerHideStatusBarMessage.Enabled Then
            _timerHideStatusBarMessage.Start()
            SetNextStatusBarMessage()
        End If
    End Sub

    Private Sub SetNextStatusBarMessage()
        _timerHideStatusBarMessage.Stop()
        If _StatusBarMessageQueue.Count > 0 Then
            Me.BarStaticItemMessage.Visibility = BarItemVisibility.Always
            Dim nextMessage = _StatusBarMessageQueue.Dequeue

            If nextMessage.IsError Then
                Me.BarStaticItemMessage.ImageOptions.SvgImage = _ErrorIcon
            Else
                Me.BarStaticItemMessage.ImageOptions.SvgImage = Nothing
            End If
            Me.BarStaticItemMessage.Caption = nextMessage.Message
            _timerHideStatusBarMessage.Interval = nextMessage.Interval

            _timerHideStatusBarMessage.Start()
        Else
            Me.BarStaticItemMessage.Visibility = BarItemVisibility.Never
            Me.BarStaticItemMessage.Caption = Nothing
        End If
    End Sub

    Private Sub OnHideStatusBarMessageTimerTick(sender As Object, e As EventArgs) Handles _timerHideStatusBarMessage.Tick
        SetNextStatusBarMessage()
    End Sub

#End Region

#Region "User Customization"

    Private Sub SaveUserSettings()

        If _Presenter.LibraryIsLoaded Then
            Using fs As New IO.FileStream(LocalPath_RecommendationGridSettings, IO.FileMode.Create)
                GridViewLibrary.SaveLayoutToStream(fs)
            End Using
            Using fs As New IO.FileStream(LocalPath_ComposerGridSettings, IO.FileMode.Create)
                GridViewComposers.SaveLayoutToStream(fs)
            End Using
            Using fs As New IO.FileStream(LocalPath_ErasGridSettings, IO.FileMode.Create)
                GridViewEras.SaveLayoutToStream(fs)
            End Using
            Using fs As New IO.FileStream(LocalPath_TagsGridSettings, IO.FileMode.Create)
                GridViewTags.SaveLayoutToStream(fs)
            End Using
        End If

        If _Presenter.SeasonPlannerListIsLoaded Then
            Using fs As New IO.FileStream(LocalPath_SeasonPlanningGridSettings, IO.FileMode.Create)
                GridViewSeasonPlanner.SaveLayoutToStream(fs)
            End Using
        End If

        If _Presenter.WorkingSeasonIsLoaded Then
            For Each concertControl In GetConcertGrids()
                concertControl.SaveLayout()
            Next
            _Presenter.SaveWorkingSeasonInformation(_CurrentlyAppliedSeason, _Presenter.WorkingSeasonInformation.WorkingSeasonInformation)
        End If

    End Sub

    Private Sub ApplyUserGridSettings_Library()
        If IO.File.Exists(LocalPath_RecommendationGridSettings) Then
            Using fs As New IO.FileStream(LocalPath_RecommendationGridSettings, IO.FileMode.Open)
                Try
                    GridViewLibrary.RestoreLayoutFromStream(fs)
                Catch ex As Exception
                End Try
            End Using
        End If
    End Sub

    Private Sub ApplyUserGridSettings_Composers()
        If IO.File.Exists(LocalPath_ComposerGridSettings) Then
            Using fs As New IO.FileStream(LocalPath_ComposerGridSettings, IO.FileMode.Open)
                Try
                    GridViewComposers.RestoreLayoutFromStream(fs)
                    GridViewComposers.OptionsView.ShowGroupPanel = False
                Catch ex As Exception
                End Try
            End Using
        End If
    End Sub

    Private Sub ApplyUserGridSettings_Eras()
        If IO.File.Exists(LocalPath_ErasGridSettings) Then
            Using fs As New IO.FileStream(LocalPath_ErasGridSettings, IO.FileMode.Open)
                Try
                    GridViewEras.RestoreLayoutFromStream(fs)
                    GridViewEras.OptionsView.ShowGroupPanel = False
                Catch ex As Exception
                End Try
            End Using
        End If
    End Sub

    Private Sub ApplyUserGridSettings_Tags()
        If IO.File.Exists(LocalPath_TagsGridSettings) Then
            Using fs As New IO.FileStream(LocalPath_TagsGridSettings, IO.FileMode.Open)
                Try
                    GridViewTags.RestoreLayoutFromStream(fs)
                    GridViewTags.OptionsView.ShowGroupPanel = False
                Catch ex As Exception
                End Try
            End Using
        End If
    End Sub

    Private Sub ApplyUserGridSettings_Seasons()
        If IO.File.Exists(LocalPath_SeasonPlanningGridSettings) Then
            Using fs As New IO.FileStream(LocalPath_SeasonPlanningGridSettings, IO.FileMode.Open)
                Try
                    GridViewSeasonPlanner.RestoreLayoutFromStream(fs)
                Catch ex As Exception
                End Try
            End Using
        End If
    End Sub

    Private Sub ApplyUserGridSettings_Concerts()
        For Each concertControl In GetConcertGrids()
            concertControl.RestoreLayout()
        Next
    End Sub

#End Region

#Region "Grids"

    Private _ContextMenuItem As Recommendation

    Private Sub GridViewLibrary_PopupMenuShowing(sender As Object, e As PopupMenuShowingEventArgs) Handles GridViewLibrary.PopupMenuShowing
        If e.HitInfo.InGroupRow Then
            If e.Menu Is Nothing Then
                Dim newMenu As New DevExpress.XtraGrid.Menu.GridViewMenu(GridViewLibrary)
                e.Menu = newMenu
            End If

            Dim expandAllItem As New DevExpress.Utils.Menu.DXMenuItem("Expand All")
            AddHandler expandAllItem.Click, Sub() GridViewLibrary.ExpandAllGroups()

            Dim collapseAllItem As New DevExpress.Utils.Menu.DXMenuItem("Collapse All")
            AddHandler collapseAllItem.Click, Sub() GridViewLibrary.CollapseAllGroups()

            e.Menu.Items.Add(expandAllItem)
            e.Menu.Items.Add(collapseAllItem)
        ElseIf e.HitInfo.InRow Then
            Dim contextMenuItem As Recommendation = GridViewLibrary.GetRow(e.HitInfo.RowHandle)
            If contextMenuItem Is Nothing Then Exit Sub

            _ContextMenuItem = contextMenuItem

            Dim isChecked As Boolean = _Presenter.GetSeasonPlannerContainsItem(_ContextMenuItem)
            Dim addToSeasonItem As New DevExpress.Utils.Menu.DXMenuCheckItem("Include Season Planner", isChecked)
            AddHandler addToSeasonItem.CheckedChanged, AddressOf OnSeasonItemCheckedChangedHandler
            e.Menu.Items.Add(addToSeasonItem)
        End If
    End Sub

    Private Sub OnSeasonItemCheckedChangedHandler(sender As Object, e As EventArgs)
        Dim menuItem As DevExpress.Utils.Menu.DXMenuCheckItem = sender
        Select Case menuItem.Checked
            Case True
                _Presenter.AddSeasonPlannerItem(_ContextMenuItem)
            Case False
                _Presenter.RemoveSeasonPlannerItem(_ContextMenuItem)
        End Select
    End Sub

    Private Sub GridViewEras_InitNewRow(sender As Object, e As InitNewRowEventArgs) Handles GridViewEras.InitNewRow
        Dim era As Era = GridViewEras.GetRow(e.RowHandle)
        If era IsNot Nothing Then
            era.Name = "Untitled"
            Dim userResponse = DevExpress.XtraEditors.XtraInputBox.Show("Enter the new Era name", "Create New Era", "")

            If userResponse IsNot Nothing AndAlso userResponse.Trim <> Nothing Then
                era.Name = userResponse.Trim
            End If
        End If
    End Sub

    Private Sub GridViewTags_InitNewRow(sender As Object, e As InitNewRowEventArgs) Handles GridViewTags.InitNewRow
        Dim tag As Tag = GridViewTags.GetRow(e.RowHandle)
        If tag IsNot Nothing Then
            tag.Name = "Untitled"
            Dim userResponse = DevExpress.XtraEditors.XtraInputBox.Show("Enter the new Tag name", "Create New Tag", "")

            If userResponse IsNot Nothing AndAlso userResponse.Trim <> Nothing Then
                tag.Name = userResponse.Trim
            End If
        End If
    End Sub


    Private Sub GridViewLibrary_CustomRowCellEdit(sender As Object, e As CustomRowCellEditEventArgs) Handles GridViewLibrary.CustomRowCellEdit
        If e.Column.FieldName = NameOf(Recommendation.Era) Then
            Dim edit = New DevExpress.XtraEditors.Repository.RepositoryItemComboBox()
            Dim erasList = _Presenter.Eras.ToList
            erasList.Sort(Function(x, y) If(x.Name, String.Empty).CompareTo(If(y.Name, String.Empty)))

            edit.AllowNullInput = DevExpress.Utils.DefaultBoolean.True
            edit.Items.Add(New Era(""))

            edit.Items.AddRange(erasList)
            edit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor

            e.RepositoryItem = edit
        ElseIf e.Column.FieldName = NameOf(Recommendation.TagsString) OrElse e.Column.FieldName = NameOf(Recommendation.Tags) Then
            Dim edit = New DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit()
            Dim tagsList = _Presenter.Tags.ToList
            tagsList.Sort(Function(x, y) If(x.Name, String.Empty).CompareTo(If(y.Name, String.Empty)))

            For Each tagItem In tagsList
                edit.Items.Add(tagItem)
            Next

            edit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor

            e.RepositoryItem = edit
        End If
    End Sub

    Private Sub GridViewEras_CustomDrawRowIndicator(sender As Object, e As RowIndicatorCustomDrawEventArgs) Handles GridViewLibrary.CustomDrawRowIndicator, GridViewComposers.CustomDrawRowIndicator, GridViewEras.CustomDrawRowIndicator, GridViewTags.CustomDrawRowIndicator
        Dim row As ISupportHasChanges = DirectCast(sender, GridView).GetRow(e.RowHandle)
        If row IsNot Nothing AndAlso row.HasChanges AndAlso e.Info.ImageIndex <> 0 Then
            e.Info.ImageIndex = 2
        End If
    End Sub

    Private Sub GridViewLibrary_CustomDrawCell(sender As Object, e As Views.Base.RowCellCustomDrawEventArgs) Handles GridViewLibrary.CustomDrawCell
        If e.Column.FieldName = NameOf(Recommendation.LastPerformed) Then
            If e.CellValue = 0 Then
                e.DisplayText = String.Empty
            End If
        ElseIf e.Column.FieldName = NameOf(Recommendation.Length) Then
            If e.CellValue Is Nothing OrElse DirectCast(e.CellValue, TimeSpan) = TimeSpan.Zero Then
                e.DisplayText = String.Empty
            End If
        End If
    End Sub

    Private Sub GridViewSeasonPlanner_CustomDrawCell(sender As Object, e As Views.Base.RowCellCustomDrawEventArgs) Handles GridViewSeasonPlanner.CustomDrawCell
        If e.Column.FieldName = NameOf(SeasonItem.Length) Then
            If e.CellValue Is Nothing OrElse DirectCast(e.CellValue, TimeSpan) = TimeSpan.Zero Then
                e.DisplayText = String.Empty
            End If
        End If
    End Sub

#End Region

#Region "Metadata Fetch"

    Private _ComposerMetadatas As New ComposerMetadatas
    Private _WorkMetadatas As New WorkMetadatas

    Private Sub BarButtonItemRefreshMetadataAll_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BarButtonItemRefreshMetadataAll.ItemClick
        For Each rec In _Presenter.Library
            rec.HasMetadataFromComposer = False
            rec.HasMetadataFromTitle = False
            Try
                PopulateMetadata(rec)
            Catch ex As Exception
                ' Cancel
                Exit For
            End Try
        Next
    End Sub

    Private Sub BarButtonItemRefreshMetadataSelected_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BarButtonItemRefreshMetadataSelected.ItemClick

        Dim selections As New List(Of Recommendation)
        If GridViewLibrary.GetSelectedRows.Count > 0 Then
            For Each selectedRowHandle In GridViewLibrary.GetSelectedRows
                selections.Add(GridViewLibrary.GetRow(selectedRowHandle))
            Next
        End If
        If selections.Count > 0 Then
            For Each rec In selections
                rec.HasMetadataFromComposer = False
                rec.HasMetadataFromTitle = False
                Try
                    PopulateMetadata(rec)
                Catch ex As Exception
                    ' Cancel
                    Exit For
                End Try
            Next
        End If
    End Sub

    Private Sub PopulateMetadata(rec As Recommendation)
        Dim composerMetadata As ComposerMetadata = Nothing
        Dim workMetadata As WorkMetadata = Nothing

        Try
            If rec.HasMetadataFromComposer Then Exit Sub
            Dim composer = _Presenter.Composers.FindComposer(rec)
            If composer IsNot Nothing Then
                rec.HasMetadataFromComposer = True

                composerMetadata = GetComposerMetadata(composer)
                If composerMetadata IsNot Nothing Then
                    If composerMetadata.Metadata Is Nothing Then Exit Sub

                    composer.MetadataId = composerMetadata.Metadata.id

                    If Not rec.HasMetadataFromTitle Then
                        rec.HasMetadataFromTitle = True

                        workMetadata = GetWorkMetadata(rec, composer)

                        If workMetadata IsNot Nothing Then
                            If workMetadata.Metadata Is Nothing Then Exit Sub

                            rec.MetadataId = workMetadata.Metadata.id
                        End If
                    End If
                End If
            End If

            If composerMetadata IsNot Nothing Then
                Dim foundEra = _Presenter.Eras.Find(composerMetadata.Metadata.epoch)

                Dim shouldReplaceEra As Boolean = True
                If composer.Era IsNot Nothing AndAlso composer.Era.Name <> Nothing Then
                    shouldReplaceEra = False
                    If Not composer.Era.Equals(foundEra) Then
                        Select Case DevExpress.XtraEditors.XtraMessageBox.Show($"Replace era {composer.Era.Name} with {foundEra.Name} for {composer.PrimaryName}?", "Replace Era?", MessageBoxButtons.YesNoCancel)
                            Case DialogResult.Yes
                                shouldReplaceEra = True
                            Case DialogResult.No
                                shouldReplaceEra = False
                            Case Else
                                ' Cancel
                                Throw New Exception("Cancellation")
                        End Select
                    End If
                End If
                If shouldReplaceEra Then composer.Era = foundEra
                If foundEra IsNot Nothing Then
                    rec.Era = composer.Era
                End If
            End If
            If workMetadata IsNot Nothing Then
                Dim shouldReplaceTitle As Boolean = True
                If rec.Title <> Nothing Then
                    shouldReplaceTitle = False
                    If Not rec.Title.Equals(workMetadata.Metadata.title) Then
                        Select Case DevExpress.XtraEditors.XtraMessageBox.Show($"Replace title '{rec.Title}' with '{workMetadata.Metadata.title}'?", "Replace Title?", MessageBoxButtons.YesNoCancel)
                            Case DialogResult.Yes
                                shouldReplaceTitle = True
                            Case DialogResult.No
                                shouldReplaceTitle = False
                            Case Else
                                ' Cancel
                                Throw New Exception("Cancellation")
                        End Select
                    End If
                End If
                If shouldReplaceTitle Then
                    Dim oldTitle = rec.Title
                    If Not rec.AlternateTitleSpellings.Contains(oldTitle) Then
                        rec.AlternateTitleSpellings.Add(oldTitle)
                    End If
                    rec.Title = workMetadata.Metadata.title
                Else
                    If Not rec.AlternateTitleSpellings.Contains(workMetadata.Metadata.title) Then
                        rec.AlternateTitleSpellings.Add(workMetadata.Metadata.title)
                    End If
                End If

                Dim foundTag = _Presenter.Tags.Find(workMetadata.Metadata.genre)

                If foundTag IsNot Nothing AndAlso rec.Tags IsNot Nothing Then
                    If rec.Tags.Find(foundTag) Is Nothing Then
                        rec.Tags.Add(foundTag)
                    End If
                End If
            End If
        Finally
            If workMetadata IsNot Nothing Then
                QueueStatusBarMessage($"Retrieved Metadata for {rec}", False, 3000)
            Else
                QueueStatusBarMessage($"No match found for {rec}", True, 10000)
            End If
        End Try

    End Sub

    Private Function GetComposerMetadata(composer As ComposerAlias) As ComposerMetadata
        Dim metadata = _ComposerMetadatas.GetMetadata(composer)
        If metadata Is Nothing Then Return Nothing

        _ComposerMetadatas.Add(metadata)
        Return metadata
    End Function

    Private Function GetWorkMetadata(rec As Recommendation, composer As ComposerAlias) As WorkMetadata
        Dim metadata = _WorkMetadatas.GetMetadata(rec, composer)
        If metadata Is Nothing Then Return Nothing

        _WorkMetadatas.Add(metadata)
        Return metadata
    End Function

    Private Sub BarButtonItem2_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BarButtonItemViewDocumentation.ItemClick
        Process.Start("https://docs.google.com/document/d/1H_WCUtz6nCjn5WMp1eDRa-292tDWeAw2KcZOby6VvTA/edit?usp=sharing")
    End Sub

    Private Sub BarButtonItemCheckForUpdate_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BarButtonItemCheckForUpdate.ItemClick
        CheckForUpdateAndApply(False)
    End Sub

    Private Sub CheckForUpdateAndApply(askToApply As Boolean)
        If Not Deployment.Application.ApplicationDeployment.IsNetworkDeployed Then Exit Sub

        Me.ShowMarqueeProgress("Checking for update…")
        Dim client As New Net.WebClient()
        client.CachePolicy = New Net.Cache.RequestCachePolicy(Net.Cache.RequestCacheLevel.NoCacheNoStore)
        client.DownloadFile("https://joedombrowski.com/apps/hcs-library-tool/HcsLibraryTool.application", "test.application")

        Dim progressIsMarquee As Boolean = True
        Dim inPlaceHostingManager = New InPlaceHostingManager(Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateLocation, False)
        AddHandler inPlaceHostingManager.GetManifestCompleted,
            Sub(sender2 As Object, e2 As GetManifestCompletedEventArgs)
                HideProgress()

                If e2.Version > Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion Then
                    If askToApply Then
                        If DevExpress.XtraEditors.XtraMessageBox.Show("A new version of this application was found. Would you like to update?", "Update Available", MessageBoxButtons.YesNo) <> DialogResult.Yes Then
                            Exit Sub
                        End If
                    End If

                    DirectCast(sender2, InPlaceHostingManager).AssertApplicationRequirements(True)
                    progressIsMarquee = False
                    DirectCast(sender2, InPlaceHostingManager).DownloadApplicationAsync()
                ElseIf Not askToApply Then
                    QueueStatusBarMessage("No update found, you are on the latest version.", False)
                End If
            End Sub

        AddHandler inPlaceHostingManager.DownloadProgressChanged,
            Sub(sender2 As Object, e2 As DownloadProgressChangedEventArgs)
                If Not progressIsMarquee Then
                    ShowProgresss(0, 100, e2.ProgressPercentage, "Downloading Update…")
                End If
            End Sub

        AddHandler inPlaceHostingManager.DownloadApplicationCompleted,
            Sub(sender2 As Object, e2 As DownloadApplicationCompletedEventArgs)
                HideProgress()
                If Not e2.Cancelled AndAlso e2.Error Is Nothing Then
                    QueueStatusBarMessage("Update ready, restart to apply.", False)
                Else
                    QueueStatusBarMessage("Update failed!", True)
                End If
            End Sub
        inPlaceHostingManager.GetManifestAsync()
    End Sub

    Private Sub GridViewLibrary_CustomRowFilter(sender As Object, e As Views.Base.RowFilterEventArgs) Handles GridViewLibrary.CustomRowFilter
        If e.Visible Then Exit Sub
        If GridViewLibrary.FindFilterText = Nothing OrElse GridViewLibrary.FindFilterText.Trim = Nothing Then
            e.Visible = True
            e.Handled = True
            Exit Sub
        End If

        Dim thisRow As Recommendation = _Presenter.Library(e.ListSourceRow)
        Dim colValues = New List(Of String) From {thisRow.Arranger, thisRow.Composer, If(thisRow.Era?.ToString, ""), thisRow.MetadataId.ToString, thisRow.Title}
        colValues.AddRange(thisRow.Tags.Select(Function(x) x.Name))
        colValues.AddRange(thisRow.AlternateTitleSpellings)
        Dim colValuesNoAccents = colValues.Where(Function(s) s <> Nothing).Select(Function(x) RemoveDiacritics(x)).ToList

        For Each columnValue In String.Join(" ", colValuesNoAccents).Split(" ")
            For Each filterValue As String In If(GridViewLibrary.FindFilterText?.Split(" "), "")
                If columnValue.ToLowerInvariant.Contains(filterValue.ToLowerInvariant) Then
                    e.Visible = True
                    e.Handled = True
                    Exit Sub
                End If
            Next
        Next



        'Dim score = Extensions.StringExtensions.GetSimilarityScore(String.Join(" ", colValuesNoAccents), GridViewLibrary.FindFilterText, True)
        'e.Visible = score.HighestSingleMatchScore > 98
        'e.Handled = True
    End Sub

    Private Shared Function RemoveDiacritics(text As String) As String
        Dim returnString = text
        returnString = returnString.Replace("ł", "l").Replace("Ł", "L").Replace("ø", "o").Replace("Ø", "O").Replace("æ", "ae").Replace("Æ", "Ae")

        Dim normalizedString = returnString.Normalize(NormalizationForm.FormD)
        Dim stringBuilder = New StringBuilder(capacity:=normalizedString.Length)

        For i = 0 To normalizedString.Length - 1

            Dim c As Char = normalizedString(i)
            Dim unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c)
            If unicodeCategory <> UnicodeCategory.NonSpacingMark Then
                stringBuilder.Append(c)
            End If
        Next

        Return stringBuilder.ToString().Normalize(NormalizationForm.FormC)

    End Function

#End Region

End Class
