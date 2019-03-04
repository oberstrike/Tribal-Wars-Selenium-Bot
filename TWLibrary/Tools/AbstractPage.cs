using OpenQA.Selenium.Firefox;
using TWLibrary.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWLibrary.Tools
{
    public abstract class AbstractPage
    {
        public abstract List<IUpdater> Updaters { get; } 


        public Village Village { get; set; }

        public abstract string URL { get; }


        public AbstractPage(Village village) {
            Village = village;
        } 

        public virtual void Update()
        {
            Village.Driver.GoTo(URL);

            foreach(var updater in Updaters)
            {
                updater.Update(Village);
            }
           
        }

    }

}
