﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CLKB_DBEntities : DbContext
    {
        public CLKB_DBEntities()
            : base("name=CLKB_DBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<KBFileTable> KBFileTable { get; set; }
        public DbSet<KBNRTable> KBNRTable { get; set; }
        public DbSet<KBFileTabel_zy> KBFileTabel_zy { get; set; }
        public DbSet<KBNRTable_ALL> KBNRTable_ALL { get; set; }
        public DbSet<AdminInfo> AdminInfo { get; set; }
        public DbSet<EmpInfo> EmpInfo { get; set; }
        public DbSet<NewsInfo> NewsInfo { get; set; }
        public DbSet<ProjectInfo> ProjectInfo { get; set; }
        public DbSet<TabClassInfo> TabClassInfo { get; set; }
        public DbSet<TabQuest> TabQuest { get; set; }
        public DbSet<TabStudentInfo> TabStudentInfo { get; set; }
        public DbSet<WXIdToStudentName> WXIdToStudentName { get; set; }
        public DbSet<WeChatUsers> WeChatUsers { get; set; }
    }
}
