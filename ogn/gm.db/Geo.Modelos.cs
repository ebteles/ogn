using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gm.Geo
{

    #region GEO_BRASIL
    public class RegistroGeoBrasilUF
    {
        public string uf { get; set; }
        public string descricao { get; set; }
        public string poligono { get; set; }
    }

    public class RegistroGeoBrasilMeso
    {
        public short id_geo_meso { get; set; }
        public string uf { get; set; }
        public string meso_regiao { get; set; }
        public string poligono { get; set; }
    }

    public class RegistroGeoBrasilMicro
    {
        public short id_geo_micro { get; set; }
        public short id_geo_meso { get; set; }
        public string uf { get; set; }
        public string micro_regiao { get; set; }
        public string poligono { get; set; }
    }

    public class RegistroGeoBrasilCidade
    {
        public int id_geo_cidade { get; set; }
        public string categoria { get; set; }
        public string uf { get; set; }
        public string municipio { get; set; }
        public string distrito { get; set; }
        public int? populacao { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string altitude { get; set; }
        public string cep { get; set; }
        public string poligono { get; set; }
        public short? id_geo_meso { get; set; }
        public short? id_geo_micro { get; set; }
    }

    #endregion

    #region #Google_Maps

    /// <summary>
    /// Lista de overlays do tipo "Marker" para ser inseridos no Google Maps
    /// Exemplo: Lista de Lojas
    /// </summary>
    public class RegistroGoogleMapsMarcas
    {
        public string id_marca { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string icone { get; set; }
        public object complemento { get; set; }
    }

    #endregion

    #region Google_Maps_Places (JSON)
    //
    // Classes criadas a partir de um objeto json gerado conforme exemplo abaixo:
    // ex: https://maps.googleapis.com/maps/api/place/details/json?placeid=ChIJxa8YUymYpgARU95or1evz84&key=API_KEY
    // 
    // Conversão de json para classes feitas via site http://http://json2csharp.com/
    //
    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
    }

    public class Close
    {
        public int day { get; set; }
        public string time { get; set; }
    }

    public class Open
    {
        public int day { get; set; }
        public string time { get; set; }
    }

    public class Period
    {
        public Close close { get; set; }
        public Open open { get; set; }
    }

    public class OpeningHours
    {
        public bool open_now { get; set; }
        public List<Period> periods { get; set; }
        public List<string> weekday_text { get; set; }
    }

    public class Photo
    {
        public int height { get; set; }
        public List<string> html_attributions { get; set; }
        public string photo_reference { get; set; }
        public int width { get; set; }
    }

    public class Aspect
    {
        public int rating { get; set; }
        public string type { get; set; }
    }

    public class Review
    {
        public List<Aspect> aspects { get; set; }
        public string author_name { get; set; }
        public string author_url { get; set; }
        public string language { get; set; }
        public int rating { get; set; }
        public string text { get; set; }
        public int time { get; set; }
    }

    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public string adr_address { get; set; }
        public string formatted_address { get; set; }
        public string formatted_phone_number { get; set; }
        public Geometry geometry { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public string international_phone_number { get; set; }
        public string name { get; set; }
        public OpeningHours opening_hours { get; set; }
        public List<Photo> photos { get; set; }
        public string place_id { get; set; }
        public double rating { get; set; }
        public string reference { get; set; }
        public List<Review> reviews { get; set; }
        public string scope { get; set; }
        public List<string> types { get; set; }
        public string url { get; set; }
        public int user_ratings_total { get; set; }
        public int utc_offset { get; set; }
        public string vicinity { get; set; }
        public string website { get; set; }
    }

    public class RootObject
    {
        public List<object> html_attributions { get; set; }
        public Result result { get; set; }
        public string status { get; set; }
    }
    #endregion
}
