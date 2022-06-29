using System;

namespace gm
{
    public partial class mp : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (IsPostBack)
            {
                return;
            }

            try
            {
                string usuario = Session["seg_usuario"] == null ? "" : Session["seg_usuario"].ToString().Trim();
                string nome_rede = Session["seg_nome_rede"] == null ? "Rede ???" : Session["seg_nome_rede"].ToString().Trim();

                string controle = Session["pv_controle"] == null ? "" : Session["pv_controle"].ToString().Trim();

                Session["pv_controle"] = "";

                string rede_logo = "";
                try
                {
                    rede_logo = Session["dashboard_nome_rede"].ToString();
                }
                catch (Exception) { }

                if (usuario.Length > 0)
                {
                    string u = usuario.Substring(0, usuario.IndexOf("@")).ToLower();
                    u = u.Substring(0, 1).ToUpper() + u.Substring(1) + "...";
                    //lblUsuario.InnerHtml = "<span>" + u + "</span>";
                }
                //else
                //    lblUsuario.InnerHtml = "";

            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

    }
}