Public Class CatDisplay
    Private _mouseLocked As Boolean
    Private _mouseLockingEnabled As Boolean = False
    Private _keys(256) As Boolean
    Private _mouseOffsetX As Integer
    Private _mouseOffsetY As Integer
    ''' <summary>
    ''' Включить захват мыши и клавиатуры при щелчке
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MouseLocking() As Boolean
        Get
            Return _mouseLockingEnabled
        End Get
        Set(ByVal value As Boolean)
            _mouseLockingEnabled = value
        End Set
    End Property
    ''' <summary>
    ''' Захвачена ли мышь.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property MouseLocked() As Boolean
        Get
            Return _mouseLocked
        End Get
        Set(ByVal value As Boolean)
            _mouseLocked = value
            If _mouseLocked Then
                Cursor.Position = pbDisplay.PointToScreen(New Point(pbDisplay.Width / 2, pbDisplay.Height / 2))
                pbDisplay.Cursor = Cursors.Cross
                tbKeysDetector.Focus()
            Else
                pbDisplay.Cursor = Cursors.Default
                For i As Integer = 0 To _keys.Length - 1
                    _keys(i) = False
                Next
            End If
        End Set
    End Property
    Private Sub pbDisplay_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbDisplay.Click
        If MouseLocking Then MouseLocked = Not MouseLocked
    End Sub
    Public Event MouseMoved(ByVal offsetX As Integer, ByVal offsetY As Integer)
    Private Sub pbDisplay_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pbDisplay.MouseMove
        If _mouseLocked Then
            _mouseOffsetY += e.X - pbDisplay.Width / 2
            _mouseOffsetY += e.Y - pbDisplay.Height / 2
            If e.X - pbDisplay.Width / 2 <> 0 Or e.Y - pbDisplay.Height / 2 <> 0 Then
                RaiseEvent MouseMoved(e.X - pbDisplay.Width / 2, e.Y - pbDisplay.Height / 2)
                Cursor.Position = pbDisplay.PointToScreen(New Point(pbDisplay.Width / 2, pbDisplay.Height / 2))
            End If
        End If
    End Sub

    Private Sub tbKeysDetector_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles tbKeysDetector.KeyDown
        If _mouseLocked Then
            _keys(e.KeyCode) = True
            If e.KeyCode = Windows.Forms.Keys.Escape Then _mouseLocked = False
        End If
    End Sub

    Private Sub tbKeysDetector_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles tbKeysDetector.KeyUp
        If _mouseLocked Then
            _keys(e.KeyCode) = False
            If e.KeyCode = Windows.Forms.Keys.Escape Then _mouseLocked = False
            tbKeysDetector.Clear()
        End If
    End Sub

    Public Property Bitmap() As Bitmap
        Get
            Return pbDisplay.Image
        End Get
        Set(ByVal value As Bitmap)
            pbDisplay.Image = value
        End Set
    End Property
    Public Sub RefreshBitmap()
        MeInvoke(AddressOf pbDisplay.Refresh)
    End Sub
    Public ReadOnly Property PressedKeys() As Boolean()
        Get
            Return _keys
        End Get
    End Property
    Public Property MouseOffsetY() As Integer
        Get
            Return _mouseOffsetY
        End Get
        Set(ByVal value As Integer)
            _mouseOffsetY = value
        End Set
    End Property
    Public Property MouseOffsetX() As Integer
        Get
            Return _mouseOffsetX
        End Get
        Set(ByVal value As Integer)
            _mouseOffsetX = value
        End Set
    End Property

#Region "МегаХуитка! - Конвертер процедуры в функцию и простой инвокер!"
    Private Delegate Sub Sub0Delegate()
    Private Delegate Sub Sub1Delegate(ByVal arg As Object)
    Private Delegate Sub Sub2Delegate(ByVal arg1 As Object, ByVal arg2 As Object)
    Private Delegate Sub Sub2RDelegate(ByRef arg1 As Object, ByRef arg2 As Object)
    'позволяет использовать процедуры как функции в лямбда функциях
    Private Function SubCaller(ByVal calledSub As Sub0Delegate)
        calledSub()
        Return Nothing
    End Function
    Private Overloads Sub MeInvoke(ByVal calledSub As Sub0Delegate)
        Try
            Me.Invoke(calledSub)
        Catch ex As Exception
        End Try
    End Sub
    Private Overloads Sub MeInvoke(ByVal calledSub As Sub1Delegate, ByVal arg As Object)
        Try
            Me.Invoke(calledSub, arg)
        Catch ex As Exception
        End Try
    End Sub
    Private Overloads Sub MeInvoke(ByVal calledSub As Sub2Delegate, ByVal arg1 As Object, ByVal arg2 As Object)
        Try
            Me.Invoke(calledSub, arg1, arg2)
        Catch ex As Exception
        End Try
    End Sub
    Private Overloads Sub MeInvoke(ByRef obj As Object, ByVal value As Object)
        Try
            Me.Invoke(New Sub2RDelegate(AddressOf MeInvokeSetValue), obj, value)
        Catch ex As Exception
        End Try
    End Sub
    Private Sub MeInvokeSetValue(ByRef obj As Object, ByRef value As Object)
        obj = value
    End Sub
#End Region

    Private Sub tbKeysDetector_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbKeysDetector.TextChanged

    End Sub
End Class
