//--------------------------------------------------------------
// <copyright file="FxStreetRequester.cs" company="Nautech Systems Pty Ltd.">
//   Copyright (C) 2015-2018 Nautech Systems Pty Ltd. All rights reserved.
//   The use of this source code is governed by the license as found in the LICENSE.txt file.
//   http://www.nautechsystems.net
// </copyright>
//--------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NautilusDB.FxStreet
{
    public sealed class FxStreetRequester : IEconomicNewsEventCollector
    {
        public QueryResult<IReadOnlyCollection<EconomicNewsEvent>> GetAllEvents()
        {
            var rawData = this.RequestRawDataAsync().Result;

            if (string.IsNullOrWhiteSpace(rawData))
            {
                return QueryResult<IReadOnlyCollection<EconomicNewsEvent>>.Fail("Cannot get data");
            }

            var formedData = this.ParseEconomicEventsAsync(rawData).Result;

            return formedData.Count == 0
                 ? QueryResult<IReadOnlyCollection<EconomicNewsEvent>>.Fail("No economic news events")
                 : QueryResult<IReadOnlyCollection<EconomicNewsEvent>>.Ok(formedData);
        }

        public QueryResult<IReadOnlyCollection<EconomicNewsEvent>> GetEvents(ZonedDateTime fromDateTime)
        {
            throw new System.NotImplementedException();
        }

        private async Task<string> RequestRawDataAsync()
        {
            using (var client = new WebClient())
            {
                client.Headers.Add("Referer", "http://www.fxstreet.com/fundamental/economic-calendar/");
                client.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.3");
                client.Encoding = Encoding.UTF8;

                var volatility = 1;
                var queryString =
                    string.Format(
                        "http://calendar.fxstreet.com/eventdate/csv?timezone=SE+Asia+Standard+Time&rows=0&view=current&countrycode=AU%2CCA%2CCN%2CEMU%2CDE%2CFR%2CDE%2CGR%2CIT%2CJP%2CNZ%2CPT%2CES%2CCH%2CUK%2CUS&volatility={0}&culture=en-US&columns=CountryCurrency",
                        volatility);

                return await client
                           .DownloadStringTaskAsync(queryString)
                           .ConfigureAwait(false);
            }
        }

        private async Task<IReadOnlyCollection<EconomicNewsEvent>> ParseEconomicEventsAsync(string rawData)
        {
            var reader = new StringReader(rawData);
            var results = new List<EconomicNewsEvent>();
            var line = string.Empty;

            while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                var columns = line.Split(',');
                // be careful about not available data, n/a tentative etc etc
                try
                {
                    results.Add(
                        new EconomicNewsEvent(
                            ParseDateTime(columns[0].Trim('"')),
                            columns[1].Trim('"'),
                            columns[2].Trim('"').ToEnum<Country>(),
                            FxStreetCurrencyRegistry.ForCountry(columns[2].Trim('"')),
                            columns[3].Trim('"').ToEnum<NewsImpact>(),
                            SafeConvert.ToDecimal(columns[4].Trim('"')),
                            SafeConvert.ToDecimal(columns[5].Trim('"')),
                            SafeConvert.ToDecimal(columns[6].Trim('"'))));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return results;
        }

        private static ZonedDateTime ParseDateTime(string date)
        {
            var parts = date.Split(' ');
            var datePart = parts[0];
            var timePart = parts[1];

            var year = Convert.ToInt32(datePart.Substring(0, 4));
            var month = Convert.ToInt32(datePart.Substring(4, 2));
            var day = Convert.ToInt32(datePart.Substring(6, 2));

            var time = DateTime.Parse(timePart);

            return new ZonedDateTime(new LocalDateTime(year, month, day, time.Hour, time.Minute, time.Second), DateTimeZone.Utc, Offset.Zero);
        }
    }
}