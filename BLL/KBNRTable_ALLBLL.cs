using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DAL;
namespace BLL
{
    public partial class KBNRTable_ALLBLL:BLLBase<KBNRTable_ALL>
    {
        KBNRTable_ALLDAL kbnrtable_all = new KBNRTable_ALLDAL();

        public override void SetDAL()
        {
            this.dal = kbnrtable_all;
        }
    }
}
