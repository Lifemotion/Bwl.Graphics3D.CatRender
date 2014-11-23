<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CatRender
    Inherits System.Windows.Forms.UserControl

    'Пользовательский элемент управления (UserControl) переопределяет метод Dispose для очистки списка компонентов.
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

    'Является обязательной для конструктора форм Windows Forms
    Private components As System.ComponentModel.IContainer

    'Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
    'Для ее изменения используйте конструктор форм Windows Form.  
    'Не изменяйте ее в редакторе исходного кода.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lState = New System.Windows.Forms.Label
        Me.display = New CatDisplay
        Me.SuspendLayout()
        '
        'lState
        '
        Me.lState.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lState.Location = New System.Drawing.Point(-1, 482)
        Me.lState.Name = "lState"
        Me.lState.Size = New System.Drawing.Size(644, 22)
        Me.lState.TabIndex = 1
        Me.lState.Text = "Состояние"
        '
        'display
        '
        Me.display.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.display.Bitmap = Nothing
        Me.display.Location = New System.Drawing.Point(1, 1)
        Me.display.MouseLocked = False
        Me.display.MouseLocking = False
        Me.display.MouseOffsetX = 0
        Me.display.MouseOffsetY = 0
        Me.display.Name = "display"
        Me.display.Size = New System.Drawing.Size(640, 480)
        Me.display.TabIndex = 2
        '
        'CatRender
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Controls.Add(Me.display)
        Me.Controls.Add(Me.lState)
        Me.Name = "CatRender"
        Me.Size = New System.Drawing.Size(642, 500)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lState As System.Windows.Forms.Label
    Friend WithEvents display As CatDisplay

End Class
