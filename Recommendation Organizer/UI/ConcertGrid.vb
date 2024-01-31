Imports System.ComponentModel
Imports DevExpress.Utils.Behaviors
Imports DevExpress.Utils.DragDrop
Imports DevExpress.XtraGrid.Views.Grid

Public Class ConcertGrid

#Region "Properties"

    Private ReadOnly Property _Presenter As LibraryToolPresenter
    Private ReadOnly Property _ConcertInformation As ConcertInformation

    Public Property Title As String
        Get
            Return _Title
        End Get
        Set(value As String)
            _Title = value
            SetTitleText()
        End Set
    End Property
    Private _Title As String

#End Region

#Region "Initialization"

    Friend Sub Initialize(presenter As LibraryToolPresenter, behaviorManager1 As BehaviorManager)
        __Presenter = presenter
        behaviorManager1.Attach(Of DragDropBehavior)(GridViewProgram,
                                                     Sub(behavior)
                                                         behavior.Properties.AllowDrop = True
                                                         behavior.Properties.AllowDrag = True
                                                         behavior.Properties.InsertIndicatorVisible = True
                                                         behavior.Properties.PreviewVisible = True
                                                         AddHandler behavior.DragDrop, AddressOf OnDragDropToSeasonGrid
                                                     End Sub)

    End Sub

    Private Sub OnDragDropToSeasonGrid(sender As Object, e As DragDropEventArgs)

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
                    targetView.DataSource.Add(row)
                Next
                e.Handled = True
            Case DragDropActions.Move
                Dim hi = targetView.CalcHitInfo(targetView.GridControl.PointToClient(e.Location))
                Dim insertLocation = -1
                If hi.InDataRow AndAlso e.InsertType <> InsertType.AsChild Then
                    Dim targetRow As SeasonItem = targetView.GetRow(hi.RowHandle)
                    insertLocation = targetView.DataSource.IndexOf(targetRow)
                    If e.InsertType <> InsertType.Before Then
                        insertLocation += 1
                    End If
                End If

                For i As Integer = rowsToMove.Count - 1 To 0 Step -1
                    Dim row = rowsToMove(i)

                    If sourceView.DataSource IsNot _Presenter.SeasonPlannerItems Then
                        sourceView.DataSource.Remove(row)
                    End If

                    If insertLocation >= 0 AndAlso insertLocation < targetView.DataRowCount Then
                        targetView.DataSource.Insert(insertLocation, row)
                    Else
                        targetView.DataSource.Add(row)
                    End If
                    targetView.FocusedRowHandle = targetView.GetRowHandle(targetView.DataSource.IndexOf(row))
                Next

                e.Handled = True
        End Select
        _Presenter.EnsureNoSeasonPlanningDuplicates()
    End Sub

    Friend Sub InitializeInfo(concertInformation As ConcertInformation, eras As Eras, tags As Tags)
        SetConcertInformation(concertInformation, eras, tags)
    End Sub

    Private Sub SetConcertInformation(concertInformation As ConcertInformation, eras As Eras, tags As Tags)
        Me.__ConcertInformation = concertInformation

        Me.TokenEditEras.Properties.DataSource = eras
        Me.TokenEditEras.Properties.DisplayMember = "Name"
        Me.TokenEditEras.Properties.ValueMember = "Name"
        Me.TokenEditEras.Properties.Separators.Add(";")

        AddHandler concertInformation.Compositions.ListChanged, AddressOf OnErasChanged
        SetEras()

        Me.TokenEditTags.Properties.DataSource = tags
        Me.TokenEditTags.Properties.DisplayMember = "Name"
        Me.TokenEditTags.Properties.ValueMember = "Name"
        Me.TokenEditTags.Properties.Separators.Add(";")

        AddHandler concertInformation.Compositions.ListChanged, AddressOf OnTagsChanged
        SetTags()

        GridControlProgram.DataSource = concertInformation.Compositions
        GridViewProgram.Columns("Recommendation").Visible = False
        GridViewProgram.Columns("Recommendation").OptionsColumn.ShowInCustomizationForm = False

        GridViewProgram.Columns("Era").Visible = False
        GridViewProgram.Columns("Difficulty").Visible = False

        GridViewProgram.AutoFillColumn = GridViewProgram.Columns("Title")

        GridViewProgram.BestFitColumns()

        DateEditConcertDate.DataBindings.Clear()
        DateEditConcertDate.DataBindings.Add(New Binding(NameOf(DateEditConcertDate.EditValue), concertInformation, NameOf(concertInformation.Date), False, DataSourceUpdateMode.OnPropertyChanged))

        TextEditLocation.DataBindings.Clear()
        TextEditLocation.DataBindings.Add(New Binding(NameOf(TextEditLocation.EditValue), concertInformation, NameOf(concertInformation.Location), False, DataSourceUpdateMode.OnPropertyChanged))

        ProgressBarControlDifficulty.DataBindings.Clear()
        ProgressBarControlDifficulty.DataBindings.Add(New Binding(NameOf(ProgressBarControlDifficulty.Position), concertInformation, NameOf(concertInformation.DifficultyScore), False, DataSourceUpdateMode.OnPropertyChanged))

        SetTitleText()
        SetProgressColor()

        AddHandler concertInformation.PropertyChanged, AddressOf OnConcertInformationPropertyChanged
    End Sub

#End Region

