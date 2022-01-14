using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using PS7Api.Controllers;
using PS7Api.Models;
using PS7Api.UnitTests;
using PS7Api.Utilities;
using Xunit;

namespace EndToEndTests;

public class Scenario
{
    [Fact]
    public async Task S1()
    {
        await using var app = new Ps7Fixture();
        var client = app.CreateClient();
        
        //juliette récupère la liste des documents

        var queryJuliette = new Dictionary<string, string>
        {
            ["nationality"] = "FR",
            ["origin"] = "FR",
            ["destination"] = "GB",
        };
        var responseJuliette = await client.GetAsync(QueryHelpers.AddQueryString("/api/RequiredDocument/", queryJuliette));
        var resJuliette = await responseJuliette.Content.ReadFromJsonAsync<List<string>>();

        Assert.Equal(HttpStatusCode.OK, responseJuliette.StatusCode);
        Assert.Equal(4, resJuliette.Count);
        
        //Leslie scan le document de juliette puis elle passe

        client.Login("customs");
        var crossInfo = new CrossingInfo(new TollOffice("fr"));
        var resp = await client.PostAsync("/api/CrossingInfo", JsonContent.Create(crossInfo));
        var crossInfoResp = await resp.Content.ReadFromJsonAsync<CrossingInfo>();
        var crossInfoId = crossInfoResp?.Id;
        
        var imgBytes = await File.ReadAllBytesAsync("../../../Image/declaration_douane.png");
        var content = new MultipartFormDataContent { { new ByteArrayContent(imgBytes), "file", "image.jpg" } };
        var res = await client.PostAsync("/api/CrossingInfo/"+crossInfoId+"/Document", content);

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        
        var query = new Dictionary<string, string?>
        {
            ["id"] = "1",
            ["tollId"] = "2"
        };
        var result = await client.PatchAsync(QueryHelpers.AddQueryString("/api/CrossingInfo/", query), JsonContent.Create(DateTime.Now.AddDays(1).Iso8601()));
        Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        
        //todo Lucien consulte les flux
        
    }

    [Fact]
    public async Task S2()
    {
        await using var app = new Ps7Fixture();
        var client = app.CreateClient();
        
        //Leslie scan un document qui est invalide
        
        client.Login("customs");
        var crossInfo = new CrossingInfo(new TollOffice("fr"));
        var resp = await client.PostAsync("/api/CrossingInfo", JsonContent.Create(crossInfo));
        var crossInfoResp = await resp.Content.ReadFromJsonAsync<CrossingInfo>();
        var crossInfoId = crossInfoResp?.Id;
        
        var imgBytes = await File.ReadAllBytesAsync("../../../Image/declaration_douane.png");
        var content = new MultipartFormDataContent { { new ByteArrayContent(new byte[42]), "file", "image.jpg" } };
        var res = await client.PostAsync("/api/CrossingInfo/"+crossInfoId+"/Document", content);

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        
        var query = new Dictionary<string, string?>
        {
            ["id"] = "1",
            ["tollId"] = "2"
        };
        var result = await client.PatchAsync(QueryHelpers.AddQueryString("/api/CrossingInfo/", query), JsonContent.Create(DateTime.Now.AddDays(1).Iso8601()));
        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        //Leslie scan un document qui est valide
        crossInfo = new CrossingInfo(new TollOffice("fr"));
        resp = await client.PostAsync("/api/CrossingInfo", JsonContent.Create(crossInfo));
        crossInfoResp = await resp.Content.ReadFromJsonAsync<CrossingInfo>();
        crossInfoId = crossInfoResp?.Id;
        
        imgBytes = await File.ReadAllBytesAsync("../../../Image/declaration_douane.png");
        content = new MultipartFormDataContent { { new ByteArrayContent(imgBytes), "file", "image.jpg" } };
        res = await client.PostAsync("/api/CrossingInfo/"+crossInfoId+"/Document", content);

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        
        query = new Dictionary<string, string?>
        {
            ["id"] = "1",
            ["tollId"] = "2"
        };
        //Leslie signale une anomalie
        var anomaliesDesc = new[] { "coin coin", "42", "GRRRR" };
        var anomalies = new DocumentController.AnomaliesBody(anomaliesDesc);
        res = await client.PostAsync("/api/Document/1/Non-compliant", JsonContent.Create(anomalies));
     
        result = await client.PatchAsync(QueryHelpers.AddQueryString("/api/CrossingInfo/", query), JsonContent.Create(DateTime.Now.AddDays(1).Iso8601()));

        Assert.Equal(HttpStatusCode.Forbidden, result.StatusCode);
        
        
        //todo Sebastian gère les anomalies
    }

    [Fact]
    public async Task S3()
    {
        //todo plein de gens passent
        //todo Waze filtre les flux
    }
}