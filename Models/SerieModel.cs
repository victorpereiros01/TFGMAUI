﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Spi;

namespace TFGMaui.Models
{
    public class SeriesModel : HobbieModel
    {
        public bool Adult { get; set; }

        public string BackdropPath { get; set; }

        public List<object> CreatedBy { get; set; }

        public List<int> EpisodeRunTime { get; set; }

        public string OriginalLanguage { get; set; }

        public string FirstAirDate { get; set; }

        public List<GenreM> Genres { get; set; }

        public int Id { get; set; }

        public bool InProduction { get; set; }

        public string Name { get; set; }

        public List<string> OriginCountry { get; set; }

        public int NumberOfEpisodes { get; set; }

        public int NumberOfSeasons { get; set; }

        public string Status { get; set; }

        public string Tagline { get; set; }

        public string Type { get; set; }

        public double VoteAverage { get; set; }

        public List<ProductionCompany> ProductionCompanies { get; set; }

        public List<Season> Seasons { get; set; }

        public string Overview { get; set; }

        public double Popularity { get; set; }

        public string PosterPath { get; set; }
    }

    public class Season
    {
        public DateTime? AirDate { get; set; }

        public int EpisodeCount { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Overview { get; set; }

        public string PosterPath { get; set; }

        public int SeasonNumber { get; set; }
    }

    public class Episode
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Overview { get; set; }

        public DateTime? AirDate { get; set; }

        public int EpisodeNumber { get; set; }

        public int SeasonNumber { get; set; }

        public string StillPath { get; set; }

        public double VoteAverage { get; set; }

        public int VoteCount { get; set; }
    }
}