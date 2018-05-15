using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PicTool
{
    public partial class Form1 : Form
    {

        /// <summary>
        /// 配置信息
        private List<String> picNamesList = new List<string>();//图片名称
        private List<String> singlePicNamesList = new List<string>();//图片名称
        private List<String> regularPicNamesList = new List<string>();//遗漏检测图片名称
        private List<String> picNamesFixList = new List<string>();//有效图片名称
        private Dictionary<String, InfoBean> picInfos = new Dictionary<String, InfoBean>();//图片对应信息
        private Dictionary<String, InfoBean> singlePicInfo = new Dictionary<String, InfoBean>();//图片对应信息
        /// </summary>
        private List<String> parentPath = new List<string>();//父文件夹
        
        private bool isCheck;//是否勾选

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (picNamesList.Count > 0) return;
            
            String infoDir = "info.txt";
            StreamReader reader = new StreamReader(infoDir);

            String line = "";

            while ((line = reader.ReadLine()) != null) {

                //1,2,3,4,15,16---1200*1200---1024---jpg
                //601-650---750#1600---1024---jpg
                Console.WriteLine("line:" + line);

                String[] infos = Regex.Split(line, "---");

                InfoBean infoBean = new InfoBean();

                String picName = "";
                for (int i = 0; i < infos.Length; i++)
                {

                    String info = infos[i];

                    switch (i)
                    {

                        case 0://图片名称

                            picName = info;

                            break;

                        case 1://图片像素(宽高)

                            if (info.Contains("#"))//有最大限制高度
                            {

                                String[] pix = info.Split('#');
                                infoBean.SetWidth(int.Parse(pix[0]));
                                infoBean.SetMaxHeight(int.Parse(pix[1]));
                                infoBean.setIsHaveMaxHeight(true);

                            }
                            else
                            {

                                String[] pix = info.Split('*');//没有限制高度
                                infoBean.SetWidth(int.Parse(pix[0]));
                                infoBean.SetHeight(int.Parse(pix[1]));
                            }

                            break;

                        case 2://图片大小

                            if (info.Contains("-"))
                            {//图片有限制大小

                                String[] sizes = info.Split('-');

                                infoBean.SetMinSize(int.Parse(sizes[0]) * 1024);
                                infoBean.SetMaxSize(int.Parse(sizes[1]) * 1024);
                                infoBean.setIsHaveSize(true);
                            }
                            else
                            {

                                infoBean.SetSize(int.Parse(info) * 1024);
                            }


                            break;

                        case 3://图片格式

                            infoBean.SetPicType(info);

                            break;

                    }

                }


                if (picName.StartsWith("$"))
                {
                    //文件下单独文件的图片信息 例如1.jgp
                    singlePicNamesList.Add(picName.Substring(1));
                    singlePicInfo.Add(picName.Substring(1), infoBean);

                }
                else
                {

                    picNamesList.Add(picName);
                    picInfos.Add(picName, infoBean);

                    if (picName.Contains(","))
                    {

                        String[] names = picName.Split(',');
                        foreach (String name in names)
                        {

                            regularPicNamesList.Add(name);
                            picNamesFixList.Add(name);

                        }
                    }
                    else if (picName.Contains("-"))
                    {
                        String[] names = picName.Split('-');

                        int minName = int.Parse(names[0]);
                        int maxName = int.Parse(names[1]);

                        regularPicNamesList.Add(names[0]);

                        for (int i = 0; i <= (maxName-minName); i++)
                        {
                            picNamesFixList.Add(minName + "");
                            minName += 1;
                        }
                    }

                    else
                    {

                        regularPicNamesList.Add(picName);
                        picNamesFixList.Add(picName);
                    }


                }
                Application.DoEvents();
            }
            reader.Close();
        }

        private void 开始_Click(object sender, EventArgs e)
        {
            clearDatas();

            isCheck = checkBox1.Checked;//是否勾选，默认为true
            //开始检测图片
            String dir = textBox1.Text;
            DirectoryInfo folders = new DirectoryInfo(dir);
            //遍历文件夹
            ListFiles(folders);

            //listBox1.Items.
            listBox1.Items.Add("All files checked! ୧(๑•̀⌄•́๑)૭✧");

        }

        private void clearDatas()
        {
            parentPath.Clear();

            if (listBox1.Items.Count > 0) listBox1.Items.Clear();

        }

        private void ListFiles(FileSystemInfo info)
        {

            if (!info.Exists) return;

            DirectoryInfo dir = info as DirectoryInfo;

            if (dir == null) return;

            FileSystemInfo[] files = dir.GetFileSystemInfos();

            if (isCheck)
            {//检测是否缺失文件

                DirectoryInfo lastPath = checkLastPath(files);
                
                if (lastPath != null && !parentPath.Contains(lastPath.FullName)) {

                    parentPath.Add(lastPath.FullName);

                    String picPath = checkMissingPic(lastPath);//检测是否缺失图片
                    
                    if (picPath != "")
                        {
                          
                            String name = Path.GetFileNameWithoutExtension(picPath);
                               
                            if (singlePicNamesList.Contains(name))
                            {

                                InfoBean infoBean = singlePicInfo[name];
                                checkSiglePicInfo(picPath, infoBean);
                            
                           }
                    }
                        
                }
                    
            }

            List<String> picNames = new List<string>();

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;

                if (file != null && checkIsLastPath(files))
                {

                    String name = Path.GetFileNameWithoutExtension(file.FullName);

                    Console.WriteLine("name:" + name);

                    if (picNamesFixList.Contains(name))
                    {

                        picNames.Add(file.FullName);
                    }
                    else
                    {
                        if (!Path.GetExtension(file.FullName).Equals(".db"))
                        {

                            listBox1.Items.Add("多余文件:" + file.FullName);
                        }

                    }
                    
                }
                else ListFiles(files[i]);
            }

            checkPicCount(picNames);//检测图片数量

            checkPicInfo(picNames);//检测图片规格

            Application.DoEvents();
        }

        private void checkPicInfo(List<string> picNames)
        {
            foreach (String picName in picNames) {

                foreach (String mPicName in picNamesList) {

                    String name = Path.GetFileNameWithoutExtension(picName);

                    if (mPicName.Contains(name)) {
                        
                        InfoBean infoBean = picInfos[mPicName];
                        
                        String extension = Path.GetExtension(picName).Substring(1);
                        FileInfo info = new FileInfo(picName);
                        long picSize = info.Length;
                        //Console.WriteLine("picSize:" + picSize);
                        System.Drawing.Image image = System.Drawing.Image.FromFile(picName);

                        int picWidth = image.Width;
                        int picHeight = image.Height;

                        if (!String.Equals(extension,infoBean.GetPicType(),StringComparison.CurrentCultureIgnoreCase)) {

                            listBox1.Items.Add("图片格式不正确:" + picName + "---width:" + picWidth + "---height:" + picHeight + "---size:" + picSize / 1024 + "kb");
                            break;
                        }

                        if (picWidth == infoBean.GetWidth())
                        {

                            if (infoBean.getIsHaveMaxHeight())//是否有限制高度
                            {
                                if (picHeight > infoBean.GetMaxHeight()) {

                                    listBox1.Items.Add("图片像素不正确:" + picName + "---width:" + picWidth + "---height:" + picHeight + "---size:" + picSize / 1024 + "kb");
                                    break;
                                }

                            }
                            else
                            {
                                if (picHeight != infoBean.GetHeight()) {

                                    listBox1.Items.Add("图片像素不正确:" + picName + "---width:" + picWidth + "---height:" + picHeight + "---size:" + picSize / 1024 + "kb");
                                    break;

                                }

                            }

                        }
                        else if(picWidth != infoBean.GetWidth() && infoBean.GetWidth() != 0){

                            listBox1.Items.Add("图片像素不正确:" + picName + "---width:" + picWidth + "---height:" + picHeight + "---size:" + picSize / 1024 + "kb");
                            break;
                        }

                        
                        if (infoBean.getIsHaveMaxSize())//是否有限制大小
                        {
                            if (  picSize >  infoBean.GetMinSize() && picSize  < infoBean.GetMaxSize()) {

                                listBox1.Items.Add("图片大小不正确:" + picName + "---width:" + picWidth + "---height:" + picHeight + "---size:" + picSize / 1024 + "kb");
                                break;
                            }

                        }
                        else {

                            if (picSize > infoBean.GetSize()) {

                                listBox1.Items.Add("图片大小不正确:" + picName + "---width:" + picWidth + "---height:" + picHeight + "---size:" + picSize / 1024 + "kb");
                                break;

                            }

                        }

                        image.Dispose();

                        break;
                    }
                    
                }
                

            }


        }

        private void checkSiglePicInfo(String picPath,InfoBean infoBean) {

            String extension = Path.GetExtension(picPath).Substring(1);
            FileInfo info = new FileInfo(picPath);
            long picSize = info.Length;
            //Console.WriteLine("picSize:" + picSize);
            System.Drawing.Image image = System.Drawing.Image.FromFile(picPath);

            int picWidth = image.Width;
            int picHeight = image.Height;

            if (picWidth != infoBean.GetWidth() || picHeight != infoBean.GetHeight() || picSize > infoBean.GetSize() || !String.Equals(extension, infoBean.GetPicType(), StringComparison.CurrentCultureIgnoreCase)) {

                listBox1.Items.Add("图片问题:" + picPath + "---width:" + picWidth + "---height:" + picHeight + "---size:" + picSize / 1024 + "kb");
            }
            
            image.Dispose();
        }

        private void checkPicCount(List<string> picNames)
        {
            if (picNames.Count <= 0) return;

            String path = Path.GetDirectoryName(picNames.ElementAt(0));
            List<String> temp = new List<String>(regularPicNamesList);

            for (int i = 0; i < picNames.Count; i++)
            {

                if (temp.Count == 0) break;

                String pathWithoutExten = Path.GetFileNameWithoutExtension(picNames.ElementAt(i));
                //如果
                if (regularPicNamesList.Contains(pathWithoutExten))
                {
                    temp.RemoveAt(temp.IndexOf(pathWithoutExten));
                }
                
            }

            if (temp.Count > 0)
            {

                for (int x = 0; x < temp.Count; x++)
                {
                    listBox1.Items.Add("缺图片:" + path + "\\" + temp.ElementAt(x));
                }

            }

        }

        private String checkMissingPic(DirectoryInfo lastPath) {

            String picPath = "";
            int len = 0;

            //DirectoryInfo directoryInfo = file.Directory;
            FileSystemInfo[] pathWithPic = lastPath.GetFileSystemInfos();
            foreach (FileSystemInfo item in pathWithPic)
            {
                if (item is FileInfo)//判断是否是文件
                {
                    //文件
                    FileInfo fileInfo = item as FileInfo;
                    String name = Path.GetFileNameWithoutExtension(fileInfo.FullName);

                    Console.WriteLine("name:" + name);
                    //bool isContain = singlePicNamesList.Contains("1");

                    if (singlePicNamesList.Contains(name))
                    {
                        picPath = fileInfo.FullName;
                        len += 1;
                    }
                    else
                    {
                        if(Path.GetExtension(fileInfo.FullName) != ".db")
                            listBox1.Items.Add("多余文件:" + fileInfo.FullName);
                    }


                }
                //else if (item is DirectoryInfo)//判断时候是文件夹
                //{

                //}
            }
            if (len != singlePicNamesList.Count)
            {

                //String path = lastPath.FullName;
                String path = lastPath.FullName + "\\";

                //Console.WriteLine("path:" + path);
                //foreach (String singlePinName in singlePicNamesList) 
                listBox1.Items.Add("该文件夹下缺少必要的图片:" + path);

            }
            return picPath;
        }

        private bool checkIsLastPath(FileSystemInfo[] files) {

            bool isLastPath = false;
            int len = 0;
            foreach (FileSystemInfo item in files)
            {
                if (item is FileInfo)//判断是否是文件
                {
                    //文件
                    len += 1;
                }
                else if (item is DirectoryInfo)//判断时候是文件夹
                {
                    len -= 1;
                }
            }

            if (len == files.Length)
            {

                isLastPath = true;
            }

            return isLastPath;
        }

        private DirectoryInfo checkLastPath(FileSystemInfo[] files)
        {
            DirectoryInfo parentPath = null;

            if (checkIsLastPath(files)) {

                FileInfo file = files[0] as FileInfo;

                parentPath = Directory.GetParent(file.DirectoryName);
            }

            return parentPath;


        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox list = (ListBox)sender;
            //Console.WriteLine(list.Items[e.Index].ToString());
            String text = list.Items[e.Index].ToString();
            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Define the default color of the brush as black.
            Brush myBrush = Brushes.Black;
            
            // Draw the current item text based on the current Font 
            // and the custom brush settings.
            e.Graphics.DrawString(text,
                e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }

     
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBox1.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {

                String path = this.listBox1.SelectedItem.ToString();
                int start = path.IndexOf(':') + 1;
                int end = path.LastIndexOf('\\');
                String filePath = path.Substring(start, end - start);
                System.Diagnostics.Process.Start(filePath);
            }
        }
    }

    class InfoBean {

        private int width;
        private int height;
        private int maxHeight;//限制高度
        //private int minHeight;
        private long size;
        private long minSize;//
        private long maxSize;
        private String picType;
        private bool isHaveMaxHeight;//是否有限制高度
        private bool isHaveMaxSize;//是否有限制大小

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight() {

            return height;
        }
        public int GetMaxHeight() {

            return maxHeight;
        }
        //public int GetMinHeight() {

        //    return minHeight;
        //}

        public long GetSize()
        {
            return size;
        }
        public long GetMaxSize()
        {
            return maxHeight;
        }
        public long GetMinSize()
        {
            return minSize;
        }

        public String GetPicType() {

            return picType;
        }

        public bool getIsHaveMaxHeight() {

            return isHaveMaxHeight;
        }

        public bool getIsHaveMaxSize() {

            return isHaveMaxSize;
        }

        public void SetWidth(int width)
        {
            this.width = width;
        }

        public void SetHeight(int height) {

            this.height = height;
        }
        public void SetMaxHeight(int maxHeight) {

            this.maxHeight = maxHeight;
        }

        //public void SetMinHeight(int minHeight) {

        //    this.minHeight = minHeight;
        //}

        public void SetSize(long size) {

            this.size = size;
        }
        public void SetMaxSize(long maxSize) {

            this.maxSize = maxSize;
        }
        public void SetMinSize(long minSize) {

            this.minSize = minSize;
        }

        public void SetPicType(String picType) {

            this.picType = picType;
        }

        public void setIsHaveMaxHeight(bool isHaveMaxHeight) {

            this.isHaveMaxHeight = isHaveMaxHeight;
        }

        public void setIsHaveSize(bool isHaveMaxSize) {

            this.isHaveMaxSize = isHaveMaxSize;
        }
    }
}
