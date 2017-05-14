using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SambariEnterprises.Helpers
{
    public class CookieHelper
    {
        public static string CookieName { get; set; }
        public virtual User User { get; set; }
        public virtual Application App { get; set; }


        public MyCookie(Application app)
        {
            CookieName = "MyCookie" + app;
            App = app;
        }

        public void SetCookie(User user)
        {
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[CookieName] ?? new HttpCookie(CookieName);
            myCookie.Values["UserId"] = user.UserId.ToString();
            myCookie.Values["LastVisit"] = DateTime.Now.ToString();
            myCookie.Expires = DateTime.Now.AddDays(365);
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }

        public HttpCookie GetCookie()
        {
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[CookieName];
            if (myCookie != null)
            {
                int userId = Convert.ToInt32(myCookie.Values["UserId"]);
                User user = session.Get<User>(userId);
                return user;
            }
            return null;
        }
    }
}