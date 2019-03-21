using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    /// <summary>
    /// 操作结果类
    /// </summary>
    public  class OptionResult
    {
        /// <summary>
        /// 操作返回代码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 操作返回描述
        /// </summary>
        public string Message { get; set; }
    }
}
