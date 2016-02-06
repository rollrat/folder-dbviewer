'/*************************************************************************
'
'   Copyright (C) 2015. rollrat. All Rights Reserved.
'
'   Author: HyunJun Jeong
'
'***************************************************************************/

Imports System.IO
Imports System.Collections.ObjectModel
Imports System.Text

Public Class frmMain

    Public FolderGrid As New ArrayList
    Private count As Integer
    Private search_time As Boolean = False
    Private miss_count As Integer
    Private last_sdb_addr As String
    Private FirstLoad As Boolean = True

    ' 이 값은 Math.tanh(i / 7)의 함숫값임.
    Public ReadOnly tanh_value As Double() = {0, 0.141893193766933, 0.278185490325702, 0.404126739757859, 0.51640765518518,
            0.613357260395383, 0.694782670314738, 0.761594155955765, 0.815373942495404, 0.857999961698402,
            0.891373467734719, 0.917252697879849, 0.93717125793686, 0.952414115203671, 0.964027580075817,
            0.972846166112511, 0.979525425675133, 0.984574576974176, 0.988385883107241, 0.991259640913111,
            0.993424677228132, 0.99505475368673, 0.996281474641925, 0.997204320833022, 0.997898380495443, 0.998420268116527}

    '
    '   모든 숫자를 같게하면 모든 컨트롤들이 하나같이 움직이게 되므로 시각효과가 떨어짐
    '   PictureBox1은 시작시엔 보이지 않으므로 0으로 설정함
    '
    Private control_list As List(Of Object)
    '3, 1, 2, 4, 6, 5, 10, 9, 11
    Private ReadOnly control_appear As Integer() = {500, 600, 400, 700, 900, 800, 1000, 900, 1100, _
                                                     500, 700, 300, 400, 1200, 600, 700, 600, 700, 1000, 500, 0}
    Private ReadOnly control_disappear As Integer() = {1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900, _
                                                       1000, 1500, 1100, 1200, 1900, 1400, 1500, 1600, 1700, 1700, 1300, 2000}

    Public Shared Function CompactString(ByVal MyString As String, ByVal Width As Integer,
                    ByVal Font As Drawing.Font,
                    ByVal FormatFlags As TextFormatFlags) As String

        Dim Result As String = String.Copy(MyString)

        TextRenderer.MeasureText(Result, Font, New Drawing.Size(Width, 0),
            FormatFlags Or TextFormatFlags.ModifyString)

        Return Result


    End Function

    Public Shared Function ParseExtension(ByVal strExtension As String)
        Dim pic_extension As String() = strExtension.Split("|"c)
        Return pic_extension
    End Function

    Public Shared pic_extension As String()

    Public Structure _Bookmark_Data
        Dim hash As String
        Dim addrs As String()
    End Structure

    Public Shared book_mark_all As New List(Of _Bookmark_Data)

    Private Sub frmStatistics_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text += Version.VersionText
        pic_extension = ParseExtension(".jpg|.jpeg|.png|.bmp|.gif")

        Dim fileExists As Boolean
        fileExists = My.Computer.FileSystem.FileExists(Application.StartupPath() & "\bookmark.dat")
        If fileExists = True Then
            Dim ret As String() = Nothing
            ret = File.ReadAllLines(Application.StartupPath() & "\bookmark.dat")
            For Each bmdata As String In ret
                If bmdata.Length <> 0 Then
                    Dim spt As String() = bmdata.Split("|"c)
                    Dim hash As String = spt(0)
                    Dim addrs As String() = spt(1).Split("?"c)
                    Dim bmkt As _Bookmark_Data
                    bmkt.hash = hash
                    bmkt.addrs = addrs
                    book_mark_all.Add(bmkt)
                End If
            Next
        End If

        '
        '   프로그레스바는 제외함
        '
        control_list = New List(Of Object)({Button1, Button2, Button3, Button4, Button5, Button6, _
           Button9, Button10, Button11, TreeView1, ListBox1, Label1, Label2, Label7, Label8, Label9, Label10, _
           Label11, TextBox2, TextBox1, PictureBox1})
        For Each sControls As Object In control_list
            sControls.visible = False
        Next

        Me.Show()
        ' 효과 상세는 frmPicviewer참고
        Dim custlocx As Integer = Me.Location.X + Me.Width / 2
        Dim locationx As Integer = Me.Location.X
        Dim locationy As Integer = Me.Location.Y
        Dim originw As Integer = Me.Width
        Dim originh As Integer = Me.Height
        Me.Height = 0
        For i As Integer = 0 To 25
            Me.Location = New Point(custlocx - originw * tanh_value(i) / 2, locationy)
            Me.Width = originw * tanh_value(i)
            Application.DoEvents()
            Threading.Thread.Sleep(10)
        Next
        Me.Focus()
        Me.Location = New Point(locationx, locationy)
        Me.Width = originw
        Dim destx As New List(Of Integer)
        Dim senderx As New List(Of Integer)
        Dim sendery As New List(Of Integer)
        For i As Integer = 0 To control_list.Count - 1
            destx.Add(control_list(i).Location.X)
            senderx.Add(control_list(i).Location.X + control_appear(i))
            sendery.Add(control_list(i).Location.Y)
        Next
        For i As Integer = 0 To 25
            '
            '   컨트롤이 빠르게 날라와 천천히 정착하는 효과를 만들어 냅니다.
            '
            For j As Integer = 0 To control_list.Count - 1
                control_list(j).visible = True
                control_list(j).Location = New Point(destx(j) + senderx(j) * (1 - tanh_value(i)), sendery(j))
            Next
            Me.Height = originh * tanh_value(i)
            Application.DoEvents()
            Threading.Thread.Sleep(20)
        Next
        For i As Integer = 0 To control_list.Count - 1
            control_list(i).Location = New Point(destx(i), sendery(i))
        Next
        Me.Height = originh
    End Sub

    Private Sub SaveBookmark()

        ' Find bookmark datas in all data.
        Dim isexist As Boolean = False
        Dim count As Integer = 0
        For Each bmkt As _Bookmark_Data In book_mark_all
            If bmkt.hash = MD5Str(FolderGrid(0)) Then
                isexist = True
                bmkt.addrs = book_mark.ToArray()
                book_mark_all(count) = bmkt
                Exit For
            End If
            count += 1
        Next

        If Not isexist Then
            Dim hash As String = MD5Str(FolderGrid(0))
            Dim addrs As String() = book_mark.ToArray()
            Dim bmkt As _Bookmark_Data
            bmkt.hash = hash
            bmkt.addrs = addrs
            book_mark_all.Add(bmkt)
        End If

    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Dim fileExists As Boolean
        fileExists = My.Computer.FileSystem.FileExists(Application.StartupPath() & "\bookmark.dat")
        If fileExists = False Then
            My.Computer.FileSystem.WriteAllText(Application.StartupPath() & "\bookmark.dat", String.Empty, False)
        End If

        If FolderGrid.Count = 0 Then
            GoTo CloseEffect
        End If

        SaveBookmark()

        Dim strbuilder As New StringBuilder

        'Data Format:
        'MD5 HASH|Addr Data?Addr Data?...?...
        For Each bmkt As _Bookmark_Data In book_mark_all
            If bmkt.addrs.Length <> 0 Then
                Dim strsmallbuilder As New StringBuilder
                strsmallbuilder.Append(bmkt.hash & "|"c)
                For i As Integer = 0 To bmkt.addrs.Length - 1
                    strsmallbuilder.Append(bmkt.addrs(i))
                    If i <> bmkt.addrs.Length - 1 Then
                        strsmallbuilder.Append("?"c)
                    End If
                Next
                strbuilder.Append(strsmallbuilder)
                strbuilder.Append(vbCrLf)
                strsmallbuilder.Clear()
            End If
        Next

        My.Computer.FileSystem.WriteAllText( _
            Application.StartupPath() & "\bookmark.dat", strbuilder.ToString, False)
        strbuilder.Clear()
        book_mark.Clear()

