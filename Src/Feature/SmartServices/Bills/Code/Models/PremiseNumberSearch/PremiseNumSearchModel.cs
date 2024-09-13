// <copyright file="PremiseNumSearchModel.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\mayur.prajapati</author>

using DEWAXP.Foundation.Content.Models.Base;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Models.PremiseNumberSearch
{
    [Serializable]
    /// <summary>
    /// Defines the <see cref="PremiseNumSearchModel" />.
    /// </summary>
    public class PremiseNumSearchModel
    {
        public List<PremiseNumSearchOptions> SearchOptions { get; set; }
        public string selectedSearchType { get; set; }
        public string SearchText { get; set; }
        public string gisxyValue { get; set; }
        public bool IsLoggedIn { get; set; }
        public string resDescription { get; set; }
        public List<PremiseNumSearchResult> PremiseNumSearchResultList { get; set; }
        public List<PremiseNumSearchResult> PremiseFilterSeachResultList { get; set; }
        public PaginationModel PaginationInfo { get; set; }
        public int currentPage { get; set; }
        public string cachedKey { get; set; }
        public PremiseNumSearchCountData columnsCount { get; set; }
    }

    public class PremiseNumSearchResult
    {
        public string BusinessPartner { get; set; }
        public string CommunityName { get; set; }
        public string ContractAccount { get; set; }
        public string ElectricityApplication { get; set; }
        public string GisX { get; set; }
        public string GisY { get; set; }
        public string LegacyNumber { get; set; }
        public string Load { get; set; }
        public string MakaniNumber { get; set; }
        public string Name { get; set; }
        public string PremiseType { get; set; }
        public string PtypeText { get; set; }
        public string RoomNumber { get; set; }
        public string UnitNumber { get; set; }
        public string WaterApplication { get; set; }
    }

    public class PremiseNumSearchCountData
    {
        public int BusinessPartnerCount { get; set; }
        public int CommunityNameCount { get; set; }
        public int ContractAccountCount { get; set; }
        public int ElectricityApplicationCount { get; set; }
        public int GisXCount { get; set; }
        public int GisYCount { get; set; }
        public int LegacyNumberCount { get; set; }
        public int LoadCount { get; set; }
        public int MakaniNumberCount { get; set; }
        public int NameCount { get; set; }
        public int PremiseTypeCount { get; set; }
        public int PtypeTextCount { get; set; }
        public int RoomNumberCount { get; set; }
        public int UnitNumberCount { get; set; }
        public int WaterApplicationCount { get; set; }
    }

    public class PremiseNumSearchOptions
    {
        public string key { get; set; }
        public string value { get; set; }
        public string dummy { get; set; }
    }
}
