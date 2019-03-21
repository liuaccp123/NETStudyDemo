using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Models;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Data.SqlClient;
namespace BLL
{
    public partial class KBFileTableBLL : BLLBase<KBFileTable>
    {
        KBFileTableDAL kbfiledal = new KBFileTableDAL();

        public override void SetDAL()
        {
            this.dal = kbfiledal;
        }

        /////////////////////
        KBNRTableDAL kbnrDAL = new KBNRTableDAL();//课表内容对象

        #region 增加，上传数据，并读取课表中的信息
        /// <summary>
        /// 增加并处理上传的格式数据
        /// </summary>
        /// <param name="tabObj"></param>
        /// <param name="gzbwz">工作表位置</param>
        /// <param name="jshs">教室行数</param>
        /// <returns></returns>
        public List<APaonKeBiao> apaokebiaoList = new List<APaonKeBiao>();//安排课的集合，用来存放所有的安排课数据
        public OptionResult AddKBFile(KBFileTable tabObj, string fileBasePath, int gzbwz, int jshs, int pkts)
        {

            try
            {
               
                string GuidId = Guid.NewGuid().ToString(); //生成唯一编码字符串数据
                tabObj.Guid_kbfile = GuidId;//编号
                kbfiledal.Add(tabObj);//增加上传内容数据
                ///////////////////////////////
                apaokebiaoList.Clear();//清空集合数据

                ReadClassRoomData(fileBasePath, gzbwz, jshs, tabObj.StartTime, tabObj.EndTime,pkts);//先读教室数据
                ReadMachineData(fileBasePath, gzbwz, jshs, tabObj.StartTime, tabObj.EndTime,pkts);//读机房数据
                ////////////////////////////////////////////////
                ///////////先转存本周上课的数据到正在上课的周课表//////////////////////
                string str = apaokebiaoList[0].日期;//取出一个日期
                var count = kbnrDAL.GetAllData2().Where(p => p.KB_RQ == str || p.KB_RQ.Contains(str) ).Count();//如果日期已存在的检查
                if (count == 0) //说明正在排下周的课表内容,转存本周的课表安排，到本周的课表安排
                {
                    string sql = @"Insert into KBFileTabel_zy(Guid_Kb, KB_RQ, 
                                KB_XQ, KB_ZYM, KB_NR, KB_JYM, KB_SKBJ, KB_SJD,
                                 KB_SJFW, KB_ZYLX, Guid_KBID) 
                                 select  Guid_Kb, KB_RQ, KB_XQ, KB_ZYM, KB_NR, KB_JYM, KB_SKBJ, 
                                 KB_SJD, KB_SJFW, KB_ZYLX, Guid_KBID
                                 from KBNRTable
 
                                 delete from KBNRTable";
                    kbnrDAL.ExcuteCommand(sql);//执行sql语句
                }
                //////////////////////////////////////////////////////////////////////

                ///////////////////////////////////////////////
                ///////把数据加入到数据库中////////////////
                foreach (APaonKeBiao apkbobj in apaokebiaoList)
                {
                    KBNRTable khnrObj = new KBNRTable();//课表内容对象
                    khnrObj.Guid_Kb = Guid.NewGuid().ToString();//课表内容的编号
                    khnrObj.Guid_KBID = GuidId;//外键，对应计划
                    khnrObj.KB_JYM = apkbobj.教员名;//教员名
                    khnrObj.KB_NR = apkbobj.内容;//
                    khnrObj.KB_RQ = apkbobj.日期;//
                    khnrObj.KB_SJD = apkbobj.时间段;//时间段
                    khnrObj.KB_SJFW = apkbobj.时间范围;//
                    khnrObj.KB_SKBJ = apkbobj.授课班级;//班级
                    khnrObj.KB_XQ = apkbobj.星期;//星期
                    khnrObj.KB_ZYLX = apkbobj.资源类型;//资源类型
                    khnrObj.KB_ZYM = apkbobj.资源名;
                    //加入到集合中
                    kbnrDAL.Add(khnrObj);//加入到课程内容表中

                }
                //////////////////////////////////////////
                ///////////////////////////////
                kbfiledal.Save();//同步一次提交


            }
            catch (Exception ex)
            {
                return new OptionResult { Code = 1, Message = ex.Message };//出错返回
            }
            return new OptionResult { Code = 0, Message = "执行成功" }; //成功返回

        }


