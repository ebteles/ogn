using System;
using System.Collections.Generic;
using System.Linq;
using System.Net; // para postagem de página web
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using System.Web.Script.Serialization;
using gm.Geo;

using System.IO;

namespace gm.ws
{
    /// <summary>
    /// Summary description for gmGeo
    /// </summary>
    public class gmGeo : IHttpHandler
    {
        #region obrigatório
        public void ProcessRequest(HttpContext context)
        {
            string metodo = context.Request["method"];
            string retorno1 = string.Concat(context.Request.Params["jsonp_callback"], "({0});"); // jquery
            object retorno2 = new object();

            switch (metodo.ToLower())
            {
                // retorna KML a ser desenhado no google maps, conforme conexto.tipo_kml
                case "kml":
                    retorno2 = getKml(context);
                    break;

                // retorna dados conforme "filtro de rede de varejo" informado
                case "listalojas":
                    retorno2 = getListaLojas(context);
                    break;

                case "gmplaces": // google maps places
                    retorno2 = getGoogleMapsPlaces(context);
                    break;

                case "gmplacesetid": // salvar ids em arquivo texto
                    retorno2 = GoogleMapsPlaceSetID(context);
                    break;

                case "gmplacegetid": // recuperar ids previamente salvos e recuperar dados de cada um
                    retorno2 = GoogleMapsPlaceGetID(context);
                    break;

                default:
                    retorno2 = "{\"a_msg\":\"Método não localizado: " + metodo + "\"}";
                    break;
            }

            retorno1 = string.Format(retorno1, retorno2);

            context.Response.ContentType = "text/plain";
            context.Response.Write(retorno1);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region GET KML

        /// <summary>
        /// Compor dinamicamente arquivo KML para uso no Google Maps
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string getKml(HttpContext context)
        {

            string tipo_kml = context.Request["tipo_kml"];
            string bairro = context.Request["bairro"];
            string cidade = context.Request["cidade"];
            string regiao = context.Request["regiao"];
            string uf = context.Request["uf"];

            string kml = "";

            string kml_nome = "";
            string kml_descricao = "";
            string[] kml_poligono = new string[0];
            bool isMultiGeometry = false;

            try
            {
                switch (tipo_kml.ToLower())
                {
                    case "cidade":
                        cidade = cidade == "" ? regiao : cidade;
                        var oCid = new gm.Geo.Brasil.Cidade();
                        List<RegistroGeoBrasilCidade> oListaCidade = oCid.Listar(municipio: cidade, uf: uf);
                        if (oListaCidade.Count > 0)
                        {
                            kml_nome = oListaCidade[0].municipio;
                            kml_descricao = string.Format("{0}, {1}", oListaCidade[0].municipio, oListaCidade[0].uf);
                            kml_poligono = oListaCidade[0].poligono.Split('|');
                            isMultiGeometry = kml_poligono.Length > 1;
                        }
                        break;

                    case "uf":
                        var oUF = new gm.Geo.Brasil.UF();
                        List<RegistroGeoBrasilUF> oListaUF = oUF.Listar(uf: uf);
                        if (oListaUF.Count > 0)
                        {
                            kml_nome = oListaUF[0].descricao;
                            kml_descricao = string.Format("{0}, {1}", oListaUF[0].descricao, oListaUF[0].uf);
                            kml_poligono = oListaUF[0].poligono.Split('|');
                            isMultiGeometry = kml_poligono.Length > 1;
                        }
                        break;

                    case "meso":
                        cidade = cidade == "" ? regiao : cidade;
                        var oMeso = new gm.Geo.Brasil.Meso();
                        List<RegistroGeoBrasilMeso> oListaMeso = oMeso.Listar(uf: uf, municipio: cidade);
                        if (oListaMeso.Count > 0)
                        {
                            kml_nome = oListaMeso[0].meso_regiao;
                            kml_descricao = string.Format("{0}, {1}", oListaMeso[0].meso_regiao, oListaMeso[0].uf);
                            kml_poligono = oListaMeso[0].poligono.Split('|');
                            isMultiGeometry = kml_poligono.Length > 1;
                        }
                        break;

                    case "micro":
                        cidade = cidade == "" ? regiao : cidade;
                        var oMicro = new gm.Geo.Brasil.Micro();
                        List<RegistroGeoBrasilMicro> oListaMicro = oMicro.Listar(uf: uf, municipio: cidade);
                        if (oListaMicro.Count > 0)
                        {
                            kml_nome = oListaMicro[0].micro_regiao;
                            kml_descricao = string.Format("{0}, {1}", oListaMicro[0].micro_regiao, oListaMicro[0].uf);
                            kml_poligono = oListaMicro[0].poligono.Split('|');
                            isMultiGeometry = kml_poligono.Length > 1;
                        }
                        break;
                }

                // template kml (desmembrado em 3 partes para tratar casos "multigeometry")
                string kmlIni = string.Format(
                         "<!DOCTYPE kml>" +
                         "<kml xmlns=\"\">" +
                          "<Document>" +
                           "<Placemark>" +
                            "<name>{0}</name>" +
                            "<open>1</open>" +
                            "<description>{1}</description>", kml_nome, kml_descricao) +
                            (isMultiGeometry ? "<MultiGeometry>" : "");

                string kmlAux = "<Polygon>" +
                              "<outerBoundaryIs>" +
                                  "<LinearRing>" +
                                      "<coordinates>" +
                                          "{0}" +
                                      "</coordinates>" +
                                  "</LinearRing>" +
                              "</outerBoundaryIs>" +
                             "</Polygon>";

                string kmlFim = (isMultiGeometry ? "</MultiGeometry>" : "") +
                            "</Placemark>" +
                          "</Document>" +
                         "</kml>";

                StringBuilder sb = new StringBuilder();
                for (var i = 0; i < kml_poligono.Length; i++)
                {
                    sb.Append(string.Format(kmlAux, kml_poligono[i]));
                }
                kml = string.Concat(kmlIni, sb.ToString(), kmlFim);

                object oRetorno = new
                {
                    a_msg = "OK",
                    kml = kml
                };

                return new JavaScriptSerializer().Serialize(oRetorno);
            }
            catch (Exception ex)
            {
                string msgErro = ex.Message;
                object oRetorno = new
                {
                    a_msg = msgErro,
                };
                return new JavaScriptSerializer().Serialize(oRetorno);
            }
        }

        #endregion

        #region GET Dados de Rede de Varejo

        /// <summary>
        /// Retorna dados do Varejo, conforme filtros aplicados, para exibição em carta geográfica
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string getListaLojas(HttpContext context)
        {

            short codigo_rede = short.Parse(context.Request["codigo_rede"]);
            string codigo_loja = context.Request["codigo_loja"];
            string retorno = "";
            try
            {
                var oRede = new gm.Geo.Rede();
                List<RegistroGoogleMapsMarcas> oMarcas = oRede.ListaLojas(codigo_rede, codigo_loja);
                if (oMarcas.Count > 0)
                {
                    object oRetorno = new
                    {
                        a_msg = "OK",
                        marcas = oMarcas
                    };

                    retorno = new JavaScriptSerializer().Serialize(oRetorno);
                }
            }
            catch (Exception ex)
            {
                string msgErro = ex.Message;
                object oRetorno = new
                {
                    a_msg = msgErro,
                };
                retorno = new JavaScriptSerializer().Serialize(oRetorno);
            }

            return retorno;

        }

        #endregion

        #region GOOGLE MAPS Places

        /// <summary>
        /// Recuperar dados do google maps (usando webclient para funcionar também em localhost)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string getGoogleMapsPlaces(HttpContext context)
        {

            string texto = context.Request["texto"];
            string tipo = context.Request["tipo"];
            string retorno = "";

            switch (tipo.ToLower())
            {
                case "supermercado": tipo = "grocery_or_supermarket"; break;
                case "farma": tipo = "pharmacy"; break;
            }
            tipo = tipo == "" ? "grocery_or_supermarket" : tipo;
            
            try
            {
                string url = string.Format("https://maps.googleapis.com/maps/api/place/queryautocomplete/json?key={0}&input={1}&types={2}&language=pt-BR",
                                            getKey(),
                                            texto,
                                            tipo
                                          );

                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;

                    retorno = client.DownloadString(url);
                    retorno = Regex.Replace(retorno, @"\r|\n|\n", "");
                    retorno = string.Format("{{\"a_msg\": \"{0}\", \"o_places\": {1} }}", "OK", retorno); //new JavaScriptSerializer().Serialize(oRetorno);
                }

            }
            catch (Exception ex)
            {
                string msgErro = ex.Message;
                object oRetorno = new
                {
                    a_msg = msgErro,
                };
                retorno = new JavaScriptSerializer().Serialize(oRetorno);
            }

            return retorno;

        }

