using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Models;
namespace BLL
{
   public partial class KBNRTableBLL:BLLBase<KBNRTable>
    {
       /// <summary>
       /// 
       /// </summary>
       KBNRTableDAL bknrDAL = new KBNRTableDAL();

        public override void SetDAL()
        {
            this.dal = bknrDAL;
        }
       //////////////////////////////

    }
}
