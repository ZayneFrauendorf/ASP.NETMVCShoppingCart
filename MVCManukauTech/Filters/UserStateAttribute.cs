using System.Web.Mvc;

namespace MVCManukauTech
{
    public class UserStateAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);

            if (filterContext?.HttpContext?.Session == null ||
                filterContext.HttpContext?.Request == null) return;

            var checkMembershipRaw = filterContext.HttpContext.Session["IsMembershipExpired"];
            if (null != checkMembershipRaw)
            {
                var checkData = checkMembershipRaw.ToString();
                if (checkData == "YES")
                {
                    filterContext.Controller.ViewBag.IsMembershipExpired = true;
                }
                else
                {
                    filterContext.Controller.ViewBag.IsMembershipExpired = false;
                }
            }
            else
            {
                filterContext.Controller.ViewBag.IsMembershipExpired = false;
            }
        }
    }
}