        #region 读取教室数据
        private void ReadClassRoomData(string filePath, int gzbwz, int jshs, string strartTime, string EndTime, int pkts)
        {
            //string filePath = openFileDialog1.FileName;//获取文件名
            //工作薄：excel文件对象
            HSSFWorkbook hssfworkbook;
            #region//初始化信息
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("打开excel文档出错，请检查文件格式"+ex.Message);
            }
            #endregion
            int index = gzbwz - 1;//周工作表所在索引0,第1个，1为第二个工作表
            int jiaoshinum = jshs;//需要读几个教室的
            DateTime t1 = DateTime.Parse(strartTime);//开始日期
            DateTime t2 = DateTime.Parse(EndTime);//结束日期
            int sp1 = (t2 - t1).Days + 1;//两者之间的天数：需要加1，因为有前有后
            // MessageBox.Show(sp1 + " 时期 ");
            string[] TimeFaleString = new string[] { "8:30-10:20", "10:40-12:30", "13:30-15:20", "15:30-17:20" };//时间标识字符串
            string[] TimeFWString = new string[] { "上午8:30-10:20", "上午10:40-12:30", "下午13:30-15:20", "下午15:30-17:20" };
            //日期字符串数组
            List<string> dataStringList = new List<string>(); //保存这些天的日期标识
            for (int d = 0; d < sp1; d++)
            {
                dataStringList.Add(t1.AddDays(d).ToString("yyyy-MM-dd"));
            }
            string[] WeekStringList = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            //星期字符串数组
            List<string> WeekStringListData = new List<string>(); //保存这些天的星期标识集合
            for (int d = 0; d < sp1; d++)
            {
                DateTime dataTimeTemp = t1.AddDays(d);
                //求星期
                WeekStringListData.Add(WeekStringList[(int)dataTimeTemp.DayOfWeek]);
            }

