using CesiumLanguageWriter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CzmlWriterExample.Controllers
{
    public class CzmlController : ApiController
    {
        public HttpResponseMessage Get()
        {
            StringWriter sw = new StringWriter();

            CesiumOutputStream output = new CesiumOutputStream(sw);
            CesiumStreamWriter czmlWriter = new CesiumStreamWriter();

            output.WriteStartSequence();

            using (var entity = czmlWriter.OpenPacket(output))
            {
                entity.WriteId("document");
                entity.WriteVersion("1.0");
            }

            for (int lat = 0; lat < 60; lat = lat + 5)
            {
                for (int lon = 0; lon < 100; lon = lon + 5)
                {
                    using (var entity = czmlWriter.OpenPacket(output))
                    {
                        entity.WriteId("facility_" + lat.ToString() + "_" + lon.ToString());
                        entity.WriteName("facility_" + lat.ToString() + "_" + lon.ToString());
                        entity.WritePositionPropertyCartographicDegrees(new Cartographic(lon, lat, 100));

                        using (var billboard = entity.OpenBillboardProperty())
                        {
                            Uri uri = new Uri("http://localhost:64225/Data/Facility.png");
                            CesiumResource resource = new CesiumResource(uri, CesiumResourceBehavior.LinkTo);
                            billboard.WriteImageProperty(resource);
                        }

                        using (var label = entity.OpenLabelProperty())
                        {
                            label.WriteTextProperty("facility_" + lat.ToString() + "_" + lon.ToString());
                            label.WriteHorizontalOriginProperty(CesiumHorizontalOrigin.Left);
                            label.WriteVerticalOriginProperty(CesiumVerticalOrigin.Bottom);
                        }

                        using (var model = entity.OpenModelProperty())
                        {
                            model.WriteGltfProperty(new CesiumResource(
                                new Uri("http://localhost:64225/Data/Cesium_Man.bgltf"),
                                CesiumResourceBehavior.LinkTo));
                            model.WriteScaleProperty(500000);
                        }
                    }
                }
            }

            output.WriteEndSequence();

            var response = Request.CreateResponse();
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent(sw.ToString(), System.Text.Encoding.UTF8, "application/json");

            return response;
        }
    }
}
