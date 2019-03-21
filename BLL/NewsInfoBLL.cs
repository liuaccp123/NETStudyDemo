using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using DAL;
namespace BLL
{
    public partial class NewsInfoBLL : BLLBase<NewsInfo>
    {
        NewsInfoDAL newsdal = new NewsInfoDAL();
        public override void SetDAL()
        {
            this.dal = newsdal;
        }
    }
}
