﻿<Serializable()> _
Public Enum Object3DType
    model = 1
    sprite = 2
    light = 3
End Enum
<Serializable()> _
Public Class Object3D
    Public type As Object3DType
    <NonSerialized()> _
    Public model As Model
    Public modelMesh As Integer
    <NonSerialized()> _
   Public sprite As Sprite
    <NonSerialized()> _
    Public light As Lighter
    Public positionX, positionY, positionZ As Single
    Public rotateX, rotateY, rotateZ As Single
    Public scale As Single
    Public hidden As Boolean
    Public renderMode As RenderParameters
    Public objectName As String
    Public name As String
    Private myUID As Integer
    Private Shared UIDgen As Integer
    Sub New(ByVal objectName As String)
        name = objectName
        Init()
    End Sub
    Sub New()
        Init()
    End Sub
    Private Sub Init()
        scale = 1
        modelMesh = -1
        UIDgen += 1
        myUID = UIDgen
    End Sub
    Public ReadOnly Property UID() As Integer
        Get
            Return myUID
        End Get
    End Property


End Class