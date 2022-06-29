/*
    Classe que renderiza google maps em formulário web
*/

var oGeo;
var oGeoMarcas = [];
var oGeoOpcoesAnalise = {
    criterioAnalise: '',
    habilitarSelecao: false,
    tipoSelecao: '',
    tipoSelecaoTxt: '',
    tipoSelecaoZoom: 8,
    listaPoligonos: [] // {nome: '', descricao: ''}
};

GMGeo = function () {

    //----------------------------------------------------------------------
    // variáveis privadas
    //----------------------------------------------------------------------
    var _oLatLngInicial;
    var _oMapa;
    var _oParser;
    var _oThis = this;
    var _cores = {
        padrao: '#2A6BA6',
        destaque: '#FFFF00',
        borda: '#FFFFFF'
    }

    var _oUltimoClick = { // latitude e longitude do último click
        latitude: null,
        longitude: null,
        latLng: null,
        dentroPoligono: false,
        posicaoPoligono: null
    };

    //----------------------------------------------------------------------
    // propriedades
    //----------------------------------------------------------------------
    this.elementoHtml = 'map';
    this.configuracao = {
        zoom: 8,
        center: _oLatLngInicial,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        mapTypeControlOptions: {
            style: google.maps.MapTypeControlStyle.DEFAULT
        }
    }

    //----------------------------------------------------------------------
    // métodos públicos (principais)
    //----------------------------------------------------------------------

    // metodo: inicializar
    this.inicializar = function (elementoHtml, latitude, longitude) {

        // assumir brasília (centro do mapa) nos casos em que coordenada inicial não for informada
        if (latitude === undefined || longitude == undefined) {
            latitude = -15.7941;
            longitude = -47.8879;
        }

        elementoHtml = elementoHtml === undefined
                     ? this.elementoHtml
                     : elementoHtml;

        var oElemento = document.getElementById(elementoHtml);

        oElemento.style.height = auxGetAltura();
        _oLatLngInicial = new google.maps.LatLng(latitude, longitude);
        this.configuracao.center = _oLatLngInicial;
        _oMapa = new google.maps.Map(oElemento, this.configuracao);

        // searchbox: início
        var input = document.getElementById('txtLocalizar');
        var autocomplete = new google.maps.places.Autocomplete(input);
        autocomplete.bindTo('bounds', _oMapa);

        google.maps.event.addListener(autocomplete, 'place_changed', function () {

            var place = autocomplete.getPlace();
            if (!place.geometry) {
                return;
            }

            var auxZoom = place.address_components[0].types[0] == 'locality'
                ? 8
                : oGeoOpcoesAnalise.tipoSelecaoZoom;
            _oMapa.setCenter(place.geometry.location);
            _oMapa.setZoom(auxZoom);

            auxAdicionarMarca('localizar', place.geometry.location, 'gm_pin_32_azul');
        });
        // searchbox: fim

        // adicionar sempre o evento 'click'
        google.maps.event.addListener(_oMapa, 'click', function (event) {

            auxRemoverMarca('localizar');

            _oUltimoClick.latitude = event.latLng.G;
            _oUltimoClick.longitude = event.latLng.K;
            _oUltimoClick.latLng = event.latLng;

            var pontoDentroPoligono = auxPontoDentroPoligono();
            _oUltimoClick.dentroPoligono = pontoDentroPoligono;

            //_oMapa.setCenter(event.latLng); // centralizar último click
            _oMapa.panTo(event.latLng);   // centralizar último click

            if (!pontoDentroPoligono && oGeoOpcoesAnalise.habilitarSelecao) {
                try { mapaOnClick('M'); } catch (e) { }; // evoca função a ser implementada pelo usuário
            }
        });

        _oParser = new geoXML3.parser({
            map: _oMapa,
            processStyles: true,
            singleInfoWindow: true,
            suppressInfoWindows: true,
            zoom: false,
            afterParse: auxAfterParse
        });

        // teste teles
        //auxGetPlaces("verdemar", "supermercado");

    };

    // Obtem dados de Polígo KML a partir de latitude e longitude informados
    // parâmetros de entrada:
    //      metodo: Ex: 'kml'
    //      tipo_kml: bairro, municipio, cidade, microregiao ibge e mesoregião ibge, uf, pais
    this.loadKML = function (metodo, tipo_kml) {

        metodo = metodo === undefined ? 'Kml' : metodo;
        tipo_kml = tipo_kml === undefined ? oGeoOpcoesAnalise.tipoSelecao : '';

        var oDados = {
            method: metodo,
            tipo_kml: tipo_kml,
            bairro: '',
            cidade: '',
            regiao: '',
            uf: '',
            uf_descricao: ''
        };

        auxGetDataFromLatLng(oDados);

        // critérios para busca do KML 
        if ((oGeoOpcoesAnalise.tipoSelecao == 'uf') ||
            (oGeoOpcoesAnalise.tipoSelecao == 'cidade' && (oDados.cidade.length > 0 || oDados.regiao.length > 0)) ||
            ('meso.micro'.indexOf(oGeoOpcoesAnalise.tipoSelecao) >= 0 && (oDados.cidade.length > 0 || oDados.regiao.length > 0))) {

            var url = '/gm/ws/gmGeo.ashx'; //var url = getPrefixoURL() + 'ws/allupaGeo.ashx';

            aguarde(true);

            var myFuncOK = function (d) {
                if (d.a_msg == 'OK') {
                    _oParser.parseKmlString(d.kml);
                    aguarde(false);
                }
                else {
                    aguarde(false);
                    myAlert(d.a_msg);
                }
            };

            execAjaxJsonP(url, oDados, myFuncOK);
        }
    };

    this.refresh = function () {
        try {
            while (_oParser.docs.length > 0) {
                var a = _oParser.docs[0].gpolygons[0];
                a.setMap(null);
                _oParser.docs.splice(0, 1);
            }

            document.getElementById('lstPoligonosTitulo').innerHTML = '';
            document.getElementById('lstPoligonosItens').innerHTML = '';
            $('#divLstPoligonosContainer').css('display', 'none');

            google.maps.event.trigger(_oMapa, 'resize');
            _oMapa.setZoom(oGeoOpcoesAnalise.tipoSelecaoZoom);
        }
        catch (e) {
            console.log('Teles error this.refresh: ', e) // não apagar!!!
        }
    }

    // Efeito highlight sobre polígono
    //
    this.destacaPoligono = function (destaque, pm) {
        var oPoly = _oParser.docs[pm].gpolygons[0];
        var oCor = destaque == 1
                 ? { fillColor: _cores.destaque }
                 : { fillColor: _cores.padrao };
        oPoly.setOptions(oCor);
    }


    // 
    // Zoom em polígono ao clicar sobre o nome dele na janela que
    // lista os poligonos.
    //
    this.zoomPoligono = function (pm) {
        _oMapa.fitBounds(_oParser.docs[pm].placemarks[0].polygon.bounds);
    }
    this.zoomMapa = function () {
        _oMapa.setZoom(oGeoOpcoesAnalise.tipoSelecaoZoom);
    }


    this.adicionarMarca = function (oMarcas, remover) {

        if (!(remover === undefined)) {
            auxRemoverMarca(remover);
        }

        for (var i = 0; i < oMarcas.length; i++) {
            var latLng;
            var erro = '';
            try {
                latLng = new google.maps.LatLng(oMarcas[i].latitude, oMarcas[i].longitude);
                auxAdicionarMarca(oMarcas[i].id_marca, latLng, oMarcas[i].icone);
            }
            catch (e) {
                erro += (oMarcas[i].id_marca + ', ');
            }
        }
        if (erro.length > 0) {
            myAlert('Não foi possível identificar as coordenadas dos itens abaixo:<br/><br/>' + erro);
        }
    }
    //----------------------------------------------------------------------
    // Métodos auxiliares privados (consumidos internamente)
    //----------------------------------------------------------------------

    // apoio: Calcular altura da área de exibição do mapa
    //        Necessário informar em 'px' por estar dentro
    //        de uma masterpage. Não fosse por isso e a
    //        solução seria informar height='100%' por exemplo.
    function auxGetAltura() {
        var body = document.body;
        var html = document.documentElement;
        var height = Math.max(body.scrollHeight, body.offsetHeight,
                               html.clientHeight, html.scrollHeight, html.offsetHeight);

        height -= 60; // 60 + 60 das duas linhas de header;
      
        return height + 'px';
    }

    function auxAfterParse(doc) {

        if (doc[0].gpolygons.length == 0) {
            return;
        }

        //listar polígonos na janela flutuante:
        auxListarPoligonos(doc);

        var oPoly = doc[0].gpolygons[0];

        oPoly.setOptions({
            //title: 'Teste',
            strokeWeight: 1.0,
            strokeColor: _cores.borda,
            fillColor: _cores.padrao,
            fillOpacity: 0.6
        });

        // adicionar sempre o evento 'click'
        google.maps.event.addListener(oPoly, 'click', function (event) {

            if (oGeoOpcoesAnalise.habilitarSelecao) {
                this.setMap(null);
                for (var i = 0; i < _oParser.docs.length; i++) {
                    var a = _oParser.docs[i].gpolygons[0];
                    if (a.map == null) {
                        _oParser.docs.splice(i, 1);
                        break;
                    }
                }
                auxListarPoligonos();
            }
            else {
                try { mapaOnClick('P'); } catch (e) { }
            }
        });

        google.maps.event.addListener(oPoly, "mouseover", function () {
            $('li:contains("' + oPoly.title + '")').css('background-color', _cores.destaque);
            this.setOptions({ fillColor: _cores.destaque });
        });
        google.maps.event.addListener(oPoly, "mouseout", function () {
            $('li:contains("' + oPoly.title + '")').css('background-color', '');
            this.setOptions({ fillColor: _cores.padrao });
        });
    }

    // MÉTODO: adiciona "marker" em uma determinada coordenada
    //         parâmetro de entrada: referencia
    //              1. id:  string de identificação da marca
    //              2. location: coordenadas de inserção da marca
    //              3. icon: nome da figura (sem path e sem extensão)
    function auxAdicionarMarca(id, location, icon) {

        var pathIcon = '/apa/Imagens/mod11/#icon#.png';

        location = (location === undefined || location == null)
                    ? _oUltimoClick.latLng
                    : location;

        icon = (icon === undefined || icon == null)
                ? pathIcon.replace('#icon#', 'gm_pin_32_azul')
                : pathIcon.replace('#icon#', icon);

        var marker = new google.maps.Marker({
            position: location,
            map: _oMapa,
            icon: icon,
            title: id
        });

        oGeoMarcas.push({ id: id.toLowerCase(), marca: marker });
    }

    //
    // Remoção de Marca. Parâmetro:
    //      id: identificar da marca ou 'all' para todas
    //
    function auxRemoverMarca(id) {
        id = id.toLowerCase();
        if (id == 'all') {
            for (var i = 0; i < oGeoMarcas.length; i++) {
                oGeoMarcas[i].marca.setMap(null);
            }
            oGeoMarcas = [];
        }
        else {
            var i = 0;
            while (i < oGeoMarcas.length) {
                if (oGeoMarcas[i].id == id) {
                    oGeoMarcas[i].marca.setMap(null);
                    oGeoMarcas.splice(i, 1);
                }
                else {
                    i++;
                }
            }
        }
    }

    function auxListarPoligonos(doc) {

        var t = '<ul class="map_poly_ul">';
        var r = '';
        var f = _oParser.docs.length;
        var show = f > 0;
        var nome = '';
        var nomes = '';
        var i = 0;

        oGeoOpcoesAnalise.listaPoligonos = [];

        for (i = 0; i < f; i++) {
            var nome = _oParser.docs[i].placemarks[0].name;
            var desc = _oParser.docs[i].placemarks[0].description;

            nomes += (',' + nome);
            r += '<li id="li' + i + '" onclick="oGeo.zoomPoligono(' + i + ')" onmouseover="oGeo.destacaPoligono(1, ' + i + ')" onmouseout="oGeo.destacaPoligono(0, ' + i + ')">' + nome + '</li>';

            oGeoOpcoesAnalise.listaPoligonos.push({ nome: nome, descricao: desc });
        }

        if (!(doc === undefined)) {
            var nome = doc[0].placemarks[0].name;
            var desc = doc[0].placemarks[0].description;

            if (nomes.indexOf(nome) < 0) {
                show = true;
                r += '<li id="li' + i + '" onclick="oGeo.zoomPoligono(' + i + ')" onmouseover="oGeo.destacaPoligono(1, ' + i + ')" onmouseout="oGeo.destacaPoligono(0, ' + i + ')">' + nome + '</li>';
                oGeoOpcoesAnalise.listaPoligonos.push({ nome: nome, descricao: desc });
            }
        }

        t += r + '</ul>';

        document.getElementById('lstPoligonosTitulo').innerHTML = oGeoOpcoesAnalise.tipoSelecaoTxt;
        document.getElementById('lstPoligonosItens').innerHTML = t;

        $('#divLstPoligonosContainer').css('display', (show ? '' : 'none'));

    }

    //
    // A partir de coordenadas (lat e lng) retorna
    // json com informações gerais (rua, cep, cidade, uf, etc.)
    //
    function auxGetDataFromLatLng(oRetorno) {
        var lat, lng;
        if (_oUltimoClick.latitude == null) {
            lat = _oLatLngInicial.G;
            lng = _oLatLngInicial.K;
        }
        else {
            lat = _oUltimoClick.latitude;
            lng = _oUltimoClick.longitude;
        }

        $.ajax({
            url: 'http://maps.googleapis.com/maps/api/geocode/json',
            data: 'latlng=' + lat + ',' + lng + '&sensor=false',
            dataType: 'json',
            cache: false,
            async: false,
            success: function (data) {
                switch (data.status) {
                    case "OK":
                        if (data.results.length > 0) {
                            var o = data.results[0].address_components;
                            var bairro = '';
                            var cidade = '';
                            var regiao = '';
                            var uf = '';
                            var uf_descricao = '';
                            for (var i = 0; i < o.length; i++) {
                                switch (o[i].types[0]) {
                                    case 'neighborhood': bairro = o[i].long_name; break;
                                    case 'locality': cidade = o[i].long_name; break;
                                    case 'administrative_area_level_1':
                                        uf = o[i].short_name;
                                        uf_descricao = o[i].long_name;
                                        break;
                                    case 'administrative_area_level_2': regiao = o[i].short_name; break;

                                }
                            }
                            oRetorno.uf = uf;
                            oRetorno.uf_descricao = uf_descricao;
                            oRetorno.regiao = regiao;
                            oRetorno.cidade = cidade;
                            oRetorno.bairro = bairro;
                        }
                        break;
                    case "ZERO_RESULTS":
                        myAlert('<h2>Ops,<br/>Google Maps não retornou resultado.')
                        break;
                    case "OVER_QUERY_LIMIT":
                        myAlert('<h2>Ops,<br/>Sua cota diária de utilização do Google Maps foi atingida.')
                        break;
                    case "REQUEST_DENIED":
                        myAlert('<h2>Ops,<br/>Google Maps negou-se a atender a seu pedido.')
                        break;
                    case "INVALID_REQUEST":
                        myAlert('<h2>Ops,<br/>Google Maps não reconheceu endereço, componente ou coordenadas (LatLng).')
                        break;
                    case "UNKNOWN_ERROR":
                        myAlert('<h2>Ops,<br/>Google Maps não pode processar seu pedido devido a problemas no(s) servidor(es).')
                        break;
                }
            }
        });
    }

    // MÉTODO: Retorna true/false se as coordenadas do último 'click' estão dentro ou fora de um polígono
    function auxPontoDentroPoligono() {

        var ok = false;
        var len = _oParser.docs.length;
        if (len > 0) {
            var l = _oUltimoClick.latLng;
            for (var i = 0; i < len; i++) {
                var p = _oParser.docs[i].gpolygons[0];
                ok = google.maps.geometry.poly.containsLocation(l, p);
                _oUltimoClick.posicaoPoligono = i;
                if (ok) break;
            }
        }
        return ok;
    }

    //
    // Recuperar lista no format JSON conforme tipo especificado
    // Parâmetros:  (1) tipo: padrão é "grocery_or_supermarket"
    //                        lista de tipos: https://developers.google.com/places/supported_types            
    //              (2) texto: o que será localizado. Exemplor "verdemar"
    //              (3) key: chave
    //
    //function auxGetPlaces(texto, tipo) {
    //    var url = '/gm/ws/gmGeo.ashx';
    //    var param = {
    //        method: 'gmplaces',
    //        texto: texto,
    //        tipo: tipo
    //    };
    //    var func = function (d) {
    //        if (d.a_msg == "OK") {
    //            if (d.o_places.status == "OK") {
    //                var oPlaces = d.o_places.predictions;
    //            }
    //            else {
    //                myAlert('<h2>Ops</h2> Não foi possível recuperar dados. Status: ' + d.o_places.status);
    //            }
    //        }
    //        else {
    //            myAlert(d.a_msg);
    //        }
    //    }
    //    execAjaxJsonP(url, param, func);
    //}

}


//
// Função genérica quer retorna JsonP
// Parâmetros:  url:    Ex: '/apa/ws/pagina.aspx
//              dados:  Ex: {method: 'kml', param1: 'a', param2: 'b', etc: 'c' }
//              funcao: Função a ser executada em caso de sucesso. 
//
// Exemplo de uso:
//
//          var oDados = {
//              p1: 'abc',
//              pn: 123
//          };
//          var myFuncOK = function (d) {
//              if (d.a_msg == 'OK') 
//                  alert('OK')
//              else 
//                  alert('não ok');
//              };
//          execuAjaxJsonP('/path/pagina', oDados, muFuncOK);
//
function execAjaxJsonP(url, dados, funcaoOK) {
    $.ajax({
        dataType: 'jsonp',
        async: true,
        jsonp: 'jsonp_callback',
        contentType: "application/javascript;charset=ISO-8859-1",
        url: url,
        data: dados,
        success: function (d) {
            funcaoOK(d);
        },
        error: function (xhr, status, error) {
            aguarde(false);
            myAlert('Exceção não tratada: ' + xhr.responseText);
            console.log('Erro', xhr, status, error)
        }

    });
}