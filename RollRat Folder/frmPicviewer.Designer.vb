<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPicviewer
    Inherits System.Windows.Forms.Form

    'Form은 Dispose를 재정의하여 구성 요소 목록을 정리합니다.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows Form 디자이너에 필요합니다.
    Private components As System.ComponentModel.IContainer

    '참고: 다음 프로시저는 Windows Form 디자이너에 필요합니다.
    '수정하려면 Windows Form 디자이너를 사용하십시오.  
    '코드 편집기를 사용하여 수정하지 마십시오.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.ScrollFixLayoutPanel1 = New RollRat_Folder.ScrollFixLayoutPanel()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.ScrollFixLayoutPanel1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1151, 701)
        Me.Panel1.TabIndex = 4
        '
        'ScrollFixLayoutPanel1
        '
        Me.ScrollFixLayoutPanel1.AutoScroll = True
        Me.ScrollFixLayoutPanel1.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ScrollFixLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ScrollFixLayoutPanel1.CausesValidation = False
        Me.ScrollFixLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ScrollFixLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.ScrollFixLayoutPanel1.Name = "ScrollFixLayoutPanel1"
        Me.ScrollFixLayoutPanel1.Size = New System.Drawing.Size(1151, 701)
        Me.ScrollFixLayoutPanel1.TabIndex = 0
        '
        'frmPicviewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1151, 701)
        Me.Controls.Add(Me.Panel1)
        Me.Font = New System.Drawing.Font("맑은 고딕", 9.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.Name = "frmPicviewer"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Picture Viewer"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Private WithEvents ScrollFixLayoutPanel1 As RollRat_Folder.ScrollFixLayoutPanel
End Class
