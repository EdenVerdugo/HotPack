// See https://aka.ms/new-console-template for more information

using HotPack.App;
using HotPack.Classes;
using HotPack.Database;
using HotPack.Test;
using System.Data;
using System.Net;
using System.Reflection;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var articulo = new TestModel();
        var xml = articulo.ToXml();

        var path = Globals.Instance.ApplicationPath;
        var version = Globals.Instance.ApplicationVersion;
        var name = Globals.Instance.ApplicationName;

        var defaultParams = new ConexionParametersBuilder();
        defaultParams.AddDefaultOutputParam("pResultado", ConexionDbType.Bit);
        defaultParams.AddDefaultOutputParam("pMsg", ConexionDbType.VarChar, 300);

        var parametros = defaultParams.CreateDefault();
        parametros.Add("pCodSucursal", ConexionDbType.Int, 9);

        

        var conexion = new Conexion("data source = 172.19.1.202; initial catalog = farmasiweb; user id = sa; password = Dosmil14; TrustServerCertificate=True");


        await conexion.ExecuteWithResultsAsync("ProcCatArticulosNuevosCon", parametros, (row) =>
        {
            var codArticulo = row["CodArticulo"].ToInt32();
            var descripcion = row["Descripcion"].ToString();
        });


        var fecha = await conexion.ExecuteScalarAsync<DateTime>("select getdate()", commandType: System.Data.CommandType.Text);             

        var value = await conexion.ExecuteToObjectAsync<TestModel>("ProcCatArticulosNuevosCon", parametros);
        

        var resultList = new ResultList<TestModel>();

        var pr = defaultParams.CreateDefault();

        pr.Add("pChile", ConexionDbType.Int, 299);

        await conexion.ExecuteWithMultipleResultsAsync("SELECT 1 as CodArticulo; select 2 as CodArticulo", null, async (r) =>
        {
            var a = await r.ResultsToObjectAsync<TestModel>();

            var b = await r.ResultsAsync<TestModel>();

        }, System.Data.CommandType.Text);

        //var art = await conexion.ExecuteWithResultsAsync<TestModel>("ProcCatArticulosNuevosCon", parametros);

        try
        {

        }
        catch (Exception ex)
        {
            ex.Log();
        }

        int x = 0;
    }
}