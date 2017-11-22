namespace MVCManukauTech.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MembershipChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "MembershipPayId", c => c.String());
            AddColumn("dbo.AspNetUsers", "MemberExpireAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "MemberExpireAt");
            DropColumn("dbo.AspNetUsers", "MembershipPayId");
        }
    }
}
