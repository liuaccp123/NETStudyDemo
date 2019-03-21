using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DAL;

namespace BLL
{
    public partial  class EmpInfoBLL :  BLLBase<EmpInfo>
    {
        EmpInfoDAL empinfodal = new EmpInfoDAL();

        public override void SetDAL()
        {
            this.dal = empinfodal;
        }
    }
}
