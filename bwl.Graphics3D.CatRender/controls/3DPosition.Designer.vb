<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Position3D
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
        Me.tbX = New System.Windows.Forms.NumericUpDown()
        Me.tbRX = New System.Windows.Forms.NumericUpDown()
        Me.tbRZ = New System.Windows.Forms.NumericUpDown()
        Me.tbZ = New System.Windows.Forms.NumericUpDown()
        Me.tbRY = New System.Windows.Forms.NumericUpDown()
        Me.tbY = New System.Windows.Forms.NumericUpDown()
        Me.tbFOV = New System.Windows.Forms.NumericUpDown()
        Me.lbFOV = New System.Windows.Forms.Label()
        CType(Me.tbX, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbRX, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbRZ, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbZ, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbRY, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbY, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbFOV, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tbX
        '
        Me.tbX.DecimalPlaces = 2
        Me.tbX.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.tbX.Location = New System.Drawing.Point(129, 61)
        Me.tbX.Name = "tbX"
        Me.tbX.Size = New System.Drawing.Size(62, 18)
        Me.tbX.TabIndex = 1
        '
        'tbRX
        '
        Me.tbRX.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.tbRX.Location = New System.Drawing.Point(129, 88)
        Me.tbRX.Name = "tbRX"
        Me.tbRX.Size = New System.Drawing.Size(62, 18)
        Me.tbRX.TabIndex = 2
        '
        'tbRZ
        '
        Me.tbRZ.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.tbRZ.Location = New System.Drawing.Point(58, 146)
        Me.tbRZ.Name = "tbRZ"
        Me.tbRZ.Size = New System.Drawing.Size(62, 18)
        Me.tbRZ.TabIndex = 3
        '
        'tbZ
        '
        Me.tbZ.DecimalPlaces = 2
        Me.tbZ.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.tbZ.Location = New System.Drawing.Point(75, 120)
        Me.tbZ.Name = "tbZ"
        Me.tbZ.Size = New System.Drawing.Size(62, 18)
        Me.tbZ.TabIndex = 4
        '
        'tbRY
        '
        Me.tbRY.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.tbRY.Location = New System.Drawing.Point(87, 34)
        Me.tbRY.Name = "tbRY"
        Me.tbRY.Size = New System.Drawing.Size(62, 18)
        Me.tbRY.TabIndex = 6
        '
        'tbY
        '
        Me.tbY.DecimalPlaces = 2
        Me.tbY.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.tbY.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.tbY.Location = New System.Drawing.Point(87, 7)
        Me.tbY.Name = "tbY"
        Me.tbY.Size = New System.Drawing.Size(62, 18)
        Me.tbY.TabIndex = 5
        '
        'tbFOV
        '
        Me.tbFOV.Location = New System.Drawing.Point(129, 171)
        Me.tbFOV.Name = "tbFOV"
        Me.tbFOV.Size = New System.Drawing.Size(58, 20)
        Me.tbFOV.TabIndex = 8
        '
        'lbFOV
        '
        Me.lbFOV.AutoSize = True
        Me.lbFOV.Location = New System.Drawing.Point(52, 175)
        Me.lbFOV.Name = "lbFOV"
        Me.lbFOV.Size = New System.Drawing.Size(71, 13)
        Me.lbFOV.TabIndex = 7
        Me.lbFOV.Text = "Угол обзора"
        '
        'Position3D
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BackgroundImage = Global.bwl.Graphics._3D.CatRender.My.Resources.Resources.pic1
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.Controls.Add(Me.tbFOV)
        Me.Controls.Add(Me.lbFOV)
        Me.Controls.Add(Me.tbRY)
        Me.Controls.Add(Me.tbY)
        Me.Controls.Add(Me.tbZ)
        Me.Controls.Add(Me.tbRZ)
        Me.Controls.Add(Me.tbRX)
        Me.Controls.Add(Me.tbX)
        Me.MaximumSize = New System.Drawing.Size(196, 194)
        Me.MinimumSize = New System.Drawing.Size(196, 170)
        Me.Name = "Position3D"
        Me.Size = New System.Drawing.Size(196, 194)
        CType(Me.tbX, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbRX, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbRZ, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbZ, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbRY, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbY, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbFOV, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tbX As System.Windows.Forms.NumericUpDown
    Friend WithEvents tbRX As System.Windows.Forms.NumericUpDown
    Friend WithEvents tbRZ As System.Windows.Forms.NumericUpDown
    Friend WithEvents tbZ As System.Windows.Forms.NumericUpDown
    Friend WithEvents tbRY As System.Windows.Forms.NumericUpDown
    Friend WithEvents tbY As System.Windows.Forms.NumericUpDown
    Friend WithEvents tbFOV As System.Windows.Forms.NumericUpDown
    Friend WithEvents lbFOV As System.Windows.Forms.Label

End Class
