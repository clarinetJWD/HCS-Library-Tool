<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ComposerMergeForm
    Inherits DevExpress.XtraEditors.XtraForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.LayoutControl1 = New DevExpress.XtraLayout.LayoutControl()
        Me.SimpleButtonCancel = New DevExpress.XtraEditors.SimpleButton()
        Me.SimpleButtonSave = New DevExpress.XtraEditors.SimpleButton()
        Me.ComboBoxEditComposer = New DevExpress.XtraEditors.ComboBoxEdit()
        Me.Root = New DevExpress.XtraLayout.LayoutControlGroup()
        Me.LayoutControlItemName = New DevExpress.XtraLayout.LayoutControlItem()
        Me.EmptySpaceItem1 = New DevExpress.XtraLayout.EmptySpaceItem()
        Me.LayoutControlItemButtonSave = New DevExpress.XtraLayout.LayoutControlItem()
        Me.EmptySpaceItem2 = New DevExpress.XtraLayout.EmptySpaceItem()
        Me.LayoutControlItemButtonCancel = New DevExpress.XtraLayout.LayoutControlItem()
        CType(Me.LayoutControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.LayoutControl1.SuspendLayout()
        CType(Me.ComboBoxEditComposer.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Root, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItemName, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.EmptySpaceItem1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItemButtonSave, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.EmptySpaceItem2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItemButtonCancel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LayoutControl1
        '
        Me.LayoutControl1.AllowCustomization = False
        Me.LayoutControl1.Controls.Add(Me.SimpleButtonCancel)
        Me.LayoutControl1.Controls.Add(Me.SimpleButtonSave)
        Me.LayoutControl1.Controls.Add(Me.ComboBoxEditComposer)
        Me.LayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LayoutControl1.Location = New System.Drawing.Point(0, 0)
        Me.LayoutControl1.Name = "LayoutControl1"
        Me.LayoutControl1.Root = Me.Root
        Me.LayoutControl1.Size = New System.Drawing.Size(413, 102)
        Me.LayoutControl1.TabIndex = 0
        Me.LayoutControl1.Text = "LayoutControl1"
        '
        'SimpleButtonCancel
        '
        Me.SimpleButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.SimpleButtonCancel.Location = New System.Drawing.Point(305, 68)
        Me.SimpleButtonCancel.Name = "SimpleButtonCancel"
        Me.SimpleButtonCancel.Size = New System.Drawing.Size(96, 22)
        Me.SimpleButtonCancel.StyleController = Me.LayoutControl1
        Me.SimpleButtonCancel.TabIndex = 6
        Me.SimpleButtonCancel.Text = "Cancel"
        '
        'SimpleButtonSave
        '
        Me.SimpleButtonSave.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.SimpleButtonSave.Location = New System.Drawing.Point(208, 68)
        Me.SimpleButtonSave.Name = "SimpleButtonSave"
        Me.SimpleButtonSave.Size = New System.Drawing.Size(93, 22)
        Me.SimpleButtonSave.StyleController = Me.LayoutControl1
        Me.SimpleButtonSave.TabIndex = 5
        Me.SimpleButtonSave.Text = "Save Changes"
        '
        'ComboBoxEditComposer
        '
        Me.ComboBoxEditComposer.Location = New System.Drawing.Point(141, 12)
        Me.ComboBoxEditComposer.Name = "ComboBoxEditComposer"
        Me.ComboBoxEditComposer.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)})
        Me.ComboBoxEditComposer.Size = New System.Drawing.Size(260, 20)
        Me.ComboBoxEditComposer.StyleController = Me.LayoutControl1
        Me.ComboBoxEditComposer.TabIndex = 4
        '
        'Root
        '
        Me.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.[True]
        Me.Root.GroupBordersVisible = False
        Me.Root.Items.AddRange(New DevExpress.XtraLayout.BaseLayoutItem() {Me.LayoutControlItemName, Me.EmptySpaceItem1, Me.LayoutControlItemButtonSave, Me.EmptySpaceItem2, Me.LayoutControlItemButtonCancel})
        Me.Root.Name = "Root"
        Me.Root.Size = New System.Drawing.Size(413, 102)
        Me.Root.TextVisible = False
        '
        'LayoutControlItemName
        '
        Me.LayoutControlItemName.Control = Me.ComboBoxEditComposer
        Me.LayoutControlItemName.ControlAlignment = System.Drawing.ContentAlignment.TopLeft
        Me.LayoutControlItemName.Location = New System.Drawing.Point(0, 0)
        Me.LayoutControlItemName.Name = "LayoutControlItemName"
        Me.LayoutControlItemName.Size = New System.Drawing.Size(393, 24)
        Me.LayoutControlItemName.Text = "Primary Composer Name"
        Me.LayoutControlItemName.TextSize = New System.Drawing.Size(117, 13)
        '
        'EmptySpaceItem1
        '
        Me.EmptySpaceItem1.AllowHotTrack = False
        Me.EmptySpaceItem1.Location = New System.Drawing.Point(0, 24)
        Me.EmptySpaceItem1.Name = "EmptySpaceItem1"
        Me.EmptySpaceItem1.Size = New System.Drawing.Size(393, 32)
        Me.EmptySpaceItem1.TextSize = New System.Drawing.Size(0, 0)
        '
        'LayoutControlItemButtonSave
        '
        Me.LayoutControlItemButtonSave.Control = Me.SimpleButtonSave
        Me.LayoutControlItemButtonSave.Location = New System.Drawing.Point(196, 56)
        Me.LayoutControlItemButtonSave.Name = "LayoutControlItemButtonSave"
        Me.LayoutControlItemButtonSave.Size = New System.Drawing.Size(97, 26)
        Me.LayoutControlItemButtonSave.TextSize = New System.Drawing.Size(0, 0)
        Me.LayoutControlItemButtonSave.TextVisible = False
        '
        'EmptySpaceItem2
        '
        Me.EmptySpaceItem2.AllowHotTrack = False
        Me.EmptySpaceItem2.Location = New System.Drawing.Point(0, 56)
        Me.EmptySpaceItem2.Name = "EmptySpaceItem2"
        Me.EmptySpaceItem2.Size = New System.Drawing.Size(196, 26)
        Me.EmptySpaceItem2.TextSize = New System.Drawing.Size(0, 0)
        '
        'LayoutControlItemButtonCancel
        '
        Me.LayoutControlItemButtonCancel.Control = Me.SimpleButtonCancel
        Me.LayoutControlItemButtonCancel.Location = New System.Drawing.Point(293, 56)
        Me.LayoutControlItemButtonCancel.Name = "LayoutControlItemButtonCancel"
        Me.LayoutControlItemButtonCancel.Size = New System.Drawing.Size(100, 26)
        Me.LayoutControlItemButtonCancel.TextSize = New System.Drawing.Size(0, 0)
        Me.LayoutControlItemButtonCancel.TextVisible = False
        '
        'ComposerMergeForm
        '
        Me.AcceptButton = Me.SimpleButtonSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.SimpleButtonCancel
        Me.ClientSize = New System.Drawing.Size(413, 102)
        Me.Controls.Add(Me.LayoutControl1)
        Me.IconOptions.SvgImage = Global.HcsLibraryTool.My.Resources.Resources.hcs_icon
        Me.KeyPreview = True
        Me.Name = "ComposerMergeForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Merge or Edit Composer"
        CType(Me.LayoutControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.LayoutControl1.ResumeLayout(False)
        CType(Me.ComboBoxEditComposer.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Root, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItemName, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.EmptySpaceItem1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItemButtonSave, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.EmptySpaceItem2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItemButtonCancel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LayoutControl1 As DevExpress.XtraLayout.LayoutControl
    Friend WithEvents Root As DevExpress.XtraLayout.LayoutControlGroup
    Friend WithEvents EmptySpaceItem1 As DevExpress.XtraLayout.EmptySpaceItem
    Friend WithEvents ComboBoxEditComposer As DevExpress.XtraEditors.ComboBoxEdit
    Friend WithEvents LayoutControlItemName As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents SimpleButtonSave As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents LayoutControlItemButtonSave As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents EmptySpaceItem2 As DevExpress.XtraLayout.EmptySpaceItem
    Friend WithEvents SimpleButtonCancel As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents LayoutControlItemButtonCancel As DevExpress.XtraLayout.LayoutControlItem
End Class