#Region "Concert Information Handlers"

    Private Sub OnConcertInformationPropertyChanged(sender As Object, e As PropertyChangedEventArgs)
        Select Case e.PropertyName
            Case NameOf(ConcertInformation.Length), NameOf(ConcertInformation.Date), NameOf(ConcertInformation.Location)
                SetTitleText()
            Case NameOf(ConcertInformation.DifficultyScore)
                SetProgressColor()
        End Select
    End Sub

    Private Sub OnErasChanged(sender As Object, e As ListChangedEventArgs)
        SetEras()
    End Sub

    Private Sub OnTagsChanged(sender As Object, e As ListChangedEventArgs)
        SetTags()
    End Sub

#End Region

#Region "UI Helpers/Update"

    Private Sub SetTitleText()
        Dim titleText = _Title

        If _ConcertInformation IsNot Nothing Then
            If _ConcertInformation.Date <> Nothing Then
                titleText &= $" - {_ConcertInformation.Date:MMM d}"
            End If
            If _ConcertInformation.Location <> Nothing Then
                titleText &= $" at {_ConcertInformation.Location}"
            End If
            If _ConcertInformation.Length > TimeSpan.Zero Then
                titleText &= $" [{_ConcertInformation.Length.Hours}:{_ConcertInformation.Length.Minutes:D2}]"
            End If
        End If

        LayoutControlGroupDetails.Text = titleText
        LayoutControlGroupDetails.CustomizationFormText = _Title
    End Sub

    Private Sub SetProgressColor()

        ProgressBarControlDifficulty.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat
        ProgressBarControlDifficulty.LookAndFeel.UseDefaultLookAndFeel = False
        ProgressBarControlDifficulty.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid

        Select Case _ConcertInformation.DifficultyScore
            Case < CDbl(Difficulties.Intermediate) / CDbl(Difficulties.TooHard) * 100.0
                ProgressBarControlDifficulty.Properties.StartColor = Color.FromArgb(73, 130, 82)
                ProgressBarControlDifficulty.Properties.EndColor = Color.FromArgb(73, 130, 82)
            Case < CDbl(Difficulties.Difficult) / CDbl(Difficulties.TooHard) * 100.0
                ProgressBarControlDifficulty.Properties.StartColor = Color.FromArgb(212, 143, 15)
                ProgressBarControlDifficulty.Properties.EndColor = Color.FromArgb(212, 143, 15)
            Case < CDbl(Difficulties.VeryDifficult) / CDbl(Difficulties.TooHard) * 100.0
                ProgressBarControlDifficulty.Properties.StartColor = Color.FromArgb(192, 85, 24)
                ProgressBarControlDifficulty.Properties.EndColor = Color.FromArgb(192, 85, 24)
            Case < CDbl(Difficulties.TooHard) / CDbl(Difficulties.TooHard) * 100.0
                ProgressBarControlDifficulty.Properties.StartColor = Color.FromArgb(164, 42, 36)
                ProgressBarControlDifficulty.Properties.EndColor = Color.FromArgb(164, 42, 36)
            Case Else
                ProgressBarControlDifficulty.Properties.StartColor = Color.FromArgb(164, 42, 36)
                ProgressBarControlDifficulty.Properties.EndColor = Color.FromArgb(155, 9, 0)
        End Select

    End Sub

    Private Sub SetEras()
        Dim eraString = Me._ConcertInformation.Eras.GetErasTokenString()
        If eraString Is Nothing OrElse eraString.Trim = Nothing Then
            LayoutControlItemEras.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never
        Else
            LayoutControlItemEras.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always
        End If

        Me.TokenEditEras.EditValue = eraString
    End Sub

    Private Sub SetTags()
        Dim tagsString = Me._ConcertInformation.Tags.GetTagsTokenString()
        If tagsString Is Nothing OrElse tagsString.Trim = Nothing Then
            LayoutControlItemTags.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never
        Else
            LayoutControlItemTags.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always
        End If

        Me.TokenEditTags.EditValue = tagsString
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

    Private Sub TokenEditTags_CustomDrawTokenBackground(sender As Object, e As DevExpress.XtraEditors.TokenEditCustomDrawTokenBackgroundEventArgs) Handles TokenEditTags.CustomDrawTokenBackground
        e.Cache.FillRoundedRectangle(Color.FromArgb(40, GetColorForTagaDescription(e.Description)), e.Bounds, New DevExpress.Utils.Drawing.CornerRadius(4))
    End Sub

    Private Function GetColorForTagaDescription(tag As String) As Color
        Dim foundTag = _Presenter.Tags.Find(tag)
        If foundTag IsNot Nothing AndAlso foundTag.Color <> Color.Transparent Then
            Return foundTag.Color
        End If

        Return Color.Gray
    End Function

    Private Sub GridViewProgram_CustomDrawCell(sender As Object, e As DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs) Handles GridViewProgram.CustomDrawCell
        If e.Column.FieldName = NameOf(SeasonItem.Length) Then
            If e.CellValue Is Nothing OrElse DirectCast(e.CellValue, TimeSpan) = TimeSpan.Zero Then
                e.DisplayText = String.Empty
            End If
        End If
    End Sub

#End Region

#Region "User Customization"

    Friend Sub SaveLayout()
        Using fs As New IO.FileStream(LocalPath_ConcertGridSettings(Me.Name), IO.FileMode.Create)
            GridViewProgram.SaveLayoutToStream(fs)
        End Using
    End Sub

    Friend Sub RestoreLayout()
        If IO.File.Exists(LocalPath_ConcertGridSettings(Me.Name)) Then
            Using fs As New IO.FileStream(LocalPath_ConcertGridSettings(Me.Name), IO.FileMode.Open)
                Try
                    GridViewProgram.RestoreLayoutFromStream(fs)
                    GridViewProgram.OptionsView.ShowGroupPanel = False
                Catch ex As Exception
                End Try
            End Using
        End If
    End Sub

#End Region

End Class
