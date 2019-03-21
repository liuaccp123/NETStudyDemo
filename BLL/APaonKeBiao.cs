using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    /// <summary>
    /// 安排的课表对应实体结构:两节课一次
    /// </summary>
    public class APaonKeBiao
    {
        public string 日期 { get; set; }
        public string 星期 { get; set; }
        public string 资源名 { get; set; }//教室机房的编号
        public string 内容 { get; set; }//授课内容
        public string 教员名 { get; set; }//教员名
        public string 授课班级 { get; set; }
        public string 时间段 { get; set; }//时间段
        public string 时间范围 { get; set; } //上午8:30-12:20 //暂不实现晚上上课模型
        public string 资源类型 { get; set; }//机房还是教室
    }
}
