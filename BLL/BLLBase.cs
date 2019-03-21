using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DAL;
namespace BLL
{
    public abstract class BLLBase<T> where T : class,new()
    {
        protected DALBase<T> dal;
        public BLLBase()
        {
            SetDAL();
        }
        public abstract void SetDAL();
        /////////////////////////////
        //增加
        public int Add(T t)
        {
            dal.Add(t);
            return dal.Save();
        }

        //修改
        public int Modify(T t)
        {
            dal.Modify(t);
            return dal.Save();
        }
        //删除
        public int Delete(T t)
        {
            dal.Delete(t);
            return dal.Save();
        }
        //单值查询[主值都为整型]
        public T GetOneData(object id)
        {
            return dal.GetOneData(id);
        }
        //查询所有数据
        public List<T> GetAllData()
        {
            return dal.GetAllData();
        }
        public  IQueryable<T> GetAllData2()
        {
            return dal.GetAllData2();
        }
    }
}
