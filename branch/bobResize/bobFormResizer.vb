Imports System.Windows.Forms
Public Class bobFormResizer
   Inherits System.Windows.Forms.UserControl

   Dim controlAttributes(6, 0) As String
   'c(0,n) = name
   'c(1,n) = left
   'c(2,n) = top
   'c(3,n) = width
   'c(4,n) = height
   'c(5,n) = font.size
   'c(6,n) = font.style
   Dim xx, yy As Long


#Region " Windows Form Designer generated code "

   Public Sub New()
      MyBase.New()

      'This call is required by the Windows Form Designer.
      InitializeComponent()


      'Add any initialization after the InitializeComponent() call

   End Sub

   'UserControl1 overrides dispose to clean up the component list.
   Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
      If disposing Then
         If Not (components Is Nothing) Then
            components.Dispose()
         End If
      End If
      MyBase.Dispose(disposing)
   End Sub

   'Required by the Windows Form Designer
   Private components As System.ComponentModel.IContainer

   'NOTE: The following procedure is required by the Windows Form Designer
   'It can be modified using the Windows Form Designer.  
   'Do not modify it using the code editor.
   <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
      '
      'UserControl1
      '
      Me.Name = "UserControl1"
      Me.Size = New System.Drawing.Size(88, 72)

   End Sub

#End Region




   Private Sub setInitialSizes(ByVal inObject As Object)
      Dim i, k As Integer
      Dim ctl As Control
      For Each ctl In inObject.Controls
         If ctl.Name = "" Then Exit For
         k = controlAttributes.GetUpperBound(1)
         ReDim Preserve controlAttributes(6, k + 1)
         controlAttributes(0, k + 1) = ctl.Name
         controlAttributes(1, k + 1) = ctl.Left
         controlAttributes(2, k + 1) = ctl.Top
         controlAttributes(3, k + 1) = ctl.Width
         controlAttributes(4, k + 1) = ctl.Height
         controlAttributes(5, k + 1) = ctl.Font.Size
         controlAttributes(6, k + 1) = ctl.Font.Style

         'because a control can own other controls we want to get all
         'controls on the form not just the controls owned by the form.
         'This is why we are using inObject instead of inForm
         i = ctl.Controls.Count
         If i > 0 Then
            setInitialSizes(ctl)
         End If
      Next ctl
   End Sub

   Public Sub resizeMe()

      If Not IsNothing(myForm) Then
         Dim newX, newY As Double
         Dim numInArray As Integer
         Dim currentControl As Integer



         newX = myForm.ClientSize.Width / xx
         newY = myForm.ClientSize.Height / yy
         numInArray = controlAttributes.GetUpperBound(1)


         For currentControl = 0 To numInArray
            reSizeControl(myForm, controlAttributes(0, currentControl), currentControl, newX, newY)
         Next
      End If
   End Sub
   ' the form to be resized
   Public Property Form() As System.Windows.Forms.Form
      Get
         Return myForm
      End Get
      Set(ByVal Value As System.Windows.Forms.Form)
         If Not IsNothing(Value) Then
            myForm = Value
            Debug.Write("inside set form")
            xx = myForm.ClientSize.Width
            yy = myForm.ClientSize.Height
            setInitialSizes(myForm)
         End If
      End Set
   End Property

   Public Sub reSizeControl(ByVal inObject As Object, _
                             ByVal objectName As String, _
                             ByVal numCtrl As Integer, _
                             ByVal newX As Double, _
                             ByVal newY As Double)

      Dim ctrl As Control


      For Each ctrl In inObject.controls
         Try

            If ctrl.Name = objectName Then
               ctrl.Left = controlAttributes(1, numCtrl) * newX
               ctrl.Top = controlAttributes(2, numCtrl) * newY
               ctrl.Width = controlAttributes(3, numCtrl) * newX
               ctrl.Height = controlAttributes(4, numCtrl) * newY
               If newX < newY Then
                  ctrl.Font = New System.Drawing.Font(ctrl.Font.Name, controlAttributes(5, numCtrl) * newX)
               Else
                  ctrl.Font = New System.Drawing.Font(ctrl.Font.Name, controlAttributes(5, numCtrl) * newY)
               End If
               ctrl.Font = New System.Drawing.Font(ctrl.Font, controlAttributes(6, numCtrl))
               Exit For
            Else
               reSizeControl(ctrl, objectName, numCtrl, newX, newY)
            End If

         Catch ex As Exception

         End Try

      Next

   End Sub

   Private myForm As System.Windows.Forms.Form
End Class
