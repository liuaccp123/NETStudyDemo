//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class WeChatUsers
    {
        public int Id { get; set; }
        public string UnionId { get; set; }
        public string OpenID { get; set; }
        public string NickName { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string AvatarUrl { get; set; }
        public string Appid { get; set; }
        public string Timestamp { get; set; }
        public string Memo { get; set; }
        public Nullable<int> counts { get; set; }
    }
}
