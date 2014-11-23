Imports System.Math
Public Class TransformMatrix
    Public matrix(,) As Single
    Sub New()
        ReDim matrix(3, 3)
    End Sub

    Public Sub Zeros()
        Dim x, y As Integer
        For x = 0 To 3
            For y = 0 To 3
                matrix(x, y) = 0
            Next
        Next
    End Sub
    Public Sub Unitary()
        Dim x, y As Integer
        For x = 0 To 3
            For y = 0 To 3
                If x <> y Then matrix(x, y) = 0 Else matrix(x, y) = 1
            Next
        Next
    End Sub
    Public Sub RotationX(ByVal angle As Single)
        Unitary()
        matrix(1, 1) = Cos(angle)
        matrix(1, 2) = -Sin(angle)
        matrix(2, 1) = Sin(angle)
        matrix(2, 2) = Cos(angle)
    End Sub
    Public Sub RotationY(ByVal angle As Single)
        Unitary()
        matrix(0, 0) = Cos(angle)
        matrix(0, 2) = -Sin(angle)
        matrix(2, 0) = Sin(angle)
        matrix(2, 2) = Cos(angle)
    End Sub
    Public Sub RotationZ(ByVal angle As Single)
        Unitary()
        matrix(0, 0) = Cos(angle)
        matrix(0, 1) = -Sin(angle)
        matrix(1, 0) = Sin(angle)
        matrix(1, 1) = Cos(angle)
    End Sub
    Public Sub Scaling(ByVal scale As Single)
        Dim x, y As Integer
        For x = 0 To 3
            For y = 0 To 3
                If x <> y Then matrix(x, y) = 0 Else matrix(x, y) = scale
            Next
        Next
        matrix(3, 3) = 1
    End Sub
    Public Sub Transition(ByVal dX As Single, ByVal dY As Single, ByVal dZ As Single)
        Unitary()
        matrix(0, 3) = dX
        matrix(1, 3) = dY
        matrix(2, 3) = dZ
    End Sub
    ''' <summary>
    ''' Умножение этой матрицы на указанную
    ''' </summary>
    ''' <param name="argument"></param>
    ''' <remarks></remarks>
    Public Sub MulOnMatrix(ByRef argument As TransformMatrix)
        Dim a11, a12, a13, a14 As Single
        Dim a21, a22, a23, a24 As Single
        Dim a31, a32, a33, a34 As Single
        Dim a41, a42, a43, a44 As Single
        Dim b11, b12, b13, b14 As Single
        Dim b21, b22, b23, b24 As Single
        Dim b31, b32, b33, b34 As Single
        Dim b41, b42, b43, b44 As Single
        a11 = matrix(0, 0)
        a12 = matrix(0, 1)
        a13 = matrix(0, 2)
        a14 = matrix(0, 3)
        a21 = matrix(1, 0)
        a22 = matrix(1, 1)
        a23 = matrix(1, 2)
        a24 = matrix(1, 3)
        a31 = matrix(2, 0)
        a32 = matrix(2, 1)
        a33 = matrix(2, 2)
        a34 = matrix(2, 3)
        a41 = matrix(3, 0)
        a42 = matrix(3, 1)
        a43 = matrix(3, 2)
        a44 = matrix(3, 3)
        b11 = argument.matrix(0, 0)
        b12 = argument.matrix(0, 1)
        b13 = argument.matrix(0, 2)
        b14 = argument.matrix(0, 3)
        b21 = argument.matrix(1, 0)
        b22 = argument.matrix(1, 1)
        b23 = argument.matrix(1, 2)
        b24 = argument.matrix(1, 3)
        b31 = argument.matrix(2, 0)
        b32 = argument.matrix(2, 1)
        b33 = argument.matrix(2, 2)
        b34 = argument.matrix(2, 3)
        b41 = argument.matrix(3, 0)
        b42 = argument.matrix(3, 1)
        b43 = argument.matrix(3, 2)
        b44 = argument.matrix(3, 3)
        matrix(0, 0) = a11 * b11 + a12 * b21 + a13 * b31 + a14 * b41
        matrix(0, 1) = a11 * b12 + a12 * b22 + a13 * b32 + a14 * b42
        matrix(0, 2) = a11 * b13 + a12 * b23 + a13 * b33 + a14 * b43
        matrix(0, 3) = a11 * b14 + a12 * b24 + a13 * b34 + a14 * b44

        matrix(1, 0) = a21 * b11 + a22 * b21 + a23 * b31 + a24 * b41
        matrix(1, 1) = a21 * b12 + a22 * b22 + a23 * b32 + a24 * b42
        matrix(1, 2) = a21 * b13 + a22 * b23 + a23 * b33 + a24 * b43
        matrix(1, 3) = a21 * b14 + a22 * b24 + a23 * b34 + a24 * b44

        matrix(2, 0) = a31 * b11 + a32 * b21 + a33 * b31 + a34 * b41
        matrix(2, 1) = a31 * b12 + a32 * b22 + a33 * b32 + a34 * b42
        matrix(2, 2) = a31 * b13 + a32 * b23 + a33 * b33 + a34 * b43
        matrix(2, 3) = a31 * b14 + a32 * b24 + a33 * b34 + a34 * b44

        matrix(3, 0) = a41 * b11 + a42 * b21 + a43 * b31 + a44 * b41
        matrix(3, 1) = a41 * b12 + a42 * b22 + a43 * b32 + a44 * b42
        matrix(3, 2) = a41 * b13 + a42 * b23 + a43 * b33 + a44 * b43
        matrix(3, 3) = a41 * b14 + a42 * b24 + a43 * b34 + a44 * b44
    End Sub
    Public Sub CopyFrom(ByRef source As TransformMatrix)
        Dim x, y As Integer
        For x = 0 To 3
            For y = 0 To 3
                matrix(x, y) = source.matrix(x, y)
            Next
        Next
    End Sub
End Class
