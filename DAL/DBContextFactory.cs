using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using System.Runtime.Remoting.Messaging;
namespace DAL
{
    /// <summary>
    /// 上下文件创建工厂
    /// 使用   单例模式/工厂模式
    /// </summary>
   public  class DBContextFactory
    {
       /// <summary>
       /// 提供上下文的工厂方法
       /// </summary>
       /// <returns></returns>
       public static CLKB_DBEntities CreateDbContext()
       {
           CLKB_DBEntities dbcontex = null;
           dbcontex = CallContext.GetData("db") as CLKB_DBEntities;
           if (dbcontex == null)
           {
               dbcontex = new CLKB_DBEntities();
               CallContext.SetData("db", dbcontex);
           }
           return dbcontex;
       }
    }
}