        /// <summary>
        /// Salvar places id em arquivo texto para uso posterior
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GoogleMapsPlaceSetID(HttpContext context)
        {

            GravarLog("GoogleMapsPlaceSetID - Início", true);

            string place_ids = context.Request["place_ids"];
            string local = context.Request["local"];
            string pesquisa = context.Request["pesquisa"];
            string id_ajax = context.Request["id_ajax"];
            string retorno = "";
            string msgErro = "";

            StreamWriter oFile = null;

            try
            {
                local = RemoverCaracteresEspeciais(RemoverAcentos(local.Trim())).Replace(" ","_");
                pesquisa = RemoverCaracteresEspeciais(RemoverAcentos(pesquisa.Trim())).Replace(" ", "_");

                string arquivo = string.Format(@"c:\temp\{0}_{1}_{2}.txt", "place_id", local, pesquisa);

                if (id_ajax == "1")
                    oFile = File.CreateText(arquivo);
                else
                    oFile = File.AppendText(arquivo);

                oFile.WriteLine(place_ids);
                oFile.Close();

                object oRetorno = new { a_msg = "OK" };
                retorno = new JavaScriptSerializer().Serialize(oRetorno);

            }
            catch (Exception ex)
            {
                msgErro = ex.Message;

                object oRetorno = new { a_msg = msgErro };
                try
                {
                    oFile.Close();
                }
                catch (Exception) { };

                GravarLog("GoogleMapsPlaceSetID - ERRO: " + msgErro);

                retorno = new JavaScriptSerializer().Serialize(oRetorno);
            }

            GravarLog("GoogleMapsPlaceSetID - Fim");
            return retorno;

        }