            //获取工作薄的第一个工作表文件
            using (HSSFSheet sheet = hssfworkbook.GetSheetAt(index)) //索引值获取工作表的第几张
            {
                #region 读取教室安排数据信息
                int startRowIndex = 4;//第5行，以0开始所有少1
                int startColIndex = 2;//第3列，以0开始所有少1
                for (int i = 1; i <= sp1; i++) //循环几天 控件所有的天数循环
                {
                    string strWeekShow = WeekStringListData[i - 1];//星期数
                    string strDataShow = dataStringList[i - 1];
                    for (int j = 1; j <= jiaoshinum; j++) //每天循环所有教室资源循环
                    {
                        //定位行数据
                        HSSFRow dataHSSRow = sheet.GetRow((i - 1) * jiaoshinum + startRowIndex + (j - 1));//第几天*教室数+起始行
                        string ZYName = GetCellValue(dataHSSRow.GetCell(startColIndex));//资源名
                        //定义列数据 //每次读三次，循环三次，每次三列//三列读取为一次课的安排，2节课一次安排
                        for (int k = 1; k <= 4; k++)
                        {
                            string KChengName = GetCellValue(dataHSSRow.GetCell(startColIndex + 1 + (k - 1) * 3));//科目名
                            string teachName = GetCellValue(dataHSSRow.GetCell(startColIndex + 2 + (k - 1) * 3));//教员名
                            string bjname = GetCellValue(dataHSSRow.GetCell(startColIndex + 3 + (k - 1) * 3));//班级名
                            string timename = TimeFaleString[k - 1];
                            string timefwname = TimeFWString[k - 1];
                            //构建一个对象，用来存储数据项。。。。。
                            APaonKeBiao pb = new APaonKeBiao();
                            pb.教员名 = teachName;
                            pb.内容 = KChengName;
                            pb.日期 = strDataShow;
                            pb.时间段 = timename;
                            pb.授课班级 = bjname;
                            pb.时间范围 = timefwname;
                            pb.星期 = strWeekShow;
                            pb.资源名 = ZYName;
                            pb.资源类型 = "教室";
                            //加入到集合中
                            apaokebiaoList.Add(pb);
                        }

                    }
                }



                #endregion

                //读完后显示数据
                //this.dataGridView1.DataSource = apaokebiaoList.ToList();
                #region 读取机房安排信息

                #endregion


            }
        }
        #endregion

        #region 读机房上课数据
        public void ReadMachineData(string filePath, int gzbwz, int jshs, string strartTime, string EndTime, int pkts)
        {
            //string filePath = openFileDialog1.FileName;//获取文件名
            //工作薄：excel文件对象
            HSSFWorkbook hssfworkbook;
            #region//初始化信息
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion
            int index = gzbwz - 1;//周工作表所在索引0,第1个，1为第二个工作表
            int jiaoshinum = jshs;//需要读几个教室的
            DateTime t1 = DateTime.Parse(strartTime);//开始日期
            DateTime t2 = DateTime.Parse(EndTime);//结束日期
            int sp1 = (t2 - t1).Days + 1;//两者之间的天数：需要加1，因为有前有后
            // MessageBox.Show(sp1 + " 时期 ");
            string[] TimeFaleString = new string[] { "8:30-10:20", "10:40-12:30", "13:30-15:20", "15:30-17:20" };//时间标识字符串
            string[] TimeFWString = new string[] { "上午8:30-10:20", "上午10:40-12:30", "下午13:30-15:20", "下午15:30-17:20" };
            //日期字符串数组
            List<string> dataStringList = new List<string>(); //保存这些天的日期标识
            for (int d = 0; d < sp1; d++)
            {
                dataStringList.Add(t1.AddDays(d).ToString("yyyy-MM-dd"));
            }
            string[] WeekStringList = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            //星期字符串数组
            List<string> WeekStringListData = new List<string>(); //保存这些天的星期标识集合
            for (int d = 0; d < sp1; d++)
            {
                DateTime dataTimeTemp = t1.AddDays(d);
                //求星期
                WeekStringListData.Add(WeekStringList[(int)dataTimeTemp.DayOfWeek]);
            }

            //获取工作薄的第一个工作表文件
            using (HSSFSheet sheet = hssfworkbook.GetSheetAt(index)) //索引值获取工作表的第几张
            {
                #region 读取机房安排数据信息
                int startRowIndex = 4;//第5行，以0开始所有少1
                //int startColIndex = 14;//第15列，以0开始所有少1
                for (int i = 1; i <= sp1; i++) //循环几天 控件所有的天数循环
                {
                    string strWeekShow = WeekStringListData[i - 1];//星期数
                    string strDataShow = dataStringList[i - 1];
                    for (int j = 1; j <= jiaoshinum; j++) //每天循环所有教室资源循环
                    {
                        //定位行数据
                        HSSFRow dataHSSRow = sheet.GetRow((i - 1) * jiaoshinum + startRowIndex + (j - 1));//第几天*教室数+起始行

                        string SW_data1 = dataHSSRow.GetCell(15) == null ? "" : GetCellValue(dataHSSRow.GetCell(15));//上午安排的班级
                        string XW_data2 = dataHSSRow.GetCell(16) == null ? "" : GetCellValue(dataHSSRow.GetCell(16));
                        string zyname = dataHSSRow.GetCell(17) == null ? "" : GetCellValue(dataHSSRow.GetCell(17));//资源名在第17列，，下标为
                        // MessageBox.Show(SW_data1 + "  " + XW_data2 + "   " + zyname);

                        APaonKeBiao pb = new APaonKeBiao();
                        pb.教员名 = "自行安排";
                        pb.内容 = "自习";
                        pb.日期 = strDataShow;
                        pb.时间段 = TimeFaleString[0];
                        pb.授课班级 = SW_data1;
                        pb.时间范围 = TimeFWString[0];
                        pb.星期 = strWeekShow;
                        pb.资源名 = zyname;
                        pb.资源类型 = "机房";
                        //加入到集合中
                        apaokebiaoList.Add(pb);

                        APaonKeBiao pb1 = new APaonKeBiao();
                        pb1.教员名 = "自行安排";
                        pb1.内容 = "自习";
                        pb1.日期 = strDataShow;
                        pb1.时间段 = TimeFaleString[1];
                        pb1.授课班级 = SW_data1;
                        pb1.时间范围 = TimeFWString[1];
                        pb1.星期 = strWeekShow;
                        pb1.资源名 = zyname;
                        pb1.资源类型 = "机房";

                        //加入到集合中
                        apaokebiaoList.Add(pb1);

                        APaonKeBiao pb2 = new APaonKeBiao();
                        pb2.教员名 = "自行安排";
                        pb2.内容 = "自习";
                        pb2.日期 = strDataShow;
                        pb2.时间段 = TimeFaleString[2];
                        pb2.授课班级 = XW_data2;
                        pb2.时间范围 = TimeFWString[2];
                        pb2.星期 = strWeekShow;
                        pb2.资源名 = zyname;
                        pb2.资源类型 = "机房";

                        //加入到集合中
                        apaokebiaoList.Add(pb2);


                        APaonKeBiao pb3 = new APaonKeBiao();
                        pb3.教员名 = "自行安排";
                        pb3.内容 = "自习";
                        pb3.日期 = strDataShow;
                        pb3.时间段 = TimeFaleString[3];
                        pb3.授课班级 = XW_data2;
                        pb3.时间范围 = TimeFWString[3];
                        pb3.星期 = strWeekShow;
                        pb3.资源名 = zyname;
                        pb3.资源类型 = "机房";

                        //加入到集合中
                        apaokebiaoList.Add(pb3);

                    }
                }
                #endregion
            }
        }
        #endregion

        #region NPOI通过单元格获取类型的功能方法
        /// <summary>
        /// 根据Excel列类型获取列的值
        /// </summary>
        /// <param name="cell">Excel列</param>
        /// <returns></returns>

        private string GetCellValue(HSSFCell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case HSSFCellType.BLANK:
                    return string.Empty;
                case HSSFCellType.BOOLEAN:
                    return cell.BooleanCellValue.ToString();
                case HSSFCellType.ERROR:
                    return cell.ErrorCellValue.ToString();
                case HSSFCellType.NUMERIC:
                case HSSFCellType.Unknown:
                default:
                    return cell.ToString();//This is a trick to get the correct value of the cell. NumericCellValue will return a numeric value no matter the cell value is a date or a number
                case HSSFCellType.STRING:
                    return cell.StringCellValue;
                case HSSFCellType.FORMULA:
                    try
                    {

                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }


        #endregion
        #endregion

        #region 启用,该上传文件，即采用本次的数据
        /// <summary>
        /// 课表文件,启用需要采用操作
        /// </summary>
        /// <param name="kbno"></param>
        /// <returns></returns>
        public int QY(string kbno)
        {
            string sql = @" delete from KBNRTable where Guid_KBID!=@Guid_KBID
                            update KBFileTable set ISCD=1,IsCY=1 where Guid_kbfile=@Guid_KBID";
            SqlParameter[] pars = new SqlParameter[] 
            {
                new SqlParameter("@Guid_KBID",kbno) 
            };
            int n= kbnrDAL.ExcuteCommand(sql, pars);
            return n;
        }
        #endregion
    }
}
