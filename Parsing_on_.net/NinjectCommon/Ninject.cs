using Ninject;
using Parsing_on_.net.BLL;
using Parsing_on_.net.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parsing_on_.net.NinjectCommon
{
    public class Ninject
    {
        private static readonly IKernel _kernel = new StandardKernel();

        public static IKernel Kernel => _kernel;

        public static void Registration()
        {
            _kernel.Bind<IShopDAO>().To<ShopDAO>();
            _kernel.Bind<IParsingLogic>().To<ParsingLogic>();
        }
    }
}