        /// <summary>
        /// Recuperar dados do google maps (usando webclient para funcionar também em localhost)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GoogleMapsPlaceGetID(HttpContext context)
        {

            GravarLog("GoogleMapsPlaceGetID - Início");

            string local = context.Request["local"];
            string pesquisa = context.Request["pesquisa"];
            string retorno = "";
            string msgErro = "";
            string linhaDados = "";

            int controle_ultimo_i = -1;
            int controle_mesmo_i = 0;

            StreamWriter oFile = null;

            try
            {

                local = RemoverCaracteresEspeciais(RemoverAcentos(local.Trim())).Replace(" ", "_");
                pesquisa = RemoverCaracteresEspeciais(RemoverAcentos(pesquisa.Trim())).Replace(" ", "_");

                string nome_arquivo = @"c:\temp\{0}_{1}_{2}.txt";
                string arquivo = string.Format(nome_arquivo, "place_id", local, pesquisa);
                string place_ids = "";
                

                if (File.Exists(arquivo))
                {
                    place_ids = File.ReadAllText(arquivo);
                    place_ids = Regex.Replace(place_ids, @"\r|\n|\n", "");
                }

                string[] place_id = place_ids.Split(',');

                // criar novo arquivo (sobrepor caso exista)
                arquivo = string.Format(nome_arquivo, "dados", local, pesquisa);
                oFile = File.CreateText(arquivo);
                oFile.WriteLine("rua;numero;cep;bairro;cidade;uf;pais;latitude;longitude;telefone;place_name;place_id");

                GravarLog(string.Format("GoogleMapsPlaceGetID - Loop para tratar {0} place_ids.", place_id.Length));

                for (var i = 0; i < place_id.Length; i++)
                {

                    string url = string.Format("https://maps.googleapis.com/maps/api/place/details/json?placeid={0}&key={1}&language=pt-BR",
                                                place_id[i],
                                                getKey()
                                              );

                    using (WebClient client = new WebClient())
                    {
                        client.Encoding = System.Text.Encoding.UTF8;

                        retorno = client.DownloadString(url);

                        RootObject oJson = new JavaScriptSerializer().Deserialize<RootObject>(retorno);

                        if (oJson.status != "OK")
                        {
                            msgErro = oJson.status;

                            GravarLog(string.Format("GoogleMapsPlaceGetID - ERRO Loop: {0}", msgErro));

                            if (msgErro == "INVALID_REQUEST")
                            {
                                if (controle_ultimo_i == i)
                                {
                                    if (++controle_mesmo_i >= 3)
                                    {
                                        controle_ultimo_i = i;
                                        controle_mesmo_i = 0;
                                        i++; // avançar para o próximo após três tentativas
                                             // break;
                                    }
                                }
                                else
                                {
                                    controle_ultimo_i = i;
                                    controle_mesmo_i = 0;
                                }

                                System.Threading.Thread.Sleep(5000); // aguardar 5 segundos e tentar novamente;
                                i--;

                                continue;
                            }
                            else
                            {
                                oFile.WriteLine(msgErro);
                                break;
                            }
                        }

                        string numero = "";
                        string rua = "";
                        string bairro = "";
                        string cidade = "";
                        string uf = "";
                        string pais = "";
                        string cep = "";
                        string latitude = "";
                        string longitude = "";
                        string telefone = "";
                        string place_name = "";

                        foreach (var oEndereco in oJson.result.address_components)
                        {
                            if (oEndereco.types.Count > 0)
                            {
                                switch (oEndereco.types[0])
                                {
                                    case "street_number": numero = oEndereco.long_name; break;
                                    case "route": rua = oEndereco.long_name; break;
                                    case "neighborhood": bairro = oEndereco.long_name; break;
                                    case "locality": cidade = oEndereco.long_name; break;
                                    case "administrative_area_level_1": uf = oEndereco.short_name; break;
                                    case "postal_code": cep = oEndereco.long_name; break;
                                    case "country": pais = oEndereco.long_name; break;
                                }
                            }
                        }

                        latitude = oJson.result.geometry.location.lat.ToString();
                        longitude = oJson.result.geometry.location.lng.ToString();
                        telefone = oJson.result.formatted_phone_number;
                        place_name = oJson.result.name;

                        linhaDados = string.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11}",
                                                rua,
                                                numero,
                                                cep,
                                                bairro,
                                                cidade,
                                                uf,
                                                pais,
                                                latitude,
                                                longitude,
                                                telefone,
                                                place_name,
                                                place_id[i]);

                        oFile.WriteLine(linhaDados);


                    }

                }

