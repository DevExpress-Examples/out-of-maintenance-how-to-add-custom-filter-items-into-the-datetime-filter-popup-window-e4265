Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Forms

Namespace DateRange
    Friend NotInheritable Class Program

        Private Sub New()
        End Sub

        <STAThread> _
        Shared Sub Main()

            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            Application.Run(New Form1())
        End Sub
    End Class
End Namespace
