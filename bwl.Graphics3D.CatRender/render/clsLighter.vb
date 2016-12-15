Public Enum LighterTypeEnum
    ''' <summary>
    ''' Фоновый свет, равномерный со всех сторон.
    ''' </summary>
    ''' <remarks></remarks>
    ambient = 0
    ''' <summary>
    ''' Точечный источник света, может быть только один пока.
    ''' </summary>
    ''' <remarks></remarks>
    point = 1
    ''' <summary>
    ''' Простой источник, рассчитывается только для всей грани целиком.
    ''' </summary>
    ''' <remarks></remarks>
    simple = 2
    ''' <summary>
    ''' Освещение скайбокса.
    ''' </summary>
    ''' <remarks></remarks>
    sky = 3
End Enum
''' <summary>
''' Источник освещения, в том числе фоновый свет.
''' </summary>
''' <remarks></remarks>
Public Class Lighter
    Public colorR, colorG, colorB As Integer
    Public type As LighterTypeEnum
    Public intense As Integer
    Public attenutionA, attenutionB As Single
    Public name As String
End Class
