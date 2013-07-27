using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WCFServiceWebRole1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : IService1
    {
        public List<ProjAppTable> ViewProfile()
        {

            using (var context = new SadAss2SQLEntities())
            {

                // select query (linq style) to select all profiles from user_profile table in database

                var results = from r in context.ProjAppTables

                              select r;

                // return results to list which will we bind to a listbox in our mobile app

                return results.ToList();

            }

        }
    }
}