                oFile.Close();

                if (msgErro != "")
                {
                    throw new System.InvalidOperationException(string.Format("Google Maps status: {0}", msgErro));
                }

            }
            catch (Exception ex)
            {
                msgErro = ex.Message;
                object oRetorno = new
                {
                    a_msg = msgErro,
                };
                try
                {
                    oFile.Close();
                }
                catch (Exception) { };

                retorno = new JavaScriptSerializer().Serialize(oRetorno);
            }

            GravarLog("GoogleMapsPlaceGetID - FIM");

            return retorno;

        }

        #endregion

        #region OUTROS (private)

        private void GravarLog(string msgLog, bool novoArquivo = false)
        {
            string linha = string.Format("{0:dd/MM/yyyy hh:mm:ss.fff}: {1}", DateTime.Now, msgLog);
            StreamWriter oFile = null;
            
            string arquivo = @"c:\temp\apa_log.txt";
            if (novoArquivo)
                oFile = File.CreateText(arquivo);
            else
                oFile = File.AppendText(arquivo);

            oFile.WriteLine(linha);
            oFile.Close();
        }

        /// <summary>
        /// Remover acentos
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string RemoverAcentos(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            else
            {
                byte[] bytes = System.Text.Encoding.GetEncoding("iso-8859-8").GetBytes(input);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
        }

        /// <summary>
        /// Remover caracteres especiais
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string RemoverCaracteresEspeciais(string input)
        {
            Regex r = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return r.Replace(input, String.Empty);
        }

        private string getKey()
        {
            
            //string myKey = "AIzaSyC_xXdfnKS4xFT6Ha7a6Ux5JkPm8mLxlok";
            //string myKey = "AIzaSyDWlUgxawyoQYbaoX0VgOCTpx_ahqZVh3s";
            string myKey = "AIzaSyCQleSH_qZSoMDo5SzcAWu1S_RgGk8etpQ";
            return myKey;

        }
        #endregion

    }


}