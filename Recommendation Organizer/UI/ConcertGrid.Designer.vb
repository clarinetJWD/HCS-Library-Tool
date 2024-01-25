<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ConcertGrid
    Inherits DevExpress.XtraEditors.XtraUserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.LayoutControl1 = New DevExpress.XtraLayout.LayoutControl()
        Me.TextEditLocation = New DevExpress.XtraEditors.TextEdit()
        Me.DateEditConcertDate = New DevExpress.XtraEditors.DateEdit()
        Me.ProgressBarControlDifficulty = New DevExpress.XtraEditors.ProgressBarControl()
        Me.TokenEditTags = New DevExpress.XtraEditors.TokenEdit()
        Me.TokenEditEras = New DevExpress.XtraEditors.TokenEdit()
        Me.GridControlProgram = New DevExpress.XtraGrid.GridControl()
        Me.GridViewProgram = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.Root = New DevExpress.XtraLayout.LayoutControlGroup()
        Me.LayoutControlGroupDetails = New DevExpress.XtraLayout.LayoutControlGroup()
        Me.SimpleSeparatorTop = New DevExpress.XtraLayout.SimpleSeparator()
        Me.LayoutControlGroupDetailsInner = New DevExpress.XtraLayout.LayoutControlGroup()
        Me.LayoutControlItemLocation = New DevExpress.XtraLayout.LayoutControlItem()
        Me.LayoutControlItemConcertDate = New DevExpress.XtraLayout.LayoutControlItem()
        Me.LayoutControlItemEras = New DevExpress.XtraLayout.LayoutControlItem()
        Me.LayoutControlItemTags = New DevExpress.XtraLayout.LayoutControlItem()
        Me.LayoutControlItemGrid = New DevExpress.XtraLayout.LayoutControlItem()
        Me.LayoutControlItemDifficulty = New DevExpress.XtraLayout.LayoutControlItem()
        Me.SimpleSeparatorBottom = New DevExpress.XtraLayout.SimpleSeparator()
        CType(Me.LayoutControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.LayoutControl1.SuspendLayout()
        CType(Me.TextEditLocation.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DateEditConcertDate.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DateEditConcertDate.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ProgressBarControlDifficulty.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TokenEditTags.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TokenEditEras.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridControlProgram, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridViewProgram, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Root, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlGroupDetails, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SimpleSeparatorTop, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlGroupDetailsInner, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItemLocation, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItemConcertDate, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItemEras, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItemTags, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItemGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItemDifficulty, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SimpleSeparatorBottom, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LayoutControl1
        '
        Me.LayoutControl1.AllowCustomization = False
        Me.LayoutControl1.Controls.Add(Me.TextEditLocation)
        Me.LayoutControl1.Controls.Add(Me.DateEditConcertDate)
        Me.LayoutControl1.Controls.Add(Me.ProgressBarControlDifficulty)
        Me.LayoutControl1.Controls.Add(Me.TokenEditTags)
        Me.LayoutControl1.Controls.Add(Me.TokenEditEras)
        Me.LayoutControl1.Controls.Add(Me.GridControlProgram)
        Me.LayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LayoutControl1.Location = New System.Drawing.Point(0, 0)
        Me.LayoutControl1.Name = "LayoutControl1"
        Me.LayoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = New System.Drawing.Rectangle(6360, 250, 650, 400)
        Me.LayoutControl1.Root = Me.Root
        Me.LayoutControl1.Size = New System.Drawing.Size(553, 538)
        Me.LayoutControl1.TabIndex = 0
        Me.LayoutControl1.Text = "LayoutControl1"
        '
        'TextEditLocation
        '
        Me.TextEditLocation.Location = New System.Drawing.Point(330, 33)
        Me.TextEditLocation.Name = "TextEditLocation"
        Me.TextEditLocation.Size = New System.Drawing.Size(216, 20)
        Me.TextEditLocation.StyleController = Me.LayoutControl1
        Me.TextEditLocation.TabIndex = 10
        '
        'DateEditConcertDate
        '
        Me.DateEditConcertDate.EditValue = Nothing
        Me.DateEditConcertDate.Location = New System.Drawing.Point(59, 33)
        Me.DateEditConcertDate.Name = "DateEditConcertDate"
        Me.DateEditConcertDate.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.DateEditConcertDate.Properties.CalendarTimeProperties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.DateEditConcertDate.Size = New System.Drawing.Size(215, 20)
        Me.DateEditConcertDate.StyleController = Me.LayoutControl1
        Me.DateEditConcertDate.TabIndex = 9
        '
        'ProgressBarControlDifficulty
        '
        Me.ProgressBarControlDifficulty.Location = New System.Drawing.Point(0, 520)
        Me.ProgressBarControlDifficulty.MinimumSize = New System.Drawing.Size(0, 16)
        Me.ProgressBarControlDifficulty.Name = "ProgressBarControlDifficulty"
        Me.ProgressBarControlDifficulty.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.ProgressBarControlDifficulty.Properties.Step = 1
        Me.ProgressBarControlDifficulty.Size = New System.Drawing.Size(553, 18)
        Me.ProgressBarControlDifficulty.StyleController = Me.LayoutControl1
        Me.ProgressBarControlDifficulty.TabIndex = 8
        '
        'TokenEditTags
        '
        Me.TokenEditTags.Location = New System.Drawing.Point(39, 81)
        Me.TokenEditTags.Name = "TokenEditTags"
        Me.TokenEditTags.Properties.AutoHeightMode = DevExpress.XtraEditors.TokenEditAutoHeightMode.Expand
        Me.TokenEditTags.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.TokenEditTags.Properties.ReadOnly = True
        Me.TokenEditTags.Properties.Separators.AddRange(New String() {","})
        Me.TokenEditTags.Properties.ShowDropDown = False
        Me.TokenEditTags.Properties.ShowTokenGlyph = False
        Me.TokenEditTags.Size = New System.Drawing.Size(507, 18)
        Me.TokenEditTags.StyleController = Me.LayoutControl1
        Me.TokenEditTags.TabIndex = 7
        '
        'TokenEditEras
        '
        Me.TokenEditEras.Location = New System.Drawing.Point(37, 59)
        Me.TokenEditEras.Name = "TokenEditEras"
        Me.TokenEditEras.Properties.AutoHeightMode = DevExpress.XtraEditors.TokenEditAutoHeightMode.Expand
        Me.TokenEditEras.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.TokenEditEras.Properties.ReadOnly = True
        Me.TokenEditEras.Properties.Separators.AddRange(New String() {","})
        Me.TokenEditEras.Properties.ShowDropDown = False
        Me.TokenEditEras.Properties.ShowTokenGlyph = False
        Me.TokenEditEras.Size = New System.Drawing.Size(509, 18)
        Me.TokenEditEras.StyleController = Me.LayoutControl1
        Me.TokenEditEras.TabIndex = 6
        '
        'GridControlProgram
        '
        Me.GridControlProgram.Location = New System.Drawing.Point(1, 110)
        Me.GridControlProgram.MainView = Me.GridViewProgram
        Me.GridControlProgram.Name = "GridControlProgram"
        Me.GridControlProgram.Size = New System.Drawing.Size(551, 386)
        Me.GridControlProgram.TabIndex = 5
        Me.GridControlProgram.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.GridViewProgram})
        '
        'GridViewProgram
        '
        Me.GridViewProgram.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.GridViewProgram.GridControl = Me.GridControlProgram
        Me.GridViewProgram.Name = "GridViewProgram"
        Me.GridViewProgram.OptionsBehavior.Editable = False
        Me.GridViewProgram.OptionsCustomization.AllowGroup = False
        Me.GridViewProgram.OptionsDetail.EnableMasterViewMode = False
        Me.GridViewProgram.OptionsMenu.EnableGroupPanelMenu = False
        Me.GridViewProgram.OptionsView.ShowGroupPanel = False
        '
        'Root
        '
        Me.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.[True]
        Me.Root.GroupBordersVisible = False
        Me.Root.Items.AddRange(New DevExpress.XtraLayout.BaseLayoutItem() {Me.LayoutControlGroupDetails, Me.LayoutControlItemGrid, Me.LayoutControlItemDifficulty, Me.SimpleSeparatorBottom})
        Me.Root.Name = "Root"
        Me.Root.Padding = New DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0)
        Me.Root.Size = New System.Drawing.Size(553, 538)
        Me.Root.TextVisible = False
        '
        'LayoutControlGroupDetails
        '
        Me.LayoutControlGroupDetails.ExpandButtonVisible = True
        Me.LayoutControlGroupDetails.GroupStyle = DevExpress.Utils.GroupStyle.Title
        Me.LayoutControlGroupDetails.Items.AddRange(New DevExpress.XtraLayout.BaseLayoutItem() {Me.SimpleSeparatorTop, Me.LayoutControlGroupDetailsInner})
        Me.LayoutControlGroupDetails.Location = New System.Drawing.Point(0, 0)
        Me.LayoutControlGroupDetails.Name = "LayoutControlGroupDetails"
        Me.LayoutControlGroupDetails.Padding = New DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0)
        Me.LayoutControlGroupDetails.Size = New System.Drawing.Size(553, 109)
        Me.LayoutControlGroupDetails.Spacing = New DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0)
        Me.LayoutControlGroupDetails.Text = "Concert Blah"
        '
        'SimpleSeparatorTop
        '
        Me.SimpleSeparatorTop.AllowHotTrack = False
        Me.SimpleSeparatorTop.Location = New System.Drawing.Point(0, 82)
        Me.SimpleSeparatorTop.Name = "SimpleSeparatorTop"
        Me.SimpleSeparatorTop.Padding = New DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0)
        Me.SimpleSeparatorTop.Size = New System.Drawing.Size(553, 1)
        '
        'LayoutControlGroupDetailsInner
        '
        Me.LayoutControlGroupDetailsInner.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.[True]
        Me.LayoutControlGroupDetailsInner.GroupBordersVisible = False
        Me.LayoutControlGroupDetailsInner.Items.AddRange(New DevExpress.XtraLayout.BaseLayoutItem() {Me.LayoutControlItemLocation, Me.LayoutControlItemConcertDate, Me.LayoutControlItemEras, Me.LayoutControlItemTags})
        Me.LayoutControlGroupDetailsInner.Location = New System.Drawing.Point(0, 0)
        Me.LayoutControlGroupDetailsInner.Name = "LayoutControlGroupDetailsInner"
        Me.LayoutControlGroupDetailsInner.Padding = New DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5)
        Me.LayoutControlGroupDetailsInner.Size = New System.Drawing.Size(553, 82)
        Me.LayoutControlGroupDetailsInner.Spacing = New DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0)
        '
        'LayoutControlItemLocation
        '
        Me.LayoutControlItemLocation.Control = Me.TextEditLocation
        Me.LayoutControlItemLocation.Location = New System.Drawing.Point(271, 0)
        Me.LayoutControlItemLocation.Name = "LayoutControlItemLocation"
        Me.LayoutControlItemLocation.Size = New System.Drawing.Size(272, 24)
        Me.LayoutControlItemLocation.Text = "Location"
        Me.LayoutControlItemLocation.TextSize = New System.Drawing.Size(40, 13)
        '
        'LayoutControlItemConcertDate
        '
        Me.LayoutControlItemConcertDate.Control = Me.DateEditConcertDate
        Me.LayoutControlItemConcertDate.Location = New System.Drawing.Point(0, 0)
        Me.LayoutControlItemConcertDate.Name = "LayoutControlItemConcertDate"
        Me.LayoutControlItemConcertDate.Size = New System.Drawing.Size(271, 24)
        Me.LayoutControlItemConcertDate.Text = "Date"
        Me.LayoutControlItemConcertDate.TextSize = New System.Drawing.Size(40, 13)
        '
        'LayoutControlItemEras
        '
        Me.LayoutControlItemEras.Control = Me.TokenEditEras
        Me.LayoutControlItemEras.Location = New System.Drawing.Point(0, 24)
        Me.LayoutControlItemEras.Name = "LayoutControlItemEras"
        Me.LayoutControlItemEras.Padding = New DevExpress.XtraLayout.Utils.Padding(2, 2, 4, 2)
        Me.LayoutControlItemEras.Size = New System.Drawing.Size(543, 24)
        Me.LayoutControlItemEras.Text = "Eras:"
        Me.LayoutControlItemEras.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize
        Me.LayoutControlItemEras.TextSize = New System.Drawing.Size(25, 13)
        Me.LayoutControlItemEras.TextToControlDistance = 5
        '
        'LayoutControlItemTags
        '
        Me.LayoutControlItemTags.Control = Me.TokenEditTags
        Me.LayoutControlItemTags.Location = New System.Drawing.Point(0, 48)
        Me.LayoutControlItemTags.Name = "LayoutControlItemTags"
        Me.LayoutControlItemTags.Padding = New DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 4)
        Me.LayoutControlItemTags.Size = New System.Drawing.Size(543, 24)
        Me.LayoutControlItemTags.Text = "Tags:"
        Me.LayoutControlItemTags.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize
        Me.LayoutControlItemTags.TextSize = New System.Drawing.Size(27, 13)
        Me.LayoutControlItemTags.TextToControlDistance = 5
        '
        'LayoutControlItemGrid
        '
        Me.LayoutControlItemGrid.Control = Me.GridControlProgram
        Me.LayoutControlItemGrid.Location = New System.Drawing.Point(0, 109)
        Me.LayoutControlItemGrid.Name = "LayoutControlItemGrid"
        Me.LayoutControlItemGrid.Padding = New DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0)
        Me.LayoutControlItemGrid.Size = New System.Drawing.Size(553, 388)
        Me.LayoutControlItemGrid.Spacing = New DevExpress.XtraLayout.Utils.Padding(1, 1, 1, 1)
        Me.LayoutControlItemGrid.TextSize = New System.Drawing.Size(0, 0)
        Me.LayoutControlItemGrid.TextVisible = False
        '
        'LayoutControlItemDifficulty
        '
        Me.LayoutControlItemDifficulty.ContentVertAlignment = DevExpress.Utils.VertAlignment.Bottom
        Me.LayoutControlItemDifficulty.Control = Me.ProgressBarControlDifficulty
        Me.LayoutControlItemDifficulty.Location = New System.Drawing.Point(0, 498)
        Me.LayoutControlItemDifficulty.Name = "LayoutControlItemDifficulty"
        Me.LayoutControlItemDifficulty.Padding = New DevExpress.XtraLayout.Utils.Padding(0, 0, 4, 0)
        Me.LayoutControlItemDifficulty.Size = New System.Drawing.Size(553, 40)
        Me.LayoutControlItemDifficulty.Text = "  Difficulty"
        Me.LayoutControlItemDifficulty.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize
        Me.LayoutControlItemDifficulty.TextLocation = DevExpress.Utils.Locations.Top
        Me.LayoutControlItemDifficulty.TextSize = New System.Drawing.Size(48, 13)
        Me.LayoutControlItemDifficulty.TextToControlDistance = 5
        '
        'SimpleSeparatorBottom
        '
        Me.SimpleSeparatorBottom.AllowHotTrack = False
        Me.SimpleSeparatorBottom.Location = New System.Drawing.Point(0, 497)
        Me.SimpleSeparatorBottom.Name = "SimpleSeparatorBottom"
        Me.SimpleSeparatorBottom.Padding = New DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0)
        Me.SimpleSeparatorBottom.Size = New System.Drawing.Size(553, 1)
        '
        'ConcertGrid
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.LayoutControl1)
        Me.Name = "ConcertGrid"
        Me.Size = New System.Drawing.Size(553, 538)
        CType(Me.LayoutControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.LayoutControl1.ResumeLayout(False)
        CType(Me.TextEditLocation.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DateEditConcertDate.Properties.CalendarTimeProperties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DateEditConcertDate.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ProgressBarControlDifficulty.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TokenEditTags.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TokenEditEras.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridControlProgram, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridViewProgram, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Root, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlGroupDetails, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SimpleSeparatorTop, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlGroupDetailsInner, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItemLocation, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItemConcertDate, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItemEras, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItemTags, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItemGrid, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItemDifficulty, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SimpleSeparatorBottom, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LayoutControl1 As DevExpress.XtraLayout.LayoutControl
    Friend WithEvents Root As DevExpress.XtraLayout.LayoutControlGroup
    Friend WithEvents GridControlProgram As DevExpress.XtraGrid.GridControl
    Friend WithEvents GridViewProgram As DevExpress.XtraGrid.Views.Grid.GridView
    Friend WithEvents LayoutControlGroupDetails As DevExpress.XtraLayout.LayoutControlGroup
    Friend WithEvents LayoutControlItemGrid As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents TokenEditEras As DevExpress.XtraEditors.TokenEdit
    Friend WithEvents LayoutControlItemEras As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents TokenEditTags As DevExpress.XtraEditors.TokenEdit
    Friend WithEvents LayoutControlItemTags As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents ProgressBarControlDifficulty As DevExpress.XtraEditors.ProgressBarControl
    Friend WithEvents LayoutControlItemDifficulty As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents SimpleSeparatorTop As DevExpress.XtraLayout.SimpleSeparator
    Friend WithEvents SimpleSeparatorBottom As DevExpress.XtraLayout.SimpleSeparator
    Friend WithEvents TextEditLocation As DevExpress.XtraEditors.TextEdit
    Friend WithEvents DateEditConcertDate As DevExpress.XtraEditors.DateEdit
    Friend WithEvents LayoutControlItemConcertDate As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents LayoutControlItemLocation As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents LayoutControlGroupDetailsInner As DevExpress.XtraLayout.LayoutControlGroup
End Class
