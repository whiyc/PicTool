using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 图片处理工具1._0
{
    public partial class Form1 : Form
    {
        
        //图片目录总集
        private List<String> picLists = new List<String>() ;
        //图片宽,高,大小
        private Dictionary<int, List<int>> param = new Dictionary<int, List<int>>();
        //图片,高,大小
        //private List<int> heightAndSize = new List<int>();
        //图片后缀名
        private List<String> picExten = new List<string>();
        //图片固定集合
        private List<String> picNames = new List<string>();
        //检查图片是否缺图
        private List<String> regularPicNames = new List<string>();

        //限制高度
        private String picLimteHeight;

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            
            InitInfo();
            String dir = textBox1.Text;
            DirectoryInfo folders = new DirectoryInfo(dir);
            //遍历文件夹
            ListFiles(folders);

        }

        private void InitInfo()
        {

            InitPicName();

            InitPicInfo();

            InitPicExten();

        }

        private void InitPicExten()
        {
            picExten.Add(".png");
            picExten.Add(".jpg");
            picExten.Add(".jpeg");
            picExten.Add(".bmp");
            picExten.Add(".gif");
            picExten.Add(".JPG");
            picExten.Add(".PNG");
            picExten.Add(".JPEG");
            picExten.Add(".BMP");
            picExten.Add(".GIF");
            

        }

        private void InitPicName()
        {

            regularPicNames.Add("1");
            regularPicNames.Add("2");
            regularPicNames.Add("3");
            regularPicNames.Add("4");
            regularPicNames.Add("5");
            regularPicNames.Add("7");
            regularPicNames.Add("15");
            regularPicNames.Add("16");
            regularPicNames.Add("601");

            picNames.Add("1");
            picNames.Add("2");
            picNames.Add("3");
            picNames.Add("4");
            picNames.Add("5");
            picNames.Add("7");
            picNames.Add("15");
            picNames.Add("16");
            

            for (int y = 601; y <= 650; y++)
            {
                picNames.Add(y.ToString());
            }
        }

        private void InitPicInfo()
        {

            String picWidth = textBox2.Text;
            String picHeight = textBox3.Text;
            String picSize = textBox4.Text;
            picLimteHeight = textBox5.Text;

            String[] widthStr = picWidth.Split(',');
            String[] heightStr = picWidth.Split(',');
            String[] sizeStr = picWidth.Split(',');

            for (int x = 0; x < heightStr.Length; x++) {
                List<int> heightAndSize = new List<int>();
                heightAndSize.Add(int.Parse(heightStr[x]));
                heightAndSize.Add(int.Parse(sizeStr[x]));
                param.Add(int.Parse(widthStr[x]),heightAndSize);
            }
            

            //Console.WriteLine(widthStr[0]+ widthStr[1]+ widthStr[2]);

        }

        private void ListFiles(FileSystemInfo info)
        {

            if (!info.Exists) return;

            DirectoryInfo dir = info as DirectoryInfo;

            if (dir == null) return;

            FileSystemInfo[] files = dir.GetFileSystemInfos();

            List<String> regularPicName = new List<string>();
            for (int z = 0;z<regularPicNames.Count;z++) {

                regularPicName.Add(Path.GetDirectoryName(files.ElementAt(0).FullName)+"\\"+regularPicNames.ElementAt(z));
            }
           
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;
                
                if (file != null) {
         
                    if (picExten.Contains(file.Extension))
                    {
                        picLists.Add(file.FullName);
                    }
                    else {
                        listBox1.Items.Add(file.FullName);
                    }
                    
                    
                }
                else ListFiles(files[i]);
            }

            if (regularPicNames.Count > 0) {

                foreach (String picMame in regularPicNames) {

                    listBox1.Items.Add(files.ElementAt(0).FullName);
                }
            }

        }

        private void 开始_Click(object sender, EventArgs e)
        {
            checkPic();
        }

        private void checkPic()
        {
            
        }
    }
}
