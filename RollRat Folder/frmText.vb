'/*************************************************************************
'
'   Copyright (C) 2015. rollrat. All Rights Reserved.
'
'   Author: HyunJun Jeong
'
'***************************************************************************/

Imports System.IO
Imports System.Text
Imports System.Drawing.Text

Public Class frmText

    Private TextsPages As New List(Of String)
    Dim pagesc As Integer = 0

    Private Sub frmText_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        '/////////////////////////
        Dim originw As Integer = Me.Width
        Dim originh As Integer = Me.Height
        For i As Integer = 25 To 0 Step -1
            Me.Height = originh * frmMain.tanh_value(i)
            'Application.DoEvents()
            Threading.Thread.Sleep(10)
        Next
        For i As Integer = 25 To 0 Step -1
            Me.Width = originw * frmMain.tanh_value(i)
            Threading.Thread.Sleep(10)
        Next
        '/////////////////////////
    End Sub

    Private Sub frmText_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text += " - " & frmMain.txt_addr
        Me.Show()
        '/////////////////////
        Dim originw As Integer = Me.Width
        Dim originh As Integer = Me.Height
        Me.Height = 0
        For i As Integer = 0 To 25
            Me.Width = originw * frmMain.tanh_value(i)
            Threading.Thread.Sleep(10)
        Next
        Me.Width = originw
        For i As Integer = 0 To 25
            Me.Height = originh * frmMain.tanh_value(i)
            Threading.Thread.Sleep(10)
        Next
        Me.Height = originh
        '/////////////////////
        Dim ret As String() = Nothing
        Dim sb As New StringBuilder
        ret = File.ReadAllLines(frmMain.txt_addr, System.Text.Encoding.Default)
        For Each strt As String In ret
            sb.Append(" " & strt & vbCrLf)
        Next

        TextMeasure(sb.ToString)
    End Sub

    Private Sub frmText_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then Me.Close()
        If e.KeyCode = Keys.Left Then
            If pagesc <> 0 Then
                pagesc -= 1
                PictureBox1.Refresh()
                NumericUpDown1.Value = pagesc + 1
            End If
        ElseIf e.KeyCode = Keys.Right Then
            If pagesc < TextsPages.Count - 1 Then
                pagesc += 1
                PictureBox1.Refresh()
                NumericUpDown1.Value = pagesc + 1
            End If
        End If
    End Sub

    Private Sub PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox1.Paint

        Dim font As Font = New Font(TextBox1.Font, TextBox1.Font.Style)
        Dim stf As New StringFormat

        stf.Trimming = StringTrimming.Word
        stf.Alignment = StringAlignment.Near
        'e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias
        e.Graphics.DrawString(TextsPages(pagesc), font, Brushes.Black, New RectangleF(30, 30, PictureBox1.Width - 55, PictureBox1.Height - 55), stf)

        stf.Alignment = StringAlignment.Center
        e.Graphics.DrawString("- " & (pagesc + 1).ToString & " -", font, Brushes.Black, New RectangleF(0, PictureBox1.Height - 40, PictureBox1.Width, PictureBox1.Height), stf)

    End Sub

    Private Sub TextMeasure(ByVal filetext As String)

        Dim gp As Graphics = PictureBox1.CreateGraphics
        Dim font As Font = New Font(TextBox1.Font, TextBox1.Font.Style)
        Dim stf As New StringFormat
        Dim chars As Integer
        Dim lines As Integer

        stf.Trimming = StringTrimming.Word
        stf.Alignment = StringAlignment.Near

        ' Fast Split Method

        Dim prev As String = 0

        Do
            gp.MeasureString(filetext.Substring(prev, 2000), font, New SizeF(PictureBox1.Width - 30, PictureBox1.Width - 40), stf, chars, lines)
            TextsPages.Add(filetext.Substring(prev, chars))
            prev = prev + chars
            Label1.Text = "/" & TextsPages.Count
            Application.DoEvents()
        Loop While prev + 2000 < filetext.Length - 1
        filetext = filetext.Substring(prev)
        Do
            gp.MeasureString(filetext, font, New SizeF(PictureBox1.Width - 30, PictureBox1.Width - 30), stf, chars, lines)
            TextsPages.Add(filetext.Substring(0, chars))
            filetext = filetext.Substring(chars)
            Label1.Text = "/" & TextsPages.Count
            Application.DoEvents()
        Loop While filetext.Length > 0

        NumericUpDown1.Maximum = TextsPages.Count

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If pagesc <> 0 Then
            pagesc -= 1
            PictureBox1.Refresh()
            NumericUpDown1.Value = pagesc + 1
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If pagesc < TextsPages.Count - 1 Then
            pagesc += 1
            PictureBox1.Refresh()
            NumericUpDown1.Value = pagesc + 1
        End If
    End Sub

    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        PictureBox1.Refresh()
        pagesc = NumericUpDown1.Value - 1
        PictureBox1.Refresh()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If FolderBrowserDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            Dim folderExists As Boolean
            folderExists = My.Computer.FileSystem.DirectoryExists(FolderBrowserDialog1.SelectedPath & "\" & Path.GetFileNameWithoutExtension(frmMain.txt_addr))
            If folderExists Then
                MsgBox("계속할 수 없습니다. 이미 처리되었거나 중복된 폴더가 있습니다.", MsgBoxStyle.Critical)
            Else
                My.Computer.FileSystem.CreateDirectory(FolderBrowserDialog1.SelectedPath & "\" & Path.GetFileNameWithoutExtension(frmMain.txt_addr))
                Dim folder As String = FolderBrowserDialog1.SelectedPath & "\" & Path.GetFileNameWithoutExtension(frmMain.txt_addr) & "\"
                ProgressBar1.Maximum = TextsPages.Count - 1
                ProgressBar1.Visible = True
                Label2.Visible = True
                Button3.Enabled = False
                For i As Integer = 0 To TextsPages.Count - 1
                    Dim bitmap As New Bitmap(PictureBox1.Width, PictureBox1.Height)
                    Dim fgraphics As Graphics = Graphics.FromImage(bitmap)

                    For x As Integer = 0 To bitmap.Width - 1
                        For y As Integer = 0 To bitmap.Height - 1
                            bitmap.SetPixel(x, y, Color.White)
                        Next
                    Next

                    Dim font As Font = New Font(TextBox1.Font, TextBox1.Font.Style)
                    Dim stf As New StringFormat

                    stf.Trimming = StringTrimming.Word
                    stf.Alignment = StringAlignment.Near
                    fgraphics.DrawString(TextsPages(i), font, Brushes.Black, New RectangleF(30, 30, PictureBox1.Width - 55, PictureBox1.Height - 55), stf)

                    stf.Alignment = StringAlignment.Center
                    fgraphics.DrawString("- " & (i + 1).ToString & " -", font, Brushes.Black, New RectangleF(0, PictureBox1.Height - 40, PictureBox1.Width, PictureBox1.Height), stf)

                    Dim ctrtxt As String = CStr(i + 1)
                    Dim len As Integer = CInt(TextsPages.Count.ToString.Length) - ctrtxt.Length
                    For ji = 1 To len
                        ctrtxt = "0"c & ctrtxt
                    Next

                    bitmap.Save(folder & ctrtxt & ".jpeg", System.Drawing.Imaging.ImageFormat.Jpeg)
                    bitmap.Dispose()
                    ProgressBar1.Value = i
                    Label2.Text = (i + 1).ToString & "/" & TextsPages.Count
                    Application.DoEvents()
                Next
                Label2.Visible = False
                ProgressBar1.Visible = False
                Button3.Enabled = True
            End If
        End If
    End Sub

End Class