using System;
using System.Web;

namespace gm.a001
{

    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string usuario = Session["seg_usuario"] == null ? "" : Session["seg_usuario"].ToString();

            if (!IsPostBack)
            {
                string plataforma = "";
                try
                {
                    plataforma = Request.QueryString["vm"].ToString(); // informado na tela de login
                }
                catch (Exception) { }
                finally
                {
                    if (plataforma == "")
                    {
                        try
                        {
                            plataforma = Session["seg_plataforma"].ToString();
                        }
                        catch (Exception)
                        {
                            var o = new gm.GlobalLogin();
                            plataforma = o.DetectarPlataforma();
                            o = null;
                        }
                    }
                }

                if (usuario == "")
                    Response.Redirect("~/a001/login.aspx?modulo=gm", false);
                else
                    Response.Redirect("~/a001/modGM/home.aspx", false);
                
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();

            }

        }

    }
}