<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TestForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TestForm))
        Me.CatRender1 = New bwl.Graphics._3D.CatRender.CatRender()
        Me.SuspendLayout()
        '
        'CatRender1
        '
        Me.CatRender1.RenderAutoMove = False
        Me.CatRender1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.CatRender1.RenderCamera = CType(resources.GetObject("CatRender1.Camera"), bwl.Graphics._3D.CatRender.Camera3D)
        Me.CatRender1.RenderDrawingEnabled = False
        Me.CatRender1.Location = New System.Drawing.Point(12, 0)
        Me.CatRender1.RenderMouseLocking = False
        Me.CatRender1.Name = "CatRender1"
        Me.CatRender1.Size = New System.Drawing.Size(642, 500)
        Me.CatRender1.TabIndex = 0
        Me.CatRender1.RenderWorkingEnabled = False
        '
        'TestForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(732, 530)
        Me.Controls.Add(Me.CatRender1)
        Me.Name = "TestForm"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents CatRender1 As Bwl.Graphics._3D.CatRender.CatRender

End Class
