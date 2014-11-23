Public Class TextMessage
    Public text As String
    Public font As BitmapFont
    Public red, green, blue As Integer
    Public startAlpha, endAlpha As Single
    Public alpha As Single
    Private stepX, stepY As Single
    Public currentX, currentY As Single
    Private currentStep, stepsCount As Integer
    Private isActive As Boolean
    Private isFinished As Boolean
    Public ReadOnly Property Active() As Boolean
        Get
            Return isActive
        End Get
    End Property
    Public ReadOnly Property Finished() As Boolean
        Get
            Return isFinished
        End Get
    End Property
    Public Sub Start(ByVal x As Integer, ByVal y As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal time As Single)
        currentX = x
        currentY = y
        Dim timeKoeff As Single
        timeKoeff = time / (globalTickPeriod / 1000)
        If timeKoeff > 0 Then
            stepX = dx / timeKoeff
            stepY = dy / timeKoeff
            stepsCount = time * 1000 / globalTickPeriod
            currentStep = 0
            alpha = startAlpha
            isActive = True
        Else
            stepsCount = Integer.MaxValue
            currentStep = 0
            stepX = 0
            stepY = 0
            alpha = endAlpha
            isActive = True
        End If
    End Sub
    Public Sub Tick()
        If isActive Then
            currentStep += 1
            If currentStep < stepsCount Then
                currentX += stepX
                currentY += stepY
                alpha = startAlpha + (currentStep / stepsCount) * (endAlpha - startAlpha)
            End If
            If currentStep = stepsCount Then
                Finish()
                alpha = endAlpha
            End If
        End If
    End Sub
    Public Sub Finish()
        If Not isFinished Then
            isActive = False
            isFinished = True
        End If
    End Sub
End Class
