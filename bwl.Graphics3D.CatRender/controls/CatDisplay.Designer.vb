﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CatDisplay
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
        Me.pbDisplay = New System.Windows.Forms.PictureBox
        Me.tbKeysDetector = New System.Windows.Forms.TextBox
        CType(Me.pbDisplay, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pbDisplay
        '
        Me.pbDisplay.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbDisplay.BackColor = System.Drawing.Color.White
        Me.pbDisplay.Location = New System.Drawing.Point(0, 0)
        Me.pbDisplay.Name = "pbDisplay"
        Me.pbDisplay.Size = New System.Drawing.Size(640, 480)
        Me.pbDisplay.TabIndex = 1
        Me.pbDisplay.TabStop = False
        '
        'tbKeysDetector
        '
        Me.tbKeysDetector.Location = New System.Drawing.Point(-20, 457)
        Me.tbKeysDetector.MaxLength = 3660
        Me.tbKeysDetector.Name = "tbKeysDetector"
        Me.tbKeysDetector.Size = New System.Drawing.Size(17, 20)
        Me.tbKeysDetector.TabIndex = 2
        '
        'CatDisplay
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.tbKeysDetector)
        Me.Controls.Add(Me.pbDisplay)
        Me.Name = "CatDisplay"
        Me.Size = New System.Drawing.Size(640, 480)
        CType(Me.pbDisplay, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents pbDisplay As System.Windows.Forms.PictureBox
    Friend WithEvents tbKeysDetector As System.Windows.Forms.TextBox

End Class
