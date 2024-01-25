<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PasscodeForm
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
        Me.TextEditPasscode = New DevExpress.XtraEditors.TextEdit()
        Me.Root = New DevExpress.XtraLayout.LayoutControlGroup()
        Me.LayoutControlItemPasscode = New DevExpress.XtraLayout.LayoutControlItem()
        Me.EmptySpaceItem1 = New DevExpress.XtraLayout.EmptySpaceItem()
        Me.LayoutControlItemSave = New DevExpress.XtraLayout.LayoutControlItem()
        Me.EmptySpaceItem2 = New DevExpress.XtraLayout.EmptySpaceItem()
        Me.LayoutControlItemCancel = New DevExpress.XtraLayout.LayoutControlItem()
        CType(Me.LayoutControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.LayoutControl1.SuspendLayout()
        CType(Me.TextEditPasscode.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Root, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItemPasscode, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.EmptySpaceItem1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItemSave, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.EmptySpaceItem2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LayoutControlItemCancel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LayoutControl1
        '
        Me.LayoutControl1.AllowCustomization = False
        Me.LayoutControl1.Controls.Add(Me.SimpleButtonCancel)
        Me.LayoutControl1.Controls.Add(Me.SimpleButtonSave)
        Me.LayoutControl1.Controls.Add(Me.TextEditPasscode)
        Me.LayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LayoutControl1.Location = New System.Drawing.Point(0, 0)
        Me.LayoutControl1.Name = "LayoutControl1"
        Me.LayoutControl1.Root = Me.Root
        Me.LayoutControl1.Size = New System.Drawing.Size(430, 133)
        Me.LayoutControl1.TabIndex = 0
        Me.LayoutControl1.Text = "LayoutControl1"
        '
        'SimpleButtonCancel
        '
        Me.SimpleButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.SimpleButtonCancel.Location = New System.Drawing.Point(318, 99)
        Me.SimpleButtonCancel.Name = "SimpleButtonCancel"
        Me.SimpleButtonCancel.Size = New System.Drawing.Size(100, 22)
        Me.SimpleButtonCancel.StyleController = Me.LayoutControl1
        Me.SimpleButtonCancel.TabIndex = 6
        Me.SimpleButtonCancel.Text = "Cancel"
        '
        'SimpleButtonSave
        '
        Me.SimpleButtonSave.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.SimpleButtonSave.Location = New System.Drawing.Point(216, 99)
        Me.SimpleButtonSave.Name = "SimpleButtonSave"
        Me.SimpleButtonSave.Size = New System.Drawing.Size(98, 22)
        Me.SimpleButtonSave.StyleController = Me.LayoutControl1
        Me.SimpleButtonSave.TabIndex = 5
        Me.SimpleButtonSave.Text = "Save"
        '
        'TextEditPasscode
        '
        Me.TextEditPasscode.Location = New System.Drawing.Point(69, 12)
        Me.TextEditPasscode.Name = "TextEditPasscode"
        Me.TextEditPasscode.Properties.UseSystemPasswordChar = True
        Me.TextEditPasscode.Size = New System.Drawing.Size(349, 20)
        Me.TextEditPasscode.StyleController = Me.LayoutControl1
        Me.TextEditPasscode.TabIndex = 4
        '
        'Root
        '
        Me.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.[True]
        Me.Root.GroupBordersVisible = False
        Me.Root.Items.AddRange(New DevExpress.XtraLayout.BaseLayoutItem() {Me.LayoutControlItemPasscode, Me.EmptySpaceItem1, Me.LayoutControlItemSave, Me.EmptySpaceItem2, Me.LayoutControlItemCancel})
        Me.Root.Name = "Root"
        Me.Root.Size = New System.Drawing.Size(430, 133)
        Me.Root.TextVisible = False
        '
        'LayoutControlItemPasscode
        '
        Me.LayoutControlItemPasscode.Control = Me.TextEditPasscode
        Me.LayoutControlItemPasscode.Location = New System.Drawing.Point(0, 0)
        Me.LayoutControlItemPasscode.Name = "LayoutControlItemPasscode"
        Me.LayoutControlItemPasscode.Size = New System.Drawing.Size(410, 24)
        Me.LayoutControlItemPasscode.Text = "Passcode"
        Me.LayoutControlItemPasscode.TextSize = New System.Drawing.Size(45, 13)
        '
        'EmptySpaceItem1
        '
        Me.EmptySpaceItem1.AllowHotTrack = False
        Me.EmptySpaceItem1.Location = New System.Drawing.Point(0, 24)
        Me.EmptySpaceItem1.Name = "EmptySpaceItem1"
        Me.EmptySpaceItem1.Size = New System.Drawing.Size(410, 63)
        Me.EmptySpaceItem1.TextSize = New System.Drawing.Size(0, 0)
        '
        'LayoutControlItemSave
        '
        Me.LayoutControlItemSave.Control = Me.SimpleButtonSave
        Me.LayoutControlItemSave.Location = New System.Drawing.Point(204, 87)
        Me.LayoutControlItemSave.Name = "LayoutControlItemSave"
        Me.LayoutControlItemSave.Size = New System.Drawing.Size(102, 26)
        Me.LayoutControlItemSave.TextSize = New System.Drawing.Size(0, 0)
        Me.LayoutControlItemSave.TextVisible = False
        '
        'EmptySpaceItem2
        '
        Me.EmptySpaceItem2.AllowHotTrack = False
        Me.EmptySpaceItem2.Location = New System.Drawing.Point(0, 87)
        Me.EmptySpaceItem2.Name = "EmptySpaceItem2"
        Me.EmptySpaceItem2.Size = New System.Drawing.Size(204, 26)
        Me.EmptySpaceItem2.TextSize = New System.Drawing.Size(0, 0)
        '
        'LayoutControlItemCancel
        '
        Me.LayoutControlItemCancel.Control = Me.SimpleButtonCancel
        Me.LayoutControlItemCancel.Location = New System.Drawing.Point(306, 87)
        Me.LayoutControlItemCancel.Name = "LayoutControlItemCancel"
        Me.LayoutControlItemCancel.Size = New System.Drawing.Size(104, 26)
        Me.LayoutControlItemCancel.TextSize = New System.Drawing.Size(0, 0)
        Me.LayoutControlItemCancel.TextVisible = False
        '
        'PasscodeForm
        '
        Me.AcceptButton = Me.SimpleButtonSave
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.SimpleButtonCancel
        Me.ClientSize = New System.Drawing.Size(430, 133)
        Me.Controls.Add(Me.LayoutControl1)
        Me.IconOptions.SvgImage = Global.HcsLibraryTool.My.Resources.Resources.hcs_icon
        Me.KeyPreview = True
        Me.Name = "PasscodeForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Enter Passcode"
        CType(Me.LayoutControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.LayoutControl1.ResumeLayout(False)
        CType(Me.TextEditPasscode.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Root, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItemPasscode, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.EmptySpaceItem1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItemSave, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.EmptySpaceItem2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LayoutControlItemCancel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents LayoutControl1 As DevExpress.XtraLayout.LayoutControl
    Friend WithEvents Root As DevExpress.XtraLayout.LayoutControlGroup
    Friend WithEvents LayoutControlItemPasscode As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents EmptySpaceItem1 As DevExpress.XtraLayout.EmptySpaceItem
    Friend WithEvents SimpleButtonSave As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents LayoutControlItemSave As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents EmptySpaceItem2 As DevExpress.XtraLayout.EmptySpaceItem
    Friend WithEvents SimpleButtonCancel As DevExpress.XtraEditors.SimpleButton
    Friend WithEvents LayoutControlItemCancel As DevExpress.XtraLayout.LayoutControlItem
    Friend WithEvents TextEditPasscode As DevExpress.XtraEditors.TextEdit
End Class
