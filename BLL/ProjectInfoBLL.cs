using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DAL;
namespace BLL
{
    public partial class ProjectInfoBLL : BLLBase<ProjectInfo>
    {
        //定义子类访问对象

        ProjectInfoDAL projectinfodal = new ProjectInfoDAL();
        //给访问对象赋值
        public override void SetDAL()
        {
            this.dal = projectinfodal;
        }
    }
}
