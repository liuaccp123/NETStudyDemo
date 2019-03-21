using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DAL;
namespace BLL
{
    public partial class WXIdToStudentNameBLL:BLLBase<WXIdToStudentName>
    {
        WXIdToStudentNameDAL wxidtostudentNamedal = new WXIdToStudentNameDAL();

        public override void SetDAL()
        {
            this.dal = wxidtostudentNamedal;
        }
    }
}