CloseEffect:
        Dim originw As Integer = Me.Width
        Dim originh As Integer = Me.Height
        Dim destx As New List(Of Integer)
        Dim senderx As New List(Of Integer)
        Dim sendery As New List(Of Integer)
        For i As Integer = 0 To control_list.Count - 1
            destx.Add(control_list(i).Location.X)
            senderx.Add(control_list(i).Location.X + control_disappear(i))
            sendery.Add(control_list(i).Location.Y)
        Next
        For i As Integer = 25 To 0 Step -1
            '
            '   컨트롤이 천천히 이륙해 빠르게 날라가는 효과를 만들어 냅니다.
            '
            For j As Integer = 0 To control_list.Count - 1
                control_list(j).Location = New Point(destx(j) + senderx(j) * (1 - tanh_value(i)), sendery(j))
            Next
            Me.Height = originh * tanh_value(i)
            Application.DoEvents()
            Threading.Thread.Sleep(20)
        Next
        Dim custlocx As Integer = Me.Location.X + Me.Width / 2
        Dim locationx As Integer = Me.Location.X
        Dim locationy As Integer = Me.Location.Y
        For i As Integer = 25 To 0 Step -1
            Me.Location = New Point(custlocx - originw * tanh_value(i) / 2, locationy)
            Me.Width = originw * tanh_value(i)
            Threading.Thread.Sleep(10)
        Next

        End
    End Sub

    '
    '   트리뷰의 주소를 이용하여 특정한 노드를 찾아 확장합니다.
    '
    Dim deepinloop As Integer
    Dim deepinstr As String()
    Public Sub ExpandByFullPath(ByVal str As String)
        Dim strstr As String() = str.Split("\"c)
        For Each tn As TreeNode In TreeView1.Nodes
            If tn.Text = strstr(0) Then
                deepinloop = 0
                deepinstr = strstr
                __internal_iterate_expandbyfullpath(tn)
            End If
        Next
    End Sub
    Public Sub __internal_iterate_expandbyfullpath(node As TreeNode)
        node.Expand()
        If deepinloop = deepinstr.Length - 1 Then
            TreeView1.Focus()
            TreeView1.SelectedNode = node
            Exit Sub
        End If
        deepinloop += 1
        For Each tn As TreeNode In node.Nodes
            If tn.Text = deepinstr(deepinloop) Then
                __internal_iterate_expandbyfullpath(tn)
            End If
        Next
    End Sub
    Public Sub FindAndExpand(ByVal addr As String)
        ExpandByFullPath(addr.Substring(top_addr.Length))
    End Sub
    Public Sub RequestFileViewAlternative(ByVal addr As String)
        FindAndExpand(addr)
        Button5.PerformClick()
    End Sub
    Public Sub RequestFolderViewAlternative(ByVal addr As String)
        ' Get folder's directory
        FindAndExpand(Path.GetDirectoryName(addr))
        Button6.PerformClick()
    End Sub

    '
    '   PicViewer 모아보기
    '
    Private on_menukey As Boolean = False
    Private origin_title As String
    Private Sub frmStatistics_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Menu AndAlso on_menukey = False Then
            'origin_title = Me.Text
            'Me.Text += " - 북마크 -"
            'on_menukey = True
        End If
        If e.KeyCode = Keys.Escape Then Me.Close()
        If e.Modifiers = Keys.Control Then
            If e.KeyCode = Keys.L Then
                hfrmControlLab.Show()
            ElseIf e.KeyCode = Keys.S Then
                If frmSearch.Visible Then
                    frmSearch.Close()
                End If
                frmSearch.Show()
            End If
        End If
    End Sub
    Private Sub frmMain_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.Menu Then
            'Me.Text = origin_title
            'on_menukey = False
        End If
    End Sub

    '
    '   모든 리스트를 트리뷰에 나열합니다.
    '
    Dim globalcount As Integer
    Dim perpect_then_exit As Boolean = False
    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Dim prog As Integer = Math.Round((count / FolderGrid.Count) * 100)
        ProgressBar1.Value = prog
        Application.DoEvents()
    End Sub
    Private Sub list_treeview()

        Timer2.Interval = 30
        Timer2.Start()
        count = 0
        miss_count = 0
        perpect_then_exit = False

        Dim j As Integer = 0

        '
        '   DB파일의 최상위 폴더를 가져옴
        '
        top_addr = Path.GetDirectoryName(FolderGrid(0)) & "\"
        If top_addr.EndsWith("\\") Then

            '
            '   최상위 폴더(드라이브 폴더)에 포함된 폴더에 대한 대비
            '
            top_addr = top_addr.Substring(0, top_addr.Length - 1)
        End If
        For i As Integer = 0 To FolderGrid.Count - 1
            count = i
            Application.DoEvents()
            TreeView1.Nodes.Add(Path.GetFileName(FolderGrid(i)))

            '
            '   주소 맨 끝에 \를 추가하여 해당 폴더가 포함되어있는지 확인합니다.
            '   ex) D:\2014\rollrat에 D:\2014가 포함되어있는지 알아보려면,
            '       D:\2014\가 포함되어있는지 알아보는 것이 D:\2014보다 정확합니다.
            '       가령, D:\2014-abs라는 폴더는 D:\2014가 포함된 주소이므로,
            '       Contains함수는 해당 주소가 포함되어있다고 True가 반환됩니다.
            '
            If CType(FolderGrid(i + 1), String).Contains(FolderGrid(i) & "\") Then
                globalcount = i
                integernal_list_treeview(FolderGrid(i), TreeView1.Nodes(j))
            End If

            i = globalcount
            j += 1
        Next

        Timer2.Stop()
    End Sub
    Private Sub integernal_list_treeview(ByVal obstr As String, ByVal panode As TreeNode)
        Application.DoEvents()
        For i As Integer = globalcount + 1 To FolderGrid.Count - 1
            count = i
            If CType(FolderGrid(i), String).Contains(obstr & "\") Then
                Dim folderExists As Boolean
                folderExists = My.Computer.FileSystem.DirectoryExists(FolderGrid(i))
                If Not folderExists Then
                    miss_count += 1
                    Continue For
                End If

                Label2.Text = CompactString(Path.GetFileName(FolderGrid(i)), Me.Location.X - Label2.Location.X, Label2.Font, TextFormatFlags.PathEllipsis)
                Dim chnode As New TreeNode(Path.GetFileName(FolderGrid(i)))
                panode.Nodes.Add(chnode)
                If CType(FolderGrid(i + 1), String).Contains(FolderGrid(i) & "\") Then
                    globalcount = i

                    '
                    '   globalcount 는 FolderGrid.Count - 1이 되면 이 부분에서 무한 재귀를 일으키므로
                    '   perpect_then_exit전역변수를 만들어 모든 재귀 루프가 끝나도록 설정함
                    '
                    If globalcount = FolderGrid.Count - 1 Then
                        perpect_then_exit = True
                        Exit Sub
                    End If
                    integernal_list_treeview(FolderGrid(i), chnode)
                    If perpect_then_exit Then
                        Exit Sub
                    End If
                    i = globalcount - 1
                End If
            Else
                globalcount = i
                Exit Sub
            End If
        Next
    End Sub

    '
    '   DB의 최상위 항목(최상위층 폴더)
    '
    Public top_addr As String = ""

    '
    '   현재 트리뷰에 선택된 폴더의 물리적 주소(윈도우즈 실제주소)
    '
    Public Shared current_addr As String

    '
    '   listbox_item_text: 현재 선택된 폴더에 포함된 모든 파일을 리스팅한 리스트
    '
    Public Shared listbox_item_text As New ArrayList
    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect
        current_addr = top_addr & TreeView1.SelectedNode.FullPath

        If on_menukey Then
            For Each info As String In book_mark
                If info = current_addr Then
                    Me.Text += " - 북마크에 이미 추가된 항목입니다." & TreeView1.SelectedNode.FullPath
                    Exit Sub
                End If
            Next
            book_mark.Add(current_addr)
            Me.Text += " - 북마크에 항목이 추가되었습니다." & TreeView1.SelectedNode.FullPath
            Exit Sub
        End If

        '
        '   트리뷰 클릭시 리스트 박스 아이템 재설정
        '
        ListBox1.Items.Clear()
        listbox_item_text.Clear()
        Dim files As ReadOnlyCollection(Of String)
        files = My.Computer.FileSystem.GetFiles(current_addr, FileIO.SearchOption.SearchTopLevelOnly, "*.*")
        Label9.Text = files.Count
        For Each x In files
            listbox_item_text.Add(Path.GetFileName(x))
            ListBox1.Items.Add(Path.GetFileName(x))
        Next
        Application.DoEvents()

        '
        '   pic_extension에 포함된 확장자는 미리보기를 지원하여 보여줌
        '   모든 리스트 박스 아이템을 전부 찾아서 확장자가 있는지 확인함
        '   없다면 선택인덱스를 첫 번째 아이템으로 둠
        '
        Dim j = 0
        If files.Count > 0 Then
RE:
            ListBox1.SelectedIndex = j
            If Not pic_extension.Contains(IO.Path.GetExtension(ListBox1.SelectedItem).ToLower) Then
                j += 1
                If files.Count > j Then
                    GoTo RE
                Else
                    ListBox1.SelectedIndex = 0
                End If
            End If
        Else
            If Me.Size.Width <> 624 Then
                If search_time Then
                    FormSizeEffectingAuto(New Point(624, 561))
                Else
                    FormSizeEffectingAuto(New Point(624, 534))
                End If
            End If
        End If

        '
        '   Folder Count
        '
        Label10.Text = Directory.GetDirectories(current_addr).Length
    End Sub

    Private Sub TreeView1_DoubleClick(sender As Object, e As EventArgs) Handles TreeView1.DoubleClick
        If Not TreeView1.SelectedNode Is Nothing Then
            Process.Start(top_addr & TreeView1.SelectedNode.FullPath)
        End If
    End Sub

    Private Sub ListBox1_DoubleClick(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        If Not ListBox1.SelectedItem Is Nothing Then
            Dim addr As String = current_addr & "\" & ListBox1.SelectedItem
            If IO.Path.GetExtension(addr).ToLower = ".txt" Then
                txt_addr = addr
                frmText.Show()
                Exit Sub
            End If

            Process.Start(current_addr & "\" & ListBox1.SelectedItem)
        End If
    End Sub

    Public Shared txt_addr As String
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        '
        '   선택된 파일의 물리적 주소
        '
        Dim addr As String = current_addr & "\" & ListBox1.SelectedItem

        If pic_extension.Contains(IO.Path.GetExtension(addr).ToLower) Then
            '624, 561
            '1003, 561
            '624, 534
            '1003, 534
            '965, 534
            Dim img As Image = Image.FromFile(addr)
            If Not PictureBox1.Image Is Nothing Then
                PictureBox1.Image.Dispose()
            End If
            PictureBox1.Image = img
            pic_addr = addr
            If search_time Then
                FormSizeEffectingAuto(New Point(965, 561), False)
            Else
                FormSizeEffectingAuto(New Point(965, 534), False)
            End If
            Exit Sub
        End If

        If Me.Size.Width <> 624 Then
            If search_time Then
                FormSizeEffectingAuto(New Point(624, 561))
            Else
                FormSizeEffectingAuto(New Point(624, 534))
            End If
        End If
    End Sub

    Public Shared book_mark As New List(Of String)

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        '
        '   북마크 중복검사 실행후 해당 북마크를 북마크 리스트에 추가합니다.
        '
        If Not TreeView1.SelectedNode Is Nothing Then
            If Not book_mark.Contains(top_addr & TreeView1.SelectedNode.FullPath) Then
                book_mark.Add(top_addr & TreeView1.SelectedNode.FullPath)
            End If
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim svp As String

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            svp = FolderBrowserDialog1.SelectedPath
        Else
            Exit Sub
        End If

        If Not PictureBox1.Image Is Nothing Then
            PictureBox1.Image.Dispose()
        End If

        For Each sr As String In book_mark
            'Directory.Move(sr & "\", svp)
            'My.Computer.FileSystem.CreateDirectory(svp & "\" & Path.GetFileName(sr))
            My.Computer.FileSystem.MoveDirectory(sr & "\", svp & "\" & Path.GetFileName(sr))
        Next
        book_mark.Clear()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim svp As String

        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            svp = FolderBrowserDialog1.SelectedPath
        Else
            Exit Sub
        End If

        If Not PictureBox1.Image Is Nothing Then
            PictureBox1.Image.Dispose()
        End If

        For Each sr As String In book_mark
            My.Computer.FileSystem.CopyDirectory(sr & "\", svp & "\" & Path.GetFileName(sr))
        Next
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        frmBookmark.Show()
    End Sub

    Public Shared pic_addr As String
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        frmPicPreview.Show()
    End Sub

    Dim df_Directories As New ArrayList
    Dim df_stringb As New StringBuilder
    Dim n_str As String

    Sub df_approach(fdrectory As String)
        n_str = fdrectory
        df_stringb.Append(fdrectory & "*")
        df_Directories.Add(fdrectory)
        If Directory.GetDirectories(fdrectory).Length Then
            For Each recu As String In Directory.GetDirectories(fdrectory)
                If Directory.Exists(recu) Then
                    df_approach(recu)
                End If
                Application.DoEvents()
            Next
        End If
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Dim svp As String
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            svp = FolderBrowserDialog1.SelectedPath
        Else
            Exit Sub
        End If
        Button10.Enabled = False
        Button9.Enabled = False
        Button11.Enabled = False

        '//////////////

        'PictureBox 못 누르게 설정함
        FormSizeEffectingAuto(New Point(624, 561))

        My.Computer.FileSystem.CreateDirectory(System.IO.Directory.GetCurrentDirectory & "\db")
        ProgressBar1.Visible = True
        Label1.Visible = True
        Label2.Visible = True
        ProgressBar1.Style = ProgressBarStyle.Marquee
        search_time = True
        Timer3.Interval = 50
        Timer3.Start()

        df_approach(svp)

        Timer3.Stop()
        df_Directories.Clear()
        ProgressBar1.Style = ProgressBarStyle.Blocks
        ProgressBar1.Visible = False
        Label1.Visible = False
        Label2.Visible = False
        search_time = False

        '//////////////

        Dim fileExists As Boolean
        fileExists = My.Computer.FileSystem.FileExists(System.IO.Directory.GetCurrentDirectory & "\db\" & _
                                                       Path.GetFileName(FolderBrowserDialog1.SelectedPath) & ".sdb")
        If fileExists = True Then
            If MsgBox("이미 파일이 있습니다. 덮어쓰시겠습니까?", MsgBoxStyle.Exclamation Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                My.Computer.FileSystem.DeleteFile(System.IO.Directory.GetCurrentDirectory & "\db\" & _
                                Path.GetFileName(FolderBrowserDialog1.SelectedPath) & ".sdb", _
                                FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
                fileExists = False
            End If
        End If

        If fileExists Then
            Return
        End If
        My.Computer.FileSystem.CreateDirectory(System.IO.Directory.GetCurrentDirectory & "\tmp")
        File.WriteAllText(System.IO.Directory.GetCurrentDirectory & "\tmp\file.tmp", df_stringb.ToString)
        df_stringb.Clear()
        IO.Compression.ZipFile.CreateFromDirectory(System.IO.Directory.GetCurrentDirectory & "\tmp", _
                                                   System.IO.Directory.GetCurrentDirectory & "\db\" & _
                                                   Path.GetFileName(FolderBrowserDialog1.SelectedPath) & ".sdb")
        My.Computer.FileSystem.DeleteFile(System.IO.Directory.GetCurrentDirectory & "\tmp\file.tmp", _
                                          FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        Directory.Delete(System.IO.Directory.GetCurrentDirectory & "\tmp")

        FormSizeEffectingAuto(New Point(Me.Size.Width, 534))

        Button10.Enabled = True
        Button9.Enabled = True
        Button11.Enabled = True

        MsgBox("데이터베이스 작성이 완료되었습니다.", MsgBoxStyle.Information)
    End Sub

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        Label1.Text = "Status : " & df_Directories.Count
        'Label2.Text = n_str
        Label2.Text = CompactString(n_str, Me.Location.X - Label2.Location.X, Label2.Font, TextFormatFlags.PathEllipsis)

        Application.DoEvents()
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Dim f = OpenFileDialog1.ShowDialog()

        If f = DialogResult.OK Then
            last_sdb_addr = OpenFileDialog1.FileName
        Else
            Exit Sub
        End If

        Button10.Enabled = False
        Button9.Enabled = False
        Button11.Enabled = False

        list_array(last_sdb_addr)

        Button11.Enabled = True

        If miss_count > 5 Then
            If MsgBox("SDB가 오래된 것 같습니다. 새로만드시겠습니까?", MsgBoxStyle.Information _
                      Or MsgBoxStyle.YesNo, "RollRat Folder") = MsgBoxResult.Yes Then
                Button11.Enabled = False
                ListBox1.Items.Clear()
                TreeView1.Nodes.Clear()
                auto_rebuild_sdb()
                list_array(last_sdb_addr)
                Button11.Enabled = True
            End If
        End If
        Button10.Enabled = True
        Button9.Enabled = True
    End Sub

    Private Sub list_array(ByVal svp As String)
        FolderGrid.Clear()

        '
        '   \tmp를 생성하여 압축을 풀은 뒤 데이터를 모두 읽고 \tmp삭제
        '
        My.Computer.FileSystem.CreateDirectory(System.IO.Directory.GetCurrentDirectory & "\tmp")
        IO.Compression.ZipFile.ExtractToDirectory(svp, System.IO.Directory.GetCurrentDirectory & "\tmp")
        Try
            FolderGrid.AddRange(File.ReadAllText(System.IO.Directory.GetCurrentDirectory & "\tmp\file.tmp").Split({"*"c}))
        Catch ex As Exception
            'MsgBox("이 데이터베이스는 너무 커서 x86모드로 열 수 없습니다.", MsgBoxStyle.Critical)
            MsgBox("이 데이터베이스는 너무 커서 열 수 없습니다.", MsgBoxStyle.Critical)
            Exit Sub
        End Try
        My.Computer.FileSystem.DeleteFile(System.IO.Directory.GetCurrentDirectory & "\tmp\file.tmp", _
                                           FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        Directory.Delete(System.IO.Directory.GetCurrentDirectory & "\tmp")

        refresh_sdb()
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        refresh_sdb()
    End Sub

    Function MD5Str(ByVal str As String) As String
        Dim md5service As New Security.Cryptography.MD5CryptoServiceProvider
        Dim sb As New StringBuilder
        For Each bytex In md5service.ComputeHash(Encoding.ASCII.GetBytes(str))
            sb.Append(bytex.ToString("x2"))
        Next
        Return sb.ToString
    End Function

    Private Sub refresh_sdb()
        TreeView1.Nodes.Clear()
        ListBox1.Items.Clear()
        Label1.Visible = True
        Label2.Visible = True
        Button10.Enabled = False
        Button9.Enabled = False
        Button11.Enabled = False
        ProgressBar1.Visible = True
        Label1.Text = "폴더 나열중..."
        search_time = True
        Application.DoEvents()
        FormSizeEffectingAuto(New Point(624, 561))
        list_treeview()

        Label2.Visible = False
        If Me.Size.Width = 624 Then
            FormSizeEffectingAuto(New Point(624, 534))
        End If
        Label1.Visible = False
        ProgressBar1.Visible = False
        search_time = False
        Button10.Enabled = True
        Button9.Enabled = True
        Button11.Enabled = True

        If Not FirstLoad Then
            SaveBookmark()
        Else
            FirstLoad = True
        End If
        book_mark.Clear()
        For Each bmkt As _Bookmark_Data In book_mark_all
            If bmkt.hash = MD5Str(FolderGrid(0)) Then
                For Each addr As String In bmkt.addrs
                    book_mark.Add(addr)
                Next
                Exit For
            End If
        Next

    End Sub

    Private Sub auto_rebuild_sdb()
        If Me.Size.Width <> 624 Then
            FormSizeEffectingAuto(New Point(624, 561))
        Else
            FormSizeEffectingAuto(New Point(965, 561))
        End If

        My.Computer.FileSystem.CreateDirectory(System.IO.Directory.GetCurrentDirectory & "\db")
        ProgressBar1.Visible = True
        Label1.Visible = True
        Label2.Visible = True
        ProgressBar1.Style = ProgressBarStyle.Marquee
        search_time = True
        Timer3.Interval = 50
        Timer3.Start()

        df_approach(FolderGrid(0))

        Timer3.Stop()
        df_Directories.Clear()
        ProgressBar1.Style = ProgressBarStyle.Blocks
        ProgressBar1.Visible = False
        Label1.Visible = False
        Label2.Visible = False
        search_time = False

        '//////////////

        Dim fileExists As Boolean
        fileExists = My.Computer.FileSystem.FileExists(last_sdb_addr)
        If fileExists = True Then
            My.Computer.FileSystem.DeleteFile(last_sdb_addr, _
                            FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        End If
        My.Computer.FileSystem.CreateDirectory(System.IO.Directory.GetCurrentDirectory & "\tmp")
        File.WriteAllText(System.IO.Directory.GetCurrentDirectory & "\tmp\file.tmp", df_stringb.ToString)
        df_stringb.Clear()
        IO.Compression.ZipFile.CreateFromDirectory(System.IO.Directory.GetCurrentDirectory & "\tmp", _
                                                   last_sdb_addr)
        My.Computer.FileSystem.DeleteFile(System.IO.Directory.GetCurrentDirectory & "\tmp\file.tmp", _
                                          FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        Directory.Delete(System.IO.Directory.GetCurrentDirectory & "\tmp")
    End Sub

    Dim FilterString As String
    Dim FilterStrings As String()
    Dim ArrayProcess As Boolean = False

    Private Function __if_inside_filter(ByVal strChunk As String) As Boolean
        If ArrayProcess Then
            For Each string_ As String In FilterStrings
                If strChunk.IndexOf(string_, 0, StringComparison.CurrentCultureIgnoreCase) > -1 Then
                    Return True
                End If
            Next
        Else
            If strChunk.IndexOf(FilterString, 0, StringComparison.CurrentCultureIgnoreCase) > -1 Then
                Return True
            End If
        End If
        Return False
    End Function

    Private Sub list_treeview_with_filter()
        Timer2.Interval = 30
        Timer2.Start()
        count = 0
        miss_count = 0
        perpect_then_exit = False

        Dim j As Integer = 0

        top_addr = Path.GetDirectoryName(FolderGrid(0)) & "\"
        For i As Integer = 0 To FolderGrid.Count - 1
            count = i
            Application.DoEvents()
            TreeView1.Nodes.Add(Path.GetFileName(FolderGrid(i)))

            If CType(FolderGrid(i + 1), String).Contains(FolderGrid(i) & "\") Then
                globalcount = i
                integernal_list_treeview_with_filter(FolderGrid(i), TreeView1.Nodes(j))
            End If
            i = globalcount
            j += 1
        Next

        Timer2.Stop()
    End Sub
    Private Sub integernal_list_treeview_with_filter(ByVal obstr As String, ByVal panode As TreeNode)
        Application.DoEvents()

        For i As Integer = globalcount + 1 To FolderGrid.Count - 1
            count = i
            If CType(FolderGrid(i), String).Contains(obstr & "\") Then
                Dim folderExists As Boolean
                folderExists = My.Computer.FileSystem.DirectoryExists(FolderGrid(i))
                If Not folderExists Then
                    miss_count += 1
                    Continue For
                End If

                Label2.Text = CompactString(Path.GetFileName(FolderGrid(i)), Me.Location.X - Label2.Location.X, Label2.Font, TextFormatFlags.PathEllipsis)
                Dim chnode As New TreeNode(Path.GetFileName(FolderGrid(i)))

                If __if_inside_filter(Path.GetFileName(FolderGrid(i))) Or _
                    CType(FolderGrid(i + 1), String).Contains(FolderGrid(i) & "\") Then
                    panode.Nodes.Add(chnode)
                End If

                If CType(FolderGrid(i + 1), String).Contains(FolderGrid(i) & "\") Then
                    globalcount = i

                    If globalcount = FolderGrid.Count - 1 Then
                        perpect_then_exit = True
                        Exit Sub
                    End If
                    integernal_list_treeview_with_filter(FolderGrid(i), chnode)
                    If perpect_then_exit Then
                        Exit Sub
                    End If
                    i = globalcount - 1
                End If
            Else
                globalcount = i
                Exit Sub
            End If
        Next
    End Sub

    Dim continutious As Boolean
    Private Sub TextBox1_KeyUp(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyUp
        If e.KeyCode = Keys.Enter Then
            If Button11.Enabled Then
                TreeView1.Nodes.Clear()
                ListBox1.Items.Clear()
                FormSizeEffectingAuto(New Point(624, 561))
                Label1.Visible = True
                Label2.Visible = True
                ProgressBar1.Visible = True
                Label1.Text = "폴더 탐색중..."
                FilterString = TextBox1.Text
                FilterStrings = TextBox1.Text.Split("|"c)
                ArrayProcess = False
                If FilterStrings.Length > 1 Then
                    ArrayProcess = True
                End If
                list_treeview_with_filter()

                Label1.Visible = False
                Label2.Visible = False
                ProgressBar1.Visible = False
                FormSizeEffectingAuto(New Point(624, 534))

                TreeView1.ExpandAll()
                Application.DoEvents()
                Do
                    continutious = False
                    __delete_none()
                    Application.DoEvents()
                    If Not continutious Then
                        Exit Do
                    End If
                Loop
            End If
        End If
    End Sub
    Private Sub TextBox2_KeyUp(sender As Object, e As KeyEventArgs) Handles TextBox2.KeyUp
        If e.KeyCode = Keys.Enter Then
            If Button11.Enabled Then
                FilterString = TextBox2.Text
                FilterStrings = TextBox2.Text.Split("|"c)
                ArrayProcess = False
                If FilterStrings.Length > 1 Then
                    ArrayProcess = True
                End If
                Do
                    continutious = False
                    __delete_none()
                    If Not continutious Then
                        Exit Do
                    End If
                Loop
                TreeView1.ExpandAll()
            End If
        End If
    End Sub
    Private Sub __inside_delete_none(ByVal panode As TreeNode)
        If panode Is Nothing Then
            Exit Sub
        End If
        If Not __if_inside_filter(panode.FullPath) Then
            If panode.Nodes.Count = 0 Then
                continutious = True
                TreeView1.Nodes.Remove(panode)
            End If
        End If
        For Each pa As TreeNode In panode.Nodes
            __inside_delete_none(pa)
        Next
    End Sub
    Private Sub __delete_none()
        For Each pa As TreeNode In TreeView1.Nodes
            __inside_delete_none(pa)
        Next
    End Sub

    Public PicViewerPictureList As List(Of String)
    Public IsFolderLoad As Boolean
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim picViewList As New List(Of String)

        If current_addr = "" Then
            Exit Sub
        End If

        If Not IsNothing(PicViewerPictureList) Then
            PicViewerPictureList.Clear()
        End If

        ' 폴더안에 폴더만 전부 가져옴
        Dim dirList As New List(Of String)
        If on_menukey Then
            For Each dirs As String In book_mark
                For Each recu As String In Directory.GetDirectories(dirs)
                    dirList.Add(recu)
                Next
            Next
        Else
            If Directory.GetDirectories(current_addr).Length Then
                For Each recu As String In Directory.GetDirectories(current_addr)
                    dirList.Add(recu)
                Next
            End If
        End If

        If dirList.Count = 0 Then
            MsgBox("폴더가 없어 리스트를 만들 수 없습니다.", MsgBoxStyle.Critical, "RollRat Folder")
            Exit Sub
        End If

        ' 폴더를 모두 뒤져 사진이 있으면 그것을 가져옴
        For Each dir As String In dirList
            Dim files As ReadOnlyCollection(Of String)
            files = My.Computer.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchTopLevelOnly, "*.*")

            For Each addr As String In files
                If pic_extension.Contains(IO.Path.GetExtension(addr).ToLower) Then
                    picViewList.Add(addr)
                    Exit For
                End If
            Next
        Next

        IsFolderLoad = True
        PicViewerPictureList = picViewList

        If frmPicviewer.Visible Then
            frmPicviewer.Close()
        End If
        frmPicviewer.Show()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click

        If current_addr = "" Then
            Exit Sub
        End If

        If Not IsNothing(PicViewerPictureList) Then
            PicViewerPictureList.Clear()
        End If

        Dim picViewList As New List(Of String)

        If on_menukey Then
            For Each dirs As String In book_mark
                Dim files As ReadOnlyCollection(Of String)
                files = My.Computer.FileSystem.GetFiles(dirs, FileIO.SearchOption.SearchTopLevelOnly, "*.*")
                For Each addr As String In files
                    If pic_extension.Contains(IO.Path.GetExtension(addr).ToLower) Then
                        picViewList.Add(addr)
                        Exit For
                    End If
                Next
            Next
        Else
            Dim files As ReadOnlyCollection(Of String)
            files = My.Computer.FileSystem.GetFiles(current_addr, FileIO.SearchOption.SearchTopLevelOnly, "*.*")

            For Each addr As String In files
                If pic_extension.Contains(IO.Path.GetExtension(addr).ToLower) Then
                    picViewList.Add(addr)
                End If
            Next
            If files.Count = 0 Then
                Exit Sub
            End If
        End If

        If picViewList.Count = 0 Then
            Exit Sub
        End If

        IsFolderLoad = False
        PicViewerPictureList = picViewList

        If frmPicviewer.Visible Then
            frmPicviewer.Close()
        End If
        frmPicviewer.Show()
    End Sub

    '
    '   사용하기 쉽게 함수로 만듦
    '
    Private Sub FormSizeEffectingAuto(ByVal size As Size, Optional ByVal ExpandWithEvents As Boolean = True)
        Dim originw As Integer = Me.Width
        Dim originh As Integer = Me.Height
        If Me.Size = size Then
            Exit Sub
        End If
        Dim exsizew As Integer = Math.Abs(originw - size.Width)
        Dim exsizeh As Integer = Math.Abs(originh - size.Height)
        If originw < size.Width Then
            For i As Integer = 0 To 25
                Me.Width = originw + exsizew * tanh_value(i)
                If ExpandWithEvents Then
                    Application.DoEvents()
                End If
                Threading.Thread.Sleep(10)
            Next
        Else
            For i As Integer = 0 To 25
                Me.Width = originw - exsizew * tanh_value(i)
                If ExpandWithEvents Then
                    Application.DoEvents()
                End If
                Threading.Thread.Sleep(10)
            Next
        End If
        If originh < size.Height Then
            For i As Integer = 0 To 25
                Me.Height = originh + exsizeh * tanh_value(i)
                If ExpandWithEvents Then
                    Application.DoEvents()
                End If
                Threading.Thread.Sleep(10)
            Next
        Else
            For i As Integer = 0 To 25
                Me.Height = originh - exsizeh * tanh_value(i)
                If ExpandWithEvents Then
                    Application.DoEvents()
                End If
                Threading.Thread.Sleep(10)
            Next
        End If
        Me.Size = size
    End Sub

    'Private Structure MoveControlStructure
    '    Dim sender As Object
    '    Dim whereby As Integer
    'End Structure

    ''
    ''   컨트롤 나타내기
    ''
    'Private Sub AppearMoveControlX(sender As List(Of Object), ByVal whereby As Integer())
    '    Dim destx As New List(Of Integer)
    '    Dim senderx As New List(Of Integer)
    '    Dim sendery As New List(Of Integer)
    '    For i As Integer = 0 To sender.Count - 1
    '        destx.Add(sender(i).Location.X)
    '        senderx.Add(sender(i).Location.X + whereby(i))
    '        sendery.Add(sender(i).Location.Y)
    '    Next
    '    For i As Integer = 0 To 25
    '        For j As Integer = 0 To sender.Count - 1
    '            sender(j).visible = True
    '            sender(j).Location = New Point(destx(j) + senderx(j) * (1 - tanh_value(i)), sendery(j))
    '        Next
    '        Application.DoEvents()
    '        Threading.Thread.Sleep(20)
    '    Next
    '    For i As Integer = 0 To sender.Count - 1
    '        sender(i).Location = New Point(destx(i), sendery(i))
    '    Next
    'End Sub

    ''
    ''   컨트롤 사라지게 하기
    ''
    'Private Sub DisappearMoveControlX(sender As List(Of Object), ByVal whereby As Integer())
    '    Dim destx As New List(Of Integer)
    '    Dim senderx As New List(Of Integer)
    '    Dim sendery As New List(Of Integer)
    '    For i As Integer = 0 To sender.Count - 1
    '        destx.Add(sender(i).Location.X)
    '        senderx.Add(sender(i).Location.X + whereby(i))
    '        sendery.Add(sender(i).Location.Y)
    '    Next
    '    For i As Integer = 25 To 0 Step -1
    '        For j As Integer = 0 To sender.Count - 1
    '            sender(j).Location = New Point(destx(j) + senderx(j) * (1 - tanh_value(i)), sendery(j))
    '        Next
    '        Application.DoEvents()
    '        Threading.Thread.Sleep(20)
    '    Next
    'End Sub

End Class