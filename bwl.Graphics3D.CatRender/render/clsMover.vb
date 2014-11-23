Public Class Mover
    Private Enum targetTypeEnum As Byte
        object3D = 1
        camera3D = 2
    End Enum
    Public name As String
    Private targetCamera As Camera3D
    Private targetObject As Object3D
    Private targetType As targetTypeEnum
    Private currentX, currentY, currentZ As Single
    Private currentRX, currentRY, currentRZ As Single
    Private currentR, currentG, currentB As Single
    Private targetX, targetY, targetZ As Single
    Private targetRX, targetRY, targetRZ As Single
    Private targetR, targetG, targetB As Single
    Private stepX, stepY, stepZ As Single
    Private stepRX, stepRY, stepRZ As Single
    Private stepR, stepG, stepB As Single
    Private currentStep, stepsCount As Integer
    Private affectColor As Boolean
    Private affectPosition As Boolean
    Private affectRotate As Boolean
    Private isRunning As Boolean
    Private isFinished As Boolean
    Private startTime, endTime As Single
    Sub New()

    End Sub
    Sub New(ByVal running As Boolean)
        running = True
    End Sub
    Public Overloads Sub CaptureSource(ByVal source As Object3D)
        targetType = targetTypeEnum.object3D
        targetObject = source
        currentX = source.positionX
        currentY = source.positionY
        currentZ = source.positionZ
        currentRX = source.rotateX
        currentRY = source.rotateY
        currentRZ = source.rotateZ
        If Not (source.light Is Nothing) Then
            currentR = source.light.colorR
            currentG = source.light.colorG
            currentB = source.light.colorB
        End If
    End Sub
    Public Overloads Sub CaptureSource(ByVal source As Camera3D)
        targetType = targetTypeEnum.camera3D
        targetCamera = source
        currentX = source.PositionX
        currentY = source.PositionY
        currentZ = source.PositionZ
        currentRX = source.Pitch
        currentRY = source.Yaw
        currentRZ = source.Roll
    End Sub
    Public Sub SetTargetPR(ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal rx As Single, ByVal ry As Single, ByVal rz As Single, ByVal time As Single, ByVal difference As Boolean)
        If difference Then
            targetX = x + currentX
            targetY = y + currentY
            targetZ = z + currentZ
            targetRX = rx + currentRX
            targetRY = ry + currentRY
            targetRZ = rz + currentRZ
        Else
            targetX = x
            targetY = y
            targetZ = z
            targetRX = rx
            targetRY = ry
            targetRZ = rz
        End If
        Dim timeKoeff As Single
        timeKoeff = time / (globalTickPeriod / 1000)
        startTime = Microsoft.VisualBasic.Timer
        endTime = time
        If CInt(timeKoeff) > 0 Then
            stepX = (targetX - currentX) / timeKoeff
            stepY = (targetY - currentY) / timeKoeff
            stepZ = (targetZ - currentZ) / timeKoeff
            stepRX = (targetRX - currentRX) / timeKoeff
            stepRY = (targetRY - currentRY) / timeKoeff
            stepRZ = (targetRZ - currentRZ) / timeKoeff
            stepsCount = CInt(timeKoeff)
            currentStep = 0
            affectColor = False
            affectPosition = True
            affectRotate = True
            isRunning = True
        Else
            isRunning = False
            currentX = targetX
            currentY = targetY
            currentZ = targetZ
            currentRX = targetRX
            currentRY = targetRY
            currentRZ = targetRZ
        End If
    End Sub
    Public Sub SetTargetPC(ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal r As Single, ByVal g As Single, ByVal b As Single, ByVal time As Single, ByVal difference As Boolean)
        If difference Then
            targetX = x - currentX
            targetY = y - currentY
            targetZ = z - currentZ
            targetR = r + currentR
            targetG = g + currentG
            targetB = b + currentB
        Else
            targetX = x
            targetY = y
            targetZ = z
            targetR = r
            targetG = g
            targetB = b
        End If
        Dim timeKoeff As Single
        timeKoeff = time / (globalTickPeriod / 1000)
        startTime = Microsoft.VisualBasic.Timer
        endTime = time
        If CInt(timeKoeff) > 0 Then
            stepX = (targetX - currentX) / timeKoeff
            stepY = (targetY - currentY) / timeKoeff
            stepZ = (targetZ - currentZ) / timeKoeff
            stepR = (targetR - currentR) / timeKoeff
            stepG = (targetG - currentG) / timeKoeff
            stepB = (targetB - currentB) / timeKoeff
            stepsCount = CInt(timeKoeff)
            currentStep = 0
            affectColor = True
            affectPosition = True
            affectRotate = False
            isRunning = True
        Else
            currentX = targetX
            currentY = targetY
            currentZ = targetZ
            currentR = targetR
            currentG = targetG
            currentB = targetB
            MoveTarget()
            isRunning = False
            isFinished = True
        End If
    End Sub
    Public ReadOnly Property Running() As Boolean
        Get
            Return isRunning
        End Get
    End Property
    Public ReadOnly Property Finished() As Boolean
        Get
            Return isFinished
        End Get
    End Property
    Public Sub Tick()
        Static lastStep As Integer
        If isRunning Then
            If currentStep = 0 Then lastStep = 0
            lastStep = currentStep
            'currentStep += 1
            currentStep = (((Microsoft.VisualBasic.Timer - startTime) / endTime) * stepsCount)
            If currentStep > stepsCount Then currentStep = stepsCount
            If currentStep < stepsCount Then
                currentX += stepX * (currentStep - lastStep)
                currentY += stepY * (currentStep - lastStep)
                currentZ += stepZ * (currentStep - lastStep)
                currentRX += stepRX * (currentStep - lastStep)
                currentRY += stepRY * (currentStep - lastStep)
                currentRZ += stepRZ * (currentStep - lastStep)
                currentR += stepR * (currentStep - lastStep)
                currentG += stepG * (currentStep - lastStep)
                currentB += stepB * (currentStep - lastStep)
            End If
            If currentStep >= stepsCount Then
                Finish()
            End If
            MoveTarget()
        End If
    End Sub
    Public Sub Finish()
        If Not isFinished Then
            isRunning = False
            isFinished = True
            currentX = targetX
            currentY = targetY
            currentZ = targetZ
            currentRX = targetRX
            currentRY = targetRY
            currentRZ = targetRZ
            currentR = targetR
            currentG = targetG
            currentB = targetB
            MoveTarget()
        End If
    End Sub
    Private Sub MoveTarget()
        If targetType = targetTypeEnum.object3D Then
            If affectPosition Then
                targetObject.positionX = currentX
                targetObject.positionY = currentY
                targetObject.positionZ = currentZ
            End If
            If affectRotate Then
                targetObject.rotateX = currentRX
                targetObject.rotateY = currentRY
                targetObject.rotateZ = currentRZ
            End If
            If affectColor And Not (targetObject.light Is Nothing) Then
                targetObject.light.colorR = currentR
                targetObject.light.colorG = currentG
                targetObject.light.colorB = currentB
            End If
        End If
        If targetType = targetTypeEnum.camera3D Then
            If affectPosition Then
                targetCamera.PositionX = currentX
                targetCamera.PositionY = currentY
                targetCamera.PositionZ = currentZ
            End If
            If affectRotate Then
                targetCamera.Pitch = currentRX
                targetCamera.Yaw = currentRY
                targetCamera.Roll = currentRZ
            End If
        End If
    End Sub
    'Public Sub Tick()
    '    If isRunning Then
    '        currentStep += 1
    '        If currentStep < stepsCount Then
    '            If targetType = targetTypeEnum.object3D Then
    '                If affectPosition Then
    '                    targetObject.positionX += stepX
    '                    targetObject.positionY += stepY
    '                    targetObject.positionZ += stepZ
    '                End If
    '                If affectRotate Then
    '                    targetObject.rotateX += stepRX
    '                    targetObject.rotateY += stepRY
    '                    targetObject.rotateZ += stepRZ
    '                End If
    '                If affectColor And Not (targetObject.light Is Nothing) Then
    '                    targetObject.light.colorR += stepR
    '                    targetObject.light.colorG += stepG
    '                    targetObject.light.colorB += stepB
    '                End If
    '            End If
    '            If targetType = targetTypeEnum.camera3D Then
    '                If affectPosition Then
    '                    targetCamera.PositionX += stepX
    '                    targetCamera.PositionY += stepY
    '                    targetCamera.PositionZ += stepZ
    '                End If
    '                If affectRotate Then
    '                    targetCamera.Pitch += stepRX
    '                    targetCamera.Yaw += stepRY
    '                    targetCamera.Roll += stepRZ
    '                End If
    '            End If
    '        End If
    '        If currentStep = stepsCount Then
    '            isRunning = False
    '            isFinished = True
    '        End If
    '    End If
    'End Sub
End Class
