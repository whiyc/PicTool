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
        //private List<String> picLists = new List<String>() ;
        //图片宽,高,大小
        private Dictionary<int, List<int>> param = new Dictionary<int, List<int>>();
        //图片,高,大小
        //private List<int> heightAndSize = new List<int>();
        //图片后缀名
        private List<String> picExten = new List<string>();
        //图片固定集合
        private List<String> picNamesFix = new List<string>();
        //检查图片是否缺图
        private List<String> regularPicNames = new List<string>();

        //限制高度
        private long picLimteHeight;

        public Form1()
        {
            InitializeComponent();
            InitInfo();
        }

        private void Button1_Click(object sender, EventArgs e)
        {

            if (listBox1.Items.Count > 0) listBox1.Items.Clear();

            String dir = textBox1.Text;
            DirectoryInfo folders = new DirectoryInfo(dir);
            //遍历文件夹
            ListFiles(folders);

            //listBox1.Items.
            listBox1.Items.Add("All the file already checked !!!");
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

            picNamesFix.Add("1");
            picNamesFix.Add("2");
            picNamesFix.Add("3");
            picNamesFix.Add("4");
            picNamesFix.Add("5");
            picNamesFix.Add("7");
            picNamesFix.Add("15");
            picNamesFix.Add("16");


            for (int y = 601; y <= 650; y++)
            {
                picNamesFix.Add(y.ToString());
            }
        }

        private void InitPicInfo()
        {

            String picWidth = textBox2.Text;
            String picHeight = textBox3.Text;
            String picSize = textBox4.Text;
            picLimteHeight = long.Parse(textBox5.Text) * 1024;

            String[] widthStr = picWidth.Split(',');
            String[] heightStr = picHeight.Split(',');
            String[] sizeStr = picSize.Split(',');

            for (int x = 0; x < widthStr.Length; x++)
            {
                List<int> heightAndSize = new List<int>();
                heightAndSize.Add(int.Parse(heightStr[x]));
                heightAndSize.Add(int.Parse(sizeStr[x]) * 1024);
                param.Add(int.Parse(widthStr[x]), heightAndSize);
            }


            //Console.WriteLine(widthStr[0]+ widthStr[1]+ widthStr[2]);

        }

        private void ListFiles(FileSystemInfo info)
        {

            if (!info.Exists) return;

            DirectoryInfo dir = info as DirectoryInfo;

            if (dir == null) return;

            FileSystemInfo[] files = dir.GetFileSystemInfos();

            List<String> picNames = new List<string>();

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;

                if (file != null)
                {

                    String name = Path.GetFileNameWithoutExtension(file.FullName);

                    if (picExten.Contains(file.Extension) && picNamesFix.Contains(name))
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

            checkPic(picNames);

        }

        private void checkPic(List<String> picNames)
        {

            if (picNames.Count <= 0) return;

            String path = Path.GetDirectoryName(picNames.ElementAt(0));
            List<String> temp = new List<String>(regularPicNames);

            for (int i = 0; i < picNames.Count; i++)
            {

                if (temp.Count == 0) break;

                String pathWithoutExten = Path.GetFileNameWithoutExtension(picNames.ElementAt(i));
                //如果
                if (regularPicNames.Contains(pathWithoutExten))
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

            checkPicInfo(picNames);

            Application.DoEvents();

        }

        private void checkPicInfo(List<string> temp)
        {
            for (int x = 0; x < temp.Count; x++)
            {

                String name = Path.GetFileNameWithoutExtension(temp.ElementAt(x));
                FileInfo info = new FileInfo(temp.ElementAt(x));
                long picSize = info.Length;
                //Console.WriteLine("picSize:" + picSize);
                System.Drawing.Image image = System.Drawing.Image.FromFile(temp.ElementAt(x));

                int picWidth = image.Width;
                int picHeight = image.Height;


                if (name.Equals("1") || name.Equals("2"))
                {
                    if (picWidth != 1100 || picHeight != 1390 || picSize > 300 * 1024)
                    {

                        listBox1.Items.Add("问题图片:" + temp.ElementAt(x) + "---width:" + picWidth + "---height:" + picHeight + "---size:" + picSize / 1024 + "kb");
                    }

                }
                else if (name.Equals("5") || name.Equals("7"))
                {

                    if (picWidth != 420 || picHeight != 531 || picSize > 80 * 1024)
                    {

                        listBox1.Items.Add("问题图片:" + temp.ElementAt(x) + "---width:" + picWidth + "---height:" + picHeight + "---size:" + picSize / 1024 + "kb");
                    }
                }
                else
                {

                    if (param.ContainsKey(picWidth))
                    {

                        List<int> heightAndSize = param[picWidth];

                        //Console.WriteLine("picHeight:" + heightAndSize[0] + "---picSize:" + heightAndSize[1]);

                        if (heightAndSize[0] == 0)
                        {
                            if (picSize > picLimteHeight)
                            {

                                listBox1.Items.Add("问题图片:" + temp.ElementAt(x) + "---width:" + picWidth + "---height:" + picHeight + "---size:" + picSize / 1024 + "kb");
                            }
                        }
                        else
                        {

                            if (picHeight != heightAndSize[0] || picSize > heightAndSize[1])
                            {

                                listBox1.Items.Add("问题图片:" + temp.ElementAt(x) + "---width:" + picWidth + "---height:" + picHeight + "---size:" + picSize / 1024 + "kb");
                            }
                        }


                    }
                }

                image.Dispose();
            }


            //Console.WriteLine("picWidth:"+picWidth+"---picHeight:"+picHeight);


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
                //MessageBox.Show("end"+end);
                //MessageBox.Show(filePath);
                System.Diagnostics.Process.Start(filePath);
            }
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

            // Determine the color of the brush to draw each item based 
            // on the index of the item to draw.
            //int index = e.Index % 2;

            //if (text.StartsWith("缺"))
            //{
            //    myBrush = Brushes.Red;

            //}
           // else if (text.StartsWith("多"))
            //{
            //    myBrush = Brushes.Orange;
           // }
           // else if (text.StartsWith("问")) {

            //    myBrush = Brushes.Purple;
           // }

            

            // Draw the current item text based on the current Font 
            // and the custom brush settings.
            e.Graphics.DrawString(text,
                e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();
        }
    }
}
