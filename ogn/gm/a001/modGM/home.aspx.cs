using System;
using System.Web.Script.Serialization;

namespace gm.a001.modGM
{
    public partial class home : System.Web.UI.Page
    {
        #region ASP.NET

        private gm.GlobalPages.PageParametros oPageParametros = new gm.GlobalPages.PageParametros();

        protected void Page_Load(object sender, EventArgs e)
        {

            if (IsPostBack)
            {
                return;
            }

            try
            {

                var oPage = new gm.GlobalPages();
                object oGoogleMapas = new
                {
                    tp = "grocery_or_supermarket",                  // tipo de pesquisa
                    ch = "AIzaSyC_xXdfnKS4xFT6Ha7a6Ux5JkPm8mLxlok"  // key (google maps)
                };
                oPage.InicializarVariaveis(ref oPageParametros);
                oPageParametros.outros = new JavaScriptSerializer().Serialize(oGoogleMapas);
                divParametrosGerais.InnerHtml = oPage.SerializePageParametros(oPageParametros);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
