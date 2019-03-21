using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DAL;
using System.Data.SqlClient;
namespace BLL
{
   public partial   class KBFileTabel_zyBLL :  BLLBase<KBFileTabel_zy>
    {
       KBFileTabel_zyDAL zydalDal = new KBFileTabel_zyDAL();

        public override void SetDAL()
        {
            this.dal = zydalDal;
        }

        #region 把本周的课表数据增加到历史记录中，并删除本次数据
        public int AddDataIntoHistoryKb()
        {
            string strdata = DateTime.Now.ToString("yyyy-MM-dd");
            //获取当前时间
            string sql = @" Insert into KBNRTable_ALL(Guid_Kb, KB_RQ, 
                    KB_XQ, KB_ZYM, KB_NR, KB_JYM, KB_SKBJ, KB_SJD,
                     KB_SJFW, KB_ZYLX, Guid_KBfile) 
                     select  Guid_Kb, KB_RQ, KB_XQ, KB_ZYM, KB_NR, 
                     KB_JYM, KB_SKBJ, 
                     KB_SJD, KB_SJFW, KB_ZYLX, Guid_KBID
                     from KBFileTabel_zy where KB_RQ<@KB_RQ
 
                     delete from KBFileTabel_zy where KB_RQ<@KB_RQ";
            SqlParameter[] pars = new SqlParameter[] { 
                new SqlParameter("@KB_RQ",strdata) 
            };
            int n = zydalDal.ExcuteCommand(sql,pars);
            return n;
        }
        #endregion

        #region 下周课表变成当周课表,思路当今天的日期查询在数据表有就操作
        public bool AddDataIntoHistoryKb2()
        {
            bool f = false;
            string sql1 = @"select count(*) from KBNRTable where cast(KB_RQ as datetime)<=@kb";

            SqlParameter[] pars1 = new SqlParameter[] { 
                new SqlParameter("@kb",DateTime.Now) 
            };

            List<int> a = zydalDal.GetDataBySQL<int>(sql1, pars1);//大于0说明

            if (a[0] > 0)
            {
                string sql = @"Insert into KBFileTabel_zy(Guid_Kb, KB_RQ, 
                                KB_XQ, KB_ZYM, KB_NR, KB_JYM, KB_SKBJ, KB_SJD,
                                 KB_SJFW, KB_ZYLX, Guid_KBID) 
                                 select  Guid_Kb, KB_RQ, KB_XQ, KB_ZYM, KB_NR, KB_JYM, KB_SKBJ, 
                                 KB_SJD, KB_SJFW, KB_ZYLX, Guid_KBID
                                 from KBNRTable
 
                                 delete from KBNRTable";
                int n = zydalDal.ExcuteCommand(sql);
            }
            return f;
        }
        #endregion
    }
}
